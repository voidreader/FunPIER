using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// Event sent by game manager to indicate loading state.
	/// </summary>
	public class GamePhaseEventArgs : System.EventArgs 
	{

		/// <summary>
		/// Current loading phase.
		/// </summary>
		/// <value>The phase.</value>
		public GamePhase Phase
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.GamePhaseEventArgs"/> class.
		/// </summary>
		/// <param name="phase">Phase.</param>
		public GamePhaseEventArgs(GamePhase phase)
		{
			this.Phase = phase;
		}
	}
}