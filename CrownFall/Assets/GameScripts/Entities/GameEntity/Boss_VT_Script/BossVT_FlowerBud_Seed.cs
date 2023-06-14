using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossVT_FlowerBud_Seed : MonoBehaviour
{
	//---------------------------------------
	private Action _spreadSeedAnimCallback = null;

	//---------------------------------------
	public void SetSpreadSeedAnimCallback(Action pCallback)
	{
		_spreadSeedAnimCallback = pCallback;
	}

	//---------------------------------------
	public void OnSeedAnim()
	{
		if (_spreadSeedAnimCallback != null)
			_spreadSeedAnimCallback();
	}
}


/////////////////////////////////////////
//---------------------------------------