using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class BossSkillViewElement : MonoBehaviour
{
	//---------------------------------------
	[Header("OBJECT SETTING")]
	[SerializeField]
	private Text _skillName = null;
	[SerializeField]
	private Text _skillDesc = null;
	[SerializeField]
	private Text _skillInfo_CoolTime = null;
	[SerializeField]
	private Text _skillInfo_Range = null;

	[Header("ELEMENT SETTING")]
	[SerializeField]
	private Color _skillNameColor_All = Color.white;
	[SerializeField]
	private Color _skillNameColor_Normal = Color.blue;
	[SerializeField]
	private Color _skillNameColor_Hard = Color.red;

	//---------------------------------------
	const string _locale_CoolTime = "boss_skill_view_cooltime";
	const string _locale_Range = "boss_skill_view_range";

	const string _locale_Range_Min = "boss_skill_view_range_min";
	const string _locale_Range_Max = "boss_skill_view_range_max";
	const string _locale_Range_MinMax = "boss_skill_view_range_minmax";

	//---------------------------------------
	public bool SetSkillInfo(ActorSkillContainer pContainer)
	{
		pContainer.Init();

		//-----
		eGameDifficulty eGameDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		if (eGameDiff < pContainer._skillInfo_AddSkillDiff)
			return false;

		Color pSkillNameColor = _skillNameColor_All;
		switch(pContainer._skillInfo_AddSkillDiff)
		{
			case eGameDifficulty.eNormal:
				pSkillNameColor = _skillNameColor_Normal;
				break;

			case eGameDifficulty.eHard:
				pSkillNameColor = _skillNameColor_Hard;
				break;
		}

		_skillName.text = HT.HTLocaleTable.GetLocalstring(pContainer._skillName);
		_skillName.color = pSkillNameColor;

		string[] vSkillDescs = pContainer._skillInfo_Desc;

		int nDescIndex = 0;
		if (vSkillDescs.Length >= (int)eGameDiff)
			nDescIndex = (int)eGameDiff;

		_skillDesc.text = vSkillDescs[nDescIndex];

		//-----
		ActorSkill pCurSkill = pContainer.GetSkillByLevel();

		string szCoolTimeDesc = HT.HTLocaleTable.GetLocalstring(_locale_CoolTime);
		_skillInfo_CoolTime.text = string.Format(szCoolTimeDesc, (int)pCurSkill.m_fSkillCoolTime);

		string szRangeDesc = HT.HTLocaleTable.GetLocalstring(_locale_Range);

		string szRange = null;
		if (pCurSkill.m_fSkillMinRange <= 0.0f || pCurSkill.m_fSkillMaxRange > 20.0f)
		{
			if (pCurSkill.m_fSkillMinRange <= 0.0f)
			{
				szRange = HT.HTLocaleTable.GetLocalstring(_locale_Range_Max);
				szRange = string.Format(szRange, (int)pCurSkill.m_fSkillMaxRange);
			}
			else
			{
				szRange = HT.HTLocaleTable.GetLocalstring(_locale_Range_Min);
				szRange = string.Format(szRange, (int)pCurSkill.m_fSkillMinRange);
			}
		}
		else
		{
			szRange = HT.HTLocaleTable.GetLocalstring(_locale_Range_MinMax);
			szRange = string.Format(szRange, pCurSkill.m_fSkillMinRange, pCurSkill.m_fSkillMaxRange);
		}

		_skillInfo_Range.text = string.Format(szRangeDesc, (int)pCurSkill.m_fSkillMinRange);

		//-----
		return true;
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------