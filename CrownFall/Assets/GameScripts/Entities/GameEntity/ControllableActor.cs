using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HT;

public sealed class ControllableActor : IActorBase
{
	/////////////////////////////////////////
	//---------------------------------------
	public override IActorBase.eActorType GetActorType()
	{
		return IActorBase.eActorType.eControllable;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public Action onCastDash = null;
	public Action onCastSpcAtk = null;
	public Action onDamageIgnored = null;


	/////////////////////////////////////////
	//---------------------------------------
	[Header("ATTACK INFO")]
	[SerializeField]
	private ParticleSystem _attackEffect = null;
	[SerializeField]
	private AttackChargeHelper _chargeHelper = null;
	[SerializeField]
	private ParticleSystem _particleWhenCritical = null;
	[SerializeField]
	private float _attackVectorLerpMaxDegree = 15.0f;

	//---------------------------------------
	[Header("SPECIAL ATTACK")]
	[SerializeField]
	private int _spcAtk_ChargeMax = 100;
	[SerializeField]
	private int _spcAtk_ChargeDef = 5;
	[SerializeField]
	private int _spcAtk_ChargeIncrease = 1;
	[SerializeField]
	private int _spcAtk_ChargeIncRequireDamage = 70;
    [SerializeField]
    private int _spcAtk_ChargeWhenDash = 5;
    [SerializeField]
    private int _spcAtk_Charge_WhenCounter = 50;
    [SerializeField]
	private LightAutoDestroyer _spcAtk_ShotLight = null;
	[SerializeField]
	private ActorBuff _spcAtk_ShotLight_ExtBuff = null;
	[SerializeField]
	private LightAutoDestroyer _spcAtk_ShotLight_ExtLight = null;
	[SerializeField]
	private AudioClip _spcAtk_Sound = null;
	[SerializeField]
	private AudioClip _spcAtk_Sound_FullChargeOn = null;

    [Header("SPECIAL ATTACK - FULL CHARGE SHOT")]
    [SerializeField]
	private int _spcAtk_FullCharge_ProjCnt = 5;
	[SerializeField]
	private int _spcAtk_FullCharge_DamagePerProj = 50;
    [SerializeField]
    private int _spcAtk_FullCharge_DamagePerProj_Last = 50;
    [SerializeField]
	private float _spcAtk_FullCharge_RepeatTime = 0.2f;
	[SerializeField]
	private float _spcAtk_FullCharge_RepeatTime_Last = 0.2f;
	[SerializeField]
	private float _spcAtk_FullCharge_CamShake = 2.0f;
	[SerializeField]
	private float _spcAtk_FullCharge_CamShake_Last = 4.0f;
	[SerializeField]
	private string _spcAtk_FullCharge_StartAnim = null;
	[SerializeField]
	private string _spcAtk_FullCharge_DuringAnim = null;
	[SerializeField]
	private string _spcAtk_FullCharge_EndAnim = null;
	[SerializeField]
	private ParticleSystem _spcAtk_FullCharge_Effect_Ready = null;
	[SerializeField]
	private ParticleSystem _spcAtk_FullCharge_Effect_Cast = null;
	[SerializeField]
	private ParticleSystem _spcAtk_FullCharge_Effect_Last = null;
	[SerializeField]
	private ParticleSystem _spcAtk_FullCharge_Effect_IsOn = null;

	private bool _spcAtk_FullCharge_Ready = false;
	private bool _spcAtk_FullCharge_Aiming = false;
	private bool _spcAtk_FullCharge_ReadyStateChanged = false;

	[Header("SPECIAL ATTACK - COUNTER ATTACK")]
    [SerializeField]
    private bool _spcAtk_Counter_Enabled = true;
    [SerializeField]
	private float _spcAtk_Counter_DashInvincibleTime = 0.1f;
	[SerializeField]
	private float _spcAtk_Counter_DashInvincibleTime_Mobile = 0.15f;
	[SerializeField]
	private float _spcAtk_Counter_DashInvincibleTime_Extend = 0.15f;
	[SerializeField]
	private ParticleSystem _spcAtk_Counter_DashInvincibleEffect = null;
	[SerializeField]
	private int _spcAtk_Counter_ChargePerProj = 10;
	[SerializeField]
	private int _spcAtk_Counter_DamagePerProj = 50;
	[SerializeField]
	private int _spcAtk_Counter_MaxProjCount = 3;
	[SerializeField]
	private float _spcAtk_Counter_RepeatTime = 0.1f;
	[SerializeField]
	private float _spcAtk_Counter_CamShake = 1.0f;

	[Header("SPECIAL ATTACK - STATE")]
	[SerializeField]
	private int _spcAtk_CurCharged = 0;
	[SerializeField]
	private int _spcAtk_CurChainedCnt = 0;
	[SerializeField]
	private float _spcAtk_Counter_LeastInvTime = 0.0f;
	public float SpcAtk_Counter_LeastInvTime { get { return _spcAtk_Counter_LeastInvTime; } }

	private bool _spcAtk_Counter_InvincibleEffected = false;

	//---------------------------------------
	public int SpcAtk_ChargeMax { get { return _spcAtk_ChargeMax; } }
	public int SpcAtk_CurCharged
	{
		get { return _spcAtk_CurCharged; }
		set { _spcAtk_CurCharged = value; }
	}


	/////////////////////////////////////////
	//---------------------------------------
	private bool _controlEnabled = true;

	private bool m_bDuringDash = false;
	private Action _onInteractionCancled = null;

	//---------------------------------------
	private bool _isStunning = false;

	//---------------------------------------
	private HTKey _key_Fire1 = null;
	private HTKey _key_Fire2 = null;
	private HTKey _key_SpcAtk = null;

	//---------------------------------------
	private GameObject _projectilePosDummy = null;
	private ActorSkill_Projectile m_pDefAtkSkillBook = null;
	private ActorSkill m_pDashSkillBook = null;

	//---------------------------------------
	private string _archiveWhenDash = null;
	public string ArchiveWhenDash
	{
		get { return _archiveWhenDash; }
		set { _archiveWhenDash = value; }
	}

	private string _archiveWhenIgnoreDamage = null;
	public string ArchiveWhenIgnoreDamage
	{
		get { return _archiveWhenIgnoreDamage; }
		set { _archiveWhenIgnoreDamage = value; }
	}

	public ActorSkill_Projectile DefAtkSkillBook { get { return m_pDefAtkSkillBook; } }
	public ActorSkill DashSkillBook { get { return m_pDashSkillBook; } }
	

	//---------------------------------------
	private Coroutine _specialAtk_Proc = null;
	private Vector3 _controllerLerpVector = Vector3.zero;


	//---------------------------------------
	private const string s_szSkillName_DefAtk = "DEFATK";
	private const string s_szSkillName_Dash = "DASH";

	private const string s_szChargingGuidAnimName = "Object_ChargingGuid";

	private const string s_szInteractionAnimName = "INTERACTION_{0}";

	private const string s_szAnumName_Stun = "STUN";

	private const string s_szDummyName_ShotPoint = "DUMMY_SHOTPOINT";


	//---------------------------------------
	private const string _joystick_SecondAxis_Horz = "Horizontal_Second";
	private const string _joystick_SecondAxis_Vert = "Vertical_Second";


	/////////////////////////////////////////
	//---------------------------------------
	private void OnPlayerAttack(Projectile pProj)
	{
		if (BattleFramework._Instance != null)
			HT.Utils.SafeInvoke(BattleFramework._Instance.onPlayerAttack, pProj);
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override void Init()
	{
		//-----
		ActorSkill pDefAtkSkill = FindSkillInfo(s_szSkillName_DefAtk);
		m_pDefAtkSkillBook = pDefAtkSkill.GetComponent<ActorSkill_Projectile>();
		m_pDefAtkSkillBook.onSkillThrow += OnPlayerAttack;

		//-----
		m_pDashSkillBook = FindSkillInfo(s_szSkillName_Dash);

		//-----
		_key_Fire1 = HTInputManager.Instance.GetKey(GameDefine.szKeyName_Fire1);
		_key_Fire2 = HTInputManager.Instance.GetKey(GameDefine.szKeyName_Fire2);

		_key_SpcAtk = HTInputManager.Instance.GetKey(GameDefine.szKeyName_SpcAtk);

		//-----
		base.Init();
	}

	//---------------------------------------
	public override void Frame()
	{
		if (TimeUtils.TimeScale <= 0.0f)
			return;

		if (_spcAtk_Counter_LeastInvTime > 0.0f)
			_spcAtk_Counter_LeastInvTime -= HT.TimeUtils.GameTime;

		//-----
		if (m_bActorIncapacitation || _controlEnabled == false)
		{
			m_vMoveVector = Vector3.zero;

			base.Frame();
			return;
		}

		//-----
		PlayerData pPlayerData = GameFramework._Instance.m_pPlayerData;

		bool bCheckSuccessDash = false;

		//-----
		HTInputManager pInputSys = HTInputManager.Instance;
		Vector3 vMoveVector = new Vector3(pInputSys.Horizontal, 0.0f, pInputSys.Vertical);
		Vector3 vViewVector = vMoveVector;

		if (pInputSys.JoystickConnected == false)// && HTAfxPref.IsMobilePlatform == false)
		{
			vViewVector = (pInputSys.MousePickingPos - gameObject.transform.position).normalized;
			vViewVector.y = 0.0f;
		}
		else
		{
			if (vViewVector.sqrMagnitude < float.Epsilon)
				vViewVector = gameObject.transform.right;
		}

		//-----
		if (_spcAtk_CurCharged >= _spcAtk_ChargeMax)
		{
			if (_spcAtk_FullCharge_ReadyStateChanged == false && _key_SpcAtk.IsDown)
			{
				_spcAtk_FullCharge_ReadyStateChanged = true;
				_spcAtk_FullCharge_Ready = !_spcAtk_FullCharge_Ready;
				_spcAtk_FullCharge_Aiming = false;

				//-----
				if (_spcAtk_FullCharge_Ready)
				{
					if (_spcAtk_Sound_FullChargeOn != null)
						HT.HTSoundManager.PlaySound(_spcAtk_Sound_FullChargeOn);
					
					if (_spcAtk_FullCharge_Effect_Ready != null)
					{
						ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_spcAtk_FullCharge_Effect_Ready);
						pEffect.transform.position = transform.position + (Vector3.up * 0.5f);

						HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
					}
				}
			}
			else if (_key_SpcAtk.IsUp || _key_SpcAtk.IsFree)
				_spcAtk_FullCharge_ReadyStateChanged = false;
		}
		else
		{
			_spcAtk_FullCharge_Ready = false;
			_spcAtk_FullCharge_Aiming = false;
		}

		if (_spcAtk_FullCharge_Effect_IsOn != null)
		{
			ParticleSystem.EmissionModule pEmission = _spcAtk_FullCharge_Effect_IsOn.emission;
			bool bPrevState = pEmission.enabled;
			pEmission.enabled = _spcAtk_FullCharge_Ready;

			if (_spcAtk_FullCharge_Ready && bPrevState != _spcAtk_FullCharge_Ready)
				_spcAtk_FullCharge_Effect_IsOn.Play();
		}

		//-----
		if (_isStunning == false)
		{
			//----- Skill key event
			Vector3 vPrevViewVector = m_vViewVector;
			if (m_eActorState != eActorState.eAction)
			{
				m_vViewVector = vViewVector;

				bool bSkillCasted = false;
				if (_key_Fire1.IsDown)
				{
					if (_spcAtk_FullCharge_Ready || _spcAtk_FullCharge_Aiming)
					{
						_spcAtk_FullCharge_Ready = false;
						_spcAtk_FullCharge_Aiming = true;

						SetAction(_spcAtk_FullCharge_StartAnim);
						SetActionReadyTime(float.PositiveInfinity);

                        _controllerLerpVector = Vector3.zero;

                        _chargeHelper.ShowJustAim();
					}
					else
					{
						if (CastSkill(m_pDefAtkSkillBook))
						{
							bSkillCasted = true;

							_controllerLerpVector = Vector3.zero;
							CreateChargingGuid(m_pDefAtkSkillBook.CalculateCastingTime());

							++pPlayerData.m_vBossKillRecord[pPlayerData.m_nKillRecordWriteIndex].nPlayerAttackCount;
						}
					}
				}

				//-----
				if (bSkillCasted)
					m_vViewVector = vPrevViewVector;
			}

			if (m_bDuringDash == false && _key_Fire2.IsDown)
			{
				if (m_pDashSkillBook.IsCastable(this))
				{
					// Canceling BowCharge
					StopCharging();

					// Canceling Interacting
					StopInteracting();

					// Canceling Special Attack
					StopSpcAtk();

					//-----
					Vector3 vPrevMove = m_vMoveVector;
					Vector3 vPrevView = m_vViewVector;

					bool bCasted = false;

					if (HTAfxPref.IsMobilePlatform == false && HTInputManager.Instance.JoystickConnected == false && GameFramework._Instance._option_dash_useMoveDir == false)
						m_vMoveVector = vMoveVector = (pInputSys.MousePickingPos - gameObject.transform.position).normalized;

					if (vMoveVector.sqrMagnitude > 0.0f)
					{
						m_vMoveVector = vMoveVector;
						m_vViewVector = vMoveVector;
					}
					else
					{
						m_vMoveVector = gameObject.transform.right;
						m_vViewVector = gameObject.transform.right;
					}

					if (CastSkill(m_pDashSkillBook))
					{
						// Dash Progress
						++pPlayerData.m_vBossKillRecord[pPlayerData.m_nKillRecordWriteIndex].nPlayerDashCount;

						float fDashDelayDecrease = pPlayerData.GetUpgrades(PlayerData.ePlayerUpgrades.eDashDelay) * GameDefine.fDashDelayDecreaseRatio;
						m_pDashSkillBook.SetSkillCooling(m_pDashSkillBook.GetSkillCooling() - fDashDelayDecrease);

						//-----
						bCasted = true;

						//-----
						if (string.IsNullOrEmpty(_archiveWhenDash) == false)
						{
							Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveWhenDash);
							pArchives.Archive.OnArchiveCount(1);
						}

                        //-----
                        _spcAtk_CurCharged += _spcAtk_ChargeWhenDash;
                        if (_spcAtk_CurCharged >= _spcAtk_ChargeMax)
                            _spcAtk_CurCharged = _spcAtk_ChargeMax;

                        _spcAtk_Counter_InvincibleEffected = false;

						_spcAtk_Counter_LeastInvTime = _spcAtk_Counter_DashInvincibleTime;
						if (HT.HTAfxPref.IsMobilePlatform)
							_spcAtk_Counter_LeastInvTime = _spcAtk_Counter_DashInvincibleTime_Mobile;

						Utils.SafeInvoke(onCastDash);
					}

					if (bCasted == false)
					{
						m_vMoveVector = vPrevMove;
						m_vViewVector = vPrevView;
					}
				}
			}

			//----- Action state update
			switch (m_eActorState)
			{
				case eActorState.eIdle:
				case eActorState.eMove:
					{
						_onInteractionCancled = null;

						if (HTInputManager.Instance.JoystickConnected)
						{
							if (Mathf.Abs(vMoveVector.x) < 0.1f)
								vMoveVector.x = 0.0f;

							if (Mathf.Abs(vMoveVector.z) < 0.1f)
								vMoveVector.z = 0.0f;

							vMoveVector.Normalize();
							vMoveVector.x = Mathf.Clamp(vMoveVector.x * 2.0f, -1.0f, 1.0f);
							vMoveVector.z = Mathf.Clamp(vMoveVector.z * 2.0f, -1.0f, 1.0f);
							vMoveVector.y = 0.0f;
						}

						m_vMoveVector = vMoveVector;
					}
					break;

				case eActorState.eAction:
					if (_chargeHelper.IsCharging || _spcAtk_FullCharge_Aiming)
                    {
                        if (_spcAtk_FullCharge_Aiming)
                        {
                            m_vMoveVector = Vector3.zero;
                            m_pRigidBody.velocity = Vector3.zero;
                        }

                        if (_key_Fire1.IsUp)
						{
							if (_spcAtk_FullCharge_Aiming)
							{                                
								_spcAtk_FullCharge_Aiming = false;
								_chargeHelper.StopChargeAnim();

								if (_spcAtk_CurCharged >= _spcAtk_ChargeMax)
									_specialAtk_Proc = StartCoroutine(OnSpcAtk_FullCharge_Internal());

								else
								{
									SetAction(m_szIDLEAnimName);
									SetActionReadyTime(0.0f);

									SetActorState(eActorState.eIdle);
								}
							}
							else
							{
								if (m_pDefAtkSkillBook.m_bCastYet == false)
									DoShotDefaultAttack();
							}
						}
						else
						{
							if (pInputSys.JoystickConnected || HTAfxPref.IsMobilePlatform)
							{
								Vector3 vOriginalVieVector = vViewVector;

								//-----
								if (pInputSys.JoystickConnected)
								{
									if (GameFramework._Instance._option_aim_useRightAnalog)
									{
										vViewVector.x = Input.GetAxis(_joystick_SecondAxis_Horz);
										vViewVector.z = Input.GetAxis(_joystick_SecondAxis_Vert);
									}
									else
									{
										if (vMoveVector.sqrMagnitude > 0.0f)
											vViewVector = vMoveVector;
										else
											vViewVector = gameObject.transform.right;
									}

									vViewVector.y = 0.0f;
									vViewVector.Normalize();
								}

								//-----
								if (_controllerLerpVector.sqrMagnitude > 0.0f)
								{
									_controllerLerpVector = vViewVector;

									float fVectorDot = Vector3.Dot(m_vViewVector, _controllerLerpVector);
									if (360.0f - (fVectorDot + 1.0f) * 180.0f < _attackVectorLerpMaxDegree)
									{
										float fLerpRatio = TimeUtils.GameTime * 3.33f;
										m_vViewVector.x = Mathf.Lerp(m_vViewVector.x, _controllerLerpVector.x, fLerpRatio);
										m_vViewVector.z = Mathf.Lerp(m_vViewVector.z, _controllerLerpVector.z, fLerpRatio);
									}
									else
										m_vViewVector = _controllerLerpVector;

									m_vViewVector.y = 0.0f;
								}
								else
								{
									_controllerLerpVector = vViewVector;

									m_vViewVector.x = vViewVector.x;
									m_vViewVector.z = vViewVector.z;
									m_vViewVector.y = 0.0f;
								}

								//-----
								vViewVector = vOriginalVieVector;
							}
							else
								m_vViewVector = vViewVector;
						}
					}
					else if (FindEnabledActorBuff("PLAYER_DASH") != null)
					{
						m_vMoveVector = m_vViewVector;

						bCheckSuccessDash = true;
						m_bDuringDash = true;
					}
					break;
			}

			//-----
		}

		//----- 
		if (m_bDuringDash && bCheckSuccessDash == false)
		{
			m_bDuringDash = false;
			m_vMoveVector = m_vViewVector;
		}

		//-----
		base.Frame();
	}

