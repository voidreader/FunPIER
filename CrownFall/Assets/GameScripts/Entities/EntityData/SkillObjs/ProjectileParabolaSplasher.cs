using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class ProjectileParabolaSplasher : ObjectSplasher
{
	//---------------------------------------
	[Header("PROJECTILE SPLASHER INFO")]
	public Projectile_Parabola m_pProjectile = null;

	public float _fallDest_DistMin = 0.0f;
	public float _fallDest_DistMax = 0.0f;

	public float _splashDirMin = 0.0f;
	public float _splashDirMax = 0.0f;


	/////////////////////////////////////////
	//---------------------------------------
	protected override void CreateProjectiles()
	{
		for (int nInd = 0; nInd < m_nProjectileCount; ++nInd)
		{
			Projectile_Parabola pNewProj = HT.Utils.Instantiate(m_pProjectile);

			pNewProj.m_pParent = m_pCaster;
			pNewProj.m_pSkill_Proj = null;

			//-----
			if (m_fProjectileLifeTime > 0.0f)
				HT.Utils.SafeDestroy(pNewProj.gameObject, m_fProjectileLifeTime);

			pNewProj._damage = m_nProjectileDamage;
			pNewProj._flyHeight = Random.Range(m_fProjectileMinViewHeight, m_fProjectileMaxViewHeight);
			pNewProj._speed = Random.Range(m_fProjectileMoveSpeed_Min, m_fProjectileMoveSpeed_Max);

			//-----
			Vector3 vPos = gameObject.transform.position;
			float fProjectileRadius = 0.1f;
			SphereCollider pSphere = pNewProj.GetComponent<SphereCollider>();
			if (pSphere != null)
			{
				fProjectileRadius = pSphere.radius;
			}

			Vector3 vMoveVec = gameObject.transform.forward;
			vMoveVec.y = 0.0f;
			vMoveVec.Normalize();

			float fRot = Random.Range(_splashDirMin, _splashDirMax);
			vMoveVec = Quaternion.Euler(0.0f, fRot, 0.0f) * vMoveVec;

			//-----
			Vector3 vStartPos = vPos + (vMoveVec * fProjectileRadius);

			Vector3 vDestPos = vStartPos + (vMoveVec * Random.Range(_fallDest_DistMin, _fallDest_DistMax));
			vDestPos = GameFramework._Instance.GetPositionByPhysic(vDestPos);

			pNewProj.Init(vStartPos, vDestPos);
			pNewProj.UpdateRotate();
		}
	}
}


/////////////////////////////////////////
//---------------------------------------