using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class BossGL_PartsGuageView : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private Slider _mainSlider = null;
    [SerializeField]
    private Image _mainSlider_Img = null;
    [SerializeField]
	private Slider _backSlider = null;
    [SerializeField]
    private Image _backSlider_Img = null;

    //---------------------------------------
    [SerializeField]
	private float _sliderLerpWaitTime = 1.0f;
	[SerializeField]
	private AIActor_Extend_GL.eParts _managedParts = AIActor_Extend_GL.eParts.Max;

	//---------------------------------------
	private Animation _animation = null;
	private AIActor_Extend_GL _golemActorExtend = null;
	private float _prevValue = 0.0f;
	private float _waitTime = 0.0f;

	//---------------------------------------
	private void Start()
	{
		_animation = GetComponent<Animation>();
		ResetSlider();

		_golemActorExtend = BattleFramework._Instance.m_pEnemyActor.GetComponent<AIActor_Extend_GL>();
	}

	void ResetSlider()
	{
        SetMainSlider(1.0f);
        SetBackSlider(1.0f);
	}

	//---------------------------------------
	private void FixedUpdate()
	{
		float fMax = 0.0f;
		float fNow = 0.0f;

		if (_managedParts != AIActor_Extend_GL.eParts.Max)
		{
			DamagablePart curParts = _golemActorExtend.FindParts(_managedParts);
			fMax = (float)curParts._maxHealth;
			fNow = (float)curParts._leastHealth.val;
		}
		else
		{
			fNow = _golemActorExtend.m_pActorBase.GetCurrHP();
			fMax = _golemActorExtend.PartsHPincreaseOffset_Body;

			if (fNow > fMax)
				fNow = fMax;
		}

		//-----
		float fRatio = fNow / fMax;
        SetMainSlider(fRatio);

		if (_prevValue > fRatio && _animation != null)
			_animation.Play();

		//-----
		if (Mathf.Abs(_prevValue - fRatio) > float.Epsilon)
		{
			_prevValue = fRatio;
			_waitTime = 0.0f;
		}

		//-----
		if ((GetBackSlider() - GetMainSlider()) > float.Epsilon)
		{
			_waitTime += HT.TimeUtils.GameTime;

			if (_waitTime >= _sliderLerpWaitTime)
			{
				float fBackValue = GetBackSlider();
				fBackValue = Mathf.Lerp(fBackValue, fRatio, HT.TimeUtils.GameTime);

                SetBackSlider(fBackValue);
			}
		}
		else
		{
            SetBackSlider(GetMainSlider());
		}
	}

    //---------------------------------------
    public void SetMainSlider(float fValue)
    {
        if (_mainSlider != null)
            _mainSlider.value = fValue;

        if (_mainSlider_Img != null)
            _mainSlider_Img.fillAmount = fValue;
    }

    public void SetBackSlider(float fValue)
    {
        if (_backSlider != null)
            _backSlider.value = fValue;

        if (_backSlider_Img != null)
            _backSlider_Img.fillAmount = fValue;
    }

    public float GetMainSlider()
    {
        if (_mainSlider != null)
            return _mainSlider.value;

        if (_mainSlider_Img != null)
            return _mainSlider_Img.fillAmount;

        return 0.0f;
    }

    public float GetBackSlider()
    {
        if (_backSlider != null)
            return _backSlider.value;

        if (_backSlider_Img != null)
            return _backSlider_Img.fillAmount;

        return 0.0f;
    }
}


/////////////////////////////////////////
//---------------------------------------