using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// HTFramework의 초기화를 진행해주는 Behaviour입니다.
	/// Game의 첫 번째 Scene에만 등록되어있으면 됩니다.
	/// Game Initializer에서 상속받으면 Load Frag를 사용해서 초기화 작업을 추가 할 수 있습니다.
	/// </summary>
	public class HTInitializer_Async : MonoBehaviour
	{
		//---------------------------------------
		public static Action onSceneLoadComplete = null;
		public static Action onSceneChangeCaller = null;

		//---------------------------------------
		[SerializeField, FormerlySerializedAs("_projectDefine")]
		private GEnv _gEnv = null;
		public static GEnv initGEnv = null;

		[SerializeField]
		private string _nextScene = null;

		[SerializeField]
		private bool _autoStart = true;
		[SerializeField]
		private bool _autoSceneChange = true;

		//---------------------------------------
		private List<ILoadFrag> _loadFrags = new List<ILoadFrag>();
		private bool _initializeStarted = false;
		
		//---------------------------------------
		private void Awake()
		{
			initGEnv = _gEnv;

			//-----
			RegistLoadFragments(new LoadFrag_HTFramework());

			OnAwake();

			RegistLoadFragments(new LoadFrag_SceneChange(_nextScene, _autoSceneChange));

			//-----
			if (_autoStart)
				InitializeStart();
		}

		public void InitializeStart()
		{
			if (_initializeStarted)
				return;

			_initializeStarted = true;
			StartCoroutine(Initialize_Internal());
		}

		private IEnumerator Initialize_Internal()
		{
			OnChangeLoadProgress(0.0f);
			yield return new WaitForEndOfFrame();

			float fTotalProc = 0.0f;
			for (int nInd = 0; nInd < _loadFrags.Count; ++nInd)
			{
				bool bWaitForProc = true;

				_loadFrags[nInd].OnRegistProgressess();
				StartCoroutine(_loadFrags[nInd].Load_Internal((float fProc) =>
				{
					float fPerFrags = 1.0f / _loadFrags.Count;
					fTotalProc = (nInd * fPerFrags) + (fPerFrags * fProc);
				}, () =>
				{
					bWaitForProc = false;
				}));

				//-----
				while (bWaitForProc)
				{
					OnChangeLoadProgress(fTotalProc);
					yield return new WaitForEndOfFrame();
				}
			}

			//-----
			OnChangeLoadProgress(1.0f);
			OnLoadComplete();
		}

		private void OnDestroy()
		{
		}

		//---------------------------------------
		public void RegistLoadFragments(ILoadFrag pFrag)
		{
			_loadFrags.Add(pFrag);
		}

		//---------------------------------------
		protected virtual void OnAwake()
		{

		}

		protected virtual void OnChangeLoadProgress(float fProc)
		{

		}

		protected virtual void OnLoadComplete()
		{

		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	public interface ILoadFrag
	{
		void OnRegistProgressess();
		IEnumerator Load_Internal(Action<float> onProc, Action onComplete);
	}

	/// <summary>
	/// 단순한 함수만 차례대로 호출하면 되는 처리를 할 떄 사용됩니다.
	/// OnRegistProgressess에서 Callback을 모두 등록하면,
	/// LoadInternal에서 자동적으로 순차 호출되면서 Load Progress가 갱신됩니다.
	/// </summary>
	public class ILoadFrag_Simple : ILoadFrag
	{
		//---------------------------------------
		private List<Action> _progressList = new List<Action>();
		protected void RegistProgress(Action pAction)
		{
			_progressList.Add(pAction);
		}

		public virtual void OnRegistProgressess()
		{

		}

		//---------------------------------------
		public IEnumerator Load_Internal(Action<float> onProc, Action onComplete)
		{
			for(int nInd = 0; nInd < _progressList.Count; ++nInd)
			{
				Utils.SafeInvoke(_progressList[nInd]);

				//-----
				Utils.SafeInvoke(onProc, (float)nInd / (float)_progressList.Count);
				yield return new WaitForEndOfFrame();
			}

			Utils.SafeInvoke(onComplete);
		}

		//---------------------------------------
	}

	/// <summary>
	/// Load Progress를 직접 관리하려 할 때 사용됩니다.
	/// </summary>
	public class ILoadFrag_Custom : ILoadFrag
	{
		//---------------------------------------
		public void OnRegistProgressess()
		{

		}

		public virtual IEnumerator Load_Internal(Action<float> onProc, Action onComplete)
		{
			yield break;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	public sealed class LoadFrag_HTFramework : ILoadFrag_Custom
	{
		public override IEnumerator Load_Internal(Action<float> onProc, Action onComplete)
		{
			bool bComplete = false;
			HTFramework.Instance.Initialize(true, HTInitializer_Async.initGEnv, (float fProc) =>
			{
				Utils.SafeInvoke(onProc, fProc);
			}, () =>
			{
				bComplete = true;
			});
			HTGlobalUI.Instance.MaskFade(true);

			while (bComplete == false)
				yield return new WaitForEndOfFrame();

			Utils.SafeInvoke(onComplete);
		}
	}

	//---------------------------------------
	public sealed class LoadFrag_SceneChange : ILoadFrag_Custom
	{
		private string _nextSceneName = null;
		private bool _autoSceneChange = true;
		public LoadFrag_SceneChange(string szNextSceneName, bool bAutoSceneChange)
		{
			_nextSceneName = szNextSceneName;
			_autoSceneChange = bAutoSceneChange;
		}

		public override IEnumerator Load_Internal(Action<float> onProc, Action onComplete)
		{
			bool bComplete = false;

			//-----
			Action<Action> pOnWaitEvent = null;
			if (_autoSceneChange == false)
			{
				pOnWaitEvent = (Action pStartSceneChange) =>
				{
					Utils.SafeInvoke(HTInitializer_Async.onSceneLoadComplete);
					HTInitializer_Async.onSceneChangeCaller = ()=> 
					{
						Utils.SafeInvoke(pStartSceneChange);
						HTInitializer_Async.onSceneChangeCaller = null;
					};
				};
			}

			HTFramework.Instance.SceneChangeAsync(_nextSceneName, (float fProc) => 
			{
				Utils.SafeInvoke(onProc, fProc);
				if (fProc >= 1.0f)
				{
					bComplete = true;
				}
			}, null, pOnWaitEvent);
			
			while (bComplete == false)
				yield return new WaitForEndOfFrame();

			//-----
			Utils.SafeInvoke(onComplete);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
}
