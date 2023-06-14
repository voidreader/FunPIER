using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MessagingContentFiller : MonoBehaviour, IContentFiller
{
	#region Inner Classes

	public enum MessageType
	{
		Theirs,
		Yours,
		Count
	}
	
	public class MessageInfo
	{
		public MessageType	messageType;
		public string		name;
		public string		message;
		public string		timeStr;
	}

	#endregion

	#region Inspector Variables

	[SerializeField] private ListScrollRect	listScrollRect;
	[SerializeField] private GameObject		themeOneItemPrefab;
	[SerializeField] private GameObject		themeTwoItemPrefab;
	[SerializeField] private InputField		messageInputField;
	[SerializeField] private Dropdown		themeDropdown;

	#endregion

	#region Member Variables

	// These are the messages that will randomly be displayed, its just a bunch of puns
	private static readonly string[] randomPuns = new string[]
	{
		"Did you hear about the guy whose whole left side was cut off? He's all right now.",
		"I wondered why the baseball was getting bigger. Then it hit me.",
		"I'm reading a book about anti-gravity. It's impossible to put down.",
		"I'd tell you a chemistry joke but I know I wouldn't get a reaction.",
		"I used to be a banker but I lost interest.",
		"It's not that the man did not know how to juggle, he just didn't have the balls to do it.",
		"Did you hear about the guy who got hit in the head with a can of soda? He was lucky it was a soft drink.",
		"So what if I don't know what apocalypse means!? It's not the end of the world!",
		"A friend of mine tried to annoy me with bird puns, but I soon realized that toucan play at that game.",
		"Have you ever tried to eat a clock? It's very time consuming.",
		"Claustrophobic people are more productive thinking outside the box.",
		"Yesterday I accidentally swallowed some food coloring. The doctor says I'm OK, but I feel like I've dyed a little inside.",
		"Need an ark to save two of every animal? I noah guy.",
		"I couldn't quite remember how to throw a boomerang, but eventually it came back to me.",
		"My friend's bakery burned down last night. Now his business is toast.",
	};
	
	private List<MessageInfo>	messageInfos;
	private int					waitTillRespond;
	private bool				useThemeTwo;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		messageInfos	= new List<MessageInfo>();
		waitTillRespond	= -1;

		CreateDefaultStartingMessages();
	}

	private void Update()
	{
		if (waitTillRespond == 0)
		{
			MessageInfo messageInfo = new MessageInfo();
			
			messageInfo.messageType = MessageType.Theirs;
			messageInfo.message		= randomPuns[Random.Range(0, randomPuns.Length)];
			messageInfo.name		= "John Doe";
			messageInfo.timeStr		= ParseDateTime(System.DateTime.Now);
			
			messageInfos.Add(messageInfo);
			
			listScrollRect.RefreshContent();
			listScrollRect.GoToListItem(messageInfos.Count - 1);
		}

		if (waitTillRespond >= 0)
		{
			waitTillRespond--;
		}
	}

	#endregion

	#region Public Methods

	#region IContentFiller Methods
	
	public GameObject GetListItem(int index, int itemType, GameObject obj)
	{
		if (obj == null)
		{
			if (useThemeTwo)
			{
				// If we are using theme two then instantiate the themeTwoItemPrefab
				obj = Instantiate(themeTwoItemPrefab);
			}
			else
			{
				// Else instantiate the themeOneItemPrefab
				obj = Instantiate(themeOneItemPrefab);
			}
		}
		
		MessageInfo	messageInfo	= messageInfos[index];
		bool		hideTime	= !IsLastMessageInChain(index);
		
		if (useThemeTwo)
		{
			ThemeTwoMessageItem messageItem = obj.GetComponent<ThemeTwoMessageItem>();
			messageItem.Setup(messageInfo, hideTime);
		}
		else
		{
			ThemeOneMessageItem	messageItem	= obj.GetComponent<ThemeOneMessageItem>();
			bool				hideName	= !IsFirstMessageInChain(index);
			messageItem.Setup(messageInfo, hideTime, hideName);
		}
		
		return obj;
	}
	
	public int GetItemCount()
	{
		return messageInfos.Count;
	}
	
	public int GetItemType(int index)
	{
		// There will only ever be one item type being used at a time (ThemeOne or ThemeTwo)
		return 0;
	}
	
	#endregion

	public void OnSendButtonClicked()
	{
		string message = messageInputField.text;

		if (!string.IsNullOrEmpty(message))
		{
			MessageInfo messageInfo = new MessageInfo();

			messageInfo.messageType = MessageType.Yours;
			messageInfo.message		= message;
			messageInfo.name		= "You";
			messageInfo.timeStr		= ParseDateTime(System.DateTime.Now);

			messageInfos.Add(messageInfo);

			listScrollRect.RefreshContent();
			listScrollRect.GoToListItem(messageInfos.Count - 1);

			waitTillRespond = 100;
		}
	}

	public void OnSelectTheme()
	{
		bool temp = useThemeTwo;

		useThemeTwo = (themeDropdown.value == 1);

		if (temp != useThemeTwo)
		{
			float paddingTop	= useThemeTwo ? 10 : 0;
			float paddingBottom	= useThemeTwo ? 10 : 0;
			float spacing		= useThemeTwo ? 10 : 0;

			// Signal ListScrollRect to rebuild all the list items since the prefab we are using changed
			listScrollRect.RebuildContent(0, 0, paddingTop, paddingBottom, spacing);
		}
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Checks if MessageInfo at index is the first message in a chain of messages from the same person (ei. MessageType)
	/// </summary>
	private bool IsFirstMessageInChain(int index)
	{
		if (index == 0)
		{
			return true;
		}

		MessageType messageType1 = messageInfos[index].messageType;
		MessageType messageType2 = messageInfos[index - 1].messageType;

		return messageType1 != messageType2;
	}

	/// <summary>
	/// Checks if MessageInfo at index is the last message in a chain of messages from the same person (ei. MessageType)
	/// </summary>
	private bool IsLastMessageInChain(int index)
	{
		if (index == messageInfos.Count - 1)
		{
			return true;
		}
		
		MessageType messageType1 = messageInfos[index].messageType;
		MessageType messageType2 = messageInfos[index + 1].messageType;
		
		return messageType1 != messageType2;
	}

	/// <summary>
	/// Creates a bunch of random messages to be displayed in the list
	/// </summary>
	private void CreateDefaultStartingMessages()
	{
		System.DateTime time = System.DateTime.Now;

		for (int i = 0; i < 50; i++)
		{
			MessageInfo messageInfo = new MessageInfo();

			messageInfo.messageType = (MessageType)Random.Range(0, (int)MessageType.Count);
			
			switch (messageInfo.messageType)
			{
			case MessageType.Yours:
				messageInfo.name = "You";
				break;
			case MessageType.Theirs:
				messageInfo.name = "John Doe";
				break;
			}
			
			messageInfo.message = randomPuns[Random.Range(0, randomPuns.Length)];
			messageInfo.timeStr	= ParseDateTime(time);

			time = time.Add(new System.TimeSpan(0, -Random.Range(1, 11), 0));

			messageInfos.Insert(0, messageInfo);
		}
	}

	/// <summary>
	/// Parses the date time and returns a string to be displayed
	/// </summary>
	private string ParseDateTime(System.DateTime time)
	{
		string[] months = {"Jan", "Feb", "Mar", "April", "May", "June", "July", "Aug", "Sept", "Oct", "Nov", "Dec"};

		string	ampm = (time.Hour >= 12) ? "pm" : "am";
		int		hour = time.Hour;

		if (hour == 0)
		{
			hour = 12;
		}
		else if (hour > 12)
		{
			hour -= 12;
		}

		string minuteStr = time.Minute < 10 ? "0" + time.Minute : time.Minute.ToString();

		return string.Format("{0}, {1} {2} {3}:{4}{5}", months[time.Month - 1], time.Day, time.Year, hour, minuteStr, ampm);
	}

	#endregion
}
