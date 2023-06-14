using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThemeOneMessageItem : MessageItem
{
	#region Inspector Variables
	
	[SerializeField] private Text nameText;

	#endregion

	#region Public Methods
	
	public void Setup(MessagingContentFiller.MessageInfo messageInfo, bool hideTime, bool hideName)
	{
		base.Setup(messageInfo, hideTime);
		
		nameText.text = messageInfo.name;
		
		nameText.gameObject.SetActive(!hideName);
	}

	#endregion
}
