using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition which only passes if character is above/below a certain height.
	/// </summary>
	public class HeightCondition : AdditionalCondition 
	{
		/// <summary>
		/// Y position of character. Character must be higher than this to pass.
		/// </summary>
		[Tooltip ("Y position of character.")]
		public float height;

		/// <summary>
		/// If true condition is relative to current transform, false for an absolute world position.
		/// </summary>
		[Tooltip ("If true condition is relative to current transform, false for an absolute world position.")]
		public bool isRelative = true;

		/// <summary>
		/// If true character must be at or below given height, false they must be at or above.
		/// </summary>
		public bool mustBeBelow;

		/// <summary>
		/// Returns true if character is above/below given height.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (mustBeBelow)
			{
				if (character.transform.position.y <= (height + (isRelative ? transform.position.y : 0.0f))) return true;
			} 
			else
			{
				if (character.transform.position.y >= (height + (isRelative ? transform.position.y : 0.0f))) return true;
			}
			return false;
		}
	}
}
