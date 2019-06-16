using System;
using UnityEngine;

public class Course : MonoBehaviour
{
	[SerializeField]
	private RectTransform m_Finger;

	[SerializeField]
	private GameObject m_Margin;

	private bool m_Play;

	[SerializeField]
	private float m_MinValue = 1f;

	[SerializeField]
	private float m_MaxValue = 1f;

	[SerializeField]
	private float m_ChangeValue = 1f;

	[SerializeField]
	private float m_Value = 1f;

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
		this.m_Margin.gameObject.SetActive(false);
	}

	public void Show()
	{
		this.m_Margin.gameObject.SetActive(true);
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
		this.m_Finger.localScale = this.m_Value * Vector3.one;
		if (this.m_Value > this.m_MaxValue / 2f + this.m_MinValue / 2f)
		{
			this.m_Margin.gameObject.SetActive(true);
		}
		else
		{
			this.m_Margin.gameObject.SetActive(false);
		}
		this.m_Value += this.m_ChangeValue;
	}

	private void Update()
	{
		if (this.m_Play)
		{
			this.Animation();
		}
	}
}
