using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class AIActor_Extend_HAL : AIActor_Extend
{
	//---------------------------------------
	[Header("OBJECTS")]
	[SerializeField]
	private BossHAL_Monitor[] _monitors = null;

	//---------------------------------------
	public override void Extended_Init()
	{
	}

	public override void Extended_PostInit()
	{
	}

	public override void Extended_Frame()
	{
	}

	public override void Extended_Release()
	{
	}

	//---------------------------------------
	public void SetMonitorEyeChasing(bool bSet)
	{
		for (int nInd = 0; nInd < _monitors.Length; ++nInd)
			_monitors[nInd].SetEyeChasing(bSet);
	}

	//---------------------------------------
	public void Event_MonitorEyeChasingStart()
	{
		SetMonitorEyeChasing(true);
	}

	//---------------------------------------
	public override bool Extended_IsDamageEnable()
	{
		Field_PowerPlant pField = BattleFramework._Instance.m_pField as Field_PowerPlant;
		if (pField.IsEscapeSeq_InvincibleState())
			return false;

		return true;
	}
}


/////////////////////////////////////////
//---------------------------------------