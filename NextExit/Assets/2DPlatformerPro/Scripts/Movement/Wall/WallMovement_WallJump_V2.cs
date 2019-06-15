#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PlatformerPro
{
	/// <summary>
	/// A wall movement class that lets you jump away from a wall and will automatically slide down a wall if you touch it.
	/// </summary>
	public class WallMovement_WallJump_V2: WallMovement_WallJump
	{
		
	
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Wall Jump/Standard (Require Hold)";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A wall movement that allows you to jump away from a wall with various controls, and requires you to hold towards wlal to initiate the cling. " +
			"When you jump away you can only move in the opposite direction of wall until you start falling.";
		
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
		
		#region Unity hooks
		
		/// <summary>
		/// Unity Update() hook
		/// </summary>
		void Update()
		{
			if (ableToWallJumpTimer > 0.0f) ableToWallJumpTimer -= TimeManager.FrameTime;
		}
		
		#endregion
		
		#region public methods

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (movingAwayFromWall || character.CurrentWall == null) 
				{
					AnimationState ani = character.DefaultAirMovement.AnimationState;
					if (ani != AnimationState.FALL) return AnimationState.WALL_JUMP;
					return AnimationState.FALL;
				}
				return AnimationState.WALL_SLIDE;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate a wall clinging behaviour.
		/// </summary>
		override public bool WantsCling()
		{
			// Cant wall climb if grounded unless climbing up
			if (character.Grounded) return false;
			// Moving away from wall
			if (movingAwayFromWall) return true;
			// Allow some leeway for the jump control.
			if (ableToWallJumpTimer > 0) return true;
			// Not moving away from wall and no wall
			if (character.CurrentWall == null) return false;
			// Or we don't have enough wall colliders
			if (character.CurrentWallColliderCount < RequiredColliders) return false;
			// Only wall cling if the highest collider is colliding
			if ((character.Colliders[character.CurrentWallCollider] == highestLeftCollider || character.Colliders[character.CurrentWallCollider] == highestRightCollider)
			    && (character.Input.HorizontalAxisDigital == character.Colliders[character.CurrentWallCollider].GetDirection().x || ableToWallJumpTimer > 0)) 
			{
				// If we are now hittin an opposite direction we are no longer moving away from wall
				if (movingAwayFromWall && cachedWallDirection != (int) character.Colliders[character.CurrentWallCollider].GetDirection().x)
				{
					movingAwayFromWall = false;
				}
				cachedWall = character.CurrentWall;
				cachedWallDirection = (int) character.Colliders[character.CurrentWallCollider].GetDirection().x;
				// Make sure we clear any residual velocity
				character.SetVelocityX(0);
				// Reset able to wall jump timer
				return true;
			}

			return false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			ableToWallJumpTimer = controlLeeway;
			base.DoMove ();
		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't. In this case control unless the character dismounts.
		/// </summary>
		override public bool WantsControl()
		{
			if (!movingAwayFromWall && 
			    !character.Grounded && 
			    cachedWall == character.CurrentWall &&
			    character.CurrentWall != null && 
			    (character.Input.HorizontalAxisDigital == character.Colliders[character.CurrentWallCollider].GetDirection().x || ableToWallJumpTimer > 0) &&
			    character.Velocity.y < 0)
				return true;
			return false;
		}

		
		#endregion


#if UNITY_EDITOR
		
		#region draw inspector
		
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			return WallMovement_WallJump.DrawInspector(movementData, ref showDetails, target);
		}
		
		#endregion
		
#endif
		
	}
}

