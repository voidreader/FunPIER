using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A class for an event for which the key piece of data is an integer.
	/// </summary>
	public class IntegerBasedEventArgs : System.EventArgs
	{
		
		/// <summary>
		/// Gets or sets the  int value.
		/// </summary>
		/// <value>The previous scene.</value>
		virtual public int IntValue
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.IntegerBasedEventArgs"/> class.
		/// </summary>
		public IntegerBasedEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.IntegerBasedEventArgs"/> class.
		/// </summary>
		/// <param name="intValue">Int value.</param>
		public IntegerBasedEventArgs(int intValue)
		{
			IntValue = intValue;
		}
	}
}