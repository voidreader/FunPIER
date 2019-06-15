#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An air movement with no changing direciton in the air.
	/// </summary>
	public class AirMovement_DigitalNoDirChange : AirMovement
	{
		
		#region members
		
		/// <summary>
		/// The (horizontal) speed the character moves at in the air.
		/// </summary>
		[TaggedProperty ("speedLimit")]
		[TaggedProperty ("agility")]
		public float airSpeed;
		
		/// <summary>
		/// How high the character jumps.
		/// </summary>
		[TaggedProperty ("jumpHeight")]
		[TaggedProperty ("agility")]
		public float jumpHeight;

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
		/// How long after pressing jump before jump occurs (only applied when standing still).
		/// </summary>
		public float standingJumpDelay;

		/// <summary>
		/// Derived initial velocity based on jumpHeight and relativeGravity.
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
		/// Automatically jump when the character holds the jump button down.
		/// </summary>
		protected bool jumpWhenButtonHeld;

		/// <summary>
		/// Jump count, set to 0 while not jumping, 1 while jumping.
		/// </summary>
		protected int jumpCount;

		/// <summary>
		/// Direction this jump started in.
		/// </summary>
		protected int initialJumpDirection;

		/// <summary>
		/// Where we expect the jump to peak.
		/// </summary>
		protected float expectedJumpPeak;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Digital/No Direction Change";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which has a fixed jump and no changing of direction in the air.";
		
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
		/// The default height of the jump.
		/// </summary>
		protected const float DefaultJumpHeight = 2.0f;

		/// <summary>
		/// The default relative jump gravity.
		/// </summary>
		protected const float DefaultRelativeJumpGravity = 1.0f;

		/// <summary>
		/// The default grounded leeway.
		/// </summary>
		protected const float DefaultGroundedLeeway = 0.1f;

		/// <summary>
		/// The default standing jump delay.
		/// </summary>
		protected const float DefaultStandingJumpDelay = 0.0f;

		/// <summary>
		/// The index for the air speed value in the movement data.
		/// </summary>
		protected const int AirSpeedIndex = 0;
		
		/// <summary>
		/// The index for the air speed value in the movement data.
		/// </summary>
		protected const int JumpHeightIndex = 1;
		
		/// <summary>
		/// The index for the value of show jumped details. Only used by editor.
		/// </summary>
		protected const int ShowJumpDetailsIndex = 2;
		
		/// <summary>
		/// The index for the relative jump gravity in the movement data.
		/// </summary>
		protected const int JumpRelativeGravityIndex = 3;
		
		/// <summary>
		/// The index for the ground leeway in the movement data.
		/// </summary>
		protected const int GroundedLeewayIndex = 4;

		/// <summary>
		/// The index for the Jump When Button Held in the movement data.
		/// </summary>
		protected const int JumpWhenButtonHeldIndex = 5;

		/// <summary>
		/// The index for the Stanindg Jump Delay in the movement data.
		/// </summary>
		protected const int StandingJumpDelayIndex = 6;

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
			if (((jumpCount == 0 && character.TimeSinceGroundedOrOnLadder <= groundedLeeway)) && character.Input.VerticalAxisDigital != -1 &&
			    (character.Input.JumpButton == ButtonState.DOWN || (jumpWhenButtonHeld && character.Input.JumpButton == ButtonState.HELD)) && !jumpStart)
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
				if (character.TimeSinceGroundedOrOnLadder > groundedLeeway || character.Velocity.y <= 0.0f) jumpStart = false;
				showJumpStartedAnimation = true;
			}
			MoveInX(0.0f, 0, ButtonState.NONE);
			MoveInY();
		}

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			this.character = character;

			// Calculate initial velocity
			initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * relativeJumpGravity * jumpHeight);

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
			
			// Set variables
			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				airSpeed = movementData[AirSpeedIndex].FloatValue;
				jumpHeight = movementData[JumpHeightIndex].FloatValue;
				relativeJumpGravity = movementData[JumpRelativeGravityIndex].FloatValue;
				groundedLeeway = movementData[GroundedLeewayIndex].FloatValue;
				jumpWhenButtonHeld = movementData[JumpWhenButtonHeldIndex].BoolValue;
				standingJumpDelay = movementData[StandingJumpDelayIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			
			// Calculate initial velocity
			initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * relativeJumpGravity * jumpHeight);

			return this;
		}
		
		/// <summary>
		/// If the jump just started force control.
		/// </summary>
		override public bool WantsControl()
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
			initialJumpDirection = character.Input.HorizontalAxisDigital;
		}

		/// <summary>
		/// Called when the movement loses control. Reset the jump count.
		/// </summary>
		override public void LosingControl()
		{
			jumpCount = 0;
			initialJumpDirection = 0;
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
					showJumpStartedAnimation = false;
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
		/// This overriden version always returns the initial jump direction.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				return initialJumpDirection;
			}
		}
		
		#endregion
		
		#region protected methods
		
		/// <summary>
		/// Does the X movement.
		/// </summary>
		override protected void MoveInX (float horizontalAxis, int horizontalAxisDigital, ButtonState runButton)
		{
			if (initialJumpDirection == 1)
			{
				character.SetVelocityX(airSpeed);
				character.Translate(airSpeed * TimeManager.FrameTime, 0, true);
			}
			else if (initialJumpDirection == -1)
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
				if (character.Velocity.y <= 0.0f || expectedJumpPeak - character.transform.position.y <= 0)
				{
					character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
				}
				// Jumping
				else
				{
					// Keep recalculating velocity to account for floating point discrepancies, grounded discrepancies, etc
					float requiredVelocity = Mathf.Sqrt(-2 * (expectedJumpPeak - character.transform.position.y) * character.DefaultGravity * relativeJumpGravity);
					if ((requiredVelocity * TimeManager.FrameTime) < 0.001f)
					{
						// Ensure we move at least 0.001f
						requiredVelocity = (0.001f / TimeManager.FrameTime);
					}
					character.SetVelocityY(requiredVelocity);
				}
			}
			else if (!jumpStart)
			{
				character.SetVelocityY(0);
			}
			// Translate
			character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
		}
		
		/// <summary>
		///  Do the jump by translating and applying velocity.
		/// </summary>
		override public void DoJump()
		{
			// If we allow jump height to be changed using tagged properties we need to recalcualte this
			initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * relativeJumpGravity * jumpHeight);

			initialJumpDirection = character.Input.HorizontalAxisDigital;
			//jumpStart = true;
			// jumpCount++;
			// If we are not grounded this MUST be a double jump
			if (character.TimeSinceGroundedOrOnLadder > groundedLeeway) jumpCount = 2;
			if (character.Input.HorizontalAxisDigital == 0 && standingJumpDelay > 0.0f)
			{
				StartCoroutine (DelayedJump());
			}
			else
			{
				jumpStart = true;
				jumpCount++;
				expectedJumpPeak = character.transform.position.y + jumpHeight;
				character.SetVelocityY(initialVelocity);
			}
		}

		/// <summary>
		/// Do the jump with overriden height and jumpCount.
		/// </summary>
		override public void DoOverridenJump(float newHeight, int newJumpCount)
		{
			jumpStart = true;
			jumpCount = newJumpCount;
			float newVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * relativeJumpGravity * newHeight);
			expectedJumpPeak = character.transform.position.y + newHeight;
			character.SetVelocityY(newVelocity);
		}
		
		/// <summary>
		/// Does a jump after a delay. Used to play a jump animation before jumping.
		/// </summary>
		/// <returns>The jump.</returns>
		virtual protected IEnumerator DelayedJump() {
			showJumpStartedAnimation = true;
			yield return new WaitForSeconds (standingJumpDelay);
			jumpStart = true;
			jumpCount++;
			expectedJumpPeak = character.transform.position.y + jumpHeight;
			character.SetVelocityY(initialVelocity);
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
			
			// Air speed
			if (movementData[AirSpeedIndex] == null) movementData[AirSpeedIndex] = new MovementVariable(DefaultAirSpeed);
			movementData[AirSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Air Speed (x)", "How fast the character moves in the X direction whle in the air."), movementData[AirSpeedIndex].FloatValue);
			if (movementData[AirSpeedIndex].FloatValue < 0) movementData[AirSpeedIndex].FloatValue = 0.0f;
			
			// Jump height
			if (movementData[JumpHeightIndex] == null) movementData[JumpHeightIndex] = new MovementVariable(DefaultJumpHeight);
			movementData[JumpHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Jump Height", "How high the character jumps"), movementData[JumpHeightIndex].FloatValue);
			if (movementData[JumpHeightIndex].FloatValue < 0) movementData[JumpHeightIndex].FloatValue = 0.0f;

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

			return movementData;
		}
		
		#endregion
#endif
	}

}