	//---------------------------------------
	public override void Release()
	{
		//-----
		m_pDefAtkSkillBook.onSkillThrow -= OnPlayerAttack;

		//-----
		base.Release();
	}

	//---------------------------------------
	void OnCollisionEnter(Collision pCollision)
	{
		if (BattleFramework._Instance == null)
			return;

		if (IsEssential())
			return;

		int nDefDamage = 1;
		AIActor pEnemyActor = BattleFramework._Instance.m_pEnemyActor as AIActor;
		if (pCollision.gameObject == pEnemyActor.gameObject)
		{
			if (pEnemyActor._damageWhenCollide)
				OnDamaged(nDefDamage);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override void OnBattleStart()
	{
		base.OnBattleStart();
	}

	public override void OnBattleEnd(bool bPlayerWin)
	{
		base.OnBattleEnd(bPlayerWin);

		//-----
		StopCharging();
		StopSpcAtk();
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override bool CastSkill(ActorSkill pSkill)
	{
		if (pSkill.SkillCast(this, null))
		{
			m_vMoveVector = Vector3.zero;
			return true;
		}

		return false;
	}

	public bool DoInteraction(float fTime, Action onInteractionCancel)
	{
		if (GetActorState() == eActorState.eAction)
			return false;

		_onInteractionCancled = onInteractionCancel;
		int nTime = (int)(fTime * 10.0f);
		SetAction(string.Format(s_szInteractionAnimName, nTime));

		m_vMoveVector = Vector3.zero;
		return true;
	}


	/////////////////////////////////////////
	//---------------------------------------
	protected override void OnDamage_Calculated(int nDamage)
	{
		//if (IsEssential ())
		//	return;

		if (BattleFramework._Instance != null && BattleFramework._Instance.m_eBattleState != BattleFramework.eBattleState.eBattle)
			return;

		//-----
		if (_spcAtk_Counter_LeastInvTime > 0.0f)
		{
			if (_spcAtk_Counter_InvincibleEffected == false)
			{
				PlayerData pPlayerData = GameFramework._Instance.m_pPlayerData;
				++pPlayerData.m_vBossKillRecord[pPlayerData.m_nKillRecordWriteIndex].nDamageIgnored;

				//-----
				if (string.IsNullOrEmpty(_archiveWhenIgnoreDamage) == false)
				{
					Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveWhenIgnoreDamage);
					pArchives.Archive.OnArchiveCount(1);
				}

				//-----
				_spcAtk_Counter_InvincibleEffected = true;
				_spcAtk_Counter_LeastInvTime += _spcAtk_Counter_DashInvincibleTime_Extend;

				Utils.SafeInvoke(onDamageIgnored);
				StartCoroutine(OnSpcAtk_Counter_DashInvincible());
			}

			return;
		}

		//-----
		if (IsEssential() == false && nDamage > 0)
		{
			PlayerData pPlayerData = GameFramework._Instance.m_pPlayerData;

			//-----
			if (BattleFramework._Instance != null)
			{
				GameObject pAlertEffect = HT.Utils.Instantiate(BattleFramework._Instance.m_pPlayer_DamageAlert);
				pAlertEffect.transform.position = pAlertEffect.transform.position + transform.position;

				float fAutoDestroy = 1.0f;
				ParticleSystem pPtclSys = pAlertEffect.GetComponent<ParticleSystem>();
				if (pPtclSys != null)
					fAutoDestroy = pPtclSys.TotalSimulationTime();

				HT.Utils.SafeDestroy(pAlertEffect, fAutoDestroy);

				//-----
				Animation pAnim = BattleFramework._Instance.m_pPlayer_UI_DamageAlert.GetComponent<Animation>();
				pAnim.Stop();
				pAnim.Play("DamageAlert_UI");
			}

			//-----
			CameraManager._Instance.SetCameraShake(1.0f);

			if (BattleFramework._Instance != null)
			{
				AudioClip pDamageSound = BattleFramework._Instance.m_pPlayer_DamageSounds;
				HT.HTSoundManager.PlaySound(pDamageSound);
			}

			SetEssentialTime(GameDefine.fEssentialTime);
			
			//-----
#if UNITY_EDITOR
			if (GameFramework._Instance._characterInvincible == false)
#endif // UNITY_EDITOR
			{
				int nHPVal = m_pActorInfo.m_cnNowHP.val;
				m_pActorInfo.m_cnNowHP.val = nHPVal - nDamage;

				pPlayerData.m_vBossKillRecord[pPlayerData.m_nKillRecordWriteIndex].nPlayerDamaged += nDamage;
			}

			//-----
			//if (GetCurrHP() <= 0)
			//{
			//	HT.Utils.SafeStopCorutine(this, ref _specialAtk_Proc);
			//
			//	StopCharging();
			//	StopSpcAtk();
			//}

			if (BattleFramework._Instance != null)
				HT.Utils.SafeInvoke(BattleFramework._Instance.onPlayerDamaged, m_pActorInfo.m_cnNowHP.val);

			//-----
#if UNITY_ANDROID || UNITY_IOS
			if (HT.HTAfxPref.IsMobilePlatform || HT.HTInputManager.Instance.JoystickConnected)
				Handheld.Vibrate();
#endif // UNITY_ANDROID || UNITY_IOS
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	void DoShotDefaultAttack()
	{
		if (GetCurrHP () <= 0 || (BattleFramework._Instance != null && BattleFramework._Instance.m_eBattleState != BattleFramework.eBattleState.eBattle))
			return;

		//-----
		PlayerData pPlayerData = GameFramework._Instance.m_pPlayerData;

		//-----
		//float fSpread = GameDefine.fPlayer_Arrow_SpreadDeg * m_fChargingGuidLifeTime;
		//float fSpreadDefault = m_pDefAtkSkillBook.m_fProj_Spread;
		//
		//m_pDefAtkSkillBook.m_fProj_Spread = fSpreadDefault + fSpread;

		//-----
		float fChargeRatio = _chargeHelper.ChargeRatio;
		int nDamageDefault = m_pDefAtkSkillBook.m_nProjectileDamage;

		int nDamageUpgrade = pPlayerData.GetUpgrades(PlayerData.ePlayerUpgrades.eAtckPower);
		float fDamageRatio = fChargeRatio;//1.0f - (fChargeRatio * 0.5f);
		fDamageRatio *= 1.0f + (nDamageUpgrade * GameDefine.fAttackPowerIncreaseRatio);
		
		fDamageRatio += CalculateActorBuffEffect(ActorBuff.eBuffType.eDamage_Increase);

		m_pDefAtkSkillBook.m_nProjectileDamage = (int)(fDamageRatio * nDamageDefault);

#if UNITY_EDITOR
		if (GameFramework._Instance._enemyOneKill)
			m_pDefAtkSkillBook.m_nProjectileDamage = 100000;
#endif // UNITY_EDITOR

		//-----
		m_pDefAtkSkillBook.SkillThrow();

		//-----
		float fSkillCooling = m_pDefAtkSkillBook.GetSkillCooling();
		fSkillCooling = fSkillCooling * (1.5f - fChargeRatio);
		fSkillCooling = Mathf.Clamp(fSkillCooling, 0.0f, m_pDefAtkSkillBook.m_fSkillCoolTime);
		m_pDefAtkSkillBook.SetSkillCooling(fSkillCooling);

		//-----
		m_pDefAtkSkillBook.m_fProj_Spread = 0.0f;
		m_pDefAtkSkillBook.m_nProjectileDamage = nDamageDefault;

		//-----
		_chargeHelper.StopChargeAnim();
		OnShotEffect(fChargeRatio);
	}

	void CreateChargingGuid(float fLifeTime)
	{
		_chargeHelper.StartChargeAnim(fLifeTime, ()=> {
			OnShotEffect(0.0f);
		});
	}

	private void OnShotEffect(float fRatio)
	{
		if (_attackEffect != null)
		{
			if (_projectilePosDummy == null)
				_projectilePosDummy = FindDummyPivot(s_szDummyName_ShotPoint);

			CreateShotEffect(_projectilePosDummy.transform.position, _projectilePosDummy.transform.forward);
		}

		if (fRatio >= 1.5f)
		{
			_particleWhenCritical.Play();
			CameraManager._Instance.SetCameraShake(3.0f);
		}
	}

	private void CreateShotEffect(Vector3 vPos, Vector3 vView)
	{
		ParticleSystem pParticle = Utils.InstantiateFromPool(_attackEffect);
		pParticle.transform.position = vPos;
		pParticle.transform.forward = vView;

		pParticle.Stop();
		pParticle.Play(true);
		Utils.SafeDestroy(pParticle.gameObject, pParticle.TotalSimulationTime());
	}

	//---------------------------------------
	public void OnProjectileDamageToThing(int nDamage)
	{
		if (_spcAtk_ChargeIncRequireDamage > nDamage)
		{
			_spcAtk_CurChainedCnt = 0;
			return;
		}

		//-----
		_spcAtk_CurCharged += _spcAtk_ChargeDef + (_spcAtk_CurChainedCnt * _spcAtk_ChargeIncrease);
		++_spcAtk_CurChainedCnt;

		_spcAtk_CurCharged = Mathf.Clamp(_spcAtk_CurCharged, 0, _spcAtk_ChargeMax);

		//-----
		PlayerData pPlayerData = GameFramework._Instance.m_pPlayerData;
		++pPlayerData.m_vBossKillRecord[pPlayerData.m_nKillRecordWriteIndex].nCriticalCount;
	}


	/////////////////////////////////////////
	//---------------------------------------
	private void StopCharging()
	{
		if (_chargeHelper.IsCharging)
		{
			if (m_pDefAtkSkillBook.m_bCastYet == false && GetCurrHP() > 0)
				DoShotDefaultAttack();

			_chargeHelper.StopChargeAnim();
		}
	}

	private void StopSpcAtk()
	{
		if (_spcAtk_FullCharge_Aiming)
			_chargeHelper.StopChargeAnim();

		_spcAtk_FullCharge_Ready = false;
		_spcAtk_FullCharge_Aiming = false;

		Utils.SafeStopCorutine(this, ref _specialAtk_Proc);
		HT.TimeUtils.SetTimeScale(1.0f, GameDefine.nTimeScaleLayer_FullChargeAtk);
	}

	private void StopInteracting()
	{
		if (_onInteractionCancled != null)
			_onInteractionCancled();

		_onInteractionCancled = null;
	}

	private void StopDash()
	{
		RemoveActorBuff("PLAYER_DASH", false);

		m_vMoveVector = m_vViewVector;
		m_bDuringDash = false;
	}

	public void OnStun(float fTime)
	{
		StopCharging();
		StopSpcAtk();

		StopInteracting();
		StopDash();

		_isStunning = true;

		m_vMoveVector = Vector3.zero;

		SetAction(s_szAnumName_Stun);
		Invoke("StunEnd", fTime);
	}

	private void StunEnd()
	{
		_isStunning = false;
		m_pAnimations.Play(m_szIDLEAnimName);
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override void SetControlEnable(bool bSet)
	{
		_controlEnabled = bSet;
		if (bSet == false)
		{
			StopCharging();
			StopSpcAtk();

			StopInteracting();
			StopDash();

			m_vMoveVector = Vector3.zero;

			//-----
			SetAction(s_szAnumName_Stun);
			m_fActionReadyTime = float.PositiveInfinity;
		}
		else
		{
			SetAction(m_szIDLEAnimName);
			m_eActorState = eActorState.eIdle;
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void OnExplosionForce()
	{
		Collider[] vColliders = GetComponentsInChildren<Collider>();
		for (int nInd = 0; nInd < vColliders.Length; ++nInd)
		{
			if (vColliders[nInd].enabled == false)
				continue;

			Rigidbody pBody = vColliders[nInd].GetComponent<Rigidbody>();
			if (pBody == null)
				continue;

			pBody.AddExplosionForce(1000.0f, gameObject.transform.position, 10.0f);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	private IEnumerator OnSpcAtk_Counter_DashInvincible()
	{
        _spcAtk_CurCharged += _spcAtk_Charge_WhenCounter;
        if (_spcAtk_CurCharged >= _spcAtk_ChargeMax)
            _spcAtk_CurCharged = _spcAtk_ChargeMax;

        if (_spcAtk_Counter_DashInvincibleEffect != null)
		{
			ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_spcAtk_Counter_DashInvincibleEffect);
			pEffect.transform.position = transform.position + (Vector3.up * 0.5f);

			HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
		}

		CameraManager._Instance.m_pMainCamera.fieldOfView -= 1.0f;
		HT.TimeUtils.SetTimeScale(0.0f, GameDefine.nTimeScaleLayer_CounterAtk);

		if (_spcAtk_Sound != null)
			HT.HTSoundManager.PlaySound(_spcAtk_Sound);
		
		yield return new WaitForSecondsRealtime(0.2f);

		CameraManager._Instance.m_pMainCamera.fieldOfView += 1.0f;
		HT.TimeUtils.SetTimeScale(1.0f, GameDefine.nTimeScaleLayer_CounterAtk);

        //-----
        if (_spcAtk_Counter_Enabled == false)
            yield break;

		List<GameObject> vAutoTargetList = BattleFramework._Instance.AutoTargetList;
		if (vAutoTargetList == null || vAutoTargetList.Count == 0)
			yield break;

		//-----
		int nProjCount = _spcAtk_CurCharged / _spcAtk_Counter_ChargePerProj;
		if (nProjCount > _spcAtk_Counter_MaxProjCount)
			nProjCount = _spcAtk_Counter_MaxProjCount;

		_spcAtk_CurCharged -= nProjCount * _spcAtk_Counter_ChargePerProj;
		
		CameraManager._Instance.SetCameraShake(_spcAtk_Counter_CamShake);

		float fCurDamageRatio = 1.0f + CalculateActorBuffEffect(ActorBuff.eBuffType.eDamage_Increase);
		for (int nInd = 0; nInd < nProjCount; ++nInd)
		{
			vAutoTargetList = BattleFramework._Instance.AutoTargetList;

			//-----
			GameObject pNearObj = null;
			if (vAutoTargetList.Count > 1)
			{
				float fNearDistance = float.MaxValue;
				pNearObj = null;
				for(int nObj = 0; nObj < vAutoTargetList.Count; ++nObj)
				{
					float fCurDist = Vector3.Distance(transform.position, vAutoTargetList[nObj].transform.position);
					if (fCurDist < fNearDistance)
					{
						fNearDistance = fCurDist;
						pNearObj = vAutoTargetList[nInd];
					}
				}
			}
			else
				pNearObj = vAutoTargetList[0];

			//-----
			if (_projectilePosDummy == null)
				_projectilePosDummy = FindDummyPivot(s_szDummyName_ShotPoint);

			Vector3 vDir = (pNearObj.transform.position - transform.position);
			vDir.y = 0.0f;
			vDir.Normalize();

			//-----
			Vector3 vStartPos = gameObject.transform.position + vDir;
			vStartPos.y += _projectilePosDummy.transform.position.y;

			CreateSpecialProjectile(vStartPos, vDir, (int)(_spcAtk_Counter_DamagePerProj * fCurDamageRatio), true);
			CreateSpecialAttackLight(vStartPos);

			//-----
			if (m_pDefAtkSkillBook._throwSound != null)
				HT.HTSoundManager.PlaySound(m_pDefAtkSkillBook._throwSound);

			//-----
			yield return new WaitForSeconds(_spcAtk_Counter_RepeatTime);
		}
	}

	//---------------------------------------
	private IEnumerator OnSpcAtk_FullCharge_Internal()
	{
		_spcAtk_CurCharged = 0;

		CameraManager._Instance.m_pMainCamera.fieldOfView -= 1.0f;
		HT.TimeUtils.SetTimeScale(0.2f, GameDefine.nTimeScaleLayer_FullChargeAtk);

		if (_spcAtk_Sound != null)
			HT.HTSoundManager.PlaySound(_spcAtk_Sound);

		if (_spcAtk_FullCharge_Effect_Cast != null)
		{
			ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_spcAtk_FullCharge_Effect_Cast);
			pEffect.transform.position = transform.position + (Vector3.up * 0.5f);

			HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
		}

		yield return new WaitForSecondsRealtime(0.5f);
		CameraManager._Instance.m_pMainCamera.fieldOfView += 1.0f;

		//-----
		//float fAnimTime = SetAction(_spcAtk_FullCharge_StartAnim);
		//SetActionReadyTime(float.PositiveInfinity);
		//yield return new WaitForSeconds(fAnimTime);

		//-----
		SetAction(_spcAtk_FullCharge_DuringAnim);
		SetActionReadyTime(float.PositiveInfinity);

		//-----
		Vector3 vDir = gameObject.transform.right;
		if (BattleFramework._Instance != null && m_pDefAtkSkillBook._enableAutoTargeting)
		{
			float fMaxDot = m_pDefAtkSkillBook._autoTargetingMaxDot_PC;
			if (HT.HTAfxPref.IsMobilePlatform)
				fMaxDot = m_pDefAtkSkillBook._autoTargetingMaxDot_Mobile;
			else if (HT.HTInputManager.Instance.JoystickConnected)
				fMaxDot = m_pDefAtkSkillBook._autoTargetingMaxDot_Pad;

			vDir = BattleFramework._Instance.FindNearestAutoTarget(gameObject.transform.position, vDir, fMaxDot);
		}

		if (_projectilePosDummy == null)
			_projectilePosDummy = FindDummyPivot(s_szDummyName_ShotPoint);

		float fCurDamageRatio = 1.0f + CalculateActorBuffEffect(ActorBuff.eBuffType.eDamage_Increase);
		for (int nInd = 0; nInd < _spcAtk_FullCharge_ProjCnt; ++nInd)
		{
            int nDamage = _spcAtk_FullCharge_DamagePerProj;
            if (nInd == _spcAtk_FullCharge_ProjCnt - 1)
			{
				yield return new WaitForSeconds(_spcAtk_FullCharge_RepeatTime_Last);

                if (_spcAtk_FullCharge_Effect_Last != null)
				{
					ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_spcAtk_FullCharge_Effect_Last);
					pEffect.transform.position = _projectilePosDummy.transform.position;
					pEffect.transform.forward = _projectilePosDummy.transform.forward;

					HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
				}

                nDamage = _spcAtk_FullCharge_DamagePerProj_Last;
            }

			CameraManager._Instance.SetCameraShake(_spcAtk_FullCharge_CamShake);

			CreateSpecialProjectile(_projectilePosDummy.transform.position, vDir, (int)(nDamage * fCurDamageRatio), true);
			CreateSpecialAttackLight(_projectilePosDummy.transform.position);

			if (nInd < _spcAtk_FullCharge_ProjCnt - 1)
				yield return new WaitForSeconds(_spcAtk_FullCharge_RepeatTime);
		}

		//-----
		CameraManager._Instance.SetCameraShake(_spcAtk_FullCharge_CamShake_Last);
		HT.TimeUtils.SetTimeScale(1.0f, GameDefine.nTimeScaleLayer_FullChargeAtk);
        SetAction(_spcAtk_FullCharge_EndAnim);

		_specialAtk_Proc = null;

		HT.Utils.SafeInvoke(onCastSpcAtk);
	}

	//---------------------------------------
	private Projectile_Player CreateSpecialProjectile(Vector3 vPos, Vector3 vDir, int nDamage, bool bCreateShotEffect)
	{
		if (GetCurrHP() <= 0 || (BattleFramework._Instance != null && BattleFramework._Instance.m_eBattleState != BattleFramework.eBattleState.eBattle))
			return null;

		//-----
		if (bCreateShotEffect)
		{
			if (m_pDefAtkSkillBook._throwSound != null)
				HT.HTSoundManager.PlaySound(m_pDefAtkSkillBook._throwSound);

			CreateShotEffect(vPos, vDir);
		}

		//-----
		Projectile_Player pProj = null;
		if (m_pDefAtkSkillBook._projectileExtend_Buff != null && FindEnabledActorBuff(m_pDefAtkSkillBook._projectileExtend_Buff) != null)
			pProj = HT.Utils.Instantiate(m_pDefAtkSkillBook._projectileExtend_Projectile) as Projectile_Player;
		else
			pProj = HT.Utils.Instantiate(m_pDefAtkSkillBook.m_pProjectile) as Projectile_Player;

		pProj.m_pSkill_Proj = m_pDefAtkSkillBook;
		pProj.m_pParent = this;
		pProj._isSpcAtkProjectile = true;

		//-----
		PlayerData pPlayerData = GameFramework._Instance.m_pPlayerData;
		int nDamageUpgrade = pPlayerData.GetUpgrades(PlayerData.ePlayerUpgrades.eAtckPower);
		float fDamageRatio = 1.0f + (nDamageUpgrade * GameDefine.fAttackPowerIncreaseRatio);
		fDamageRatio += CalculateActorBuffEffect(ActorBuff.eBuffType.eDamage_Increase);

		pProj.m_nProjectileDamage = (int)(nDamage * fDamageRatio);
		pProj.m_fSpeed = m_pDefAtkSkillBook.m_fProjectileSpeed;

		//-----
		pProj.m_vMoveVector = vDir;
		pProj.UpdateRotate();

		pProj.Init(vPos);

		//-----
		return pProj;
	}

	public void OnSpecialAttakDamageToThing(int nDamage)
	{
		PlayerData pPlayerData = GameFramework._Instance.m_pPlayerData;
		pPlayerData.m_vBossKillRecord[pPlayerData.m_nKillRecordWriteIndex].nSpcAtkDamage += nDamage;
	}

	//---------------------------------------
	private void CreateSpecialAttackLight(Vector3 vPos)
	{
		if (GetCurrHP() <= 0 || (BattleFramework._Instance != null && BattleFramework._Instance.m_eBattleState != BattleFramework.eBattleState.eBattle))
			return;

		LightAutoDestroyer pLight = _spcAtk_ShotLight;
		if (_spcAtk_ShotLight_ExtLight != null && FindEnabledActorBuff(_spcAtk_ShotLight_ExtBuff) != null)
			pLight = _spcAtk_ShotLight_ExtLight;

		if (pLight != null && HTAfxPref.CheckPlatform(pLight._enablePlatform) && HTAfxPref.Quality >= pLight._requireQuality)
		{
			LightAutoDestroyer pNewObj = HT.Utils.InstantiateFromPool(pLight);
			pNewObj.transform.position = _projectilePosDummy.transform.position;
		}
	}
}
