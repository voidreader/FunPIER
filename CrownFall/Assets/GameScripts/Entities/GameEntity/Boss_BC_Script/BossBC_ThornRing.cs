using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossBC_ThornRing : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private ParticleSystem _effect = null;

	[SerializeField]
	private float _delayTime = 1.5f;
	[SerializeField]
	private float _lifeTime = 4.0f;
	[SerializeField]
	private float _damageRange_Width = 0.5f;
	[SerializeField]
	private AudioClip _damageSound = null;
	[SerializeField]
	private int _damage = 1;

	//---------------------------------------
	private float _curRadius = 0.0f;

	//---------------------------------------
	public void Init(float fRange)
	{
		ParticleSystem.ShapeModule pShapeModule = _effect.shape;
		pShapeModule.radius = fRange;
		_curRadius = fRange;

		_effect.Play();

		Invoke("OnDamage", _delayTime);

		HT.Utils.SafeDestroy(gameObject, _lifeTime);
	}


	//---------------------------------------
	private void OnDamage()
	{
		if (_damageSound != null)
			HT.HTSoundManager.PlaySound(_damageSound);

		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
		float fDistance = Vector3.Distance(gameObject.transform.position, pPlayer.transform.position);
		if (fDistance >= (_curRadius - (_damageRange_Width * 0.5f)) && fDistance <= (_curRadius + (_damageRange_Width * 0.5f)))
		{
			pPlayer.OnDamaged(_damage);
		}
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------