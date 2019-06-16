using DG.Tweening;
using System;
using UnityEngine;

public class ScaleTweener : MonoBehaviour
{
	public float MaxScale = 1.2f;

	public float time = 0.5f;

	private void OnEnable()
	{
		base.transform.localScale = Vector3.one;
		base.transform.DOScale(Vector3.one * this.MaxScale, this.time).SetLoops(-1, LoopType.Yoyo);
	}

	private void OnDisable()
	{
		base.transform.DOKill(false);
	}
}
