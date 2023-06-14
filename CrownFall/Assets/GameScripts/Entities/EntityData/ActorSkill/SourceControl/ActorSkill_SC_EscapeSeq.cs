using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class ActorSkill_SC_EscapeSeq : ActorSkill
{
	//---------------------------------------
	[SerializeField]
	private float _whenCastHpRatio = 0.35f;
	[SerializeField]
	private string _animName_IDLE_Default = null;
	[SerializeField]
	private string _animName_IDLE_RedScreen = null;

	//---------------------------------------
	private int _castedCount = 0;
	private bool _processing = false;

	//---------------------------------------
	public override void ResetAll()
	{
		_castedCount = 0;
		_processing = false;
	}

	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
		float fHPRatio = m_pCaster.GetCurrHP() / (float)m_pCaster.m_pActorInfo.m_cnMaxHP.val;
		float fCastHP = 1.0f - ((_castedCount + 1) * _whenCastHpRatio);
		if (fHPRatio > fCastHP)
			return false;

		Field_PowerPlant pPlant = BattleFramework._Instance.m_pField as Field_PowerPlant;
		if (pPlant == null)
			return false;

		++_castedCount;
		_processing = true;
		pPlant.OnCastEscapeSequence();

		m_pCaster.m_szIDLEAnimName = _animName_IDLE_RedScreen;
		m_pCaster.m_pAnimations.Play(_animName_IDLE_RedScreen);

		return true;
	}

	protected override void Frame_Child()
	{
		if (_processing)
		{
			Field_PowerPlant pPlant = BattleFramework._Instance.m_pField as Field_PowerPlant;
			if (pPlant.IsProcessing_EscapeSeq() == false)
				SkillThrow();
		}
	}

	public override void SkillThrow_Child()
	{
		_processing = false;

		m_pCaster.m_szIDLEAnimName = _animName_IDLE_Default;
		m_pCaster.m_pAnimations.Play(_animName_IDLE_Default);

		//Field_PowerPlant pPlant = BattleFramework._Instance.m_pField as Field_PowerPlant;
		//pPlant.StopCastEscapeSequence();
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------