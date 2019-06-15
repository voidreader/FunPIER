using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{

	/// <summary>
	/// Stores data about a multi-projectile attack.
	/// </summary>
	[System.Serializable]
	public class MultiProjectileAttackData : BasicAttackData
	{
		public List<MultiProjectileData> projectileData;

	}
	/// <summary>
	/// Stores data about a projectile for a multi-projectile attack.
	/// </summary>
	[System.Serializable]
	public class MultiProjectileData
	{
		/// <summary>
		/// Prefab to use.
		/// </summary>
		public GameObject projectilePrefab;

		/// <summary>
		/// Offset applied to firiing position.
		/// </summary>
		public Vector2 positionOffset;

		/// <summary>
		/// Per projectile firing delay.
		/// </summary>
		public float delay;

		/// <summary>
		/// Rotation applied to facing direction.
		/// </summary>
		public float angleOffset;

		/// <summary>
		/// Should we flip X?
		/// </summary>
		public 	bool flipX;

		/// <summary>
		/// Should we flip Y?
		/// </summary>
		public bool flipY;
	}
}

