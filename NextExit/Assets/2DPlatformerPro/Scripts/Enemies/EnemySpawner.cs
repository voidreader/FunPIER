using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	public class EnemySpawner : Spawner
	{

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
	}
}
