using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class Field_Hall_WallHand_Trigger : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private BoxCollider _triggerArea = null;

	//---------------------------------------
	private bool _isPlayerIn = false;

	//---------------------------------------
	public float AreaX { get { return _triggerArea.size.x; } }
	public float AreaZ { get { return _triggerArea.size.z; } }

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == BattleFramework._Instance.m_pPlayerActor.gameObject)
			_isPlayerIn = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == BattleFramework._Instance.m_pPlayerActor.gameObject)
			_isPlayerIn = false;
	}

	//---------------------------------------
	public void OnDamage(int nValue)
	{
		if (_isPlayerIn)
			BattleFramework._Instance.m_pPlayerActor.OnDamaged(nValue);
	}
}


/////////////////////////////////////////
//---------------------------------------