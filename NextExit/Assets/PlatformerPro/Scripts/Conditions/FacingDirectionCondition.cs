using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	public class FacingDirectionCondition : AdditionalCondition 
	{
		/// <summary>
		/// Direction character must be facing.
		/// </summary>
		[Tooltip ("Direction character must be facing.")]
		[Range (-1, 1)]
		public int facingDirection;

		/// <summary>
		/// If true used the last faced direciton instead of the facing direction/
		/// </summary>
		[Tooltip ("If true used the last faced direciton instead of the facing direction/")]
		public bool useLastFacedDirection = true;

		/// <summary>
		/// Returns true if jump is being pressed.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if ((useLastFacedDirection ? character.LastFacedDirection : character.FacingDirection) == facingDirection) return true;
			return false;
		}
		
		/// <summary>
		/// Returns true if trigger should trigger exit event.
		/// </summary>
		override public bool CheckInverseCondition(Character character, object other)
		{
			if (!applyOnInverse) return true;
			return CheckCondition(character, other);
		}
	}
}
