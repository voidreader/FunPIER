using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossHAL_FallGuard : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private int _fallDamage = 1;

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		if (BattleFramework._Instance == null)
			return;

		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
		if (other.gameObject == pPlayer.gameObject)
		{
			pPlayer.OnDamaged(_fallDamage);

			Field pField = BattleFramework._Instance.m_pField;
			pPlayer.transform.position = pField.m_pPlayerStartPos.transform.position;
		}
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------