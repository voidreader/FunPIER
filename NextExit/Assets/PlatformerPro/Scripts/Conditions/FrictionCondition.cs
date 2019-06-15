using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// Friction condition. Activates when friction is above or below a given threshold.
	/// </summary>
	public class FrictionCondition : AdditionalCondition
	{
		/// <summary>
		/// What friction do we activate on.
		/// </summary>
		[Tooltip ("What friction do we activate on.")]
		public float requiredFriction;

		/// <summary>
		/// If true friction must be less than or equal to provided number, else it must be more than or equal to.
		/// </summary>
		[Tooltip ("If true friction must be less than or equal to provided number, else it must be more than or equal to.")]
		public bool frictionMustBeLessThan;

		/// <summary>
		/// Checks the condition. For example a check when entering a trigger.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{

			if (character.StoodOnPlatform != null)
			{
				if (frictionMustBeLessThan)
				{
					if (character.StoodOnPlatform.Friction < requiredFriction)
						return true;
				} 
				else
				{
					if (character.StoodOnPlatform.Friction > requiredFriction) return true;
				}
				return false;
			}
			return true;
		}
	}
}