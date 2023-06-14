using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ListItemTwo : MonoBehaviour
{
	#region Inspector Variables
	
	[SerializeField] private Text itemIndexText;
	[SerializeField] private Text wordsText;

	#endregion

	#region Public Methods
	
	public void Initialize(int index, int wordCound)
	{
		itemIndexText.text = string.Format("List Item: {0}", index);
		
		string words = "text";
		
		for (int i = 0; i < wordCound; i++)
		{
			words += " text";
		}
		
		wordsText.text = words;
	}

	#endregion
}
