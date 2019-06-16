using DG.Tweening;
using System;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class SelectPopPanel : GameShow
{
	[SerializeField]
	private Text m_Content;

    private Action m_ConfirmCallback;

	public override void Init()
	{
		base.Init();
		this.m_inEase = Ease.OutBack;
		this.m_outEase = Ease.InBack;
        m_ConfirmCallback = null;
	}

	public override void Open(string notice, Action confirmCallback, Action callBack = null, float dealy = 0f)
	{
		m_ConfirmCallback = confirmCallback;
		base.Open(callBack, dealy);
        m_Content.text = notice;
	}

	public override void Close(Action callBack = null, float dealy = 0f)
	{
		base.Close(callBack, dealy);
	}

	public override void Refresh()
	{
		base.Refresh();
	}

    public void OnConfirmPanel()
    {
        m_ConfirmCallback?.Invoke();
    }

    public void CancelPanel()
    {
        base.Close(null, 0f);
    }
}
