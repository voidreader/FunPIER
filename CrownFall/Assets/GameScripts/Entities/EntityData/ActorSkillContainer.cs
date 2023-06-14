using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class ActorSkillContainer : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("SKILL VIEW INFO")]
	public bool _showToSkillInfo = true;
	public string _skillInfo_Name = null;
	public string[] _skillInfo_Desc = null;
	public eGameDifficulty _skillInfo_AddSkillDiff = eGameDifficulty.eEasy;

	[Header("SKILL SET")]
	public string _skillName = null;
	public ActorSkill[] m_pvSkills;


	/////////////////////////////////////////
	//---------------------------------------
	static private ActorSkill _dummySkill = null;


	/////////////////////////////////////////
	//---------------------------------------
	public void Init()
	{
		_skillName = m_pvSkills[0].m_szSkillName;

		for (int nInd = 0; nInd < m_pvSkills.Length; ++nInd)
		{
			ActorSkill pSkill = m_pvSkills[nInd];
			pSkill.ResetAll();

			//-----
			pSkill.m_bCastYet = false;

			//-----
			float fTotalCoolTime = pSkill.m_fSkillCoolTime + pSkill.m_fSkillCoolTime_Extend_Mobile;
			if (pSkill.m_bSkillPrewarmed)
			{
				float fPrewarmTime = fTotalCoolTime - (fTotalCoolTime * pSkill._skillPrewarmRatio);
				pSkill.SetSkillCooling(fPrewarmTime);
			}
			else
				pSkill.SetSkillCooling(fTotalCoolTime);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public ActorSkill GetSkillByLevel()
	{
		if (_showToSkillInfo == false)
		{
			if (_dummySkill == null)
			{
				GameObject pEmptyObj = HT.Utils.Instantiate();

				_dummySkill = pEmptyObj.AddComponent<ActorSkill>();

				_dummySkill.m_fSkillMinRange = -1.0f;
				_dummySkill.m_fSkillMaxRange = -1.0f;
				_dummySkill.m_bSkillPrewarmed = false;

				_dummySkill.m_fSkillCoolTime = float.PositiveInfinity;
				_dummySkill.SetSkillCooling(float.PositiveInfinity);
			}

			return _dummySkill;
		}

		eGameDifficulty pDifficulty = GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		int nLevel = Mathf.Min(m_pvSkills.Length - 1, (int)pDifficulty);

		return m_pvSkills[nLevel];
	}
}
