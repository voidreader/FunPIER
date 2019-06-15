#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Physics based air movement.
	/// </summary>
	public class AirMovement_Physics : AirMovement, IPhysicsMovement
	{
		
		#region members

		/// <summary>
		/// The velocity applied in y when the character jumps.
		/// </summary>
		[TaggedProperty ("agility")]
		[TaggedProperty ("jumpHeight")]
		public float jumpVelocity;

		/// <summary>
		/// The speed the character accelerates at in x .
		/// </summary>
		[TaggedProperty ("acceleration")]
		[TaggedProperty("agility")]
		public float acceleration;
		
		/// <summary>
		/// The maximum speed the character can accelerate to in x. Note they can go faster than this if they do a running and jump and don't change direction.
		/// </summary>
		[TaggedProperty("agility")]
		[TaggedProperty("speedLimit")]
		public float maxSpeed;
		
		/// <summary>
		/// The air drag.
		/// </summary>
		public float drag;

		/// <summary>
		/// If true the acceleration will be multiplied via the analogue input value.
		/// </summary>
		public bool useAnalogueInput;

		/// <summary>
		/// After the user leaves the gorund how much additional time do we give the user to press jump 
		/// and still consider them grounded.
		/// </summary>
		public float jumpGroundedLeeway;

		/// <summary>
		/// Forces smaller than this will be ignored (generally applies to very small input values).
		/// </summary>
		public float ignoreForce;

		/// <summary>
		/// Can the character double jump.
		/// </summary>
		[TaggedProperty("canDoubleJump")]
		public bool canDoubleJump;
		
		/// <summary>
		/// The velocity applied in y when the character double jumps.
		/// </summary>
		public float doubleJumpVelocity;

		/// <summary>
		/// Should we jump away from the current angle, or just jump up?
		/// </summary>
		public bool jumpAway;

		/// <summary>
		/// How many times the character can double jump. -1 means infinite.
		/// </summary>
		[TaggedProperty ("maxDoubleJumpCount")]
		public int maxDoubleJumpCount;

		/// <summary>
		/// Character can only jump if this timer is less than zero.
		/// </summary>
		protected float readyToJumpTimer;
		
		/// <summary>
		/// True before the jump leaves the ground.
		/// </summary>
		protected bool jumpStart;
		
		/// <summary>
		/// Ensure we play the jump start animation.
		/// </summary>
		protected bool showJumpStartedAnimation;

		/// <summary>
		/// If a user keeps holding in the direction they jumped, they will jump further. If they stop doing so
		/// they can no longer move faster than max speed.
		/// </summary>
		protected bool hasRecievedCounterInput;

		/// <summary>
		/// The direction the character was moving when they started the jump.
		/// </summary>
		protected int initialDirection;
		
		/// <summary>
		/// Jump count, set to 0 while not jumping, 1 while jumping, 2 while double 
		/// </summary>
		protected int jumpCount;

		/// <summary>
		/// Automatically jump when the character holds the jump button down.
		/// </summary>
		protected bool jumpWhenButtonHeld;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Physics/Standard";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which is based loosely on real physics. If you run fast and jump you jump further than if you don't. It still allows changing direction in the air.";
		
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
		/// The index for the acceleration value in the movement data.
		/// </summary>
		protected const int AccelerationIndex = 0;
		
		/// <summary>
		/// The index for the max speed value in the movement data.
		/// </summary>
		protected const int MaxSpeedIndex = 1;
		
		/// <summary>
		/// The index for the drag index value in the movement data.
		/// </summary>
		protected const int DragIndex = 2;
		
		/// <summary>
		/// The index for use analgoue input in the movement data.
		/// </summary>
		protected const int UseAnalogueIndex = 3;

		/// <summary>
		/// The index for the ground leeay in the movement data.
		/// </summary>
		protected const int GroundedLeewayIndex = 4;

		/// <summary>
		/// The index for the ignore force value in the movement data.
		/// </summary>
		protected const int IgnoreForceIndex = 5;

		/// <summary>
		/// The index for the jump velocity value in the movement data.
		/// </summary>
		protected const int JumpVelocityIndex = 6;

		/// <summary>
		/// The index for can double jump in the movement data.
		/// </summary>
		protected const int CanDoubleJumpIndex = 7;
		
		/// <summary>
		/// The index for the double jump velocity in the movement data.
		/// </summary>
		protected const int DoubleJumpVelocityIndex = 8;

		/// <summary>
		/// The index for the Jump When Button Held in the movement data.
		/// </summary>
		protected const int JumpWhenButtonHeldIndex = 9;

		/// <summary>
		/// The index for the Jump Away setting in the movement data.
		/// </summary>
		protected const int JumpAwayIndex = 10;

		/// <summary>
		/// The index for the Max Double Jump Count in the movement data.
		/// </summary>
		protected const int MaxDoubleJumpCountIndex = 11;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 12;

		/// <summary>
		/// The default jump velocity.
		/// </summary>
		protected const float DefaultJumpVelocity = 10.0f;

		/// <summary>
		/// The default acceleration.
		/// </summary>
		protected const float  DefaultAcceleration = 60.0f;

		/// <summary>
		/// The default double jump velocity.
		/// </summary>
		protected const float DefaultDoubleJumpVelocity = 7.0f;

		/// <summary>
		/// The default max speed.
		/// </summary>
		protected const float DefaultMaxSpeed = 7.0f;

		
		/// <summary>
		/// The default drag.
		/// </summary>
		protected const float DefaultDrag = 5.0f;

		#endregion
		
		#region properties
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (showJumpStartedAnimation)
				{
					showJumpStartedAnimation = false;
					if (jumpCount > 1) return AnimationState.DOUBLE_JUMP;
					return AnimationState.JUMP;
				}
				else if (character.Velocity.y >= 0)
				{
					return AnimationState.AIRBORNE;
				}
				else
				{
					return AnimationState.FALL;
				}
			}
		}

		/// <summary>
		/// Gets the priority for the animation state.
		/// </summary>
		override public int AnimationPriority
		{
			get 
			{
				if (showJumpStartedAnimation)
				{
					return 5;
				}
				else if (character.Velocity.y >= 0)
				{
					return 0;
				}
				else
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// This overriden version always returns the input direction.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				return character.Input.HorizontalAxisDigital;
			}
		}

		/// <summary>
		/// How does this movement use Velocity.
		/// </summary>
		override public VelocityType VelocityType
		{
			get
			{
				return VelocityType.WORLD;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// rotations to be calculated and applied by the character.
		/// </summary>
		override public bool ShouldDoRotations {
			get
			{
				return false;
			}
		}

		#endregion
		
		#region public methods

		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate the jump.
		/// </summary>
		/// <value><c>true</c> if this instance should jump; otherwise, <c>false</c>.</value>
		override public bool WantsJump()
		{
			if (!enabled) return false;
			// Pressing jump and on the ground or on a ladder (if the ladder wont allow us to jump then the ladder will retain control).
			if (((jumpCount == 0 && character.TimeSinceGroundedOrOnLadder <= jumpGroundedLeeway) || (canDoubleJump && (maxDoubleJumpCount == -1 || jumpCount < maxDoubleJumpCount))) && 
			    character.Input.JumpButton == ButtonState.DOWN && !jumpStart)
			{
				return true;
			}
			return false;
		}


		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			DoRotation ();
			// If we left the ground move state to JUMPING
			if (jumpStart) 
			{
				// Jump has started once we leave the ground OR if we have hit our head (i.e. velocity back to zero or less)
				if (character.Velocity.y <= 0 || character.TimeSinceGroundedOrOnLadder > character.groundedLeeway) jumpStart = false;
				showJumpStartedAnimation = true;
			}
			MoveInX(character.Input.HorizontalAxis , character.Input.HorizontalAxisDigital, character.Input.RunButton);
			MoveInY();
		}
		
		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);
			
			// Set variables
			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				acceleration = movementData[AccelerationIndex].FloatValue;
				drag = movementData[DragIndex].FloatValue;
				maxSpeed = movementData[MaxSpeedIndex].FloatValue;
				useAnalogueInput = movementData[UseAnalogueIndex].BoolValue;
				jumpGroundedLeeway = character.groundedLeeway;
				ignoreForce = movementData[IgnoreForceIndex].FloatValue;
				jumpVelocity = movementData[JumpVelocityIndex].FloatValue;
				canDoubleJump = movementData[CanDoubleJumpIndex].BoolValue;
				doubleJumpVelocity = movementData[DoubleJumpVelocityIndex].FloatValue;
				jumpWhenButtonHeld = movementData[JumpWhenButtonHeldIndex].BoolValue;
				jumpAway = movementData[JumpAwayIndex].BoolValue;
				maxDoubleJumpCount = movementData[MaxDoubleJumpCountIndex].IntValue;
			}
