using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A trapdoor is a platform that stops applying conditions when active. Usually needs to be activated by a trigger.
	/// </summary>
	public class TrapdoorPlatform : Platform
	{
		/// <summary>
		/// Called to determine if collision should be ignored. 
		/// </summary>
		/// <returns>true when active</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="collider">Collider.</param>
		override public bool IgnoreCollision(Character character, BasicRaycast collider)
		{
			// Ignore all collissions when active
			if (Activated) return true;
			if (base.IgnoreCollision(character, collider)) return true;
			return false;
		}

	}
}
