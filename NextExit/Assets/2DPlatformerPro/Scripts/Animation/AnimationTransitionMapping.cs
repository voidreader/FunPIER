using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Animation transition mapping. Relates a state and previous state.
	/// </summary>
	[System.Serializable]
	public class AnimationTransitionMapping
	{
		[Tooltip ("The state we are moving from")]
		public AnimationState previousState;

		[Tooltip ("The state we are moving to")]
		public AnimationState newState;
	}
}
