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
	public class DepthSwitchPlatform : Platform
	{
		/// <summary>
		/// The z-layer to switch to when moving right.
		/// </summary>
		public int movingRightLayer;

		/// <summary>
		/// The z-layer to switch to when moving left.
		/// </summary>
		public int movingLeftLayer;

		/// <summary>
		/// Custom collission switch depth based on movement direction.
		/// </summary>
		/// <param name="PlatformCollisionArgs">Arguments describing a platform collision.</param>
		/// <returns>true if character should be parented to this platform, otherwise false.</returns>
		/// <param name="args">Arguments.</param>
		override protected bool CustomCollide(PlatformCollisionArgs args)
		{
			if (args.RaycastCollider.RaycastType == RaycastType.FOOT)
			{
				if (args.Character.Velocity.x > 0) args.Character.ZLayer = movingRightLayer;
				if (args.Character.Velocity.x < 0) args.Character.ZLayer = movingLeftLayer;
			}
			return false;
		}
	}
	
}