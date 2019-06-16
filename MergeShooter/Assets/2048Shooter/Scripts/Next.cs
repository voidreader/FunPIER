using System;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

// 다음 차례에 나올 블럭 
public class Next : MonoBehaviour
{
	[SerializeField]
	private int m_Index;

	[SerializeField]
	private int m_Value;

	[SerializeField]
	private Image m_Image;

	public int Index
	{
		get
		{
			return this.m_Index;
		}
	}

	public int Value
	{
		get
		{
			return this.m_Value;
		}
	}

	private void Awake()
	{
		base.transform.localPosition = Vector3.up * -470f;
		this.m_Image.raycastTarget = false;
	}

	public void Init(int index, int value)
	{
		this.m_Index = index;
		this.m_Value = value;
		this.m_Image.sprite = MonoSingleton<ConfigeManager>.Instance.GetValueIcon(this.m_Value);
		base.transform.localPosition = (float)(this.m_Index - 2) * Vector3.right * 118f + Vector3.up * -470f;
		base.gameObject.SetActive(true);
	}

	public void UseSkin(SkinType type)
	{
		if (type == SkinType.NEON)
		{
		}
	}
}
