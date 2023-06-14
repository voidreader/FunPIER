using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSkill_SimpleDamage : ActorSkill
{
	//---------------------------------------
	[Header("SIMPLE DAMAGE SETTINGS")]
	[SerializeField]
	private int _damage = 1;
	[SerializeField]
	private ParticleSystem _throwParticles = null;
    [SerializeField]
    private bool _refreshViewVector = false;

	//---------------------------------------
	private Vector3 _attackDirection = Vector3.zero;

	//---------------------------------------
	public override void ResetAll()
	{
	}

	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
        if (_refreshViewVector)
        {
            Vector3 vViewVector = (m_pTarget.transform.position - m_pCaster.transform.position).normalized;
            m_pCaster.m_vViewVector = vViewVector;
        }

		_attackDirection = m_pCaster.m_vViewVector;

		return true;
	}

	public override void SkillThrow_Child()
	{
		Vector3 vDamageCheckPos = GetAlertTargetPos();

		if (_throwParticles != null)
		{
			ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_throwParticles);
			pEffect.transform.position = vDamageCheckPos;

			HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
		}
		
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
			switch(_ringType)
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

			m_pTarget.OnDamaged(_damage);
		}
		while (false);

		CallSkillObject_Throw();
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------