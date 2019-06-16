using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

// UI Panel 관리
public class GameShow : MonoBehaviour
{
	[Serializable]
	public class ShowMoveObject
	{
		public GameObject m_Object;

		public Vector3 m_StartValue = Vector3.zero;

		public Vector3 m_EndValue = Vector3.zero;

		public float m_Time;

		public float m_Delay;
	}

	[Serializable]
	public class ShowAphlaObject
	{
		public GameObject m_Object;

		public Color m_StartColor = Color.white;

		public Color m_EndColor = Color.white;

		public float m_Time;

		public float m_Delay;
	}

	[Serializable]
	public class ShowScaleObject
	{
		public GameObject m_Object;

		public Vector3 m_StartScale = Vector3.zero;

		public Vector3 m_EndScale = Vector3.zero;

		public float m_Time;

		public float m_Delay;
	}

	protected float m_Dealy;

	private Vector3 m_Position = Vector3.zero;

	[SerializeField]
	protected List<GameShow.ShowMoveObject> m_ShowList = new List<GameShow.ShowMoveObject>();

	[SerializeField]
	private List<GameShow.ShowAphlaObject> m_ShowAphlaList = new List<GameShow.ShowAphlaObject>();

	[SerializeField]
	private List<GameShow.ShowScaleObject> m_ShowScaleList = new List<GameShow.ShowScaleObject>();

	private Action m_FadeInCallBack;

	private Action m_FadeOutCallBack;

	private bool _Tweener_k__BackingField;

	private bool _UIState_k__BackingField;

	protected Ease m_inEase = Ease.OutQuad;

	protected Ease m_outEase = Ease.InQuad;

	public bool Tweener
	{
		get;
		private set;
	}

	public bool UIState
	{
		get;
		private set;
	}

	private void Awake()
	{
		this.m_Position = base.transform.localPosition;
	}

	public virtual void Init()
	{
		this.Out();
		this.Hide();
	}

	protected virtual void In()
	{
		foreach (GameShow.ShowMoveObject current in this.m_ShowList)
		{
			if (current.m_Object != null)
			{
				current.m_Object.transform.localPosition = current.m_StartValue;
			}
		}
		foreach (GameShow.ShowAphlaObject current2 in this.m_ShowAphlaList)
		{
			if (current2.m_Object != null)
			{
				Image component = current2.m_Object.GetComponent<Image>();
				if (component != null)
				{
					component.color = current2.m_StartColor;
				}
				Text component2 = current2.m_Object.GetComponent<Text>();
				if (component2 != null)
				{
					Color color = component2.color;
					color.a = current2.m_StartColor.a;
					component2.color = color;
				}
			}
		}
		foreach (GameShow.ShowScaleObject current3 in this.m_ShowScaleList)
		{
			if (current3.m_Object != null)
			{
				current3.m_Object.transform.localScale = current3.m_StartScale;
			}
		}
	}

	protected virtual void Out()
	{
		foreach (GameShow.ShowMoveObject current in this.m_ShowList)
		{
			if (current.m_Object != null)
			{
				current.m_Object.transform.localPosition = current.m_EndValue;
			}
		}
		foreach (GameShow.ShowAphlaObject current2 in this.m_ShowAphlaList)
		{
			if (current2.m_Object != null)
			{
				Image component = current2.m_Object.GetComponent<Image>();
				if (component != null)
				{
					component.color = current2.m_EndColor;
				}
				Text component2 = current2.m_Object.GetComponent<Text>();
				if (component2 != null)
				{
					Color color = component2.color;
					color.a = current2.m_EndColor.a;
					component2.color = color;
				}
			}
		}
		foreach (GameShow.ShowScaleObject current3 in this.m_ShowScaleList)
		{
			if (current3.m_Object != null)
			{
				current3.m_Object.transform.localScale = current3.m_EndScale;
			}
		}
	}

	protected virtual void FadeIn(Action callBack = null)
	{
		if (this.Tweener && this.UIState)
		{
			return;
		}
		this.Tweener = true;
		this.UIState = true;
		this.Out();
		this.Show();
		this.m_FadeInCallBack = callBack;
		this.FadeInAction();
	}

	private void FadeInAction()
	{
		float num = 0f;
		foreach (GameShow.ShowMoveObject current in this.m_ShowList)
		{
			if (current.m_Object != null)
			{
				num = Mathf.Max(num, current.m_Time + current.m_Delay + this.m_Dealy);
				current.m_Object.transform.DOLocalMove(current.m_StartValue, current.m_Time, false).SetEase(this.m_inEase).SetDelay(current.m_Delay + this.m_Dealy);
			}
		}
		foreach (GameShow.ShowAphlaObject current2 in this.m_ShowAphlaList)
		{
			if (current2.m_Object != null)
			{
				num = Mathf.Max(num, current2.m_Time + current2.m_Delay + this.m_Dealy);
				Image component = current2.m_Object.GetComponent<Image>();
				if (component != null)
				{
					component.DOColor(current2.m_StartColor, current2.m_Time).SetEase(this.m_inEase).SetDelay(current2.m_Delay + this.m_Dealy);
				}
				Text component2 = current2.m_Object.GetComponent<Text>();
				if (component2 != null)
				{
					component2.DOFade(current2.m_StartColor.a, current2.m_Time).SetEase(this.m_inEase).SetDelay(current2.m_Delay + this.m_Dealy);
				}
			}
		}
		foreach (GameShow.ShowScaleObject current3 in this.m_ShowScaleList)
		{
			if (current3.m_Object != null)
			{
				num = Mathf.Max(num, current3.m_Time + current3.m_Delay);
				current3.m_Object.transform.DOScale(current3.m_StartScale, current3.m_Time).SetEase(Ease.InOutBack).SetDelay(current3.m_Delay + this.m_Dealy);
			}
		}
		base.transform.DOLocalMove(this.m_Position, num, false).OnComplete(delegate
		{
			if (this.m_FadeInCallBack != null)
			{
				this.m_FadeInCallBack();
				this.m_FadeInCallBack = null;
			}
			this.Tweener = false;
		});
	}

