using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Combines conditions and returns true if any of them are true.
	/// </summary>
	public class OrConditionCombiner : AdditionalCondition 
	{
		/// <summary>
		/// The conditions to check.
		/// </summary>
		[Tooltip ("List of conditions to check. If any is true, then this will evaluate to true.")]
		public AdditionalCondition[] conditions;

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			foreach (AdditionalCondition condition in conditions)
			{
				if (condition.gameObject == gameObject) Debug.LogWarning("The additional conditions in an OrConditionCombiner must be on a different game object to the OrConditionCombiner or else they will be picked up by the base object");
			}
		}

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
				if (condition.CheckCondition(character, other)) return true;
			}
			return false;
		}
	}
}