using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContentFillerOne : MonoBehaviour, IContentFiller
{
	#region Inspector Variables

	[SerializeField] private GameObject verticalListItemPrefab;
	[SerializeField] private GameObject horizontalListItemPrefab;

	#endregion

	#region Member Variables

	private int		itemCount	= 100;
	private bool	isVertical	= true;

	#endregion

	#region Properties

	public int	ItemCount	{ get { return itemCount; }		set { itemCount = value; } }
	public bool	IsVertical	{ get { return isVertical; }	set { isVertical = value; } }

	#endregion

	#region Public Methods

	#region IContentFiller Methods

	public GameObject GetListItem(int index, int itemType, GameObject obj)
	{
		if (obj == null)
		{
			if (isVertical)
			{
				obj = Instantiate(verticalListItemPrefab);
			}
			else
			{
				obj = Instantiate(horizontalListItemPrefab);
			}
		}

		Text uiText = obj.transform.GetChild(0).GetComponent<Text>();

		uiText.text = "Item Index " + index;

		return obj;
	}

	public int GetItemCount()
	{
		return itemCount;
	}

	public int GetItemType(int index)
	{
		return 0;
	}

	#endregion

	#endregion
}
