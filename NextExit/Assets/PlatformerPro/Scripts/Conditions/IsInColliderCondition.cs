using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition which only passes if character transform is inside a collider.
	/// </summary>
	public class IsInColliderCondition : AdditionalCondition 
	{
		/// <summary>
		/// Cached collider reference.
		/// </summary>
		protected Collider2D colliderRef;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start() 
		{
			colliderRef = GetComponent<Collider2D> ();
			if (colliderRef == null) Debug.LogWarning ("IsInColliderCondition requires a collider to be on the same GameObject as the condition.");
		}
		/// <summary>
		/// Returns true if character is inside collider.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (colliderRef.OverlapPoint(character.transform.position)) return true;
			return false;
		}
	}
}
