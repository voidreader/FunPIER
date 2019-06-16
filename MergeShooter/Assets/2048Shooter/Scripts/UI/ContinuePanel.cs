using System;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class ContinuePanel : GameShow
{
	private const int BottumPosY = -300;

	[SerializeField]
	private Image m_TimerImage;

	[SerializeField]
	private GameObject m_ReturnTimeImage;

	[SerializeField]
	private float m_WaitTime = 5f;

	[SerializeField]
	private float m_ReturnTime = 2f;

	private float m_Timer;

	private bool m_ShowTimer;

	[SerializeField]
	private ImageAnimation quanAnim;

	public override void Refresh()
	{
		base.Refresh();
		//==this.m_TimerImage.transform.localPosition = Vector3.zero;
		this.m_Timer = this.m_WaitTime;
		this.m_ShowTimer = true;
		this.m_ReturnTimeImage.gameObject.SetActive(false);
		
		quanAnim.InitIndex();
	}

	public override void Close(Action callBack, float dealy = 0f)
	{
		this.m_ReturnTimeImage.gameObject.SetActive(false);
		this.m_ShowTimer = false;
		base.Close(callBack, dealy);
	}

	public void TimerState(bool state = true)
	{
		this.m_ShowTimer = state;
	}

	private void Update()
	{
		if (this.m_ShowTimer && MonoSingleton<GamePlayManager>.Instance.enabled)
		{
			if (this.m_Timer > 0f)
			{
				this.m_Timer -= Time.deltaTime;
				Vector3 localPosition = new Vector3(0f, (1f - this.m_Timer / this.m_WaitTime) * -300f, 0f);
				//==this.m_TimerImage.transform.localPosition = localPosition;
				if (this.m_Timer < this.m_ReturnTime)
				{
					this.m_ReturnTimeImage.gameObject.SetActive(true);
				}
			}
			else
			{
				this.m_Timer = 0f;
				this.m_ShowTimer = false;
				//==this.m_TimerImage.transform.localPosition = new Vector3(0f, -300f, 0f);
				MonoSingleton<GameManager>.Instance.ContinueToGameOver();
			}
		}
	}
}
