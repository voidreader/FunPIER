using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Projectile used in projectile (ranged) attacks. Note you don't have to use projectile class for projectiles.
	/// </summary>
	public class Projectile : MonoBehaviour
	{
		/// <summary>
		/// Should this object be destroyed when this object hits an enemy?
		/// </summary>
		[Tooltip ("Should this object be destroyed when this object hits an enemy?")]
		public bool destroyOnEnemyHit;

		/// <summary>
		/// Should this object be destroyed when this object hits scenery?
		/// </summary>
		[Tooltip ("Should this object be destroyed when this object hits scenery?")]
		public bool destroyOnSceneryHit;

		/// <summary>
		/// How fast the projectile moves.
		/// </summary>
		[Tooltip ("How fast the projectile moves.")]
		public float speed;

		/// <summary>
		/// The hit box that causes damage to enemies.
		/// </summary>
		[Tooltip ("The hit box that causes damage to enemies.")]
		public ProjectileHitBox projectileHitBox;

		/// <summary>
		/// How long after projectile fires should we wait before automatically exploding it. 0 Means it wont explodes until it hits something.
		/// </summary>
		[Tooltip ("How long after projectile fires should we wait before automatically exploding it. 0 Means it wont explodes until it hits something.")]
		public float autoExplodeDelay;

		/// <summary>
		/// How long after projectile collides should we wait before destroying it.
		/// </summary>
		[Tooltip ("How long after projectile collides should we wait before destroying it.")]
		public float destroyDelay;

		/// <summary>
		/// Should the projectile be rotated to face the direction of travel.
		/// </summary>
		[Tooltip ("Should the projectile be rotated to face the direction of travel.")]
		public bool rotate = true;

		/// <summary>
		/// Damage this projectile will cause.
		/// </summary>
		protected DamageInfo damageInfo;

		/// <summary>
		/// Movement direction.
		/// </summary>
		protected Vector2 direction;

		/// <summary>
		/// Have we been fired?
		/// </summary>
		protected bool fired;

		/// <summary>
		/// The actual speed.
		/// </summary>
		protected float actualSpeed;

		/// <summary>
		/// How long before we destory this bullet.
		/// </summary>
		protected float remainingLifetime;

		/// <summary>
		/// Cached rigidbody. IF a non-kinematic rigidboy is attached we launch by setting velocity and do not move each frame.
		/// </summary>
		protected Rigidbody2D myRigidbody;

		/// <summary>
		/// Gets the charge which can be used by effects, etc.
		/// </summary>
		/// <value>The charge.</value>
		public float Charge
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the charge as an int which can be used by effects, etc.
		/// </summary>
		/// <value>The charge.</value>
		public int ChargeLevel
		{
			get;
			protected set;
		}

		/// <summary>
		/// Event for when a projectile is fired. Typically used for enabling effects.
		/// </summary>
		public event System.EventHandler <System.EventArgs> ProjectileFired;

		/// <summary>
		/// Event for when a projectile crashes in to something. If the object being crashed 
		/// in to is not an enemy then the damage info will be null.
		/// </summary>
		public event System.EventHandler <DamageInfoEventArgs> ProjectileDestroyed;

		/// <summary>
		/// Raises the projectile fired event.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual protected void OnProjectileFired()
		{
			if (ProjectileFired != null)
			{
				ProjectileFired(this, null);
			}
		}

		/// <summary>
		/// Raises the projectile destroyed event.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual protected void OnProjectileDestroyed(DamageInfo info)
		{
			if (ProjectileDestroyed != null)
			{
				DamageInfoEventArgs args = new DamageInfoEventArgs(info);
				ProjectileDestroyed(this, args);
			}
		}

		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update() {
			if (fired) Move();
		}

		/// <summary>
		/// Move the projectile, override if you want custom movement.
		/// </summary>
		virtual protected void Move()
		{
			if (myRigidbody != null)
			{
				return;
			}
			Vector3 translation = direction * TimeManager.FrameTime * actualSpeed;
			transform.Translate(translation, Space.World);
			if (remainingLifetime > 0)
			{
				remainingLifetime -= TimeManager.FrameTime;
				if (remainingLifetime < 0) DestroyProjectile(false);
			}
		}

		/// <summary>
		/// Call to start the projectile moving.
		/// </summary>
		/// <param name="damageAmount">Damage amount.</param>
		/// <param name="damageType">Damage type.</param>
		/// <param name="direction">Direction of fire</param>
		/// <param name="character">Character doing the firing.</param>
		/// <param name="charge">How charged is the attack. Ignored by default you need to use a projectile type like ChargedProjectile to utilise this.</param>
		virtual public void Fire(int damageAmount, DamageType damageType, Vector2 direction, IMob character, float charge = 1.0f) 
		{
			Charge = charge;
			fired = true;
			damageInfo = new DamageInfo(CalculateDamageAmount(damageAmount, charge), damageType, Vector2.zero, character);
			if (projectileHitBox != null) projectileHitBox.Init(damageInfo, character, this, destroyOnEnemyHit, destroyOnSceneryHit);
			this.direction = direction;
			this.direction.Normalize();
			actualSpeed = CalculateSpeed(charge);
			if (rotate)
			{
				transform.rotation = Quaternion.FromToRotation(Vector2.right, direction);
			}
			if (autoExplodeDelay > 0) remainingLifetime = autoExplodeDelay;
			myRigidbody = GetComponent<Rigidbody2D> ();
			if (myRigidbody != null && myRigidbody.isKinematic)
			{
				myRigidbody = null;
			}
			else if (myRigidbody != null)
			{
				myRigidbody.centerOfMass = new Vector2 (2.0f, 0);
				myRigidbody.AddForce (this.direction * actualSpeed, ForceMode2D.Impulse);

			}
			OnProjectileFired ();
		}

		/// <summary>
		/// Calculates the actual damage amount.
		/// </summary>
		/// <returns>The damage amount.</returns>
		/// <param name="damageAmount">Damage amount from the attack.</param>
		/// <param name="charge">Charge time.</param>
		virtual protected int CalculateDamageAmount(int damageAmount, float charge)
		{
			return damageAmount;
		}

		/// <summary>
		/// Calculates the actual speed.
		/// </summary>
		/// <returns>The speed.</returns>
		/// <param name="charge">Charge time.</param>
		virtual protected float CalculateSpeed(float charge)
		{
			return speed;
		}

		/// <summary>
		/// End the characters attack, for most projectiles you will ignore this, but it may be useful for some movement types.
		/// </summary>
		virtual public void Finish()
		{

		}

		/// <summary>
		/// Destroy projectile.
		/// </summary>
		virtual public void DestroyProjectile(bool isEnemyHit)
		{
			fired = false;
			projectileHitBox.gameObject.SetActive (false);
			if (myRigidbody != null) myRigidbody.simulated = false;
			OnProjectileDestroyed(isEnemyHit ? damageInfo : null);
			StartCoroutine(DoDestroy(isEnemyHit));
		}

		/// <summary>
		/// Sends the desstory event, then waits for detroy delay, then destroys GO.
		/// </summary>
		virtual protected IEnumerator DoDestroy(bool isEnemyHit)
		{
			yield return new WaitForSeconds(destroyDelay);
			Destroy (gameObject);
		}
	}

}