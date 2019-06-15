using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Interface for animation bridges.
	/// </summary>
	public interface IAnimationBridge
	{
		/// <summary>
		/// Called when the animator is changed.
		/// </summary>
		void Reset();

		/// <summary>
		/// Gets the associated animator. Returns null if an animator is not being used.
		/// </summary>
		Animator Animator
		{
			get;
		}
	}
}