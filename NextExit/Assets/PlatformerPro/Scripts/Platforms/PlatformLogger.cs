using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A debugging class that logs the calls received by the platform.
	/// </summary>
	public class PlatformLogger : Platform
	{

		/// <summary>
		/// Called when one of the characters colliders collides with this platform.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="type">Type of raycast.</param>
		/// <returns>tAlways returns false.</returns>
		override public bool Collide(PlatformCollisionArgs args)
		{
			Debug.Log ("Collide: " + args.Character + " " + args.RaycastCollider.RaycastType);
			return false;
		}

	}

}