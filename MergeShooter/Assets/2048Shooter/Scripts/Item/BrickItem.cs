using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrickItem : MonoBehaviour
{
	public ItemType m_Type;

	public int m_Index;

	[SerializeField]
	private Image m_Image;

	[SerializeField]
	private Text amount;
	
	public int m_Count;

	public void Init(ItemType type, int index, int count)
	{
		this.m_Type = type;
		this.m_Index = index;
		this.m_Count = count;

		string resName = null;
		if(GameDataManager.Instance.PlayMode == 0)
		{
			resName = "icon_classic_" + (index + 1);
		}
		else
		{
			resName = "icon_Challenge_" + (index + 1);
		}

		string path = string.Format("Items/{0}", resName);
        Sprite sprite = (Resources.Load(path, typeof(Sprite)) as Sprite);
		this.m_Image.sprite = sprite;
		this.amount.text = count.ToString();
	}

	public void UseItem()
	{
		if (GamePlayManager.Instance.BricksDic.Values.Count < 2 || GamePlayManager.Instance.State != GamePlayManager.GameState.Handle)
        {
			return;
		}
		if(GamePlayManager.Instance.State == GamePlayManager.GameState.UseItem || ItemManager.Instance.gamingStopTime)
		{
			return;
		}
		if(GameDataManager.Instance.BrickItemsInfo[GameDataManager.Instance.PlayMode][this.m_Index] == 0)
		{
			GamePlayManager.Instance.GamePause();
			GameManager.Instance.GameItemStore();
			return;
		}

		//this.m_Count--;
		this.amount.text = m_Count.ToString();

		if(this.m_Type != ItemType.stopTime)
		{
			foreach (List<Brick> current in GamePlayManager.Instance.BricksDic.Values)
			{
				for (int i = current.Count - 1; i >= 0; i--)
				{
					current[i].m_Image.raycastTarget = true;
				}
			}
		}

		//GameDataManager.Instance.BrickItemsInfo[GameDataManager.Instance.PlayMode][this.m_Index] = this.m_Count;
        //GameDataManager.Instance.saveGamingItems();

		ItemManager.Instance.type = this.m_Type;
		GamePlayManager.Instance.PauseByUseItem();
	}

	void LateUpdate()
	{
		this.amount.text = GameDataManager.Instance.BrickItemsInfo[GameDataManager.Instance.PlayMode][this.m_Index].ToString();
	}
}
