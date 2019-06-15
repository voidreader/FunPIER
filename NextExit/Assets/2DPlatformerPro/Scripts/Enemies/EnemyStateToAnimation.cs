using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Relates an enemy state to an enemy movement.
	/// </summary>
	[System.Serializable]
	public class EnemyStateToAnimation
	{
		/// <summary>
		/// When an enemy is in this state the animation should be played.
		/// </summary>
		public EnemyState state;
		
		/// <summary>
		/// The aimation that maps to the state.
		/// </summary>
		public AnimationState animation;
	}
}