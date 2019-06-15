using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// An item in a menu.
	/// </summary>
	public class UIMenuItem_NumberOfPlayers : UIMenuItem
	{
		
//		/// <summary>
//		/// A list of default inputs.
//		/// </summary>
//		[Tooltip ("A list of the player IDs which will be matched in the loader.")]
//		public List<string> playerIds = new List<string>() {"Player 1", "Player 2"};
//
//		/// <summary>
//		/// String to use for title if its a one player game.
//		/// </summary>
//		[Tooltip ("String to use for title if its a one player game..")]
//		public string singlePlayerFormatString = "1 Player Game";
//
//		/// <summary>
//		/// String to use for title if its a multiplayer game.
//		/// </summary>
//		[Tooltip ("String to use for title if its a multi-player game. Will be passed through string.Format() with the number of players as variable {0}.")]
//		public string multiPlayerFormatString = "{0} Player Game";
//
//		/// <summary>
//		/// Number of players selected
//		/// </summary>
//		protected int numberOfPlayers = 1;
//
//		/// <summary>
//		/// Cached menu reference.
//		/// </summary>
//		protected UIBasicMenu menu;
//
//		/// <summary>
//		/// Gets or sets the number of players.
//		/// </summary>
//		/// <value>The number of players.</value>
//		public int NumberOfPlayers 
//		{
//			get 
//			{
//				return numberOfPlayers;
//			}
//			set
//			{
//				if (value > playerIds.Count) numberOfPlayers = 1;
//				else if (value < 1) numberOfPlayers = playerIds.Count;
//				else numberOfPlayers = value;
//				PlatformerProGameManager.SetCharacterForPlayer(i, 
//			}
//		}
//
//		/// <summary>
//		/// Unity Awake() hook.
//		/// </summary>
//		void Awake() 
//		{
//			numberOfPlayers = 1;
//			if (playerIds == null || playerIds.Count < 1)
//			{
//				Debug.LogWarning("You need at least 2 players configured to be able to pick between players.");
//			}
//		}
//
//		void Start()
//		{
//			menu = gameObject.GetComponentInParent<UIBasicMenu> ();
//		}
//
//		/// <summary>
//		/// Gets the title.
//		/// </summary>
//		override public string Title
//		{
//			get
//			{ 
//				if (numberOfPlayers == 1) return singlePlayerFormatString;
//				return string.Format(multiPlayerFormatString, numberOfPlayers);
//			}
//		}
//
//		/// <summary>
//		/// Hitting the action key does nothing for this menu item type.
//		/// </summary>
//		override public void DoAction()
//		{
//			NumberOfPlayers++;
//			if (menu != null) menu.Refresh();
//		}
//
//		/// <summary>
//		/// Do the action for when the user presses right.
//		/// </summary>
//		override public void DoRightAction()
//		{
//			NumberOfPlayers++;
//			if (menu != null) menu.Refresh();
//		}
//
//		/// <summary>
//		/// Do the action for when the user presses left.
//		/// </summary>
//		override public void DoLeftAction()
//		{
//			NumberOfPlayers--;
//			if (menu != null) menu.Refresh();
//		}

	
	}


}