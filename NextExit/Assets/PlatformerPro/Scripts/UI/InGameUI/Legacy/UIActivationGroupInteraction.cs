using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Optional component which can be added to control ActivationGroups with action buttons. Typically used for adding
	/// next/prev behaviour to an ActionvationGroup used  by UIActionBarButtons.
	/// </summary>
	public class UIActivationGroupInteraction : MonoBehaviour 
	{

		/// <summary>
		/// Action button to use for moving to the next item in the list or -1 for no next button.
		/// </summary>
		[Tooltip("Action button to use for moving to the next item in the list or -1 for no next button.")]
		public int actionButtonNext =-1;

		/// <summary>
		/// The action button to use for mocing to the previous item in the list or -1 for no prev button.
		/// </summary>
		[Tooltip("The action button to use for mocing to the previous item in the list or -1 for no prev button.")]
		public int actionButtonPrev = -1;

		[Header ("Character Reference")]

		/// <summary>
		/// Input to use, if null component will try to find one in the scene.
		/// </summary>
		[Tooltip("Character to use, if null component will try to find character in the scene.")]
		public Character character;
		
		/// <summary>
		/// The character loader.
		/// </summary>
		[Tooltip("Character loader to use, if null component will try to find one in the scene")]
		public PlatformerProGameManager characterLoader;

		[Header ("Group Reference")]

		/// <summary>
		/// Activation group to interact with.
		/// </summary>
		[Tooltip("ActivationGroup to use, you must specify one of group or groupName")]
		public ActivationGroup group;
		
		/// <summary>
		/// The name of the activation group.
		/// </summary>
		[Tooltip("Name of the ActivationGroup to use, you must specify one of group or groupName")]
		public string groupName;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start() 
		{
			GetCharacter ();
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update() 
		{
			if (character != null && group != null) CheckButtons ();
		}

		/// <summary>
		/// Checks for presses of next or previous button.
		/// </summary>
		virtual protected void CheckButtons()
		{
			if (actionButtonNext != -1 && character.Input.GetActionButtonState (actionButtonNext) == ButtonState.DOWN) group.Next ();
			else if (actionButtonPrev != -1 && character.Input.GetActionButtonState(actionButtonPrev) == ButtonState.DOWN) group.Previous ();
		}

		/// <summary>
		/// Do the destroy actions (remove event listeners).
		/// </summary>
		virtual protected void DoDestroy()
		{
			if (characterLoader != null) characterLoader.CharacterLoaded -= HandleCharacterLoaded;
		}

		/// <summary>
		/// Gets a character ref or defers to loader event.
		/// </summary>
		virtual protected void GetCharacter()
		{
			if (character != null)
			{
				GetGroup();
				return;
			}
			if (characterLoader == null)
			{
				characterLoader = FindObjectOfType<PlatformerProGameManager>();
			}
			if (characterLoader != null)
			{
				characterLoader.CharacterLoaded += HandleCharacterLoaded;
			}
			if (characterLoader == null)
			{
				character = FindObjectOfType<Character>();
				GetGroup();
			}
			if (character == null && characterLoader == null)
			{
				Debug.LogWarning("UIActivationGroupInteraction couldn't find a Character or CharacterLoader");
			}

		}

		void GetGroup() {
			if (character != null && group == null)
			{
				// We only get called if groups was null, so try to find the group
				foreach (ActivationGroup g in character.GetComponentsInChildren<ActivationGroup>())
				{
					if (g.groupName == groupName)
					{
						group = g;
						break;
					}
				}
				if (group == null)
				{
					// Search rest of scene
					foreach (ActivationGroup g in FindObjectsOfType<ActivationGroup>())
					{
						if (g.groupName == groupName)
						{
							group = g;
							break;
						}
					}
				}
			}
			if (group == null)
			{
				Debug.LogWarning("UIActivationGroupInteraction couldn't find Activation Group");
			}
		}

		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			character = e.Character;
			GetGroup();
		}
	}
}