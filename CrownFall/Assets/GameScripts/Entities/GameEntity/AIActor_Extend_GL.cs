using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
[Serializable]
public struct DamagablePart
{
	public AIActor_Extend_GL.eParts _enum;
	public BossGL_Parts _parts;
	public ActorBuff _buff;
	public string _dividAnimName;
	public HT.CInt _leastHealth;
	public int _maxHealth;
}


/////////////////////////////////////////
//---------------------------------------
public sealed class AIActor_Extend_GL : AIActor_Extend
{
	//---------------------------------------
	public enum eParts
	{
		ArmR,
		ArmL,
		LegR,
		LegL,

		Max,
	}

	public enum ePartsState
	{
		eHasAllLeg,
		eHasOneLeg,
		eNoParts,
	}

	//---------------------------------------
	[Header("BASE SETTINGS")]
	public float _step_CamShake = 0.0f;
	public AudioClip _stepSound = null;
	public ActorSkill_Projectile _playerAtkSkill = null;

	[Header("PARTS")]
	public DamagablePart[] _damagableParts = null;
    public int _partsHPincreaseOffset = 200;
	public int _partsHPincreaseOffset_Body = 600;
	public ParticleSystem _dividParts_Effect = null;
	public AudioClip _divideParts_Sounds = null;

	public ActorBuff[] _ignoreLegBuff = null;

	//---------------------------------------
	public string _animName_IDLE_Def = null;
	public string _animName_IDLE_LegR = null;
	public string _animName_IDLE_LegL = null;
	public string _animName_IDLE_Rolling = null;

	//---------------------------------------
	public ActorBuff _legLeftEnableAtk = null;
	public ActorBuff _legRightEnableAtk = null;
	public GameObject _instance_BlankGameObject = null;

	//---------------------------------------
	[Header("PHYSIC CONTROl")]
	public Vector3 _defColliderOffset = Vector3.zero;
	public float _defColliderHeight = 0.0f;
	public Vector3 _divColliderOffset = Vector3.zero;
	public float _divColliderHeight = 0.0f;
	public float _forceWhenDividedAll = 0.0f;

	//---------------------------------------
	[Header("ROLLING PATTERN")]
	public float _rollingTime = 20.0f;
	public GameObject _forcePivotWhenRolling = null;
	public ForceMode _forceModeWhenRolling = ForceMode.VelocityChange;
	public float[] _forceWhenRolling = null;
	public float _rollingStopWhenDamaged = 3.0f;
	public GameObject _shadow = null;

	//---------------------------------------
	[Header("JUMP PATTERN")]
	public ActorBuff _jumpSlowBuff = null;
	public ActorBuff _jumpDmgDownBuff = null;
	public float _stunTime = 0.5f;

	//---------------------------------------
	[Header("ARCHIVEMENTS")]
	public string _archiveOrder_Name = null;
	public eParts[] _archiveOrder = null;

	private eParts[] _destroyOrder = null;

	public string _archiveRollTimeLimit_Name = null;
	public float _archiveRollTimeLimit = 10.0f;

	private float _archiveRollTimeLimit_Least = 0.0f;

	//---------------------------------------
	private Rigidbody _rigidBody = null;
	private ePartsState _partsState = ePartsState.eHasAllLeg;
	private CapsuleCollider _collider = null;
	private GameObject _dividedPartsParent = null;
	private BossGL_Parts _currActivatedParts = null;
	private bool _damageEnabled = false;
	private float _rollingStopTime = 0.0f;

	public BossGL_Parts CurrActivatedParts { get { return _currActivatedParts; } }
	public int PartsHPincreaseOffset_Body { get { return _partsHPincreaseOffset_Body; } }

