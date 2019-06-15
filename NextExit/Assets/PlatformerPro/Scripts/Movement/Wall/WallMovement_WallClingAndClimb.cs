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
	/// A wall movment class that can stick to, climb and slide down walls. The character can also jump from the wall.
	/// </summary>
	public class WallMovement_WallClingAndClimb : WallMovement_WallCling
	{

		#region members

		/// <summary>
		/// How fast character climbs when the player presses up.
		/// </summary>
		public float climbUpSpeed;

		/// <summary>
		/// How quickly we come to a halt when the user is not pressing down.
		/// </summary>
		public float clingDrag;

		/// <summary>
		/// Maximum speed we can slide down (negative).
		/// </summary>
		public float slideDownSpeed;

		/// <summary>
		/// Tracks if we are climbing.
		/// </summary>
		protected bool isClimbing;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Wall Jump/Cling and Climb";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Wall movement which allows the character to stick to and climb up certain walls.";
		
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
		/// The index of the climb up speed in movement data.
		/// </summary>
		protected const int ClimbUpSpeedIndex = 8;

		/// <summary>
		/// The index of the side down  speed in movement data.
		/// </summary>
		protected const int SlideDownSpeedIndex = 9;

		/// <summary>
		/// The index of the cling drad  acceleration in movement data.
		/// </summary>
		protected const int ClingDragIndex = 10;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 11;

		/// <summary>
		/// The default climb up speed.
		/// </summary>
		protected const float DefaultClimbUpSpeed = 2.0f;

		/// <summary>
		/// The default slide down speed.
		/// </summary>
		protected const float DefaultSlideDownSpeed = -5.0f;

		/// <summary>
		/// The default cling drag.
		/// </summary>
		protected const float DefaultClingDrag = 15.0f;

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity Update() hook
		/// </summary>
		void Update()
		{
			if (ignoreWallClimbTimer > 0.0f) ignoreWallClimbTimer -= TimeManager.FrameTime;
			if (ignoreJumpTimer > 0.0f) ignoreJumpTimer -= TimeManager.FrameTime;
			if (ableToacceptInputTimer > 0.0f) ableToacceptInputTimer -= TimeManager.FrameTime;
		}

		#endregion

		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			isClimbing = false;

			if (ableToacceptInputTimer <= 0 && currentWallDirection != 0 &&
			    ((mustHoldToMaintainCling && character.Input.HorizontalAxisDigital != currentWallDirection) ||
				character.Input.HorizontalAxisDigital == -currentWallDirection))
			{
				ableToacceptInputTimer = ableToWallJumpTime;
				clingStarted = false;
			}
		
			// If we have stopped sliding but are still able to jump
			if (!clingStarted && ableToacceptInputTimer > 0.0f)
			{
				// Allow jump
				if (ignoreJumpTimer <= 0.0f && (character.Input.JumpButton == ButtonState.DOWN || character.Input.JumpButton == ButtonState.HELD )) 
				{
					character.DefaultAirMovement.DoOverridenJump(jumpHeight, 1);
					ignoreWallClimbTimer = ignoreClingAfterJumpTime;
					hasJumped = true;
				}
				// But if jump isn't happening fall through to default air
				else
				{
					character.DefaultAirMovement.DoMove();
				}
			}
			else
			{
				// Jump
				if (ignoreJumpTimer <= 0.0f && (character.Input.JumpButton == ButtonState.DOWN || character.Input.JumpButton == ButtonState.HELD )) 
				{
					character.DefaultAirMovement.DoOverridenJump(jumpHeight, 1);
					ignoreWallClimbTimer = ignoreClingAfterJumpTime;
					hasJumped = true;
				}
				else 
				{
					// Check for up key and climb
					if (character.Input.VerticalAxisDigital == 1)
					{
						if (character.Velocity.y < 0)
						{
							// Come to a stop
							character.AddVelocity(0, TimeManager.FrameTime * clingDrag, false);
							if (character.Velocity.y > 0)character.SetVelocityY (0);
						}
						else
						{
							character.SetVelocityY (climbUpSpeed);
							isClimbing = true;
						}
					} 
					// Check for down key and slide
					else if (character.Input.VerticalAxisDigital == -1)
					{
						// Slide down
						character.AddVelocity(0, TimeManager.FrameTime * clingGravity, false);
						if (character.Velocity.y < slideDownSpeed) character.SetVelocityY (slideDownSpeed);
					}
					else
					{
						character.AddVelocity(0, TimeManager.FrameTime * clingDrag, false);
						if (character.Velocity.y > 0)character.SetVelocityY (0);
					}

				}

				// Translate
				character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
			}
		}

		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);

			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				RequiredColliders = movementData[RequiredCollidersIndex].IntValue;
				clingGravity = movementData[ClingGravityIndex].FloatValue;
				gravityDelay = 1.0f; ignoreGravityTimer = 1.0f;
				jumpHeight = movementData[JumpHeightIndex].FloatValue;
				ignoreClingAfterJumpTime = movementData[IgnoreClingAfterJumpTimeIndex].FloatValue;
				mustHoldToMaintainCling = false;
				ignoreJumpTime = movementData[IgnoreJumpTimeIndex].FloatValue;
				climbUpSpeed = movementData [ClimbUpSpeedIndex].FloatValue;
				clingDrag = movementData [ClingDragIndex].FloatValue;
				slideDownSpeed = movementData [SlideDownSpeedIndex].FloatValue;
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
				if (hasJumped) return AnimationState.WALL_JUMP;
				if (isClimbing) return AnimationState.WALL_CLIMB_UP;
				if (character.Velocity.x == 0) return AnimationState.WALL_CLING;
				return AnimationState.WALL_SLIDE;
			}
		}

		/// <summary>
		/// Gets the priority of the animation state that this movement wants to set.
		/// </summary>
		/// <value>The animation priority.</value>
		override public int AnimationPriority
		{
			get 
			{
				if (hasJumped) return 5;
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

			// Not moving away from wall and no wall
			if (character.CurrentWall == null) return false;

			// Or we don't have enough wall colliders
			if (character.CurrentWallColliderCount < RequiredColliders) return false;

			if (ignoreWallClimbTimer > 0.0f) return false;

			// Hold towards wall to cling
			if (character.CurrentWall != null && ((int)character.Colliders[character.CurrentWallCollider].GetDirection().x) == character.Input.HorizontalAxisDigital) 
			{
				// Only wall cling if the highest collider is colliding
				if (character.Colliders[character.CurrentWallCollider] == highestLeftCollider || character.Colliders[character.CurrentWallCollider] == highestRightCollider) return true;
			}
			return false;
		}

		// <summary>
		/// Does the cling.
		/// </summary>
		override public void DoCling()
		{
			base.DoCling ();
			ignoreGravityTimer = 1.0f;
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			base.LosingControl ();
			currentWallDirection = 0;
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

#if UNITY_EDITOR

		#region draw inspector

		/// <summary>
		/// Draws the inspector.
		/// </summary>
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length != MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Climb speed
			if (movementData[ClimbUpSpeedIndex] == null) movementData[ClimbUpSpeedIndex] = new MovementVariable(DefaultClimbUpSpeed);
			movementData[ClimbUpSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Climb Up Speed", "How fast the character climbs up the wall when the user presses up."), movementData[ClimbUpSpeedIndex].FloatValue);
			if (movementData[ClimbUpSpeedIndex].FloatValue < 0) movementData[ClimbUpSpeedIndex].FloatValue = 0;

			// Climb speed
			if (movementData[SlideDownSpeedIndex] == null) movementData[SlideDownSpeedIndex] = new MovementVariable(DefaultSlideDownSpeed);
			movementData[SlideDownSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Slide Down Speed", "How fast the character slides down the wall (negative)."), movementData[SlideDownSpeedIndex].FloatValue);
			if (movementData[SlideDownSpeedIndex].FloatValue > 0) movementData[SlideDownSpeedIndex].FloatValue = 0;

			// Cling drag
			if (movementData[ClingDragIndex] == null) movementData[ClingDragIndex] = new MovementVariable(DefaultClingDrag);
			movementData[ClingDragIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Cling Drag", "How quickly we come to a halt when the user is not pressing down."), movementData[ClingDragIndex].FloatValue);
			if (movementData[ClingDragIndex].FloatValue < 0) movementData[ClingDragIndex].FloatValue = 0;

			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = WallMovement_WallCling.DrawInspectorWithOptions (movementData, ref showDetails, target, false, false);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			return movementData;
		}

		#endregion

#endif
	}

}

