using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class UserTableSetting
	{
		public void Initialize(string szParam)
		{
			if (_table != null)
				return;

			int nKeyEnd = szParam.IndexOf("/");
			_key = szParam.Substring(0, nKeyEnd);

			_name = szParam.Substring(nKeyEnd + 1);
			_address = string.Format("{0}/{1}", HTFramework.Instance.RegistGEnv._tableFileFolder, _name);

			_table = new HTTable();
			_table.LoadFromBin(_address, true);
		}

		public string _key = null;
		public string _name = null;
		public string _address = null;
		public HTTable _table = null;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public class HTFramework : MonoBehaviour
	{
		//---------------------------------------
		/// <summary>
		/// Scene 전환이 완료된 시점 이후에 호출됩니다.
		/// 이 시점에서 생성되는 GameObject들은 전환 된 Scene에 귀속됩니다.
		/// </summary>
		public static Action onSceneChange = null;

		/// <summary>
		/// Client 실행 후 처음 Update 함수가 진행되면 호출됩니다.
		/// </summary>
		public static Action onFirstUpdateInitialize = null;

		//---------------------------------------
		private static List<HTComponent> _registedComponentList = new List<HTComponent>();
		private static List<IHTAutoUpdater> _autoUpdaters = new List<IHTAutoUpdater>();

		private static HTFramework _instance = null;
		public static HTFramework Instance
		{
			get
			{
				if (_instance == null)
				{
					string szPrefabAddr = GEnv.HTPrefabFolder + GEnv.Prefab_HTFramework;
					GameObject pObject = Utils.Instantiate(szPrefabAddr) as GameObject;
					if (pObject != null)
						_instance = pObject.GetComponent<HTFramework>();
				}

				return _instance;
			}
		}

		public static HTFramework Instanced
		{
			get { return _instance; }
		}

		//---------------------------------------
		static bool _gameClosed = false;
		static public bool GameClosed
		{
			get { return _gameClosed; }
			private set
			{
				_gameClosed = value;
			}
		}

		//---------------------------------------
		[Header("INSTANCES")]
		[SerializeField]
		private Canvas _defaultCanvas = null;
		public Canvas DefaultCanvas { get { return _defaultCanvas; } }

		//---------------------------------------
		private GEnv _registGEnv = null;
		public GEnv RegistGEnv { get { return _registGEnv; } }

		//---------------------------------------
		private Coroutine _proc_SceneChange = null;
		private bool _initialized = false;
		private bool _initializedComplete = false;

		//---------------------------------------
		private Mutex _mutex = null;
		private int _lastGCFrame = 0;
		private static float[] _samplingedDeltas = new float[30];

		private bool _firstUpdateInitialized = false;

		//---------------------------------------
		private static ulong _afxCycleFrame = 0;
		public static ulong AfxCycleFrame { get { return _afxCycleFrame; } }

		private static int _mainThreadID = 0;
		public static int MainThreadID { get { return _mainThreadID; } }

		private static Camera _lastCamera = null;
		public static Camera LastCamera { get { return _lastCamera; } }

		//---------------------------------------
		private static uint _lastEntityUniqueID = 0;
		public static uint CreateEntityUniqueID()
		{
			return _lastEntityUniqueID++;
		}

		private static List<HTEntity> _htEntities = new List<HTEntity>();
		private static List<HTNetworkEntity> _htNetEntities = new List<HTNetworkEntity>();

		public static void AddInstantiatedHTEntities(HTEntity pEntity)
		{
			_htEntities.Add(pEntity);

			HTNetworkEntity pNetEntity = pEntity as HTNetworkEntity;
			if (pNetEntity != null)
				_htNetEntities.Add(pNetEntity);
		}

		public static void RemoveInstantiatedHTEntities(HTEntity pEntity)
		{
			_htEntities.Remove(pEntity);

			HTNetworkEntity pNetEntity = pEntity as HTNetworkEntity;
			if (pNetEntity != null)
				_htNetEntities.Remove(pNetEntity);
		}

		public static void RemoveInstantiatedHTNetEntities(uint nUniqueID)
		{
			HTNetworkEntity pNetEntity = GetHTNetEntity(nUniqueID);
			_htEntities.Remove(pNetEntity);

			if (pNetEntity != null)
				_htNetEntities.Remove(pNetEntity);
		}

		public static HTNetworkEntity GetHTNetEntity(uint nUniqueID)
		{
			for (int nInd = 0; nInd < _htNetEntities.Count; ++nInd)
				if (_htNetEntities[nInd].EntityUniqueID == nUniqueID)
					return _htNetEntities[nInd];

			HTDebug.PrintLog(eMessageType.Warning, string.Format("[HTFramework] Failed to found network entity of id {0}.", nUniqueID));
			return null;
		}

		//---------------------------------------
		private void Awake()
		{
			_instance = this;
			DontDestroyOnLoad(_instance);
		}

		private void Start()
		{
			if (_registGEnv._mobileIsNeverSleep)
				Screen.sleepTimeout = SleepTimeout.NeverSleep;

			// Resolution Change
			if (HTAfxPref.IsMobilePlatform)
			{
				if (_registGEnv._resolutionFix_Mobile == eScreenResolution.Fixed)
					Screen.SetResolution((int)_registGEnv._resolutionMobile.x, (int)_registGEnv._resolutionMobile.y, true);
			}
			else
			{
				if (_registGEnv._resolutionFix_PC == eScreenResolution.Fixed)
					Screen.SetResolution((int)_registGEnv._resolutionPC.x, (int)_registGEnv._resolutionPC.y, _registGEnv._resolutionPC_FullScreen);
			}
		}

		//---------------------------------------
		private void OnApplicationPause(bool pause)
		{
			SingletonContainer.OnApplicationPause(pause);

			//-----
			if (pause == false && _registGEnv._timeUseOnlyServerTime)
				TimeUtils.TimeSyncRefreshFromServerTime(null);

			//-----
			HTAfxPref.SaveAfxPref();
		}

		private void OnApplicationQuit()
		{
			HTAfxPref.SaveAfxPref();
			GameClosed = true;
		}

		void OnDestroy()
		{
			if (_mutex != null)
				_mutex.ReleaseMutex();

			//-----
			for (int nInd = 0; nInd < _registedComponentList.Count; ++nInd)
			{
				_registedComponentList[nInd].OnDestroy();
				_registedComponentList[nInd] = null;
			}

			_registedComponentList.Clear();

			//-----
			SingletonContainer.Release();

			//-----
			_instance = null;
		}

		//---------------------------------------
		public void Initialize(bool bAsync, GEnv pProjDfn, Action<float> pProcCallback, Action pEndCallback)
		{
			if (_initialized)
			{
				Utils.SafeInvoke(pEndCallback);
				return;
			}

			//-----
			_mainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;

			//-----
#if ENABLE_DEBUG || UNITY_EDITOR
			Application.logMessageReceived += HTDebug.OnLogMessageRecived;
#endif // ENABLE_DEBUG || UNITY_EDITOR

			//-----
			_registGEnv = pProjDfn;
			if (_registGEnv == null)
				HTDebug.PrintLog(eMessageType.Error, "[HTAfx] UserDefine has not registed!");

			//-----
#if UNITY_ANDROID || UNITY_IOS
#else // UNITY_ANDROID || UNITY_IOS
			if (_registGEnv._preventMultiClient)
			{
				bool bIsNewMutex = false;
				_mutex = new Mutex(true, Application.productName, out bIsNewMutex);
				if (bIsNewMutex == false)
				{
					HTDebug.PrintLog(eMessageType.Error, "[HTAfx] Multi client is preventing!");
					Application.Quit();
				}
			}

#endif // UNITY_ANDROID || UNITY_IOS

			//-----
			int nFrameRate = int.MaxValue;
			switch (HTAfxPref.Platform)
			{
				case eOS.Android:
					nFrameRate = _registGEnv._frameLimit_AOS;
					break;
				case eOS.IOS:
					nFrameRate = _registGEnv._frameLimit_IOS;
					break;
				case eOS.PC:
					nFrameRate = _registGEnv._frameLimit_PC;
					break;
			}

			Application.targetFrameRate = nFrameRate;

			//-----
			StartCoroutine(InitializeAsyc_Internal(bAsync, pProcCallback, pEndCallback));
		}

		IEnumerator InitializeAsyc_Internal(bool bAsync, Action<float> pProcCallback, Action pEndCallback)
		{
			_initialized = true;

			// Addon Components Initialize
			MathUtils.Init();
			HTSoundManager.Init();

			// Add HT Components
			_registedComponentList.Add(new HTTableManager());
			_registedComponentList.Add(new HTInputManager());
			_registedComponentList.Add(new HTUIManager());

			//-----
			Utils.SafeInvoke(pProcCallback, 0.0f);
			yield return new WaitForEndOfFrame();

			int nTotalProcCount = _registedComponentList.Count;

			//-----
			for (int nInd = 0; nInd < _registedComponentList.Count; ++nInd)
			{
				_registedComponentList[nInd].Initialize();

				if (bAsync)
				{
					Utils.SafeInvoke(pProcCallback, (float)nInd / nTotalProcCount);
					yield return new WaitForEndOfFrame();
				}
			}

			//-----
			HTAfxPref.LoadAfxPref();

			if (HTDataMD5Keys.PARAM_USE_DATAPURITY)
			{
				bool bFailedChecks = true;
				do
				{
#if UNITY_EDITOR
#else // UNITY_EDITOR
					string szModuleAddr = string.Format("{0}/../GameAssembly.dll", Application.dataPath);
					if (CryptUtils.CheckCodeSignatureCertificatePurity(szModuleAddr) == false)
						break;
#endif // UNITY_EDITOR

					bool bIsPurify = true;
					HTTableManager pManager = GetRegistedComponent<HTTableManager>();
					UserTableSetting[] vTables = pManager.UserTables;
					for (int nInd = 0; nInd < vTables.Length; ++nInd)
					{
						string szKey = CryptUtils.DecryptString(HTDataMD5Keys.FindMD5(vTables[nInd]._name));
						if (szKey != CryptUtils.GetResourceHashMD5(vTables[nInd]._address))
						{
							bIsPurify = false;
							break;
						}
					}

					if (bIsPurify == false)
						break;

					bFailedChecks = false;
				}
				while (false);

				if (bFailedChecks)
				{
					GameShutdown();
					while (true)
						yield return new WaitForEndOfFrame();
				}
			}

			//-----
			_initializedComplete = true;

			Utils.SafeInvoke(pEndCallback);
		}

		void Update()
		{
			++_afxCycleFrame;
			if (_afxCycleFrame >= ulong.MaxValue)
			{
				HTDebug.PrintLog(eMessageType.None, "[HTAfx] Cycle frame index reset to 0");
				_afxCycleFrame = 0;
			}

			//-----
			Camera pCurCam = Camera.main;
			if (pCurCam != null)
				_lastCamera = pCurCam;

			//-----
			if (_initializedComplete == false)
				return;

			if (_firstUpdateInitialized == false)
			{
				_firstUpdateInitialized = true;
				Utils.SafeInvoke(onFirstUpdateInitialize);

				//-----
				HTAfxPref.RefreshQualityLevel();
			}

			//-----
			float fDelta = TimeUtils.GameTime;
			for (int nInd = 0; nInd < _registedComponentList.Count; ++nInd)
				_registedComponentList[nInd].Tick(fDelta);

			SingletonContainer.Update(fDelta);

			if (_autoUpdaters.Count > 0)
			{
				for (int nInd = 0; nInd < _autoUpdaters.Count; ++nInd)
					_autoUpdaters[nInd].Update(fDelta);

				for (int nInd = _autoUpdaters.Count - 1; nInd >= 0; --nInd)
					if (_autoUpdaters[nInd].ReadyToDestroy)
						_autoUpdaters.RemoveAt(nInd);
			}

			//-----
			++_lastGCFrame;
			for (int nInd = _samplingedDeltas.Length - 1; nInd >= 1; --nInd)
				_samplingedDeltas[nInd] = _samplingedDeltas[nInd - 1];
			_samplingedDeltas[0] = fDelta;
			
			if (_lastGCFrame > _registGEnv._autoGCWhenFrame_Min)
			{
				float fPeakTime = Mathf.Max(_samplingedDeltas);
				if (fPeakTime - _registGEnv._autoGCWhenDeltaTime >= fDelta || _registGEnv._autoGCWhenFrame_Max > _lastGCFrame)
				{
					GC.Collect();
					_lastGCFrame = 0;
				}
			}

			//-----
#if ENABLE_DEBUG || UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.BackQuote))
			{
				if (HTConsoleWindow.IsOpened)
					HTConsoleWindow.CloseWindow();
				else
					HTConsoleWindow.OpenWindow();
			}
#endif // ENABLE_DEBUG || UNITY_EDITOR

			//-----
			Utils.InvokeAll();

			HTDebug.UpdateDebug();
			StopWatch.UpdateAllStopWatch();
		}

		void FixedUpdate()
		{
			if (_initializedComplete == false)
				return;

			//-----
			float fDelta = TimeUtils.GameTime;
			for (int nInd = 0; nInd < _registedComponentList.Count; ++nInd)
				_registedComponentList[nInd].FixedTick(fDelta);

			//-----
			if (UnityEngine.EventSystems.EventSystem.current != null)
			{
				int nCentimeter = (int)(_registGEnv._ui_dragThresholdDist * Screen.dpi / 2.54f);
				UnityEngine.EventSystems.EventSystem.current.pixelDragThreshold = nCentimeter;
			}

			//-----
			SingletonContainer.FixedUpdate(fDelta);
		}

		public void GameShutdown()
		{
			GameClosed = true;
			Application.Quit();

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif // UNITY_EDITOR
		}

		public T GetRegistedComponent<T>() where T : class
		{
			for (int nInd = 0; nInd < _registedComponentList.Count; ++nInd)
			{
				T pCast = _registedComponentList[nInd] as T;
				if (pCast != null)
					return pCast;
			}

			return default(T);
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// Scene을 Change합니다.
		/// Fade Out -> Scene Change -> Fade In 의 과정으로 작동됩니다.
		/// </summary>
		public void SceneChange(string szSceneName, float fFadeOutTime = GEnv.GLOBALUI_DEFAULT_FADETIME, float fFadeInTime = 0.5f)
		{
			HTStartSceneHelper.RecordFirstSceneName();

			//-----
			Utils.SafeStopCorutine(this, ref _proc_SceneChange);
			_proc_SceneChange = StartCoroutine(SceneChange_Internal(szSceneName, fFadeOutTime, fFadeInTime));
		}

		private IEnumerator SceneChange_Internal(string szSceneName, float fFadeOutTime, float fFadeInTime)
		{
			HTAfxPref.WaitingForSceneChange = true;
			HTGlobalUI.Instance.MaskFade(false, fFadeOutTime);

			yield return new WaitForSecondsRealtime(fFadeOutTime + 0.1f);

			Utils.SafeInvoke(onSceneChange);

			HTAfxPref.WaitingForSceneChange = false;
			UnityEngine.SceneManagement.SceneManager.LoadScene(szSceneName);

			HTGlobalUI.Instance.MaskFade(true, fFadeInTime);
		}

		//---------------------------------------
		/// <summary>
		/// Scene을 Aync Load 한 뒤 Change합니다.
		/// Scene Async Load -> Fade Out -> Scene Change -> Fade In 의 과정으로 작동됩니다.
		/// </summary>
		public void SceneChangeAsync(string szSceneName, Action<float> onChangeProgress, Action pOnSceneChange = null, Action<Action> pOnLoadCompleteAndChange = null, float fFadeInTime = 0.5f)
		{
			HTStartSceneHelper.RecordFirstSceneName();

			//-----
			Utils.SafeStopCorutine(this, ref _proc_SceneChange);
			_proc_SceneChange = StartCoroutine(SceneChangeAsync_Internal(szSceneName, onChangeProgress, pOnSceneChange, pOnLoadCompleteAndChange, fFadeInTime));
		}

		private IEnumerator SceneChangeAsync_Internal(string szSceneName, Action<float> onChangeProgress, Action pOnSceneChange, Action<Action> pOnLoadCompleteAndChange, float fFadeInTime)
		{
			HTGlobalUI pGlobalUI = HTGlobalUI.Instance;

			//-----
			HTAfxPref.WaitingForSceneChange = true;

			AsyncOperation pAsyncProc = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(szSceneName);
			pAsyncProc.allowSceneActivation = false;

			while (pAsyncProc.isDone == false)
			{
				Utils.SafeInvoke(onChangeProgress, pAsyncProc.progress);
				if (pAsyncProc.progress >= GEnv.UNITY_ALLOWSCENEACTIVATION_WAITPROG)
					break;

				yield return new WaitForEndOfFrame();
			}

			//-----
			Utils.SafeInvoke(onChangeProgress, 1.0f);

			//-----
			if (pOnLoadCompleteAndChange != null)
			{
				bool bWaitEvent = true;
				Utils.SafeInvoke(pOnLoadCompleteAndChange, () =>
				{
					bWaitEvent = false;
				});

				while (bWaitEvent)
					yield return new WaitForEndOfFrame();
			}

			//-----
			pGlobalUI.MaskFade(false, 1.0f);
			yield return new WaitForSecondsRealtime(1.0f);

			//-----
			pAsyncProc.allowSceneActivation = true;

			while(pAsyncProc.isDone == false)
				yield return new WaitForEndOfFrame();

			yield return new WaitForEndOfFrame();

			Utils.SafeInvoke(pOnSceneChange);
			Utils.SafeInvoke(onSceneChange);

			yield return new WaitForEndOfFrame();

			//-----
			HTAfxPref.WaitingForSceneChange = false;
			pGlobalUI.MaskFade(true, fFadeInTime);
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// 현재 Call Stack이 Main Thread에서 호출 된 것인지 확인합니다.
		/// </summary>
		public bool IsMainThead()
		{
			if (System.Threading.Thread.CurrentThread.ManagedThreadId == _mainThreadID)
				return true;

			return false;
		}


		/////////////////////////////////////////
		//---------------------------------------
		public void AddAutoUpdater(IHTAutoUpdater pUpdater)
		{
			_autoUpdaters.Add(pUpdater);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
}
