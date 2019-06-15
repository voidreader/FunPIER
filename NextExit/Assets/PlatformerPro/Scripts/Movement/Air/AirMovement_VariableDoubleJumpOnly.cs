#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An air movement that has a variable height jump but only works when already in the air.
	/// </summary>
	public class AirMovement_VariableDoubleJumpOnly : AirMovement
	{
		
		#region members
		
		/// <summary>
		/// The (horizontal) speed the character moves at in the air.
		/// </summary>
		[TaggedProperty ("speedLimit")]
		[TaggedProperty ("agility")]
		public float airSpeed;
		
		/// <summary>
		/// Smallest height the character can jump
		/// </summary>
		public float minJumpHeight;

		/// <summary>
		/// Largest height the character can jump.
		/// </summary>
		[TaggedProperty ("jumpHeight")]
		[TaggedProperty ("agility")]
		public float maxJumpHeight;

		/// <summary>
		/// How long the character has to hold the jump button to jump the max height.
		/// </summary>
		public float maxJumpAccelerationTime;

		/// <summary>
		/// The gravity applied during jump (relative to the characters Gravity component).
		/// </summary>
		public float relativeJumpGravity;
		
		/// <summary>
		/// After the user leaves the ground how much additional time do we give the user to press jump 
		/// and still consider them grounded.
		/// </summary>
		public float groundedLeeway;

		/// <summary>
		/// Automatically jump when the character holds the jump button down.
		/// </summary>
		public bool jumpWhenButtonHeld;

		/// <summary>
		/// Derived initial velocity based on minJumpHeight and relativeGravity.
		/// </summary>
		protected float initialVelocity;
		
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
		/// Times how long player has held Jump button.
		/// </summary>
		protected float jumpHeldTimer;

		/// <summary>
		/// Track where the last jump started so we can calculate speed that should be set each frame.
		/// </summary>
		protected float startingYPosition;

		/// <summary>
		/// The actual height of the max jump this can change when for example we get an overriden jump from
		/// something like a springbaord platform.
		/// </summary>
		protected float actualMaxJumpHeight;

		/// <summary>
		/// The jump count.
		/// </summary>
		protected int jumpCount;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Digital/Variable Height Double Jump Only";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which has a variable jump height and fixed air speed. It can only be used as a double jump in conjunction with another movement. Should not be the default.";
		
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
		/// The default air speed.
		/// </summary>
		protected const float DefaultAirSpeed = 5.0f;

		/// <summary>
		/// The default min height of the jump.
		/// </summary>
		protected const float DefaultMinJumpHeight = 0.75f;

		/// <summary>
		/// The default max height of the jump.
		/// </summary>
		protected const float DefaultMaxJumpHeight = 2.5f;

		/// <summary>
		/// The default relative jump gravity.
		/// </summary>
		protected const float DefaultRelativeJumpGravity = 1.5f;

		/// <summary>
		/// The default grounded leeway.
		/// </summary>
		protected const float DefaultGroundedLeeway = 0.1f;

		/// <summary>
		/// The default max jump acceleration time.
		/// </summary>
		protected const float DefaultMaxJumpAccelerationTime = 0.25f;

		/// <summary>
		/// The index for the air speed value in the movement data.
		/// </summary>
		protected const int AirSpeedIndex = 0;
		
		/// <summary>
		/// The index for the min jump height in the movement data.
		/// </summary>
		protected const int MinJumpHeightIndex = 1;

		/// <summary>
		/// The index for the max jump height in the movement data.
		/// </summary>
		protected const int MaxJumpHeightIndex = 2;

		/// <summary>
		/// The index for the Max Jump Accereration in the movement data.
		/// </summary>
		protected const int MaxJumpAccelerationTimeIndex = 3;

		/// <summary>
		/// The index for the relative jump gravity in the movement data.
		/// </summary>
		protected const int JumpRelativeGravityIndex = 4;
		
		/// <summary>
		/// The index for the ground leeway in the movement data.
		/// </summary>
		protected const int GroundedLeewayIndex = 5;

		/// <summary>
		/// The index for the Jump When Button Held in the movement data.
		/// </summary>
		protected const int JumpWhenButtonHeldIndex = 6;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 7;
		
		#endregion
		
		#region properties
		
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
		
		
		/// <summary>
		/// Gets a value indicating the current gravity, only used if this
		/// movement doesn't apply the default gravity.
		/// <seealso cref="ShouldApplyGravity()"/>
		/// </summary>
		override public float CurrentGravity
		{
			get
			{
				// Falling
				if (character.Velocity.y < 0.0f)
				{
					return character.DefaultGravity;
				}
				// Jumping
				else
				{
					return character.DefaultGravity * relativeJumpGravity;
				}
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate the jump.
		/// </summary>
		/// <value><c>true</c> if this instance should jump; otherwise, <c>false</c>.</value>
		override public bool WantsJump()
		{
			if (!enabled) return false;
			// Pressing jump and on the ground or on a ladder (if the ladder wont allow us to jump then the ladder will retain control).
			if (jumpCount == 0 && !character.Grounded && character.Input.JumpButton == ButtonState.DOWN && !jumpStart)
			{
				return true;
			}
			return false;
		}
		
		#endregion
		
		#region public methods
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// If we left the ground move state to JUMPING
			if (jumpStart) 
			{
				// Jump has moved beyond start once we leave the ground OR if we have hit our head (i.e. velocity back to zero or less)
				if (character.TimeSinceGroundedOrOnLadder > groundedLeeway || character.Velocity.y <= 0.0f) 
				{
					jumpStart = false;
					showJumpStartedAnimation = false;
				}
				else
				{
					showJumpStartedAnimation = true;
				}
			}
			MoveInX(character.Input.HorizontalAxis , character.Input.HorizontalAxisDigital, character.Input.RunButton);
			MoveInY();
		}
		
		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);
			
			// Set variables
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				airSpeed = movementData[AirSpeedIndex].FloatValue;
				minJumpHeight = movementData[MinJumpHeightIndex].FloatValue;
				maxJumpHeight = movementData[MaxJumpHeightIndex].FloatValue;
				maxJumpAccelerationTime = movementData[MaxJumpAccelerationTimeIndex].FloatValue;
				relativeJumpGravity = movementData[JumpRelativeGravityIndex].FloatValue;
				groundedLeeway = movementData[GroundedLeewayIndex].FloatValue;
				jumpWhenButtonHeld = movementData[JumpWhenButtonHeldIndex].BoolValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}

			// Calculate initial velocity and acceleration
			initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * relativeJumpGravity * minJumpHeight);
			
			return this;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement in the air.
		/// Default is false with movement falling back to default air. Override if you want control.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		/// <returns><c>true</c>, if air control was wantsed, <c>false</c> otherwise.</returns>
		override public bool WantsAirControl()
		{
			if (jumpHeldTimer > 0) return true;
			return false;
		}

		/// <summary>
		/// If the jump just started force control.
		/// </summary>
		override public bool ForceMaintainControl()
		{
			if (jumpStart) return true;
			return false;
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		/// <returns><c>true</c>, if control was losinged, <c>false</c> otherwise.</returns>
		override public void LosingControl()
		{
			jumpCount = 0;
			jumpStart = false;
			jumpHeldTimer = 0;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (showJumpStartedAnimation)
				{
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
		
		#endregion
		
		#region protected methods
		
		/// <summary>
		/// Do the X movement.
		/// </summary>
		override protected void MoveInX (float horizontalAxis, int horizontalAxisDigital, ButtonState runButton)
		{
			if (horizontalAxisDigital == 1)
			{
				character.SetVelocityX(airSpeed);
				character.Translate(airSpeed * TimeManager.FrameTime, 0, true);
			}
			else if (horizontalAxisDigital == -1)
			{
				character.SetVelocityX(-airSpeed);
				character.Translate(-airSpeed * TimeManager.FrameTime, 0, true);
			}
			else
			{
				character.SetVelocityX(0);
			}
		}
		
		/// <summary>
		/// Do the Y movement.
		/// </summary>
		override protected void MoveInY ()
		{
			// Apply gravity
			if (!character.Grounded || character.Velocity.y > 0)
			{
				// Falling
				if (character.Velocity.y < 0.0f)
				{
					character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
				}
				// Jumping
				else
				{
					character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity * relativeJumpGravity, false);
				}
				
			}
			else if (!jumpStart)
			{
				// Ground Movement should handle this
				// character.SetVelocityY(0);
			}
			// Set speed if player is holding jump
			if ((character.Input.JumpButton == ButtonState.DOWN || character.Input.JumpButton == ButtonState.HELD) && jumpHeldTimer > 0.0f && character.Velocity.y > 0.0f)
			{
				jumpHeldTimer -= TimeManager.FrameTime;
				float expectedJumpHeight = Mathf.Lerp (minJumpHeight, actualMaxJumpHeight, (maxJumpAccelerationTime - jumpHeldTimer) / maxJumpAccelerationTime);
				float relativeJumpHeight = expectedJumpHeight - (character.Transform.position.y - startingYPosition);
				if (relativeJumpHeight < 0)
				{
					character.SetVelocityY(0);
				} 
				else 
				{
					float newVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * relativeJumpGravity * relativeJumpHeight);
					character.SetVelocityY(newVelocity);
				}

			}
			// Player let go of jump so stop accelerating
			else if (character.Input.JumpButton != ButtonState.HELD && character.Input.JumpButton != ButtonState.DOWN )
			{
				jumpHeldTimer = 0.0f;
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
			jumpCount = 1;
			character.SetVelocityY(initialVelocity);
			jumpHeldTimer = maxJumpAccelerationTime;
			startingYPosition = character.Transform.position.y;
			actualMaxJumpHeight = maxJumpHeight;
		}

		/// <summary>
		/// Do the jump with overriden height and jumpCount.
		/// </summary>
		override public void DoOverridenJump(float newHeight, int ignoredJumpCount, bool skipPowerUps = false)
		{
			character.DefaultAirMovement.DoOverridenJump (newHeight, ignoredJumpCount);
		}

		#endregion

#if UNITY_EDITOR

		#region draw inspector
		
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}
			
			// Air speed
			if (movementData[AirSpeedIndex] == null) movementData[AirSpeedIndex] = new MovementVariable(DefaultAirSpeed);
			movementData[AirSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Air Speed (x)", "How fast the character moves in the X direction whle in the air."), movementData[AirSpeedIndex].FloatValue);
			if (movementData[AirSpeedIndex].FloatValue < 0) movementData[AirSpeedIndex].FloatValue = 0.0f;
			
			// Min Jump height
			if (movementData[MinJumpHeightIndex] == null) movementData[MinJumpHeightIndex] = new MovementVariable(DefaultMinJumpHeight);
			movementData[MinJumpHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Min Jump Height", "How high the character jumps if jump button is tapped."), movementData[MinJumpHeightIndex].FloatValue);
			if (movementData[MinJumpHeightIndex].FloatValue < 0) movementData[MinJumpHeightIndex].FloatValue = 0.0f;

			// Max Jump height
			if (movementData[MaxJumpHeightIndex] == null) movementData[MaxJumpHeightIndex] = new MovementVariable(DefaultMaxJumpHeight);
			movementData[MaxJumpHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Max Jump Height", "How high the character jumps if jump button is held for full jump acceleration time."), movementData[MaxJumpHeightIndex].FloatValue);
			if (movementData[MaxJumpHeightIndex].FloatValue < movementData[MinJumpHeightIndex].FloatValue) movementData[MaxJumpHeightIndex].FloatValue = movementData[MinJumpHeightIndex].FloatValue;

			// Jump Time
			if (movementData[MaxJumpAccelerationTimeIndex] == null) movementData[MaxJumpAccelerationTimeIndex] = new MovementVariable(DefaultMaxJumpAccelerationTime);
			movementData[MaxJumpAccelerationTimeIndex].FloatValue = EditorGUILayout.Slider(new GUIContent("Max Jump Time", "How long the player has to hold jump for the max height to be reached."), movementData[MaxJumpAccelerationTimeIndex].FloatValue, 0.01f, 1.0f);

			showDetails = EditorGUILayout.Foldout(showDetails, "Details");

			// Make sure we set defaults regardless of showDetails
			if (movementData[JumpRelativeGravityIndex] == null || movementData[JumpRelativeGravityIndex].FloatValue < 0.01f || movementData[JumpRelativeGravityIndex].FloatValue > 2.0f)
			{
				movementData[JumpRelativeGravityIndex] = new MovementVariable(DefaultRelativeJumpGravity);
			}
			if (movementData[GroundedLeewayIndex] == null)
			{
				movementData[GroundedLeewayIndex] = new MovementVariable(DefaultGroundedLeeway);
			}
			if (movementData[JumpWhenButtonHeldIndex] == null) movementData[JumpWhenButtonHeldIndex] = new MovementVariable();


			if (showDetails) 
			{
				movementData = AirMovement.DrawStandardJumpDetails(movementData, JumpRelativeGravityIndex, GroundedLeewayIndex, JumpWhenButtonHeldIndex);
			}

			// As this may not be set if show details are closed lets set it.
			if (movementData [JumpRelativeGravityIndex] == null) movementData [JumpRelativeGravityIndex] = new MovementVariable (DefaultRelativeJumpGravity);

			// Check for things that don't look right
			if (((movementData[MaxJumpHeightIndex].FloatValue - movementData[MinJumpHeightIndex].FloatValue) * movementData[MaxJumpAccelerationTimeIndex].FloatValue ) > 0.5f)  
			{
				EditorGUILayout.HelpBox("Your jump settings mean that your character may moves in an unnatural floaty way if jump is held. Consider reducing Max Jump Time or increasing the Min Jump Height.", MessageType.Warning);
				float betterTime = 0.5f / (movementData[MaxJumpHeightIndex].FloatValue - movementData[MinJumpHeightIndex].FloatValue);
				if (betterTime < 1 && betterTime >= 0.1f)
				{
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(string.Format("Set time to {0:0.00}", betterTime)))
					{
						movementData[MaxJumpAccelerationTimeIndex].FloatValue = betterTime;
					}
					GUILayout.EndHorizontal();
				}
			}

			// Check for things that don't look right
			if (movementData[JumpRelativeGravityIndex].FloatValue < 1.4f)  
			{
				EditorGUILayout.HelpBox("Variable height jumps tend to look more natural with a high relative jump gravity.", MessageType.Warning);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Set to " + DefaultRelativeJumpGravity))
				{
					movementData[JumpRelativeGravityIndex].FloatValue = DefaultRelativeJumpGravity;
				}
				GUILayout.EndHorizontal();
			}
			return movementData;
		}
		
		#endregion

#endif
	}

}