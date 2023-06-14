using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class LoadSceneFramework : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("UI ELEMENTS")]
	public Button _backButton = null;

	public Toggle _hardCoreMode = null;
	public GameObject m_pSceneMask;

	[Header("HARD MODE LOCK")]
	public Button _hdl_HardButton = null;
	public Image _hdl_LockImage = null;

	//---------------------------------------
	private bool _hardLevelIsLocked = false;
	private bool _hardCoreModeOn = false;

	//---------------------------------------
	private HT.UIPopup_MessageBoxParam _closeMsgBoxParam = null;
	private HT.UIPopup_MessageBoxParam _hardLockMsgBoxParam = null;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Awake()
	{
		m_pSceneMask.SetActive(true);

		//-----
		_closeMsgBoxParam = new HT.UIPopup_MessageBoxParam();
		_closeMsgBoxParam.eType = HT.eMessageBoxType.Question;
		_closeMsgBoxParam.szSubject = "msgwin_subj_notice";
		_closeMsgBoxParam.szDescription = "ui_stageselect_totitle_msg";
		_closeMsgBoxParam.szLBtnLocale = "msgwin_btn_ok";
		_closeMsgBoxParam.szRBtnLocale = "msgwin_btn_cancel";
		_closeMsgBoxParam.onLBtnClick = () =>
		{
			HT.HTSoundManager.StopMusic(false);
			HT.HTFramework.Instance.SceneChange(GameDefine.szMainTitleSceneName);
		};

		//-----
		_hardLockMsgBoxParam = new HT.UIPopup_MessageBoxParam();
		_hardLockMsgBoxParam.eType = HT.eMessageBoxType.Exclamation;
		_hardLockMsgBoxParam.eButton = HT.eMessageBoxButton.OneButton;
		_hardLockMsgBoxParam.szSubject = "msgwin_subj_notice";
		_hardLockMsgBoxParam.szDescription = "ui_hardlock_msgbox_desc";
		_hardLockMsgBoxParam.szMBtnLocale = "msgwin_btn_close";

		//-----
		HT.HTUIManager.RegistOnCloseBtnClicked(() => {
			ShowToTitleMessageBox();
		});

		_backButton.onClick.AddListener(ShowToTitleMessageBox);

		//-----
		GameFramework pGameAfx = GameFramework._Instance;
		if (pGameAfx.m_pPlayerData.Initialized)
		{
			int nLevelIndex = pGameAfx.m_pPlayerData.m_nActivatedLevel;
			if (nLevelIndex > 0)
			{
				string szSceneName = pGameAfx.m_vLevelSettings[nLevelIndex].GetLevelName();
				HT.HTFramework.Instance.SceneChange(szSceneName, 0.0f);
			}
			else
			{
				HT.HTFramework.Instance.SceneChange(GameDefine.szSelectFieldSceneName, 0.0f);
			}
		}

		_hardCoreMode.onValueChanged.AddListener(SetHardCore);
		_hardCoreModeOn = _hardCoreMode.isOn;

		//-----
		bool bHardModeOpened = GameFramework._Instance._hardMode_Opened;

		_hardLevelIsLocked = (bHardModeOpened) ? false : true;
		//_hdl_HardButton.interactable = bHardModeOpened;
		_hdl_LockImage.gameObject.SetActive(_hardLevelIsLocked);

		//-----
		HT.HTThirdParty_Adbrix.Instance.OnSubmitFirstTimeEvent("Scene_Load");
	}

	//---------------------------------------
	private void ShowToTitleMessageBox()
	{
		HT.HTUIManager.OpenMessageBox(_closeMsgBoxParam);
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void SelectDifficulty_Easy()
	{
		ShowMessageBox("ui_diff_select_easy", ()=> {
			SetPlayInitialize(eGameDifficulty.eEasy);
		}, null);
	}

	public void SelectDifficulty_Normal()
	{
		ShowMessageBox("ui_diff_select_normal", () => {
			SetPlayInitialize(eGameDifficulty.eNormal);
		}, null);
	}

	public void SelectDifficulty_Hard()
	{
#if GAME_ENABLE_HARDLEVEL
#else // GAME_ENABLE_HARDLEVEL
		if (_hardLevelIsLocked)
		{
			HT.HTUIManager.OpenMessageBox(_hardLockMsgBoxParam);
			return;
		}
#endif // GAME_ENABLE_HARDLEVEL

		ShowMessageBox("ui_diff_select_hard", () => {
			SetPlayInitialize(eGameDifficulty.eHard);
		}, null);
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void SetHardCore(bool bSet)
	{
		if (bSet)
		{
			ShowMessageBox("ui_diff_hardcoremode", ()=> {
				_hardCoreModeOn = bSet;
			}, ()=> {
				_hardCoreModeOn = false;
				_hardCoreMode.isOn = false;
			});
		}
		else
		{
			_hardCoreModeOn = bSet;
		}
	}

	//---------------------------------------
	public void ShowMessageBox(string szDesc, Action pOnClickOK, Action pOnClickCancel)
	{
		HT.UIPopup_MessageBoxParam pParam = new HT.UIPopup_MessageBoxParam();
		pParam.szSubject = "msgwin_subj_notice";
		pParam.eType = HT.eMessageBoxType.Question;

		pParam.szLBtnLocale = "msgwin_btn_ok";
		pParam.szRBtnLocale = "msgwin_btn_cancel";

		pParam.szDescription = szDesc;
		pParam.onLBtnClick = pOnClickOK;
		pParam.onRBtnClick = pOnClickCancel;

		HT.HTUIManager.OpenMessageBox(pParam);
	}

	public void SetPlayInitialize(eGameDifficulty eDiff)
	{
		GameFramework pGame = GameFramework._Instance;

		//-----
		HT.HTThirdParty_Adbrix.Instance.OnSubmitFirstTimeEvent(string.Format("Scene_Load_Select_{0}", eDiff.ToString()));
		if (_hardCoreModeOn)
			HT.HTThirdParty_Adbrix.Instance.OnSubmitFirstTimeEvent("Scene_Load_Select_HardCore");

		pGame.ResetSaveDatas();
		pGame.m_pPlayerData.Initialized = true;

		//-----
		pGame.m_pPlayerData.m_eDifficulty = eDiff;
		pGame.m_pPlayerData._isHardCoreMode = _hardCoreModeOn;

		//-----
		pGame.m_pPlayerData.m_nActivatedLevel = -1;
		pGame.SetSaveInitialized();

		HT.HTFramework.Instance.SceneChange(GameDefine.szSelectFieldSceneName);

		//-----
	}


	/////////////////////////////////////////
	//---------------------------------------
}
