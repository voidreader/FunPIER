using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Level event arguments.
	/// </summary>
	public class LevelEventArgs : System.EventArgs
	{

		/// <summary>
		/// Gets or sets the level name.
		/// </summary>
		public string LevelName
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.LevelEventArgs"/> class.
		/// </summary>
		/// <param name="levelName">Level name.</param>
		public LevelEventArgs(string levelName)
		{
			LevelName = levelName;
		}

	}
	
}
