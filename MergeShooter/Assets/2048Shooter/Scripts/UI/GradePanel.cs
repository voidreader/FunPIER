using System;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class GradePanel : GameShow
{
	private int m_StarCount = 5;

	[SerializeField]
	private List<Image> m_Stars = new List<Image>();

	[SerializeField]
	private Sprite m_Enable;

	[SerializeField]
	private Sprite m_Disable;

	public override void Init()
	{
		base.Init();
		this.m_StarCount = 5;
	}

	public void ChooseStar(int count)
	{
		this.m_StarCount = count;
		for (int i = 0; i < this.m_Stars.Count; i++)
		{
			this.m_Stars[i].sprite = ((i >= this.m_StarCount) ? this.m_Disable : this.m_Enable);
		}
		MonoSingleton<GameManager>.Instance.GradeGame(this.m_StarCount);
	}

	public void GradeIt()
	{
		MonoSingleton<GameManager>.Instance.GradeGame(this.m_StarCount);
	}

	public void GradeLater()
	{
		this.Close(null, 0f);
	}
}
