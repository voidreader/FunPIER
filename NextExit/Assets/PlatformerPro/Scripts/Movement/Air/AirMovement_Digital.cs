#if UNITY_EDITOR
using UnityEditor;
using PlatformerPro.Validation;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Air movement digital.
	/// </summary>
	public class AirMovement_Digital : AirMovement, IFlippableGravityMovement
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
		/// Can the character double jump.
		/// </summary>
		[TaggedProperty ("canDoubleJump")]
		public bool canDoubleJump;
		
		/// <summary>
		/// How high the character jumps on a double jump.
		/// </summary>
		[TaggedProperty ("agility")]
		[TaggedProperty ("jumpHeight")]
		[TaggedProperty ("doubleJumpHeight")]
		public float doubleJumpHeight;

		/// <summary>
		/// How many times the character can double jump. -1 means infinite.
		/// </summary>
		[TaggedProperty ("maxDoubleJumpCount")]
		public int maxDoubleJumpCount;

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
		/// Jump count, set to 0 while not jumping, 1 while jumping, 2 while double 
		/// </summary>
		protected int jumpCount;

		/// <summary>
		/// Derived initial velocity based on doubleJumpHeight and time.
		/// </summary>
		protected float doubleJumpInitialVelocity;

		/// <summary>
		/// Automatically jump when the character holds the jump button down.
		/// </summary>
		protected bool jumpWhenButtonHeld;

		/// <summary>
		/// Where we expect the jump to peak.
		/// </summary>
		protected float expectedJumpPeak;

		/// <summary>
		/// For double track if the jump button has been released. We don't double jump if the user just holds jump.
		/// </summary>
		protected bool jumpButtonReleased;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Digital/Basic";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which has a fixed jump height and fixed air speed.";
		
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
		/// The default can double jump value.
		/// </summary>
		protected const bool DefaultCanDoubleJump = false;

		/// <summary>
		/// The default height of the double jump.
		/// </summary>
		protected const float DefaultDoubleJumpHeight = 2.0f;

		/// <summary>
		/// The index for the air speed value in the movement data.
		/// </summary>
		protected const int AirSpeedIndex = 0;
		
		/// <summary>
		/// The index for the air speed value in the movement data.
		/// </summary>
		protected const int JumpHeightIndex = 1;
		
		/// <summary>
		/// The index for the jump curve value in the movement data.
		/// </summary>
		protected const int JumpCurveIndex = 2;
		
		/// <summary>
		/// The index for the value of show jumped details. Only used by editor.
		/// </summary>
		protected const int ShowJumpDetailsIndex = 3;
		
		/// <summary>
		/// The index for the relative jump gravity in the movement data.
		/// </summary>
		protected const int JumpRelativeGravityIndex = 4;
		
		/// <summary>
		/// The index for the ground leeway in the movement data.
		/// </summary>
		protected const int GroundedLeewayIndex = 5;

		/// <summary>
		/// The index for can double jump in the movement data.
		/// </summary>
		protected const int CanDoubleJumpIndex = 6;

		/// <summary>
		/// The index for the double jump height in the movement data.
		/// </summary>
		protected const int DoubleJumpHeightIndex = 7;

		/// <summary>
		/// The index for the Jump When Button Held in the movement data.
		/// </summary>
		protected const int JumpWhenButtonHeldIndex = 8;

		
		/// <summary>
		/// The index for the Max Double Jump Count in the movement data.
		/// </summary>
		protected const int MaxDoubleJumpCountIndex = 9;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 10;
		
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
		/// Gets a value indicating whether this movement wants to control the movement in the air.
		/// Default is false with movement falling back to default air. Override if you want control.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		/// <returns><c>true</c>, if air control was wantsed, <c>false</c> otherwise.</returns>
		override public bool WantsAirControl() {
			// Default air movement doesn't ever want air control, it gets it via the defaulting behaviour
			if (this == character.DefaultAirMovement) return false;
			if (jumpCount > 0 && !character.Grounded) return true;
			return false;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate the jump.
		/// </summary>
		/// <value><c>true</c> if this instance should jump; otherwise, <c>false</c>.</value>
		override public bool WantsJump()
		{
			if (!enabled) return false;
			// Pressing jump and on the ground or on a ladder (if the ladder wont allow us to jump then the ladder will retain control).
			if (((jumpCount == 0 && character.TimeSinceGroundedOrOnLadder <= groundedLeeway) || (canDoubleJump && (maxDoubleJumpCount == -1 || jumpCount < maxDoubleJumpCount))) && 
			    (character.Input.JumpButton == ButtonState.DOWN || (jumpCount == 0 && character.Grounded && jumpWhenButtonHeld && character.Input.JumpButton == ButtonState.HELD)) && !jumpStart)
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
			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				airSpeed = movementData[AirSpeedIndex].FloatValue;
				jumpHeight = movementData[JumpHeightIndex].FloatValue;
				relativeJumpGravity = movementData[JumpRelativeGravityIndex].FloatValue;
				groundedLeeway = movementData[GroundedLeewayIndex].FloatValue;
				canDoubleJump = movementData[CanDoubleJumpIndex].BoolValue;
				doubleJumpHeight = movementData[DoubleJumpHeightIndex].FloatValue;
				jumpWhenButtonHeld = movementData[JumpWhenButtonHeldIndex].BoolValue;
				maxDoubleJumpCount = movementData[MaxDoubleJumpCountIndex].IntValue;
			}
			else
			{
				Debug.LogError("Invalid movement data (GO: " + this.gameObject.name + "  Movement: " + this.GetType().Name + ")");
#if UNITY_EDITOR
				if (movementData != null && movementData.Length == MovementVariableCount - 1)
				{
					Debug.Log("Open the movement in the inspector to auto upgrade.");
				}
#endif
			}
			
			// Calculate initial velocity
			initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * relativeJumpGravity * jumpHeight);
			doubleJumpInitialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * relativeJumpGravity * doubleJumpHeight);

			AddGravityFlipHandler ();

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
		/// Called when the movement loses control. Reset the jump count.
		/// </summary>
		override public void LosingControl()
		{
			jumpCount = 0;
			jumpStart = false;
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
		/// This overriden version always returns the input direction.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (character.IsGravityFlipped) return -character.Input.HorizontalAxisDigital;
				return character.Input.HorizontalAxisDigital;
			}
		}
		
		#endregion
		
		#region protected methods
		
		/// <summary>
		/// Does the X movement.
		/// </summary>
		override protected void MoveInX (float horizontalAxis, int horizontalAxisDigital, ButtonState runButton)
		{
			if (horizontalAxisDigital == 1)
			{
				character.SetVelocityX(character.IsGravityFlipped ? -airSpeed : airSpeed);
				character.Translate((character.IsGravityFlipped ? -airSpeed : airSpeed) * TimeManager.FrameTime, 0, false);
			}
			else if (horizontalAxisDigital == -1)
			{
				character.SetVelocityX(character.IsGravityFlipped ? airSpeed : -airSpeed);
				character.Translate((character.IsGravityFlipped ? airSpeed : -airSpeed) * TimeManager.FrameTime, 0, false);
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
				if (character.Velocity.y <= 0.0f || jumpCount < 1 ||
				    (!character.IsGravityFlipped && (expectedJumpPeak - character.transform.position.y <= 0)) || 
				    (character.IsGravityFlipped && (character.transform.position.y - expectedJumpPeak <= 0)))
				{
					character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
				}
				// Jumping
				else
				{
					// Keep recalculating velocity to account for floating point discrepancies, grounded discrepancies, etc
					float requiredVelocity = Mathf.Sqrt(-2 * (character.IsGravityFlipped ? (character.transform.position.y - expectedJumpPeak) : (expectedJumpPeak - character.transform.position.y)) * character.DefaultGravity * relativeJumpGravity);
					if ((requiredVelocity * TimeManager.FrameTime) < 0.001f)
					{
						// Ensure we move at 0.001f or else we might get stuck
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
			initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * relativeJumpGravity * GetJumpHeightOrForce(jumpHeight));
			jumpStart = true;
			jumpCount++;
			// If we are not grounded this MUST be a double jump
			if (character.TimeSinceGroundedOrOnLadder > groundedLeeway && jumpCount < 2) jumpCount = 2;
			if (jumpCount > 1 && (maxDoubleJumpCount == -1 || jumpCount <= maxDoubleJumpCount))
			{
				expectedJumpPeak = character.transform.position.y + (character.IsGravityFlipped ? - GetDoubleJumpHeightOrForce(doubleJumpHeight) : GetDoubleJumpHeightOrForce(doubleJumpHeight));
				character.SetVelocityY(doubleJumpInitialVelocity);
			}
			else 
			{
				expectedJumpPeak = character.transform.position.y + (character.IsGravityFlipped ? - GetJumpHeightOrForce(jumpHeight) : GetJumpHeightOrForce(jumpHeight));
				character.SetVelocityY(initialVelocity);
			}
		}

		/// <summary>
		/// Do the jump with overriden height and jumpCount.
		/// </summary>
		override public void DoOverridenJump(float newHeight, int newJumpCount, bool skipPowerUps = false)
		{
			if (!skipPowerUps) newHeight = GetJumpHeightOrForce(newHeight);
			jumpStart = true;
			jumpCount = newJumpCount;
			float v = -2.0f * character.DefaultGravity * relativeJumpGravity * newHeight;
			if (v < 0) v = 0;
			float newVelocity = Mathf.Sqrt(v);
			expectedJumpPeak = character.transform.position.y + (character.IsGravityFlipped ? - newHeight : newHeight);
			character.SetVelocityY(newVelocity);
		}

		/// <summary>
		/// Handles the gravity being flipped.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		override protected void HandleGravityFlipped (object sender, System.EventArgs e)
		{
			if (maxDoubleJumpCount != -1)
			{
				jumpCount = maxDoubleJumpCount + 1;
			}
			else
			{
				jumpCount = 1;
			}
			expectedJumpPeak = transform.position.y;
		}

		#endregion

#if UNITY_EDITOR

		/// <summary>
		/// Custom movement validation.
		/// </summary>
		/// <returns>A list of validation errors</returns>
		/// <param name="c">Character to validate against.</param>
		override public List<ValidationResult> ValidateMovement(Character c)
		{
			List<ValidationResult> result = new List<ValidationResult> ();
			// Physics things that wont work with this movement
			if (FindObjectOfType(typeof(WindAffector)) != null)
			{
				result.Add(new ValidationResult("Note that WindAffectors wont correctly push airborne characters with a digital air movement. You may want to use a physics movement instead.", MessageType.Info));
			}
			return result;
		}

		#region draw inspector
		
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData != null  && movementData.Length == MovementVariableCount - 1)
			{
				MovementVariable[] tmp = movementData;
				movementData = new MovementVariable[MovementVariableCount];
				System.Array.Copy(tmp, movementData, MovementVariableCount - 1);
			}
			else if (movementData == null || movementData.Length < MovementVariableCount)
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

			// Can double-jump
			if (movementData[CanDoubleJumpIndex] == null) movementData[CanDoubleJumpIndex] = new MovementVariable(DefaultCanDoubleJump);
			movementData[CanDoubleJumpIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Can Double-jump", "Can the character double-jump?"), movementData[CanDoubleJumpIndex].BoolValue);

			// Double-jump settings
			if (movementData[DoubleJumpHeightIndex] == null) movementData[DoubleJumpHeightIndex] = new MovementVariable(DefaultDoubleJumpHeight);
			if (movementData[MaxDoubleJumpCountIndex] == null) movementData[MaxDoubleJumpCountIndex] = new MovementVariable(2);
			if (movementData[CanDoubleJumpIndex].BoolValue)
			{
				// Double-jump height
				movementData[DoubleJumpHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Double Jump Height", "How high the character jumps on a double-jump."), movementData[DoubleJumpHeightIndex].FloatValue);
				if (movementData[DoubleJumpHeightIndex].FloatValue < 0) movementData[DoubleJumpHeightIndex].FloatValue = 0.0f;

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