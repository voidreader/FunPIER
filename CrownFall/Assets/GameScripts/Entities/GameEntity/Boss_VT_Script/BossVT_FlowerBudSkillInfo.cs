using System;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossVT_FlowerBudSkillInfo : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("FLAMEWALL BUD")]
	public float[] _flamewall_RepeatTime = null;
	public BossVT_FlamewallTarget[] _flamewall_Target = null;
	public Projectile_Parabola[] _flamewall_Projectile = null;
	public Vector3 _flamewall_RangeMin = Vector3.zero;
	public Vector3 _flamewall_RangeMax = Vector3.zero;
	public float _flamewall_Height = 3.0f;
	public float _flamewall_Speed = 3.0f;
	public AudioClip _flamewall_Sounds = null;
	public ParticleSystem _flamewall_ShootEffect = null;

	//---------------------------------------
	[Header("FRENZY BUD")]
	public float _frenzy_RepeatTime = 0.0f;
	public Projectile_Parabola[] _frenze_Projectile = null;
	public int[] _frenzy_Count = null;
	public float _frenzy_Height = 3.0f;
	public float _frenzy_Speed = 3.0f;

	//---------------------------------------
	[Header("THORN BUD")]
	public float[] _thorn_RepeatTime = null;
	public Projectile_Trigger _thorn_Instance = null;
	public int[] _thorn_Count = null;
	public float[] _thorn_Speed = null;
	public int _thorn_Damage = 1;

	//---------------------------------------
	[Header("FREEZE BUD")]
	public float[] _freeze_RepeatTime = null;
	public Projectile_Parabola[] _freeze_Projectile = null;
	public float _freeze_Height = 3.0f;
	public float _freeze_Speed = 3.0f;

	//---------------------------------------
	[Header("COMMON - IGNORE PHYS LIST")]
	public GameObject[] _ignoreList = null;

	//---------------------------------------
	public float GetRepeatTime(BossVT_FlowerBud.eBudType eType)
	{
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		switch (eType)
		{
			case BossVT_FlowerBud.eBudType.FlameWall:
				return _flamewall_RepeatTime[nDiff];

			case BossVT_FlowerBud.eBudType.Frenzy:
				return _frenzy_RepeatTime;

			case BossVT_FlowerBud.eBudType.Thorns:
				return _thorn_RepeatTime[nDiff];

			case BossVT_FlowerBud.eBudType.Freezy:
				return _freeze_RepeatTime[nDiff];
		}

		return 0.0f;
	}
}


/////////////////////////////////////////
//---------------------------------------