using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy movement which spawns a projectile and sets an animation override whilst patrolling back and forth.
	/// </summary>
	public class EnemyMovement_PatrolWithShoot : EnemyMovement_SimpleShoot
	{
		
		#region members

		/// The distance from starting position to the right extent.
		/// </summary>
		public float rightOffset;
		
		/// <summary>
		/// The distance from starting position to the left extent.
		/// </summary>
		public float leftOffset;
		
		/// <summary>
		/// The speed the platform moves at.
		/// </summary>
		public float speed;
		
		/// <summary>
		/// Will the enemy change direction when it hits the character?
		/// </summary>
		public bool bounceOnHit;
		
		/// <summary>
		/// The right extent.
		/// </summary>
		protected float rightExtent;
		
		/// <summary>
		/// The left extent.
		/// </summary>
		protected float leftExtent;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Patrol with Shoot";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Enemy movement which spawns a projectile and sets an animation override whilst patrolling back and forth.";
		
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
				if (setAnimationState && isShooting) return AnimationState.ATTACK_SHOOT;
				return AnimationState.WALK;
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
			leftExtent = enemy.transform.position.x - leftOffset;
			rightExtent = enemy.transform.position.x + rightOffset;
			return this;
		}
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public bool DoMove()
		{
			if (speed > 0)
			{
				// We have the additional check so we can beter support enemies starting at the wrong spot
				if (enemy.Transform.position.x >= rightExtent)
				{
					speed *= -1;
				}
				else
				{
					float actualSpeed = speed * Mathf.Abs (Mathf.Cos (enemy.SlopeTargetRotation * Mathf.Deg2Rad));
					enemy.Translate(actualSpeed * TimeManager.FrameTime, 0, false);
					if (enemy.Transform.position.x > rightExtent)
					{
						// Should we set this directly or add a new method to enemy?
						enemy.Transform.position = new Vector3(rightExtent, enemy.Transform.position.y, enemy.Transform.position.z);
						speed *= -1;
					}
				}
			}
			else if (speed < 0)
			{
				if (enemy.Transform.position.x <= leftExtent)
				{
					speed *= -1;
				}
				else
				{
					float actualSpeed = speed * Mathf.Abs (Mathf.Cos (enemy.SlopeTargetRotation * Mathf.Deg2Rad));
					enemy.Translate(actualSpeed * TimeManager.FrameTime, 0, false);
					if (enemy.Transform.position.x < leftExtent)
					{
						// Should we set this directly or add a new method to enemy?
						enemy.Transform.position = new Vector3(leftExtent, enemy.Transform.position.y, enemy.Transform.position.z);
						speed *= -1;
					}
				}
			}
			if (firingTimer <= 0.0f)
			{
				DoShoot();
			}
			return false;
		}

		#endregion

		
#if UNITY_EDITOR

		/// <summary>
		/// Draw handles for showing extents.
		/// </summary>
		void OnDrawGizmos()
		{
			DrawGizmos();
		}

		/// <summary>
		/// Draw gizmos for showing extents.
		/// </summary>
		virtual public void DrawGizmos()
		{
			// These handles don't make sense once the game is playing as they would move
			if (!Application.isPlaying)
			{
				float left = 0.0f; float right = 0.0f;

				// TODO Better bounds finding, we should look for a hazard and use the colldier on that by default
				Collider2D collider2D = GetComponent<Collider2D>();
				if (collider2D is EdgeCollider2D)
				{
					for (int i = 0; i < ((EdgeCollider2D)collider2D).points.Length; i++)
					{
						if (((EdgeCollider2D)collider2D).points[i].x > right) right = ((EdgeCollider2D)collider2D).points[i].x;
						if (((EdgeCollider2D)collider2D).points[i].x < left) left = ((EdgeCollider2D)collider2D).points[i].x;
					}
				}
				else if (collider2D is BoxCollider2D)
				{
					right = ((BoxCollider2D)collider2D).size.x / 2.0f;
					left = ((BoxCollider2D)collider2D).size.x / -2.0f;
				}

				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position,  transform.position + new Vector3(rightOffset + right, 0, 0));
				Gizmos.DrawLine(transform.position + new Vector3(rightOffset + right, 0.25f, 0), transform.position + new Vector3(rightOffset + right, -0.25f, 0));
				Gizmos.DrawLine(transform.position,  transform.position + new Vector3(-leftOffset + left, 0, 0));
				Gizmos.DrawLine(transform.position + new Vector3(-leftOffset + left, 0.25f, 0),  transform.position + new Vector3(-leftOffset, -0.25f, 0));
			}
		}
#endif
	
	}
}