	protected virtual void FadeOut(Action callBack = null)
	{
		
		if (this.Tweener && !this.UIState)
		{
			return;
		}
		this.Tweener = true;
		this.m_FadeOutCallBack = callBack;
		this.FadeOutAction();
		this.UIState = false;
	}

	private void FadeOutAction()
	{
		float num = 0f;
		foreach (GameShow.ShowMoveObject current in this.m_ShowList)
		{
			if (current.m_Object != null)
			{
				num = Mathf.Max(num, current.m_Time + current.m_Delay + this.m_Dealy);
				current.m_Object.transform.DOLocalMove(current.m_EndValue, current.m_Time, false).SetEase(this.m_outEase).SetDelay(current.m_Delay + this.m_Dealy);
			}
		}
		foreach (GameShow.ShowAphlaObject current2 in this.m_ShowAphlaList)
		{
			if (current2.m_Object != null)
			{
				num = Mathf.Max(num, current2.m_Time + current2.m_Delay + 0.2f + this.m_Dealy);
				Image component = current2.m_Object.GetComponent<Image>();
				if (component != null)
				{
					component.DOColor(current2.m_EndColor, current2.m_Time).SetEase(this.m_outEase).SetDelay(current2.m_Delay + 0.2f + this.m_Dealy);
				}
				Text component2 = current2.m_Object.GetComponent<Text>();
				if (component2 != null)
				{
					component2.DOFade(current2.m_EndColor.a, current2.m_Time).SetEase(this.m_outEase).SetDelay(current2.m_Delay + 0.2f + this.m_Dealy);
				}
			}
		}
		foreach (GameShow.ShowScaleObject current3 in this.m_ShowScaleList)
		{
			if (current3.m_Object != null)
			{
				num = Mathf.Max(num, current3.m_Time + current3.m_Delay + this.m_Dealy);
				current3.m_Object.transform.DOScale(current3.m_EndScale, current3.m_Time).SetEase(Ease.InBack).SetDelay(current3.m_Delay + this.m_Dealy);
			}
		}
		base.transform.DOLocalMove(this.m_Position, num, false).OnComplete(delegate
		{
			if (this.m_FadeOutCallBack != null)
			{
				this.m_FadeOutCallBack();
				this.m_FadeOutCallBack = null;
			}
			this.Tweener = false;
			this.Hide();
		});
	}

	private void Load()
	{
		foreach (GameShow.ShowMoveObject current in this.m_ShowList)
		{
			if (current.m_Object != null)
			{
				current.m_EndValue = current.m_Object.transform.localPosition;
				current.m_StartValue = current.m_Object.transform.localPosition;
			}
		}
	}

	protected virtual void Show()
	{
		base.gameObject.SetActive(true);
	}

	protected virtual void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public virtual void Open(Action callBack = null, float dealy = 0f)
	{
		this.Refresh();
		this.m_Dealy = dealy;
		this.FadeIn(callBack);
	}

	public virtual void Open(string text, Action callBack = null, float dealy = 0f)
	{
		this.Refresh();
		this.m_Dealy = dealy;
		this.FadeIn(callBack);
	}

	public virtual void Open(string text, Action confirmCallback, Action callBack = null, float dealy = 0f)
	{
		this.Refresh();
		this.m_Dealy = dealy;
		this.FadeIn(callBack);
	}

	public virtual void Close(Action callBack = null, float dealy = 0f)
	{
		this.m_Dealy = dealy;
		this.FadeOut(callBack);
	}

	public virtual void Refresh()
	{
	}

	[ContextMenu("ClearConfig")]
	private void ClearConfig()
	{
		for (int i = this.m_ShowList.Count - 1; i >= 0; i--)
		{
			if (this.m_ShowList[i].m_Object == null)
			{
				this.m_ShowList.RemoveAt(i);
			}
		}
		for (int j = this.m_ShowAphlaList.Count - 1; j >= 0; j--)
		{
			if (this.m_ShowAphlaList[j].m_Object == null)
			{
				this.m_ShowAphlaList.RemoveAt(j);
			}
		}
		for (int k = this.m_ShowScaleList.Count - 1; k >= 0; k--)
		{
			if (this.m_ShowScaleList[k].m_Object == null)
			{
				this.m_ShowScaleList.RemoveAt(k);
			}
		}
	}

	public virtual void UnlockSkin(SkinType type)
	{
		if (this.UIState)
		{
			this.Refresh();
		}
	}
}