	//---------------------------------------
	public override void Extended_Init()
	{
		ActorInfo pCurInfo = m_pActorBase.m_pActorInfo;

		int nBaseHP = GetPartsBaseHP();
		int nTotalHP = _partsHPincreaseOffset_Body;
		for (int nInd = 0; nInd < _damagableParts.Length; ++nInd)
		{
			_damagableParts[nInd]._leastHealth = new HT.CInt();
			_damagableParts[nInd]._leastHealth.val = nBaseHP;
			_damagableParts[nInd]._maxHealth = nBaseHP;

			nTotalHP += nBaseHP;
		}
		pCurInfo.m_cnNowHP.val = pCurInfo.m_cnMaxHP.val = pCurInfo.m_nCalculatedHP = nTotalHP;

		//-----
		_collider = GetComponent<CapsuleCollider>();
		_collider.center = _defColliderOffset;
		_collider.height = _defColliderHeight;

		_rigidBody = GetComponent<Rigidbody>();

		//-----
		_dividedPartsParent = HT.Utils.Instantiate(_instance_BlankGameObject);
		_dividedPartsParent.transform.position = Vector3.zero;
		_dividedPartsParent.transform.Rotate(Vector3.zero);
		_dividedPartsParent.transform.localScale = Vector3.one;

		//-----
		_partsState = ePartsState.eHasAllLeg;

		_destroyOrder = new eParts[_archiveOrder.Length];
		for (int nInd = 0; nInd < _destroyOrder.Length; ++nInd)
			_destroyOrder[nInd] = eParts.Max;

		_archiveRollTimeLimit_Least = _archiveRollTimeLimit;

		//-----
		_shadow.SetActive(true);
	}

	public override void Extended_PostInit()
	{
		for (int nInd = 0; nInd < _damagableParts.Length; ++nInd)
		{
			m_pActorBase.AddActorBuff(_damagableParts[nInd]._buff);
			SetPartsInfo(_damagableParts[nInd]._parts);
		}

		//-----
		for (int nInd = 0; nInd < _ignoreLegBuff.Length; ++nInd)
			m_pActorBase.RemoveActorBuff(_ignoreLegBuff[nInd], false);

		m_pActorBase.AddActorBuff(HT.RandomUtils.Array(_ignoreLegBuff));

		//-----
		m_pActorBase.AddActorBuff(_legLeftEnableAtk);
		m_pActorBase.AddActorBuff(_legRightEnableAtk);
	}

	private void SetPartsInfo(BossGL_Parts pParts)
	{
		pParts._parentActorExtend = this;
		pParts._dividedPartsParent = _dividedPartsParent;

		if (pParts._linkParts != null)
			SetPartsInfo(pParts._linkParts);
	}

	public override void Extended_Frame()
	{
		if (_partsState == ePartsState.eHasOneLeg)
		{
			m_pActorBase.SetReadyTime(0.0f);
		}
		else if (_partsState == ePartsState.eNoParts)
		{
			if (_shadow.activeInHierarchy)
				_shadow.SetActive (false);
			
			if (_rollingStopTime > 0.0f)
			{
				_rollingStopTime -= HT.TimeUtils.GameTime;
				_rigidBody.velocity = Vector3.zero;
				_rigidBody.angularVelocity = Vector3.zero;
			}
			else
			{
				Vector3 vForceVec = BattleFramework._Instance.m_pPlayerActor.transform.position - _forcePivotWhenRolling.transform.position;
				vForceVec.Normalize();

				float fForce = _forceWhenRolling[(int)GameFramework._Instance.m_pPlayerData.m_eDifficulty];
				_rigidBody.AddForce(vForceVec * (fForce * HT.TimeUtils.frame60DeltaTime), _forceModeWhenRolling);
			}

			_archiveRollTimeLimit_Least -= Time.deltaTime;
		}

		//-----
		ControllableActor pPlayer = BattleFramework._Instance.m_pPlayerActor as ControllableActor;
		IActorBase.ActorBuffEnable pJumpSlowBuff = pPlayer.FindEnabledActorBuff(_jumpSlowBuff);
		if (pJumpSlowBuff != null)
		{
			if (pJumpSlowBuff.nStackCount >= pJumpSlowBuff.pBuff._maxStackCount)
			{
				pPlayer.RemoveActorBuff(_jumpDmgDownBuff, false);
				pPlayer.RemoveActorBuff(_jumpSlowBuff, false);

				pPlayer.OnStun(_stunTime);
			}
		}
	}

	public override void Extended_Release()
	{
	}

	//---------------------------------------
	public override bool Extended_IsDamageEnable()
	{
		return _damageEnabled;
	}

	//---------------------------------------
	public void OnPartsActivate(eParts eParts)
	{
		_currActivatedParts = FindParts(eParts)._parts;
		BattleFramework._Instance.AddAutoTargetObject(_currActivatedParts.gameObject);
	}

