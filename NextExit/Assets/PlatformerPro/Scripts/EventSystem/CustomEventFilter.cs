using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlatformerPro {
	
	/// <summary>
	/// Extend this to write your own filters for EventResponders. There's a sample implementation
	/// called SkipOnDeathEventFilter.
	/// </summary>
	public abstract class CustomEventFilter : MonoBehaviour {
		
		/// <summary>
		/// Custom event filter. Returns true if conditions met or false otherwise.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="args">Arguments.</param>
		abstract public bool Filter (Character character, EventResponse action, System.EventArgs args);
	}
}