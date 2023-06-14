using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class Projectile : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("BASE INFO")]
	public ActorSkill_Projectile m_pSkill_Proj;

	public IActorBase m_pParent;

	//---------------------------------------
	[Header("PROJECTILE SETTING")]
	public Vector3 m_vMoveVector;
	public float m_fSpeed;
	//public bool m_bIsHoming;
	public int m_nProjectileDamage;

	public bool m_bMoveByImpulse = false;

	//---------------------------------------
	public bool m_bExplodeWhenHit = true;

	[Header("ARCHIVEMENT")]
	public string _archiveWhenHit = null;

	//---------------------------------------
	[Header("EFFECTS")]
	public ParticleSystem m_pInst_TrailParticle;
	public ParticleSystem m_pInst_ExplodeParticle;

	//---------------------------------------
	public AudioClip _explosionSound = null;
    public float _explosionSound_Volume = 1.0f;
	public bool m_bExpSoundPlayWhenLifeOver;

	public ISkillObject m_pSpawnObjectWhenExplode;
	public LightAutoDestroyer _lightWhenExplode = null;


	/////////////////////////////////////////
	//---------------------------------------
	Rigidbody m_pRigidBody;

	//---------------------------------------
	ParticleSystem m_pTrailParticle;

	//---------------------------------------
	List<GameObject> _ignoreObjectList = null;
	private float _leastLifeTime = -1.0f;
	bool m_bIsCollide = false;


	/////////////////////////////////////////
	//---------------------------------------
	public void Init(Vector3 vPos, float lifeTime = -1.0f)
	{
		gameObject.transform.position = vPos;

		if (m_pInst_TrailParticle != null)
			m_pTrailParticle = HT.Utils.InstantiateFromPool(m_pInst_TrailParticle);

		//-----
		if (m_pRigidBody == null)
			m_pRigidBody = GetComponent<Rigidbody>();

		m_pRigidBody.maxDepenetrationVelocity = m_fSpeed * HT.TimeUtils.frame60DeltaTime;

		if (m_bMoveByImpulse)
			m_pRigidBody.velocity = m_vMoveVector * m_fSpeed * HT.TimeUtils.frame60DeltaTime;

		//-----
		_leastLifeTime = lifeTime;
	}
	
	void Update()
	{
		// Velocity가 이미 프레임 단위로 이동 속도를 제공하기 때문에, GameTime을 곱했을 경우
		// 프레임이 떨어지면 Velocity가 60프레임 일 때 보다 높아져 비정상적으로 빠르게 움직이게 된다.
		// 그러므로 GameTime을 곱하는게 아닌, 60프레임에서 맞춰놓은 속도로 움직이도록 1/60을 곱해준다.
		if (m_bMoveByImpulse == false)
			m_pRigidBody.velocity = m_vMoveVector * m_fSpeed * HT.TimeUtils.frame60DeltaTime;

		//-----
		UpdateRotate();

		//-----
		if (m_pTrailParticle != null)
		{
			m_pTrailParticle.transform.position = gameObject.transform.position;
			m_pTrailParticle.transform.rotation = gameObject.transform.rotation;
		}

		//-----
		if (_leastLifeTime > 0.0f)
		{
			_leastLifeTime -= TimeUtils.GameTime;
			if (_leastLifeTime <= 0.0f)
			{
				Release();
				HT.Utils.SafeDestroy(gameObject);
			}
		}
	}

	//---------------------------------------
	private void OnDestroy()
	{
		Release();
	}

	void Release()
	{
		if (_ignoreObjectList != null)
			_ignoreObjectList.Clear();

		if (HT.HTFramework.GameClosed)
			return;

		if (m_pSpawnObjectWhenExplode != null)
		{
			ISkillObject pNewObj = HT.Utils.Instantiate(m_pSpawnObjectWhenExplode);
			pNewObj.transform.position = GameFramework._Instance.GetPositionByPhysic(gameObject.transform.position);

			if (pNewObj.m_bInheritRotation)
				pNewObj.transform.rotation = gameObject.transform.rotation;

			pNewObj.m_pCaster = m_pParent;
		}

		if (_lightWhenExplode != null && HTAfxPref.CheckPlatform(_lightWhenExplode._enablePlatform) && HTAfxPref.Quality >= _lightWhenExplode._requireQuality)
		{
			LightAutoDestroyer pNewObj = HT.Utils.InstantiateFromPool(_lightWhenExplode);
			pNewObj.transform.position = gameObject.transform.position;
		}

		if (m_pTrailParticle != null)
		{
			if (m_pTrailParticle.isPlaying)
				m_pTrailParticle.Stop();

			float fLifeTime = m_pTrailParticle.main.duration;
			HT.Utils.SafeDestroy(m_pTrailParticle.gameObject, fLifeTime + 0.01f);
		}

		if (_explosionSound != null)
		{
			bool bPlaySound = true;
			if (m_bExpSoundPlayWhenLifeOver == false && m_bIsCollide == false)
				bPlaySound = false;

			if (bPlaySound)
				HT.HTSoundManager.PlaySound(_explosionSound, _explosionSound_Volume);
		}
	}

	void OnCollisionEnter(Collision pCollision)
	{
		OnCollision(pCollision.gameObject, pCollision.collider);
	}

	private void OnCollision(GameObject pObj, Collider other)
	{
		if (_ignoreObjectList != null && _ignoreObjectList.Count > 0)
		{
			for (int nInd = 0; nInd < _ignoreObjectList.Count; ++nInd)
			{
				if (_ignoreObjectList[nInd] == pObj)
					return;
			}
		}

		if (pObj != m_pParent.gameObject &&
			pObj.GetComponent<Projectile>() == null &&
			pObj.GetComponent<ActorSkill_DynamicPolys_Instance>() == null)
		{
			//-----
			if (m_pInst_ExplodeParticle != null)
			{
				ParticleSystem pExpPart = HT.Utils.InstantiateFromPool(m_pInst_ExplodeParticle);
				pExpPart.transform.position = gameObject.transform.position;
				pExpPart.transform.rotation = gameObject.transform.rotation;

				Release();
				HT.Utils.SafeDestroy(pExpPart.gameObject, pExpPart.TotalSimulationTime());
			}

			//-----
			bool bDamaged = false;
			IActorBase pActor = pObj.GetComponent<IActorBase>();
			if (pActor != null)
			{
				bool bDamageEnable = true;
				if (pActor.GetActorType() == IActorBase.eActorType.eSpawn)
				{
					SpawnActor pSpawnAct = (SpawnActor)pActor;
					if (pSpawnAct._parentActor == m_pParent)
						bDamageEnable = false;
				}

				if (bDamageEnable)
				{
					if (pActor.IsEssential() == false)
						OnDamageToThing(m_nProjectileDamage);

					pActor.OnDamaged(m_nProjectileDamage);
					bDamaged = true;
				}
			}

			GameObj_HitListener pHitListener = other.gameObject.GetComponent<GameObj_HitListener>();
			if (pHitListener != null)
			{
				if (pHitListener.IsEnabled())
					OnDamageToThing(m_nProjectileDamage);

				pHitListener.OnHitByActor(m_pParent, m_nProjectileDamage);
				bDamaged = true;
			}

			if (bDamaged && string.IsNullOrEmpty(_archiveWhenHit) == false)
			{
				Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveWhenHit);
				pArchives.Archive.OnArchiveCount(1);
			}

			//-----
			if (m_bExplodeWhenHit)
			{
				m_bIsCollide = true;

				Release();
				HT.Utils.SafeDestroy(gameObject);
			}
			else
			{
				Collider pCollider = GetComponent<Collider>();
				pCollider.enabled = false;
			}
		}
	}

	/////////////////////////////////////////
	//---------------------------------------
	public void UpdateRotate()
	{
		Quaternion qQuat = gameObject.transform.rotation;
		qQuat.SetLookRotation(m_vMoveVector);
		gameObject.transform.rotation = qQuat;
	}

	public void AddIgnorePhysList(GameObject obj)
	{
		if (_ignoreObjectList == null)
			_ignoreObjectList = new List<GameObject>();

		_ignoreObjectList.Add(obj);
	}

	public void AddIgnorePhysList(GameObject[] obj)
	{
		if (_ignoreObjectList == null)
			_ignoreObjectList = new List<GameObject>();

		for (int nInd = 0; nInd < obj.Length; ++nInd)
			_ignoreObjectList.Add(obj[nInd]);
	}

	//---------------------------------------
	public virtual void OnDamageToThing(int nDamage)
	{
	}
}
