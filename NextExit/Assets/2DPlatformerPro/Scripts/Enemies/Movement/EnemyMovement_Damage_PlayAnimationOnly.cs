using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy death movement which just plays an animation which is mapped to enemy state.
	/// </summary>
	public class EnemyMovement_Damage_PlayAnimationOnly : EnemyDeathMovement
	{

		#region members

		/// <summary>
		/// Which animation state to play when damaged?
		/// </summary>
		public AnimationState damagedState;

		/// <summary>
		/// Which animation state to play when dead?
		/// </summary>
		public AnimationState deathState;

		/// <summary>
		/// On death the GameObject will be destroyed after this many seconds.
		/// </summary>
		public float destroyDelay;

		/// <summary>
		/// Track if this is a death movement
		/// </summary>
		protected bool isDeath;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Play Animation Only";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy death/damage movement that simply plays an animation.";
		
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
				if (isDeath) return deathState;
				return damagedState;
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
			return this;
		}
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public bool DoMove()
		{
			return true;
		}
		/// <summary>
		/// Do the damaged movement
		/// </summary>
		override public void DoDamage(DamageInfo info)
		{
			isDeath = false;
		}

		/// <summary>
		/// Do the death movement
		/// </summary>
		override public void DoDeath(DamageInfo info)
		{
			isDeath = true;
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
