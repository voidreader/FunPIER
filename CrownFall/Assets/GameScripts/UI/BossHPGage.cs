using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class BossHPGage : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public Slider m_pMainSlider;
	public Image _mainSlider_Image = null;
	public Slider m_pBackSlider;
	public Image _backSlider_Image = null;

	public Text m_pActorName;

	//---------------------------------------
	public float m_fSliderLerpWaitTime = 1.0f;

	//---------------------------------------
	Animation m_pAnimation;

	float m_fPrevValue = 0.0f;
	float m_fWaitTime = 0.0f;

	//---------------------------------------
	bool m_bResetSuccess = false;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start()
	{
		m_pAnimation = GetComponent<Animation>();

		ResetSlider();
		HT.HTLocaleTable.onLanguageChanged += UpdateText;
	}

	private void OnDestroy()
	{
		HT.HTLocaleTable.onLanguageChanged -= UpdateText;
	}

	// Update is called once per frame
	void Update()
	{
		if (BattleFramework._Instance == null)
			return;

		IActorBase pBoss = BattleFramework._Instance.m_pEnemyActor;
		if (m_bResetSuccess && pBoss != null)
		{
			float fMax = (float)pBoss.m_pActorInfo.m_cnMaxHP.val;
			float fNow = (float)pBoss.m_pActorInfo.m_cnNowHP.val;

			//-----
			float fRatio = fNow / fMax;
			SetMainSlider(fRatio);

			//-----
			if (m_fPrevValue > fRatio)
			{
				m_pAnimation.Play();
			}

			//-----
			if (Mathf.Abs(m_fPrevValue - fRatio) > float.Epsilon)
			{
				m_fPrevValue = fRatio;
				m_fWaitTime = 0.0f;
			}

			//-----
			if ((GetBackSlider() - GetMainSlider()) > float.Epsilon)
			{
				m_fWaitTime += HT.TimeUtils.GameTime;

				if (m_fWaitTime >= m_fSliderLerpWaitTime)
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
		else
		{
			ResetSlider();
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public float GetMainSlider()
	{
		if (m_pMainSlider != null)
			return m_pMainSlider.value;

		if (_mainSlider_Image != null)
			return _mainSlider_Image.fillAmount;

		return 0.0f;
	}

	public float GetBackSlider()
	{
		if (m_pBackSlider != null)
			return m_pBackSlider.value;

		if (_backSlider_Image != null)
			return _backSlider_Image.fillAmount;

		return 0.0f;
	}

	//---------------------------------------
	public void SetMainSlider(float fVal)
	{
		if (m_pMainSlider != null)
			m_pMainSlider.value = fVal;

		if (_mainSlider_Image != null)
			_mainSlider_Image.fillAmount = fVal;
	}

	public void SetBackSlider(float fVal)
	{
		if (m_pBackSlider != null)
			m_pBackSlider.value = fVal;

		if (_backSlider_Image != null)
			_backSlider_Image.fillAmount = fVal;
	}


	/////////////////////////////////////////
	//---------------------------------------
	void ResetSlider()
	{
		m_bResetSuccess = false;

		SetMainSlider(1.0f);
		SetBackSlider(1.0f);

		m_fPrevValue = GetMainSlider();

		if (BattleFramework._Instance == null)
			return;

		if (BattleFramework._Instance.m_pEnemyActor == null)
			return;

		UpdateText();

		m_bResetSuccess = true;
	}


	/////////////////////////////////////////
	//---------------------------------------
	private void UpdateText()
	{
		IActorBase pBoss = BattleFramework._Instance.m_pEnemyActor;
		m_pActorName.text = HT.HTLocaleTable.GetLocalstring(pBoss.m_pActorInfo.m_szActorName);
	}
}
