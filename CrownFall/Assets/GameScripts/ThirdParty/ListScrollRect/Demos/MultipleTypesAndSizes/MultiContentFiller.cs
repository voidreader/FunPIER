using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiContentFiller : MonoBehaviour, IContentFiller
{
	private enum ItemTypes
	{
		ItemOne,
		ItemTwo
	}

	#region Inspector Variables
	
	[SerializeField] private GameObject	listItemOne	= null;
	[SerializeField] private GameObject	listItemTwo	= null;
	[SerializeField] private int		itemCount	= 100;

	#endregion

	#region Member Variables
	
	private List<ItemTypes>	listItemTypes;
	private List<int>		itemTwoRandomWordCount;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		listItemTypes			= new List<ItemTypes>();
		itemTwoRandomWordCount	= new List<int>();
	}

	#endregion

	#region IContentFiller Methods

	public GameObject GetListItem(int index, int itemType, GameObject obj)
	{
		if (obj == null)
		{
			switch ((ItemTypes)itemType)
			{
			case ItemTypes.ItemOne:
				obj = Instantiate(listItemOne);
				break;
			case ItemTypes.ItemTwo:
				obj = Instantiate(listItemTwo);
				break;
			}
		}
		
		switch ((ItemTypes)itemType)
		{
		case ItemTypes.ItemOne:
			ListItemOne listItemOne = obj.GetComponent<ListItemOne>();
			
			listItemOne.Initialize(index);
			
			break;
		case ItemTypes.ItemTwo:
			ListItemTwo listItemTwo = obj.GetComponent<ListItemTwo>();
			
			while(itemTwoRandomWordCount.Count <= index)
			{
				itemTwoRandomWordCount.Add(Random.Range(5, 40));
			}
			
			listItemTwo.Initialize(index, itemTwoRandomWordCount[index]);
			
			break;
		}
		
		return obj;
	}
	
	public int GetItemCount()
	{
		return itemCount;
	}
	
	public int GetItemType(int index)
	{
		while (listItemTypes.Count <= index)
		{
			listItemTypes.Add((ItemTypes)Random.Range(0, 2));
		}
		
		return (int)listItemTypes[index];
	}

	#endregion
}
