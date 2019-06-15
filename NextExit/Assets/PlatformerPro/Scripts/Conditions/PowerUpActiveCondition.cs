using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition which requires a specific power up to be active.
	/// </summary>
	public class PowerUpActiveCondition : AdditionalCondition
	{
		/// <summary>
		/// The item manager.
		/// </summary>
		protected PowerUpManager powerUpManager;
		
		/// <summary>
		/// Type of power up to check for.
		/// </summary>
		[Tooltip ("The type of PowerUp to check for.")]
		public string powerUpType;
		
		/// <summary>
		/// If only one character will ever use this, save a reference to the item manager.
		/// </summary>
		[Tooltip ("If only one character will ever use this condition, save a reference to the item manager.")]
		public bool cachePowerUpManager = true;
		
		/// <summary>
		/// Checks the condition. For example a check when entering a trigger.
		/// </summary>
		/// <returns><c>true</c>, if enter trigger was shoulded, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (powerUpManager == null || !cachePowerUpManager)
			{
				powerUpManager = character.GetComponentInChildren<PowerUpManager> ();
			}
			if (powerUpManager == null) 
			{
				Debug.LogWarning("Conditions checks for power-ups but the character has no power up manager.");
				return false;
			}
			if (powerUpManager.IsActive(powerUpType)) return true;
			return false;
		}
		
	}
	
}
