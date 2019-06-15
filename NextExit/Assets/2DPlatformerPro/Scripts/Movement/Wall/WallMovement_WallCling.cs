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
	/// A wall movment class that can stick to and slide down walls. The character can also jump from the wall.
	/// </summary>
	public class WallMovement_WallCling : WallMovement
	{

		#region members

		/// <summary>
		/// The gravity applied whle clining. Use 0 to allow infinite wall cling.
		/// </summary>
		public float clingGravity;

		/// <summary>
		/// How long before the (potentially reduced) gravity is applied to a character that is wall clinging.
		/// </summary>
		public float gravityDelay;

		/// <summary>
		/// How high the character jumps on a wall jump. If 0 character will just fall from wall when jump is pressed.
		/// </summary>
		public float jumpHeight;

		/// <summary>
		/// How long after jumping the character is not able to wall cling.
		/// </summary>
		public float ignoreClingAfterJumpTime;

		/// <summary>
		/// Does the user need to hold towards the wall constantly to maintain the cling?
		/// </summary>
		public bool mustHoldToMaintainCling;

		/// <summary>
		/// How long after clinging to the wall will the jump button be ignored.
		/// </summary>
		public float ignoreJumpTime;

		/// <summary>
		/// While larger than zero the wall cling will be ignored. Usualy set after jumping.
		/// </summary>
		protected float ignoreWallClimbTimer;

		/// <summary>
		/// While larger than zero no gravity will be applied.
		/// </summary>
		protected float ignoreGravityTimer;

		/// <summary>
		/// While larger than zero player cannot jump off of wall.
		/// </summary>
		protected float ignoreJumpTimer;

		/// <summary>
		/// The highest right collider.
		/// </summary>
		protected BasicRaycast highestRightCollider;

		/// <summary>
		/// The highest left collider.
		/// </summary>
		protected BasicRaycast highestLeftCollider;

		/// <summary>
		/// If this is non-zero the character is able to press jump in order to wall jump.
		/// </summary>
		protected float ableToWallJumpTimer;

		/// <summary>
		/// How long after releaseing the wall can the character still wall jump.
		/// TODO: This should be user controllable.
		/// </summary>
		protected float ableToWallJumpTime = 0.1f;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Wall Jump/Cling";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Wall movement which allows the character to stick to and climb certain walls. " +
										   "Control is handed back as soon as the character leaves the wall. Inspired by BroForce.";
		
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
		/// The index of the property for Cling Gravity.
		/// </summary>
		protected const int ClingGravityIndex = 2;

		/// <summary>
		/// The index of the property for Gravity Delay.
		/// </summary>
		protected const int GravityDelayIndex = 3;

		/// <summary>
		/// The index of the property for Jump height.
		/// </summary>
		protected const int JumpHeightIndex = 4;

		/// <summary>
		/// The index of the property for IgnoreClingAfterJumpTime.
		/// </summary>
		protected const int IgnoreClingAfterJumpTimeIndex = 5;

		/// <summary>
		/// The index of the property for MustHoldToMaintainCling.
		/// </summary>
		protected const int MustHoldToMaintainClingIndex = 6;

		/// <summary>
		/// The index of the property for IgnoreJumpTime.
		/// </summary>
		protected const int IgnoreJumpTimeIndex = 7;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 8;

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity Update() hook
		/// </summary>
		void Update()
		{
			if (ignoreWallClimbTimer > 0.0f) ignoreWallClimbTimer -= TimeManager.FrameTime;
			if (ignoreGravityTimer > 0.0f) ignoreGravityTimer -= TimeManager.FrameTime;
			if (ignoreJumpTimer > 0.0f) ignoreJumpTimer -= TimeManager.FrameTime;
			if (ableToWallJumpTimer > 0.0f) ableToWallJumpTimer -= TimeManager.FrameTime;
		}

		#endregion

		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// If we have stopped sliding but are still able to jump
			if (ableToWallJumpTimer > 0.0f)
			{
				// Allow jump
				if (ignoreJumpTimer <= 0.0f && (character.Input.JumpButton == ButtonState.DOWN || character.Input.JumpButton == ButtonState.HELD )) 
				{
					character.DefaultAirMovement.DoOverridenJump(jumpHeight, 1);
					ignoreWallClimbTimer = ignoreClingAfterJumpTime;
				}
				// But if jump isn't happening fall through to default air
				else
				{
					character.DefaultAirMovement.DoMove();
				}
			}
			else
			{
				if (ignoreJumpTimer <= 0.0f && (character.Input.JumpButton == ButtonState.DOWN || character.Input.JumpButton == ButtonState.HELD )) 
				{
					character.DefaultAirMovement.DoOverridenJump(jumpHeight, 1);
					ignoreWallClimbTimer = ignoreClingAfterJumpTime;
				}
				else if (ignoreGravityTimer > 0.0f)
				{
					// Nothing to do here?
				}
				else
				{
					character.AddVelocity(0, TimeManager.FrameTime * clingGravity, false);
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
				clingGravity = movementData[ClingGravityIndex].FloatValue;
				gravityDelay = movementData[GravityDelayIndex].FloatValue;
				jumpHeight = movementData[JumpHeightIndex].FloatValue;
				ignoreClingAfterJumpTime = movementData[IgnoreClingAfterJumpTimeIndex].FloatValue;
				mustHoldToMaintainCling = movementData[MustHoldToMaintainClingIndex].BoolValue;
				ignoreJumpTime = movementData[IgnoreJumpTimeIndex].FloatValue;
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
				return AnimationState.WALL_CLING;
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
			ignoreGravityTimer = gravityDelay;
			ignoreJumpTimer = ignoreJumpTime;
			ignoreWallClimbTimer = 0.0f;
			character.SetVelocityX (0.0f);
			character.SetVelocityY (0.0f);
			
			// Because we are moving and changing velocity all the time the colliders don't guarantee that we snap exactly to the wall
			// So here we calculate the absolute wall position with a new raycast
//			Vector2 initialPosition = character.Colliders [character.CurrentWallCollider].WorldPosition - new Vector2 (character.LastFacedDirection * 0.05f, 0);
//			Vector2 dir = (Vector2)character.CurrentWall.transform.position - character.Colliders [character.CurrentWallCollider].WorldPosition;
//			RaycastHit2D hit = Physics2D.Raycast (initialPosition, dir, 5.0f, 1 << character.CurrentWall.gameObject.layer);
//			if (hit.fraction > 0) character.Translate (hit.point.x - character.Colliders [character.CurrentWallCollider].WorldExtent.x, 0, false);

		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't. In this case control unless the character dismounts.
		/// </summary>
	    override public bool WantsControl()
		{
			if (character.Grounded) return false;
			if (ableToWallJumpTimer > 0.0f) return true;
			if (ignoreWallClimbTimer > 0.0f) return false;
			if (character.CurrentWall == null || character.CurrentWallCollider == -1) return false;
			if (((int)character.Colliders[character.CurrentWallCollider].GetDirection().x) == -character.Input.HorizontalAxisDigital) return false;
			if (mustHoldToMaintainCling && character.Input.HorizontalAxisDigital != (int)character.Colliders[character.CurrentWallCollider].GetDirection().x) return false;
			return true;
	    }

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			ableToWallJumpTimer = 0.0f;
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
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length != MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Cling Gravity
			if (movementData[ClingGravityIndex] == null) movementData[ClingGravityIndex] = new MovementVariable();
			movementData[ClingGravityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Cling Gravity", "Gravity to apply while wall clinging (negative number). Use 0 to allow infinite cling."), movementData[ClingGravityIndex].FloatValue);
			if (movementData[ClingGravityIndex].FloatValue > 0) movementData[ClingGravityIndex].FloatValue = 0;

			// Gravity Delay
			if (movementData[GravityDelayIndex] == null) movementData[GravityDelayIndex] = new MovementVariable();
			movementData[GravityDelayIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Gravity Delay", "How long to wait before applying gravity."), movementData[GravityDelayIndex].FloatValue);
			if (movementData[GravityDelayIndex].FloatValue < 0) movementData[GravityDelayIndex].FloatValue = 0;

			// JumpHeightIndex
			if (movementData[JumpHeightIndex] == null) movementData[JumpHeightIndex] = new MovementVariable();
			movementData[JumpHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Jump Height", "How high the character jumps away from the wall. Use zero to have jump simply detach from the wall."), movementData[JumpHeightIndex].FloatValue);
			if (movementData[JumpHeightIndex].FloatValue < 0) movementData[JumpHeightIndex].FloatValue = 0;

			showDetails = EditorGUILayout.Foldout(showDetails, "Details");
			if (showDetails)
			{
				// Required Colliders
				if (movementData[RequiredCollidersIndex] == null) movementData[RequiredCollidersIndex] = new MovementVariable();
				movementData[RequiredCollidersIndex].IntValue = EditorGUILayout.IntField(new GUIContent("Required Colliders", "The number of colliders required to initiate wall cling."), movementData[RequiredCollidersIndex].IntValue);
				if (movementData[RequiredCollidersIndex].IntValue < 1) movementData[RequiredCollidersIndex].IntValue = 2;

				// Ignore cling after jump time
				if (movementData[IgnoreClingAfterJumpTimeIndex] == null) movementData[IgnoreClingAfterJumpTimeIndex] = new MovementVariable();
				movementData[IgnoreClingAfterJumpTimeIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Ignore After Jump Time", "The number of seconds after jumping that all wall clings will be ignored."), movementData[IgnoreClingAfterJumpTimeIndex].FloatValue);
				if (movementData[IgnoreClingAfterJumpTimeIndex].FloatValue < 0) movementData[IgnoreClingAfterJumpTimeIndex].FloatValue = 0;

				// Must hold towards wall cosntantly to maintain cling
				if (movementData[MustHoldToMaintainClingIndex] == null) movementData[MustHoldToMaintainClingIndex] = new MovementVariable();
				movementData[MustHoldToMaintainClingIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Must Hold to Cling", "If true the user must constantly hold towards the wall to cling."), movementData[MustHoldToMaintainClingIndex].BoolValue);

				// Ignore jump time
				if (movementData[IgnoreJumpTimeIndex] == null) movementData[IgnoreJumpTimeIndex] = new MovementVariable();
				movementData[IgnoreJumpTimeIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Ignore Jump Time", "The number of seconds that the jump button will be ignored after a cling starts."), movementData[IgnoreJumpTimeIndex].FloatValue);
				if (movementData[IgnoreJumpTimeIndex].FloatValue < 0) movementData[IgnoreJumpTimeIndex].FloatValue = 0;

			}

			return movementData;
		}

		#endregion

#endif
	}

}

