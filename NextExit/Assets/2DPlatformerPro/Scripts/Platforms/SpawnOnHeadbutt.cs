using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Platform that spawns (or adds) an item on headbutt.
	/// </summary>
	public class SpawnOnHeadbutt : Platform
	{

		[Header ("Damage")]
		/// <summary>
		/// If this is non-null the attached collider will be briefly enabled on headbut causing damage to anything touching it.
		/// This can be used for the typical mario style headbut of the block to cause damage,
		/// </summary>
		[Tooltip ("If this is non-null the attached collider will be briefly enabled on headbut causing damage to anything touching it.")]
		public Collider2D damageCollider;

		[Header ("Item Spawning")]
		/// <summary>
		/// The item  to add to the character item stack.
		/// </summary>
		[Tooltip ("Item  to add to the character. Leave empty if you don't want to add an item.")]
		public Item itemToAdd;

		[Tooltip ("If you wish to use a random item spawner, assign it here..")]
		public RandomItemSpawner randomItemSpawner;

		/// <summary>
		/// The number to spawn.
		/// </summary>
		[Tooltip ("How many items to spawn before spawning stops.")]
		public int numberToSpawn = 1;

		/// <summary>
		/// The sprite to switch to once spawning has stopped.
		/// </summary>
		[Tooltip ("The sprite to switch to once spawning has stopped. Leave blank if not using sprites.")]
		public Sprite spawningStoppedSprite;

		/// <summary>
		/// The bump amount.
		/// </summary>
		[Tooltip ("Nudge the box this high in to the air each time it is hit.")]
		public float nudgeAmount = 0.25f;

		/// <summary>
		/// True if we are playing the nudge coroutine.
		/// </summary>
		protected bool isInNudge;

		/// <summary>
		/// Tracks the force being applied during nudge.
		/// </summary>
		protected float velocity;

		/// <summary>
		/// The initial position.
		/// </summary>
		protected Vector3 initialPosition;

		/// <summary>
		/// The initial velocity calculated based on nudge amount.
		/// </summary>
		protected float initialVelocity;

		/// <summary>
		/// The spawns remaining.
		/// </summary>
		public int spawnsRemaining;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init ()
		{
			base.Init ();
			initialPosition = transform.position;
			initialVelocity = Mathf.Sqrt(-2.0f * Physics2D.gravity.y * nudgeAmount);
			spawnsRemaining = numberToSpawn;
			if (randomItemSpawner != null)
			{
				randomItemSpawner.numberToSpawn = numberToSpawn;
			}
		}

		/// <summary>
		/// Called when one of the characters colliders collides with this platform. This should be overriden for platform specific behaviour.
		/// </summary>
		/// <param name="PlatformCollisionArgs">Arguments describing a platform collision.</param>
		/// <returns>true if character should be parented to this platform, otherwise false.</returns>
		override protected bool CustomCollide(PlatformCollisionArgs args)
		{
			if (args.RaycastCollider.RaycastType == RaycastType.HEAD && args.Character is Character)
			{
				DoHeadbutt((Character)args.Character);
			}
			return false;
		}

		/// <summary>
		/// Do the headbutt
		/// </summary>
		virtual protected void DoHeadbutt(Character character)
		{
			if (!isInNudge) 
			{
				StartCoroutine (DoNudge (character));
			}
		}

		/// <summary>
		/// Do the nudge.
		/// </summary>
		/// <returns>The nudge.</returns>
		virtual protected IEnumerator DoNudge(Character character)
		{
			isInNudge = true;
			spawnsRemaining--;
			if (spawnsRemaining == 0) ShowStopSpawningSprite();
			if (itemToAdd != null && spawnsRemaining >= 0)
			{

				ItemManager itemManager = character.GetComponentInChildren<ItemManager>();
				if (itemManager != null)
				{
					itemManager.CollectItem(itemToAdd);
				}
				else
				{
					Debug.LogWarning("Tried to add an item but the item manager was not found");
				}

			}
			if (randomItemSpawner != null && spawnsRemaining >= 0)
			{
				if (!randomItemSpawner.Spawn())
				{
					spawnsRemaining = 0;
					ShowStopSpawningSprite();
				}
			}
			if (damageCollider != null)
			{
				damageCollider.enabled = true;
			}
			if (nudgeAmount > 0) 
			{
				velocity = initialVelocity;
				while (Physics2D.gravity.y < 0 && (velocity > 0 || initialPosition.y < transform.position.y))
				{
					yield return true;
					transform.Translate(0, velocity * TimeManager.FrameTime, 0);
					velocity += Physics2D.gravity.y * TimeManager.FrameTime;
				}
			}
			else
			{
				yield return true;
			}
			if (damageCollider != null)
			{
				damageCollider.enabled = false;
			}
			isInNudge = false;
		}

		/// <summary>
		/// Shows the stop spawning sprite.
		/// </summary>
		virtual protected void ShowStopSpawningSprite()
		{
			if (spawnsRemaining == 0 && spawningStoppedSprite)
			{
				SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
				if (spriteRenderer != null)
				{
					spriteRenderer.sprite = spawningStoppedSprite;
				}
				else
				{
					Debug.LogWarning("Couldn't set spawningStoppedSprite as no SpriteRenderer was found.");
				}
			}
		}

	}
}