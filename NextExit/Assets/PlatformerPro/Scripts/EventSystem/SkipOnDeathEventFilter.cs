using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro {
	
	/// <summary>
	/// A custom event filter that skips the event if the character is dead.
	/// </summary>
	public class SkipOnDeathEventFilter : CustomEventFilter {

		/// <summary>
		/// The cahracter health reference. You can leave this blank and the GetCharacter method of GenericResponder will be used to 
		/// try to find a Character and associated health.
		/// </summary>
		[Tooltip ("The character health reference. You can leave this blank and the GetCharacter method of GenericResponder will be used to try to find a Character and associated health.")]
		public CharacterHealth characterHealth;

		/// <summary>
		/// Custom event filter. Returns true if character is alive, false if not.
		/// </summary>
		/// <param name="character">Character or null.</param>
		/// <param name="action">Action.</param>
		/// <param name="args">Arguments.</param>
		public override bool Filter (Character character, EventResponse action, System.EventArgs args)
		{
			CharacterHealth tmpHealth = characterHealth;
			if (tmpHealth == null) 
			{
				if (character != null) 
				{
					tmpHealth = character.GetComponentInChildren<CharacterHealth>();
				}
			}
			if (tmpHealth != null)
			{ 
				if (tmpHealth.CurrentHealth <= 0) return false;
			} 
			else
			{
				Debug.LogWarning ("EventResponder had a SkipOnDeathEventFilter attached but we couldn't find the CharacterHealth associated with the event.");
			}
			return true;
		}
		
	}
}
