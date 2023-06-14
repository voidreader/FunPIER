using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class ShareButton : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private Button _shareButton = null;

	//---------------------------------------
	private static Texture2D _screenShot = null;
	private Coroutine _shareProc = null;

	//---------------------------------------
	private const string _share_CaptionBase = "ui_share_caption";
	private const string _share_MessageBase = "ui_share_message_base";

	private const string _share_Message_Easy = "ui_difficulty_easy";
	private const string _share_Message_Normal = "ui_difficulty_normal";
	private const string _share_Message_Hard = "ui_difficulty_hard";
	private const string _share_Message_HardCore = "ui_difficulty_hardcore";

	private const string _share_Message_Win = "killrecord_win";
	private const string _share_Message_Defeat = "killrecord_defeat";

	private const string _share_Message_End_Win = "!";
	private const string _share_Message_End_Defeat = "...";

	private string _share_MessageCompared = null;

	//---------------------------------------
#if UNITY_ANDROID
	private AN_Permission[] _permissions_Require = new AN_Permission[] { AN_Permission.READ_CONTACTS, AN_Permission.READ_EXTERNAL_STORAGE };

	private List<AN_Permission> _permission_DeniedList = new List<AN_Permission>();
	private Action<bool> _permission_Successed = null;
#endif // UNITY_ANDROID

	//---------------------------------------
	public void Init(int nRecordIndex)
	{
		InitShareButton(nRecordIndex);

		if (_shareButton != null)
		{
			_shareButton.onClick.RemoveAllListeners();
			_shareButton.onClick.AddListener(OnClickShareButton);
		}
	}

	//---------------------------------------
	private void InitShareButton(int nRecordIndex)
	{
		GameFramework pGame = GameFramework._Instance;
		BossKillRecord pRecord = pGame.m_pPlayerData.m_vBossKillRecord[nRecordIndex];
		LevelSettings pLevel = pGame.m_vLevelSettings[pRecord.nBossIndex];

		//-----
		string szMessage = HT.HTLocaleTable.GetLocalstring(_share_MessageBase);

		//-----
		string szMessage_Diff = null;
		switch (pGame.m_pPlayerData.m_eDifficulty)
		{
			case eGameDifficulty.eEasy:
				szMessage_Diff = _share_Message_Easy;
				break;

			case eGameDifficulty.eNormal:
				szMessage_Diff = _share_Message_Normal;
				break;

			case eGameDifficulty.eHard:
				szMessage_Diff = _share_Message_Hard;
				break;
		}
		szMessage_Diff = HT.HTLocaleTable.GetLocalstring(szMessage_Diff);

		//-----
		string szMessage_Hardcore = string.Empty;
		if (pGame.m_pPlayerData._isHardCoreMode)
			szMessage_Hardcore = string.Format(" ({0})", HT.HTLocaleTable.GetLocalstring(_share_Message_HardCore));

		//-----
		string szResult = null;
		string szMessageEnd = null;
		if (pRecord.bVictory)
		{
			szResult = _share_Message_Win;
			szMessageEnd = _share_Message_End_Win;
		}
		else
		{
			szResult = _share_Message_Defeat;
			szMessageEnd = _share_Message_End_Defeat;
		}
		szResult = HT.HTLocaleTable.GetLocalstring(szResult);

		//-----
		string szBossName = HT.HTLocaleTable.GetLocalstring(pLevel.m_pEnemyActor.m_pActorInfo.m_szActorName);

		//-----
		_share_MessageCompared = string.Format(szMessage, szMessage_Diff, szMessage_Hardcore, szBossName, szResult, szMessageEnd);
	}

	private void OnClickShareButton()
	{
		if (_shareProc == null)
		{
#if UNITY_ANDROID
			ProcessPermissions(() => {
#endif // UNITY_ANDROID
				_shareProc = StartCoroutine(ShareProc_Internal());
#if UNITY_ANDROID
			});
#endif // UNITY_ANDROID
		}
	}

	//---------------------------------------
