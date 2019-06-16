using System;
using UnityEngine;

public class ScreenAdapt : MonoBehaviour
{
	[SerializeField]
	private RectTransform m_Parent;

	[SerializeField]
	private GameObject m_Mine;

	private void Start()
	{
		this.Adapt();
	}

	private void Adapt()
	{
		float height = this.m_Parent.rect.height;
		if (height > 1284f)
		{
			this.m_Mine.transform.localScale = 1280f / height * Vector3.one;
		}
	}

	private void Update()
	{
	}
}
