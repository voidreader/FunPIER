using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSkill_Projectile : ActorSkill
{
	/////////////////////////////////////////
	//---------------------------------------
	public Action<Projectile> onSkillThrow = null;

	/////////////////////////////////////////
	//---------------------------------------
	[Header("SKILL SET")]
	public Projectile m_pProjectile;

	public ActorBuff _projectileExtend_Buff = null;
	public Projectile _projectileExtend_Projectile = null;

	public int m_nProjectileCount = 1;

	//---------------------------------------
	public int m_nProjectileDamage;
	public float m_fProjectileSpeed;

	//---------------------------------------
	public float m_fProj_Spread = 0.0f;

	//---------------------------------------
	public float m_fLifeTime;

	//---------------------------------------
	[Header("AUTO TARGETING")]
	public bool _enableAutoTargeting = false;
	public float _autoTargetingMaxDot_PC = 0.015f;
	public float _autoTargetingMaxDot_Pad = 0.05f;
	public float _autoTargetingMaxDot_Mobile = 0.075f;


	/////////////////////////////////////////
	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
		return true;
	}

	public override void SkillThrow_Child()
	{
		if (m_pProjectile == null)
			return;

		if (m_pCaster.GetCurrHP () <= 0 || (BattleFramework._Instance != null && BattleFramework._Instance.m_eBattleState != BattleFramework.eBattleState.eBattle))
			return;

		for (int nInd = 0; nInd < m_nProjectileCount; ++nInd)
		{
			Projectile pInstance = m_pProjectile;
			if (_projectileExtend_Buff != null && m_pCaster.FindEnabledActorBuff(_projectileExtend_Buff) != null)
				pInstance = _projectileExtend_Projectile;
				
			Projectile pProj = HT.Utils.Instantiate(pInstance);
			if (m_fLifeTime > 0.0f)
				HT.Utils.SafeDestroy(pProj.gameObject, m_fLifeTime);

			//-----
			pProj.m_pSkill_Proj = this;
			pProj.m_pParent = m_pCaster;

			//-----
			pProj.m_nProjectileDamage = m_nProjectileDamage;
			pProj.m_fSpeed = m_fProjectileSpeed;

			//-----
			Vector3 vMoveVec = m_pCaster.m_vViewVector;
			if (vMoveVec.sqrMagnitude <= 0.0f)
				vMoveVec = m_pCaster.transform.right;

			vMoveVec.Normalize();

			if (m_fProj_Spread > 0.0f)
				vMoveVec = HT.RandomUtils.GetRandomVector(vMoveVec, m_fProj_Spread);

			Vector3 vBasePosition = m_pCaster.transform.position;
			if (BattleFramework._Instance != null && _enableAutoTargeting && vMoveVec.magnitude > 0.0f)
			{
				float fMaxDot = _autoTargetingMaxDot_PC;

				if (HT.HTAfxPref.IsMobilePlatform) 
					fMaxDot = _autoTargetingMaxDot_Mobile;
				
				else if (HT.HTInputManager.Instance.JoystickConnected)
					fMaxDot = _autoTargetingMaxDot_Pad;

				vMoveVec = BattleFramework._Instance.FindNearestAutoTarget (vBasePosition, vMoveVec, fMaxDot);
			}

			pProj.m_vMoveVector = vMoveVec;
			pProj.UpdateRotate();

			//-----
			float fColliderRadius = 1.0f;
			CapsuleCollider pCapsule = m_pCaster.GetComponent<CapsuleCollider>();
			if (pCapsule != null)
				fColliderRadius = pCapsule.radius;

			Vector3 vPos = pProj.transform.position;
			pProj.Init(vPos + vBasePosition + (pProj.m_vMoveVector * fColliderRadius));

			//-----
			//pProj.m_pExplodeSound = m_pProjectileExpSound;
			//pProj.m_bExpSoundPlayWhenLifeOver = m_bExpSoundPlayWhenLifeOver;

			HT.Utils.SafeInvoke(onSkillThrow, pProj);
		}
	}

	/////////////////////////////////////////
	//---------------------------------------
}
