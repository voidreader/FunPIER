using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public sealed class ProjectileSplasher : ObjectSplasher
{
	//---------------------------------------
	[Header("PROJECTILE")]
	public Projectile m_pProjectile;

	//---------------------------------------
	protected override void CreateProjectiles()
	{
		for (int nInd = 0; nInd < m_nProjectileCount; ++nInd)
		{
			Projectile pNewProj = HT.Utils.Instantiate(m_pProjectile);

			pNewProj.m_pParent = m_pCaster;

			//-----
			Vector3 vViewVec = Vector3.zero;
			vViewVec.x = Random.Range(-1.0f, 1.0f);
			vViewVec.z = Random.Range(-1.0f, 1.0f);
			vViewVec.y = Random.Range(m_fProjectileMinViewHeight, m_fProjectileMaxViewHeight);
			vViewVec.Normalize();

			pNewProj.m_vMoveVector = vViewVec;
			pNewProj.m_fSpeed = Random.Range(m_fProjectileMoveSpeed_Min, m_fProjectileMoveSpeed_Max);
			pNewProj.UpdateRotate();

			if (m_fProjectileLifeTime > 0.0f)
				HT.Utils.SafeDestroy(pNewProj.gameObject, m_fProjectileLifeTime);

			pNewProj.m_nProjectileDamage = m_nProjectileDamage;

			//-----
			float fColliderRadius = 1.0f;
			CapsuleCollider pCapsule = m_pCaster.GetComponent<CapsuleCollider>();
			if (pCapsule != null)
			{
				fColliderRadius = pCapsule.radius;
			}

			float fProjectileRadius = 0.1f;
			SphereCollider pSphere = pNewProj.GetComponent<SphereCollider>();
			if (pSphere != null)
			{
				fProjectileRadius = pSphere.radius;
			}

			Vector3 vPos = pNewProj.transform.position;
			pNewProj.Init(vPos + m_pCaster.transform.position + (pNewProj.m_vMoveVector * (fColliderRadius + fProjectileRadius + 0.1f)));
		}
	}

	//---------------------------------------
}
