using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class BossGL_Parts : GameObj_HitListener
{
	//---------------------------------------
	[Header("AUTO SETTING OBJS")]
	public GameObject _dividedPartsParent = null;
	public AIActor_Extend_GL _parentActorExtend = null;

	[Header("OBJECTS")]
	public Rigidbody _rigidBody = null;
	public BossGL_Parts _linkParts = null;

	//---------------------------------------
	[Header("SETTINGS")]
	public bool _isDamageEnabled = false;

	//---------------------------------------
	public void OnPartsDivid()
	{
		gameObject.SetActive(false);

		_rigidBody.gameObject.SetActive(true);
		_rigidBody.useGravity = true;
		_rigidBody.transform.localPosition = Vector3.zero;
		_rigidBody.transform.Rotate(Vector3.zero);

		_rigidBody.gameObject.transform.SetParent(_dividedPartsParent.transform);

		if (_parentActorExtend._dividParts_Effect != null)
		{
			ParticleSystem pParticle = HT.Utils.InstantiateFromPool(_parentActorExtend._dividParts_Effect);
			pParticle.transform.position = gameObject.transform.position;

			HT.Utils.SafeDestroy(pParticle.gameObject, pParticle.TotalSimulationTime());
		}

		HT.HTSoundManager.PlaySound(_parentActorExtend._divideParts_Sounds);
	}

	//---------------------------------------
	public override void OnHitByActor(IActorBase pActor, int nDamage)
	{
		if (IsEnabled() == false)
			return;

		if (pActor == _parentActorExtend.m_pActorBase)
			return;

		_parentActorExtend.OnPartsDamaged(this, nDamage);
	}

	//---------------------------------------
	public override bool IsEnabled()
	{
		if (_parentActorExtend == null)
			return false;

		if (_isDamageEnabled == false)
			return false;

		if (_parentActorExtend.CurrActivatedParts != this)
			return false;

		return true;
	}
}


/////////////////////////////////////////
//---------------------------------------