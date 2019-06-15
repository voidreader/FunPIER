using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Projectile used in projectile (ranged) attacks that is controlled by physics.
	/// </summary>
	[RequireComponent (typeof(Rigidbody2D))]
	public class GrenadeProjectile : Projectile
	{
		/// <summary>
		/// How does the grenade work?
		/// </summary>
		[Tooltip ("How does the grenade trigger?")]
		public GrenadeType grenadeTrigger;

		/// <summary>
		/// How long is out timer (or how long after hitting something until we explode if type == EXPLODE_ON_HIT).
		/// </summary>
		[Tooltip ("How long is out timer (or how long after hitting something until we explode if type == EXPLODE_ON_HIT).")]
		public float triggerDelay = 0.0f;

		/// <summary>
		/// Has greande triggered?
		/// </summary>
		protected bool triggered;

		/// <summary>
		/// Character reference.
		/// </summary>
		protected IMob character;
		
		/// <summary>
		/// Reference to the rigidbody 2D.
		/// </summary>
		new protected Rigidbody2D rigidbody2D;

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update() {
			// Override update, no need to move, rigidbody will handle it
		}

		/// <summary>
		/// Call to start the projectile moving.
		/// </summary>
		/// <param name="damageAmount">Damage amount.</param>
		/// <param name="damageType">Damage type.</param>
		override public void Fire(int damageAmount, DamageType damageType, Vector2 direction, IMob character) 
		{
			rigidbody2D = GetComponent<Rigidbody2D> ();
			fired = true;
			damageInfo = new DamageInfo(damageAmount, damageType, Vector2.zero, character);
			rigidbody2D.velocity = (new Vector2(0,1) + direction) * speed;
			this.character = character;
			if (grenadeTrigger == GrenadeType.EXPLODE_AFTER_DELAY)
			{
				StartCoroutine(DoTrigger());
			}
		}

		/// <summary>
		/// Destroy projectile.
		/// </summary>
		override public void DestroyProjectile(bool isEnemyHit)
		{
			// Ignore
		}

		/// <summary>
		/// Delays then sends event and enables hit box.
		/// </summary>
		/// <returns>The trigger.</returns>
		virtual protected IEnumerator DoTrigger()
		{
			yield return new WaitForSeconds(triggerDelay);
			projectileHitBox.enabled = true;
			projectileHitBox.Init (damageInfo, character, this, false, false);
			rigidbody2D.isKinematic = true;
			rigidbody2D.gravityScale = 0;
			OnProjectileDestroyed(damageInfo);
			yield return new WaitForSeconds(destroyDelay);
			fired = false;
			projectileHitBox.gameObject.SetActive (false);
			Destroy (gameObject);
		}
		
		/// <summary>
		/// Unity collider trigger. Start the explosiion coroutine.
		/// </summary>
		void OnCollisionEnter2D (Collision2D info) 
		{
			if (!triggered && grenadeTrigger == GrenadeType.EXPLODE_ON_HIT) 
			{
				triggered = true;
				StartCoroutine(DoTrigger());
			}
		}
	}

	public enum GrenadeType
	{
		EXPLODE_ON_HIT,
		EXPLODE_AFTER_DELAY
	}
}