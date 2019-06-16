using DG.Tweening;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanel : GameShow
{
	private sealed class _Show_c__AnonStorey0
	{
		internal Action openCallBack;

		internal Action closeCallBack;

		internal LoadPanel _this;

		internal void __m__0()
		{
			this._this.m_Show = false;
			if (this.openCallBack != null)
			{
				this.openCallBack();
			}
			this._this.Close(this.closeCallBack, 0f);
		}
	}

	[SerializeField]
	private Text m_Text;

	[SerializeField]
	private float m_ShowTime;

	private bool m_Show;

	private int count;

	[SerializeField]
	private float m_ShowIntervale;

	private float m_ShowTimer;

	public void Show(Action openCallBack = null, Action closeCallBack = null)
	{
		this.m_Show = true;
		this.m_ShowTimer = 0f;
		base.Show();
		MyDebugger.Log("-------------SHOW");
		base.transform.DOLocalMove(base.transform.localPosition, this.m_ShowTime, false).OnComplete(delegate
		{
				MyDebugger.Log("-------------SHOW complete");
			this.m_Show = false;
			if (openCallBack != null)
			{
				openCallBack();
			}
			this.Close(closeCallBack, 0f);
		});
	}

	private void Update()
	{
		if (this.m_Show)
		{
			string text = "LOADING";
			if (this.count == 0)
			{
				text += ".";
			}
			else if (this.count == 1)
			{
				text += "..";
			}
			else if (this.count == 2)
			{
				text += "...";
			}
			this.m_ShowTimer += Time.deltaTime;
			if (this.m_ShowTimer >= this.m_ShowIntervale)
			{
				this.count++;
				this.count %= 3;
				this.m_ShowTimer = 0f;
			}
			this.m_Text.text = text;
		}
	}
}
