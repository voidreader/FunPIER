using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class AIActor : IActorBase
{
	/////////////////////////////////////////
	//---------------------------------------
	public override IActorBase.eActorType GetActorType()
	{
		return IActorBase.eActorType.eAI;
	}


	/////////////////////////////////////////
	//---------------------------------------
	[Header("AI ACTOR INFO")]
	public ActorSkillContainer[] m_vActorSkillCont_Phase1 = null;
	public ActorSkillContainer[] m_vActorSkillCont_Phase2 = null;
	public ActorSkillContainer[] m_vActorSkillCont_Phase3 = null;
	private List<ActorSkillContainer> _enabledSkills = new List<ActorSkillContainer>();
	private List<ActorSkillContainer> _randomizedSkills = new List<ActorSkillContainer>();

	public AIActor_Extend m_pExtendedFrameworks;

	public bool _damageWhenCollide = true;
	public float m_fReadyForAction_MinTime = 0.5f;
	public float m_fReadyForAction_MaxTime = 1.0f;

	[Header("AUTO TARGET INFO")]
	public bool _isAutoTargetable = true;

	//---------------------------------------
	private GameObject _chasingTarget = null;
	public GameObject ChasingTarget
	{
		get { return _chasingTarget; }
		set { _chasingTarget = value; }
	}
	public bool _skillCastableWhenChaseTarget = false;

	//---------------------------------------
	private float m_fReadyForAction = 0.0f;
	private bool m_bCastSkilled = false;
	private bool m_bForceUpdateViewVector = false;

	//---------------------------------------
	public enum eAIType
	{
		eChasing = 0,
		eStatic,
		eFixed,
		eRagdoll,
	}

	public eAIType m_eAIType = eAIType.eChasing;
	private Vector3 _startedPosition = Vector3.zero;

	//---------------------------------------
	public enum eActorEventCallback
	{
		eDamaged = 0,
		eSkillCast,

		ePlayer_Damaged,
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override void Init()
	{
		//-----
		_startedPosition = transform.position;
		UpdateReadyTime();

		//-----
		if (m_pExtendedFrameworks != null)
		{
			m_pExtendedFrameworks.m_pActorBase = this;

			m_pExtendedFrameworks.Init();
			m_pExtendedFrameworks.Extended_Init();
		}

		//-----
		base.Init();

		if (m_eAIType == eAIType.eFixed || m_eAIType == eAIType.eRagdoll)
		{
			m_vViewVector = Vector3.forward;
			UpdateViewVector();
		}

		//-----
		if (m_pExtendedFrameworks != null)
			m_pExtendedFrameworks.Extended_PostInit();

		//-----
		_enabledSkills.Clear();
		_enabledSkills.AddRange(m_vActorSkillCont);

		if (BattleFramework._Instance != null)
		{
			if (FindEnabledActorBuff(BattleFramework._Instance.m_vAddPatternBuffs[0]) != null)
				AddPatternSkillsOverride(m_vActorSkillCont_Phase1);

			if (FindEnabledActorBuff(BattleFramework._Instance.m_vAddPatternBuffs[1]) != null)
				AddPatternSkillsOverride(m_vActorSkillCont_Phase2);

			if (FindEnabledActorBuff(BattleFramework._Instance.m_vAddPatternBuffs[2]) != null)
				AddPatternSkillsOverride(m_vActorSkillCont_Phase3);
		}
	}

	private void AddPatternSkillsOverride(ActorSkillContainer[] vSkillArray)
	{
		for (int nInd = 0; nInd < vSkillArray.Length; ++nInd)
		{
			vSkillArray[nInd].Init();

			bool bOverrided = false;
			for (int nSearch = 0; nSearch < _enabledSkills.Count; ++nSearch)
			{
				if (_enabledSkills[nSearch]._skillName == vSkillArray[nInd]._skillName)
				{
					bOverrided = true;
					_enabledSkills[nSearch] = vSkillArray[nInd];
					break;
				}
			}

			if (bOverrided == false)
				_enabledSkills.Add(vSkillArray[nInd]);
		}
	}

	//---------------------------------------
	public override void Frame()
	{
		if (m_bActorIncapacitation)
		{
			base.Frame();
			return;
		}

		//-----
		GameObject pMoveTarget = null;
		if (_chasingTarget != null && (_skillCastableWhenChaseTarget == false || m_eActorState != eActorState.eAction))
			pMoveTarget = _chasingTarget;
		else
			pMoveTarget = BattleFramework._Instance.m_pPlayerActor.gameObject;

		Vector3 vMoveTargetPos = pMoveTarget.transform.position;
		Vector3 vActorPos = gameObject.transform.position;

		//-----
		if (m_eActorState != eActorState.eAction)
			m_fReadyForAction -= HT.TimeUtils.GameTime;

		else
		{
			m_vMoveVector = Vector3.zero;
		}

		if (m_eActorState != eActorState.eAction && m_fReadyForAction <= 0.0f)
		{
			m_bForceUpdateViewVector = false;

			bool bSkillCasted = false;
			if (m_bCastSkilled == false && (_chasingTarget == null || _skillCastableWhenChaseTarget))
			{
				Vector3 vSkillTargetPos = BattleFramework._Instance.m_pPlayerActor.gameObject.transform.position;
				float fCharDistance = Vector3.Distance(vSkillTargetPos, vActorPos);

				//-----
				List<int> randomIndeces = new List<int>();
				for (int nInd = 0; nInd < _enabledSkills.Count; ++nInd)
					randomIndeces.Add(nInd);

				_randomizedSkills.Clear();
				for (int nInd = 0; nInd < _enabledSkills.Count; ++nInd)
				{
					int nCollectedIndex = Random.Range(0, randomIndeces.Count);

					int nCurInd = randomIndeces[nCollectedIndex];
					randomIndeces.RemoveAt(nCollectedIndex);

					_randomizedSkills.Add(_enabledSkills[nCurInd]);
				}

				//-----
				for (int nInd = 0; nInd < _randomizedSkills.Count; ++nInd)
				{
					ActorSkill pSkill = _randomizedSkills[nInd].GetSkillByLevel();
					if (pSkill.m_fSkillMinRange <= fCharDistance && pSkill.m_fSkillMaxRange >= fCharDistance)
					{
						if (pSkill.SkillCast(this, BattleFramework._Instance.m_pPlayerActor) == false)
							continue;

						OnEventCallback(eActorEventCallback.eSkillCast, pSkill.gameObject);

						if (pSkill._updateDirectionInCast)
							m_bForceUpdateViewVector = true;

						bSkillCasted = true;
						break;
					}
				}
			}

			//-----
			if (bSkillCasted == false)
			{
				m_bCastSkilled = false;
				float fActionRate = Random.Range(0.0f, 1.0f);

				//-----
				switch (m_eAIType)
				{
					case eAIType.eChasing:
						{
							if (fActionRate > 0.1f)
								m_eActorState = eActorState.eMove;
							else
								m_eActorState = eActorState.eIdle;
						}
						break;

					case eAIType.eStatic:
					case eAIType.eFixed:
						m_eActorState = eActorState.eIdle;
						break;
				}
			}

			//-----
			UpdateReadyTime();
		}

		//-----
		if (m_eAIType == eAIType.eChasing || m_eAIType == eAIType.eStatic)
		{
			Vector3 vMoveVector = transform.position;
			if (Vector3.Distance(vMoveVector, vMoveTargetPos) > 1.5f)
			{
				vMoveVector.y = 0.0f;
				vMoveTargetPos.y = 0.0f;

				vMoveVector = vMoveTargetPos - vMoveVector;
				vMoveVector.Normalize();

				//-----
				switch (m_eActorState)
				{
					case eActorState.eIdle:
						m_vViewVector = vMoveVector;
						break;

					case eActorState.eMove:
						m_vMoveVector = vMoveVector;
						break;

					case eActorState.eAction:
						if (m_bForceUpdateViewVector)
							m_vViewVector = vMoveVector;

						break;
				}
			}
			else
				m_vMoveVector = Vector3.zero;
		}
		else if (m_eAIType == eAIType.eFixed)
		{
			m_vViewVector = Vector3.forward;
			m_vMoveVector = Vector3.zero;
			
			transform.position = _startedPosition;
		}
		else if (m_eAIType == eAIType.eRagdoll)
		{
			m_vViewVector = Vector3.zero;
			m_vMoveVector = Vector3.zero;
		}

		//-----
		if (m_pExtendedFrameworks != null)
			m_pExtendedFrameworks.Extended_Frame();

		//-----
		base.Frame();
	}

	//---------------------------------------
	public override void Release()
	{
		//-----
		if (m_pExtendedFrameworks != null)
		{
			m_pExtendedFrameworks.Release();
			m_pExtendedFrameworks.Extended_Release();
		}

		//-----
		base.Release();
	}

	//---------------------------------------
	public override void OnBattleStart()
	{
		if (_isAutoTargetable)
			BattleFramework._Instance.AddAutoTargetObject(gameObject);

		base.OnBattleStart();

		if (m_pExtendedFrameworks != null)
			m_pExtendedFrameworks.OnBattleStart();
	}

	public override void OnBattleEnd(bool bPlayerWin)
	{
		BattleFramework._Instance.RemoveAutoTargetObject(gameObject);

		base.OnBattleEnd(bPlayerWin);

		if (m_pExtendedFrameworks != null)
			m_pExtendedFrameworks.OnBattleEnd(bPlayerWin);
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void UpdateReadyTime()
	{
		m_fReadyForAction = Random.Range(m_fReadyForAction_MinTime, m_fReadyForAction_MaxTime);
	}

	public void SetReadyTime(float fSet)
	{
		m_fReadyForAction = fSet;
	}

	//---------------------------------------
	public override void OnCallSetAction(AnimationClip pAnimClip)
	{
		//UpdateReadyTime ();
		m_bCastSkilled = true;
	}


	/////////////////////////////////////////
	//---------------------------------------
	protected override void OnDamage_Calculated(int nDamage)
	{
		if (IsEssential())
			return;

		if (BattleFramework._Instance.m_eBattleState != BattleFramework.eBattleState.eBattle)
			return;
		
		//if (nDamage > 0) {
		PlayerData pPlayerData = GameFramework._Instance.m_pPlayerData;

		int nHPVal = m_pActorInfo.m_cnNowHP.val;
		m_pActorInfo.m_cnNowHP.val = nHPVal - nDamage;

		if (m_pActorInfo.m_cnNowHP.val > m_pActorInfo.m_cnMaxHP.val)
			m_pActorInfo.m_cnNowHP.val = m_pActorInfo.m_cnMaxHP.val;

		OnEventCallback(eActorEventCallback.eDamaged, null);

		++pPlayerData.m_vBossKillRecord[pPlayerData.m_nKillRecordWriteIndex].nBossHitCount;
		pPlayerData.m_vBossKillRecord[pPlayerData.m_nKillRecordWriteIndex].nBossDamaged += nDamage;
		//}
	}


	/////////////////////////////////////////
	//---------------------------------------
	void OnEventCallback(eActorEventCallback eEvent, GameObject pParam)
	{
		if (m_pExtendedFrameworks != null)
		{
			m_pExtendedFrameworks.Extended_EventCallback(eEvent, pParam);
		}
	}

	public override bool IsEssential()
	{
		if (base.IsEssential())
			return true;

		if (m_pExtendedFrameworks != null && m_pExtendedFrameworks.Extended_IsDamageEnable() == false)
			return true;

		return false;
	}
}
