using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTweener : MonoBehaviour
{
	[SerializeField]
	private Text m_Score;

	[SerializeField]
	private float m_time = 0.6f;

	[SerializeField]
	private Vector3 m_Offset = Vector3.zero;

	public void Show(int number, Vector3 pos)
	{
		this.m_Score.text = string.Format("+{0}", number);
		base.transform.localPosition = pos + Vector3.up * 280f;
		base.transform.localScale = Vector3.one;
		this.Tweener();
	}

	[ContextMenu("Tweener")]
	private void Tweener()
	{
		this.m_Score.transform.DOLocalMove(base.transform.localPosition + this.m_Offset, this.m_time, false).OnComplete(delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		});
	}
}
