using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Combines conditions and returns true if all of them are true.
	/// </summary>
	public class AndConditionCombiner : AdditionalCondition 
	{
		/// <summary>
		/// The conditions to check.
		/// </summary>
		[Tooltip ("List of conditions to check. If all are true, then this will evaluate to true.")]
		public AdditionalCondition[] conditions;
		
		/// <summary>
		/// Returns true if any cindition is true.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			foreach (AdditionalCondition condition in conditions)
			{
				if (!condition.CheckCondition(character, other)) return false;
			}
			return true;
		}
	}
}