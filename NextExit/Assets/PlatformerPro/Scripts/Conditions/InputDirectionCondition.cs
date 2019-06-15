using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition which is true if user is pressing or holding a given direction.
	/// </summary>
	public class InputDirectionCondition : AdditionalCondition 
	{
		[Range (-1, 1)]
		[Tooltip ("Required Horizontal Input")]
		public int horizontalInput;

		[Tooltip ("is the horizontal direction relative to facing direction?")]
		public bool relativeToFacingDirection = true;

		[Range (-1, 1)]
		[Tooltip ("Required Vertical Input")]
		public int verticalInput;

		/// <summary>
		/// Returns true if jump is being pressed.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (horizontalInput != 0) 
			{
				if (!(character.Input.HorizontalAxisDigital == (relativeToFacingDirection ? (character.LastFacedDirection * horizontalInput) : horizontalInput))) return false;
			}
			if (verticalInput != 0) 
			{
				if (!(character.Input.VerticalAxisDigital == verticalInput)) return false;
			}
			return true;
		}
		
	}
}
