using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition which requires a specific amount of health.
	/// </summary>
	public class MinimumHealthCondition : AdditionalCondition
	{
		/// <summary>
		/// The character health
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// If this is not empty require the character to have an item with the matching type before triggering.
		/// </summary>
		[Tooltip ("The amount of health required.")]
		public int requiredHealth;

		/// <summary>
		/// The optional number of the item to consume when the effect is activated.
		/// </summary>
		[Tooltip ("The optional amount of health consumed when the effect is activated.")]
		public int numberConsumed = 0;

		/// <summary>
		/// If only one character will ever use this, save a reference to the cached health.
		/// </summary>
		[Tooltip ("If only one character will ever use this condition, save a reference to the character health.")]
		public bool cacheCharacterHealth = true;

		/// <summary>
		/// Checks the condition. For example a check when entering a trigger.
		/// </summary>
		/// <returns><c>true</c>, if enter trigger was shoulded, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			
			if (characterHealth == null || !cacheCharacterHealth)
			{
				characterHealth = character.GetComponentInChildren<CharacterHealth> ();
			}
			if (characterHealth == null) 
			{
				Debug.LogWarning("Conditions requires health but the character has no character health.");
				return false;
			}
			if (characterHealth.CurrentHealth >= requiredHealth) return true;
			return false;
		}

		/// <summary>
		/// Applies any activation effects.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="other">Other object supporting the condition.</param>
		override public void Activated(Character character, object other)
		{
			if (numberConsumed > 0)
			{
				if (characterHealth.CurrentHealth <= numberConsumed)
				{
					characterHealth.Kill ();
				}
				else
				{
					characterHealth.CurrentHealth = characterHealth.CurrentHealth - numberConsumed;
				}
			}
		}
	}

}
