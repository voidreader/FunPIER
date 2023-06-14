using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class Lobby_Battle : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public AudioClip _miscIntoBattle = null;

	//---------------------------------------
	private HT.UIPopup_MessageBoxParam _msgBoxParam = null;


	/////////////////////////////////////////
	//---------------------------------------
	private void Awake()
	{
		Button pButton = GetComponent<Button>();
		pButton.onClick.AddListener(OnClicked);

		_msgBoxParam = new HT.UIPopup_MessageBoxParam();
		_msgBoxParam.eType = HT.eMessageBoxType.Question;
		_msgBoxParam.szSubject = "msgwin_subj_warning";
		_msgBoxParam.szDescription = "msgwin_desc_battlestart";
		_msgBoxParam.szLBtnLocale = "msgwin_btn_ok";
		_msgBoxParam.szRBtnLocale = "msgwin_btn_cancel";
		_msgBoxParam.onLBtnClick = EventIntoBattle;
	}

	public void OnClicked()
	{
		HTUIManager.OpenMessageBox(_msgBoxParam);
	}

	//---------------------------------------
	public void EventIntoBattle()
	{
		//GameFramework._Instance.m_pPlayerData.m_bInitialized = false;

		//-----
		HT.HTSoundManager.PlaySound(_miscIntoBattle);
		BattleFramework._Instance.StartBattle();
	}
}
