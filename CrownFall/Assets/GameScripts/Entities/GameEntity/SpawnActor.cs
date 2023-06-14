using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;

public sealed class SpawnActor : IActorBase
{
	/////////////////////////////////////////
	//---------------------------------------
	public override IActorBase.eActorType GetActorType()
	{
		return IActorBase.eActorType.eSpawn;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public Action<SpawnActor> onDestroy = null;


	/////////////////////////////////////////
	//---------------------------------------
	[Header("SPAWN ACTOR INFO")]
	public string _spawnAnimation = null;
	public int _baseHealth = 1;
	public float _moveSpeed = 2.0f;
	public IActorBase _parentActor = null;
	public SpawnActor_Extend _actorExtend = null;

	public enum eAIType
	{
		eChasing = 0,
		eStatic,
		eByExtend,
	}

	public eAIType m_eAIType = eAIType.eChasing;

	//---------------------------------------
	public bool _isTriggerObject = false;

	//---------------------------------------
	[Header("ATTACK INFO")]
	public int _attackDamage = 1;
	public bool _destroyWhenAttack = false;

	//---------------------------------------
	[Header("DESTROY INFO")]
	public ParticleSystem m_pInst_ExplodeParticle;
	
	public AudioClip _explosionSound = null;
	public ISkillObject m_pSpawnObjectWhenExplode;

	//---------------------------------------
	private float _moveTimeDelay = 0.0f;


	/////////////////////////////////////////
	//---------------------------------------
	private int _currHP = 1;


	/////////////////////////////////////////
	//---------------------------------------
	private void Start()
	{
		if (BattleFramework._Instance == null)
			return;

		Init();

		if (_actorExtend != null)
			_actorExtend.Init();

		if (string.IsNullOrEmpty(_spawnAnimation) == false)
			SetAction(_spawnAnimation);

		BattleFramework._Instance.AddAutoTargetObject(gameObject);
	}

	private void Update()
	{
		if (BattleFramework._Instance == null)
			return;

		Frame();

		if (_actorExtend != null)
			_actorExtend.Frame();
	}

	private void OnDestroy()
	{
		if (BattleFramework._Instance == null)
			return;

		Release();

		if (_actorExtend != null)
			_actorExtend.Release();

		BattleFramework._Instance.RemoveAutoTargetObject(gameObject);
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override void Init()
	{
		base.Init();
		_currHP = _baseHealth;
	}

	//---------------------------------------
	public override void Frame()
	{
		//-----

		//-----
		switch (m_eAIType)
		{
			case eAIType.eStatic:
				{
					m_vViewVector = Vector3.zero;
					m_vMoveVector = Vector3.zero;
				}
				break;

			case eAIType.eChasing:
				{
					if (_moveTimeDelay <= 0.0f)
					{
						Vector3 vPlayerPos = BattleFramework._Instance.m_pPlayerActor.transform.position;
						vPlayerPos.y = 0.0f;

						Vector3 vActorPos = gameObject.transform.position;
						vActorPos.y = 0.0f;

						if (Vector3.Distance(vPlayerPos, vActorPos) > 0.5f)
						{
							Vector3 vMoveVector = vPlayerPos - vActorPos;
							vMoveVector.Normalize();

							m_vViewVector = vMoveVector;
							m_vMoveVector = vMoveVector;
						}
						else
							m_vMoveVector = Vector3.zero;
					}
					else
					{
						m_vMoveVector = Vector3.zero;
						m_pRigidBody.velocity = Vector3.zero;

						_moveTimeDelay -= HT.TimeUtils.GameTime;
					}
				}
				break;
		}

		//-----
		base.Frame();
	}

	//---------------------------------------
	public override void Release()
	{
		if (HT.HTFramework.GameClosed == false && BattleFramework._Instance.GameEnd == false)
		{
			HT.HTSoundManager.PlaySound(_explosionSound);

			//-----
			if (m_pInst_ExplodeParticle != null)
			{
				ParticleSystem pExpPart = HT.Utils.InstantiateFromPool(m_pInst_ExplodeParticle);
				pExpPart.transform.position = gameObject.transform.position;
				//pExpPart.transform.rotation = gameObject.transform.rotation;
				pExpPart.transform.localScale = gameObject.transform.localScale;

				HT.Utils.SafeDestroy(pExpPart.gameObject, pExpPart.TotalSimulationTime());
			}

			if (m_pSpawnObjectWhenExplode != null)
			{
				ISkillObject pNewObj = HT.Utils.Instantiate(m_pSpawnObjectWhenExplode);
				Vector3 vPos = gameObject.transform.position;
				vPos.y = 0.0f;
				pNewObj.transform.position = vPos;

				if (pNewObj.m_bInheritRotation)
					pNewObj.transform.rotation = gameObject.transform.rotation;

				pNewObj.m_pCaster = BattleFramework._Instance.m_pEnemyActor;
			}

			//-----
			Utils.SafeInvoke(onDestroy, this);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override int GetCurrHP()
	{
		return _currHP;
	}

	public override float GetMoveSpeed()
	{
		return _moveSpeed;
	}

	public void SetStopTime(float fSet)
	{
		_moveTimeDelay = fSet;
	}


	/////////////////////////////////////////
	//---------------------------------------
	void OnCollisionEnter(Collision pCollision)
	{
		if (_isTriggerObject == false)
		{
			IActorBase pActor = pCollision.gameObject.GetComponent<IActorBase>();
			if (pActor != null && pActor.GetActorType() != eActorType.eSpawn)
			{
				if (pActor != _parentActor)
				{
					pActor.OnDamaged(_attackDamage);

					if (_destroyWhenAttack)
						HT.Utils.SafeDestroy(gameObject);
				}

				if (_actorExtend != null)
					_actorExtend.OnCollisionToActor();
			}
		}
	}

	void OnTriggerEnter(Collider pCollision)
	{
		if (_isTriggerObject)
		{
			IActorBase pActor = pCollision.gameObject.GetComponent<IActorBase>();
			if (pActor != null && pActor.GetActorType() != eActorType.eSpawn)
			{
				if (pActor != _parentActor)
				{
					pActor.OnDamaged(_attackDamage);

					if (_destroyWhenAttack)
						HT.Utils.SafeDestroy(gameObject);
				}

				if (_actorExtend != null)
					_actorExtend.OnCollisionToActor();
			}
			else
			{
				Projectile pProj = pCollision.gameObject.GetComponent<Projectile>();
				if (pProj != null && pProj.m_pParent == BattleFramework._Instance.m_pPlayerActor)
					OnDamaged(pProj.m_nProjectileDamage);
			}
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void SetSpawnActor(IActorBase parentActor)
	{
		_parentActor = parentActor;
	}

	protected override void OnDamage_Calculated(int nDamage)
	{
		_currHP -= nDamage;
		if (_currHP <= 0)
			HT.Utils.SafeDestroy(gameObject);

		if (_actorExtend != null)
			_actorExtend.OnEvent_Damage(nDamage);
	}


	/////////////////////////////////////////
	//---------------------------------------
}
