using System;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class AttackChargeHelper : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private Animation _attackChargeAnim = null;
	[SerializeField]
	private string _attackChargeAnim_IDLE = null;
	[SerializeField]
	private string _attackChargeAnim_AIM = null;
	[SerializeField]
	private string _attackChargeAnim_JUSTAIM = null;

	//---------------------------------------
	private float _chargeRatio = 0.0f;
	public float ChargeRatio
	{
		get { return _chargeRatio; }
		set { _chargeRatio = value; }
	}

	private float _leastTime = -1.0f;
	private Action _onEndCharge = null;

	//---------------------------------------
	public bool IsCharging { get { return (_leastTime > 0.0f) ? true : false; } }

	//---------------------------------------
	public void SetChargeRate(float fRate)
	{
		_chargeRatio = fRate;
	}

	//---------------------------------------
	public void StartChargeAnim(float fTime, Action onEndCharge)
	{
		_leastTime = fTime;
		_onEndCharge = onEndCharge;

		_attackChargeAnim.Play(_attackChargeAnim_AIM);
	}

	public void StopChargeAnim()
	{
		_leastTime = -1.0f;
		_onEndCharge = null;

		_attackChargeAnim.Play(_attackChargeAnim_IDLE);
	}

	//---------------------------------------
	public void ShowJustAim()
	{
		_attackChargeAnim.Play(_attackChargeAnim_JUSTAIM);
	}

	//---------------------------------------
	private void Update()
	{
		if (_leastTime > 0.0f)
		{
			_leastTime -= HT.TimeUtils.GameTime;
			if (_leastTime <= 0.0f)
			{
				HT.Utils.SafeInvoke(_onEndCharge);
				StopChargeAnim();
			}
		}
	}
}


/////////////////////////////////////////
//---------------------------------------