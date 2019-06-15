using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PlatformerPro
{
	/// <summary>
	/// Spawns enemies.
	/// </summary>
	public class Spawner : Pool
	{
		[Header ("Spawning")]

		/// <summary>
		/// How many enemies to spawn, use -1 for infinite.
		/// </summary>
		[Tooltip ("How many objects to spawn, use -1 for infinite.")]
		public int spawnAmount;

		/// <summary>
		/// How fast should we spawn.
		/// </summary>
		[Tooltip ("How fast should we spawn.")]
		public float spawnRate;
		
		/// <summary>
		/// If true the first instance will be delayed by the spawn rate, else it will spawn instantly.
		/// </summary>
		[Tooltip ("If true the first instance will be delayed by the spawn rate, else it will spawn instantly.")]
		public bool delayOnFirstInstance;

		/// <summary>
		/// How many spwns remain.
		/// </summary>
		protected int spawnsRemaining;

		/// <summary>
		/// Initialise pool.
		/// </summary>
	    override protected void Init()
	    {
			base.Init ();
			spawnsRemaining = spawnAmount;
			StartCoroutine (Spawn ());
		}

		/// <summary>
		/// Coroutine that spawns enemies.
		/// </summary>
		virtual protected IEnumerator Spawn()
		{
			float timer = delayOnFirstInstance ? spawnRate : 0;
			while (spawnsRemaining > 0 || spawnsRemaining == -1)
			{
				while (timer > 0)
				{
					if (enabled) timer -= TimeManager.FrameTime;
					yield return true;
				}
				DoSpawn ();
				timer = spawnRate;
			}
		}

		/// <summary>
		/// Do the spawning.
		/// </summary>
		virtual protected void DoSpawn()
		{
			GameObject instance = GetInstance ();
			// Only decrement the spawn if there was an enemy in the pool
			if (instance != null) spawnsRemaining--;
		}
	}
}
