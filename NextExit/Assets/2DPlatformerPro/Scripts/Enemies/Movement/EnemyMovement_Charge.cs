#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An enemy movement that simply runs in a given direction optionaly shotting projectiles.
	/// </summary>
	public class EnemyMovement_Charge : EnemyMovement
	{
		#region members
		/// <summary>
		/// The speed the platform moves at.
		/// </summary>
		public float speed;

		/// <summary>
		/// Will the enemy change direction when it hits the character?
		/// </summary>
		public bool bounceOnHit;

		/// <summary>
		/// How often to shoot
		/// </summary>
		[Tooltip ("How often to shoot. 0 for never.")]
		public float rateOfFire = 2.0f;

		/// <summary>
		/// How often to shoot
		/// </summary>
		[Tooltip ("How long to stay in the shoot state.")]
		public float shootTime = 0.25f;

		/// <summary>
		/// The prefab to use for the projectile.
		/// </summary>
		[Tooltip ("The prefab to use for the projectile.")]
		public GameObject projectilePrefab;
		
		/// <summary>
		/// The damage amount.
		/// </summary>
		[Tooltip ("The amount of damage done.")]
		public int damageAmount;
		
		/// <summary>
		/// The type of the damage.
		/// </summary>
		[Tooltip ("The type of damage.")]
		public DamageType damageType;

		/// <summary>
		/// The animation state to set while charging.
		/// </summary>
		[Tooltip ("The animation state to set while charging.")]
		public AnimationState animationState = AnimationState.WALK;

		/// <summary>
		/// When this is zero ... shoot!
		/// </summary>
		protected float firingTimer;
		
		/// <summary>
		/// Cached reference to a projectile aimer, or null if there is no aimer.
		/// </summary>
		protected ProjectileAimer projectileAimer;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Charge";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that simply runs in a given direction optionaly shooting projectiles.";

		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		new public static MovementInfo Info
		{
			get
			{
				return new MovementInfo(Name, Description);
			}
		}

		
		/// <summary>
		/// Name of the animation override
		/// </summary>
		protected const string overrideName = "SHOOT";

		#endregion

		#region properties

		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				return animationState;
			}
		}
		
		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (speed > 0) return 1;
				if (speed < 0) return -1;
				return 0;
			}
		}

		#endregion

		#region Unity hooks
		
		/// <summary>
		/// Unity Update() hook.
		/// </summary>
		void Update()
		{
			if (shootTime > 0) firingTimer -= TimeManager.FrameTime;
		}
		
		#endregion

		#region public methods
		
		/// <summary>
		/// Initialise this movement and return a reference to the ready to use movement.
		/// </summary>
		override public EnemyMovement Init(Enemy enemy)
		{
			this.enemy = enemy;
			projectileAimer = GetComponent<ProjectileAimer>();
			return this;
		}
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public bool DoMove()
		{
			// This movement can be the default or it can handle the patrol state.
			if (enemy.State != EnemyState.DEFAULT && enemy.State != EnemyState.CHARGING && enemy.State != EnemyState.FALLING) return false;

			if (firingTimer <= 0.0f && rateOfFire > 0)
			{
				DoShoot();
			}

			enemy.Translate(speed * TimeManager.FrameTime, 0, false);

			return true;
		}

		/// <summary>
		/// Called when the enemy hits the character.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="info">Damage info.</param>
		override public void HitCharacter(Character character, DamageInfo info)
		{
			if (bounceOnHit) speed *= -1;
		}
		
		/// <summary>
		/// Called by the enemy to switch (x) direction of the movement. Note that not all 
		/// movements need to support this, they may do nothing.
		/// </summary>
		override public void SwitchDirection()
		{
			speed *= -1;
		}

		#endregion

		#region protected methods

		/// <summary>
		/// Do the shoot.
		/// </summary>
		virtual protected void DoShoot()
		{
			firingTimer = rateOfFire;
			StartCoroutine (ShootRoutine ());
		}

		/// Fire projectile then temporarily set an animation override.
		/// </summary>
		virtual protected IEnumerator ShootRoutine()
		{	
			// Instantiate prefab
			GameObject go = (GameObject) GameObject.Instantiate(projectilePrefab);
			Projectile projectile = go.GetComponent<Projectile>();
			if (projectileAimer != null) 
			{
				go.transform.position = enemy.transform.position + (Vector3)projectileAimer.GetAimOffset(enemy);
			}
			else
			{
				go.transform.position = enemy.transform.position;
			}
			
			if (projectile != null) {
				// Fire projectile if the projectile is of type projectile
				Vector2 direction = new Vector2(enemy.LastFacedDirection != 0 ? enemy.LastFacedDirection : 1, 0);
				// Use aimer to get direction fo fire if the aimer is configured
				if (projectileAimer != null) direction = projectileAimer.GetAimDirection(enemy);
				projectile.Fire (damageAmount, damageType, direction, enemy);
			}
			enemy.AddAnimationOverride (overrideName);
			yield return new WaitForSeconds(shootTime);
			enemy.RemoveAnimationOverride(overrideName);
		}

		/// <summary>
		/// Set the direction of the charge.
		/// </summary>
		/// <param name="direction">Direction.</param>
		override public void SetDirection(Vector2 direction)
		{
			if (direction.x > 0 && speed > 0) SwitchDirection ();
			else if (direction.x < 0 && speed < 0) SwitchDirection ();
		}

		#endregion

#if UNITY_EDITOR

		// Place holder for draw gizmos, etc.
#endif

	}

}