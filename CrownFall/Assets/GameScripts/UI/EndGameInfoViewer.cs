using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class EndGameInfoViewer : MonoBehaviour
{
	//---------------------------------------
	[Header("COMMON INFO")]
	[SerializeField]
	private Text _difficulty = null;
	[SerializeField]
	private Color _difficulty_color_Easy = Color.white;
	[SerializeField]
	private Color _difficulty_color_Normal = Color.white;
	[SerializeField]
	private Color _difficulty_color_Hard = Color.white;

	//---------------------------------------
	[Header("LEVEL INFO")]
	[SerializeField]
	private Image _levelIllust = null;

	[SerializeField]
	private Text _stateIndex = null;
	[SerializeField]
	private Text _bossName = null;

	//---------------------------------------
	[Header("PLAYER INFO")]
	[SerializeField]
	private Text _record_BattleTime = null;
	[SerializeField]
	private Text _record_TryCount = null;

	[SerializeField]
	private Text _record_PlayerDamaged = null;
	[SerializeField]
	private Text _record_PlayerAttackCnt = null;
	[SerializeField]
	private Text _record_PlayerDashCnt = null;

	//---------------------------------------
	[Header("BOSS INFO")]
	[SerializeField]
	private Text _record_BossHitCount;
	[SerializeField]
	private Text _record_BossDamaged;
	[SerializeField]
	private Text _record_CriticalCount = null;

	//---------------------------------------
	[Header("SPECTIAL ATTACK INFO")]
	[SerializeField]
	private Text _record_SpdAtkDamage = null;
	[SerializeField]
	private Text _record_DamageIgnored = null;

	//---------------------------------------
	[Header("SHARE BUTTON")]
	[SerializeField]
	private ShareButton _shareButton = null;

	//---------------------------------------
	public void Init(int nRecordIndex)
	{
		GameFramework pGame = GameFramework._Instance;
		BossKillRecord pRecord = pGame.m_pPlayerData.m_vBossKillRecord[nRecordIndex];
		LevelSettings pLevel = pGame.m_vLevelSettings[pRecord.nBossIndex];

		//-----
		if (_shareButton != null)
			_shareButton.Init(nRecordIndex);

		//-----
		string szDifficulty = "";
		Color pDifficultyColor = _difficulty_color_Easy;

		switch (GameFramework._Instance.m_pPlayerData.m_eDifficulty)
		{
			case eGameDifficulty.eEasy:
				szDifficulty = "ui_difficulty_easy";
				pDifficultyColor = _difficulty_color_Easy;
				break;

			case eGameDifficulty.eNormal:
				szDifficulty = "ui_difficulty_normal";
				pDifficultyColor = _difficulty_color_Normal;
				break;

			case eGameDifficulty.eHard:
				szDifficulty = "ui_difficulty_hard";
				pDifficultyColor = _difficulty_color_Hard;
				break;
		}

		_difficulty.color = pDifficultyColor;
		string szLocale_Difficulty = HT.HTLocaleTable.GetLocalstring("ui_difficulty");
		string szLocale_DifficultyLevel = HT.HTLocaleTable.GetLocalstring(szDifficulty);

		if (pGame.m_pPlayerData._isHardCoreMode)
		{
			string szLocale_HardCore = HT.HTLocaleTable.GetLocalstring("ui_difficulty_hardcore");
			_difficulty.text = string.Format("({0} : {1} - {2})", szLocale_Difficulty, szLocale_DifficultyLevel, szLocale_HardCore);
		}
		else
			_difficulty.text = string.Format("({0} : {1})", szLocale_Difficulty, szLocale_DifficultyLevel);

		//-----
		if (_levelIllust != null)
			_levelIllust.sprite = pLevel.m_pLevelIllust;

		_stateIndex.text = HT.HTLocaleTable.GetLocalstring("gameunit_stage") + " " + (nRecordIndex + 1);

		_bossName.color = pLevel.m_pLevelColor;
		_bossName.text = HT.HTLocaleTable.GetLocalstring(pLevel.m_pEnemyActor.m_pActorInfo.m_szActorName);

		_record_TryCount.text = string.Format("{0}", pGame.m_pPlayerData.m_vTryCount[pRecord.nBossIndex]);
		_record_BattleTime.text = string.Format("{0:N2} {1}", pRecord.fTime, HT.HTLocaleTable.GetLocalstring("gameunit_time"));

		//-----
		string szIntFormat = "{0}";
		_record_PlayerDamaged.text = string.Format(szIntFormat, pRecord.nPlayerDamaged);
		_record_PlayerAttackCnt.text = string.Format(szIntFormat, pRecord.nPlayerAttackCount);
		_record_PlayerDashCnt.text = string.Format(szIntFormat, pRecord.nPlayerDashCount);

		_record_BossHitCount.text = string.Format(szIntFormat, pRecord.nBossHitCount);
		_record_BossDamaged.text = string.Format(szIntFormat, pRecord.nBossDamaged);
		_record_CriticalCount.text = string.Format(szIntFormat, pRecord.nCriticalCount);

		_record_SpdAtkDamage.text = string.Format(szIntFormat, pRecord.nSpcAtkDamage);
		_record_DamageIgnored.text = string.Format(szIntFormat, pRecord.nDamageIgnored);
	}
}


/////////////////////////////////////////
//---------------------------------------