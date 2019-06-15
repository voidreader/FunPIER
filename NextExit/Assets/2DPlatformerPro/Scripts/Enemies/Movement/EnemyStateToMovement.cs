using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Relates an enemy state to an enemy movement.
	/// </summary>
	[System.Serializable]
	[ExecuteInEditMode]
	public class EnemyStateToMovement
	{

		/// <summary>
		/// When an enemy is in this state the movement should be executed.
		/// </summary>
		public EnemyState state;

		/// <summary>
		/// The enemy movement 
		/// </summary>
		public EnemyMovement movement;
	}

	/// <summary>
	/// High level enemy states for which different movements can be played by a EnemyMovement_Distributor.
	/// </summary>
	public enum EnemyState
	{
		DEFAULT,
		HITTING,
		ATTACKING,
		SHOOTING,
		GUARD,
		PATROL,
		FLEE,
		DAMAGED,
		DEAD,
		FALLING,
		CHARGING,
		HIDING,
		FLYING

	}



}