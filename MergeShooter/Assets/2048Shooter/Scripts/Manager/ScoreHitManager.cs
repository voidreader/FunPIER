using System;
using Toolkit;
using UnityEngine;

public class ScoreHitManager : MonoSingleton<ScoreHitManager>
{
	[SerializeField]
	private ScoreTweener m_ScoreHit;

	public void ShowScore(int score, Vector3 pos)
	{
		ScoreTweener scoreTweener = UnityEngine.Object.Instantiate<ScoreTweener>(this.m_ScoreHit);
		scoreTweener.gameObject.SetActive(true);
		scoreTweener.transform.SetParent(base.transform);
		scoreTweener.Show(score, pos);
	}
}
