using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SimpleContentFiller : MonoBehaviour, IContentFiller
{
	#region Inspector Variables
	
	[SerializeField] private GameObject	listItemPrefab	= null;
	[SerializeField] private int		itemCount		= 100;

	#endregion

	#region IContentFiller Methods

	public GameObject GetListItem(int position, int itemType, GameObject obj)
	{
		if (obj == null)
		{
			obj = Instantiate(listItemPrefab);
		}

		// Get the text component on the GameObject and set the text
		Text uiText	= obj.GetComponent<Text>();
		uiText.text	= "List Item " + position;
		
		return obj;
	}

	public int GetItemCount()
	{
		// Return the number of items we want in the list
		return itemCount;
	}
	
	public int GetItemType(int position)
	{
		// Only one item type, just return any integer
		return 0;
	}

	#endregion
}
