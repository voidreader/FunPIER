using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSkill_Projectile_Parabola : ActorSkill
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("SKILL SET")]
	[SerializeField]
	private Projectile_Parabola _projectile = null;
	[SerializeField]
	private int _projectileCount = 1;

	//---------------------------------------
	public int m_nProjectileDamage = 1;
	public float m_fProjectileSpeed = 0.0f;
	public float m_fProjectileHeight = 1.0f;

	//---------------------------------------
	[SerializeField]
	private bool _isSpread_Regular = true;
	public float m_fProj_Spread_Range = 0.0f;
	public float m_fLifeTime;

	//---------------------------------------
	[Header("SPAWN OBJECT - CONDITION")]
	[SerializeField]
	private ISkillObject _projectileSpawnObj = null;
	[SerializeField]
	private int _spawnProjectileCount = 0;


	/////////////////////////////////////////
	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
		return true;
	}

	public override void SkillThrow_Child()
	{
		if (_projectile == null)
			return;
		
		float fProjSize = 0.0f;
		SphereCollider pCollider = _projectile.GetComponent<SphereCollider>();
		if (pCollider != null)
			fProjSize = pCollider.radius;

		int nLeastSpawnObjCount = _spawnProjectileCount;
		Vector3 vFront = Quaternion.Euler(0.0f, 360.0f, 0.0f) * Vector3.forward;
		for (int nInd = 0; nInd < _projectileCount; ++nInd)
		{
			Projectile_Parabola pProj = HT.Utils.Instantiate(_projectile);
			if (m_fLifeTime > 0.0f)
				HT.Utils.SafeDestroy(pProj.gameObject, m_fLifeTime);

			//-----
			pProj.m_pSkill_Proj = this;
			pProj.m_pParent = m_pCaster;
			
			//-----
			pProj._damage = m_nProjectileDamage;
			pProj._speed = m_fProjectileSpeed;
			pProj._flyHeight = m_fProjectileHeight;

			//-----
			Vector3 vDestPos = Vector3.zero;
			if (m_bIsEnemySkill)
				vDestPos = BattleFramework._Instance.m_pPlayerActor.transform.position;
			else
				vDestPos = BattleFramework._Instance.m_pEnemyActor.transform.position;

			if (m_fProj_Spread_Range > 0.0f)
			{
				Vector3 vVec;
				if (_isSpread_Regular)
				{
					float fDot = 360.0f / _projectileCount;
					vVec = Quaternion.Euler(0.0f, fDot * nInd, 0.0f) * vFront;
					vVec = HT.RandomUtils.GetRandomVector(vVec, fDot);
				}
				else
				{
					vVec = new Vector3(Random.Range(0.0f, 1.0f), 0.0f, Random.Range(0.0f, 1.0f));
				}

				float fDistance = Random.Range(0.0f, m_fProj_Spread_Range);

				//-----
				Ray pRay = new Ray();
				pRay.origin = vDestPos;
				pRay.direction = vVec;
				
				RaycastHit[] vInfo = Physics.RaycastAll(pRay, fDistance);
				if (vInfo != null && vInfo.Length > 0)
				{
					for(int nRay = 0; nRay < vInfo.Length; ++nRay)
					{
						if (vInfo[nRay].collider.GetComponent<IActorBase>() != null)
						{
							fDistance = vInfo[nRay].distance - 0.1f;
							break;
						}
					}
				}

				//-----
				vDestPos += vVec.normalized * fDistance;
			}
			
			//-----
			float fColliderRadius = 1.0f;
			CapsuleCollider pCapsule = m_pCaster.GetComponent<CapsuleCollider>();
			if (pCapsule != null)
				fColliderRadius = pCapsule.radius;

			Vector3 vOffset = m_pCaster.transform.position;
			if (string.IsNullOrEmpty(_throwOffsetDummy) == false)
			{
				GameObject pPivot = m_pCaster.FindDummyPivot(_throwOffsetDummy);
				if (pPivot != null)
					vOffset = pPivot.transform.position;
			}

			float fProjOffset = (fProjSize + 0.25f) * nInd;
			pProj.Init(vOffset + (pProj.GetMoveVector() * (fColliderRadius + fProjOffset)), GameFramework._Instance.GetPositionByPhysic(vDestPos));

			//-----
			if (_projectileSpawnObj != null && nLeastSpawnObjCount > 0)
			{
				--nLeastSpawnObjCount;
				pProj.m_pSpawnObjectWhenExplode = _projectileSpawnObj;
			}
		}
	}
}


/////////////////////////////////////////
//---------------------------------------