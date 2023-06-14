using UnityEngine;
using System.Collections;
using HT;

/////////////////////////////////////////
//---------------------------------------
public class Field_DamageObject : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("DEFAULT INFOS")]
	public int m_nFieldDamage = 1;
	public ParticleSystem m_pDamageEffect;
	public GameObject _spawnObjWhendamage = null;
	public float m_fDamageEffectLifeTime = 0.1f;
	public AudioClip _damageSounds = null;
	public LightAutoDestroyer _lightWhenCreate = null;

	//---------------------------------------
	[Header("LIFE TIME")]
	public bool m_bDamageTimeLimit = false;
	public float m_fDamageLifeTime;

	//---------------------------------------
	[Header("DESTROY INFO")]
	public bool _destroyWhenDamage = false;
	public bool _destroyWhenDamage_SpawnEffects = true;
	public ParticleSystem m_pDestroyEffect = null;
	public LightAutoDestroyer _lightWhenExplode = null;
	public GameObject _spawnObjWhendestroy = null;
	public AudioClip _destroySounds = null;
	public float _destroySounds_Volume = 1.0f;

	[Header("DESTROY INFO - RANGE DAMAGE")]
	public bool _damageWhenDestroy = false;
	public float _destroyRange = 0.0f;
	public int _destroyRangeDamage = 0;
	public float _destroyCamShake = 0.0f;
    public bool _checkPhysicWhenDestroyDamage = false;

	//---------------------------------------
	bool m_bInCollision = false;
	ParticleSystem m_pCreatedParticle;
	float m_fEffectLifeTime;

	/////////////////////////////////////////
	//---------------------------------------
	[Header("ARCHIVES")]
	public string m_SzEffectiveArchiveName = null;
	public string _effectiveArchiveName_Splash = null;

	//---------------------------------------
	private float _leastDamageLifeTime = 0.0f;
	private bool _archiveCounted = false;


	/////////////////////////////////////////
	//---------------------------------------
	private void OnEnable()
	{
		ResetData();

		if (_lightWhenCreate != null && HTAfxPref.CheckPlatform(_lightWhenCreate._enablePlatform) && HTAfxPref.Quality >= _lightWhenExplode._requireQuality)
		{
			LightAutoDestroyer pNewObj = HT.Utils.InstantiateFromPool(_lightWhenCreate);
			pNewObj.transform.position = gameObject.transform.position;
		}
	}

	private void OnDisable()
	{
		ResetData();
	}

	private void ResetData()
	{
		_archiveCounted = false;
		_leastDamageLifeTime = m_fDamageLifeTime;
		m_bInCollision = false;
	}


	/////////////////////////////////////////
	//---------------------------------------
	void FixedUpdate()
	{
		if (BattleFramework._Instance == null)
			return;

		//-----
		if (m_bDamageTimeLimit)
		{
			float fPrevTime = _leastDamageLifeTime;
			_leastDamageLifeTime -= HT.TimeUtils.GameTime;

			//-----
			if (_damageWhenDestroy && fPrevTime > 1.0f && _leastDamageLifeTime < 1.0f)
				BattleFramework._Instance.CreateAreaAlert(gameObject.transform.position, _destroyRange, _leastDamageLifeTime);

			//-----
			if (_leastDamageLifeTime <= 0.0f)
			{
				m_bInCollision = false;

				Release(true);
				return;
			}
		}

		//-----
		if (m_bInCollision)
		{
			IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;

			if (m_nFieldDamage > 0)
				pPlayer.OnDamaged(m_nFieldDamage);

			//-----
			if (m_pDamageEffect != null && m_pCreatedParticle == null)
				m_pCreatedParticle = HT.Utils.InstantiateFromPool(m_pDamageEffect);

			m_fEffectLifeTime = m_fDamageEffectLifeTime;
			if (m_pCreatedParticle != null)
			{
				m_pCreatedParticle.transform.position = pPlayer.transform.position;
				m_pCreatedParticle.transform.rotation = pPlayer.transform.rotation;

				ParticleSystem.EmissionModule pEmittor = m_pCreatedParticle.emission;
				pEmittor.enabled = true;
			}

			if (_spawnObjWhendamage != null)
			{
				GameObject pNewObj = HT.Utils.Instantiate(_spawnObjWhendamage);
				pNewObj.transform.position = gameObject.transform.position;
			}

			HT.HTSoundManager.PlaySound(_damageSounds);

			//-----
			if (string.IsNullOrEmpty(m_SzEffectiveArchiveName) == false && _archiveCounted == false)
			{
				_archiveCounted = true;

				Archives pArchives = ArchivementManager.Instance.FindArchive(m_SzEffectiveArchiveName);
				pArchives.Archive.OnArchiveCount(1);
			}

			//-----
			if (_destroyWhenDamage)
			{
				Release(false);
				return;
			}

		}
		else
		{
			if (m_pCreatedParticle != null)
			{
				ParticleSystem.EmissionModule pEmittor = m_pCreatedParticle.emission;
				pEmittor.enabled = false;
			}
		}

		//-----
		if (m_fEffectLifeTime > 0.0f)
		{
			m_fEffectLifeTime -= HT.TimeUtils.GameTime;

			if (m_fEffectLifeTime <= 0.0f)
			{
				m_fEffectLifeTime = 0.0f;

				if (m_pCreatedParticle != null)
					HT.Utils.SafeDestroy(m_pCreatedParticle.gameObject, m_pCreatedParticle.TotalSimulationTime());

				m_pCreatedParticle = null;
			}
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void Release(bool bByTime)
	{
		if (bByTime || _destroyWhenDamage_SpawnEffects)
		{
			if (m_pDestroyEffect != null)
			{
				ParticleSystem pParticle = HT.Utils.InstantiateFromPool(m_pDestroyEffect);
				pParticle.transform.position = gameObject.transform.position;
				HT.Utils.SafeDestroy(pParticle.gameObject, pParticle.TotalSimulationTime());
			}

			if (_lightWhenExplode != null && HTAfxPref.CheckPlatform(_lightWhenExplode._enablePlatform) && HTAfxPref.Quality >= _lightWhenExplode._requireQuality)
			{
				LightAutoDestroyer pNewObj = HT.Utils.InstantiateFromPool(_lightWhenExplode);
				pNewObj.transform.position = gameObject.transform.position;
			}

			if (_spawnObjWhendestroy != null)
			{
				GameObject pNewObj = HT.Utils.Instantiate(_spawnObjWhendestroy);
				pNewObj.transform.position = gameObject.transform.position;
			}

			if (_damageWhenDestroy && _destroyRange > 0.0f)
			{
				if (_destroyRangeDamage > 0)
				{
                    IActorBase pPlayerActor = BattleFramework._Instance.m_pPlayerActor;
					Vector3 vPlayerPos = pPlayerActor.transform.position;

					float fDist = Vector3.Distance(vPlayerPos, gameObject.transform.position);
					if (fDist <= _destroyRange)
					{
                        bool bEnableDamage = true;
                        if (_checkPhysicWhenDestroyDamage)
                        {
                            Ray pRay = new Ray();
                            pRay.origin = gameObject.transform.position;
                            pRay.direction = vPlayerPos - pRay.origin;
                            float fDistance = pRay.direction.magnitude;

                            pRay.direction.Normalize();

                            RaycastHit pHitInfo = new RaycastHit();
                            if (Physics.Raycast(pRay, out pHitInfo, fDistance))
                            {
                                if (pHitInfo.collider.gameObject != pPlayerActor.gameObject)
                                    bEnableDamage = false;
                            }
                        }

                        if (bEnableDamage)
                        {
                            pPlayerActor.OnDamaged(_destroyRangeDamage);

                            if (string.IsNullOrEmpty(_effectiveArchiveName_Splash) == false)
                            {
                                Archives pArchives = ArchivementManager.Instance.FindArchive(_effectiveArchiveName_Splash);
                                pArchives.Archive.OnArchiveCount(1);
                            }
                        }
					}
				}

				if (_destroyCamShake > 0.0f)
					CameraManager._Instance.SetCameraShake(_destroyCamShake);
			}

			HT.HTSoundManager.PlaySound(_destroySounds, _destroySounds_Volume);
		}

		//-----
		if (m_pCreatedParticle != null)
			HT.Utils.SafeDestroy(m_pCreatedParticle.gameObject, m_pCreatedParticle.TotalSimulationTime());

		m_pCreatedParticle = null;

		//-----
		HT.Utils.SafeDestroy(gameObject);
	}


	/////////////////////////////////////////
	//---------------------------------------
	void OnCollisionEnter(Collision pCollision)
	{
		if (BattleFramework._Instance == null)
			return;

		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
		if (pCollision.gameObject == pPlayer.gameObject)
		{
			m_bInCollision = true;
		}
	}

	void OnCollisionExit(Collision pCollision)
	{
		if (BattleFramework._Instance == null)
			return;

		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
		if (pCollision.gameObject == pPlayer.gameObject)
		{
			m_bInCollision = false;
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	void OnTriggerEnter(Collider pCollision)
	{
		if (BattleFramework._Instance == null)
			return;

		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
		if (pCollision.gameObject == pPlayer.gameObject)
		{
			m_bInCollision = true;
		}
	}

	void OnTriggerExit(Collider pCollision)
	{
		if (BattleFramework._Instance == null)
			return;

		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
		if (pCollision.gameObject == pPlayer.gameObject)
		{
			m_bInCollision = false;
		}
	}
}
