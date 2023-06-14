using UnityEngine;
using System.Collections;
using HT;

/////////////////////////////////////////
//---------------------------------------
public class BossDK_SoulResidue : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("SETTINGS")]
	public IActorBase m_pParent;

	//---------------------------------------
	public ParticleSystem m_pCreationEffect;
	float m_fCreationDelay;
	bool m_bInitialized = false;
	bool m_bAreaAlerted = false;

	//---------------------------------------
	public GameObject m_pSoul;
	public GameObject m_pSphere;
	public GameObject m_pLight;

	//---------------------------------------
	public SphereCollider m_pCollider;
	public ParticleSystem m_pParticle;

	public GameObject m_pExplosenEffect;
	public float m_fExplosenEffectLength;

	public int m_nDamage;
	public int m_nDrainHealing;
	public float m_fLifeTime;
	public float m_fMoveSpeed;

	//---------------------------------------
	[Header("ARCHIVEMENT")]
	[SerializeField]
	private string _drainLife_Archivename = null;

	//---------------------------------------
	readonly string m_szCallBuffName = "DRAIN_RESIDUE";

	//---------------------------------------
	bool m_bExploded = false;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start()
	{
		m_fCreationDelay = m_pCreationEffect.TotalSimulationTime() - 1.0f;
		BattleFramework._Instance.AddAutoTargetObject(gameObject);
	}

	// Update is called once per frame
	void Update()
	{
		if (m_bExploded)
			return;

		if (m_bInitialized)
		{
			m_fLifeTime -= HT.TimeUtils.GameTime;

			//-----
			IActorBase.ActorBuffEnable pBuff = m_pParent.FindEnabledActorBuff(m_szCallBuffName);
			if (pBuff != null)
			{
				//Vector3 vTargetPos = BossDK_SwordScript._Instance.m_pLantern.transform.position;
				Vector3 vTargetPos = m_pParent.transform.position;
				Vector3 vPos = transform.position;

				//vTargetPos.y = vPos.y;

				//-----
				float fDistance = Vector3.Distance(vTargetPos, vPos);
				float fMovement = m_fMoveSpeed * HT.TimeUtils.GameTime;

				if (2.0f > fDistance)
				{
					m_fLifeTime = -1.0f;
					m_pParent.OnDamaged(-m_nDrainHealing);
					if (string.IsNullOrEmpty(_drainLife_Archivename) == false)
					{
						Archives pArchives = ArchivementManager.Instance.FindArchive(_drainLife_Archivename);
						pArchives.Archive.OnArchiveCount(1);
					}
				}
				else
				{
					vPos = vTargetPos - vPos;
					vPos.Normalize();

					vPos *= fMovement;
					vPos += transform.position;

					transform.position = vPos;
				}
			}

			//-----
			if (m_fLifeTime < 0.0f)
				Release();

		}
		else
		{
			m_fCreationDelay -= HT.TimeUtils.GameTime;

			if (m_bAreaAlerted == false && m_fCreationDelay < 1.5f)
			{
				m_bAreaAlerted = true;

				BattleFramework._Instance.CreateAreaAlert(transform.position, 1.0f, m_fCreationDelay);
			}

			if (m_fCreationDelay < 0.0f)
			{
				m_bInitialized = true;

				m_pSoul.SetActive(true);
				m_pLight.SetActive(true);
			}
		}
	}

	void Release()
	{
		m_bExploded = true;

		m_pExplosenEffect.SetActive(true);

		//-----
		m_pSphere.SetActive(false);
		m_pLight.SetActive(false);

		ParticleSystem.EmissionModule pEmittor = m_pParticle.emission;
		pEmittor.enabled = false;

		//-----
		BattleFramework._Instance.RemoveAutoTargetObject(gameObject);
		HT.Utils.SafeDestroy(gameObject, m_fExplosenEffectLength);
	}

	//---------------------------------------
	void OnTriggerEnter(Collider pCollision)
	{
		if (m_bExploded)
			return;

		if (m_bInitialized == false)
			return;

		//-----
		bool bDestroy = false;
		IActorBase pActorBase = pCollision.GetComponent<IActorBase>();
		if (pActorBase != null)
		{
			bool bDamageEnable = true;
			if (pActorBase != null && pActorBase.GetActorType() == IActorBase.eActorType.eSpawn)
			{
				SpawnActor pSpawnAct = (SpawnActor)pActorBase;
				if (pSpawnAct._parentActor == m_pParent)
					bDamageEnable = false;
			}

			if (bDamageEnable)
			{
				if (pActorBase == m_pParent)
					return;

				pActorBase.OnDamaged(m_nDamage);
				bDestroy = true;
			}
		}

		//-----
		Projectile pProjectile = pCollision.GetComponent<Projectile>();
		if (pProjectile != null && pProjectile.m_pParent != m_pParent)
		{
			bDestroy = true;
			HT.Utils.SafeDestroy(pProjectile.gameObject);
		}

		//-----
		if (bDestroy)
			Release();
	}


	/////////////////////////////////////////
	//---------------------------------------
}
