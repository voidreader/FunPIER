using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinComponent : MonoBehaviour
{
	private SkinType m_Type = SkinType.DAY;

	[SerializeField]
	private List<SkinObject> m_List = new List<SkinObject>();

	public virtual void Init()
	{
		foreach (SkinObject current in this.m_List)
		{
			foreach (SkinItem current2 in current.m_SkinInfos)
			{
				if (!current.m_SkinDic.ContainsKey(current2.m_Type))
				{
					current.m_SkinDic.Add(current2.m_Type, null);
				}
				current.m_SkinDic[current2.m_Type] = current2.m_Icon;
			}
		}
	}

	public virtual void UseSkin(SkinType type)
	{
		if (this.m_Type == type)
		{
			return;
		}
		this.m_Type = type;
		foreach (SkinObject current in this.m_List)
		{
			if (current.m_Texture != null)
			{
				if (current.m_SkinDic.ContainsKey(this.m_Type))
				{
					current.m_Texture.sprite = current.m_SkinDic[this.m_Type];
				}
				else
				{
					current.m_Texture.sprite = current.m_SkinDic[SkinType.DAY];
				}
			}
		}
	}
}
