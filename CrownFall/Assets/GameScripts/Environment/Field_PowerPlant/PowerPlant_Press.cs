using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class PowerPlant_Press : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private AudioClip _pressSound = null;
	[SerializeField]
	private float _pressSound_Ratio = 0.5f;

	private bool _presssed = false;

	//---------------------------------------
	public bool Presssed { get { return _presssed; } }

	//---------------------------------------
	public void PlaySound(AudioClip pClip)
	{
		if (pClip != null)
			HT.HTSoundManager.PlaySound(pClip);
	}

	public void SetPressState_On()
	{
		_presssed = true;
	}

	public void SetPressState_OFF()
	{
		_presssed = false;
	}

	//---------------------------------------
	public void OnPressSound()
	{
		HT.HTSoundManager.PlaySound(_pressSound, _pressSound_Ratio);
	}
}


/////////////////////////////////////////
//---------------------------------------