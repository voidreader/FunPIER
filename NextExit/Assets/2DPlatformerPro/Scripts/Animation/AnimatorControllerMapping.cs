using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Maps between animation states and controllers.
	/// </summary>
	[System.Serializable]
	public class AnimatorControllerMapping
	{
		/// <summary>
		/// The animation override state name as a string.
		/// </summary>
		public string overrrideState;
		
		/// <summary>
		/// The controller to map to.
		/// </summary>
		public AnimatorOverrideController controller;
		
	}
}