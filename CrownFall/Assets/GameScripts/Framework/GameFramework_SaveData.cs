using UnityEngine;
using System.Collections;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class BossKillRecord : IJSONSerialization
{
	public int nBossIndex = 0;

	public float fTime = 0;
	public int nPlayerDamaged = 0;

	public int nPlayerAttackCount = 0;
	public int nPlayerDashCount = 0;

	public int nBossHitCount = 0;
	public int nBossDamaged = 0;
	public int nCriticalCount = 0;

	public int nSpcAtkDamage = 0;
	public int nDamageIgnored = 0;

	public bool bVictory = false;

	//-----
	public bool bSetInfomationComplete = false;

	//-----
	public void Reset()
	{
		nBossIndex = -1;

		fTime = 0.0f;
		nPlayerDamaged = 0;

		nPlayerAttackCount = 0;
		nPlayerDashCount = 0;

		nBossHitCount = 0;
		nBossDamaged = 0;
		nCriticalCount = 0;

		nSpcAtkDamage = 0;
		nDamageIgnored = 0;

		bVictory = false;

		bSetInfomationComplete = false;
	}

	//-----
	public bool ParseJSON(JSONNode pNode, JSONSerializationMode eMode)
	{
		if (pNode == null)
			return false;

		JSONUtils.GetValue(pNode, "SIC", out bSetInfomationComplete);

		JSONUtils.GetValue(pNode, "BSI", out nBossIndex);
		JSONUtils.GetValue(pNode, "TIM", out fTime);

		JSONUtils.GetValue(pNode, "PDMG", out nPlayerDamaged);
		JSONUtils.GetValue(pNode, "PATKC", out nPlayerAttackCount);
		JSONUtils.GetValue(pNode, "PDSHC", out nPlayerDashCount);

		JSONUtils.GetValue(pNode, "BHC", out nBossHitCount);
		JSONUtils.GetValue(pNode, "BD", out nBossDamaged);
		JSONUtils.GetValue(pNode, "CC", out nCriticalCount);

		JSONUtils.GetValue(pNode, "SPD", out nSpcAtkDamage);
		JSONUtils.GetValue(pNode, "DI", out nDamageIgnored);

		JSONUtils.GetValue(pNode, "VTR", out bVictory);

		return true;
	}

	public JSONNode ToJSON(JSONSerializationMode eMode)
	{
		JSONClass pJSON = new JSONClass();

		JSONUtils.AddValue(pJSON, "SIC", bSetInfomationComplete);

		JSONUtils.AddValue(pJSON, "BSI", nBossIndex);
		JSONUtils.AddValue(pJSON, "TIM", fTime);

		JSONUtils.AddValue(pJSON, "PDMG", nPlayerDamaged);
		JSONUtils.AddValue(pJSON, "PATKC", nPlayerAttackCount);
		JSONUtils.AddValue(pJSON, "PDSHC", nPlayerDashCount);

		JSONUtils.AddValue(pJSON, "BHC", nBossHitCount);
		JSONUtils.AddValue(pJSON, "BD", nBossDamaged);
		JSONUtils.AddValue(pJSON, "CC", nCriticalCount);

		JSONUtils.AddValue(pJSON, "SPD", nSpcAtkDamage);
		JSONUtils.AddValue(pJSON, "DI", nDamageIgnored);

		JSONUtils.AddValue(pJSON, "VTR", bVictory);

		return pJSON;
	}
}


/////////////////////////////////////////
//---------------------------------------
public class PlayerData : IJSONSerialization
{
	/////////////////////////////////////////
	//---------------------------------------
	public eGameDifficulty m_eDifficulty = eGameDifficulty.eNormal;

	//---------------------------------------
	public enum ePlayerUpgrades
	{
		eHealth = 0,
		eAtckPower,
		eSpeed,
		eChargeTime,
		eDashDelay,

		eMax = 5,
	}

	public bool[] m_vLevelActivated = new bool[GameFramework._Instance.m_vLevelSettings.Length];
	public int[] m_vTryCount = new int[GameFramework._Instance.m_vLevelSettings.Length];
	public int[] m_vUpgrades = new int[(int)ePlayerUpgrades.eMax];
	public int m_nUpgradePoint = 1;

	public BossKillRecord[] m_vBossKillRecord = new BossKillRecord[GameFramework._Instance.m_vLevelSettings.Length];
	public int m_nKillRecordWriteIndex = 0;

	//---------------------------------------
	private bool m_bInitialized = false;
	public bool Initialized
	{
		get { return m_bInitialized; }
		set
		{
			HT.HTDebug.PrintLog(eMessageType.None, string.Format("[Save Data] Initialize state is [{0}]", (value)? "TRUE" : "FALSE"));
			m_bInitialized = value;
		}
	}
	
	public bool _isHardCoreMode = false;

	public int m_nActivatedLevel = -1;
	public bool _battleIsEnd = false;
	

	/////////////////////////////////////////
	//---------------------------------------
	public void ResetData()
	{
		//-----
		m_eDifficulty = eGameDifficulty.eNormal;

		for (int nInd = 0; nInd < m_vLevelActivated.Length; ++nInd)
			m_vLevelActivated[nInd] = false;

		//-----
		for (int nInd = 0; nInd < m_vUpgrades.Length; ++nInd)
			m_vUpgrades[nInd] = 0;

		m_nUpgradePoint = 1;

		//-----
		for (int nInd = 0; nInd < m_vBossKillRecord.Length; ++nInd)
		{
			if (m_vBossKillRecord[nInd] == null)
				m_vBossKillRecord[nInd] = new BossKillRecord();

			m_vBossKillRecord[nInd].Reset();
			m_nKillRecordWriteIndex = 0;
		}

        for(int nInd = 0; nInd < m_vTryCount.Length; ++nInd)
            m_vTryCount[nInd] = 0;

		//-----
		m_bInitialized = false;
		_battleIsEnd = false;

		//-----
		m_nActivatedLevel = -1;
	}

