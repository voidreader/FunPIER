using DG.Tweening;
using System;
using Toolkit;
using UnityEngine;

public class MoneyManager : MonoSingleton<MoneyManager>
{
	[SerializeField]
	private float m_Time = 0.5f;

	[SerializeField]
	private GameObject m_Money;

	private int m_Array;

	private int m_Index;

	private bool m_State;

	private bool m_Tween;

	public int Array
	{
		get
		{
			return this.m_Array;
		}
	}

	public int Index
	{
		get
		{
			return this.m_Index;
		}
	}

	public bool State
	{
		get
		{
			return this.m_State;
		}
	}

	public bool Tween
	{
		get
		{
			return this.m_Tween;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.m_Money.gameObject.SetActive(false);
		this.m_State = false;
	}

	public void Show(int array, int index)
	{
		this.m_Array = array;
		this.m_Index = index;
		this.m_State = true;
		this.m_Money.gameObject.SetActive(true);
		Vector3 localPosition = new Vector3(-237f, -57f, 0f) + Vector3.right * 118f * (float)this.m_Array + Vector3.down * (float)this.m_Index * 118f;
		this.m_Money.transform.localPosition = localPosition;
	}

	public void MoveTarget(int array, int index)
	{
		this.m_Array = array;
		this.m_Index = index;
		this.m_Tween = true;
		Vector3 endValue = new Vector3(-237f, -57f, 0f) + Vector3.right * 118f * (float)this.m_Array + Vector3.down * (float)this.m_Index * 118f;
		this.m_Money.transform.DOLocalMove(endValue, this.m_Time, false).OnComplete(delegate
		{
			this.m_Tween = false;
		});
	}

	public void ShowTweener()
	{
		this.Hide();
	}

	public void Hide()
	{
		this.m_State = false;
		this.m_Money.gameObject.SetActive(false);
	}
}
