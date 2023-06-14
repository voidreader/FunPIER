using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossRG_Ghost_ProjChecker : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private IActorBase _targetActor = null;

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		Projectile pProj = other.GetComponent<Projectile>();
		if (pProj == null)
			return;

		if (pProj.m_pParent != BattleFramework._Instance.m_pPlayerActor)
			return;

		_targetActor.OnDamaged(pProj.m_nProjectileDamage);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------