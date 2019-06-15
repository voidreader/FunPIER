using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Arguments for a score event (i.e. when the score changes).
	/// </summary>
	public class ScoreEventArgs : System.EventArgs
	{

		/// <summary>
		/// Gets or sets the change in score.
		/// </summary>
		/// <value>The change.</value>
		public int Change 
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the level score.
		/// </summary>
		public int Current
		{
			get;
			protected set;
		}

		/// <summary>
		/// Identifier for the score type.
		/// </summary>
		public string ScoreType
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ScoreEventArgs"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		public ScoreEventArgs(string type)
		{
			ScoreType = type;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ScoreEventArgs"/> class.
		/// </summary>
		/// <param name="change">Change.</param>
		/// <param name="current">Current.</param>
		/// <param name="type">Type.</param>
		public ScoreEventArgs(int change, int current, string type)
		{
			Change = change;
			Current = current;
			ScoreType = type;
		}

		/// <summary>
		/// Updates the score event.
		/// </summary>
		/// <param name="change">Change.</param>
		/// <param name="current">Current value.</param>
		public void UpdateScore(int change, int current)
		{
			Change = change;
			Current = current;
		}
	}
}