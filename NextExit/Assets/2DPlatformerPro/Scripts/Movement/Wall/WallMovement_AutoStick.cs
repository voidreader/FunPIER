#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A wall movement class that automatically sticks to walls and can only be dismounted by pressing jump.
	/// </summary>
	public class WallMovement_AutoStick : WallMovement
	{
		
		#region members
		
		/// <summary>
		/// The gravity applied while clinging. Use 0 to allow infinite wall cling.
		/// </summary>
		public float clingGravity;
		
		/// <summary>
		/// How high the character jumps on a wall jump. If 0 character will just fall from wall when jump is pressed.
		/// </summary>
		public float jumpHeight;
		
		/// <summary>
		/// How far the character jumps away from the wall before giving back control to player.
		/// </summary>
		public float awayDistance;
		
		/// <summary>
		/// The horizontal movement speed when jumping away from the wall.
		/// </summary>
		public float airSpeed;
		
		/// <summary>
		/// The horizontal movement speed when jumping away from the wall with run depressed.
		/// </summary>
		public float airSpeedRun;
		
		/// <summary>
		/// Derived initial velocity based on jumpHeight and relative gravity.
		/// </summary>
		protected float initialVelocity;
		
		/// <summary>
		/// Currently moving away from wall?
		/// </summary>
		protected bool movingAwayFromWall;
		
		/// <summary>
		/// Cached copy of the current walls x position (in world space).
		/// </summary>
		protected float currentWallX;
		
		/// <summary>
		/// What was the wall direction for the last wall?
		/// </summary>
		protected int cachedWallDirection;
		
		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Wall Jump/Sticky";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A wall movement class that automatically sticks to walls and can only be dismounted by pressing jump. " +
			"Inpsired by Super Meat Boy.";
		
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
		protected const int JumpHeightIndex = 3;
		
		/// <summary>
		/// The index of the property for Jump height.
		/// </summary>
		protected const int AwayDistanceIndex = 4;
		
		/// <summary>
		/// The index of the property for air speed.
		/// </summary>
		protected const int AirSpeedIndex = 5;
		
		/// <summary>
		/// The index of the property for air speed with run depressed.
		/// </summary>
		protected const int AirSpeedRunIndex = 6;
		
		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 7;
		
		/// <summary>
		/// The default cling gravity.
		/// </summary>
		protected const float DefaultClingGravity = -10.0f;
		
		/// <summary>
		/// The default height of the jump.
		/// </summary>
		protected const float DefaultJumpHeight = 2.0f;
		
		/// <summary>
		/// The default away distance.
		/// </summary>
		protected const float DefaultAwayDistance = 1.0f;
		
		/// <summary>
		/// The default air speed.
		/// </summary>
		protected const float DefaultAirSpeed = 4.0f;
		
		/// <summary>
		/// The default air speed run.
		/// </summary>
		protected const float DefaultAirSpeedRun = 7.0f;
		
		#endregion
		
		#region Unity hooks
		
		//		/// <summary>
		//		/// Unity Update() hook
		//		/// </summary>
		//		void Update()
		//		{
		//		}
		
		#endregion
		
		#region public methods
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (movingAwayFromWall)
			{
				character.DefaultAirMovement.DoOverridenMove(true, true, (float) -cachedWallDirection, character.Input.RunButton);
				if ((cachedWallDirection == 1 && character.Transform.position.x < (currentWallX - awayDistance)) ||
				    (cachedWallDirection == -1 && character.Transform.position.x > (currentWallX + awayDistance)))
				{
					movingAwayFromWall = false;
				}
			}
			else if (character.CurrentWall != null && (character.Input.JumpButton == ButtonState.DOWN)) 
			{
				character.DefaultAirMovement.DoOverridenJump(jumpHeight, 1);
				movingAwayFromWall = true;
			}
			else
			{
				// Force Stop if graivty = 0 (else we would just slide around) TODO parameter for this?
				if (character.Velocity.y != 0 && clingGravity >= 0)
				{
					float sign = Mathf.Sign (character.Velocity.y);
					character.AddVelocity (0, sign * TimeManager.FrameTime * character.DefaultGravity, false);
					if (Mathf.Sign (character.Velocity.y) != sign) character.SetVelocityY (0);
				}
				else
				{
					character.AddVelocity (0, TimeManager.FrameTime * clingGravity, false);
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
				jumpHeight = movementData[JumpHeightIndex].FloatValue;
				awayDistance = movementData[AwayDistanceIndex].FloatValue;
				airSpeed = movementData[AirSpeedIndex].FloatValue;
				airSpeedRun = movementData[AirSpeedRunIndex].FloatValue;;
				initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * jumpHeight);
			}
			else
			{
				Debug.LogError("Invalid movement data, not enough values.");
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
			if (character.Grounded) return false;
			if (character.CurrentWallColliderCount < RequiredColliders) return false;
			if (character.CurrentWall != null) return true;
			return false;
		}
		
		// <summary>
		/// Does the cling.
		/// </summary>
		override public void DoCling()
		{
			character.SetVelocityX (0.0f);
			if (character.Velocity.y < 0.0f) character.SetVelocityY(0.0f);
			
			// Because we are moving and changing velocity all the time the colliders don't guarantee that we snap exactly to the wall
			// So here we calculate the absolute wall position with a new raycast
			//			Vector2 initialPosition = character.Colliders [character.CurrentWallCollider].WorldPosition - new Vector2 (character.LastFacedDirection * 0.1f, 0);
			//			Vector2 dir = (Vector2)character.CurrentWall.transform.position - character.Colliders [character.CurrentWallCollider].WorldPosition;
			//			RaycastHit2D hit = Physics2D.Raycast (initialPosition, dir, 5.0f, 1 << character.CurrentWall.gameObject.layer);
			//			character.Translate (hit.point.x - character.Colliders [character.CurrentWallCollider].WorldExtent.x, 0, false);
			//
			// Cache values
			cachedWallDirection = (int)character.Colliders[character.CurrentWallCollider].GetDirection().x;
			currentWallX = character.Transform.position.x;
		}
		
		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't. In this case control unless the character dismounts.
		/// </summary>
		override public bool WantsControl()
		{
			if (movingAwayFromWall) return true;
			return false;
			//			if (character.Grounded) return false;
			//			if (character.CurrentWall == null) return false;
			//			return true;
		}
		
		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl() {
			movingAwayFromWall = false;
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
			if (movementData[ClingGravityIndex] == null) movementData[ClingGravityIndex] = new MovementVariable(DefaultClingGravity);
			movementData[ClingGravityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Cling Gravity", "Gravity to apply while wall clinging (negative number). Use 0 to allow infinite cling."), movementData[ClingGravityIndex].FloatValue);
			if (movementData[ClingGravityIndex].FloatValue > 0) movementData[ClingGravityIndex].FloatValue = 0;
			
			// JumpHeightIndex
			if (movementData[JumpHeightIndex] == null) movementData[JumpHeightIndex] = new MovementVariable(DefaultJumpHeight);
			movementData[JumpHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Jump Height", "How high the character jumps away from the wall. Use zero to have jump simply detach from the wall."), movementData[JumpHeightIndex].FloatValue);
			if (movementData[JumpHeightIndex].FloatValue < 0) movementData[JumpHeightIndex].FloatValue = 0;
			
			// AwayDistanceIndex
			if (movementData[AwayDistanceIndex] == null) movementData[AwayDistanceIndex] = new MovementVariable(DefaultAwayDistance);
			movementData[AwayDistanceIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Away Distance", "How far the character jumps away from the wall before giving back control to player."), movementData[AwayDistanceIndex].FloatValue);
			if (movementData[AwayDistanceIndex].FloatValue < 0) movementData[AwayDistanceIndex].FloatValue = 0;
			
			// AirSpeedIndex
			if (movementData[AirSpeedIndex] == null) movementData[AirSpeedIndex] = new MovementVariable(DefaultAirSpeed);
			movementData[AirSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Air Speed", "The horizontal movement speed when jumping away from the wall."), movementData[AirSpeedIndex].FloatValue);
			if (movementData[AirSpeedIndex].FloatValue < 0) movementData[AirSpeedIndex].FloatValue = 0;
			
			// AirSpeedRunIndex
			if (movementData[AirSpeedRunIndex] == null) movementData[AirSpeedRunIndex] = new MovementVariable(DefaultAirSpeedRun);
			movementData[AirSpeedRunIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Air Speed Run", "The horizontal movement speed when jumping away from the wall with run depressed."), movementData[AirSpeedRunIndex].FloatValue);
			if (movementData[AirSpeedRunIndex].FloatValue < movementData[AirSpeedIndex].FloatValue) movementData[AirSpeedRunIndex].FloatValue = movementData[AirSpeedIndex].FloatValue;
			
			showDetails = EditorGUILayout.Foldout(showDetails, "Details");
			if (showDetails)
			{
				// Required Colliders
				if (movementData[RequiredCollidersIndex] == null) movementData[RequiredCollidersIndex] = new MovementVariable();
				movementData[RequiredCollidersIndex].IntValue = EditorGUILayout.IntField(new GUIContent("Required Colliders", "The number of colliders required to initiate wall cling."), movementData[RequiredCollidersIndex].IntValue);
				if (movementData[RequiredCollidersIndex].IntValue < 1) movementData[RequiredCollidersIndex].IntValue = 2;
				
			}
			
			return movementData;
		}
		
		#endregion
		
		#endif
		
	}
	
}

