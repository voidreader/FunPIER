using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A point at which the character can respawn. Transform should exactly match where the character will spawn.
	/// </summary>
	public class RespawnPoint : MonoBehaviour
	{
		/// <summary>
		/// Is this point the default starting point which is activated on scene load.
		/// </summary>
		[Tooltip ("Is this point the default starting point which is activated on scene load?")]
		public bool isDefaultStartingPoint;

		/// <summary>
		/// Unique name for this respawn point. 
		/// </summary>
		[Tooltip ("Unique name for this respawn point. ")]
		public string identifier;

		/// <summary>
		/// Set this respawn point as the active one.
		/// </summary>
		virtual public void SetActive()
		{
			LevelManager.Instance.ActivateRespawnPoint (identifier);
		}

		/// <summary>
		/// Enable this respawn point.
		/// </summary>
		virtual public void SetEnabled()
		{
			LevelManager.Instance.EnableRespawnPoint (identifier);
		}
	}
}