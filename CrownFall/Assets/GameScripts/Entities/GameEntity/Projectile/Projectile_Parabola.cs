using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class Projectile_Parabola : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("BASE INFO")]
	public ActorSkill_Projectile_Parabola m_pSkill_Proj;
	public IActorBase m_pParent;

	//---------------------------------------
	[Header("PROJECTILE SETTING")]
	public Vector3 _destPosition = Vector3.zero;
	public float _speed = 1.0f;
	public float _flyHeight = 1.0f;

	[Header("DAMAGE TYPE")]
	public int _damage = 1;
	public ActorBuff _damageBuff = null;
	public float _splashDmgRange = 1.0f;
	public bool _showAlertArea = true;


	[Header("ARCHIVEMENT")]
	public string _archiveWhenHit = null;
	public string _archiveWhenSplash = null;

	//---------------------------------------
	public bool _explodeWhenCollision = true;

	//---------------------------------------
	[Header("EFFECTS")]
	public ParticleSystem m_pInst_TrailParticle;
	public ParticleSystem m_pInst_ExplodeParticle;

	//---------------------------------------
	public AudioClip _explosionSound = null;
	public bool m_bExpSoundPlayWhenLifeOver;

	public ISkillObject m_pSpawnObjectWhenExplode;
	public LightAutoDestroyer _lightWhenExplode = null;

	//---------------------------------------
	Vector3 _startPosition = Vector3.zero;
	float _totalMoveDistance = 0.0f;
	bool _explodeByCollision = false;
	ParticleSystem m_pTrailParticle;

	bool _createdExplodeEffect = false;

    //---------------------------------------
    private float _limitLifeTime = 0.0f;


    /////////////////////////////////////////
    //---------------------------------------
    private void Start()
	{
		Rigidbody pRigid = GetComponent<Rigidbody>();
		if (pRigid != null)
			pRigid.useGravity = false;
	}

	public void Init(Vector3 vPos, Vector3 vDest)
	{
		gameObject.transform.position = vPos;
		_startPosition = vPos;
		_destPosition = vDest;

		_totalMoveDistance = Vector3.Distance(_startPosition, _destPosition);
        _limitLifeTime = (_totalMoveDistance / _speed) + 0.1f;

        UpdateRotate();

		//-----
		if (m_pInst_TrailParticle != null)
			m_pTrailParticle = HT.Utils.InstantiateFromPool(m_pInst_TrailParticle);
		
		if (_showAlertArea && _splashDmgRange > 0.0f)
			BattleFramework._Instance.CreateAreaAlert(_destPosition, _splashDmgRange, _limitLifeTime);
	}

	void Update()
	{
		//-----
		Vector3 vNowPos = gameObject.transform.position;

		float fMoveDistance = _speed * HT.TimeUtils.GameTime;

		Vector3 vStart_IgnoreY = _startPosition;
		vStart_IgnoreY.y = 0.0f;
		Vector3 vNowPos_IgnoreY = vNowPos;
		vNowPos_IgnoreY.y = 0.0f;
		Vector3 vDestPos_IgnoreY = _destPosition;
		vDestPos_IgnoreY.y = 0.0f;

		//-----
		float fLeastDistance = Vector3.Distance(vNowPos_IgnoreY, vDestPos_IgnoreY);
		float fOriginDist = Vector3.Distance(vStart_IgnoreY, vDestPos_IgnoreY);
		float fCurDist = Vector3.Distance(vStart_IgnoreY, vNowPos_IgnoreY);
		if (fOriginDist <= fCurDist || fLeastDistance < fMoveDistance)
		{
			HT.Utils.SafeDestroy(gameObject);
			vNowPos = _destPosition;
		}
		else
		{
			Vector3 vMoveVec = GetMoveVector();
			vNowPos += vMoveVec * fMoveDistance;

			float fRatio = (_totalMoveDistance - fLeastDistance) / _totalMoveDistance;
			float fDestPosY = 0.0f;
			float fHeightOffset = 0.0f;
			float fHeightDiff = _destPosition.y - _startPosition.y;

			bool bJump_Up = (fRatio < 0.5f) ? true : false;
			if (bJump_Up)
			{
				fDestPosY = _startPosition.y;
				if (fHeightDiff > 0.0f)
					fHeightOffset = fHeightDiff;
			}
			else
			{
				fDestPosY = _destPosition.y;
				if (fHeightDiff < 0.0f)
					fHeightOffset = -fHeightDiff;
			}

			float fHeight = (Mathf.Sin(Mathf.PI * fRatio) * (_flyHeight + fHeightOffset)) + fDestPosY;
			vNowPos.y = fHeight;
		}

		gameObject.transform.position = vNowPos;

        //-----
        _limitLifeTime -= HT.TimeUtils.GameTime;
        if (_limitLifeTime <= 0.0f)
            HT.Utils.SafeDestroy(gameObject);

        //-----
        if (m_pTrailParticle != null)
		{
			m_pTrailParticle.transform.position = gameObject.transform.position;
			m_pTrailParticle.transform.rotation = gameObject.transform.rotation;
		}

		//-----
		UpdateRotate();
	}

	//---------------------------------------
	void OnDestroy()
	{
		if (HT.HTFramework.GameClosed)
			return;

		if (BattleFramework._Instance == null)
			return;

		if (BattleFramework._Instance.m_pPlayerActor == null)
			return;

		if (_splashDmgRange > 0.0f && (_damage > 0 || _damageBuff != null))
		{
			bool bSplashHit = false;

			IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
			if (m_pParent != pPlayer)
			{
				float fDist = Vector3.Distance(pPlayer.transform.position, gameObject.transform.position);
				if (fDist <= _splashDmgRange)
				{
					OnDamageEvent(pPlayer);
					bSplashHit = true;
				}
			}
			else
			{
				IActorBase[] vAllActors = FindObjectsOfType<IActorBase>();
				if (vAllActors != null)
				{
					for (int nInd = 0; nInd < vAllActors.Length; ++nInd)
					{
						if (vAllActors[nInd] == m_pParent)
							continue;

						bSplashHit = true;
						OnDamageEvent(vAllActors[nInd]);
					}
				}
			}

			if (bSplashHit && string.IsNullOrEmpty(_archiveWhenSplash) == false)
			{
				Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveWhenSplash);
				pArchives.Archive.OnArchiveCount(1);
			}
		}

		if (m_pSpawnObjectWhenExplode != null)
		{
			ISkillObject pNewObj = HT.Utils.Instantiate(m_pSpawnObjectWhenExplode);
			GameFramework._Instance.SetObjectPositionAndRotateByPhysic(pNewObj.gameObject, gameObject.transform.position);

			if (pNewObj.m_bInheritRotation)
				pNewObj.transform.rotation = pNewObj.transform.rotation * gameObject.transform.rotation;

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

		if (m_pInst_ExplodeParticle != null && _createdExplodeEffect == false)
		{
			_createdExplodeEffect = true;
			ParticleSystem pExpPart = HT.Utils.InstantiateFromPool(m_pInst_ExplodeParticle);
			pExpPart.transform.position = gameObject.transform.position;
			//GameFramework._Instance.SetObjectPositionAndRotateByPhysic(pExpPart.gameObject, gameObject.transform.position);

			HT.Utils.SafeDestroy(pExpPart.gameObject, pExpPart.TotalSimulationTime());
		}

		if (_explosionSound != null)
		{
			bool bPlaySound = true;
			if (m_bExpSoundPlayWhenLifeOver == false && _explodeByCollision == false)
				bPlaySound = false;

			if (bPlaySound)
				HT.HTSoundManager.PlaySound(_explosionSound);
		}
	}

	void OnCollisionEnter(Collision pCollision)
	{
		if (pCollision.gameObject != m_pParent.gameObject &&
			pCollision.gameObject.GetComponent<Projectile>() == null &&
			pCollision.gameObject.GetComponent<Projectile_Parabola>() == null &&
			pCollision.gameObject.GetComponent<ActorSkill_DynamicPolys_Instance>() == null)
		{
			//-----
			if (m_pInst_ExplodeParticle != null && _createdExplodeEffect == false)
			{
				_createdExplodeEffect = true;
				ParticleSystem pExpPart = HT.Utils.InstantiateFromPool(m_pInst_ExplodeParticle);
				pExpPart.transform.position = gameObject.transform.position;
				pExpPart.transform.rotation = gameObject.transform.rotation;

				HT.Utils.SafeDestroy(pExpPart.gameObject, pExpPart.TotalSimulationTime());
			}

			//-----
			IActorBase pActor = pCollision.gameObject.GetComponent<IActorBase>();
			if (pActor != null && (_damage > 0 || _damageBuff != null))
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
					OnDamageEvent(pActor);

					if (string.IsNullOrEmpty(_archiveWhenHit) == false)
					{
						Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveWhenHit);
						pArchives.Archive.OnArchiveCount(1);
					}
				}
			}

			//-----
			if (_explodeWhenCollision)
			{
				_explodeByCollision = true;
				HT.Utils.SafeDestroy(gameObject);
			}
			else
			{
				Collider pCollider = GetComponent<Collider>();
				pCollider.enabled = false;
			}
		}
	}

	private void OnDamageEvent(IActorBase pActor)
	{
		if (pActor == null)
			return;

		pActor.OnDamaged(_damage);

		if (_damageBuff != null)
			pActor.AddActorBuff(_damageBuff);
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void UpdateRotate()
	{
		Vector3 vMoveVec = GetMoveVector();

		Quaternion qQuat = gameObject.transform.rotation;
		qQuat.SetLookRotation(vMoveVec);
		gameObject.transform.rotation = qQuat;
	}

	public Vector3 GetMoveVector()
	{
		Vector3 vMoveVec = _destPosition - gameObject.transform.position;
		return vMoveVec.normalized;
	}
}


/////////////////////////////////////////
//---------------------------------------