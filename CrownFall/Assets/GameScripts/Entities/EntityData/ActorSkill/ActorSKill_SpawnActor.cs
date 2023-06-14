using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSKill_SpawnActor : ActorSkill
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("SPAWN ACTOR INFO")]
	[SerializeField]
	private SpawnActor _spawnActor_Instance = null;
	[SerializeField]
	private bool _inherit_Roation = false;
	[SerializeField]
	private float _positionOffset = 1.0f;
	[SerializeField]
	private int _maxSpawnActorCount = 1;

	//---------------------------------------
	private List<SpawnActor> _spawnActors = new List<SpawnActor>();


	/////////////////////////////////////////
	//---------------------------------------
	public override void ResetAll()
	{
		_spawnActors.Clear();
	}

	public override bool SkillCastReady_Child()
	{
		if (_spawnActors.Count >= _maxSpawnActorCount)
			return false;

		return true;
	}

	public override void SkillThrow_Child()
	{
		if (_spawnActor_Instance != null)
		{
			SpawnActor pActor = HT.Utils.Instantiate(_spawnActor_Instance);
			pActor._parentActor = m_pCaster;
			pActor.onDestroy = OnActorDestroy;

			_spawnActors.Add(pActor);

			//-----
			Transform pRootTransform = null;

			if (string.IsNullOrEmpty(_throwOffsetDummy) == false)
				pRootTransform = m_pCaster.FindDummyPivot(_throwOffsetDummy).transform;
			else
				pRootTransform = m_pCaster.transform;

			Vector3 vPosition = pRootTransform.position;
			vPosition = vPosition + (m_pCaster.m_vViewVector * _positionOffset);
			pActor.transform.position = vPosition;

			//-----
			if (_inherit_Roation)
				pActor.transform.rotation = m_pCaster.transform.rotation;

			//-----
			pActor.Init();
		}
		
		CallSkillObject_Throw();
	}


	//---------------------------------------
	public void OnActorDestroy(SpawnActor pActor)
	{
		_spawnActors.Remove(pActor);
	}
}


/////////////////////////////////////////
//---------------------------------------