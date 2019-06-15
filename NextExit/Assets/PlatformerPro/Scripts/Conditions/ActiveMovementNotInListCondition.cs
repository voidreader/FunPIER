using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition is met if active movement is NOT in the list.
	/// </summary>
	public class ActiveMovementNotInListCondition : AdditionalCondition 
	{
		/// <summary>
		/// Movement that must be active for this condition to be met.
		/// </summary>
		[Tooltip ("Movement that must be active for this condition to be met.")]
		public Movement[] blockedMovements;

		/// <summary>
		/// The actual blocked movements with implementation references replacing base movement refererences.
		/// </summary>
		protected Movement[] actualBlockedMovements;

		/// <summary>
		/// Returns true if jump is being pressed.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (actualBlockedMovements == null) UpdateBlockedMovements ();
			for (int i = 0; i < actualBlockedMovements.Length; i++) 
			{
				if (character.ActiveMovement.GetType () == actualBlockedMovements[i].GetType ()) return false;
			}
			return true;
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
		/// Updates the blocked movements to be the actual blocked movement (i.e. get implementation from a base movement).
		/// </summary>
		protected void UpdateBlockedMovements() 
		{
			actualBlockedMovements = new Movement[blockedMovements.Length];
			for (int i = 0; i < blockedMovements.Length; i++) 
			{
				Movement actual = null;
				actual = blockedMovements [i].Implementation;
				if (actual == null) {
					actual = blockedMovements [i];
				}
				actualBlockedMovements [i] = actual;
			}
		}

	}
}
