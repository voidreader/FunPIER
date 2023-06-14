using System;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class Field_BuffObject : MonoBehaviour
{
	//---------------------------------------
	[Header("DEFAULTS")]
	[SerializeField]
	private float _enableTime = -1.0f;
	[SerializeField]
	private ActorBuff _buff = null;
	[SerializeField]
	private bool _enableOnlyInside = false;
	[SerializeField]
	private bool _playerEnabled = true;
	[SerializeField]
	private bool _enenmyEnabled = false;

	[Header("REQUIREMENT")]
	[SerializeField]
	private eGameDifficulty _requireDifficulty = eGameDifficulty.eEasy;
	[SerializeField]
	private ActorBuff[] _requireBuff = null;
	[SerializeField]
	private ActorBuff[] _restrictBuff = null;

	//---------------------------------------
	private List<IActorBase> _actorList = new List<IActorBase>();
	private float _leastTime = -1.0f;

	//---------------------------------------
	private void Start()
	{
		if (_requireDifficulty > GameFramework._Instance.m_pPlayerData.m_eDifficulty)
			enabled = false;

		_leastTime = _enableTime;
	}

	private void Update()
	{
		if (_leastTime > 0.0f)
		{
			_leastTime -= HT.TimeUtils.GameTime;
			if (_leastTime <= 0.0f)
			{
				for (int nInd = 0; nInd < _actorList.Count; ++nInd)
				{
					if (_enableOnlyInside)
						_actorList[nInd].RemoveActorBuff(_buff, false);

					_actorList.Remove(_actorList[nInd]);
				}

				this.enabled = false;
			}
		}
	}

	private void FixedUpdate()
	{
		for (int nInd = 0; nInd < _actorList.Count; ++nInd)
		{
			if (CheckEnable(_actorList[nInd]))
				_actorList[nInd].AddActorBuff(_buff);
			else
				_actorList[nInd].RemoveActorBuff(_buff, false);
		}
	}

	//---------------------------------------
	private void OnDisable()
	{
		for (int nInd = 0; nInd < _actorList.Count; ++nInd)
			_actorList[nInd].RemoveActorBuff(_buff, false);

		_actorList.Clear();
	}

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		IActorBase pActor = null;
		if (_playerEnabled)
		{
			if (other.gameObject == BattleFramework._Instance.m_pPlayerActor.gameObject)
				pActor = BattleFramework._Instance.m_pPlayerActor;
		}

		if (_enenmyEnabled)
		{
			if (other.gameObject == BattleFramework._Instance.m_pEnemyActor.gameObject)
				pActor = BattleFramework._Instance.m_pEnemyActor;
		}

		if (pActor != null)
			_actorList.Add(pActor);
	}

	private void OnTriggerExit(Collider other)
	{
		IActorBase pActor = null;
		if (_playerEnabled)
		{
			if (other.gameObject == BattleFramework._Instance.m_pPlayerActor.gameObject)
				pActor = BattleFramework._Instance.m_pPlayerActor;
		}

		if (_enenmyEnabled)
		{
			if (other.gameObject == BattleFramework._Instance.m_pEnemyActor.gameObject)
				pActor = BattleFramework._Instance.m_pEnemyActor;
		}

		if (pActor != null && _actorList.Contains(pActor))
		{
			if (_enableOnlyInside)
				pActor.RemoveActorBuff(_buff, false);

			_actorList.Remove(pActor);
		}
	}

	//---------------------------------------
	private bool CheckEnable(IActorBase pActor)
	{
		if (pActor == null)
			return false;

		if (_requireBuff != null && _requireBuff.Length > 0)
		{
			for (int nInd = 0; nInd < _requireBuff.Length; ++nInd)
				if (pActor.FindEnabledActorBuff(_requireBuff[nInd]) == null)
					return false;
		}

		if (_restrictBuff != null && _restrictBuff.Length > 0)
		{
			for (int nInd = 0; nInd < _restrictBuff.Length; ++nInd)
				if (pActor.FindEnabledActorBuff(_restrictBuff[nInd]) != null)
					return false;
		}

		return true;
	}
}


/////////////////////////////////////////
//---------------------------------------