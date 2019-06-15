using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition that is met only when character is grounded.
	/// </summary>
	public class MustBeGroundedCondition : AdditionalCondition 
	{
		/// <summary>
		/// Returns true if  character is grounded.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (character.Grounded) return true;
			return false;
		}
		
	}
}
