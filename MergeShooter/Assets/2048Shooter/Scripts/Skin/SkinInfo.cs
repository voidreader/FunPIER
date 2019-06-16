using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkinInfo
{
	[Serializable]
	private class ValueImage
	{
		public int Value;

		public Sprite m_Image;
	}

	[SerializeField]
	private SkinType m_Type = SkinType.DAY;

	[SerializeField]
	private SkinType m_DefType = SkinType.DAY;

	[SerializeField]
	private int m_Price;

	[SerializeField]
	private Sprite m_StoreTexture;

	[SerializeField]
	private Sprite m_StoreUseIcon;

	[SerializeField]
	private string m_hexcolor;

	private Dictionary<int, Sprite> m_ValueImageDic = new Dictionary<int, Sprite>();

	private Dictionary<string, Sprite> m_SpriteList = new Dictionary<string, Sprite>();

	public SkinType Type
	{
		get
		{
			return this.m_Type;
		}
	}

	public SkinType DefType
	{
		get
		{
			return this.m_DefType;
		}
	}

	public int Price
	{
		get
		{
			return this.m_Price;
		}
	}

	public Sprite StorTexture
	{
		get
		{
			return this.m_StoreTexture;
		}
	}

	public Sprite StoreUseIcon
	{
		get
		{
			return this.m_StoreUseIcon;
		}
	}

	public string Hexcolor
	{
		get
		{
			return this.m_hexcolor;
		}
	}

	public void AddIcon(int value, Sprite sprite)
	{
		this.m_ValueImageDic.Add(value, sprite);
	}

	public Sprite GetValueIcon(int value)
	{
		if (this.m_ValueImageDic.ContainsKey(value))
		{
			return this.m_ValueImageDic[value];
		}
		return null;
	}

	public Sprite GetSpriteByName(string name)
	{
		Sprite sprite;
		if (!this.m_SpriteList.ContainsKey(name))
		{
			string path = string.Format("UI/{0}/{1}", this.m_Type.ToString(), name.ToString());
			sprite = (Resources.Load(path, typeof(Sprite)) as Sprite);
			if (sprite == null)
			{
				path = string.Format("UI/{0}/{1}", this.m_DefType.ToString(), name.ToString());
				sprite = (Resources.Load(path, typeof(Sprite)) as Sprite);
			}
			this.m_SpriteList.Add(name, sprite);
		}
		else
		{
			sprite = this.m_SpriteList[name];
		}
		return sprite;
	}
}
