using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// An enemy movement that bobbles on damage or death.
	/// </summary>
	public class EnemyMovement_Damaged_Bobble: EnemyDeathMovement
	{
		#region members

		/// <summary>
		/// The character bobbles on death, how high should it bobble.
		/// </summary>
		public float bobbleHeight;

		/// <summary>
		/// On death the GameObject will be destroyed after this many seconds.
		/// </summary>
		public float destroyDelay;

		/// <summary>
		/// The gravity to apply when bobbling
		/// </summary>

		public float gravity = -35;

		/// <summary>
		/// Stores velocity that will be applied on death.
		/// </summary>
		protected float initialBobbleVelocity;

		/// <summary>
		/// The enemies starting position (pre-bobble).
		/// </summary>
		protected float originalHeight;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Bobble (Damage and Death Only)";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that bobbles on damage or death and keeps falling on death.";
		
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

			// Calculate initial velocity - TODO Custom gravity
			initialBobbleVelocity = Mathf.Sqrt(-2.0f * gravity * bobbleHeight);
			
			return this;
		}
		
		/// <summary>
		/// Moves the enemy.
		/// </summary>
		override public bool DoMove()
		{
			// TODO custom gravities
			enemy.AddVelocity(0, TimeManager.FrameTime * gravity);
			enemy.Translate(0, enemy.Velocity.y * TimeManager.FrameTime, true);
			// Make sure we don't travel too far if we aren't dying
			if (enemy.State != EnemyState.DEAD && enemy.Transform.position.y <= originalHeight) 
			{
				enemy.Translate(0, originalHeight - enemy.Transform.position.y, true);
				// Give control back
				return false;
			}
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
		/// Do the damaged movement
		/// </summary>
		override public void DoDamage(DamageInfo info)
		{
			originalHeight = enemy.Transform.position.y;
			// Bobble
			enemy.SetVelocityY(initialBobbleVelocity);
		}

		
		/// <summary>
		/// Do the death movement
		/// </summary>
		override public void DoDeath(DamageInfo info)
		{
			// Bobble
			enemy.SetVelocityY(initialBobbleVelocity);
			enemy.characterCanFall = false;
			if (destroyDelay > 0) StartCoroutine(DestroyAfterDelay());
		}
		
		#endregion

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