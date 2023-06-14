using UnityEngine;
using System.Collections;


/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSkill_DynamicPolys : ActorSkill {
	/////////////////////////////////////////
	//---------------------------------------
	public GameObject m_pInst_MeshObject;

	public enum eMeshManagerType {
		eUnknown = 0,
		eDarkKnightSword,
	};

	public eMeshManagerType m_eMeshManager;
	public Color m_pMatColor;

	//---------------------------------------
	public float m_fPolyCreationTime = 1.0f;
	public float m_fPolyLifeTime = 1.0f;
	public int m_nCollisionDamage;

	//---------------------------------------
	private Vector3 _attackDirection = Vector3.zero;


	/////////////////////////////////////////
	//---------------------------------------
	public override bool SkillCastReady_Child ()
	{
		_attackDirection = m_pCaster.m_vViewVector;
		return true;
	}


	public override void SkillThrow_Child () {
		GameObject pObj = HT.Utils.Instantiate(m_pInst_MeshObject);
		//pObj.transform.position = m_pCaster.transform.position;

		float fCreateTime = m_fPolyCreationTime;
		switch (m_eMeshManager) {
		case eMeshManagerType.eDarkKnightSword:
			BossDK_SwordScript._Instance.m_pDynamicMeshFilter = pObj;

			if (fCreateTime <= 0.0f && (m_szThrowAnimation != null && m_szThrowAnimation.Length > 0)) {
				Animation pAnim = m_pCaster.GetComponent<Animation> ();
				fCreateTime = pAnim.GetClip (m_szThrowAnimation).length;
			}

			BossDK_SwordScript._Instance.ResetDynamicPolys (fCreateTime);
			break;
		}

		//-----
		ActorSkill_DynamicPolys_Instance pInstance = pObj.GetComponent<ActorSkill_DynamicPolys_Instance>();
		pInstance.m_fLifeTime = fCreateTime + m_fPolyLifeTime;
		pInstance.m_pNowColor = m_pMatColor;

		pInstance.m_pSkill_DynPolys = this;

		pInstance.m_pParent = m_pCaster;
		pInstance.m_pTarget = m_pTarget;

		//-----
		Vector3 vDamageCheckPos = GetAlertTargetPos();
		float fDist = Vector3.Distance(vDamageCheckPos, m_pTarget.transform.position);

		do
		{
			if (fDist > m_fSkillRealRange)
				break;

			Vector3 vViewVec = _attackDirection;
			Vector3 vTargetVec = (m_pTarget.transform.position - m_pCaster.transform.position).normalized;

			float fInner = Vector3.Dot(vViewVec, vTargetVec);
			float fInnerDegree = 180.0f - (fInner * 180.0f);

			bool bIsIn = true;
			switch (_ringType)
			{
				case eAlertRingType.Angle360:
					break;

				case eAlertRingType.Angle180:
					if (fInnerDegree > 180.0f)
						bIsIn = false;
					break;

				case eAlertRingType.Angle130:
					if (fInnerDegree > 130.0f)
						bIsIn = false;
					break;

				case eAlertRingType.Angle90:
					if (fInnerDegree > 90.0f)
						bIsIn = false;
					break;
			}

			if (bIsIn == false)
				break;

			if (string.IsNullOrEmpty(_effectiveAcv_whenHit) == false)
			{
				Archives pArchives = ArchivementManager.Instance.FindArchive(_effectiveAcv_whenHit);
				pArchives.Archive.OnArchiveCount(1);
			}

			m_pTarget.OnDamaged(m_nCollisionDamage);
		}
		while (false);

		//-----
		CallSkillObject_Throw ();
	}
}
