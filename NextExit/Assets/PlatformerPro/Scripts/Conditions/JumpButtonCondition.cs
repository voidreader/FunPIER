using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// COndition which is true if user is pressing or holding jump.
	/// </summary>
	public class JumpButtonCondition : AdditionalCondition 
	{
		/// <summary>
		/// Set to true if the user must press jump while at the trigger. Otherwise holding jump will work too.
		/// </summary>
		[Tooltip ("Set to true if the user must press jump while at the trigger. Otherwise holding jump will work too.")]
		public bool mustBePressNotHold = false;

		/// <summary>
		/// Returns true if jump is being pressed.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (character.Input.JumpButton == ButtonState.DOWN || (character.Input.JumpButton == ButtonState.HELD && !mustBePressNotHold)) return true;
			return false;
		}
	
	}
}
