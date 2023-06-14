using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class PlayerHPGauge : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private bool _isSpecialGauge = false;
	[SerializeField]
	private Image _gauge = null;
	[SerializeField]
	private CanvasGroup _gaugeEffect_WhenFull = null;
	[SerializeField]
	private CanvasGroup _gaugeEffect_WhenFull_Button = null;
	[SerializeField]
	private CanvasGroup _gaugeEffect_WhenFull_Notice = null;
	[SerializeField]
	private AudioClip _gaugeEffect_WhenFull_Notice_Sound = null;

	[SerializeField]
	private ControllableActor _playerActor = null;

	//---------------------------------------
	private void OnEnable()
	{
		if (_gaugeEffect_WhenFull != null)
			StartCoroutine(GaugeEffect_WhenFull_Internal());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	//---------------------------------------
	private void FixedUpdate()
	{
		if (_playerActor == null)
		{
			if (BattleFramework._Instance == null)
				return;

			_playerActor = BattleFramework._Instance.m_pPlayerActor as ControllableActor;
		}

		//-----
		float fPrevAmount = _gauge.fillAmount;
		if (_isSpecialGauge)
			_gauge.fillAmount = _playerActor.SpcAtk_CurCharged / (float)_playerActor.SpcAtk_ChargeMax;
		else
			_gauge.fillAmount = _playerActor.GetCurrHP() / (float)_playerActor.GetMaxHP();

		//-----
		if (_gaugeEffect_WhenFull_Notice != null && fPrevAmount < 1.0f && _gauge.fillAmount >= 1.0f)
			StartCoroutine(GaugeEffect_WhenFull_Notice_Internal());
	}

	//---------------------------------------
	private IEnumerator GaugeEffect_WhenFull_Internal()
	{
		_gaugeEffect_WhenFull.alpha = 0.0f;

		//-----
		while(true)
		{
			if (_gauge.fillAmount < 1.0f)
			{
				yield return new WaitForFixedUpdate();
				continue;
			}

			//-----
			float fTime = 0.25f;
			while (fTime > 0.0f)
			{
				fTime -= HT.TimeUtils.RealTime;
				float fAlpha = (0.25f - fTime) * 4.0f;

				_gaugeEffect_WhenFull.alpha = fAlpha;
				_gaugeEffect_WhenFull_Button.alpha = fAlpha;

				yield return new WaitForEndOfFrame();
			}

			_gaugeEffect_WhenFull.alpha = 1.0f;

			//-----
			fTime = 0.25f;
			while (fTime > 0.0f)
			{
				fTime -= HT.TimeUtils.RealTime;
				float fAlpha = fTime * 4.0f;

				_gaugeEffect_WhenFull.alpha = fAlpha;
				_gaugeEffect_WhenFull_Button.alpha = fAlpha;

				yield return new WaitForEndOfFrame();
			}

			_gaugeEffect_WhenFull.alpha = 0.0f;

			//-----
			yield return new WaitForSecondsRealtime(0.5f);
		}
	}

	//---------------------------------------
	private IEnumerator GaugeEffect_WhenFull_Notice_Internal()
	{
		if (_gaugeEffect_WhenFull_Notice_Sound != null)
			HT.HTSoundManager.PlaySound(_gaugeEffect_WhenFull_Notice_Sound);

		//-----
		float fTime = 0.25f;
		while(fTime > 0.0f)
		{
			fTime -= HT.TimeUtils.GameTime;
			_gaugeEffect_WhenFull_Notice.alpha = (0.25f - fTime) * 4.0f;

			yield return new WaitForEndOfFrame();
		}

		//-----
		fTime = 1.0f;
		while (fTime > 0.0f)
		{
			fTime -= HT.TimeUtils.GameTime;
			_gaugeEffect_WhenFull_Notice.alpha = fTime;

			yield return new WaitForEndOfFrame();
		}
	}
}


/////////////////////////////////////////
//---------------------------------------