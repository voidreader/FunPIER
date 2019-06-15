using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Scene event arguments.
	/// </summary>
	public class SceneEventArgs : System.EventArgs
	{

		/// <summary>
		/// Gets or sets the previous scene.
		/// </summary>
		/// <value>The previous scene.</value>
		public string PreviousScene
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the new scene.
		/// </summary>
		/// <value>The new scene.</value>
		public string NewScene
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the respawn point.
		/// </summary>
		/// <value>The respawn point.</value>
		public string RespawnPoint
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.SceneEventArgs"/> class.
		/// </summary>
		/// <param name="previousScene">Previous scene.</param>
		/// <param name="newScene">New scene.</param>
		public SceneEventArgs(string previousScene, string newScene)
		{
			PreviousScene = previousScene;
			NewScene = newScene;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.SceneEventArgs"/> class.
		/// </summary>
		/// <param name="previousScene">Previous scene.</param>
		/// <param name="newScene">New scene.</param>
		/// <param name="respawnPoint">Respawn point.</param>
		public SceneEventArgs(string previousScene, string newScene, string respawnPoint)
		{
			PreviousScene = previousScene;
			NewScene = newScene;
			RespawnPoint = respawnPoint;
		}

	}
	
}
