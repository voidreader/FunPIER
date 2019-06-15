using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Spawns random items.
	/// </summary>
	public class RandomItemSpawner : MonoBehaviour
	{
		/// <summary>
		/// List of prefabs we can spawn. To change liklihood of spawning different items just add more items of the same type.
		/// </summary>
		[Tooltip ("List of prefabs we can spawn. To change liklihood of spawning different items just add more items of the same type.")]
		public List<GameObject> itemPrefabs;

		/// <summary>
		/// The spawn position offset.
		/// </summary>
		[Tooltip ("Where to spawn the prefab relative to the current game objects transform.")]
		public Vector3 spawnPositionOffset;
		
		/// <summary>
		/// The spawn velocity.
		/// </summary>
		[Tooltip ("If the prefab has a Rigidbody2D, set velocity to one of these (ranodmly selected). Otherwise ignored.")]
		public List <Vector3> spawnVelocities;

		/// <summary>
		/// The number to spawn.
		/// </summary>
		[Tooltip ("How many items to spawn before spawning stops. Note some classes like SpawnOnHeadbutt may override this.")]
		public int numberToSpawn = 1;

		
		/// <summary>
		/// The number to spawn.
		/// </summary>
		[Tooltip ("How many items to spawn with each hit.")]
		public int spawnsPerHit = 1;

		/// <summary>
		/// After an item spawns should we remove it from the list? Handy if you want to spawn an exact set of items in random order
		/// or if you don't want to spawn rare items more than once.
		/// </summary>
		[Tooltip ("After an item spawns should we remove it from the list? Handy if you want to spawn an exact set of items in random order or if you don't want to spawn rare items more than once.")]
		public bool removeFromListOnSpawn;

		/// <summary>
		/// Spawn this instance.
		/// </summary>
		/// <returns>true if there are items less to spawn or false otherwise.</returns>
		public bool Spawn()
		{
			// Don't spawn if no items to spawn.
			if (itemPrefabs == null || itemPrefabs.Count == 0) return false;
			// Don't spawn if no count remains.
			if (numberToSpawn < 1) return false;

			int spawnSeed = -1;
			for (int i = spawnsPerHit; numberToSpawn > 0 && i > 0  && itemPrefabs.Count > 0; i--)
			{
				// Get prefab to spawn
				GameObject prefabToSpawn = itemPrefabs [Random.Range (0, itemPrefabs.Count)];
				if (removeFromListOnSpawn) itemPrefabs.Remove (prefabToSpawn);

				// Create instance
				GameObject spawnGo = (GameObject)GameObject.Instantiate(prefabToSpawn);
				spawnGo.transform.position = transform.position + spawnPositionOffset;
				Rigidbody2D spawnRigidbody = spawnGo.GetComponent<Rigidbody2D>();
				if (spawnRigidbody != null && spawnVelocities != null && spawnVelocities.Count > 0)
				{
					if (spawnsPerHit > 1)
					{
						// Start somewhere randomly in the velocity list
						if (spawnSeed == -1) spawnSeed = Random.Range (0, spawnVelocities.Count);
						spawnRigidbody.velocity = spawnVelocities [spawnSeed % spawnVelocities.Count];
						spawnSeed ++;
					}
					else
					{
						spawnRigidbody.velocity = spawnVelocities [Random.Range (0, spawnVelocities.Count)];
					}
				}
				numberToSpawn--;
			}
			// Return
			if (numberToSpawn <= 0 || itemPrefabs.Count == 0) return false;
			return true;
		}
	}
}
