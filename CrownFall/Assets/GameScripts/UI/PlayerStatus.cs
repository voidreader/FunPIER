using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class PlayerStatus : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	public PlayerHPGage m_pHPGage;

	public Text m_szAttackPower;
	public Text m_szMoveSpeed;

	public Text m_szChargeTime;
	public Text m_szDashDelay;

	//---------------------------------------
	public ActorSkill m_pChargeSkill;
	public ActorSkill m_pDashSkill;

	//---------------------------------------
	int m_nPrevAttackPower;
	int m_nPrevMoveSpeed;

	int m_nPrevChargeTime;
	int m_nPrevDashDelay;


	/////////////////////////////////////////
	//---------------------------------------
	void Start () {
		UpdateStatus ();
		HT.HTLocaleTable.onLanguageChanged += UpdateStatus;
	}

	private void OnDestroy()
	{
		HT.HTLocaleTable.onLanguageChanged -= UpdateStatus;
	}

	void FixedUpdate () {
		PlayerData pData = GameFramework._Instance.m_pPlayerData;
		bool bNeedUpdate = false;

		//-----
		if (bNeedUpdate == false && m_nPrevAttackPower != pData.GetUpgrades (PlayerData.ePlayerUpgrades.eAtckPower))
			bNeedUpdate = true;

		if (bNeedUpdate == false && m_nPrevMoveSpeed != pData.GetUpgrades (PlayerData.ePlayerUpgrades.eSpeed))
			bNeedUpdate = true;

		if (bNeedUpdate == false && m_nPrevChargeTime != pData.GetUpgrades (PlayerData.ePlayerUpgrades.eChargeTime))
			bNeedUpdate = true;

		if (bNeedUpdate == false && m_nPrevDashDelay != pData.GetUpgrades (PlayerData.ePlayerUpgrades.eDashDelay))
			bNeedUpdate = true;

		//-----
		if (bNeedUpdate)
			UpdateStatus ();
	}


	/////////////////////////////////////////
	//---------------------------------------
	void UpdateStatus () {
		PlayerData pData = GameFramework._Instance.m_pPlayerData;

		string szSecondLocalString = HT.HTLocaleTable.GetLocalstring("gameunit_time");

		//-----
		m_nPrevAttackPower = pData.GetUpgrades (PlayerData.ePlayerUpgrades.eAtckPower);
		m_szAttackPower.text = string.Format ("{0} %", (int) (100 + (m_nPrevAttackPower * (100 * GameDefine.fAttackPowerIncreaseRatio))));

		//-----
		m_nPrevMoveSpeed = pData.GetUpgrades (PlayerData.ePlayerUpgrades.eSpeed);
		m_szMoveSpeed.text = string.Format ("{0} %", (int) (100 + (m_nPrevMoveSpeed * (100 * GameDefine.fMoveSpeedIncreaseRatio))));

		//-----
		m_nPrevChargeTime = pData.GetUpgrades (PlayerData.ePlayerUpgrades.eChargeTime);
		m_szChargeTime.text = string.Format ("{0:N2} {1}", 1.0f - (m_nPrevChargeTime * GameDefine.fChargeTimeDecreaseRatio), szSecondLocalString);

		//-----
		m_nPrevDashDelay = pData.GetUpgrades (PlayerData.ePlayerUpgrades.eDashDelay);
		m_szDashDelay.text = string.Format ("{0:N2} {1}", m_pDashSkill.m_fSkillCoolTime - (m_nPrevDashDelay * GameDefine.fDashDelayDecreaseRatio), szSecondLocalString);
	}
}
