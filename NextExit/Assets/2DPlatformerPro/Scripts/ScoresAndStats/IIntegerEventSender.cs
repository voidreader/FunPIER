using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Interface for objects that send Integer based events
	/// </summary>
	public interface IIntegerEventSender
	{
		/// <summary>
		/// Gets the change in value.
		/// </summary>
		/// <value>The change.</value>
		int Change 
		{
			get;
		}
		
		/// <summary>
		/// Gets the current value.
		/// </summary>
		int Current
		{
			get;
		}
	}
}