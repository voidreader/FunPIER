using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class ObjectActivator : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private GameObject _targetObject = null;

	//---------------------------------------
	private IActorBase _playerActor = null;

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		if (_targetObject.activeInHierarchy)
			return;

		if (_playerActor == null)
			_playerActor = BattleFramework._Instance.m_pPlayerActor;

		if (_playerActor != null && other.gameObject == _playerActor.gameObject)
			_targetObject.SetActive(true);
	}

	private void OnTriggerExit(Collider other)
	{
		if (_targetObject.activeInHierarchy == false)
			return;

		if (_playerActor == null)
			_playerActor = BattleFramework._Instance.m_pPlayerActor;
		
		if (_playerActor != null && other.gameObject == _playerActor.gameObject)
			_targetObject.SetActive(false);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------