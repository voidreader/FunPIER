using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class PowerPlant_LightningRod_Group : MonoBehaviour
{
	//---------------------------------------
	[Header("INFO")]
	[SerializeField]
	private PowerPlant_LightningRod_Info _info = null;
	[SerializeField]
	private AudioClip _smallSparkSounds = null;

	[Header("ROD OBJECTS")]
	[SerializeField]
	private PowerPlant_LightningRod _rod_L = null;
	[SerializeField]
	private PowerPlant_LightningRod _rod_R = null;

	[Header("EFFECT SETTINGS")]
	[SerializeField]
	private Vector2 _alertRect_Scale = Vector2.one;
	[SerializeField]
	private float _alertRect_RotateY = 0.0f;

	//---------------------------------------
	private bool _playerEntered = false;
	private Coroutine _rodProc = null;

	private HT.HTLightning _lightning = null;

	//---------------------------------------
	void Awake()
	{
		_lightning = HT.Utils.Instantiate(_info._instance_Lightning);
		_lightning.transform.SetParent(gameObject.transform);
	}

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		if (other == null)
			return;

		if (BattleFramework._Instance == null || BattleFramework._Instance.m_pPlayerActor == null)
			return;

		if (other.gameObject.gameObject == BattleFramework._Instance.m_pPlayerActor.gameObject)
			_playerEntered = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.gameObject == BattleFramework._Instance.m_pPlayerActor.gameObject)
			_playerEntered = false;
	}

	//---------------------------------------
	public void ActivateRod(float fDelay)
	{
		if (_rodProc != null)
			return;

		_rodProc = StartCoroutine(ActivateRod_Internal(fDelay));
	}

	private IEnumerator ActivateRod_Internal(float fDelay)
	{
		Object_AreaAlert pAlert = BattleFramework._Instance.CreateAreaAlert_Simple(_rod_R.transform.position, _alertRect_Scale.x, _alertRect_Scale.y, fDelay + _info._areaAlertTimeExpand);
		pAlert.transform.rotation = Quaternion.Euler(new Vector3(0.0f, _alertRect_RotateY, 0.0f));

		float fCurTime = 0.0f;
		if (_info._lightningInfo != null && _info._lightningInfo.Length > 0)
		{
			int nCurInd = 0;
			while (true)
			{
				if (nCurInd >= _info._lightningInfo.Length)
					break;

				if (fCurTime >= fDelay)
					break;

				float fReady = _info._lightningInfo[nCurInd]._when - fCurTime;
				if (fReady > 0.0f)
					yield return new WaitForSeconds(fReady);

				LightningOn(ref _info._lightningInfo[nCurInd], false);

				HT.HTSoundManager.PlaySound(_smallSparkSounds);

				fCurTime = _info._lightningInfo[nCurInd]._when;
				++nCurInd;
			}
		}

		if (fCurTime < fDelay)
			yield return new WaitForSeconds(fDelay - fCurTime);

		LightningOn(ref _info._mainLightningInfo, true);
		HT.HTSoundManager.PlaySound(_info._sound_LightningEffect, _info._sound_LightingEffect_Volume);

		if (_playerEntered)
			BattleFramework._Instance.m_pPlayerActor.OnDamaged(_info._lightningDamage);

		yield return new WaitForSeconds(_info._mainLightningInfo._time + _info._mainLightningInfo._delay);

		_rodProc = null;
	}

	private void LightningOn(ref PowerPlant_LightningRod_Info.LightningEffect pInfo, bool bShowParticle)
	{
		_lightning.SetLightningInfo(pInfo._width, pInfo._size, pInfo._step);
		_lightning.Init(_rod_L.Rod_Light.gameObject, _rod_R.Rod_Light.gameObject, pInfo._time, pInfo._delay);

		_rod_L.ActivateLight(pInfo._lightTime, pInfo._intensity, pInfo._range, bShowParticle);
		_rod_R.ActivateLight(pInfo._lightTime, pInfo._intensity, pInfo._range, bShowParticle);
	}

	//---------------------------------------
	public PowerPlant_LightningRod GetRod(bool bLeft)
	{
		return (bLeft) ? _rod_L : _rod_R;
	}
}


/////////////////////////////////////////
//---------------------------------------