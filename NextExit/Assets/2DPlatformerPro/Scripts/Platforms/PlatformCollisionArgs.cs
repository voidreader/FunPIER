using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Data about a platform collision.
	/// </summary>
	public class PlatformCollisionArgs
	{
		/// <summary>
		/// The character that collided.
		/// </summary>
		public IMob Character
		{
			get; set;
		}

		/// <summary>
		/// The collider that collided.
		/// </summary>
		public BasicRaycast RaycastCollider
		{
			get; set;
		}

		/// <summary>
		/// The depth of the penetaation.
		/// </summary>
		public float Penetration
		{
			get; set;
		}

	}

}