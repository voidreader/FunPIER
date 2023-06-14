using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ContentFillerTwo : MonoBehaviour, IContentFiller
{
	#region Inspector Variables

	[SerializeField] private GameObject verticalListItemPrefab;
	[SerializeField] private GameObject horizontalListItemPrefab;

	#endregion

	#region Member Variables

	private int 		itemCount		= 100;
	private int 		minTextStrCount	= 10;
	private int 		maxTextStrCount	= 100;
	private bool		randomEverytime	= false;
	private bool		isVertical		= true;
	private List<int>	randomNumber	= null;

	#endregion

	#region Properties

	public int	ItemCount		{ get { return itemCount; }			set { itemCount = value; } }
	public int	MinTextStrCount	{ get { return minTextStrCount; }	set { minTextStrCount = value; } }
	public int	MaxTextStrCount	{ get { return maxTextStrCount; }	set { maxTextStrCount = value; } }
	public bool	RandomEverytime	{ get { return randomEverytime; }	set { randomEverytime = value; } }
	public bool	IsVertical		{ get { return isVertical; }		set { isVertical = value; } }

	#endregion

	#region Unity Methods

	private void Awake()
	{
		randomNumber = new List<int>();
	}

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

		// Crude way to get the two UI Text components
		Text uiText1 = obj.transform.GetChild(0).GetComponent<Text>();
		Text uiText2 = obj.transform.GetChild(1).GetComponent<Text>();

		// Set the first ones text to the list item number
		uiText1.text = "Item Index " + index;

		// Now lets get a random number for how many "text"s to display
		int randomTextStrCount;

		// We want to generate a new random number if the flag "randomEverytime" is true OR there doesn't exist a
		// random number in the randomNumber for index OR the random number at index is 0
		if (randomEverytime || randomNumber.Count <= index || randomNumber[index] == 0)
		{
			// Get a new random number
			randomTextStrCount = Random.Range(minTextStrCount, maxTextStrCount + 1);

			// Add 0s to the randomNumber list until we have all we need
			while (randomNumber.Count <= index)
			{
				randomNumber.Add(0);
			}

			// Set the random number in the list
			randomNumber[index] = randomTextStrCount;
		}
		else
		{
			randomTextStrCount = randomNumber[index];
		}

		// Create the "text text text..." string
		string textStr = "Random \"text\" count: " + randomTextStrCount + "\n";
		for (int i = 0; i < randomTextStrCount; i++)
		{
			textStr += (i == 0) ? "text" : " text";
		}

		uiText2.text = textStr;

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
