using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Maps between animation states and strings
	/// </summary>
	[System.Serializable]
	public class AnimationMapping
	{
		/// <summary>
		/// The animation state to map from.
		/// </summary>
		public AnimationState state;

		/// <summary>
		/// The animation name to map to.
		/// </summary>
		public string animationName;

	}
}