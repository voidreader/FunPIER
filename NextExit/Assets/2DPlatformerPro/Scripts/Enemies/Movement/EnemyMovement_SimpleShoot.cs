using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy movement which spawns a projectile and sets an animation override.
	/// </summary>
	public class EnemyMovement_SimpleShoot : EnemyMovement
	{
		
		#region members
		
		/// <summary>
		/// How often to shoot
		/// </summary>
		[Tooltip ("How often to shoot.")]
		public float rateOfFire = 1.0f;

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
		/// Should we set an animation state and override (true) or just an override (false).
		/// </summary>
		[Tooltip ("Should we set an animation state and override (true) or just an override (false).")]
		public bool setAnimationState;


		/// <summary>
		/// When this is zero ... shoot!
		/// </summary>
		protected float firingTimer;
		
		/// <summary>
		/// Cached reference to a projectile aimer, or null if there is no aimer.
		/// </summary>
		protected ProjectileAimer projectileAimer;

		/// <summary>
		/// Are we currently shooting.
		/// </summary>
		protected bool isShooting;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Shoot/Simple";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Enemy movement which spawns a projectile and sets an animation override.";
		
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
				if (setAnimationState)
				{
					if (isShooting) return AnimationState.ATTACK_SHOOT;
					return AnimationState.IDLE;
				}
				return AnimationState.NONE;
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
			if (firingTimer <= 0.0f)
			{
				DoShoot();
			}
			return false;
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

		/// <summary>
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
			isShooting = true;
			yield return new WaitForSeconds(shootTime);
			enemy.RemoveAnimationOverride(overrideName);
			isShooting = false;
		}

		#endregion
	}
}
