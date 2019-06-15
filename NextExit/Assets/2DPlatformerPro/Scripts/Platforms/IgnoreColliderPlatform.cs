using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that ignores user specified colliders. Use for making boudnaries impassable from certain directions.
	/// Generally not for passthroughs. Use the passthrough layer for passthroughs.
	/// </summary>
	public class IgnoreColliderPlatform : Platform
	{
		/// <summary>
		/// Should we ignore feet colliders?
		/// </summary>
		public bool ignoreFeet;

		/// <summary>
		/// Should we ignore head colliders?
		/// </summary>
		public bool ignoreHead;

		/// <summary>
		/// Should we ignore left side colliders?
		/// </summary>
		public bool ignoreLeft;

		/// <summary>
		/// Should we ignore right side colliders?
		/// </summary>
		public bool ignoreRight;

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Collider2D collider = GetComponent<Collider2D> ();
			if (!(collider is EdgeCollider2D)) Debug.LogWarning ("Ignore collision platforms should use an EdgeCollider2D");
		}

		/// <summary>
		/// Called to determine if collision should be ignored. Use for one way platforms or z-ordered platforms
		/// like those found in loops.
		/// </summary>
		/// <returns><c>true</c>, if Collision should be ignored, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="collider">Collider.</param>
		override public bool IgnoreCollision(Character character, BasicRaycast collider)
		{
			if (collider.RaycastType == RaycastType.FOOT && ignoreFeet) return true;
			if (collider.RaycastType == RaycastType.HEAD && ignoreHead) return true;
			if (collider.RaycastType == RaycastType.SIDE_LEFT && ignoreLeft) return true;
			if (collider.RaycastType == RaycastType.SIDE_RIGHT && ignoreRight) return true;
			return false;
		}

	}
	
}