using UnityEngine;
using System.Collections;


namespace PlatformerPro
{
	/// <summary>
	/// Interface for touch buttons.
	/// </summary>
	public interface ITouchButton
	{
		/// <summary>
		/// Gets the state of the button.
		/// </summary>
		ButtonState ButtonState { get; }
	}

}