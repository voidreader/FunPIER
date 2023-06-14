using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageItem : MonoBehaviour 
{
	#region Inspector Variables
	
	[SerializeField] protected Text		messageText;
	[SerializeField] protected Text		timeText;
	[SerializeField] protected Image	backgroundImage;

	#endregion

	#region Member Variables
	
	private Color yoursColour	= new Color(194f/255f, 221f/255f, 210f/255f);
	private Color theirsColour	= Color.white;

	#endregion

	#region Public Methods

	public virtual void Setup(MessagingContentFiller.MessageInfo messageInfo, bool hideTime)
	{
		messageText.text	= messageInfo.message;
		timeText.text		= messageInfo.timeStr;
		
		switch (messageInfo.messageType)
		{
		case MessagingContentFiller.MessageType.Yours:
			backgroundImage.color = yoursColour;
			break;
		case MessagingContentFiller.MessageType.Theirs:
			backgroundImage.color = theirsColour;
			break;
		}
		
		timeText.gameObject.SetActive(!hideTime);
	}

	#endregion
}
