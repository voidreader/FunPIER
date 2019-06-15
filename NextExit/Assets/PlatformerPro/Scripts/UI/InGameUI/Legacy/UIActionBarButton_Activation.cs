using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A button on an action bar that activates/deactivates.
	/// </summary>
	public class UIActionBarButton_Activation : UIActionBarButton 
	{
		[Header ("Activation Group and Item")]
		/// <summary>
		/// The activation group.
		/// </summary>
		[SerializeField]
		protected ActivationGroup group;

		/// <summary>
		/// The name of the activation group.
		/// </summary>
		[SerializeField]
		protected string groupName;

		/// <summary>
		/// The activation item id.
		/// </summary>
		[SerializeField]
		protected string activationItemId;

		/// <summary>
		/// Do the destroy actions (remove event listeners).
		/// </summary>
		override protected void DoDestroy()
		{
			base.DoDestroy ();
			if (group != null)
			{
				group.Activated -= HandleActivationChange;
				group.Deactivated -= HandleActivationChange;
			}
		}

		/// <summary>
		/// Gets the character reference.
		/// </summary>
		/// <value>The character.</value>
		override public Character Character
		{
			get
			{
				if (group == null) return null;
				return group.Character;
			}
		}

		/// <summary>
		/// Gets a character ref or defers to loader event. Here we wait an extra frame to give group time to load.
		/// </summary>
		override protected void GetCharacter()
		{
			StartCoroutine(GetCharacterAfterWait());
		}

		/// <summary>
		/// Waits a frame then gets character reference.
		/// </summary>
		/// <returns>The character after wait.</returns>
		protected IEnumerator GetCharacterAfterWait()
		{
			// Wait a frame for group to load
			yield return true;

			if (group == null)
			{
				base.GetCharacter ();
			} 
			else
			{
				// We use the action groups character
				if (group.requireMatchingItem)
				{
					itemId = activationItemId;
					GetItemManager ();
				}
				group.Activated += HandleActivationChange;
				group.Deactivated += HandleActivationChange;

				if (group.IsActive(activationItemId))
				{
					Activate();
				}
				else
				{
					Deactivate();
				}
			}
		}

		/// <summary>
		/// Handles the activation change.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandleActivationChange (object sender, ActivationEventArgs e)
		{
			if (group.IsActive(activationItemId))
			{
				Activate();
			}
			else
			{
				Deactivate();
			}
		}

		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		override protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			base.HandleCharacterLoaded (sender, e);
			// We only get called if groups was null, so try to find the group
			foreach (ActivationGroup g in e.Character.GetComponentsInChildren<ActivationGroup>())
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
			if (group != null)
			{
				if (group.requireMatchingItem) 
				{
					itemId = activationItemId;
					GetItemManager();
				}
				group.Activated += HandleActivationChange;
				group.Deactivated += HandleActivationChange;
			}
			else
			{
				Debug.LogWarning("No activation group with name " +groupName + " was found. Deactivating GameObject.");
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Handles the click event.
		/// </summary>
		override protected void DoPointerClick()
		{
			if (!canClickWhenPaused && TimeManager.Instance.Paused) return;
			if (group.IsActive (activationItemId))
			{
				if (allowDeactivate) group.Deactivate (activationItemId);
			}
			else
			{
				group.Activate (activationItemId);
			}
		}

	}
}
