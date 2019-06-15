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
	public class EnemyMovement_ChargeAtTarget : EnemyMovement_Charge
	{

		/// <summary>
		/// Stops the enemy spinning on the spot when really close to the player.
		/// </summary>
		[Tooltip ("Stops the enemy spinning on the spot when really close to the player. Enemy will only turn when this far away.")]
		public float turnLeeway = 0.25f;

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Charge at Target";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that simply runs in direction of target optionaly shooting projectiles.";

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
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (enemy.CurrentTargetTransform != null) 
				{
					if (enemy.LastFacedDirection == 1)
					{
						if (enemy.CurrentTargetTransform.position.x + turnLeeway < enemy.transform.position.x) return -1;
						return 1;
					}
					if (enemy.LastFacedDirection == -1)
					{
						if (enemy.CurrentTargetTransform.position.x - turnLeeway>  enemy.transform.position.x) return 1;
						return -1;
					}
				}
			    return 0;
			}
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
			if (!(this.enemy is SequenceDrivenEnemy)) Debug.LogWarning("EnemyMovememt_ChargeAtTarget expects a SequenceDrivenEnemy. The Base Enemy class doesn't set a target");
			if (bounceOnHit) Debug.LogWarning("EnemyMovememt_ChargeAtTarget doesn't support bounceOnHit == true. It always runs at the player.");
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

			enemy.Translate(FacingDirection * speed * TimeManager.FrameTime, 0, false);

			return true;
		}

		/// <summary>
		/// Called when the enemy hits the character.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="info">Damage info.</param>
		override public void HitCharacter(Character character, DamageInfo info)
		{

		}
		
		/// <summary>
		/// Called by the enemy to switch (x) direction of the movement. Note that not all 
		/// movements need to support this, they may do nothing.
		/// </summary>
		override public void SwitchDirection()
		{
		
		}

		#endregion

		#region protected methods

		/// <summary>
		/// Set the direction of the charge.
		/// </summary>
		/// <param name="direction">Direction.</param>
		override public void SetDirection(Vector2 direction)
		{
			Debug.LogWarning ("EnemyMovememt_ChargeAtTarget doesn't support SetDirection . It always runs at target");
		}

		#endregion

#if UNITY_EDITOR

		// Place holder for draw gizmos, etc.
#endif

	}

}