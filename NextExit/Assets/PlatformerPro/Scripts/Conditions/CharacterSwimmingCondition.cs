using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	public class CharacterSwimmingCondition : AdditionalCondition 
	{

		/// <summary>
		/// Returns true if required movement is active.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{			
			if (character.ActiveMovement is SpecialMovement_Swim || character.ActiveMovement is SpecialMovement_DirectionalSwim) return true;
			return false;
		}
		
	}
}