#if UNITY_EDITOR
			else if (movementData != null && movementData.Length >= MovementVariableCount - 2)
			{
				Debug.LogWarning("Movement data for AirMovement_Physics didn't match, try selecting the movement in the inspector to automatically upgrade.");
				acceleration = movementData[AccelerationIndex].FloatValue;
				drag = movementData[DragIndex].FloatValue;
				maxSpeed = movementData[MaxSpeedIndex].FloatValue;
				useAnalogueInput = movementData[UseAnalogueIndex].BoolValue;
				jumpGroundedLeeway = character.groundedLeeway;
				ignoreForce = movementData[IgnoreForceIndex].FloatValue;
				jumpVelocity = movementData[JumpVelocityIndex].FloatValue;
				canDoubleJump = movementData[CanDoubleJumpIndex].BoolValue;
				doubleJumpVelocity = movementData[DoubleJumpVelocityIndex].FloatValue;
				jumpWhenButtonHeld = movementData[JumpWhenButtonHeldIndex].BoolValue;
				// The new values
				jumpAway = false;
				maxDoubleJumpCount = canDoubleJump ? 2 : 1;
			}
#endif
			else 
			{
				Debug.LogError("Invalid movement data.");
			}

			return this;
		}
		
		/// <summary>
		/// If the jump just started force control.
		/// </summary>
		override public bool ForceMaintainControl()
		{
			if (!enabled) return false;
			if (jumpStart) return true;
			return false;
		}
		
		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			if (character.ActiveMovement.VelocityType == VelocityType.RELATIVE_X_WORLD_Y)
			{
				Vector2 newVelocity = Quaternion.Euler(0,0, -character.PreviousRotation) * character.Velocity;
				character.SetVelocityX(newVelocity.x);
				// TODO Make this a constant and think about it a bit more
				character.SetVelocityY(newVelocity.y / 1.2f);
			}
			else if (character.ActiveMovement.VelocityType == VelocityType.WORLD)
			{
				// Same as we use, no need to change
			}
			hasRecievedCounterInput = false;
			if (character.Velocity.x > 0) initialDirection = 1;
			else if (character.Velocity.x < 0) initialDirection = -1;
			else initialDirection = 0;
		}

		/// <summary>
		/// Called when the movement loses control. Reset the jump count.
		/// </summary>
		override public void LosingControl()
		{
			jumpCount = 0;
			jumpStart = false;
		}

		/// <summary>
		/// This class will handle gravity internally.
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
		/// Do the X movement.
		/// </summary>
		override protected void MoveInX (float horizontalAxis, int horizontalAxisDigital, ButtonState runButton)
		{
			// Check holding direction
			if (!hasRecievedCounterInput)
			{
				if (initialDirection == 0 || horizontalAxisDigital == -initialDirection || Mathf.Abs (character.Velocity.x) <= maxSpeed) hasRecievedCounterInput = true;
			}

			// Apply drag (we use a friction like equation which seems to look better than a drag like one)
			if (character.Velocity.x > 0) 
			{
				character.AddVelocity(-character.Velocity.x * drag * TimeManager.FrameTime, 0, true);
				if (character.Velocity.x < 0) character.SetVelocityX(0);
			}
			else if (character.Velocity.x < 0) 
			{
				character.AddVelocity(-character.Velocity.x * drag * TimeManager.FrameTime, 0, true);
				if (character.Velocity.x > 0) character.SetVelocityX(0);
			}

			// If character not holding in initial direction or moving faster than max
			if (hasRecievedCounterInput)
			{
				// Apply acceleration
				if (Mathf.Abs (horizontalAxisDigital * acceleration) > ignoreForce)
				{
					character.AddVelocity((useAnalogueInput ? horizontalAxis : (float)horizontalAxisDigital) * acceleration * TimeManager.FrameTime, 0, true);
				}

				// Limit to max speed
				if (character.Velocity.x > maxSpeed) 
				{
					character.SetVelocityX(maxSpeed);
				}
				else if (character.Velocity.x < -maxSpeed) 
				{
					character.SetVelocityX(-maxSpeed);
				}
			}
			// Translate
			character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, true);

		}
		
		/// <summary>
		/// Do the Y movement.
		/// </summary>
		override protected void MoveInY ()
		{
			// Apply gravity
			if (!character.Grounded || character.Velocity.y > 0)
			{
				character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
			}
			// Translate
			character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
		}
		
		/// <summary>
		///  Do the jump by translating and applying velocity.
		/// </summary>
		override public void DoJump()
		{
			jumpStart = true;
			jumpCount++;
			// If we are not grounded this MUST be a double jump
			if (canDoubleJump && character.TimeSinceGroundedOrOnLadder > jumpGroundedLeeway && jumpCount < 2) jumpCount = 2;

			if (jumpCount > 1 && (maxDoubleJumpCount == -1 || jumpCount <= maxDoubleJumpCount))
			{
				Vector2 velocity = new Vector2(character.Velocity.x, doubleJumpVelocity);
				character.SetVelocityX(velocity.x);
				character.SetVelocityY(velocity.y);
			}
			else 
			{
				Vector2 velocity = new Vector2(character.Velocity.x, jumpVelocity);
				if (jumpAway) velocity = Quaternion.Euler(0, 0, character.transform.rotation.eulerAngles.z) * velocity;
				character.SetVelocityX(velocity.x);
				character.SetVelocityY(velocity.y);
			}

			if (character.Velocity.x > 0) initialDirection = 1;
			else if (character.Velocity.x < 0) initialDirection = -1;
			else initialDirection = 0;
			hasRecievedCounterInput = false;
		}

		/// <summary>
		/// Does a jump with overriden values for the key variables. Primarily used to allow
		/// platforms and wall jumps to affect jump height in non-physics based jumps.
		/// </summary>
		/// <param name="newHeight">New height.</param>
		/// <param name="jumpCount">Jump count.</param>
		override public void DoOverridenJump(float newHeight, int jumpCount, bool skipPowerUps = false)
		{
			jumpStart = true;
			this.jumpCount = jumpCount;
			float initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity  * newHeight);
			Vector2 velocity = new Vector2(character.Velocity.x, initialVelocity);
			//				velocity = Quaternion.Euler(0, 0, character.transform.rotation.eulerAngles.z) * velocity;
			character.SetVelocityX(velocity.x);
			character.SetVelocityY(velocity.y);

			if (character.Velocity.x > 0) initialDirection = 1;
			else if (character.Velocity.x < 0) initialDirection = -1;
			else initialDirection = 0;
			hasRecievedCounterInput = false;
		}

		/// <summary>
		/// Rotate towards the target rotation.
		/// </summary>
		virtual protected void DoRotation()
		{
			// Determine the point we will rotate around
			float difference  = 0 - character.transform.eulerAngles.z;
			// Shouldn't really happen but just in case
			if (difference > 180) difference = difference - 360;
			if (difference < -180) difference = difference + 360;
			
			Vector3 rotateAround = character.transform.position;

			if (difference >  character.rotationSpeed * TimeManager.FrameTime) difference =  character.rotationSpeed * TimeManager.FrameTime;
			if (difference < - character.rotationSpeed * TimeManager.FrameTime) difference = - character.rotationSpeed * TimeManager.FrameTime;
			character.transform.RotateAround(rotateAround, new Vector3(0,0,1), difference);
		}

		#endregion

