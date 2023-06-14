using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class Field_DamageRepeater : MonoBehaviour
{
	//---------------------------------------
	[Header("BASE INFO")]
	[SerializeField]
	private float _repeatTime = 1.0f;

	[Header("DAMAGE INFO")]
	[SerializeField]
	private int _damage = 1;
	[SerializeField]
	private float _damageRange = 1.0f;
	[SerializeField]
	private ActorBuff _target_AddBuff = null;
	[SerializeField]
	private ParticleSystem _effectWhenDamage = null;

	[Header("ALERT INFO")]
	[SerializeField]
	private bool _showAlertArea = true;
	[SerializeField]
	private float _showAlertArea_Time = 0.5f;
	[SerializeField]
	private ParticleSystem _effectWhenAlert = null;

	//---------------------------------------
	private void OnEnable()
	{
		StartCoroutine(RepeatEvent_Internal());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	//---------------------------------------
	private IEnumerator RepeatEvent_Internal()
	{
		while(true)
		{
			float fWaitDelay = _repeatTime;
			if ((_showAlertArea || _effectWhenAlert != null) && _showAlertArea_Time > 0.0f)
			{
				if (_showAlertArea)
					BattleFramework._Instance.CreateAreaAlert(gameObject.transform.position, _damageRange, _showAlertArea_Time);

				if (_effectWhenAlert != null)
				{
					ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_effectWhenAlert);
					pEffect.transform.position = gameObject.transform.position;

					HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
				}

				yield return new WaitForSeconds(_showAlertArea_Time);
				fWaitDelay -= _showAlertArea_Time;
			}

			//-----
			yield return new WaitForSeconds(fWaitDelay);

			//-----
			if (_effectWhenDamage != null)
			{
				ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_effectWhenDamage);
				pEffect.transform.position = gameObject.transform.position;

				HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
			}

			yield return new WaitForSeconds(0.25f);

			//-----
			if (_damageRange > 0.0f && _damage > 0)
			{
				IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
				float fDistance = Vector3.Distance(gameObject.transform.position, pPlayer.transform.position);
				if (fDistance <= _damageRange)
				{
					pPlayer.OnDamaged(_damage);

					if (_target_AddBuff != null)
						pPlayer.AddActorBuff(_target_AddBuff);
				}
			}
		}
	}
}


/////////////////////////////////////////
//---------------------------------------