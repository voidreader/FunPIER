using UnityEngine;
using System.Collections;
using HT;

/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSkill_MoveAtk : ActorSkill
{
	/////////////////////////////////////////
	//---------------------------------------
	public enum eMoveTarget
	{
		ePlayerActor = 0,
		eCasterPos,
	}
	[Header("MOVE ATTACK INFO")]
	public eMoveTarget m_eMoveTarget = eMoveTarget.ePlayerActor;
	public int m_nJumpAttackDamage = 1;

	[SerializeField]
	private bool _checkPassPhysic = true;

	[SerializeField]
	private float _moveDistance = -1.0f;


	/////////////////////////////////////////
	//---------------------------------------
	public float m_fMoveTime;

	public bool m_bJumpMove;
	public float m_fJumpMove_Height;

	//---------------------------------------
	public ActorBuff m_pMoveStartBuff = null;
	public bool _moveStartBuffOnlyDuringMove = false;
	public ActorBuff m_pMoveEndBuff;
	public float m_fMoveEndBuffTimeOverwrap;

	public ParticleSystem m_pMoveEndEffect;
	public AudioClip _moveEndSound = null;
	public float m_fMoveEndCamShake;


	/////////////////////////////////////////
	//---------------------------------------
	bool m_bSkillProcessing = false;
	float m_fElapsedMoveTime = 0.0f;
	float m_fTotalMoveTime = 0.0f;

	Vector3 m_vStartPosition;
	Vector3 m_vDestination;


	/////////////////////////////////////////
	//---------------------------------------
	public override void ResetAll()
	{
		base.ResetAll();

		m_bSkillProcessing = false;
	}

	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
		switch (m_eMoveTarget)
		{
			case eMoveTarget.ePlayerActor:
				m_vDestination = m_pTarget.transform.position;
				break;

			case eMoveTarget.eCasterPos:
				m_vDestination = m_pCaster.transform.position;
				break;
		}

		//-----
		bool bCheckByPhysic = false;
		if (_checkPassPhysic)
		{
			m_vStartPosition = m_pCaster.transform.position;
			m_vStartPosition.y = m_vDestination.y;

			Vector3 vMoveVec = m_vDestination - m_vStartPosition;
			vMoveVec.Normalize();

			float fDistance = Vector3.Distance(m_vDestination, m_vStartPosition);

			float fColliderSize = 1.0f;
			CapsuleCollider pCollider = m_pCaster.GetComponent<CapsuleCollider>();
			if (pCollider)
			{
				fColliderSize = pCollider.radius;
			}

			//-----
			Ray pRay = new Ray();
			pRay.origin = m_vStartPosition + (vMoveVec * fColliderSize);
			pRay.direction = vMoveVec;

			RaycastHit[] pvRayCasts = Physics.RaycastAll(pRay, fDistance - float.Epsilon);
			for (int nInd = 0; nInd < pvRayCasts.Length; ++nInd)
			{
				if (pvRayCasts[nInd].collider.isTrigger)
					continue;

				GameObj_PhysicIgnore pPhsIgnore = pvRayCasts[nInd].collider.GetComponent<GameObj_PhysicIgnore>();
				if (pPhsIgnore != null)
					continue;

				fDistance = Vector3.Distance(m_vStartPosition, pvRayCasts[nInd].point);
				m_vDestination = m_vStartPosition + (vMoveVec * (fDistance - fColliderSize));
				bCheckByPhysic = true;
				break;
			}
		}

		//-----
		if (_moveDistance > 0.0f)
		{
			bool bDistanceChange = true;
			if (bCheckByPhysic && Vector3.Distance(m_vStartPosition, m_vDestination) < _moveDistance)
				bDistanceChange = false;

			if (bDistanceChange)
			{
				Vector3 vDir = (m_vDestination - m_vStartPosition).normalized;
				m_vDestination = m_vStartPosition + (vDir * _moveDistance);
			}
		}

		//-----
		m_vStartPosition = m_pCaster.transform.position;
		m_vTargetPosition = m_vDestination;
		
		m_bSkillProcessing = false;
		m_fTotalMoveTime = 0.0f;
		m_fElapsedMoveTime = 0.0f;

		//-----
		m_pCaster.m_vMoveVector = Vector3.zero;
		m_pCaster.m_vViewVector = (m_vDestination - m_vStartPosition).normalized;

		m_pCaster.m_pRigidBody.velocity = Vector3.zero;
		m_pCaster.m_pRigidBody.angularVelocity = Vector3.zero;

		//-----
		if (m_pMoveStartBuff != null)
			m_pCaster.AddActorBuff(m_pMoveStartBuff);

		return true;
	}

	public override void SkillThrow_Child()
	{
		m_bSkillProcessing = true;
		m_fTotalMoveTime = 0.0f;

		//-----
		m_fTotalMoveTime = m_fMoveTime;
		m_fElapsedMoveTime = 0.0f;
	}


	/////////////////////////////////////////
	//---------------------------------------
	protected override void Frame_Child()
	{
		if (m_bSkillProcessing)
		{
			if (m_pCaster == null || m_pTarget == null)
			{
				m_bSkillProcessing = false;
				return;
			}

			m_fElapsedMoveTime += HT.TimeUtils.GameTime;

			if (m_fElapsedMoveTime < m_fTotalMoveTime)
			{
				m_pCaster.m_bManagedBySystem = true;

				//-----
				Vector3 vNowPos = m_pCaster.transform.position;
				vNowPos.y = m_vDestination.y;

				Vector3 vViewVec = m_vDestination - vNowPos;
				vViewVec.Normalize();

				m_pCaster.m_vViewVector = vViewVec;

				//-----
				float fRatio = m_fElapsedMoveTime / m_fTotalMoveTime;

				//float fDistance = Vector3.Distance (m_vDestination, vNowPos);
				//vViewVec = vViewVec * (fDistance * fRatio);
				//m_pCaster.m_pRigidBody.velocity = vViewVec;

				vNowPos = Vector3.Lerp(m_vStartPosition, m_vDestination, fRatio);

				//-----
				if (m_bJumpMove)
				{
					m_pCaster.m_pRigidBody.useGravity = false;

					float fDestPosY = 0.0f;
					float fHeightOffset = 0.0f;
					float fHeightDiff = m_vDestination.y - m_vStartPosition.y;

					bool bJump_Up = (fRatio < 0.5f) ? true : false;
					if (bJump_Up)
					{
						fDestPosY = m_vStartPosition.y;
						if (fHeightDiff > 0.0f)
							fHeightOffset = fHeightDiff;
					}
					else
					{
						fDestPosY = m_vDestination.y;
						if (fHeightDiff < 0.0f)
							fHeightOffset = -fHeightDiff;
					}
					
					float fHeight = (Mathf.Sin(Mathf.PI * fRatio) * (m_fJumpMove_Height + fHeightOffset)) + fDestPosY;
					vNowPos.y = fHeight;
				}

				//-----
				m_pCaster.transform.position = vNowPos;

			}
			else
			{
				m_pCaster.m_pRigidBody.velocity = Vector3.zero;
				m_pCaster.m_bManagedBySystem = false;

				//if (m_bJumpMove)
				//{
					m_pCaster.m_pRigidBody.useGravity = true;
					m_pCaster.transform.position = m_vDestination;
				//}

				//-----
				if (m_fMoveEndCamShake > 0.0f)
					CameraManager._Instance.SetCameraShake(m_fMoveEndCamShake);

				if (m_pMoveEndEffect != null)
				{
					ParticleSystem pEffect = HT.Utils.InstantiateFromPool(m_pMoveEndEffect);
					pEffect.transform.position = m_pCaster.transform.position;

					HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
				}

				if (m_pMoveStartBuff != null && _moveStartBuffOnlyDuringMove)
					m_pCaster.RemoveActorBuff(m_pMoveStartBuff, false);

				if (m_pMoveEndBuff != null)
					m_pCaster.AddActorBuff(m_pMoveEndBuff, m_fMoveEndBuffTimeOverwrap);

				HT.HTSoundManager.PlaySound(_moveEndSound);

				//-----
				if (m_eAreaAlertType != eAreaAlertType.eNotAlert && m_fSkillRealRange > 0.0f)
				{
					float fDist = Vector3.Distance(m_pCaster.transform.position, m_pTarget.transform.position);
					if (fDist <= m_fSkillRealRange)
					{
						m_pTarget.OnDamaged(m_nJumpAttackDamage);

						if (string.IsNullOrEmpty(_effectiveAcv_whenHit) == false)
						{
							Archives pArchives = ArchivementManager.Instance.FindArchive(_effectiveAcv_whenHit);
							pArchives.Archive.OnArchiveCount(1);
						}

						if (string.IsNullOrEmpty(_effectiveAcv_whenHit2) == false)
						{
							Archives pArchives = ArchivementManager.Instance.FindArchive(_effectiveAcv_whenHit2);
							pArchives.Archive.OnArchiveCount(1);
						}
					}
				}

				//-----
				CallSkillObject_Throw();

				m_pCaster.m_vMoveVector = Vector3.zero;
				if (m_pCaster.m_pRigidBody != null)
					m_pCaster.m_pRigidBody.velocity = Vector3.zero;

				//-----
				m_bSkillProcessing = false;
				m_fTotalMoveTime = 0.0f;
				m_fElapsedMoveTime = 0.0f;

				m_vStartPosition = Vector3.zero;
				m_vDestination = Vector3.zero;
			}
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override float GetCastDelayTime()
	{
		return m_fMoveTime;
	}


	/////////////////////////////////////////
	//---------------------------------------
}
