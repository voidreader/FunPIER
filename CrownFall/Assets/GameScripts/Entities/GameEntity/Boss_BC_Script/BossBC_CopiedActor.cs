using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossBC_CopiedActor : IActorBase
{
	//---------------------------------------
	[Header("SKILL")]
	[SerializeField]
	private ActorSkillContainer[] _usableSkill = null;
	[SerializeField]
	private float _waitTimeForNextSkill = 0.5f;
	[SerializeField]
	private float _postDelay = 0.0f;
	[SerializeField]
    private bool _isFixedActor = false;

	public float PostDelay { get { return _postDelay; } }

	//---------------------------------------
	[SerializeField]
	private ActorBuff[] _defaultBuff = null;

	//---------------------------------------
	private int _lastUsedSkillIndex = 0;
	private bool _actorInitialized = false;

	private float _waitForSkillUse = 0.0f;

	protected bool _useOnlyOneSkill = false;

	//---------------------------------------
	public virtual void CopyActor_Init(bool bUseOnlyOneSkill)
	{
		if (_actorInitialized == false)
		{
			_actorInitialized = true;

			base.Init();

			for (int nInd = 0; nInd < _usableSkill.Length; ++nInd)
			{
				//m_vActorSkillCont[nInd] = Instantiate(m_vActorSkillCont[nInd]);
				_usableSkill[nInd].Init();
			}
		}

		//-----
		for (int nInd = 0; nInd < _defaultBuff.Length; ++nInd)
			AddActorBuff(_defaultBuff[nInd]);

		//-----
		_useOnlyOneSkill = bUseOnlyOneSkill;
		_lastUsedSkillIndex = 0;
		_waitForSkillUse = 0.0f;

		//-----
		m_eActorState = eActorState.eIdle;
		m_fActionReadyTime = 0.0f;
	}

	public virtual bool CopyActor_Frame()
	{
		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
		if (m_eActorState != eActorState.eAction && _isFixedActor == false)
        {
            Vector3 vView = pPlayer.transform.position - gameObject.transform.position;
            vView.y = 0.0f;

            m_vViewVector = vView.normalized;
        }

		//-----
		base.Frame();

		if (_lastUsedSkillIndex > 0)
			_usableSkill[_lastUsedSkillIndex - 1].GetSkillByLevel().Frame();

		//-----
		if (m_eActorState != eActorState.eAction && (_usableSkill == null || _usableSkill.Length <= 0))
			return false;

		//-----
		do
		{
			if (m_eActorState == eActorState.eAction)
				break;
			
			if (_usableSkill.Length <= _lastUsedSkillIndex)
				return false;

			if (_useOnlyOneSkill && _lastUsedSkillIndex > 0)
				return false;

			//-----
			_waitForSkillUse -= HT.TimeUtils.GameTime;
			if (_waitForSkillUse > 0.0f)
				break;

			//-----
			ActorSkill pCurSkill = _usableSkill[_lastUsedSkillIndex].GetSkillByLevel();
			pCurSkill.SetSkillCooling(0.0f);

			pCurSkill.SkillCast(this, pPlayer);

			//-----
			_waitForSkillUse = _waitTimeForNextSkill;
			++_lastUsedSkillIndex;
		}
		while (false);
		
		//-----
		return true;
	}

	public virtual void CopyActor_Release()
	{
		//base.Release();

		//-----
		_lastUsedSkillIndex = 0;
	}

	//---------------------------------------
	public override int GetCurrHP()
	{
		return int.MaxValue;
	}

	public override int GetMaxHP()
	{
		return int.MaxValue;
	}

	protected override void OnDamage_Calculated(int nDamage)
	{
		BattleFramework._Instance.m_pEnemyActor.OnDamaged(nDamage);
	}

	//---------------------------------------
	public void OnPartsActivate(AIActor_Extend_GL.eParts eParts) { }
	public void OnPartsDeactivate() { }

	public void Syringe_GaugeZero() { }

	//---------------------------------------
	public override void OnBattleStart()
	{
		base.OnBattleStart();
	}

	public override void OnBattleEnd(bool bPlayerWin)
	{
		base.OnBattleEnd(bPlayerWin);
	}
}


/////////////////////////////////////////
//---------------------------------------