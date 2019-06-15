using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Provides an axis via a touch input.
	/// </summary>
	public interface ITouchAxis
	{
		/// <summary>
		/// Gets the axis value.
		/// </summary>
		float Value { get; }
	}
}
