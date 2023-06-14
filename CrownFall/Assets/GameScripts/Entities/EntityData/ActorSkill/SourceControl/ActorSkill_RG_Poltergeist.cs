using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class ActorSkill_RG_Poltergeist : ActorSkill
{
	//---------------------------------------
	[SerializeField]
	private float _skillDuration = 13.0f;
	[SerializeField]
	private int _castRepeatCount = 3;
	[SerializeField]
	private ActorBuff _buffWhenPoltergeistEnd = null;

	//---------------------------------------
	private bool _processing = false;
	private float _leastDuration = 0.0f;
	private float _leastTerm = 0.0f;
	private float _totalDuration = 0.0f;

	//---------------------------------------
	public override void ResetAll()
	{
		_processing = false;
		_leastDuration = 0.0f;
		_leastTerm = 0.0f;
		_totalDuration = 0.0f;
	}

	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
		return true;
	}

	protected override void Frame_Child()
	{
		if (_processing)
		{
			float fDelta = HT.TimeUtils.GameTime;
			_leastDuration -= fDelta;
			_leastTerm -= fDelta;

			if (_leastTerm <= 0.0f)
			{
				Field_Hall pHall = BattleFramework._Instance.m_pField as Field_Hall;
				if (pHall != null)
					pHall.SetPoltergest();

				_leastTerm = _totalDuration / _castRepeatCount;
			}

			if (_leastDuration <= 0.0f)
			{
				_processing = false;
				_leastDuration = 0.0f;
				_leastTerm = 0.0f;
				_totalDuration = 0.0f;

				if (_buffWhenPoltergeistEnd != null)
					m_pCaster.AddActorBuff(_buffWhenPoltergeistEnd);
			}
		}
	}

	public override void SkillThrow_Child()
	{
		_processing = true;
		_totalDuration = _leastDuration = _skillDuration;
		_leastTerm = 0.0f;
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------