#if UNITY_EDITOR

		#region draw inspector
		
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData != null && (movementData.Length == MovementVariableCount - 1 || movementData.Length == MovementVariableCount - 2))
			{
				Debug.LogWarning("Upgrading AirMovement_Digital movement data. Double check new values.");
				MovementVariable[] tmp = movementData;
				movementData = new MovementVariable[MovementVariableCount];
				System.Array.Copy(tmp, movementData, tmp.Length);
			}
			else if (movementData == null || movementData.Length != MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			EditorGUILayout.LabelField ("Jump Settings", EditorStyles.boldLabel);

			// Jump Velocity
			if (movementData[JumpVelocityIndex] == null) movementData[JumpVelocityIndex] = new MovementVariable(DefaultJumpVelocity);
			movementData[JumpVelocityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Jump Velocity", "The speed the character jumps at."), movementData[JumpVelocityIndex].FloatValue);
			if (movementData[JumpVelocityIndex].FloatValue < 0) movementData[JumpVelocityIndex].FloatValue = 0.0f;

			// Can double-jump
			if (movementData[CanDoubleJumpIndex] == null) movementData[CanDoubleJumpIndex] = new MovementVariable();
			movementData[CanDoubleJumpIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Can Double-jump", "Can the character double-jump?"), movementData[CanDoubleJumpIndex].BoolValue);

			// Double-jump settings
			if (movementData[DoubleJumpVelocityIndex] == null) movementData[DoubleJumpVelocityIndex] = new MovementVariable(DefaultDoubleJumpVelocity);
			if (movementData[MaxDoubleJumpCountIndex] == null) movementData[MaxDoubleJumpCountIndex] = new MovementVariable(2);
			if (movementData[CanDoubleJumpIndex].BoolValue)
			{
				// Double-jump height
				movementData[DoubleJumpVelocityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Double Jump Velocity", "The speed the character double jumps at."), movementData[DoubleJumpVelocityIndex].FloatValue);
				if (movementData[DoubleJumpVelocityIndex].FloatValue < 0) movementData[DoubleJumpVelocityIndex].FloatValue = 0.0f;

				if (EditorGUILayout.Toggle(new GUIContent("Infinite Double Jumps", "Can the character jump in the air forever?"), movementData[MaxDoubleJumpCountIndex].IntValue == -1))
				{
					movementData[MaxDoubleJumpCountIndex].IntValue = -1;
				}
				else 
				{
					// Max Double Jump Count
					movementData[MaxDoubleJumpCountIndex].IntValue = 1 + EditorGUILayout.IntField(new GUIContent("Max Jumps", "How many times can the character jump while in the air."), movementData[MaxDoubleJumpCountIndex].IntValue - 1);
					if (movementData[MaxDoubleJumpCountIndex].IntValue < 2) movementData[MaxDoubleJumpCountIndex].IntValue = 2;
				}
			}
			
			// Jump When Button Held Index
			if (movementData[JumpWhenButtonHeldIndex] == null) movementData[JumpWhenButtonHeldIndex] = new MovementVariable();
			movementData[JumpWhenButtonHeldIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Jump When Button Held", "Does holding the jump button jump automatically or does the user need to press it each time."), movementData[JumpWhenButtonHeldIndex].BoolValue);
			
			// Jump Away
			if (movementData[JumpAwayIndex] == null) movementData[JumpAwayIndex] = new MovementVariable();
			movementData[JumpAwayIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Jump Away", "If true character will jump away from the slope angle they are on, false and they jump straight up."), movementData[JumpAwayIndex].BoolValue);
			

			GUILayout.Space (4);

			EditorGUILayout.LabelField ("Horizontal Settings", EditorStyles.boldLabel);

			// Max Speed
			if (movementData[MaxSpeedIndex] == null) movementData[MaxSpeedIndex] = new MovementVariable(DefaultMaxSpeed);
			movementData[MaxSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Maximum Speed", "The characters peak speed in x."), movementData[MaxSpeedIndex].FloatValue);
			if (movementData[MaxSpeedIndex].FloatValue < 0) movementData[MaxSpeedIndex].FloatValue = 0.0f;
			
			// Acceleration
			if (movementData[AccelerationIndex] == null) movementData[AccelerationIndex] = new MovementVariable(DefaultAcceleration);
			movementData[AccelerationIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Acceleration", "How fast the character accelerates."), movementData[AccelerationIndex].FloatValue);
			if (movementData[AccelerationIndex].FloatValue < 0) movementData[AccelerationIndex].FloatValue = 0.0f;
			
			// Drag
			if (movementData[DragIndex] == null) movementData[DragIndex] = new MovementVariable(DefaultDrag);
			movementData[DragIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Drag Index", "Drag to apply."), movementData[DragIndex].FloatValue);
			if (movementData[DragIndex].FloatValue < 0) movementData[DragIndex].FloatValue = 0.0f;
			
			// Use Analog
			if (movementData[UseAnalogueIndex] == null) movementData[UseAnalogueIndex] = new MovementVariable();
			movementData[UseAnalogueIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Use Analogue Input", "Should the characters acceleration be affected by how hard/far the character presses the controller?"), movementData[UseAnalogueIndex].BoolValue);

			return movementData;
		}
		
		#endregion

#endif

	}
	
}