using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Health info event arguments.
	/// </summary>
	public class HealedEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets the amount healed.
		/// </summary>
		public int Amount
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.HealedEventArgs"/> class.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public HealedEventArgs(int amount)
		{
			Amount = amount;
		}

	}
	
}