	public void OnPartsDeactivate()
	{
		BattleFramework._Instance.RemoveAutoTargetObject(_currActivatedParts.gameObject);
		_currActivatedParts = null;
	}

	public DamagablePart FindParts(BossGL_Parts pParts)
	{
		for (int nInd = 0; nInd < _damagableParts.Length; ++nInd)
			if (_damagableParts[nInd]._parts == pParts)
				return _damagableParts[nInd];

		return new DamagablePart() { _parts = null, _buff = null };
	}

	public DamagablePart FindParts(eParts eParts)
	{
		for (int nInd = 0; nInd < _damagableParts.Length; ++nInd)
			if (_damagableParts[nInd]._enum == eParts)
				return _damagableParts[nInd];

		return new DamagablePart() { _parts = null, _buff = null };
	}

	//---------------------------------------
	public void OnPartsDamaged(BossGL_Parts pParts, int nDamage)
	{
		if (_currActivatedParts != pParts)
			return;

		//-----
		DamagablePart pFindParts = FindParts(pParts);
		if (m_pActorBase.FindEnabledActorBuff(pFindParts._buff) == null)
			return;

		//-----
		_damageEnabled = true;
		int nRealDamage = Mathf.Min(pFindParts._leastHealth.val, nDamage);
		if (nRealDamage >= pFindParts._leastHealth.val && (pFindParts._enum == eParts.LegL || pFindParts._enum == eParts.LegR))
		{
			DamagablePart pStatus_LArm = FindParts(eParts.ArmL);
			bool bStatus_HasLArm = (m_pActorBase.FindEnabledActorBuff(pStatus_LArm._buff) != null) ? true : false;

			DamagablePart pStatus_RArm = FindParts(eParts.ArmR);
			bool bStatus_HasRArm = (m_pActorBase.FindEnabledActorBuff(pStatus_RArm._buff) != null) ? true : false;

			if (bStatus_HasLArm || bStatus_HasRArm)
				nRealDamage = pFindParts._leastHealth.val - 1;
		}

		pFindParts._leastHealth.val -= nRealDamage;
		m_pActorBase.OnDamaged(nRealDamage);
		_damageEnabled = false;

		bool bRefreshAllState = false;
		if (pFindParts._leastHealth.val <= 0)
		{
			bRefreshAllState = true;

			pFindParts._parts.OnPartsDivid();
			if (pFindParts._parts._linkParts != null)
				pFindParts._parts._linkParts.OnPartsDivid();

			m_pActorBase.RemoveActorBuff(pFindParts._buff, false);

			//-----
			for (int nInd = 0; nInd < _destroyOrder.Length; ++nInd)
			{
				if (_destroyOrder[nInd] == eParts.Max)
				{
					_destroyOrder[nInd] = pFindParts._enum;
					break;
				}
			}
		}

		//-----
		DamagablePart pLArm = FindParts(eParts.ArmL);
		bool bHasLArm = (m_pActorBase.FindEnabledActorBuff(pLArm._buff) != null)? true : false;

		DamagablePart pRArm = FindParts(eParts.ArmR);
		bool bHasRArm = (m_pActorBase.FindEnabledActorBuff(pRArm._buff) != null) ? true : false;

		DamagablePart pLLeg = FindParts(eParts.LegL);
		bool bHasLLeg = (m_pActorBase.FindEnabledActorBuff(pLLeg._buff) != null) ? true : false;

		DamagablePart pRLeg = FindParts(eParts.LegR);
		bool bHasRLeg = (m_pActorBase.FindEnabledActorBuff(pRLeg._buff) != null) ? true : false;

		//-----
		bool bLLegAtkEnable = false;
		bool bRLegAtkEnable = false;
		if (bHasLArm || bHasRArm)
		{
			if (pLLeg._leastHealth.val >= _playerAtkSkill.m_nProjectileDamage)
				bLLegAtkEnable = true;

			if (pRLeg._leastHealth.val >= _playerAtkSkill.m_nProjectileDamage)
				bRLegAtkEnable = true;
		}
		else
		{
			bLLegAtkEnable = true;
			bRLegAtkEnable = true;
		}

		if (bLLegAtkEnable)
			m_pActorBase.AddActorBuff(_legLeftEnableAtk);
		else
			m_pActorBase.RemoveActorBuff(_legLeftEnableAtk, false);

		if (bRLegAtkEnable)
			m_pActorBase.AddActorBuff(_legRightEnableAtk);
		else
			m_pActorBase.RemoveActorBuff(_legRightEnableAtk, false);

		//-----
		if (bRefreshAllState)
		{
			if (bHasLLeg || bHasRLeg)
			{
				_damageEnabled = false;
				m_pActorBase.SetAction(pFindParts._dividAnimName);

				if (bHasLLeg && bHasRLeg)
				{
					_partsState = ePartsState.eHasAllLeg;
					m_pActorBase.m_szIDLEAnimName = _animName_IDLE_Def;
					m_pActorBase.m_eAIType = AIActor.eAIType.eChasing;
				}
				else if (bHasLLeg && bHasRLeg == false)
				{
					_partsState = ePartsState.eHasOneLeg;
					m_pActorBase.m_szIDLEAnimName = _animName_IDLE_LegL;
					m_pActorBase.m_eAIType = AIActor.eAIType.eStatic;
				}
				else if (bHasLLeg == false && bHasRLeg)
				{
					_partsState = ePartsState.eHasOneLeg;
					m_pActorBase.m_szIDLEAnimName = _animName_IDLE_LegR;
					m_pActorBase.m_eAIType = AIActor.eAIType.eStatic;
				}
			}
			else
			{
				_damageEnabled = true;

				_partsState = ePartsState.eNoParts;

				m_pActorBase.m_pAnimations.Stop();
				m_pActorBase.m_vViewVector = Vector3.zero;
				m_pActorBase.m_vMoveVector = Vector3.zero;

				m_pActorBase.m_eAIType = AIActor.eAIType.eRagdoll;
				m_pActorBase.SetActorState(IActorBase.eActorState.eIdle);

				m_pActorBase.m_szIDLEAnimName = _animName_IDLE_Rolling;

				_collider.center = _divColliderOffset;
				_collider.height = _divColliderHeight;

				_rigidBody.drag = 1.0f;
				_rigidBody.constraints = RigidbodyConstraints.None;

				_rollingStopTime = 0.0f;

				if (_forceWhenDividedAll > 0.0f)
				{
					Vector3 vForceVec = gameObject.transform.forward;
					vForceVec = Quaternion.AngleAxis(180.0f, gameObject.transform.up) * vForceVec;
					_rigidBody.AddForce(vForceVec * _forceWhenDividedAll, ForceMode.Impulse);
				}
			}
		}
	}

