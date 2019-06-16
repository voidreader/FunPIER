using System.Collections;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;

public class ItemManager : MonoSingleton<ItemManager>
{
	[HideInInspector]
	public ItemType type;
	[HideInInspector]
	public bool gamingStopTime;

    [SerializeField]
	private BrickItem m_brickItem;

	public BrickItem SetBrickItem(ItemType type, int index, int count)
	{
        BrickItem item = UnityEngine.Object.Instantiate<BrickItem>(this.m_brickItem);
		item.transform.SetParent(base.transform);
		item.transform.localScale = Vector3.one;
		item.transform.localPosition = new Vector3(-310f + index * 120f, 76, 0);
		item.Init(type, index, count);

		return item;
	}
}
