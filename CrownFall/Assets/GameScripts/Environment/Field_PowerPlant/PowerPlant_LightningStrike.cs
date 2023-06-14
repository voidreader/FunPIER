using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class PowerPlant_LightningStrike : MonoBehaviour
{
	//---------------------------------------
	[Header("INFO")]
	[SerializeField]
	private PowerPlant_LightningRod_Info _info = null;

	[Header("OBJECTS")]
	[SerializeField]
	private SphereCollider _collider = null;
	[SerializeField]
	private float[] _colliderRange = null;
	[SerializeField]
	private GameObject _pivot1 = null;
	[SerializeField]
	private GameObject _pivot2 = null;

	//---------------------------------------
	private bool _playerEntered = false;
	private Coroutine _rodProc = null;

	private float _damaageRange = 1.0f;

	private HT.HTLightning _lightning = null;
	private ParticleSystem _strikeParticle = null;

	//---------------------------------------
	void Awake()
	{
		_collider.radius = _colliderRange[(int)GameFramework._Instance.m_pPlayerData.m_eDifficulty];
		_damaageRange = _collider.radius;

		_lightning = HT.Utils.Instantiate(_info._instance_Lightning);
		_lightning.transform.SetParent(gameObject.transform);

		_strikeParticle = HT.Utils.Instantiate(_info._rodParticle);
		_strikeParticle.transform.position = _pivot1.transform.position;
		_strikeParticle.transform.SetParent(gameObject.transform);
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
	public void ActivateStike(Vector3 vPos, float fDelay)
	{
		if (_rodProc != null)
			return;

		gameObject.transform.position = vPos;
		_rodProc = StartCoroutine(ActivateRod_Internal(fDelay));
	}

	private IEnumerator ActivateRod_Internal(float fDelay)
	{
        float alertRadius = _damaageRange * _collider.transform.lossyScale.x;
        BattleFramework._Instance.CreateAreaAlert(gameObject.transform.position, alertRadius, fDelay + _info._areaAlertTimeExpand);

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
		_lightning.Init(_pivot1, _pivot2, pInfo._time, pInfo._delay);

		_strikeParticle.Play();
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------