using DG.Tweening;
using System;
using UnityEngine;

public class RedHit : MonoBehaviour
{
	private bool m_Play;

	private bool m_PlayState;

	private float m_Angle;

	[SerializeField]
	private float m_MaxAnagel = 30f;

	[SerializeField]
	private float m_MinAngle = -30f;

	[SerializeField]
	private float m_ChangeValue = 3f;

	private float m_Timer;

	[SerializeField]
	private float m_WaitingTime = 5f;

	private int m_Count;

	[SerializeField]
	private int m_Times = 2;

	public void Play()
	{
		base.transform.localEulerAngles = Vector3.zero;
		this.m_Timer = 0f;
		this.m_Count = 0;
		this.m_Play = true;
		this.m_PlayState = false;
		this.ScaleTweenner();
	}

	public void Stop()
	{
		base.transform.localEulerAngles = Vector3.zero;
		this.m_Timer = 0f;
		this.m_Count = 0;
		this.m_Play = false;
		this.m_PlayState = false;
		DOTween.Kill(base.gameObject, false);
	}

	private void ScaleTweenner()
	{
		base.transform.localScale = Vector3.one;
		base.transform.DOScale(Vector3.one * 1.2f, 0.4f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
	}

	private void OnDisable()
	{
		this.Stop();
	}

	private void Update()
	{
		if (this.m_Play)
		{
			if (this.m_PlayState)
			{
				if (this.m_Angle >= this.m_MaxAnagel)
				{
					this.m_ChangeValue = -Mathf.Abs(this.m_ChangeValue);
					this.m_Count++;
				}
				else if (this.m_Angle <= this.m_MinAngle)
				{
					this.m_ChangeValue = Mathf.Abs(this.m_ChangeValue);
					this.m_Count++;
				}
				this.m_Angle += this.m_ChangeValue;
				if (this.m_Times * 2 == this.m_Count && this.m_Angle < Mathf.Abs(this.m_ChangeValue) && this.m_Angle > -Mathf.Abs(this.m_ChangeValue))
				{
					this.m_Count = 0;
					this.m_PlayState = false;
					this.m_Timer = 0f;
				}
				base.transform.localEulerAngles = Vector3.forward * this.m_Angle;
			}
			else
			{
				this.m_Timer += Time.deltaTime;
				if (this.m_Timer >= this.m_WaitingTime)
				{
					this.m_PlayState = true;
					this.m_Timer = 0f;
				}
			}
		}
	}
}