	public int GetUpgrades(ePlayerUpgrades eInd)
	{
		return m_vUpgrades[(int)eInd];
	}

	public BossKillRecord GetLastKillRecord()
	{
		for (int nInd = 0; nInd < m_vBossKillRecord.Length; ++nInd)
			if (m_vBossKillRecord[nInd].bSetInfomationComplete == false)
				return m_vBossKillRecord[nInd];

		return null;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public bool ParseJSON(JSONNode pNode, JSONSerializationMode eMode)
	{
		if (pNode == null)
			return false;

		//-----
		bool bIsDemo = false;
#if DEMO_VERSION
		bIsDemo = true;
#endif // DEMO_VERSION

		bool bDemoVersionSave = false;
		JSONUtils.GetValue(pNode, "DMV", out bDemoVersionSave);
		if (bIsDemo != bDemoVersionSave)
			return false;

		//-----
		JSONUtils.GetValue(pNode, "ACT", out m_bInitialized);
		JSONUtils.GetValue(pNode, "BIS", out _battleIsEnd);

		JSONUtils.GetValue(pNode, "ACL", out m_nActivatedLevel);

		JSONUtils.GetValueEnum(pNode, "DIFF", out m_eDifficulty);

		//-----
		int nActivateCount = 0;
		JSONUtils.GetValue(pNode, "ATC", out nActivateCount);

		for (int nInd = 0; nInd < m_vLevelActivated.Length; ++nInd)
			m_vLevelActivated[nInd] = false;

		for (int nInd = 0; nInd < nActivateCount; ++nInd)
			JSONUtils.GetValue(pNode, string.Format("ATS{0}", nInd), out m_vLevelActivated[nInd]);

		//-----
		int nTryCount = 0;
		JSONUtils.GetValue(pNode, "TC", out nTryCount);

		for (int nInd = 0; nInd < m_vTryCount.Length; ++nInd)
			m_vTryCount[nInd] = 0;

		for (int nInd = 0; nInd < nTryCount; ++nInd)
			JSONUtils.GetValue(pNode, string.Format("TC{0}", nInd), out m_vTryCount[nInd]);

		//-----
		int nUpgradeCount = 0;
		JSONUtils.GetValue(pNode, "UPC", out nUpgradeCount);
		for (int nInd = 0; nInd < nUpgradeCount; ++nInd)
			JSONUtils.GetValue(pNode, string.Format("UPG{0}", nInd), out m_vUpgrades[nInd]);

		//-----
		JSONUtils.GetValue(pNode, "UPP", out m_nUpgradePoint);

		JSONUtils.GetValue(pNode, "KRW", out m_nKillRecordWriteIndex);
		JSONUtils.GetValueSerialize(pNode, "KLR", out m_vBossKillRecord);

		return true;
	}

	public JSONNode ToJSON(JSONSerializationMode eMode)
	{
		JSONClass pJSON = new JSONClass();

		//-----
#if DEMO_VERSION
		JSONUtils.AddValue(pJSON, "DMV", true);
#else // DEMO_VERSION
		JSONUtils.AddValue(pJSON, "DMV", false);
#endif // DEMO_VERSION

		//-----
		if (_isHardCoreMode == false && m_bInitialized == false && _battleIsEnd == false)
		{
			for (int nInd = 0; nInd < m_vLevelActivated.Length; ++nInd)
			{
				if (m_vLevelActivated[nInd] == false)
				{
					HT.HTDebug.PrintLog(eMessageType.Warning, "[SAVE DATA] Fixed not initialized state!");
					m_bInitialized = true;
					break;
				}
			}
		}

		if (_battleIsEnd && m_bInitialized)
		{
			HT.HTDebug.PrintLog(eMessageType.Warning, "[SAVE DATA] Fixed not initialized state!");
			m_bInitialized = false;
		}

		JSONUtils.AddValue(pJSON, "ACT", m_bInitialized);
		JSONUtils.AddValue(pJSON, "BIS", _battleIsEnd);

		JSONUtils.AddValue(pJSON, "ACL", m_nActivatedLevel);

		JSONUtils.AddValue(pJSON, "DIFF", m_eDifficulty);

		//-----
		int nActivateCount = m_vLevelActivated.Length;
		JSONUtils.AddValue(pJSON, "ATC", nActivateCount);
		for (int nInd = 0; nInd < nActivateCount; ++nInd)
			JSONUtils.AddValue(pJSON, string.Format("ATS{0}", nInd), m_vLevelActivated[nInd]);

		//-----
		int nTryCount = m_vTryCount.Length;
		JSONUtils.AddValue(pJSON, "TC", nTryCount);
		for (int nInd = 0; nInd < nTryCount; ++nInd)
			JSONUtils.AddValue(pJSON, string.Format("TC{0}", nInd), m_vTryCount[nInd]);

		//-----
		int nUpgradeCount = m_vUpgrades.Length;
		JSONUtils.AddValue(pJSON, "UPC", nUpgradeCount);
		for (int nInd = 0; nInd < nUpgradeCount; ++nInd)
			JSONUtils.AddValue(pJSON, string.Format("UPG{0}", nInd), m_vUpgrades[nInd]);

		//-----
		JSONUtils.AddValue(pJSON, "UPP", m_nUpgradePoint);

		JSONUtils.AddValue(pJSON, "KRW", m_nKillRecordWriteIndex);
		JSONUtils.AddValueSerialize(pJSON, "KLR", m_vBossKillRecord);

		return pJSON;
	}
}