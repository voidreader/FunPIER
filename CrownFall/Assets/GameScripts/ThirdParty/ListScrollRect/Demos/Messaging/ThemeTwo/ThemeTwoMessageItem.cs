using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThemeTwoMessageItem : MessageItem
{
	#region Inspector Variables

	[SerializeField] private HorizontalLayoutGroup 	msgBodyContainerLayout;
	[SerializeField] private VerticalLayoutGroup 	msgBodyLayout;
	[SerializeField] private HorizontalLayoutGroup 	timeContainerLayout;
	[SerializeField] private Sprite 				yoursBackground;
	[SerializeField] private Sprite 				theirsBackground;

	#endregion

	#region Public Methods
	
	public override void Setup(MessagingContentFiller.MessageInfo messageInfo, bool hideTime)
	{
		base.Setup(messageInfo, hideTime);
		
		switch (messageInfo.messageType)
		{
		case MessagingContentFiller.MessageType.Yours:
			backgroundImage.sprite 			= yoursBackground;

			msgBodyLayout.padding.left		= 10;
			msgBodyLayout.padding.right	= 18;

			msgBodyContainerLayout.childAlignment	= TextAnchor.UpperRight;
			msgBodyContainerLayout.padding.left	= 110;
			msgBodyContainerLayout.padding.right	= 0;

			timeContainerLayout.padding.left = 0;
			timeContainerLayout.padding.right = 20;

			timeText.alignment = TextAnchor.MiddleRight;
			break;
		case MessagingContentFiller.MessageType.Theirs:
			backgroundImage.sprite 			= theirsBackground;
			
			msgBodyLayout.padding.left		= 18;
			msgBodyLayout.padding.right	= 10;

			msgBodyContainerLayout.childAlignment	= TextAnchor.UpperLeft;
			msgBodyContainerLayout.padding.left	= 0;
			msgBodyContainerLayout.padding.right	= 110;
			
			timeContainerLayout.padding.left = 20;
			timeContainerLayout.padding.right = 0;
			
			timeText.alignment = TextAnchor.MiddleLeft;
			break;
		}
	}

	#endregion
}
