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
	public class WallMovement_WallJump : WallMovement
	{
		
		#region members
		
		/// <summary>
		/// How we do a wall jump.
		/// </summary>
		public WallJumpControlType controlType;
		
		/// <summary>
		/// How high the character jumps on a wall jump. If 0 character will just fall from wall when jump is pressed.
		/// </summary>
		public float jumpHeight;
		
		/// <summary>
		/// How long can the character be away from the wall and still trigger a wall jump.
		/// </summary>
		public float controlLeeway;
		
		/// <summary>
		/// If the character is going slower than this (usually falling) then they will not be able to wall jump.
		/// </summary>
		public float minSpeedForWallJump;
		
		/// <summary>
		/// How fast does the chracter need to be falling before control reverts back to user.
		/// </summary>
		public float speedWhereWallJumpEnds;
		
		/// <summary>
		/// Gravity to apply if character touches wall.
		/// </summary>
		public float clingGravity;

		/// <summary>
		/// Max speed cling will move at. IF it goes faster than this it will slow down instead of speeding up.
		/// </summary>
		public float clingTargetSpeed;

		/// <summary>
		/// Derived initial velocity based on jumpHeight and relative gravity.
		/// </summary>
		protected float initialVelocity;
		
		/// <summary>
		/// Currently moving away from wall?
		/// </summary>
		protected bool movingAwayFromWall;
		
		/// <summary>
		/// Cached copy of the wall we jumped from
		/// </summary>
		protected Collider2D cachedWall;
		
		/// <summary>
		/// What was the wall direction for the last wall?
		/// </summary>
		protected int cachedWallDirection;
		
		/// <summary>
		/// If this is non-zero the character is able to press trigger a wall jump.
		/// </summary>
		protected float ableToWallJumpTimer;
		
		/// <summary>
		/// The highest right collider.
		/// </summary>
		protected BasicRaycast highestRightCollider;
		
		/// <summary>
		/// The highest left collider.
		/// </summary>
		protected BasicRaycast highestLeftCollider;
		
		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Wall Jump/Standard";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A wall movement that allows you to jump away from a wall with various controls, and will automatically slide down a while when you touch it. " +
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
		
		/// <summary>
		/// The index of the control type in movement data.
		/// </summary>
		protected const int ControlTypeIndex = 2;
		
		/// <summary>
		/// The index of the jump height in movement data.
		/// </summary>
		protected const int JumpHeightIndex = 3;
		
		/// <summary>
		/// The index of the control leeway in movement data.
		/// </summary>
		protected const int ControlLeewayIndex = 4;
		
		/// <summary>
		/// The index of the max speed for jump in movement data.
		/// </summary>
		protected const int MinSpeedForWallJumpIndex = 5;
		
		/// <summary>
		/// The index of the speed where wall jump ends.
		/// </summary>
		protected const int SpeedWhereWallJumpEndsIndex = 6;
		
		/// <summary>
		/// The index of the cling gravity.
		/// </summary>
		protected const int ClingGravityIndex = 7;

		/// <summary>
		/// The index of the cling target speed.
		/// </summary>
		protected const int ClingTargetSpeedIndex = 8;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 9;
		
		/// <summary>
		/// The default height of the jump.
		/// </summary>
		protected const float DefaultJumpHeight = 2.0f;
		
		/// <summary>
		/// The default away distance.
		/// </summary>
		protected const float DefaultControlLeeway = 0.2f;
		
		/// <summary>
		/// The default air speed.
		/// </summary>
		protected const float DefaultMaxSpeedForJump = -999f;
		
		/// <summary>
		/// The default air speed.
		/// </summary>
		protected const float DefaultSpeedWhereWallJumpEnds = -1f;
		
		/// <summary>
		/// The default cling gravity.
		/// </summary>
		protected const float DefaultClingGravity = -10.0f;

		/// <summary>
		/// The default cling target speed.
		/// </summary>
		protected const float DefaultClingTargetSpeed = -5.0f;

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
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			
			if (movingAwayFromWall || character.CurrentWall == null)
			{
				// Do air movement
				character.DefaultAirMovement.DoOverridenMove (true, true, (float) -cachedWallDirection, character.Input.RunButton);
				
				// Check for the end of the wall jump
				if (character.Grounded || character.Velocity.y <= speedWhereWallJumpEnds)
				{
					movingAwayFromWall = false;
				}
			}
			else
			{
				// Check for Jump
				if (character.Velocity.y > minSpeedForWallJump && CheckControls())
				{
					character.DefaultAirMovement.DoOverridenJump(jumpHeight, 1);
					movingAwayFromWall = true;
				}
				
				// Gravity
				if (character.Velocity.y > 0)
				{
					// Moving up - Apply normal gravity
					character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
				}
				else if (character.Velocity.y > clingTargetSpeed)
				{

					// Moving down - Apply reduced gravity
					character.AddVelocity(0, TimeManager.FrameTime * clingGravity, false);
					if (character.Velocity.y < clingTargetSpeed) character.SetVelocityY(clingTargetSpeed);
				}
				else if (character.Velocity.y < clingTargetSpeed)
				{
					// Moving too fast  - Apply 2 times -gravity 
					// TODO This could become a variable too
					character.AddVelocity(0, TimeManager.FrameTime * 2 * -clingGravity, false);
					if (character.Velocity.y > clingTargetSpeed) character.SetVelocityY(clingTargetSpeed);
				}

				// Translate
				character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
			}
		}
		
		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			this.character = character;
			initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * jumpHeight);
			highestRightCollider = character.Colliders.Where (c=>c.RaycastType == RaycastType.SIDE_RIGHT ).OrderByDescending(c=>c.WorldPosition.y).FirstOrDefault();
			highestLeftCollider  = character.Colliders.Where (c=>c.RaycastType == RaycastType.SIDE_LEFT ).OrderByDescending(c=>c.WorldPosition.y).FirstOrDefault();
			return this;
		}
		
		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			this.character = character;
			
			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				RequiredColliders = movementData[RequiredCollidersIndex].IntValue;
				controlType = (WallJumpControlType) movementData[ControlTypeIndex].IntValue;
				jumpHeight = movementData[JumpHeightIndex].FloatValue;
				controlLeeway = movementData[ControlLeewayIndex].FloatValue;
				minSpeedForWallJump = movementData[MinSpeedForWallJumpIndex].FloatValue;;
				clingGravity = movementData[ClingGravityIndex].FloatValue;
				speedWhereWallJumpEnds = movementData[SpeedWhereWallJumpEndsIndex].FloatValue;
				initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * jumpHeight);
				clingTargetSpeed = movementData[ClingTargetSpeedIndex].FloatValue;
				
			}
			else
			{
				Debug.LogError("Invalid movement data, not enough values.");
			}
			highestRightCollider = character.Colliders.Where (c=>c.RaycastType == RaycastType.SIDE_RIGHT ).OrderByDescending(c=>c.WorldPosition.y).FirstOrDefault();
			highestLeftCollider  = character.Colliders.Where (c=>c.RaycastType == RaycastType.SIDE_LEFT ).OrderByDescending(c=>c.WorldPosition.y).FirstOrDefault();
			return this;
		}
		
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
		/// Gets the priority for the animation state.
		/// </summary>
		override public int AnimationPriority
		{
			get 
			{
				if (movingAwayFromWall || character.CurrentWall == null) 
				{
					AnimationState ani = character.DefaultAirMovement.AnimationState;
					if (ani != AnimationState.FALL) return 0;
					return 0;
				}
				return 0;
			}
		}

		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// This overriden version always returns the wall direction.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (movingAwayFromWall) return -cachedWallDirection;
				if (character.CurrentWallCollider == -1)  return 0;
				return (int)character.Colliders[character.CurrentWallCollider].GetDirection().x;
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
			// Not moving away from wall and no wall
			if (character.CurrentWall == null) return false;
			// Or we don't have enough wall colliders
			if (character.CurrentWallColliderCount < RequiredColliders) return false;
			// Only wall cling if the highest collider is colliding
			if (character.Colliders[character.CurrentWallCollider] == highestLeftCollider || character.Colliders[character.CurrentWallCollider] == highestRightCollider) 
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
				return true;
			}

			return false;
		}
		
		// <summary>
		/// Does the cling.
		/// </summary>
		override public void DoCling()
		{
//			if (movingAwayFromWall) return;
//
//			cachedWall = character.CurrentWall;
//			cachedWallDirection = (int) character.Colliders[character.CurrentWallCollider].GetDirection().x;
//			// Make sure we clear any residual velocity
//			character.SetVelocityX(0);

			// NOTE: Commented out as it relies on a certain type of wall setup
			// Because we are moving and changing velocity all the time the colliders don't guarantee that we snap exactly to the wall
			// So here we calculate the absolute wall position with a new raycast
			//			Vector2 initialPosition = character.Colliders [character.CurrentWallCollider].WorldPosition - new Vector2 (character.LastFacedDirection * 0.1f, 0);
			//			Vector2 dir = (Vector2)character.CurrentWall.transform.position - character.Colliders [character.CurrentWallCollider].WorldPosition;
			//			RaycastHit2D hit = Physics2D.Raycast (initialPosition, dir, 5.0f, 1 << character.CurrentWall.gameObject.layer);
			//			character.Translate (hit.point.x - character.Colliders [character.CurrentWallCollider].WorldExtent.x, 0, false);

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
			    character.Velocity.y < 0)
				return true;
			return false;
		}
		
		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			ableToWallJumpTimer = 0.0f;
			// We call this to esnure jump count and the like gets reset
			character.DefaultAirMovement.LosingControl();
		}
		
		#endregion
		
		#region public properties
		
		/// <summary>
		/// Gravity handled internally
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				return false;
			}
		}
		
		#endregion
		
		#region protected methods
		/// <summary>
		/// Checks if correc tinput for a wall jump is being entered.
		/// </summary>
		/// <returns><c>true</c>, if controls was checked, <c>false</c> otherwise.</returns>
		virtual protected bool CheckControls()
		{
			switch (controlType)
			{
			case WallJumpControlType.JUMP_ONLY:
				if (character.Input.JumpButton == ButtonState.DOWN) return true;
				break;
			case WallJumpControlType.DIRECTION_ONLY:
				if (character.Input.HorizontalAxisDigital == -cachedWallDirection) return true;
				break;
			}
			return false;
		}
		
		#endregion
		#if UNITY_EDITOR
		
		#region draw inspector
		
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length != MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}
			
			// Control Type
			if (movementData[ControlTypeIndex] == null) movementData[ControlTypeIndex] = new MovementVariable();
			movementData[ControlTypeIndex].IntValue = (int) (WallJumpControlType) EditorGUILayout.EnumPopup(new GUIContent("Control Type", "What control does the user need to input to wall jump."), (WallJumpControlType) movementData[ControlTypeIndex].IntValue);
			
			// JumpHeightIndex
			if (movementData[JumpHeightIndex] == null) movementData[JumpHeightIndex] = new MovementVariable(DefaultJumpHeight);
			movementData[JumpHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Jump Height", "How high the character jumps away from the wall. Use zero to have jump simply detach from the wall."), movementData[JumpHeightIndex].FloatValue);
			if (movementData[JumpHeightIndex].FloatValue < 0) movementData[JumpHeightIndex].FloatValue = 0;
			
			// Cling Gravity 
			if (movementData[ClingGravityIndex] == null) movementData[ClingGravityIndex] = new MovementVariable(DefaultClingGravity);
			movementData[ClingGravityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Cling Gravity", "Gravity to apply whn character is touching a wall (negative number). Use 0 to disable slide."), movementData[ClingGravityIndex].FloatValue);
			if (movementData[ClingGravityIndex].FloatValue > 0) movementData[ClingGravityIndex].FloatValue = 0;

			// Cling Gravity 
			if (movementData[ClingTargetSpeedIndex] == null) movementData[ClingTargetSpeedIndex] = new MovementVariable(DefaultClingTargetSpeed);
			movementData[ClingTargetSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Target Speed", "Desired max speed for character. If going faster than this character will slow down instead of speeding up."), movementData[ClingTargetSpeedIndex].FloatValue);
			if (movementData[ClingTargetSpeedIndex].FloatValue > 0) movementData[ClingTargetSpeedIndex].FloatValue = 0;

			showDetails = EditorGUILayout.Foldout(showDetails, "Details");
			if (showDetails)
			{
				// Required Colliders
				if (movementData[RequiredCollidersIndex] == null) movementData[RequiredCollidersIndex] = new MovementVariable();
				movementData[RequiredCollidersIndex].IntValue = EditorGUILayout.IntField(new GUIContent("Required Colliders", "The number of colliders required to initiate wall cling."), movementData[RequiredCollidersIndex].IntValue);
				if (movementData[RequiredCollidersIndex].IntValue < 1) movementData[RequiredCollidersIndex].IntValue = 2;
				
				// MinSpeedForWallJump
				if (movementData[MinSpeedForWallJumpIndex] == null) movementData[MinSpeedForWallJumpIndex] = new MovementVariable(DefaultMaxSpeedForJump);
				movementData[MinSpeedForWallJumpIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Min Speed for Jump", "If the character is going slower than this (usually falling) then they will not be able to wall jump."), movementData[MinSpeedForWallJumpIndex].FloatValue);
				if (movementData[MinSpeedForWallJumpIndex].FloatValue > 0) movementData[MinSpeedForWallJumpIndex].FloatValue = 0;
				
				// ControlLeeway
				if (movementData[ControlLeewayIndex] == null) movementData[ControlLeewayIndex] = new MovementVariable(DefaultControlLeeway);
				movementData[ControlLeewayIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Control Leeway", "How long can the character be away from the wall and still trigger a wall jump."), movementData[ControlLeewayIndex].FloatValue);
				if (movementData[ControlLeewayIndex].FloatValue < 0) movementData[ControlLeewayIndex].FloatValue = 0;
				
				// Speed where jump ends
				if (movementData[SpeedWhereWallJumpEndsIndex] == null) movementData[SpeedWhereWallJumpEndsIndex] = new MovementVariable(DefaultSpeedWhereWallJumpEnds);
				movementData[SpeedWhereWallJumpEndsIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Speed where Jump ends", "How fast does the character need to be falling before the wall jump finishes and control reverts back to player?"), movementData[SpeedWhereWallJumpEndsIndex].FloatValue);
				if (movementData[SpeedWhereWallJumpEndsIndex].FloatValue > 0) movementData[SpeedWhereWallJumpEndsIndex].FloatValue = 0;
				
			}
			
			return movementData;
		}
		
		#endregion
		
		#endif
		
	}
	
	/// <summary>
	/// Different controls for wall jumping.
	/// </summary>
	public enum WallJumpControlType
	{
		JUMP_ONLY,
		DIRECTION_ONLY
	}
	
}

