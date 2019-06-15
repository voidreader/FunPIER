
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition that is met only when character is standing on a certain type of ground.
	/// </summary>
	public class GroundLayerCondition : AdditionalCondition 
	{
		/// <summary>
		/// Layers character must be standing on.
		/// </summary>
		[Tooltip ("Character must be standing on one of these layers for this condition to be true.")]
		public LayerMask groundLayer;
	
		/// <summary>
		/// The result if not grounded.
		/// </summary>
		[Tooltip ("Should we return true or false if not grounded?")]
		public bool resultIfNotGrounded;

		/// <summary>
		/// Returns true if character is standing on correct layer.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (!character.Grounded) return resultIfNotGrounded;
			if ((1 << character.GroundLayer) == (groundLayer & (1 << character.GroundLayer))) return true;
			return false;
		}
	}
}
