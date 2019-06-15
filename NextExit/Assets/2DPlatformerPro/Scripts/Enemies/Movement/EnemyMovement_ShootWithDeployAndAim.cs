using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy movement which deploys a weapon (animation) then shoots (roughly) in direction of character.
	/// </summary>
	public class EnemyMovement_ShootWithDeployAndAim : EnemyMovement_SimpleShoot
	{
		
		#region members

		/// <summary>
		/// The deploy time.
		/// </summary>
		[Tooltip ("How long it takes to deploy weapon.")]
		public float deployTime = 0.5f;

		/// <summary>
		/// Track our internal state.
		/// </summary>
		protected LocalState state = LocalState.UNDEPLOYED;

		/// <summary>
		/// The deploy timer.
		/// </summary>
		protected float deployTimer;

		/// <summary>
		/// Store the current target so we can follow them.
		/// </summary>
		protected Character currentTarget;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Shoot/Deploy and Aim";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Enemy movement which deploys a weapon (animation) then shoots (roughly) in direction of character.";
		
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

		#endregion
		
		#region properties
		
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (state == LocalState.UNDEPLOYED) return AnimationState.IDLE;
				if (state == LocalState.DEPLOYING) return AnimationState.DEPLOY;
				if (state == LocalState.READY) return AnimationState.IDLE_ARMED;
				if (state == LocalState.SHOOTING) return AnimationState.ATTACK_SHOOT;
				if (state == LocalState.UNDEPLOYING) return AnimationState.UNDEPLOY;
				return AnimationState.IDLE;
			}
		}
		
		#endregion
		
		#region Unity hooks

		/// <summary>
		/// Unity Update() hook.
		/// </summary>
		void Update()
		{
			if (deployTimer > 0) deployTimer -= TimeManager.FrameTime;
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
			if (state == LocalState.READY && enemy.AI != null && enemy.AI.CurrentTarget != null)
			{
				if (enemy.AI.CurrentTarget.transform.position.x > transform.position.x && enemy.LastFacedDirection == -1)
				{
					enemy.SwitchDirection();
				}
				else if (enemy.AI.CurrentTarget.transform.position.x < transform.position.x && enemy.LastFacedDirection == 1)
	         	{
					enemy.SwitchDirection();
				}
			}
			if (state == LocalState.UNDEPLOYED)
			{
				deployTimer = deployTime;
				state = LocalState.DEPLOYING;
			}
			else if (state == LocalState.DEPLOYING)
			{
				if (deployTimer <= 0.0f) state = LocalState.READY;
			}
			else 
			{
				if (firingTimer <= 0.0f)
				{
					DoShoot();
				}
			}
			return false;
		}

		/// <summary>
		/// Called when this movement is losing control. In this case we want to play the undeply animation.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		override public bool LosingControl()
		{
			currentTarget = null;
			if (state != LocalState.UNDEPLOYED)
    		{
				if (state == LocalState.UNDEPLOYING) 
				{
					if (deployTimer <= 0.0f) 
					{
						state = LocalState.UNDEPLOYED;
						return false;
					}
				}
				else 
				{
					deployTimer = deployTime;
					state = LocalState.UNDEPLOYING;
				}
				return true;
			}
			return false;
		}

		#endregion

		#region protected methods

		/// <summary>
		/// Fire projectile then temporarily set an animation override.
		/// </summary>
		override protected IEnumerator ShootRoutine()
		{
			state = LocalState.SHOOTING;
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
			state = LocalState.READY;
		}

		#endregion

		protected enum LocalState 
		{
			UNDEPLOYED, DEPLOYING, READY, SHOOTING, UNDEPLOYING
		}

	}
}
