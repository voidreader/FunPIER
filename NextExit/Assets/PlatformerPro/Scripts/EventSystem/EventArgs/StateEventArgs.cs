using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// State event arguments.
	/// </summary>
	public class StateEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets the state name
		/// </summary>
		public string StateName 
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.StateEventArgs"/> class.
		/// </summary>
		/// <param name="stateName">State name.</param>
		public StateEventArgs(string stateName)
		{
			StateName = stateName;
		}

		/// <summary>
		/// Updates the phase event arguments.
		/// </summary>
		/// <param name="state">State.</param>
		virtual public void UpdateAnimationEventArgs(string stateName)
		{
			StateName = stateName;
		}
	}

}
