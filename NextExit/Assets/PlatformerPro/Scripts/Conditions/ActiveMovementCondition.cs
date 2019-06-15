using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	public class ActiveMovementCondition : AdditionalCondition 
	{
		/// <summary>
		/// Movement that must be active for this condition to be met.
		/// </summary>
		[Tooltip ("Movement that must be active for this condition to be met.")]
		public Movement requiredMovement;

		/// <summary>
		/// Cached copy to underlying movement implementation.
		/// </summary>
		protected Movement actualRequiredMovement;

		/// <summary>
		/// Returns true if required movement is active.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (actualRequiredMovement == null) UpdateRequiredMovement ();
			if (character.ActiveMovement.GetType () == actualRequiredMovement.GetType ()) return true;
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

		/// <summary>
		/// Updates the required movement to be the actual required movement (i.e. get implementation from a base movement).
		/// </summary>
		protected void UpdateRequiredMovement() 
		{
			if (requiredMovement == null) Debug.LogWarning ("No movement has been assigned to ActiveMovementCondition");
			actualRequiredMovement = requiredMovement.Implementation;
			if (actualRequiredMovement == null) 
			{
				actualRequiredMovement = requiredMovement;
			}
		}
	}
}
