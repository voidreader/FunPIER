using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniBuffGage : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public Slider m_pSlider;

	//---------------------------------------
	public Image m_pSliderImg_Edge;
	public Image m_pSliderImg_Slider;

	//---------------------------------------
	float m_fTotalTime;
	float m_fLifeTime;

	Color m_pEdgeColor;
	Color m_pGageColor;

	bool m_bIsClosing;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start()
	{
		m_fLifeTime = -1.0f;
	}

	// Update is called once per frame
	void Update()
	{
		if (m_pSlider.gameObject.activeInHierarchy)
		{
			m_fLifeTime -= HT.TimeUtils.GameTime;

			//-----
			if (m_fLifeTime <= 0.0f)
			{
				float fAlpha = (m_fLifeTime * 2.0f) + 0.5f;
				if (fAlpha > 0.0f)
				{
					m_pEdgeColor.a = fAlpha;
					m_pGageColor.a = fAlpha;
				}
				else
				{
					m_pSlider.gameObject.SetActive(false);
				}
			}
			else
			{
				float fTimeElapsed = m_fTotalTime - m_fLifeTime;
				if (fTimeElapsed < 0.5f)
				{
					m_pEdgeColor.a = fTimeElapsed * 2.0f;
					m_pGageColor.a = fTimeElapsed * 2.0f;
				}
				else
				{
					m_pEdgeColor.a = 1.0f;
					m_pGageColor.a = 1.0f;
				}
			}

			//-----
			m_pSliderImg_Edge.color = m_pEdgeColor;
			m_pSliderImg_Slider.color = m_pGageColor;

			//-----
			if (m_bIsClosing == false)
				m_pSlider.value = m_fLifeTime / m_fTotalTime;
		}
	}

	/////////////////////////////////////////
	//---------------------------------------
	public void SetEnabled(float fTime, Color pGageCol)
	{
		SetEnabled(fTime, fTime, pGageCol);
	}

	public void SetEnabled(float fTotalTime, float fLeastTime, Color pGuageCol)
	{
		m_fTotalTime = fTotalTime;
		m_fLifeTime = fLeastTime;

		m_pEdgeColor = Color.white;
		m_pGageColor = pGuageCol;

		//-----
		m_bIsClosing = false;

		//-----
		m_pSlider.gameObject.SetActive(true);
	}

	public void SetDisabled()
	{
		m_bIsClosing = true;
		m_fLifeTime = 0.0f;
	}


	/////////////////////////////////////////
	//---------------------------------------
}
