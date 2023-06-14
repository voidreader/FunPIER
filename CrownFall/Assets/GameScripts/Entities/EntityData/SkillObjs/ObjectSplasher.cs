using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;


/////////////////////////////////////////
//---------------------------------------
public class ObjectSplasher : ISkillObject
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("SPLASHER INFO")]
	public float m_fSplasherLifeTime;
	public bool m_bSplasherOnce = false;
	public bool _autoDestroy = true;
	
	public float m_fProjectileLifeTime;
	public float m_fProjectileMoveSpeed_Min;
	public float m_fProjectileMoveSpeed_Max;

	public float m_fProjectileMinViewHeight;
	public float m_fProjectileMaxViewHeight;

	[FormerlySerializedAs("m_fProjectileRegenRate")]
	public float _regenRate_Min = 0.0f;
	public float _regenRate_Max = 0.0f;

	public int m_nProjectileCount;
	public int m_nProjectileDamage = 1;

	//---------------------------------------
	protected float m_fRegenDelay;

	//---------------------------------------
	protected float m_fLifeTime;
	protected bool m_bDisableSplasher = false;


	/////////////////////////////////////////
	//---------------------------------------
	public override void Init()
	{
		m_fLifeTime = m_fSplasherLifeTime;
		m_fRegenDelay = 0.0f;
	}

	public override void Frame()
	{
		if (m_bDisableSplasher)
			return;

		if (m_bSplasherOnce)
		{
			CreateProjectiles();
			DisableSplasher();
		}
		else
		{
			m_fRegenDelay -= HT.TimeUtils.GameTime;
			if (m_fRegenDelay <= 0.0f)
			{
				if (_regenRate_Max < _regenRate_Min)
					m_fRegenDelay = _regenRate_Min;
				else
					m_fRegenDelay = Random.Range(_regenRate_Min, _regenRate_Max);
						
				CreateProjectiles();
			}

			m_fLifeTime -= HT.TimeUtils.GameTime;
			if (m_fLifeTime <= 0.0f)
				DisableSplasher();
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void DisableSplasher()
	{
		m_bDisableSplasher = true;

		if (_autoDestroy)
			HT.Utils.SafeDestroy(gameObject);
	}

	protected virtual void CreateProjectiles()
	{
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void CreateProjectile_Manual()
	{
		CreateProjectiles();
	}
}


/////////////////////////////////////////
//---------------------------------------