using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Stores data that drives the level manager.
	/// </summary>
	[System.Serializable]
	public class LevelManagerData
	{
		
		/// <summary>
		/// The currently active respawn point identifier.
		/// </summary>
		public string activeRespawnPoint;
		
		/// <summary>
		/// List of all enabled respawn points.
		/// </summary>
		public List<string> enabledRespawnPoints;

		/// <summary>
		/// Name or key for the level being locked/unlocked.
		/// </summary>
		public List<LevelLockData> levelLockData;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.LevelManagerData"/> class.
		/// </summary>
		public LevelManagerData()
		{
			
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.LevelManagerData"/> class.
		/// </summary>
		/// <param name="levelKey">Level key.</param>
		/// <param name="isUnlocked">If set to <c>true</c> is unlocked.</param>
		public LevelManagerData(string activeRespawnPoint, List<string>  enabledRespawnPoints, List<LevelLockData> levelLockData)
		{
			this.activeRespawnPoint = activeRespawnPoint;
			this.enabledRespawnPoints = new List<string> ();
			this.enabledRespawnPoints.AddRange (enabledRespawnPoints);
			this.levelLockData = new List<LevelLockData> ();
			this.levelLockData.AddRange (levelLockData);
		}
	}
}