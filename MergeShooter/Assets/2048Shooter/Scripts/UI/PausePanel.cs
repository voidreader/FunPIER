using System;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : GameShow
{
	[SerializeField]
	private Text m_ScoreText;

	public override void Refresh()
	{
		base.Refresh();
		this.m_ScoreText.text = MonoSingleton<GamePlayManager>.Instance.Score.ToString();
	}

	public void Rank()
	{
		MonoSingleton<GameManager>.Instance.PauseRank();
	}

	public void Home()
	{
		MonoSingleton<GameManager>.Instance.PuaseHome();
	}

	public void Restart()
	{
		MonoSingleton<GameManager>.Instance.GameRestart();
	}

	public void Continue()
	{
		MonoSingleton<GameManager>.Instance.PauseContinue();
	}
}
