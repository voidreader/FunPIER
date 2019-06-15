using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Stores lock data (which is typically about a locked/unlocked level).
	/// </summary>
	[System.Serializable]
	public class LevelLockData
	{
		/// <summary>
		/// Name or key for the level being locked/unlocked.
		/// </summary>
		public string levelKey;

		/// <summary>
		/// Is the level unlocked?
		/// </summary>
		public bool isUnlocked;

        /// <summary>
        /// What rating did the player achieve on level complete
        /// </summary>
        public int rating;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.LevelLockData"/> class.
		/// </summary>
		public LevelLockData()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.LevelLockData"/> class.
		/// </summary>
		/// <param name="levelKey">Level key.</param>
		/// <param name="isUnlocked">If set to <c>true</c> is unlocked.</param>
		public LevelLockData(string levelKey, bool isUnlocked, int rating)
		{
			this.levelKey = levelKey;
			this.isUnlocked = isUnlocked;
            this.rating = rating;
		}
	}
}