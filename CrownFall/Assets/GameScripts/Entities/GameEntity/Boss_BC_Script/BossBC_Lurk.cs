using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossBC_Lurk : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private Transform _root = null;
	[SerializeField]
	private BoxCollider _boxCollider = null;
	[SerializeField]
	private AudioClip _createSound = null;

	//---------------------------------------
	[SerializeField]
	private ParticleSystem _thornEffect = null;
	[SerializeField]
	private float _thornSize = 0.8f;
	[SerializeField]
	private float _thornSpeed = 10.0f;
	[SerializeField]
	private float _thornTime = 5.0f;

	//---------------------------------------
	private Vector3 _startPosition = Vector3.zero;
	private float _waitTimeForDecrease = 0.0f;

	private bool _isComplete = false;

	//---------------------------------------
	public void Init()
	{
		_root.transform.localPosition = Vector3.zero;

		_boxCollider.size = new Vector3(_thornSize, 4.0f, _thornSize);
		_boxCollider.center = new Vector3(0.0f, 2.0f, 0.0f);

		//-----
		//Vector3 vDirection = (BattleFramework._Instance.m_pPlayerActor.transform.position - gameObject.transform.position).normalized;
		//gameObject.transform.forward = vDirection;

		_startPosition = Vector3.zero;

		_waitTimeForDecrease = _thornTime;
		_isComplete = false;

		//-----
		if (_createSound != null)
			HT.HTSoundManager.PlaySound(_createSound);
	}

	public void Update()
	{
		if (_isComplete == false)
		{
			if (_thornEffect.emission.enabled == false)
			{
				ParticleSystem.EmissionModule pEmiss = _thornEffect.emission;
				pEmiss.enabled = true;
			}

			Vector3 vMoveVec = _root.transform.localPosition + new Vector3(0.0f, 0.0f, _thornSpeed * HT.TimeUtils.GameTime);
			_root.transform.localPosition = vMoveVec;
		}
		else
		{
			if (_thornEffect.emission.enabled)
			{
				ParticleSystem.EmissionModule pEmiss = _thornEffect.emission;
				pEmiss.enabled = false;
			}
		}

		//-----
		if (Vector3.Distance(_root.transform.position, Vector3.zero) > 20.0f)
			_isComplete = true;

		//-----
		_waitTimeForDecrease -= HT.TimeUtils.GameTime;
		if (_waitTimeForDecrease <= 0.0f)
			_startPosition = Vector3.MoveTowards(_startPosition, _root.transform.localPosition, _thornSpeed * HT.TimeUtils.GameTime);

		_boxCollider.size = new Vector3(_thornSize, 2.0f, (_root.transform.localPosition.z - _startPosition.z) + (_thornSize * 0.5f));
		_boxCollider.center = new Vector3(_boxCollider.size.x * 0.5f, 1.0f, _startPosition.z + (_boxCollider.size.z * 0.5f));

		//-----
		if (Vector3.Distance(_startPosition, _root.transform.localPosition) < 0.1f)
			Release();
	}

	public void Release()
	{
		HT.Utils.SafeDestroy(gameObject);
	}

	//---------------------------------------
	public bool IsComplete()
	{
		return _isComplete;
	}

	public void SetComplete(bool bSet)
	{
		_isComplete = bSet;
	}

	//---------------------------------------
	public Vector3 GetCurPosition()
	{
		return _root.transform.position;
	}
}


/////////////////////////////////////////
//---------------------------------------