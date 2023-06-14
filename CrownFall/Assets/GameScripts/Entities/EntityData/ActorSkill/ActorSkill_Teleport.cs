using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSkill_Teleport : ActorSkill {
	/////////////////////////////////////////
	//---------------------------------------
	public enum eTeleportType {
		eTargetPos = 0,
		eFrontSide,
		eMousePos,
	}
	public eTeleportType m_eTeleportType;

	public float m_fMaxMoveDistance = 0;
	Vector3 m_vMovePos;

	//---------------------------------------


	/////////////////////////////////////////
	//---------------------------------------
	public override bool SkillCastReady_Child () {
		Vector3 vStartPos = m_pCaster.transform.position;

		//-----
		switch (m_eTeleportType) {
		case eTeleportType.eTargetPos:
			m_vMovePos = m_pTarget.transform.position;
			m_fMaxMoveDistance = Vector3.Distance (vStartPos, m_vMovePos);
			break;

		case eTeleportType.eFrontSide:
			m_vMovePos = m_pCaster.transform.position + (m_pCaster.m_vViewVector * m_fMaxMoveDistance);
			break;
		}

		//-----
		Vector3 vMoveVec = m_vMovePos - m_pCaster.transform.position;
		vMoveVec.Normalize ();
		
		//-----
		float fColliderSize = 1.0f;
		CapsuleCollider pCollider = m_pCaster.GetComponent<CapsuleCollider> ();
		if (pCollider) {
			fColliderSize = pCollider.radius;
		}
		
		//-----
		Ray pRay = new Ray ();
		pRay.origin = vStartPos + (vMoveVec * fColliderSize);
		pRay.direction = vMoveVec;
		
		RaycastHit [] pvRayCasts = Physics.RaycastAll (pRay, m_fMaxMoveDistance - float.Epsilon);
		for (int nInd = 0; nInd < pvRayCasts.Length; ++nInd) {
			if (pvRayCasts [nInd].collider.isTrigger)
				continue;
		
			GameObj_PhysicIgnore pPhsIgnore = pvRayCasts [nInd].collider.GetComponent<GameObj_PhysicIgnore> ();
			if (pPhsIgnore != null)
				continue;

			float fDistance = Vector3.Distance (vStartPos, pvRayCasts [nInd].point);
			if (fDistance >= m_fMaxMoveDistance)
				continue;
		
			m_vMovePos = vStartPos + (vMoveVec * (fDistance - fColliderSize));
			break;
		}

		return true;
	}

	public override void SkillThrow_Child () {
		m_pCaster.transform.position = m_vMovePos;

		//-----

		//-----
		CallSkillObject_Throw ();
	}


	/////////////////////////////////////////
	//---------------------------------------
}
