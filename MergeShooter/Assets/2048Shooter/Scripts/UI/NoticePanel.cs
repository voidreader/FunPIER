using DG.Tweening;
using System;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class NoticePanel : GameShow
{
	[SerializeField]
	private Text m_Content;

	public override void Init()
	{
		base.Init();
		this.m_inEase = Ease.OutBack;
		this.m_outEase = Ease.InBack;
	}

	public override void Open(string notice, Action callBack = null, float dealy = 0f)
	{
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

    public void CloseNoticePanel()
    {
        base.Close(null, 0f);
    }
}
