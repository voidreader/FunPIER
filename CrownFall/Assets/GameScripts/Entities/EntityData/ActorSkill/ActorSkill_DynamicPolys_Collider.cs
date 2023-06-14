using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class ActorSkill_DynamicPolys_Collider : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public ActorSkill_DynamicPolys_Instance m_pInstance;

	//---------------------------------------
	bool m_bDamaged = false;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}


	/////////////////////////////////////////
	//---------------------------------------
	//void OnCollisionEnter (Collision pCollision) {
	void OnTriggerEnter(Collider pCollision)
	{
		if (m_bDamaged)
			return;

		if (pCollision.gameObject != m_pInstance.m_pParent.gameObject &&
			pCollision.gameObject.GetComponent<Projectile>() == null &&
			pCollision.gameObject.GetComponent<ActorSkill_DynamicPolys_Instance>() == null)
		{
			//-----
			IActorBase pActor = pCollision.gameObject.GetComponent<IActorBase>();
			if (pActor != null && pActor.IsEssential() == false)
			{
				bool bDamageEnable = true;
				if (pActor.GetActorType() == IActorBase.eActorType.eSpawn)
				{
					SpawnActor pSpawnAct = (SpawnActor)pActor;
					if (pSpawnAct._parentActor == BattleFramework._Instance.m_pEnemyActor)
						bDamageEnable = false;
				}

				if (bDamageEnable)
				{
					m_bDamaged = true;

					pActor.OnDamaged(m_pInstance.m_pSkill_DynPolys.m_nCollisionDamage);

					//------
					Collider[] pColliders = GetComponents<Collider>();
					for (int nInd = 0; nInd < pColliders.Length; ++nInd)
						pColliders[nInd].enabled = false;
				}
			}
		}
	}

}
