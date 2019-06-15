using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that has a depth and only registers collissions from a certain depth.
	/// </summary>
	public class DepthPlatform : Platform
	{
		/// <summary>
		/// The zLayer or depth of this platform
		/// </summary>
		public int zLayer;
		
		/// <summary>
		/// Called to determine if collision should be ignored. Use for one way platforms or z-ordered platforms
		/// like those found in loops.
		/// </summary>
		/// <returns><c>true</c>, if Collision should be ignored, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="collider">Collider.</param>
		override public bool IgnoreCollision(Character character, BasicRaycast collider)
		{
			if (character.ZLayer != zLayer) return true;
			return false;
		}

	}
	
}