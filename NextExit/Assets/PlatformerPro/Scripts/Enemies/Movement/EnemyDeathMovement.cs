using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy death movement.
	/// </summary>
	public abstract class EnemyDeathMovement : EnemyMovement
	{

		/// <summary>
		/// What does this death movement respond to?
		/// </summary>
		/// <returns></returns>
		public DamageMovementType damageMovementType = DamageMovementType.DAMAGE_AND_DEATH; 
		
	}
	
	/// <summary>
	/// What does this damage movement respond to.
	/// </summary>
	public enum DamageMovementType
	{
		DAMAGE_AND_DEATH,
		DAMAGE_ONLY,
		DEATH_ONLY
	}
}