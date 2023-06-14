using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class BossBC_InvincibleHP : MonoBehaviour
{
	//---------------------------------------
	public static BossBC_InvincibleHP _instance = null;

	//---------------------------------------
	public Slider _hpSlider = null;

	//---------------------------------------
	private void Awake()
	{
		_instance = this;
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------