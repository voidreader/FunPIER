using DG.Tweening;
using System;
using System.Runtime.CompilerServices;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class MoneyAnimManager : MonoSingleton<MoneyAnimManager>
{
	private sealed class _ShowMoney_c__AnonStorey0
	{
		internal Text obj;

		internal MoneyAnimManager _this;

		internal void __m__0()
		{
			this.obj.DOFade(0f, this._this.Duration).SetEase(this._this.outEase).OnComplete(delegate
			{
				this.obj.gameObject.SetActive(false);
			});
		}

		internal void __m__1()
		{
			this.obj.gameObject.SetActive(false);
		}
	}

	private static readonly float DelayShowTime = 1f;

	public float Duration = 1f;

	public Vector3 startPos;

	public Vector3 endPos;

	public Vector3 allStartPos;

	public Vector3 allEndPos;

	public Vector3 homeStartPos = new Vector3(-220f, 0f, 0f);

	public Vector3 homeEndPos = new Vector3(-180f, 0f, 0f);

	private Ease inEase = Ease.OutQuart;

	private Ease outEase = Ease.InQuart;

	[SerializeField]
	private Text m_GetMoney;

	[SerializeField]
	private Text m_Money;

	[SerializeField]
	private Text m_HomeMoney;

	private bool m_isTween;

	public bool Tween
	{
		get
		{
			return this.m_isTween;
		}
	}

	protected override void Awake()
	{
		this.m_Money.gameObject.SetActive(false);
		this.m_GetMoney.gameObject.SetActive(false);
	}

	public void ShowGetMoney(Vector3 startOffset, Vector3 endOffset, float duration = 1f)
	{
		this.m_GetMoney.gameObject.SetActive(true);
		this.m_GetMoney.transform.localPosition = startOffset;
		Color color = this.m_GetMoney.transform.parent.GetComponent<Text>().color;
		color.a = 0f;
		this.m_GetMoney.color = color;
		this.m_GetMoney.transform.DOLocalMove(endOffset, this.Duration, false).SetDelay(MoneyAnimManager.DelayShowTime);
		this.m_GetMoney.DOFade(1f, this.Duration).SetEase(this.inEase).SetDelay(MoneyAnimManager.DelayShowTime).OnComplete(delegate
		{
			this.m_GetMoney.DOFade(0f, this.Duration).SetEase(this.outEase).OnComplete(delegate
			{
				this.m_GetMoney.gameObject.SetActive(false);
			});
		});
	}

	public void ShowMoney(Text obj, Vector3 startOffset, Vector3 endOffset, float duration = 1f)
	{
		obj.gameObject.SetActive(true);
		obj.transform.localPosition = startOffset;
		Color color = obj.transform.parent.GetComponent<Text>().color;
		color.a = 0f;
		obj.color = color;
		obj.transform.DOLocalMove(endOffset, this.Duration, false).SetDelay(MoneyAnimManager.DelayShowTime);
		obj.DOFade(1f, this.Duration).SetEase(this.inEase).SetDelay(MoneyAnimManager.DelayShowTime).OnComplete(delegate
		{
			obj.DOFade(0f, this.Duration).SetEase(this.outEase).OnComplete(delegate
			{
				obj.gameObject.SetActive(false);
			});
		});
	}

	public void ShowDoubleMoney(int num)
	{
		this.m_GetMoney.text = "+" + num.ToString();
		this.m_Money.text = "+" + num.ToString();
		this.ShowGetMoney(this.startPos, this.endPos, 1f);
		this.ShowMoney(this.m_Money, this.allStartPos, this.allEndPos, 1f);
	}

	public void ShowAddMoney(int num)
	{
		if (MonoSingleton<GameUIManager>.Instance.IsHomeState())
		{
			this.m_HomeMoney.text = "+" + num.ToString();
			this.ShowMoney(this.m_HomeMoney, this.homeStartPos, this.homeEndPos, 1f);
		}
		else
		{
			this.m_Money.text = "+" + num.ToString();
			this.ShowMoney(this.m_Money, this.allStartPos, this.allEndPos, 1f);
		}
	}

	private void OnDisable()
	{
		DOTween.Kill(this.m_Money, true);
		this.m_Money.gameObject.SetActive(false);
		DOTween.Kill(this.m_GetMoney, true);
		this.m_GetMoney.gameObject.SetActive(false);
	}
}
