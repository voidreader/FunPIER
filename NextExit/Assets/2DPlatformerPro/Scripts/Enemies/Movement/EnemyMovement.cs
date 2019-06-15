/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// This is the basic behaviour from which all enemy movement behaviours are derived. To create your 
	/// own enemy movement extend this class.
	/// </summary>
	public abstract class EnemyMovement : MonoBehaviour
	{
		#region members
		
		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		protected Enemy enemy;

		#endregion
		
		#region properties
		

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		virtual public AnimationState AnimationState
		{
			get 
			{
				return AnimationState.IDLE;
			}
		}
		
		/// <summary>
		/// Gets the animation override state that this movement wants to set.
		/// </summary>
		virtual public string OverrideState
		{
			get 
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		virtual public int FacingDirection
		{
			get 
			{
				return 0;
			}
		}

		/// <summary>
		/// If we are grounded shoudl we snap to the ground. Helps us handle slopes.
		/// </summary>
		/// <value><c>true</c> if should snap to ground; otherwise, <c>false</c>.</value>
		virtual public bool ShouldSnapToGround
		{
			get 
			{
				return true;
			}
		}

		#endregion
		
		#region movement info constants and properties

		// TODO These hooks are here to drive a custom editor interface later on

		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "EnemyMovement";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "The base enemy movement class, you shound't be seeing this did you forget to override MovementInfo?";
		
		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		public static MovementInfo Info
		{
			get
			{
				return new MovementInfo(Name, Description);
			}
		}
		
		#endregion

		#region public methods
		
		/// <summary>
		/// Initialise this movement and return a reference to the ready to use movement.
		/// </summary>
		virtual public EnemyMovement Init(Enemy enemy)
		{
			this.enemy = enemy;

			return this;
		}

		/// <summary>
		/// Does the movement.
		/// </summary>
		/// <returns><c>true</c>, if this movement can handle the current movement state, <c>false</c> otherwise.</returns>
		virtual public bool DoMove()
		{
			return false;
		}

		/// <summary>
		/// Do the damage movement
		/// </summary>
		virtual public void DoDamage(DamageInfo info)
		{
			
		}

		/// <summary>
		/// Do the death movement
		/// </summary>
		virtual public void DoDeath(DamageInfo info)
		{

		}

		/// <summary>
		/// Called when this movement is gaining control.
		/// </summary>
		virtual public void GainingControl()
		{

		}

		/// <summary>
		/// Called when this movement is losing control.
		/// </summary>
		/// <returns><c>true</c>, if a final animation is being played and control should not revert <c>false</c> otherwise.</returns>
		virtual public bool LosingControl()
		{
			return false;
		}

		/// <summary>
		/// Often a movement will need some kind of direction information such as where the cahracter is in relation to the enemy.
		/// Use this to set that information. Note there is no specific rule for what that information is, it could be anything.
		/// </summary>
		/// <param name="direction">Direction.</param>
		virtual public void SetDirection(Vector2 direction)
		{

		}

		/// <summary>
		/// Called when the enemy hits the character.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="info">Damage info.</param>
		virtual public void HitCharacter(Character character, DamageInfo info)
		{

		}

		/// <summary>
		/// Called by the enemy to switch (x) direction of the movement. Note that not all 
		/// movements need to support this, they may do nothing.
		/// </summary>
		virtual public void SwitchDirection()
		{
			
		}

		#endregion

	}
	
}