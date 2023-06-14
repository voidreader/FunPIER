using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClickableItem : MonoBehaviour
{
	#region Inspector Variables

	[SerializeField] private Text	uiText;
	[SerializeField] private Image	uiBackground;

	#endregion

	#region Member Variables

	private int					itemIndex;
	private System.Action<int>	callback;

	#endregion

	#region Public Methods

	public void Init(int itemIndex, bool isSelected, System.Action<int> callback)
	{
		this.itemIndex	= itemIndex;
		this.callback	= callback;

		uiText.text			= "List Index " + itemIndex;
		uiBackground.color	= isSelected ? new Color(30f/255f, 144f/255f, 1f) : Color.white;
	}

	// Called when the Unity button for this list item is clicked
	public void OnItemClicked()
	{
		if (callback != null)
		{
			callback(itemIndex);
		}
	}

	#endregion
}
