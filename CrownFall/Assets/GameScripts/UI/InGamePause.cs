using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class InGamePause : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private string _pausePopupAddr = null;
	[SerializeField]
	private Button _systemButton = null;

	//---------------------------------------
	HT.HTKey _cancelKey = null;

	//---------------------------------------
	private void Awake()
	{
		_cancelKey = HT.HTInputManager.Instance.GetKey(GameDefine.szKeyName_Cancel);
		
		//-----
		_systemButton.onClick.AddListener(OnClickLinkedButton);
	}

	//---------------------------------------
    private void OnEnable()
    {
		HTUIManager.RegistOnCloseBtnClicked(OnCloseBtnPressed);
	}

    private void OnDisable()
    {
		HTUIManager.RegistOnCloseBtnClicked(null);
    }

	//---------------------------------------
	public void OnClickLinkedButton()
	{
		if (CheckState() == false)
			return;

		ShowPopup();
	}

	private void OnCloseBtnPressed()
    {
		if (CheckState() == false)
			return;

        if (_cancelKey != null && _cancelKey.IsDown)
			ShowPopup();
	}

	private bool CheckState()
	{
		if (HT.HTAfxPref.WaitingForSceneChange)
			return false;

		if (HT.TimeUtils.TimeScale <= 0.0f)
			return false;

		switch (BattleFramework._Instance.m_eBattleState)
		{
			case BattleFramework.eBattleState.eEnd:
			case BattleFramework.eBattleState.eEnd_Wait:
			case BattleFramework.eBattleState.eGameDefeat:
			case BattleFramework.eBattleState.eGameWin:
				return false;
		}

		return true;
	}

	private void ShowPopup()
	{
		if (HTUIManager.OpenedPopupCount == 0)
			HTUIManager.OpenPopup(_pausePopupAddr);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------