#if UNITY_ANDROID
	private void ProcessPermissions(Action onPermissionSucc)
	{
		_permission_DeniedList.Clear();
		for (int nInd = 0; nInd < _permissions_Require.Length; ++nInd)
		{
			if (PermissionsManager.IsPermissionGranted(_permissions_Require[nInd]) == false)
				_permission_DeniedList.Add(_permissions_Require[nInd]);
		}

		if (_permission_DeniedList.Count > 0)
		{
			HTDebug.PrintLog(eMessageType.None, string.Format("[ShareBtn] Denied permissions count {0}. Call permission request", _permission_DeniedList.Count));

			//-----
			Action onPermissionFail = () =>
			{
				UIPopup_MessageBoxParam pFailParam = new UIPopup_MessageBoxParam();
				pFailParam.Init(eMessageBoxType.Exclamation, "msgwin_subj_error", "ui_permissionfailed", "msgwin_btn_close");

				HTUIHelper.OpenMessageBox(pFailParam);
			};

			//bool bSomethingPermissionNeverShow = false;
			//for(int nInd = 0; nInd < _permission_DeniedList.Count; ++nInd)
			//{
			//	if (PermissionsManager.ShouldShowRequestPermission(_permission_DeniedList[nInd]) == false)
			//	{
			//		HTDebug.PrintLog(eMessageType.None, string.Format("[ShareBtn] ShouldShowRequestPermission : {0}", _permission_DeniedList[nInd].ToString()));
			//		bSomethingPermissionNeverShow = true;
			//	}
			//}
			//
			//if (bSomethingPermissionNeverShow)
			//{
			//	Utils.SafeInvoke(onPermissionFail);
			//	return;
			//}

			//-----
			UIPopup_MessageBoxParam pParam = new UIPopup_MessageBoxParam();
			pParam.Init(eMessageBoxType.Exclamation, "msgwin_subj_notice", "msgwin_desc_pms_sharing", "msgwin_btn_close", () => 
			{
				CallNextPermission((bool bResult) =>
				{
					if (bResult)
						Utils.SafeInvoke(onPermissionSucc);

					else
						Utils.SafeInvoke(onPermissionFail);
				});
			});

			HTUIHelper.OpenMessageBox(pParam);
		}
		else
		{
			HTDebug.PrintLog(eMessageType.None, "[ShareBtn] Permissions all granted.");
			Utils.SafeInvoke(onPermissionSucc);
		}
	}

	private void CallNextPermission(Action<bool> onComplete)
	{
		if (_permission_DeniedList.Count == 0)
		{
			Utils.SafeInvoke(onComplete, true);
			return;
		}

		//-----
		PermissionsManager.ActionPermissionsRequestCompleted += OnPermissionResultCallback;
		_permission_Successed = onComplete;
		
		PermissionsManager.Instance.RequestPermissions(_permission_DeniedList.ToArray());
	}

	private void OnPermissionResultCallback(AN_GrantPermissionsResult pResult)
	{
		PermissionsManager.ActionPermissionsRequestCompleted -= OnPermissionResultCallback;

		//-----
		if (pResult.HasError)
			HTDebug.PrintLog(eMessageType.Error, pResult.Error.Message);

		//-----
		bool bAllGranted = true;
		_permission_DeniedList.Clear();

		foreach (KeyValuePair<AN_Permission, AN_PermissionState> pair in pResult.RequestedPermissionsState)
		{
			if (pair.Value == AN_PermissionState.PERMISSION_DENIED)
			{
				bAllGranted = false;

				if (PermissionsManager.ShouldShowRequestPermission(pair.Key))
					HTDebug.PrintLog(eMessageType.Warning, string.Format("[ShareBtn] Permission Denied {0}", pair.Key));
				else
					HTDebug.PrintLog(eMessageType.Warning, string.Format("[ShareBtn] Permission Denied {0} by never show dialog!", pair.Key));
			}
		}

		Utils.SafeInvoke(_permission_Successed, bAllGranted);
	}
#endif // UNITY_ANDROID

	//---------------------------------------
	private IEnumerator ShareProc_Internal()
	{
		yield return new WaitForEndOfFrame();

		//-----
		if (_screenShot == null)
			_screenShot = new Texture2D(Screen.width, Screen.height);
		
		_screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		_screenShot.Apply();

		//-----
		try
		{
			string szCaption = HT.HTLocaleTable.GetLocalstring(_share_CaptionBase);
#if UNITY_ANDROID
			AndroidSocialGate.StartShareIntent(szCaption, _share_MessageCompared, _screenShot);
#elif UNITY_IOS
		IOSSocialManager.Instance.ShareMedia(szCaption, _screenShot);
#endif // UNITY_IOS
		}
		catch (Exception)
		{
			UIPopup_MessageBoxParam pParam = new UIPopup_MessageBoxParam();
			pParam.Init(eMessageBoxType.Exclamation, "msgwin_subj_error", "msgwin_desc_procerror", "msgwin_btn_close");

			HTUIManager.OpenMessageBox(pParam);
		}

		_shareProc = null;
	}
}


/////////////////////////////////////////
//---------------------------------------