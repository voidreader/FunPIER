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
	public class WallMovement_WallClingAndRun : WallMovement_WallCling
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
		/// How long we can run for.
		/// </summary>
		public float runTimeLimit;

		/// <summary>
		/// How long we can look in opposite direction for.
		/// </summary>
		public float oppositeLookTimeLimit;

		/// <summary>
		/// When doing a jump away from the wall at what speed do we loose control.
		/// </summary>
		public float speedWhereWallJumpEnds;

		/// <summary>
		/// Tracks if we are climbing.
		/// </summary>
		protected bool isRunning;

		/// <summary>
		/// How long have we been running up the wall.
		/// </summary>
		protected float wallRunTimer;

		/// <summary>
		/// Have we started opposite looking.
		/// </summary>
		protected bool oppositeLookStarted;

		/// <summary>
		/// The opposite look timer. We can look as long as this is higher than zero.
		/// </summary>
		protected float oppositeLookTimer;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Wall Jump/Cling and Run";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Wall movement which allows the character to stick to and run up certain walls until their inertia runs out. The player can also hold away from wall to pause (cling) and look in opposite direction.";
		
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
		protected const int RunSpeedIndex = 8;

		/// <summary>
		/// The index of the side down  speed in movement data.
		/// </summary>
		protected const int SlideDownSpeedIndex = 9;

		/// <summary>
		/// The index of the cling drad  acceleration in movement data.
		/// </summary>
		protected const int ClingDragIndex = 10;

		/// <summary>
		/// The index of the runtime limit in movement data.
		/// </summary>
		protected const int RunTimeLimitIndex = 11;

		/// <summary>
		/// The index of the opposite look time limit in movement data.
		/// </summary>
		protected const int OppositeLookTimeIndex = 12;

		/// <summary>
		/// When we do wall jump at what y speed do we go back to normal jump.
		/// </summary>
		protected const int SpeedWhereWallJumpEndsIndex = 13;

		/// <summary>
		/// Cached copy of the wall we jumped from
		/// </summary>
		protected Collider2D cachedWall;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 14;

		/// <summary>
		/// The default climb up speed.
		/// </summary>
		protected const float DefaultRunSpeed = 2.0f;

		/// <summary>
		/// The default slide down speed.
		/// </summary>
		protected const float DefaultSlideDownSpeed = -5.0f;

		/// <summary>
		/// The default cling drag.
		/// </summary>
		protected const float DefaultClingDrag = 15.0f;

		/// <summary>
		/// The default air speed at which jump ends.
		/// </summary>
		protected const float DefaultSpeedWhereWallJumpEnds = -1f;

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity Update() hook
		/// </summary>
		void Update()
		{
			if (ignoreJumpTimer > 0.0f) ignoreJumpTimer -= TimeManager.FrameTime;
			if (ableToacceptInputTimer > 0.0f) ableToacceptInputTimer -= TimeManager.FrameTime;
			if (ignoreGravityTimer > 0.0f) ignoreGravityTimer -= TimeManager.FrameTime;
			if (wallRunTimer > 0.0f) wallRunTimer -= TimeManager.FrameTime;
			if (oppositeLookTimer > 0.0f) oppositeLookTimer -= TimeManager.FrameTime;
		}

		#endregion

		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			isRunning = false;

			if (clingStarted) ableToacceptInputTimer = ableToWallJumpTime;
				
			if (hasJumped)
			{
				if (oppositeLookStarted)
				{
					// Do air movement with overriden x
					character.DefaultAirMovement.DoOverridenMove (true, true, (float)-currentWallDirection, character.Input.RunButton);
				} else
				{
					// Do air movement no X
					character.DefaultAirMovement.DoOverridenMove (true, true, 0.0f, ButtonState.NONE);
				}
				return;
			}

			// Start Opposite Look
			if (!oppositeLookStarted && currentWallDirection != 0 && character.Input.HorizontalAxisDigital == -currentWallDirection)
			{
				oppositeLookStarted = true;
				oppositeLookTimer = oppositeLookTimeLimit;
				clingStarted = false;
			}

			// Opposite Look
			if (oppositeLookStarted)
			{
				// Cancel opposite look
				if (character.Input.HorizontalAxisDigital != -currentWallDirection)
				{
					oppositeLookTimer = 0;
				}
				// Do opposite look behaviour
				if (oppositeLookTimer > 0)
				{
					// Check for jump
					if (!hasJumped && ignoreJumpTimer <= 0.0f && (character.Input.JumpButton == ButtonState.DOWN || character.Input.JumpButton == ButtonState.HELD))
					{
						DoJump ();
					}
					// Cling - Slow down to stop
					else
					{
						character.AddVelocity (0, TimeManager.FrameTime * clingDrag, false);
						if (character.Velocity.y > 0) character.SetVelocityY (0);
					}
				}
				else
				{
					
				}
			}
			else
			{
				// Let go of button
				if (character.Input.HorizontalAxisDigital != currentWallDirection)
				{
					clingStarted = false;
				}
				// If we have stopped sliding but are still able to jump
				if (!clingStarted)
				{
					// Allow jump
					if (!hasJumped && ignoreJumpTimer <= 0.0f && (character.Input.JumpButton == ButtonState.DOWN || character.Input.JumpButton == ButtonState.HELD))
					{
						DoJump ();
					}
					// But if jump isn't happening fall through to default air
					else
					{
						character.DefaultAirMovement.DoMove ();
					}
				} 
				else
				{
					// Jump
					if (!hasJumped && ignoreJumpTimer <= 0.0f && (character.Input.JumpButton == ButtonState.DOWN || character.Input.JumpButton == ButtonState.HELD))
					{
						DoJump ();
					} 
					else
					{
						// Check for up key and climb (also check run timer/limit)
						if (wallRunTimer > 0)
						{
							if (character.Input.VerticalAxisDigital == 1)
							{
								if (character.Velocity.y < 0)
								{
									// Come to a stop
									character.AddVelocity (0, TimeManager.FrameTime * clingDrag, false);
									if (character.Velocity.y > 0)
										character.SetVelocityY (0);
								} 
								else
								{
									character.SetVelocityY (climbUpSpeed);
									isRunning = true;
								}
							} 
							else
							{
								// Cancel run
								wallRunTimer = 0;
							}
						} 

						// Check for gravity cling timer expired
						else if (ignoreGravityTimer <= 0)
						{
							// Slide down
							character.AddVelocity (0, TimeManager.FrameTime * clingGravity, false);
							if (character.Velocity.y < slideDownSpeed)
								character.SetVelocityY (slideDownSpeed);
						} 
						else
						{
							// Slow down to stop
							character.AddVelocity (0, TimeManager.FrameTime * clingDrag, false);
							if (character.Velocity.y > 0)
								character.SetVelocityY (0);
						}

					}
				}
				// Translate
				character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
			}
		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't.
		/// </summary>
		/// <returns>false always</returns>
		override public bool ForceMaintainControl()
		{	
			if (character.Grounded) return false;
			if (character.CurrentWall == null) return false;
			if (character.CurrentWallColliderCount < RequiredColliders) return false;
			if (ableToacceptInputTimer > 0.0f) return true;
			return false;
		}

		/// <summary>
		/// Does the jump
		/// </summary>
		virtual protected void DoJump()
		{
			hasJumped = true;
			character.DefaultAirMovement.DoOverridenJump (jumpHeight, 1);
			oppositeLookTimer = 0;
			clingStarted = false;
		}

		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);

			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				RequiredColliders = movementData[RequiredCollidersIndex].IntValue;
				clingGravity = movementData[ClingGravityIndex].FloatValue;
				gravityDelay = movementData[GravityDelayIndex].FloatValue;
				jumpHeight = movementData[JumpHeightIndex].FloatValue;
				ignoreClingAfterJumpTime = 0;
				mustHoldToMaintainCling = movementData[MustHoldToMaintainClingIndex].BoolValue;
				ignoreJumpTime = movementData[IgnoreJumpTimeIndex].FloatValue;
				climbUpSpeed = movementData [RunSpeedIndex].FloatValue;
				clingDrag = movementData [ClingDragIndex].FloatValue;
				slideDownSpeed = movementData [SlideDownSpeedIndex].FloatValue;
				runTimeLimit = movementData [RunTimeLimitIndex].FloatValue;
				oppositeLookTimeLimit = movementData [OppositeLookTimeIndex].FloatValue;
				speedWhereWallJumpEnds = movementData[SpeedWhereWallJumpEndsIndex].FloatValue;
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
				if (isRunning) return AnimationState.WALL_RUN;
				if (oppositeLookStarted) return AnimationState.WALL_CLING;
				if (!clingStarted) return AnimationState.FALL;
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
			// Cant wall climb if grounded and less holding up
			if (character.Grounded) return false;

			// Moving away from wall
			if (hasJumped)
			{
				if (character.Velocity.y <= speedWhereWallJumpEnds)
				{
					return false;
				}
				else if (character.CurrentWall == null || character.CurrentWall == cachedWall)
				{
					return true;
				}
				else if (character.CurrentWall != null && ((int)character.Colliders[character.CurrentWallCollider].GetDirection().x) != currentWallDirection) 
				{
					return false;
				}
			}

			// Not moving away from wall and no wall
			if (character.CurrentWall == null) return false;

			// Or we don't have enough wall colliders
			if (character.CurrentWallColliderCount < RequiredColliders) return false;

			if (ableToacceptInputTimer > 0)
			{
				// Hold control if we could potenetial start an opposite look
				if (!oppositeLookStarted && currentWallDirection != 0 && character.Input.HorizontalAxisDigital == -currentWallDirection)
				{
					return true;
				}

				// Hold control if we could start a jump
				if (ignoreJumpTimer <= 0.0f && (character.Input.JumpButton == ButtonState.DOWN || character.Input.JumpButton == ButtonState.HELD))
				{
					return true;
				}
			}
		

			// Opposite look started
			if (oppositeLookStarted && oppositeLookTimer > 0) return true;

			// Hold towards wall
			if (character.CurrentWall != null && ((int)character.Colliders[character.CurrentWallCollider].GetDirection().x) == character.Input.HorizontalAxisDigital) 
			{
				// Only wall cling if the highest collider is colliding
				if (character.Colliders[character.CurrentWallCollider] == highestLeftCollider || character.Colliders[character.CurrentWallCollider] == highestRightCollider)
				{
					cachedWall = character.CurrentWall;
					currentWallDirection = ((int)character.Colliders[character.CurrentWallCollider].GetDirection().x);
					return true;
				}
			}
			return false;
		}

		// <summary>
		/// Does the cling.
		/// </summary>
		override public void DoCling()
		{
			float tmpVelocityY = character.Velocity.y;
			base.DoCling ();
			oppositeLookTimer = 0;
			oppositeLookStarted = false;
			character.SetVelocityY (tmpVelocityY);
			// If this is a wall run then user must start by holding down run
			if (runTimeLimit > 0 && character.Input.VerticalAxisDigital == 1 && character.Velocity.y > 0)
			{
				wallRunTimer = runTimeLimit;
			} else
			{
				wallRunTimer = 0;
			}
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			base.LosingControl ();
			wallRunTimer = 0;
			oppositeLookTimer = 0;
			oppositeLookStarted = false;
			cachedWall = null;
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

			// Ledge climb types
			GUILayout.Label ("Wall Running", EditorStyles.boldLabel);

			// Climb speed
			if (movementData[RunSpeedIndex] == null) movementData[RunSpeedIndex] = new MovementVariable(DefaultRunSpeed);
			movementData[RunSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Run Speed", "How fast the character runs up the wall."), movementData[RunSpeedIndex].FloatValue);
			if (movementData[RunSpeedIndex].FloatValue < 0) movementData[RunSpeedIndex].FloatValue = 0;

			if (movementData[RunTimeLimitIndex] == null) movementData[RunTimeLimitIndex] = new MovementVariable(1);
			movementData[RunTimeLimitIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Run Time", "How long we can run up the wall."), movementData[RunTimeLimitIndex].FloatValue);
			if (movementData[RunTimeLimitIndex].FloatValue < 0) movementData[RunTimeLimitIndex].FloatValue = 0;

			// Ledge climb types
			GUILayout.Label ("Opposite Look", EditorStyles.boldLabel);

			if (movementData[OppositeLookTimeIndex] == null) movementData[OppositeLookTimeIndex] = new MovementVariable(1);
			movementData[OppositeLookTimeIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Opposite Look Time", "How long can we look in opposite direction."), movementData[OppositeLookTimeIndex].FloatValue);
			if (movementData[OppositeLookTimeIndex].FloatValue < 0) movementData[RunTimeLimitIndex].FloatValue = 0;

			// Ledge climb types
			GUILayout.Label ("Speed and Gravity", EditorStyles.boldLabel);

			// Slide down speed
			if (movementData[SlideDownSpeedIndex] == null) movementData[SlideDownSpeedIndex] = new MovementVariable(DefaultSlideDownSpeed);
			movementData[SlideDownSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Slide Down Speed", "How fast the character slides down the wall (negative)."), movementData[SlideDownSpeedIndex].FloatValue);
			if (movementData[SlideDownSpeedIndex].FloatValue > 0) movementData[SlideDownSpeedIndex].FloatValue = 0;

			// Cling drag
			if (movementData[ClingDragIndex] == null) movementData[ClingDragIndex] = new MovementVariable(DefaultClingDrag);
			movementData[ClingDragIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Cling Drag", "How quickly we come to a halt when the user is not pressing down."), movementData[ClingDragIndex].FloatValue);
			if (movementData[ClingDragIndex].FloatValue < 0) movementData[ClingDragIndex].FloatValue = 0;

			// Speed where jump ends
			if (movementData[SpeedWhereWallJumpEndsIndex] == null) movementData[SpeedWhereWallJumpEndsIndex] = new MovementVariable(DefaultSpeedWhereWallJumpEnds);
			movementData[SpeedWhereWallJumpEndsIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Speed where Jump ends", "How fast does the character need to be falling before the wall jump finishes and control reverts back to player?"), movementData[SpeedWhereWallJumpEndsIndex].FloatValue);
			if (movementData[SpeedWhereWallJumpEndsIndex].FloatValue > 0) movementData[SpeedWhereWallJumpEndsIndex].FloatValue = 0;


			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = WallMovement_WallCling.DrawInspectorWithOptions (movementData, ref showDetails, target, true, false);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			return movementData;
		}

		#endregion

#endif
	}

}

