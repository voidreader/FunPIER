using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A button on an action bar. Can be used for switching wepaons, consuming items, or just as a standard ActionButton.
	/// </summary>
	public class UIActionBarButton_Attack : UIActionBarButton
	{

		[Header ("Attack Data")]

		public BasicAttacks attack;

		public string attackName;

		/// <summary>
		/// Index of the attack.
		/// </summary>
		protected int attackIndex = -1;

		/// <summary>
		/// Gets the character reference.
		/// </summary>
		/// <value>The character.</value>
		override public Character Character
		{
			get
			{
				if (attack == null) return null;
				return attack.Character;
			}
		}

		/// <summary>
		/// Gets a character ref or defers to loader event. Here we wait an extra frame to give group time to load.
		/// </summary>
		override protected void GetCharacter()
		{
			if (attack == null)
			{
				if (character != null)
				{
					// We only get called if groups was null, so try to find the group
					foreach (BasicAttacks a in character.GetComponentsInChildren<BasicAttacks>())
					{
						for (int i = 0; i < a.attacks.Count; i++)
						{
							if (attack != null) break;
							if (a.attacks[i].name == attackName)
							{
								attack = a;
								attackIndex = i;
								break;
							}
						}
					}
					UpdateFromAttackData ();
				}
				base.GetCharacter ();
			} 
			else
			{
				if (attackName == null || attackName == "")
				{
					attackName = attack.attacks[0].name;
					attackIndex = 0;
				}
				else
				{
					for (int i = 0; i < attack.attacks.Count; i++)
					{
						if (attack.attacks[i].name == attackName)
						{
							attackIndex = i;
							break;
						}
					}
				}
				UpdateFromAttackData();
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
			foreach (BasicAttacks a in e.Character.GetComponentsInChildren<BasicAttacks>())
			{
				for (int i = 0; i < a.attacks.Count; i++)
				{
					if (attack != null) break;
					if (a.attacks[i].name == attackName)
					{
						attack = a;
						attackIndex = i;
						break;
					}
				}
			}
			UpdateFromAttackData ();

		}

		virtual protected void UpdateFromAttackData()
		{
			if (attackIndex == -1 || attack == null)
			{
				Debug.LogWarning("Couldn't find an attack with the name " + attackName);
				gameObject.SetActive(false);
				return;
			}
			
			// Ammo. Should we also look for ItemConditions?
			if (attack.attacks[attackIndex].ammoType != null && attack.attacks[attackIndex].ammoType != "")
			{
				itemId = attack.attacks[attackIndex].ammoType;
				GetItemManager();
			}
			
			// Action button
			actionButton = attack.attacks[attackIndex].actionButtonIndex;
		}

		/// <summary>
		/// Handles the click event.
		/// </summary>
		override protected void DoPointerClick()
		{
			if (!canClickWhenPaused && TimeManager.Instance.Paused) return;
			if (Character != null)
			{
				OnActivated ();
				Character.Input.ForceButtonState(actionButton, ButtonState.DOWN);
			}
		}


	}
}
