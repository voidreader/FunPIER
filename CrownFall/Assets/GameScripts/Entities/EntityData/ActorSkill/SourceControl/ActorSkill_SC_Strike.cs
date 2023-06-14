using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class ActorSkill_SC_Strike : ActorSkill
{
	//---------------------------------------
	[Header("SOURCE CONTROL INFO")]
	[SerializeField]
	private string _animName_FlyUp = null;
	[SerializeField]
	private string _animName_StrikeDown = null;
	[SerializeField]
	private string _animName_StandUp = null;
	[SerializeField]
	private ActorBuff _flyBuff = null;
	[SerializeField]
	private float _flyHeight = 30.0f;
	[SerializeField]
	private float _flyUpSpeedPow = 3.0f;

	[Header("POTION DROP")]
	[SerializeField]
	private Projectile _dropPotionProj = null;
	[SerializeField]
	private int _dropPotionCount = 8;
	[SerializeField]
	private int _dropPotionHitDamage = 1;
	[SerializeField]
	private float _dropPotionSpeed = 5.0f;
	[SerializeField]
	private float _dropPotionSize = 0.5f;
	[SerializeField]
	private float _dropPotionTerm = 0.3f;
	[SerializeField]
	private float _dropPotionSpread = 0.3f;

	[Header("STRIKE")]
	[SerializeField]
	private float _stikeRange = 7.5f;
	[SerializeField]
	private int _strikeDamage = 1;
	[SerializeField]
	private float _strikeDelay = 2.0f;
	[SerializeField]
	private float _strikeSpeedPow = 2.5f;
	[SerializeField]
	private ParticleSystem _strikeEndEffect = null;
	[SerializeField]
	private AudioClip _strikeEndSounds = null;

	[Header("ANIMATION CHANGE")]
	[SerializeField]
	private string _animName_IDLE_Default = null;
	[SerializeField]
	private string _animName_IDLE_Fly = null;

	//---------------------------------------
	private int _leastPotionCount = 0;
	private bool _skillProcessing = false;
	private float _potionDropDelay = 0.0f;

	private Vector3 _heightChangePos = Vector3.zero;

	private float _flyHeightTime = 0.0f;
	private float _flyHeightTime_Total = 0.0f;

	private float _strikeReady = 0.0f;
	private bool _strikeDown = false;
	private float _strikeDownTime = 0.0f;
	private float _strikeDownTime_Total = 0.0f;
	private Vector3 _strikeDownPos = Vector3.zero;

    //---------------------------------------
    private bool _isAIActor = false;

    //---------------------------------------
    public override void ResetAll()
	{
		base.ResetAll();

		_strikeDown = false;
		_strikeDownTime = 0.0f;
		_strikeDownTime_Total = 0.0f;

		_flyHeightTime = 0.0f;
		_flyHeightTime_Total = 0.0f;

		_skillProcessing = false;
	}

	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
		_strikeDown = false;
		_leastPotionCount = _dropPotionCount;

		_skillProcessing = true;
		m_pCaster.m_pRigidBody.useGravity = false;

		AIActor pActor = m_pCaster as AIActor;

        if(pActor != null)
        {
            pActor.ChasingTarget = BattleFramework._Instance.m_pPlayerActor.gameObject;
            _isAIActor = true;
        }
        else
            _isAIActor = false;

        _flyHeightTime = _flyHeightTime_Total = m_pCaster.SetAction(_animName_FlyUp);
		m_pCaster.SetActionReadyTime(float.MaxValue);
		
		m_pCaster.AddActorBuff(_flyBuff);

        //-----
        //if (_isAIActor)
        //    m_pCaster.m_szIDLEAnimName = _animName_IDLE_Fly;
		//else
		//	m_pCaster.SetActionReadyTime(float.MaxValue);

		//-----
		_potionDropDelay = _dropPotionTerm;

		_heightChangePos = m_pCaster.transform.position;
		_strikeReady = _strikeDelay;
		return true;
	}

	protected override void Frame_Child()
	{
		if (_skillProcessing)
		{
			if (_flyHeightTime > 0.0f)
			{
				_flyHeightTime -= HT.TimeUtils.GameTime;

				float fTimeRatio = 1.0f - (_flyHeightTime / _flyHeightTime_Total);
				fTimeRatio = Mathf.Pow(fTimeRatio, _flyUpSpeedPow);
				
				Vector3 vHeightPos = _heightChangePos;
				vHeightPos.y = _flyHeight;

				Vector3 vCurPos = Vector3.Lerp(_heightChangePos, vHeightPos, fTimeRatio);
				m_pCaster.transform.position = vCurPos;
			}
			else
            {
                if (_isAIActor == false)
                {
                    m_pCaster.SetAction(_animName_IDLE_Fly);
                    m_pCaster.SetActionReadyTime(float.MaxValue);
                }

                //-----
                if (_leastPotionCount > 0)
				{
					if (_potionDropDelay > 0.0f)
						_potionDropDelay -= HT.TimeUtils.GameTime;

					else
					{
						_potionDropDelay = _dropPotionTerm;
						--_leastPotionCount;

						//-----
						Projectile pNewProj = HT.Utils.Instantiate(_dropPotionProj);
						pNewProj.m_pSkill_Proj = null;
						pNewProj.m_pParent = m_pCaster;

						pNewProj.m_nProjectileDamage = _dropPotionHitDamage;
						pNewProj.m_fSpeed = _dropPotionSpeed;

						Vector3 vTargetPos = BattleFramework._Instance.m_pPlayerActor.transform.position;
						Vector3 vMoveVec = (vTargetPos - m_pCaster.transform.position).normalized;

                        if (_isAIActor == false)
                        {
                            Vector3 vActorPosRefresh = m_pCaster.transform.position;

                            float fMoveDist = m_pCaster.GetMoveSpeed() * HT.TimeUtils.GameTime;
                            vActorPosRefresh = vActorPosRefresh + (vMoveVec * fMoveDist);
                            vActorPosRefresh.y = _flyHeight;

                            m_pCaster.transform.position = vActorPosRefresh;
                        }

                        if (_dropPotionSpread > 0.0f)
							vMoveVec = HT.RandomUtils.GetRandomVector(vMoveVec, _dropPotionSpread);

						pNewProj.m_vMoveVector = vMoveVec;
						pNewProj.UpdateRotate();

						//-----
						float fColliderRadius = 1.0f;
						CapsuleCollider pCapsule = m_pCaster.GetComponent<CapsuleCollider>();
						if (pCapsule != null)
							fColliderRadius = pCapsule.radius;

						Vector3 vPos = pNewProj.transform.position;
						pNewProj.Init(vPos + m_pCaster.transform.position + (pNewProj.m_vMoveVector * fColliderRadius));

						//-----
						RaycastHit[] vHits = UnityEngine.Physics.RaycastAll(pNewProj.transform.position, vMoveVec);
						if (vHits != null && vHits.Length > 0)
						{
							for (int nInd = 0; nInd < vHits.Length; ++nInd)
							{
								if (vHits[nInd].collider.GetComponent<HT.MousePickableObject>() != null)
								{
									Vector3 vPoint = vHits[nInd].point;
									BattleFramework._Instance.CreateAreaAlert(vPoint, _dropPotionSize, 1.0f);
									break;
								}
							}
						}
					}
				}
				else
				{
					if (_strikeDown == false)
					{
						if (_strikeReady > 0.0f)
							_strikeReady -= HT.TimeUtils.GameTime;

						else
						{
							_strikeDown = true;

							_strikeDownTime = m_pCaster.SetAction(_animName_StrikeDown);
							m_pCaster.SetActionReadyTime(float.MaxValue);

							_strikeDownTime_Total = _strikeDownTime;

							Vector3 vTargetPos = BattleFramework._Instance.m_pPlayerActor.transform.position;
							BattleFramework._Instance.CreateAreaAlert(vTargetPos, _stikeRange, _strikeDownTime);

							_strikeDownPos = vTargetPos + ((m_pCaster.transform.position - vTargetPos).normalized * 0.5f);
							_heightChangePos = m_pCaster.transform.position;
						}
					}
					else
					{
						if (_strikeDownTime > 0.0f)
						{
							_strikeDownTime -= HT.TimeUtils.GameTime;
							float fTimeRatio = 1.0f - (_strikeDownTime / _strikeDownTime_Total);
							fTimeRatio = Mathf.Pow(fTimeRatio, _strikeSpeedPow);

							Vector3 vCurPos = Vector3.Lerp(_heightChangePos, _strikeDownPos, fTimeRatio);
							m_pCaster.transform.position = vCurPos;
						}
						else
						{
							m_pCaster.m_pRigidBody.velocity = Vector3.zero;

							SkillThrow();
						}
					}
				}
			}
		}
	}

	public override void SkillThrow_Child()
	{
		ParticleSystem pParticle = HT.Utils.Instantiate(_strikeEndEffect);
		pParticle.transform.position = m_pCaster.transform.position;

		HT.HTSoundManager.PlaySound(_strikeEndSounds);

		//-----
		if (_strikeDamage > 0)
		{
			IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
			float fDist = Vector3.Distance(m_pCaster.transform.position, pPlayer.transform.position);
			if (fDist <= _stikeRange)
				pPlayer.OnDamaged(_strikeDamage);
		}

		m_pCaster.m_szIDLEAnimName = _animName_IDLE_Default;
		m_pCaster.RemoveActorBuff(_flyBuff, false);

		m_pCaster.SetAction(_animName_StandUp);

        //-----
        _strikeDown = false;
		_strikeDownTime = 0.0f;
		_strikeDownTime_Total = 0.0f;

		_flyHeightTime = 0.0f;
		_flyHeightTime_Total = 0.0f;

		_skillProcessing = false;
		m_pCaster.m_pRigidBody.useGravity = true;

		AIActor pActor = m_pCaster as AIActor;
        if (pActor != null)
		    pActor.ChasingTarget = null;
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------