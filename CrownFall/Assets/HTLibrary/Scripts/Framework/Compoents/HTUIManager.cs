using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// UI를 관리해주는 Manager입니다.
	/// Hardware button을 사용 한 Popup 종료와 UI 가리기 / 표시하기 기능을 지원합니다.
	/// </summary>
	public sealed class HTUIManager : HTComponent
	{
		//---------------------------------------
		private static HTUIManager _instance = null;
		public static HTUIManager Instance { get { return _instance; } }

		//---------------------------------------
		private static GEnv _gEnv = null;
		private static Canvas _uiHelperCanvas = null;
		private static bool _initialized = false;

		//---------------------------------------
		private static UIToolTip _toolTip = null;

        //---------------------------------------
        private static Action _onCloseBtnPressed = null;

		//---------------------------------------
		private static bool _enableHardwareButton = true;
		public static bool EnableHardwareButton
		{
			get { return _enableHardwareButton; }
			set { _enableHardwareButton = value; }
		}

		private static bool _allUIShowState = true;
		public static bool AllUIShowState { get { return _allUIShowState; } }

		//---------------------------------------
		private static List<UIPopup> _openedPopupList = new List<UIPopup>();
		public static List<UIPopup> OpenedPopupList { get { return _openedPopupList; } }
		public static int OpenedPopupCount { get { return _openedPopupList.Count; } }

		public static UIPopup GetLastUIPopup()
		{
			if (_openedPopupList.IsNullOrEmpty())
				return null;

			return _openedPopupList[_openedPopupList.Count - 1];
		}
		
		//---------------------------------------
		private static void SetInitialize()
		{
			if (_initialized)
				return;

			//-----
			_gEnv = HTFramework.Instance.RegistGEnv;
			
			//-----
			GameObject pCanvasObj = Utils.Instantiate(HTFramework.Instance.DefaultCanvas.gameObject);
			pCanvasObj.name = GEnv.HTPrefab_UIHelperName;
			GameObject.DontDestroyOnLoad(pCanvasObj);

			_uiHelperCanvas = pCanvasObj.GetComponent<Canvas>();
			_uiHelperCanvas.sortingOrder = HTGlobalUI.Instance.MainCanvas.sortingOrder - 1;

			//-----
			_uiHelperCanvas.gameObject.AddComponent<HTCanvasMatcher>();

			//-----
			GameObject pToolTipObj = Utils.Instantiate(GEnv.HTPrefabFolder + GEnv.Prefab_UIToolTip);
			pToolTipObj.SetActive(false);
			pToolTipObj.transform.SetParent(pCanvasObj.transform);
			pToolTipObj.transform.localScale = Vector3.one;

			_toolTip = pToolTipObj.GetComponent<UIToolTip>();

			//-----
			_initialized = true;
		}

		//---------------------------------------
		/// <summary>
		/// Resource 내의 해당 UIPopup을 표시합니다.
		/// 경로는 Project Define의 _uiPrefabFolder/ + szAddress로 가져옵니다.
		/// </summary>
		public static UIPopup OpenPopup(string szAddress)
		{
			SetInitialize();

			//-----
			UIPopup pPopup = OpenPopup_Instance(szAddress);
			if (pPopup == null)
				return null;

			pPopup.OpenPopup();
			return pPopup;
		}

		/// <summary>
		/// MessageBox Param 값을 기준으로 MessageBox를 표시합니다.
		/// 경로는 Project Define의 _uiPrefabFolder/ + _uiDefaultMessageBox 로 고정되어있습니다.
		/// </summary>
		public static UIPopup_MessageBox OpenMessageBox(UIPopup_MessageBoxParam pParam)
		{
			SetInitialize();

			//-----
			string szDefaultMsgBoxAddr = _gEnv._uiDefaultMessageBox;
			UIPopup pPopup = OpenPopup_Instance(szDefaultMsgBoxAddr);
			if (pPopup == null)
				return null;

			UIPopup_MessageBox pMsgBox = pPopup as UIPopup_MessageBox;
			if (pMsgBox == null)
			{
				HTDebug.PrintLog(eMessageType.Error, string.Format("[UIHelper] [{0}] is not Message Box!", szDefaultMsgBoxAddr));
				return null;
			}

			pMsgBox.Init(pParam);

			pMsgBox.OpenPopup();
			return pMsgBox;
		}

		public static UIPopup_MessageBox OpenMessageBox(eMessageBoxType eType, string szSubj, string szDesc, string szBtnName, Action pOnOK = null)
		{
			UIPopup_MessageBoxParam pParam = new UIPopup_MessageBoxParam();
			pParam.Init(eType, szSubj, szDesc, szBtnName, pOnOK);

			return OpenMessageBox(pParam);
		}

		private static UIPopup OpenPopup_Instance(string szAddress)
		{
			SetInitialize();

			//-----
			string szPrefabAddr = string.Format("{0}/{1}", _gEnv._uiPrefabFolder, szAddress);
			GameObject pUIObject = Utils.InstantiateFromPool(szPrefabAddr);
			if (pUIObject == null)
			{
				HTDebug.PrintLog(eMessageType.Error, string.Format("[UIHelper] Failed to open UI from [{0}]", szPrefabAddr));
				return null;
			}

			UIPopup pPopup = pUIObject.GetComponent<UIPopup>();
			if (pPopup == null)
			{
				HTDebug.PrintLog(eMessageType.Error, string.Format("[UIHelper] [{0}] is not UIPopup prefab!", szPrefabAddr));
				return null;
			}

			//-----
			pPopup.transform.SetParent(_uiHelperCanvas.transform);
			return pPopup;
		}

		/// <summary>
		/// 대상 UIPopup을 닫습니다.
		/// </summary>
		public static bool ClosePopup(UIPopup pPopup)
		{
			if (pPopup == null)
				return false;
			
			pPopup.ClosePopup();
			return true;
		}

		/// <summary>
		/// 현재 표시 되고 있는 모든 UIPopup을 종료합니다.
		/// </summary>
		public static void CloseAllPopup()
		{
			if (_openedPopupList.Count > 0)
			{
				for (int nInd = 0; nInd < _openedPopupList.Count; ++nInd)
					_openedPopupList[nInd].ClosePopup();

				_openedPopupList.Clear();
			}
		}

		//---------------------------------------
		/// <summary>
		/// 열려있는 UIPopup 목록에 해당 UIPopup을 추가합니다.
		/// 직접 호출하는 것은 권장되지 않습니다.
		/// </summary>
		public static void AddToOpenQueue(UIPopup pPopup)
		{
			_openedPopupList.Add(pPopup);
		}

		/// <summary>
		/// 열려있는 UIPopup 목록에서 해당 UIPopup을 제거합니다.
		/// 직접 호출하는 것은 권장되지 않습니다.
		/// </summary>
		public static void RemoveOnOpenQueue(UIPopup pPopup)
		{
			_openedPopupList.Remove(pPopup);
		}

		private static void OnSceneChange()
		{
			for (int nInd = 0; nInd < _openedPopupList.Count; ++nInd)
				_openedPopupList[nInd].ClosePopup();

			_openedPopupList.Clear();
            _onCloseBtnPressed = null;
		}

		//---------------------------------------
		/// <summary>
		/// Hardware Button에 의해 Popup이 종료 될 때 호출 될 Callback을 설정합니다. 
		/// 직접 호출하는 것은 권장되지 않습니다.
		/// </summary>
		public static void RegistOnCloseBtnClicked(Action onCallBack)
        {
            _onCloseBtnPressed = onCallBack;
        }


        /////////////////////////////////////////
        //---------------------------------------
        private HTKey _closeButton = null;


		/////////////////////////////////////////
		//---------------------------------------
		public override void Initialize()
		{
			_gEnv = HTFramework.Instance.RegistGEnv;
			_closeButton = HTInputManager.Instance.GetKey(_gEnv._input_Interact_Close);

			_instance = this;
			
			//-----
			SetInitialize();
		}

		public override void Tick(float fDelta)
		{
			if (_initialized == false)
				return;

			if (_enableHardwareButton == false)
				return;
			
			if (_allUIShowState && _closeButton != null && _closeButton.IsDown)
			{
				if (_openedPopupList.Count > 0)
				{
					for (int nInd = _openedPopupList.Count - 1; nInd >= 0; --nInd)
					{
						if (nInd < 0)
							break;

						if (_openedPopupList[nInd].CloseByHardwareButton)
						{
							_openedPopupList[nInd].ClosePopup();
							break;
						}
					}
				}
                else
                {
                    Utils.SafeInvoke(_onCloseBtnPressed);
                }
			}
		}

		public override void OnDestroy()
		{
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// 현재 열려있는 모든 UIPopup의 표시 여부를 설정합니다.
		/// UI가 사라지거나 다시 표시되도 OnOpen / OnClose는 호출되지 않습니다.
		/// </summary>
		public static void ShowAllPopup(bool bShow)
		{
			_allUIShowState = bShow;
			for (int nInd = 0; nInd < _openedPopupList.Count; ++nInd)
			{
				_openedPopupList[nInd].gameObject.SetActive(bShow);
				if (_openedPopupList[nInd].CreatedBackground != null)
					_openedPopupList[nInd].CreatedBackground.gameObject.SetActive(bShow);
			}
		}


		/////////////////////////////////////////
		//---------------------------------------
		public static HTToolTip RegistToolTip(GameObject pGameObject, HTToolTipContainer pToolTipInfo)
		{
			HTToolTip pToolTip = pGameObject.AddComponent<HTToolTip>();
			pToolTip.Init(pToolTipInfo);
			return pToolTip;
		}

		public static void ShowToolTip(HTToolTipContainer pToolTipInfo)
		{
			Utils.SafeActive(_toolTip, true);
			_toolTip.SetToolTipInfo(pToolTipInfo);

			//-----
			_toolTip.transform.SetAsLastSibling();
		}

		public static void HideToolTip()
		{
			Utils.SafeActive(_toolTip, false);
		}


		/////////////////////////////////////////
		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
