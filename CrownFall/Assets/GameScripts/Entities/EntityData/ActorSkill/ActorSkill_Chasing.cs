using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSkill_Chasing : ActorSkill
{
	/////////////////////////////////////////
	//---------------------------------------
	public float m_fChasingTime;
	public float m_fChasingSpeed;

	//---------------------------------------
	bool m_bSkillProcessing = false;
	float m_fElapsedMoveTime = 0.0f;


	/////////////////////////////////////////
	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
		return true;
	}

	public override void SkillThrow_Child()
	{
		m_bSkillProcessing = true;
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

			//-----
			m_fElapsedMoveTime += HT.TimeUtils.GameTime;

			if (m_fElapsedMoveTime >= m_fChasingTime)
			{
				m_bSkillProcessing = false;
			}
			else
			{
				Vector3 vVector = m_pTarget.transform.position - m_pCaster.transform.position;
				vVector.y = 0.0f;
				vVector.Normalize();

				if (vVector.sqrMagnitude > 0.0f)
					m_pCaster.m_vViewVector = vVector;

				vVector *= (m_fChasingSpeed * HT.TimeUtils.frame60DeltaTime);
				vVector.y = m_pCaster.m_pRigidBody.velocity.y;

				m_pCaster.m_pRigidBody.velocity = vVector;
			}

		}
	}
}
