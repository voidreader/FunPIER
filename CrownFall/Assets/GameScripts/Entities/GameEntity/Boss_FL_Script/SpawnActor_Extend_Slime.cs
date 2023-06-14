using System;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class SpawnActor_Extend_Slime : SpawnActor_Extend
{
	//---------------------------------------
	public static Action onPlayerHit = null;

	//---------------------------------------
	[Header("SLIME SETTINGS")]
	[SerializeField]
	private float _defScale = 1.0f;
	[SerializeField]
	private float _maxScale = 5.0f;
	[SerializeField]
	private float _minSpeedRate = 0.3f;
	[SerializeField]
	private float _maxSpeedRate = 0.9f;

	[Header("SLIME SETTINGS")]
	[SerializeField]
	private GameObject _scaleChangeTarget = null;

	//---------------------------------------
	private SpawnActor[] _subActors = null;
	private int _slimeLevel = 0;
	private const int _maxSlimeLevel = 3;

	//---------------------------------------
	private SpawnActor _parentActor = null;

	//---------------------------------------
	public override void Init()
	{
		_parentActor = GetComponent<SpawnActor>();
		onPlayerHit += StopMove;
	}

	public override void Frame()
	{

	}

	public override void Release()
	{
		if (_subActors == null)
			return;

		for (int nInd = 0; nInd < _subActors.Length; ++nInd)
		{
			Vector3 vNewPos = new Vector3(HT.RandomUtils.Range(0.0f, 1.0f), HT.RandomUtils.Range(0.0f, 1.0f), HT.RandomUtils.Range(0.0f, 1.0f));
			vNewPos = vNewPos.normalized * 0.5f;
			vNewPos += transform.position;

			_subActors[nInd].transform.position = GameFramework._Instance.GetPositionByPhysic(vNewPos);
			_subActors[nInd].gameObject.SetActive(true);
		}
		
		_subActors = null;
		onPlayerHit -= StopMove;
	}

	//---------------------------------------
	public void SetSlimeInfo(float poisonFilledRate)
	{
		float fSpeedRate = Mathf.Lerp(_minSpeedRate, _maxSpeedRate, poisonFilledRate);

		int nLevel = 1;
		if (poisonFilledRate > 0.66f)
			nLevel = 3;

		else if (poisonFilledRate > 0.33f)
			nLevel = 2;

		SetSlimeInfo(fSpeedRate, nLevel);
	}

	public void SetSlimeInfo(float fSpeedRate, int nLevel)
	{
		_slimeLevel = nLevel;

		float fBaseMoveSpeed = BattleFramework._Instance.m_pPlayerActor.m_pActorInfo.m_fBaseMoveSpeed;
		SpawnActor pActorBase = GetComponent<SpawnActor>();
		pActorBase._moveSpeed = fSpeedRate * fBaseMoveSpeed;
		
		float fScale = _defScale + Mathf.Max((((_maxScale - _defScale) / (_maxSlimeLevel - 1)) * (_slimeLevel - 1)), 0.0f);
		_scaleChangeTarget.transform.localScale = Vector3.one * fScale;

		if (nLevel > 1)
		{
			_subActors = new SpawnActor[2];
			for (int nInd = 0; nInd < _subActors.Length; ++nInd)
			{
				Field_PoisonFactory pField = BattleFramework._Instance.m_pField as Field_PoisonFactory;
				_subActors[nInd] = pField.SpawnNewSlime(transform.position, _slimeLevel - 1, fSpeedRate);

				_subActors[nInd].m_pAnimations.Stop();
				_subActors[nInd]._spawnAnimation = null;
				_subActors[nInd].gameObject.SetActive(false);
			}
		}
	}

	//---------------------------------------
	public override void OnCollisionToActor()
	{
		HT.Utils.SafeInvoke(onPlayerHit);
	}

	private void StopMove()
	{
		if (_parentActor != null)
			_parentActor.SetStopTime(1.5f);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------