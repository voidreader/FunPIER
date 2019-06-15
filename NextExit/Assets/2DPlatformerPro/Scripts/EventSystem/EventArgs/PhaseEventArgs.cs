using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Phase event arguments.
	/// </summary>
	public class PhaseEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets the phase name
		/// </summary>
		public string PhaseName 
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.PhaseEventArgs"/> class.
		/// </summary>
		/// <param name="phaseName">Phase name.</param>
		public PhaseEventArgs(string phaseName)
		{
			PhaseName = phaseName;
		}

		/// <summary>
		/// Updates the phase event arguments.
		/// </summary>
		/// <param name="state">Phase.</param>
		virtual public void UpdateAnimationEventArgs(string phaseName)
		{
			PhaseName = phaseName;
		}
	}

}
