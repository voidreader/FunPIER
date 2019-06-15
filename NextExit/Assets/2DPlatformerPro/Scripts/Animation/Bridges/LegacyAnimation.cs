using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A data class which assocaites a legacy animation to an animation state.
	/// </summary>
	[System.Serializable]
	public class LegacyAnimation {

		/// <summary>
		/// The animation state.
		/// </summary>
		public AnimationState state;

		/// <summary>
		/// The animation clip to play for the given animation state.
		/// </summary>
		public AnimationClip clip;

		/// <summary>
		/// Wrap mode for the clip.
		/// </summary>
		public WrapMode wrapMode;

		/// <summary>
		/// Time for the cross fade between this and the previous state.
		/// </summary>
		public float fadeTime;

	}
}
