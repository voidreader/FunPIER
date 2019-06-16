using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SkinObject
{
	public Image m_Texture;

	public List<SkinItem> m_SkinInfos = new List<SkinItem>();

	public Dictionary<SkinType, Sprite> m_SkinDic = new Dictionary<SkinType, Sprite>();
}
