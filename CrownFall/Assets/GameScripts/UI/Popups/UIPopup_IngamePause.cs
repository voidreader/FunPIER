using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HT;


/////////////////////////////////////////
//---------------------------------------
public sealed class UIPopup_IngamePause : UIPopup
{
	//---------------------------------------
	[SerializeField]
	private Button _retryBtn = null;
	[SerializeField]
	private Button _exitBtn = null;

	//---------------------------------------
	private UIPopup_MessageBoxParam _msgBoxParam_ExitGame = null;
	private UIPopup_MessageBoxParam _msgBoxParam_Restart = null;

	//---------------------------------------
	protected override void OnAwake()
	{
		base.OnAwake();

		_msgBoxParam_ExitGame = new UIPopup_MessageBoxParam();
		_msgBoxParam_ExitGame.eType = eMessageBoxType.Exclamation;
		_msgBoxParam_ExitGame.szSubject = "msgwin_subj_warning";
		_msgBoxParam_ExitGame.szDescription = (GameFramework._Instance.m_pPlayerData._isHardCoreMode)? "msgwin_desc_quitegame_ingame" : "msgwin_desc_quitegame";
		_msgBoxParam_ExitGame.szLBtnLocale = "msgwin_btn_ok";
		_msgBoxParam_ExitGame.szRBtnLocale = "msgwin_btn_cancel";
		_msgBoxParam_ExitGame.onLBtnClick = () => 
		{
			HTFramework.Instance.GameShutdown();
		};

		if (GameFramework._Instance.m_pPlayerData._isHardCoreMode)
		{
			_retryBtn.interactable = false;
			_msgBoxParam_Restart = null;
		}
		else
		{
			_retryBtn.interactable = true;
			_msgBoxParam_Restart = new UIPopup_MessageBoxParam();
			_msgBoxParam_Restart.Init(eMessageBoxType.Question, "msgwin_subj_warning", "msgwin_desc_retry", "msgwin_btn_ok", "msgwin_btn_cancel", () =>
			{
				BattleFramework._Instance.OnClickedRetry();
				ClosePopup();
			});
		}

		UIUtils.SafeAddClickEvent(_retryBtn, OnRetryClick);
		UIUtils.SafeAddClickEvent(_exitBtn, OnExitClick);
	}

	//---------------------------------------
	protected override void OnOpen()
	{
		TimeUtils.SetTimeScale(0.0f);
	}

	protected override void OnCloseComplete()
	{
		TimeUtils.SetTimeScale(1.0f);
	}

	protected override void OnTextSetting()
	{
	}

	//---------------------------------------
	public void OnRetryClick()
	{
		if (_msgBoxParam_Restart == null)
			return;

		HTUIManager.OpenMessageBox(_msgBoxParam_Restart);
	}

	public void OnExitClick()
	{
		HTUIManager.OpenMessageBox(_msgBoxParam_ExitGame);
	}
}


/////////////////////////////////////////
//---------------------------------------