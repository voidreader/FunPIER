using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// An enemy movement thatapplies a force on damage or death.
	/// </summary>
	public class EnemyMovement_Damaged_Knockback: EnemyDeathMovement
	{
		#region members
		/// <summary>
		/// How much force to apply.
		/// </summary>
		[Tooltip("How much force to apply.")]
		public Vector2 forceMultiplier;
		
		/// <summary>
		/// Drag to apply in X.
		/// </summary>
		[Tooltip("Drag to apply in X.")]
		public float drag;
		
		/// <summary>
		/// If true just apply force in direction of x, ignoring extent of x damage direction.
		/// </summary>
		[Tooltip("If true just apply force in direction of x, ignoring extent of x damage direction.")]
		public bool fixedForceInX;

		/// <summary>
		/// If true just apply y force directly, ignoring y extent and direction form damage info.
		/// </summary>
		[Tooltip("If true just apply y force directly, ignoring y extent and direction from damage info.")]
		public bool fixedForceInY;

		/// <summary>
		/// If true lose control when hitting the ground.
		/// </summary>
		[Tooltip("If true lose control when hitting the ground.")]
		public bool cancelOnGrounded;

		[Header("Death")]
		/// <summary>
		/// On death the GameObject will be destroyed after this many seconds.
		/// </summary>
		[Tooltip("On death the GameObject will be destroyed after this many seconds. Use 0 if you do not wish to destory the enemy.")]
		public float destroyDelay;

		/// <summary>
		/// The explosion force.
		/// </summary>
		protected Vector2 explosionForce;

		/// <summary>
		/// Track if this is a death movement
		/// </summary>
		protected bool isDeath;

		/// <summary>
		/// Have we left the ground?
		/// </summary>
		protected bool hasLeftGround;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Knockback - Explosive Force";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that applies a force on damage.";
		
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
				if (enemy.State == EnemyState.DEAD) return AnimationState.DEATH;
				return AnimationState.HURT_NORMAL;
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
			if (cancelOnGrounded && !enemy.characterCanFall) Debug.LogWarning ("Cancel on grounded is true but enemy cannot fall, this probably wont work.");
			return this;
		}
		
		/// <summary>
		/// Moves the enemy.
		/// </summary>
		override public bool DoMove()
		{
			// X move
			
			// Apply drag (we use a friction like equation which seems to look better than a drag like one)
			if (enemy.Velocity.x > 0) 
			{
				enemy.AddVelocity(-enemy.Velocity.x * drag * TimeManager.FrameTime, 0);
				if (enemy.Velocity.x < 0) enemy.SetVelocityX(0);
			}
			else if (enemy.Velocity.x < 0) 
			{
				enemy.AddVelocity(-enemy.Velocity.x * drag * TimeManager.FrameTime, 0);
				if (enemy.Velocity.x > 0) enemy.SetVelocityX(0);
			}
			
			// Translate
			enemy.Translate(enemy.Velocity.x * TimeManager.FrameTime, 0, true);

			// Y Move
			
			// Apply gravity
			if (!enemy.Grounded )
			{
				enemy.AddVelocity(0, TimeManager.FrameTime * enemy.Gravity);
				// Indicate we left the ground
				if (cancelOnGrounded && enemy.characterCanFall) hasLeftGround = true;
			}
			// Check for cancel
			else if (cancelOnGrounded && enemy.characterCanFall && hasLeftGround)
			{
				enemy.MakeVulnerable();
				return false;
			}


			// Translate
			enemy.Translate(0, enemy.Velocity.y * TimeManager.FrameTime, true);

			// Apply side colliders
			if (enemy.detectSideCollisions) enemy.ApplySideCollisions ();

			return true;
		}

		/// <summary>
		/// Do the damaged movement
		/// </summary>
		override public void DoDamage(DamageInfo info)
		{
			SetUpExplosionForce (info);
			this.isDeath = false;
		}

		
		/// <summary>
		/// Do the death movement
		/// </summary>
		override public void DoDeath(DamageInfo info)
		{
			SetUpExplosionForce (info);
			this.isDeath = true;
			if (destroyDelay > 0) StartCoroutine(DestroyAfterDelay());
		}
		
		#endregion

		protected void SetUpExplosionForce(DamageInfo info)
		{
			hasLeftGround = false;
			// explosionTimer = explosionTime;
			float x = 0;
			float y = 0;
			if (fixedForceInX)
			{
				x = info.Direction.x > 0 ? -forceMultiplier.x : forceMultiplier.x;
			}
			else
			{
				x = info.Direction.x * -forceMultiplier.x;
			}
			if (fixedForceInY)
			{
				y = forceMultiplier.y;
			}
			else
			{
				y = info.Direction.y * -forceMultiplier.y;
			}
			enemy.SetVelocityX(x);
			enemy.SetVelocityY(y);
		}

		/// <summary>
		/// Wait a while then destroy the enemy.
		/// </summary>
		protected IEnumerator DestroyAfterDelay()
		{
			yield return new WaitForSeconds (destroyDelay);
			Destroy (gameObject);
		}
	}
	
}