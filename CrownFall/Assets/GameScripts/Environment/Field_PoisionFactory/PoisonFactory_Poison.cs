using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class PoisonFactory_Poison : MonoBehaviour
{
	//---------------------------------------
	[Header("BASE SETTING")]
	[SerializeField]
	private ActorBuff _debuff = null;
	[SerializeField]
	private bool _enabled = true;
	[SerializeField]
	private float _radiusOffset = 0.0f;
	[SerializeField]
	private float _activateReady = 0.0f;

	private Collider _collider = null;
	private ControllableActor _enterActor = null;

	[Header("AUTO DESTROY")]
	[SerializeField]
	private float _autoDestroyTime = 0.0f;

	//---------------------------------------
	private float _activateTime = 0.0f;

	//---------------------------------------
	private void Awake()
	{
		_collider = GetComponent<Collider>();
		SetEnable(_enabled);

		if (_autoDestroyTime > 0.0f)
			HT.Utils.SafeDestroy(gameObject, _autoDestroyTime);
	}

	private void Update()
	{
		if (_activateReady > 0.0f)
			_activateTime += HT.TimeUtils.GameTime;

		if (_activateTime < _activateReady)
			return;

		if (_enterActor != null)
		{
			float fTime = HT.TimeUtils.GameTime * GameDefine.fBoss_PG_PoisonDebuffRatio;
			if (_enterActor.AddEnableActorBuffTime(_debuff, fTime, true) == false)
				_enterActor.AddActorBuff(_debuff, fTime);
		}
	}

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		ControllableActor pCtrlActor = other.gameObject.GetComponent<ControllableActor>();
		if (pCtrlActor != null)
			_enterActor = pCtrlActor;
	}

	private void OnTriggerExit(Collider other)
	{
		_enterActor = null;
	}

	//---------------------------------------
	public void SetEnable(bool bSet)
	{
		_enabled = bSet;
		_collider.enabled = bSet;

		if (bSet == false)
			_enterActor = null;
	}

	public float GetRadius()
	{
		float fScale = _collider.transform.lossyScale.x;
		SphereCollider pSphere = _collider as SphereCollider;
		if (pSphere != null)
			return (pSphere.radius * fScale) + _radiusOffset;

		CapsuleCollider pCapsule = _collider as CapsuleCollider;
		if (pCapsule != null)
			return (pCapsule.radius * fScale) + _radiusOffset;

		return 0.0f;
	}
}


/////////////////////////////////////////
//---------------------------------------