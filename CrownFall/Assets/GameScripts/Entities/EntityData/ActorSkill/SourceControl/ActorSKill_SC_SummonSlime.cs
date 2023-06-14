using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
[Serializable]
public struct PosionSpreadInfo
{
	public float fFillMin;
	public float fFillMax;

	public int nPoisonCount;

	public Projectile_Parabola pParabola;
}


/////////////////////////////////////////
//---------------------------------------
public class ActorSKill_SC_SummonSlime : ActorSkill
{
	//---------------------------------------
	[Header("SOURCE CONTROL INFO")]
	[SerializeField]
	private GameObject _instance_EmptyObj = null;
	[SerializeField]
	private Vector3 _summonDest = Vector3.zero;
	[SerializeField]
	private float _stopWhenDistance = 1.0f;
	[SerializeField]
	private ActorBuff _removeBuffWhenCast = null;
	[SerializeField]
	private ActorBuff _removeBuffWhenThrow = null;

	[Header("SOURCE CONTROL INFO - ANIM")]
	[SerializeField]
	private AudioClip _summonSound = null;
	[SerializeField]
	private AudioClip _summonSound2 = null;
	[SerializeField]
	private ParticleSystem _summonEffect = null;
	[SerializeField]
	private string _anim_CastSummon = null;
	[SerializeField]
	private string _anim_EndSummon = null;

	[Header("SPREAD POISON INFO")]
	[SerializeField]
	private PosionSpreadInfo[] _summonInfo = null;
	[SerializeField]
	private ProjectileParabolaSplasher _instance_Splasher = null;

	//---------------------------------------
	private bool _skillProcessing = false;
	private GameObject _moveTarget = null;
	private ProjectileParabolaSplasher _createdSplasher = null;

	//---------------------------------------
	public override void ResetAll()
	{
		base.ResetAll();

		_skillProcessing = false;
		_moveTarget = null;
	}

	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
		if (m_pCaster.GetActorType() != IActorBase.eActorType.eAI)
			return false;

		if (_moveTarget == null)
		{
			_moveTarget = HT.Utils.Instantiate(_instance_EmptyObj);
			_moveTarget.SetActive(false);
		}

		Vector3 vMoveVec = (m_pCaster.transform.position - _summonDest).normalized;
		vMoveVec = _summonDest + (vMoveVec * _stopWhenDistance);
		_moveTarget.transform.position = GameFramework._Instance.GetPositionByPhysic(vMoveVec);

		AIActor pActor = m_pCaster as AIActor;
		pActor.ChasingTarget = _moveTarget;

		_skillProcessing = true;

		if (_removeBuffWhenCast != null)
			m_pCaster.RemoveActorBuff(_removeBuffWhenCast, false);

		return true;
	}

	protected override void Frame_Child()
	{
		if (_moveTarget == null)
			_skillProcessing = false;

		if (_skillProcessing)
		{
			if (Vector3.Distance(_moveTarget.transform.position, m_pCaster.transform.position) < 2.0f)
			{
				_skillProcessing = false;
				SkillThrow();
			}
		}
	}

	public override void SkillThrow_Child()
	{
		float fTime = m_pCaster.SetAction(_anim_CastSummon);
		Invoke("SkillThrow_Invoke", fTime);
	}

	//---------------------------------------
	private void SkillThrow_Invoke()
	{
		m_pCaster.SetAction(_anim_EndSummon);
		HT.HTSoundManager.PlaySound(_summonSound);
		HT.HTSoundManager.PlaySound(_summonSound2);

		AIActor pActor = m_pCaster as AIActor;
		pActor.ChasingTarget = null;

		if (_removeBuffWhenThrow != null)
			m_pCaster.RemoveActorBuff(_removeBuffWhenThrow, false);

		//-----
		if (_summonEffect != null)
		{
			ParticleSystem pParticle = HT.Utils.InstantiateFromPool(_summonEffect);
			pParticle.transform.position = _moveTarget.transform.position;

			HT.Utils.SafeDestroy(pParticle.gameObject, pParticle.TotalSimulationTime());
		}

		if (_createdSplasher == null)
		{
			_createdSplasher = HT.Utils.Instantiate(_instance_Splasher);

			_createdSplasher._autoDestroy = false;
			_createdSplasher.m_bSplasherOnce = true;

			_createdSplasher.DisableSplasher();
		}

		Field_PoisonFactory pField = BattleFramework._Instance.m_pField as Field_PoisonFactory;
		float fFillRatio = pField.PoisonFillRatio;

		PosionSpreadInfo pInfo = _summonInfo[0];
		for(int nInd = 0; nInd < _summonInfo.Length; ++nInd)
		{
			if (_summonInfo[nInd].fFillMin <= fFillRatio && _summonInfo[nInd].fFillMax > fFillRatio)
			{
				pInfo = _summonInfo[nInd];
				break;
			}
		}

		_createdSplasher.transform.position = _moveTarget.transform.position;

		_createdSplasher.m_pCaster = m_pCaster;
		_createdSplasher.m_nProjectileCount = pInfo.nPoisonCount;
		_createdSplasher.m_pProjectile = pInfo.pParabola;

		_createdSplasher.CreateProjectile_Manual();
	}
}


/////////////////////////////////////////
//---------------------------------------