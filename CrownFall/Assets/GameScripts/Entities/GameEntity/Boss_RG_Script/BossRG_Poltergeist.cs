using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossRG_Poltergeist : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private ParticleSystem _poltergeistEffect = null;

	//---------------------------------------
	private Rigidbody _rigidBody = null;
	private ParticleSystem _createdEffect = null;
	private IActorBase _player = null;

	//---------------------------------------
	const float _rigidBodyActiveTime = 5.0f;
	const float _requireVelocity = 3.0f;
	const int _collisionDamage = 1;

	private float _rigidBodyActiveTime_Least = 0.0f;

	//---------------------------------------
	private void Awake()
	{
		_rigidBody = GetComponent<Rigidbody>();

		_createdEffect = HT.Utils.Instantiate(_poltergeistEffect);
		_createdEffect.transform.SetParent(gameObject.transform);
		_createdEffect.transform.localPosition = Vector3.zero;
		_createdEffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
		_createdEffect.transform.localScale = Vector3.one;
	}

	private void Update()
	{
		if (_rigidBodyActiveTime_Least > 0.0f)
			_rigidBodyActiveTime_Least -= HT.TimeUtils.GameTime;
	}

	//---------------------------------------
	public void SetForce(float fVelocity)
	{
		if (gameObject.activeInHierarchy == false)
			return;

		StopAllCoroutines();
		StartCoroutine(SetForce_Internal(fVelocity));
	}

	//---------------------------------------
	private IEnumerator SetForce_Internal(float fVelocity)
	{
		if (_player == null)
			_player = BattleFramework._Instance.m_pPlayerActor;

		_createdEffect.Play(true);
		//BattleFramework._Instance.CreateAreaAlert(gameObject.transform.position, 1.0f, 1.0f);
		
		yield return new WaitForSeconds(1.25f);

		Vector3 vVector = (_player.transform.position - _rigidBody.transform.position).normalized;
		vVector.y = HT.RandomUtils.Range(0.1f, 1.0f);
		vVector.Normalize();

		_rigidBody.AddForce(vVector * fVelocity, ForceMode.Impulse);
		_rigidBodyActiveTime_Least = _rigidBodyActiveTime;
		
		//float fSpeedRatio = 0.0f;
		//Vector3 vCurDirection = HT.RandomUtils.Vector3(true, false, true);
		//
		//while(fSpeedRatio < 1.0f)
		//{
		//	fSpeedRatio += HT.TimeUtils.GameTime;
		//	if (fSpeedRatio > 1.0f)
		//		fSpeedRatio = 1.0f;
		//	
		//	_rigidBody.AddForce(vCurDirection * (_defaultVelocity * fSpeedRatio), ForceMode.Force);
		//	
		//	yield return new WaitForEndOfFrame();
		//}
	}

	//---------------------------------------
	private void OnCollisionEnter(Collision collision)
	{
		if (_rigidBody.velocity.sqrMagnitude < (_requireVelocity * _requireVelocity))
			return;

		if (_rigidBodyActiveTime_Least <= 0.0f)
			return;

		ControllableActor pPlayer = collision.gameObject.GetComponent<ControllableActor>();
		if (pPlayer != null)
			pPlayer.OnDamaged(_collisionDamage);
	}
}


/////////////////////////////////////////
//---------------------------------------