	//---------------------------------------
	public void StepEffect()
	{
		if (_step_CamShake > 0.0f)
			CameraManager._Instance.SetCameraShake(_step_CamShake);

		HT.HTSoundManager.PlaySound(_stepSound);
	}

	//---------------------------------------
	private void OnCollisionEnter(Collision pCollision)
	{
		if (_partsState != ePartsState.eNoParts)
			return;

		if (pCollision.gameObject == BattleFramework._Instance.m_pPlayerActor.gameObject)
			_rollingStopTime = _rollingStopWhenDamaged;
	}

	//---------------------------------------
	public override void Extended_EventCallback(AIActor.eActorEventCallback eEvent, GameObject pParam)
	{
		if (eEvent == AIActor.eActorEventCallback.eDamaged)
		{
			if (m_pActorBase.GetCurrHP() <= 0)
			{
				bool bIsDifferent = false;
				for (int nInd = 0; nInd < _archiveOrder.Length; ++nInd)
				{
					if (_archiveOrder[nInd] != _destroyOrder[nInd])
					{
						bIsDifferent = true;
						break;
					}
				}

				if (bIsDifferent == false)
				{
					Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveOrder_Name);
					pArchives.Archive.OnArchiveCount(1);
				}

				//-----
				if (_archiveRollTimeLimit_Least > 0.0f)
				{
					Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveRollTimeLimit_Name);
					pArchives.Archive.OnArchiveCount(1);
				}
			}
		}
	}

	//---------------------------------------
	public int GetPartsBaseHP()
	{
		return _partsHPincreaseOffset; /*(int)((_playerAtkSkill.m_nProjectileDamage * 1.5f) * 2.5f) + _partsHPincreaseOffset; //pCurInfo.m_nBaseHP*/
	}
}


/////////////////////////////////////////
//---------------------------------------