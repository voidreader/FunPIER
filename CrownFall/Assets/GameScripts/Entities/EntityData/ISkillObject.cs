using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class ISkillObject : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("SKILL OBJECT BASE INFO")]
	public IActorBase m_pCaster;


	/////////////////////////////////////////
	//---------------------------------------
	public bool m_bInheritRotation = true;

	//---------------------------------------
	public int m_nDamage = 1;
	public bool m_bCanMultiDamage = false;

	//---------------------------------------
	protected bool m_bDamaged = false;


	[Header("ARCHIVEMENT")]
	public string _archiveWhenHit = null;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start()
	{
		if (m_pCaster == null)
			m_pCaster = BattleFramework._Instance.m_pEnemyActor;

		Init();
	}

	// Update is called once per frame
	void Update()
	{
		Frame();
	}

	void OnDestroy()
	{
		Release();
	}


	/////////////////////////////////////////
	//---------------------------------------
	public virtual void Init()
	{
	}

	public virtual void Frame()
	{
	}

	public virtual void Release()
	{
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void OnTriggerEvent(GameObject pCollisionObj)
	{
		if (pCollisionObj == m_pCaster.gameObject)
			return;

		//-----
		IActorBase pActorBase = pCollisionObj.GetComponent<IActorBase>();
		if (pActorBase == null)
			return;

		bool bDamageEnable = true;
		if (pActorBase.GetActorType() == IActorBase.eActorType.eSpawn)
		{
			SpawnActor pSpawnAct = (SpawnActor)pActorBase;
			if (pSpawnAct._parentActor == m_pCaster)
				bDamageEnable = false;
		}

		if (bDamageEnable == false)
			return;

		if (m_bDamaged && m_bCanMultiDamage == false)
			return;

		m_bDamaged = true;

		//-----
		pActorBase.OnDamaged(m_nDamage);

		//-----
		if (string.IsNullOrEmpty(_archiveWhenHit) == false)
		{
			Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveWhenHit);
			pArchives.Archive.OnArchiveCount(1);
		}
	}
}
