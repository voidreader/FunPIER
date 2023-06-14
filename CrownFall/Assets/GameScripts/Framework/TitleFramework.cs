using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class TitleFramework : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private Button _startButton = null;
	[SerializeField]
	private Button _continueButton = null;
	[SerializeField]
	private AudioClip _titleBGM = null;

	//---------------------------------------
	private HT.UIPopup_MessageBoxParam _closeMsgBoxParam = null;

	//---------------------------------------
#if ENABLE_GAMECENTER
	private static bool _initializedGameCenter = false;
#endif // ENABLE_GAMECENTER

	//---------------------------------------
	void Awake()
	{
		_closeMsgBoxParam = new HT.UIPopup_MessageBoxParam();
		_closeMsgBoxParam.eType = HT.eMessageBoxType.Question;
		_closeMsgBoxParam.szSubject = "msgwin_subj_notice";
		_closeMsgBoxParam.szDescription = "msgwin_desc_quitegame";
		_closeMsgBoxParam.szLBtnLocale = "msgwin_btn_ok";
		_closeMsgBoxParam.szRBtnLocale = "msgwin_btn_cancel";
		_closeMsgBoxParam.onLBtnClick = () =>
		{
			HT.HTFramework.Instance.GameShutdown();
		};

		HTUIManager.RegistOnCloseBtnClicked(() => {
			HTUIManager.OpenMessageBox(_closeMsgBoxParam);
		});

		//-----
		_startButton.onClick.AddListener(OnClick_NewGame);
		_continueButton.onClick.AddListener(OnClick_Continue);

		//-----
		HT.HTSoundManager.PlayMusic(_titleBGM);

		//-----
		GameFramework pGameAfx = GameFramework._Instance;
		if (pGameAfx.LoadGameData() == false)
		{
			pGameAfx.ResetSaveDatas();
			_continueButton.interactable = false;
		}
		else
		{
			_continueButton.interactable = true;
		}

		HT.HTThirdParty_Adbrix.Instance.OnSubmitFirstTimeEvent("Scene_Title");

		//-----
#if UNITY_ANDROID
		if (GameFramework._Instance._agreePrivacyTerms == false)
		{
			HT.UIPopup pPopup = HT.HTUIHelper.OpenPopup("Popup_AccessTerms");
			pPopup.onCloseCallback = () =>
			{
				GameFramework._Instance._agreePrivacyTerms = true;
				StartLoginProcess();
			};
		}
		else
#endif // UNITY_ANDROID
			StartLoginProcess();
	}

	//---------------------------------------
	public void StartLoginProcess()
	{
		StartCoroutine(ShowPlatformLogin_Internal());
	}

	//---------------------------------------
	private void OnClick_NewGame()
	{
		if (_continueButton.IsInteractable())
		{
			HT.UIPopup_MessageBoxParam pMsgBoxParam = new HT.UIPopup_MessageBoxParam();
			pMsgBoxParam.eType = HT.eMessageBoxType.Exclamation;

			pMsgBoxParam.szSubject = "msgwin_subj_warning";
			pMsgBoxParam.szDescription = "ui_newstart_alertmsg";

			pMsgBoxParam.szLBtnLocale = "msgwin_btn_ok";
			pMsgBoxParam.szRBtnLocale = "msgwin_btn_cancel";

			pMsgBoxParam.onLBtnClick = ()=> {
				GameFramework._Instance.m_pPlayerData.ResetData();
				HT.HTFramework.Instance.SceneChange(GameDefine.szLoadSceneName);
			};

			HTUIManager.OpenMessageBox(pMsgBoxParam);
		}
		else
		{
			HT.HTFramework.Instance.SceneChange(GameDefine.szLoadSceneName);
		}
	}

	private void OnClick_Continue()
	{
		HT.HTFramework.Instance.SceneChange(GameDefine.szLoadSceneName);
	}

	//---------------------------------------
	private IEnumerator ShowPlatformLogin_Internal()
	{
		yield return new WaitForEndOfFrame();

#if ENABLE_GAMECENTER
		if (_initializedGameCenter == false)
		{
			_initializedGameCenter = true;

			HT.HTDebug.PrintLog(HT.eMessageType.None, "[HTPlatform] Regist archives");

			Archives[] vArchives = ArchivementManager.Instance.Archives;
			for (int nInd = 0; nInd < vArchives.Length; ++nInd)
				if (string.IsNullOrEmpty(vArchives[nInd].Archive.CorrectID) == false)
					GameCenterManager.RegisterAchievement(vArchives[nInd].Archive.CorrectID);
		}
#endif // ENABLE_GAMECENTER

		GameFramework pGame = GameFramework._Instance;
#if ENABLE_STEAM
#else //ENABLE_STEAM
		if (pGame._socialLogin_Called && pGame._socialLogin_Joined == false)
			yield break;
#endif // ENABLE_STEAM

		if (pGame._socialLogin_Called == false)
			HT.HTPlatform.Instance.ClearLogInfo();

		if (HT.HTPlatform.Instance.PlatformActivated && HT.HTPlatform.Instance.IsLogIn == false)
		{
			pGame._socialLogin_Called = true;

			//-----
			HT.HTPlatform.Instance.InitializePlatformCallbacks(null, (bool bResult) =>
			{
				if (bResult == false)
					return;

				ArchivementManager.Instance.RefreshAllByPlatformData();
			});

			//-----
#if UNITY_EDITOR
#else // UNITY_EDITOR
			HT.HTGlobalUI.Instance.ShowIndicator(false);
#endif // UNITY_EDITOR

			HT.HTPlatform.Instance.LogIn((bool bResult) =>
			{
#if UNITY_EDITOR
#else // UNITY_EDITOR
				HT.HTGlobalUI.Instance.HideIndicator();
#endif // UNITY_EDITOR

#if ENABLE_GAMECENTER
				LevelSettings[] vSettings = GameFramework._Instance.m_vLevelSettings;
				for (int nInd = 0; nInd < vSettings.Length; ++nInd)
				{
					string[] vIDs = vSettings[nInd].LeaderBoardIDs_iOS;
					if (vIDs == null || vIDs.Length == 0)
						continue;

					for (int nID = 0; nID < vIDs.Length; ++nID)
					{
						if (string.IsNullOrEmpty(vIDs[nID]) == false)
						{
							HT.HTDebug.PrintLog(HT.eMessageType.None, string.Format("[HTPlatform] Call load leaderboard info {0} : {1}", vSettings[nInd]._fieldName, vIDs[nID]));
							GameCenterManager.LoadLeaderboardInfo(vIDs[nID]);
						}
					}
				}
#endif // ENABLE_GAMECENTER
			});
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------