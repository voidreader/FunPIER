#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A wall movement class that grabs a ledge and climbs up it.
	/// </summary>
	public class WallMovement_LedgeClimb : WallMovement
	{
		
		#region members

		/// <summary>
		/// How do we detect if wall is actually a ledge.
		/// </summary>
		public LedgeDetectionType ledgeDetectionType;

		/// <summary>
		/// How do we detect if there is a gap the character can move through.
		/// </summary>
		public GapDetectionType gapDetectionType;

		/// <summary>
		/// How do we target the character at the ledge.
		/// </summary>
		public LedgeClimbAnimationTargetting animationTargetting;

		
		/// <summary>
		/// How long to fall from the ledge wihtout rechecking for ledge. Used to ensure the character can't regrab the ledge they just fell from.
		/// </summary>
		public float fallTime = 0.33f;
		
		/// <summary>
		/// Maximum distance below the ledge at which the character can move from reach to grasp.
		/// </summary>
		public float maxGraspLeeway;
		
		/// <summary>
		/// Minimum distance below the ledge at which the character can move from reach to grasp.
		/// </summary>
		public float minGraspLeeway;
		
		/// <summary>
		/// How long (in seconds) must a character reach out for the ledge before the are allowed to grab it.
		/// </summary>
		public float minReachTime;

		/// <summary>
		/// Stores the offset for the ledge when in BOX mode.
		/// </summary>
		protected Vector2 ledgeOffset;

		/// <summary>
		/// Stores the offset for the standing position.
		/// </summary>
		protected Vector2 standOffset;

		/// <summary>
		/// Stores the offset for the dismount position.
		/// </summary>
		protected Vector2 dismountOffset;

		/// <summary>
		/// Bone to use for BONE targetting while grasping.
		/// </summary>
		protected HumanBodyBones graspingBone;

		/// <summary>
		/// Current state of the ledge climb.
		/// </summary>
		protected LedgeClimbState ledgeClimbState; 

		/// <summary>
		/// If the animator drives animation state then keep a cached copy.
		/// </summary>
		protected Animator myAnimator;

		protected SpriteRenderer myRenderer;

		/// <summary>
		/// Cache the facing direction.
		/// </summary>
		protected int facingDirection;

		/// <summary>
		/// While this is bigger than true ignore all ledge grabs (becasue the user told us to drop).
		/// </summary>
		protected float fallTimer;

		/// <summary>
		/// Determines how long we have been reaching for a ledge.
		/// </summary>
		protected float reachTimer;

		/// <summary>
		/// Cached position of the inital sprite pivot.
		/// </summary>
		protected Vector2 initialSpritePivot;

		/// <summary>
		/// Cached copy of the ledge climb wall.
		/// </summary>
		protected Collider2D currentWall;

		/// <summary>
		/// Defines the current offset as an offset from the current wall.
		/// </summary>
		protected Vector2 currentTransformOffset;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Ledge Climb/Standard";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A wall movement that allows player to grabs a ledge and climb up it.";
		
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
		/// The default minimum reach time.
		/// </summary>
		protected const float DefaultMinReachTime = 0.1f;
		
		/// <summary>
		/// The index of the property for ledge detection type.
		/// </summary>
		protected const int LedgeDetectionTypeIndex = 2;

		/// <summary>
		/// The index of the property for gap detection type.
		/// </summary>
		protected const int GapDetectionTypeIndex = 3;

		/// <summary>
		/// The index of the animation target type.
		/// </summary>
		protected const int AnimationTargettingIndex = 4; 

		/// <summary>
		/// The index of the ledge offset.
		/// </summary>
		protected const int LedgeOffsetIndex = 5;

		/// <summary>
		/// The index of the grasping bone.
		/// </summary>
		protected const int GraspingBoneIndex = 6;

		/// <summary>
		/// The index of the stand offset.
		/// </summary>
		protected const int StandOffsetIndex = 7;

		/// <summary>
		/// The index of the max grasp leeway.
		/// </summary>
		protected const int MaxGraspLeewayIndex = 8;

		/// <summary>
		/// The index of the minimum grasp leeway.
		/// </summary>
		protected const int MinGraspLeewayIndex = 9;

		/// <summary>
		/// The  index of the minimum reach time.
		/// </summary>
		protected const int MinReachTimeIndex = 10;

		/// <summary>
		/// The index of the fall time.
		/// </summary>
		protected const int FallTimeIndex = 11;

		/// <summary>
		/// The index of the dismount offset.
		/// </summary>
		protected const int DismountOffsetIndex = 12;
		
		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 13;
		
		#endregion

		protected Vector2 CurrentLedgePosition
		{
			get 
			{
				return (Vector2)currentWall.transform.position - currentTransformOffset;
			}
		}

		#region Unity hooks
		
		/// <summary>
		/// Unity Update() hook
		/// </summary>
		void Update()
		{
			if (fallTimer > 0) fallTimer -= TimeManager.FrameTime;
			if (ledgeClimbState == LedgeClimbState.REACHING) reachTimer += TimeManager.FrameTime;
		}
		
		#endregion
		
		#region public methods
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// Early out if falling
			if (fallTimer > 0.0f) return;

			// Check for fall key
			if (UserPressingFallKey ())
			{
				if (ledgeClimbState == LedgeClimbState.HANGING || ledgeClimbState == LedgeClimbState.GRASPING  )
				{
					ledgeClimbState = LedgeClimbState.DISMOUNTING;
					return;
				}
			}

			if (ledgeClimbState == LedgeClimbState.HANGING && UserPressingClimbKey ())
			{
				ledgeClimbState = LedgeClimbState.CLIMBING;
			}


			switch (ledgeClimbState)
			{
				case LedgeClimbState.REACHING:
					character.DefaultAirMovement.DoMove();
					break;
				case LedgeClimbState.GRASPING:
					MoveToHangPosition();
					break;
				case LedgeClimbState.HANGING:
					MoveToHangPosition();
					break;
				case LedgeClimbState.DISMOUNTING:
					MoveToDismountPosition();
					break;

			}

			// TODO Only to this if required
			CheckForAnimationStateTransition ();
			
			if (ledgeClimbState == LedgeClimbState.REACHING && CheckForGrab ())
			{
				ledgeClimbState = LedgeClimbState.GRASPING;
				Platform p = character.CurrentWall.GetComponent<Platform> ();
				if (p != null) {
					character.ParentPlatform = p;
					if (facingDirection == 1)
					{
						character.ParentRaycastType = RaycastType.SIDE_RIGHT;
					}
					else
					{
						character.ParentRaycastType = RaycastType.SIDE_LEFT;
					}
				}
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
			
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				RequiredColliders = movementData[RequiredCollidersIndex].IntValue;
				ledgeDetectionType = (LedgeDetectionType) movementData[LedgeDetectionTypeIndex].IntValue;
				gapDetectionType = (GapDetectionType) movementData[GapDetectionTypeIndex].IntValue;
				animationTargetting = (LedgeClimbAnimationTargetting) movementData[AnimationTargettingIndex].IntValue;
				ledgeOffset = movementData[LedgeOffsetIndex].Vector2Value;
				standOffset = movementData[StandOffsetIndex].Vector2Value;
				dismountOffset = movementData[DismountOffsetIndex].Vector2Value;
				graspingBone = (HumanBodyBones) movementData[GraspingBoneIndex].IntValue;
				minGraspLeeway = movementData[MinGraspLeewayIndex].FloatValue;
				maxGraspLeeway = movementData[MaxGraspLeewayIndex].FloatValue;
				minReachTime = movementData[MinReachTimeIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data, not enough values.");
			}

			// TODO Only get this if required
			myAnimator = character.gameObject.GetComponentInChildren<Animator> ();
			if (animationTargetting == LedgeClimbAnimationTargetting.SPRITE_PIVOT) 
			{
				myRenderer = character.gameObject.GetComponentInChildren<SpriteRenderer> ();
				if (myRenderer == null) 
				{
					// TODO Throw error?
				} 
				else
				{
					initialSpritePivot = myRenderer.sprite.bounds.center;
				}
			}
			return this;
		}
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				switch (ledgeClimbState)
				{

					case LedgeClimbState.GRASPING: return AnimationState.LEDGE_GRASP;
					case LedgeClimbState.REACHING: return AnimationState.LEDGE_REACH;
					case LedgeClimbState.HANGING: return AnimationState.LEDGE_HANG;
					case LedgeClimbState.CLIMBING: return AnimationState.LEDGE_CLIMB;
					case LedgeClimbState.DISMOUNTING: return AnimationState.LEDGE_DISMOUNT;
				}
				return AnimationState.IDLE;
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
				return facingDirection;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate a wall clinging behaviour.
		/// </summary>
		override public bool WantsCling()
		{
			// Climb finished
			if (ledgeClimbState == LedgeClimbState.CLIMB_FINISHED) return false;
			// Dismounting
			if (ledgeClimbState == LedgeClimbState.DISMOUNTING || ledgeClimbState == LedgeClimbState.DISMOUNT_FINISHED) return false;
			// User requested us to fall from ledge
			if (fallTimer > 0.0f) return false;
			// Can't ledge climb if grounded
			if (character.Grounded) return false;
			// Or moving upwards
			if (character.Velocity.y > 0) return false;
			// Or we don't have enough wall colliders
			if (character.CurrentWallColliderCount < RequiredColliders) return false;
			// Looking good update facing direction
			facingDirection = (int)character.Colliders[character.CurrentWallCollider].GetDirection().x;
			// Check if we have a keypress that cancels a ledge climb
			if (UserPressingFallKey()) return false;
			// Check if we have a viable ledge and if so proceed with ledge climb
			if (CheckLedge() && CheckGraspLeeway(false) && CheckGap()) return true;
			return false;
		}

		// <summary>
		/// Does the cling.
		/// </summary>
		override public void DoCling()
		{
			facingDirection = (int)character.Colliders[character.CurrentWallCollider].GetDirection().x;
			ledgeClimbState = LedgeClimbState.REACHING;
			reachTimer = 0.0f;
		}
		
		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't. In this case control unless the character dismounts.
		/// </summary>
		override public bool ForceMaintainControl()
		{
			if (ledgeClimbState == LedgeClimbState.CLIMB_FINISHED)
			{
				// In sprite mode we hard snap to the final position on the last frame so the sprite doesn't jerk or fall off the ledge
				if (animationTargetting == LedgeClimbAnimationTargetting.SPRITE_PIVOT)
				{
					Vector2 actualStandOffset = standOffset;
					if (facingDirection == 1) actualStandOffset.x *= -1;
					Vector2 spriteOffset = (CurrentLedgePosition + actualStandOffset) - (Vector2)character.Transform.position;
					character.Translate (spriteOffset.x, spriteOffset.y, true);
				}
				return false;
			}
			if (ledgeClimbState == LedgeClimbState.DISMOUNT_FINISHED)
			{
				// Start falling
				fallTimer = fallTime;
				// In sprite mode we hard snap to the final position on the last frame so the sprite doesn't jerk
				if (animationTargetting == LedgeClimbAnimationTargetting.SPRITE_PIVOT)
				{
					Vector2 actualoffset = standOffset + dismountOffset;
					if (facingDirection == 1) actualoffset.x *= -1;
					actualoffset.y *= -1;
					Vector2 spriteOffset = (CurrentLedgePosition - actualoffset) - (Vector2)character.Transform.position;
					character.Translate (spriteOffset.x, spriteOffset.y, true);
				}
				return false;
			}
			if (character.Grounded) return false;
			if (fallTimer > 0.0f) return false;
			if (ledgeClimbState == LedgeClimbState.REACHING) return false;
			return true;
		}

		
		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// base collisions to be executed after its movement finishes.
		/// </summary>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				if (ledgeClimbState == LedgeClimbState.REACHING) return RaycastType.ALL;
				return RaycastType.NONE;
			}
		}


		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// rotations to be calculated and applied by the character.
		/// </summary>
		override public bool ShouldDoRotations
		{
			get
			{
				if (ledgeClimbState == LedgeClimbState.CLIMB_FINISHED) return true;
				return false;
			}
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions. You should
		/// ensure that character velocity is reset back to world-relative velocity here.
		/// </summary>
		override public void LosingControl() 
		{
			// If we are doing anything other than reaching we should set y velocity back to 0
			if (ledgeClimbState != LedgeClimbState.REACHING) character.SetVelocityY (0);
			ledgeClimbState = LedgeClimbState.NONE;
			reachTimer = 0.0f;
			facingDirection = 0;

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
		/// Checks the ledge using the  selected ledge dection type.
		/// </summary>
		/// <returns><c>true</c>, if ledge was found, <c>false</c> otherwise.</returns>
		virtual protected bool CheckLedge() 
		{
			currentWall = character.CurrentWall;
			switch (ledgeDetectionType)
			{
				case LedgeDetectionType.NONE:
					if (character.CurrentWall) 
					{
						currentTransformOffset = Vector2.zero;
						return true;
					}
					break;
				case LedgeDetectionType.BOX:
					if (character.CurrentWall && character.CurrentWall is BoxCollider2D) 
					{
						BoxCollider2D box = (BoxCollider2D) character.CurrentWall;
						facingDirection = (int)character.Colliders[character.CurrentWallCollider].GetDirection().x; 
						if (facingDirection == 1)
						{
	
							currentTransformOffset = (Vector2)currentWall.transform.position - (new Vector2(-ledgeOffset.x, ledgeOffset.y) + 
							 						// X
													(Vector2)box.transform.position + 
							 						new Vector2((box.transform.lossyScale.x < 0 ? -1 : 1) * box.transform.lossyScale.x * (box.Offset().x - (box.size.x /2.0f)),
							            			// Y
							                        box.transform.lossyScale.y * (box.Offset().y + (box.size.y / 2.0f))));
							return true;
						}
						else if (facingDirection == -1)
						{
							currentTransformOffset = (Vector2)currentWall.transform.position - (new Vector2(-ledgeOffset.x, ledgeOffset.y) + 
                                                   	// X
													(Vector2)box.transform.position + new Vector2((box.transform.lossyScale.x < 0 ? -1 : 1) * box.transform.lossyScale.x * (box.Offset().x + (box.size.x / 2.0f)),
	                                                // Y
				                                    box.transform.lossyScale.y * (box.Offset().y + (box.size.y / 2.0f))));
							return true;
						}
					}
					break;

				case LedgeDetectionType.CIRCLE_CAST:
					Vector2 origin = (Vector2)character.transform.position + new Vector2(0,2);
					origin += new Vector2(0.05f, 0) * facingDirection;
					RaycastHit2D ledgeHit = Physics2D.CircleCast(origin, 0.5f, new Vector2(0,-1), 2, character.geometryLayerMask);
					if (ledgeHit.collider  != null && ledgeHit.collider == character.CurrentWall)
					{
						currentTransformOffset = (Vector2)currentWall.transform.position - (new Vector2(-ledgeOffset.x, ledgeOffset.y) + ledgeHit.point);
						return true;
					}
					break;
			}
			return false;
		}

		/// <summary>
		/// Checked for a gap using the selected gap detection type.
		/// </summary>
		/// <returns><c>true</c>, if gap was checked, <c>false</c> otherwise.</returns>
		virtual protected bool CheckGap()
		{
			switch (gapDetectionType)
			{
			case GapDetectionType.NONE: return true;
			case GapDetectionType.RAYCAST_SIDE_COLLIDERS: return DoGapDectionFromSideColliders();
			}
			return false;
		}
		
		/// <summary>
		/// Check for a gap by casting rays extending along the side colldiers.
		/// </summary>
		/// <returns><c>true</c>, if gap dection from side colliders was done, <c>false</c> otherwise.</returns>
		virtual protected bool DoGapDectionFromSideColliders() 
		{
			RaycastType typeToCheck = character.Colliders [character.CurrentWallCollider].RaycastType;
			// Determine how much to offset the raycasts from ledge position
			Vector2 transformOffset = Vector2.zero;
			switch (animationTargetting)
			{
				case LedgeClimbAnimationTargetting.SPRITE_PIVOT:
					// For a sprite pivot use the stand offset
					transformOffset = standOffset;
					if (typeToCheck == RaycastType.SIDE_RIGHT) transformOffset.x = transformOffset.x * -1;
					break;
				case LedgeClimbAnimationTargetting.BAKED:
					// For a sprite pivot use the stand offset
					transformOffset = standOffset;
					if (typeToCheck == RaycastType.SIDE_RIGHT) transformOffset.x = transformOffset.x * -1;
					break;
			}
			foreach (BasicRaycast c in character.Colliders)
			{
				// If collider is of the right type
				if (c.RaycastType == typeToCheck)
				{
					Vector2 finalPosition = CurrentLedgePosition + c.Extent + transformOffset;
					Vector2 initialPosition = new Vector2(character.transform.position.x, finalPosition.y);
					Vector2 direction = (finalPosition - initialPosition).normalized;
					float distance = Mathf.Abs(finalPosition.x - character.transform.position.x);
					
					RaycastHit2D hit = Physics2D.Raycast(initialPosition, direction, distance, character.geometryLayerMask);
					if (hit.collider != null) {
						return false;
					}
				}
			}
			return true;
		}

		void LateUpdate()
		{
			// In baked mode we hard snap to the final position on all frames where the animator is in state LEDGE_DONE so the sprite doesn't jerk or fall off the ledge
			if ( animationTargetting == LedgeClimbAnimationTargetting.BAKED && (ledgeClimbState == LedgeClimbState.CLIMBING || ledgeClimbState == LedgeClimbState.CLIMB_FINISHED))
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				if (info.IsName(AnimationState.LEDGE_DONE.AsString()) || (info.IsName(AnimationState.LEDGE_CLIMB.AsString()) && info.normalizedTime >= 1.0f))
				{
					Vector2 actualStandOffset = standOffset;
					if (facingDirection == 1) actualStandOffset.x *= -1;
					Vector2 spriteOffset = (CurrentLedgePosition + actualStandOffset) - (Vector2)character.Transform.position;
					character.Translate (spriteOffset.x, spriteOffset.y, true);	
					ledgeClimbState = LedgeClimbState.CLIMB_FINISHED;
				}
			}
			if ( animationTargetting == LedgeClimbAnimationTargetting.BAKED && (ledgeClimbState == LedgeClimbState.DISMOUNTING || ledgeClimbState == LedgeClimbState.DISMOUNT_FINISHED))
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				if (info.IsName(AnimationState.LEDGE_DISMOUNT_DONE.AsString()))
				{
					// We assume here that dismounting returns you to the same spot as you started the grab
//					Vector2 actualOffset = ledgeOffset;
//					if (facingDirection == 1) actualOffset.x *= -1;
//					Vector2 spriteOffset = (CurrentLedgePosition - actualOffset) - (Vector2)character.Transform.position;
//					character.Translate (spriteOffset.x, spriteOffset.y, true);	
					ledgeClimbState = LedgeClimbState.DISMOUNT_FINISHED;
				}
			}
		}

		/// <summary>
		/// Checks for animation state transition. I.e. when ledge climb state should be changed based on the
		/// animation state.
		/// </summary>
		virtual protected void CheckForAnimationStateTransition() 
		{
			AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
			if (ledgeClimbState == LedgeClimbState.GRASPING)
			{
				if (info.IsName(AnimationState.LEDGE_HANG.AsString())) ledgeClimbState = LedgeClimbState.HANGING;
			}
			else if (ledgeClimbState == LedgeClimbState.CLIMBING) 
			{
				if ((info.IsName(AnimationState.LEDGE_CLIMB.AsString()) && info.normalizedTime >= 1) || info.IsName(AnimationState.LEDGE_DONE.AsString()))
				{
					/// Don't do this transition in baked mode as we need to apply the transform in LateUpdate instead
					if (animationTargetting != LedgeClimbAnimationTargetting.BAKED)
					{
						ledgeClimbState = LedgeClimbState.CLIMB_FINISHED;
					}
				} 
			}
			else if (ledgeClimbState == LedgeClimbState.DISMOUNTING) 
			{
				if ((info.IsName(AnimationState.LEDGE_DISMOUNT.AsString()) && info.normalizedTime >= 1))
				{
					ledgeClimbState = LedgeClimbState.DISMOUNT_FINISHED;
				} 
			}
		}

		/// <summary>
		/// Transform the character to the correct hang position so the hands are hanging form the edge
		/// </summary>
		virtual protected void MoveToHangPosition()
		{
			AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
			if (animationTargetting == LedgeClimbAnimationTargetting.BAKED )
			{
				Vector2 handOffset = Vector2.Lerp (Vector2.zero, CurrentLedgePosition - (Vector2)myAnimator.GetBoneTransform (graspingBone).position, info.normalizedTime);
				character.Translate (handOffset.x, handOffset.y, true);
			}
			else if (animationTargetting == LedgeClimbAnimationTargetting.SPRITE_PIVOT)
			{
				// In sprite mode we hard snap to the position
				Vector2 spriteOffset = CurrentLedgePosition - (Vector2)character.Transform.position;
				character.Translate (spriteOffset.x, spriteOffset.y, true);
			}

		}

		/// <summary>
		/// When dismounting we do the opposite of move to hang.
		/// </summary>
		virtual protected void MoveToDismountPosition()
		{
//			AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
			if (animationTargetting == LedgeClimbAnimationTargetting.BAKED )
			{
				// Here we assume the hand used to grab is the hand the pushes away from the ledge
				// Vector2 handOffset = Vector2.Lerp (Vector2.zero, CurrentLedgePosition - (Vector2)myAnimator.GetBoneTransform (graspingBone).position, 1.0f - info.normalizedTime);
				Vector2 handOffset = CurrentLedgePosition - (Vector2)myAnimator.GetBoneTransform (graspingBone).position;
				character.Translate (handOffset.x, handOffset.y, true);
			}
		}

		/// <summary>
		/// Checks the grasp leeway.
		/// </summary>
		/// <returns><c>true</c>, if grasp leeway was checked, <c>false</c> otherwise.</returns>
		/// <param name="checkMin">If set to <c>true</c> check min constraint else just check max.</param>
		virtual protected bool CheckGraspLeeway(bool checkMin)
		{
			if ((!checkMin || (character.transform.position.y - CurrentLedgePosition.y < -minGraspLeeway) )&& 
			    (character.transform.position.y - CurrentLedgePosition.y > -maxGraspLeeway)) return true;
			return false;
		}
		
		/// <summary>
		/// Check if a grasp should become a grab.
		/// </summary>
		/// <returns><c>true</c>, if we should grab, <c>false</c> otherwise.</returns>
		virtual protected bool CheckForGrab()
		{
			if (character.CurrentWall != null && (reachTimer >= minReachTime) && CheckGraspLeeway(true)) return true;
			return false;
		}

		/// <summary>
		/// Is the user pressing a key that causes them to fall.
		/// </summary>
		/// <returns><c>true</c>, if user pressing a fall key, <c>false</c> otherwise.</returns>
		virtual protected bool UserPressingFallKey()
		{
			// Pressed Jump
			if (character.Input.JumpButton == ButtonState.DOWN) return true;
			// Holding or Pressing Down
			if (character.Input.VerticalAxisDigital == -1) return true;
			// Holding or pressing away from the ledge
			if (character.Input.HorizontalAxisDigital == -1 * facingDirection) return true;

			// All good
			return false;
		}

		/// <summary>
		/// Is the user pressing a key that causes them to climb.
		/// </summary>
		/// <returns><c>true</c>, if user pressing a climb key, <c>false</c> otherwise.</returns>
		virtual protected bool UserPressingClimbKey()
		{
			// Holding or Pressing Down
			if (character.Input.VerticalAxisDigital == 1) return true;
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
			if (movementData == null)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}
			// Auto upgrade
			else if (movementData.Length == MovementVariableCount - 1)
			{
				Debug.Log ("Upgading movement data for ledge climb.");
				MovementVariable[] tmpMovementData = movementData;
				movementData = new MovementVariable[MovementVariableCount];
				System.Array.Copy (tmpMovementData, movementData, MovementVariableCount - 1);
			}
			else if (movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Ledge climb types
			GUILayout.Label ("Ledge Dectection", EditorStyles.boldLabel);

			// Ledge detection
			if (movementData[LedgeDetectionTypeIndex] == null) movementData[LedgeDetectionTypeIndex] = new MovementVariable();
			movementData[LedgeDetectionTypeIndex].IntValue = (int) (LedgeDetectionType) EditorGUILayout.EnumPopup(new GUIContent("Ledge Dection", "Method used to determine if the current wall is a ledge."), (LedgeDetectionType) movementData[LedgeDetectionTypeIndex].IntValue);
		
			if (((LedgeDetectionType)(movementData [LedgeDetectionTypeIndex].IntValue)) != LedgeDetectionType.NONE)
			{
				// Ledge offset
				if (movementData[LedgeOffsetIndex] == null) movementData[LedgeOffsetIndex] = new MovementVariable();
				movementData[LedgeOffsetIndex].Vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Ledge Offset", "How much to offset the character from the ledge."), movementData[LedgeOffsetIndex].Vector2Value) ;

			}

			EditorGUILayout.HelpBox (((LedgeDetectionType)(movementData [LedgeDetectionTypeIndex].IntValue)).GetDescription(), MessageType.Info);

			// Gap detection
			if (movementData[GapDetectionTypeIndex] == null) movementData[GapDetectionTypeIndex] = new MovementVariable();
			movementData[GapDetectionTypeIndex].IntValue = (int) (GapDetectionType) EditorGUILayout.EnumPopup(new GUIContent("Gap Detection", "Method used to determine if there is enough gap to fit the player."), (GapDetectionType) movementData[GapDetectionTypeIndex].IntValue);
			EditorGUILayout.HelpBox (((GapDetectionType)(movementData [GapDetectionTypeIndex].IntValue)).GetDescription(), MessageType.Info);

			GUILayout.Label ("Animation", EditorStyles.boldLabel);

			// Animation Target Type
			if (movementData[AnimationTargettingIndex] == null) movementData[AnimationTargettingIndex] = new MovementVariable();
			movementData[AnimationTargettingIndex].IntValue = (int) (LedgeClimbAnimationTargetting) EditorGUILayout.EnumPopup(new GUIContent("Animation Targetting", "Method used to mtarget the character at the ledge."), (LedgeClimbAnimationTargetting) movementData[AnimationTargettingIndex].IntValue);
			EditorGUILayout.HelpBox (((LedgeClimbAnimationTargetting)(movementData [AnimationTargettingIndex].IntValue)).GetDescription(), MessageType.Info);

			// Stand offset
			if (movementData[StandOffsetIndex] == null) movementData[StandOffsetIndex] = new MovementVariable();
			movementData[StandOffsetIndex].Vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Stand Offset", "How far is the standing position from the ledge, when facing right (x value will be inversed when facing left)."), movementData[StandOffsetIndex].Vector2Value) ;

			// Dismount offset
			if (movementData [DismountOffsetIndex] == null) movementData [DismountOffsetIndex] = new MovementVariable ();
			if (((LedgeClimbAnimationTargetting)movementData [AnimationTargettingIndex].IntValue) == LedgeClimbAnimationTargetting.SPRITE_PIVOT) {
				movementData [DismountOffsetIndex].Vector2Value = EditorGUILayout.Vector2Field (new GUIContent ("Dismount Offset", "An additional offset to apply at the end of sprite based animatations to ensure the dismount aligns with fall position."), movementData [DismountOffsetIndex].Vector2Value);
			}

			switch(movementData[AnimationTargettingIndex].IntValue)
			{
				case (int)LedgeClimbAnimationTargetting.BAKED:
					// Bone for GRASPING
					if (movementData[GraspingBoneIndex] == null) movementData[GraspingBoneIndex] = new MovementVariable((int)HumanBodyBones.RightHand);
					movementData[GraspingBoneIndex].IntValue = (int) (HumanBodyBones)EditorGUILayout.EnumPopup(new GUIContent("Grasping Bone", "Bone to move towards the ledge when reaching for the ledge."), (HumanBodyBones) movementData[GraspingBoneIndex].IntValue );
					break;
			}

			showDetails = EditorGUILayout.Foldout(showDetails, "Details");

			// Make sure we reset to defaults if required even if showDetails is null
			if (movementData[MinReachTimeIndex] == null) movementData[MinReachTimeIndex] = new MovementVariable(DefaultMinReachTime);
			if (movementData[GraspingBoneIndex] == null || movementData[MaxGraspLeewayIndex] == null)
			{
				movementData[MinGraspLeewayIndex] = new MovementVariable();
				movementData[MaxGraspLeewayIndex] = new MovementVariable();
				DefaultGraspSettings(movementData, target);
			}
			if (movementData[RequiredCollidersIndex] == null) movementData[RequiredCollidersIndex] = new MovementVariable();

			if (showDetails)
			{
				// Min Reach Time
				movementData[MinReachTimeIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Minimum Reach Time", "Minimum time a character must be reaching for a ledge before they can grab it. Stops snapping and helps smooth long reach/grab animations."), movementData[MinReachTimeIndex].FloatValue) ;
				if (movementData[MinReachTimeIndex].FloatValue < 0) movementData[MinReachTimeIndex].FloatValue = 0;

				float min = movementData[MinGraspLeewayIndex].FloatValue;
				float max = movementData[MaxGraspLeewayIndex].FloatValue;

				EditorGUILayout.MinMaxSlider(new GUIContent("Grasp Leeway",""),  ref min , ref max, 0, 5);
				if (min != movementData[MinGraspLeewayIndex].FloatValue) 
				{
					movementData[MinGraspLeewayIndex].FloatValue = Mathf.Round(min * 20) / 20.0f;
				}
				if (max != movementData[MaxGraspLeewayIndex].FloatValue)
				{
					movementData[MaxGraspLeewayIndex].FloatValue  = Mathf.Round(max * 20) / 20.0f;
				}
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(" ", min.ToString() + "  to  " + max.ToString());
				if (GUILayout.Button("Default", EditorStyles.miniButton)) 
				{
					DefaultGraspSettings(movementData, target);
				}
				GUILayout.EndHorizontal();

				// Required Colliders
				movementData[RequiredCollidersIndex].IntValue = EditorGUILayout.IntField(new GUIContent("Required Colliders", "The number of colliders required to initiate wall cling."), movementData[RequiredCollidersIndex].IntValue);
				if (movementData[RequiredCollidersIndex].IntValue < 1) movementData[RequiredCollidersIndex].IntValue = 2;
			}
			
			return movementData;
		}

		/// <summary>
		/// Defaults the grasp settings to the characters height +- 0.25f
		/// </summary>
		/// <param name="movementData">Movement data.</param>
		/// <param name="target">Target.</param>
		protected static void DefaultGraspSettings(MovementVariable[] movementData, Character target) 
		{
			float characterMin = 0;
			float characterMax = 0;
			foreach (BasicRaycast c in target.Colliders)
			{
				if (c.RaycastType == RaycastType.SIDE_LEFT || c.RaycastType== RaycastType.SIDE_RIGHT)
				{
					if (c.Extent.y > characterMax) characterMax = c.Extent.y;
					if (c.Extent.y < characterMin) characterMin = c.Extent.y;
				}
			}
			float diff =  Mathf.Round(Mathf.Abs (characterMax - characterMin) * 20) / 20.0f;
			if (diff > 0)
			{
				movementData[MinGraspLeewayIndex].FloatValue = (diff - 0.25f) ;
				movementData[MaxGraspLeewayIndex].FloatValue = (diff + 0.25f) ;
			}
		}

		#endregion
		
		#endif
	}
	
}

