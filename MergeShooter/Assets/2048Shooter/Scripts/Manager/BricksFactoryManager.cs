using System;
using Toolkit;
using UnityEngine;

public class BricksFactoryManager : MonoSingleton<BricksFactoryManager>
{
	[SerializeField]
	private Brick m_brick;

	public Brick CreateBrick(int array, int index, int value, int maxvalue)
	{
		Brick brick = UnityEngine.Object.Instantiate<Brick>(this.m_brick);
		brick.transform.SetParent(base.transform);
		brick.transform.localScale = Vector3.one;
		brick.transform.localPosition = (float)(array - 2) * Vector3.right * 118f + Vector3.up * -744f;
		brick.Init(array, index, value, maxvalue);
		return brick;
	}
}
