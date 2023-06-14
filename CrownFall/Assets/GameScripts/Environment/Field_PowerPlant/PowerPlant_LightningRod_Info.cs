using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class PowerPlant_LightningRod_Info : MonoBehaviour
{
	//---------------------------------------
	[Serializable]
	public struct LightningEffect
	{
		[Header("TIMES")]
		public float _when;
		public float _time;
		public float _delay;

		[Header("INFO")]
		public float _width;
		public float _size;
		public int _step;

		[Header("LIGHT")]
		public float _intensity;
		public float _range;
		public float _lightTime;
	}

	//---------------------------------------
	[Header("ROD OBJECTS")]
	public int _lightningDamage = 1;
	public float _areaAlertTimeExpand = 0.5f;
	public ParticleSystem _rodParticle = null;

	[Header("EFFECT SETTINGS")]
	public HT.HTLightning _instance_Lightning = null;
	public LightningEffect[] _lightningInfo = null;
	public LightningEffect _mainLightningInfo;

	[Header("SOUNDS")]
	public AudioClip _sound_LightningEffect = null;
	public float _sound_LightingEffect_Volume = 0.85f;

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------