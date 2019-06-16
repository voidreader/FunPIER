using System;
using UnityEngine;

public class TextTweener : MonoBehaviour
{
	[SerializeField]
	private GameObject m_Object;

	private bool m_Play;

	[SerializeField]
	private float m_MinValue = 1f;

	[SerializeField]
	private float m_MaxValue = 1f;

	[SerializeField]
	private float m_ChangeValue = 1f;

	[SerializeField]
	private float m_Value = 1f;

	private float m_Time = -1f;

	private float m_Delay;

	private float m_Timer;

	private float m_DelayTimer;

	private bool m_PlayState = true;

	public void Init()
	{
		this.Stop();
		this.Hide();
	}

	[ContextMenu("Play")]
	public void Play()
	{
		this.m_Play = true;
	}

	[ContextMenu("Stop")]
	public void Stop()
	{
		this.m_Play = false;
	}

	public void Show(float time = -1f, float delay = 0f)
	{
		this.Play();
		this.m_Value = 1f;
		this.m_Timer = 0f;
		this.m_DelayTimer = 0f;
		this.m_Time = time;
		this.m_Delay = delay;
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	private void Animation()
	{
		if (this.m_Value > this.m_MaxValue)
		{
			this.m_ChangeValue = -Mathf.Abs(this.m_ChangeValue);
		}
		else if (this.m_Value < this.m_MinValue)
		{
			this.m_ChangeValue = Mathf.Abs(this.m_ChangeValue);
		}
		this.m_Object.transform.localScale = this.m_Value * Vector3.one;
		if (this.m_Value > this.m_MaxValue / 2f + this.m_MinValue / 2f)
		{
		}
		this.m_Value += this.m_ChangeValue;
	}

	private void Update()
	{
		if (this.m_Play)
		{
			if (this.m_Delay == 0f)
			{
				if (this.m_Time == -1f)
				{
					this.m_PlayState = true;
				}
				else if (this.m_Timer < this.m_Time)
				{
					this.m_PlayState = true;
				}
				else
				{
					this.m_PlayState = false;
				}
			}
			else if (this.m_DelayTimer >= this.m_Delay)
			{
				if (this.m_Time == -1f)
				{
					this.m_PlayState = true;
				}
				else if (this.m_Timer < this.m_Time)
				{
					this.m_PlayState = true;
				}
				else
				{
					this.m_PlayState = false;
				}
			}
			else
			{
				this.m_PlayState = false;
			}
			this.m_DelayTimer += Time.deltaTime;
			if (this.m_PlayState)
			{
				this.m_Timer += Time.deltaTime;
				this.Animation();
				if (this.m_Timer >= this.m_Time)
				{
					this.m_Object.transform.localScale = this.m_Value * Vector3.one;
				}
			}
		}
	}
}
