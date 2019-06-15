using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy movement for simple melee attack.
	/// </summary>
	public class EnemyMovement_MeleeAttack : EnemyMovement, ICompletableMovement
	{

		/// <summary>
		/// The attack data.
		/// </summary>
		public BasicAttackData attack;

		/// <summary>
		/// The hazard / hit box.
		/// </summary>
		public EnemyHitBox hitBox;

		/// <summary>
		/// Time between attacks
		/// </summary>
		public float coolDown;

		/// <summary>
		/// The timer for the current attack.
		/// </summary>
		protected float currentAttackTimer;

		/// <summary>
		/// Are we in the middle of an attack?
		/// </summary>
		protected bool attacking;

		/// <summary>
		/// Has attack started?
		/// </summary>
		protected bool startedAttacking;

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Melee Attack";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that does a simple melee attack.";
		
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


		/// <summary>
		/// Init this instance.
		/// </summary>
		override public EnemyMovement Init(Enemy enemy)
		{
			base.Init (enemy);
			hitBox.damageType = attack.damageType;
			hitBox.damageAmount = attack.damageAmount;
			attacking = false;
			return this;
		}
		
		/// <summary>
		/// Do whichever attack is available.
		/// </summary>
		virtual public void Attack()
		{
			StopAllCoroutines();
			attacking = false;
			currentAttackTimer = 0.0f;
			StartCoroutine(DoAttack());
		}
			
		/// <summary>
		/// Does the attack.
		/// </summary>
		/// <returns>The attack.</returns>
		/// <param name="attackIndex">Attack index.</param>
		virtual protected IEnumerator DoAttack()
		{
			startedAttacking = true;
			attacking = true;
			hitBox.Enable(attack.attackTime * attack.attackHitBoxStart, attack.attackTime * attack.attackHitBoxEnd);
			while(currentAttackTimer < attack.attackTime && enemy.State != EnemyState.DAMAGED && enemy.State != EnemyState.DEAD)
			{
				currentAttackTimer += TimeManager.FrameTime;
				yield return true;
			}
			// Attack finished
			hitBox.ForceStop();
			attacking = false;
			enemy.AttackFinished();
			// Cool down
			while(currentAttackTimer < attack.attackTime + coolDown)
			{
				currentAttackTimer += TimeManager.FrameTime;
				yield return true;
			}
			startedAttacking = false;
			enemy.MovementComplete();
		}
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		/// <value>The state of the animation.</value>
		override public AnimationState AnimationState
		{
			get 
			{
				if (attacking) return attack.animation;
				return AnimationState.IDLE;
			}
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public bool DoMove()
		{
			if (!startedAttacking) 
			{
				Attack ();
				return true;
			}
			else if (attacking) return true;
			return false;
		}

		/// <summary>
		/// Called when this movement is losing control.
		/// </summary>
		/// <returns>true if we are still attacking as we don't want to break a melee attack mid movement</returns>
		/// <c>false</c>
		override public bool LosingControl()
		{
			if (attacking) return true;
			return false;
		}
	}
}