using System;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;

public class PopPanel : GameShow
{
    [SerializeField]
    private Transform m_Root;

    [SerializeField]
    private RectTransform m_Bg;

    [SerializeField]
    private List<RectTransform> m_List = new List<RectTransform>();

    public override void Open(Action callBack = null, float dealy = 0f)
    {
        // if (MonoSingleton<PushADManager>.Instance.ISCanShow() && MonoSingleton<PushADManager>.Instance.HaveWindow())
        // {
        // 	this.m_Bg.sizeDelta = new Vector2(this.m_Bg.sizeDelta.x, this.m_Bg.sizeDelta.y + 300f);
        // 	foreach (RectTransform current in this.m_List)
        // 	{
        // 		if (current != null)
        // 		{
        // 			current.anchoredPosition = new Vector2(current.anchoredPosition.x, current.anchoredPosition.y + 150f);
        // 		}
        // 	}
        // 	MonoSingleton<PushADManager>.Instance.OpenPushWindow(this.m_Root, null);
        // }
        base.Open(callBack, dealy);
    }

    public void Home()
    {
        MonoSingleton<GameManager>.Instance.PopGameHome();
    }

    public void Continue()
    {
        MonoSingleton<GameManager>.Instance.PopContinueGame();
    }

    public void NewGame()
    {
        MonoSingleton<GameManager>.Instance.PopNewGame();
    }
}
