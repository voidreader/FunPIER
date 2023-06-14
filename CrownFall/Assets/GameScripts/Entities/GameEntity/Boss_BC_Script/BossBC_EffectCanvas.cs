using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class BossBC_EffectCanvas : MonoBehaviour
{
	//---------------------------------------
	public static BossBC_EffectCanvas _instance = null;

	//---------------------------------------
	[SerializeField]
	private CanvasGroup _whiteMask = null;
	[SerializeField]
	private CanvasGroup _brightEffect = null;

	//---------------------------------------
	private void Awake()
	{
		_instance = this;
	}

	//---------------------------------------
	public void OnWhiteMask(float fStart, float fEnd)
	{
		StartCoroutine(OnWhiteMask_Internal(fStart, fEnd));
		StartCoroutine(OnBrightEffect_Internal());
	}

	private IEnumerator OnWhiteMask_Internal(float fStart, float fEnd)
	{
		float fTime = 0.0f;
		while(fTime < fStart)
		{
			fTime += HT.TimeUtils.GameTime;
			_whiteMask.alpha = fTime / fStart;

			yield return new WaitForEndOfFrame();
		}

		_whiteMask.alpha = 1.0f;

		fTime = 0.0f;
		while (fTime < fEnd)
		{
			fTime += HT.TimeUtils.GameTime;
			_whiteMask.alpha = 1.0f - (fTime / fStart);

			yield return new WaitForEndOfFrame();
		}
	}

	//---------------------------------------
	private IEnumerator OnBrightEffect_Internal()
	{
		while(true)
		{
			_brightEffect.alpha = 0.6f + HT.RandomUtils.Range(-0.1f, 0.1f);
			yield return new WaitForEndOfFrame();
		}
	}
}


/////////////////////////////////////////
//---------------------------------------