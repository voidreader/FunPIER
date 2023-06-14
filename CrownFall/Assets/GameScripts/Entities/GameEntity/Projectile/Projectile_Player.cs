using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class Projectile_Player : Projectile
{
	//---------------------------------------
	public bool _isSpcAtkProjectile = false;

	//---------------------------------------
	public override void OnDamageToThing(int nDamage)
	{
		if (BattleFramework._Instance == null)
			return;

		ControllableActor pPlayer = BattleFramework._Instance.m_pPlayerActor as ControllableActor;
		if (_isSpcAtkProjectile)
		{
			pPlayer.OnSpecialAttakDamageToThing(nDamage);
			return;
		}

		pPlayer.OnProjectileDamageToThing(nDamage);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------