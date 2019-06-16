using System;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPanel : GameShow
{
	[SerializeField]
	private Text m_textTip;

	public void SetText(string txt)
	{
		this.m_textTip.text = txt;
	}

	public void Continue()
	{
		MonoSingleton<GameManager>.Instance.AchievementContinue();
	}
}
