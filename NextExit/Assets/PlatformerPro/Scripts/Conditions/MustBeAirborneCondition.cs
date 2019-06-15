using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition that is met only when character is grounded.
	/// </summary>
	public class MustBeAirborneCondition : AdditionalCondition 
	{

		/// <summary>
		/// The minimum time since grounded before this will return true. If 0 this check just tests !Grounded.
		/// </summary>
		[Tooltip ("The minimum time since grounded before this will return true. If 0 this check just tests !Grounded.")]
		public float minimumTimeSinceGrounded;

		/// <summary>
		/// Returns true if character is airbone.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (!character.Grounded && (minimumTimeSinceGrounded <= 0 || character.TimeSinceGroundedOrOnLadder >= minimumTimeSinceGrounded)) return true;
			return false;
		}
		
	}
}
