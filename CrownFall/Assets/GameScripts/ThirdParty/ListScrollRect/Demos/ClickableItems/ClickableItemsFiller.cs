using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ClickableItemsFiller : MonoBehaviour, IContentFiller
{
	#region Inspector Variables

	[SerializeField] private ListScrollRect	listScrollRect;
	[SerializeField] private ClickableItem	listItemPrefab	= null;
	[SerializeField] private int			itemCount		= 100;

	#endregion

	#region Member Variables

	private int selectedListIndex;

	#endregion

	#region IContentFiller Methods

	public GameObject GetListItem(int itemIndex, int itemType, GameObject obj)
	{
		if (obj == null)
		{
			obj = Instantiate(listItemPrefab.gameObject);
		}

		// Get if this item is selected
		bool isSelected = (itemIndex == selectedListIndex);

		// Get the text component on the GameObject and set the text
		obj.GetComponent<ClickableItem>().Init(itemIndex, isSelected, OnListItemClicked);
		
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

	#region Private Methods

	private void OnListItemClicked(int itemIndex)
	{
		// Set the selected item
		selectedListIndex = itemIndex;

		// Call RefreshContent so GetListItem is called on all visible list items and the selected item can become higlighted
		listScrollRect.RefreshContent();
	}

	#endregion
}
