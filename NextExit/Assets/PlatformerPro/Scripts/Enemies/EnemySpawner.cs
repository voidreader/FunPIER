using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	public class EnemySpawner : Spawner
	{

		/// <summary>
		/// How many enemies can be alive at any one time.
		/// </summary>
		[Tooltip ("How many enemies can be alive at any one time. Use 0 for no limit.")]
		public int maxAlive;

		/// <summary>
		/// The default health to restore when resetting
		/// </summary>
		protected int defaultHealth;

		/// <summary>
		/// The default can cahracter fall setting to restore. here because often death movements change this.
		/// </summary>
		protected bool defaultCanCharacterFall;

		/// <summary>
		/// Initialise pool.
		/// </summary>
		override protected void Init()
		{
			if (poolPrefab != null)
			{
				Enemy enemy = poolPrefab.GetComponent<Enemy> ();
				if (enemy == null)
				{
					Debug.LogWarning ("Enemy spawner prefab must have an Enemy component attached.");
				}
				else 
				{
					defaultHealth = enemy.health;
					defaultCanCharacterFall = enemy.characterCanFall;

					#if UNITY_EDITOR
					EnemyMovement_Damaged_Bobble deathMovement1 = enemy.GetComponentInChildren<EnemyMovement_Damaged_Bobble> ();
					if (deathMovement1.destroyDelay > 0)
						Debug.LogWarning ("For spawning enemies the death movement destroyDelay should be set to 0 (don't destroy) else the enemy annot be resued in the pool");
					EnemyMovement_Damage_PlayAnimationOnly deathMovement2 = enemy.GetComponentInChildren<EnemyMovement_Damage_PlayAnimationOnly> ();
					if (deathMovement2.destroyDelay > 0)
						Debug.LogWarning ("For spawning enemies the death movement destroyDelay should be set to 0 (don't destroy) else the enemy annot be resued in the pool");
					EnemyMovement_Damaged_Knockback deathMovement3 = enemy.GetComponentInChildren<EnemyMovement_Damaged_Knockback> ();
					if (deathMovement3.destroyDelay > 0)
						Debug.LogWarning ("For spawning enemies the death movement destroyDelay should be set to 0 (don't destroy) else the enemy annot be resued in the pool");
					#endif

				}
			}
			// For safety lets check the pool too
#if UNITY_EDITOR
			if (pooledObjects != null)
			{
				for(int i = 0; i < pooledObjects.Count; i++)
				{
					Enemy enemy = pooledObjects[i].GetComponent<Enemy> ();
					if (enemy == null) Debug.LogWarning ("Enemy spawner pool objects should have an Enemy component attached.");
				}
			}
#endif
			base.Init ();
		}

		/// <summary>
		/// Reset the specified instance. Specific implementations should override this.
		/// </summary>
		/// <param name="instance">Instance.</param>
		override protected void Reset(GameObject instance)
		{
			Enemy enemy = instance.GetComponent<Enemy> ();
			enemy.health = defaultHealth;
			enemy.characterCanFall = defaultCanCharacterFall;
			enemy.MakeVulnerable ();
			enemy.Reset ();
			instance.transform.position = transform.position;
		}

		/// <summary>
		/// Coroutine that spawns enemies.
		/// </summary>
		override protected IEnumerator Spawn()
		{
			float timer = delayOnFirstInstance ? spawnRate : 0;
			while (spawnsRemaining > 0 || spawnsRemaining == -1)
			{
				while (timer > 0)
				{
					if (enabled) timer -= TimeManager.FrameTime;
					yield return true;
				}
				if (maxAlive <= 0 || CheckAliveCount()) DoSpawn ();
				timer = spawnRate;
			}
		}

		/// <summary>
		/// Check we don't have too many enemies alive and thus can't spawn.
		/// </summary>
		/// <returns><c>true</c>, if alive count was checked, <c>false</c> otherwise.</returns>
		virtual protected bool CheckAliveCount() {
			int aliveCount = 0;
			if (pooledObjects == null) return false;
			for (int i = 0; i < pooledObjects.Count; i++)
			{
				if (pooledObjects[i].activeInHierarchy) {
					aliveCount++;
					if (aliveCount >= maxAlive) return false;
				}
			}
			return true;
		}

	}
}
