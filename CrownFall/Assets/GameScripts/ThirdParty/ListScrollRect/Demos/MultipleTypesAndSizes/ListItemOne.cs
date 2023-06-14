using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ListItemOne : MonoBehaviour
{
	#region Inspector Variables

	[SerializeField] private Text itemIndexText;

	#endregion

	#region Public Methods

	public void Initialize(int index)
	{
		itemIndexText.text = string.Format("List Item: {0}", index);
	}

	#endregion
}
