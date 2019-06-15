#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An air movement that lets mecanim to most of the work.
	/// </summary>
	public class AirMovement_MecanimDrivenAnimation : AirMovement
	{
		
		#region members
		
		/// <summary>
		/// The (horizontal) speed the character moves at in the air. Sets velocity only.
		/// </summary>
		public float airSpeed;
		
		/// <summary>
		/// After the user leaves the ground how much additional time do we give the user to press jump 
		/// and still consider them grounded.
		/// </summary>
		public float groundedLeeway;
		
		/// <summary>
		/// Character can only jump if this timer is less than zero.
		/// </summary>
		protected float readyToJumpTimer;
		
		/// <summary>
		/// True before the jump leaves the ground.
		/// </summary>
		protected bool jumpStart;

		protected bool isJumping;

		/// <summary>
		/// Jump count, set to 0 while not jumping, 1 while jumping, 2 while double 
		/// </summary>
		protected int jumpCount;

		protected Animator myAnimator;

		protected bool isFalling;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Mecanim/Animation Driven/Basic";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which lets Mecanim do the work.";
		
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
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 9;
		
		#endregion

		#region properties
		
		/// <summary>
		/// It is expected movements will have gravity built in.
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
				return character.DefaultGravity;
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
			if ((jumpCount == 0 && character.TimeSinceGroundedOrOnLadder <= groundedLeeway) && character.Input.JumpButton == ButtonState.DOWN  && !jumpStart)
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
				// Jump has moved beyond start once we leave the ground
				if (character.TimeSinceGroundedOrOnLadder > groundedLeeway) 
				{
					isJumping = true;
					jumpStart = false;
				}
			}
			// If we aren't starting a jump or jumping we must be falling
			else if (!isJumping)
			{
				isFalling = true;
			}

			// If we started plating the fall animation we must be falling
			AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
			if ((jumpStart || isJumping) && info.IsName(AnimationState.FALL.AsString()))
			{
				jumpStart = false;
				isJumping = false;
				isFalling = true;
				character.SetVelocityY(-8);
			}

			if (character.Grounded && (isFalling || isJumping)) 
			{
				isJumping = false;
				isFalling = false;
			}

			// Only move in x once ground is left
			if (isJumping || isFalling)
			{

				MoveInX(character.Input.HorizontalAxis , character.Input.HorizontalAxisDigital, character.Input.RunButton);
			}

			// Only add height if jumping up
			if (isJumping && !isFalling)
			{
				myAnimator.MatchTarget(myAnimator.transform.position + new Vector3(0, (TimeManager.FrameTime * 50.0f), 0),
				                       myAnimator.transform.rotation, 
				                       AvatarTarget.Root,
				                       new MatchTargetWeightMask(new Vector3(0, 0.5f, 0), 0), 
				                       0.2f,
				                       0.4f);
			}
			else if (isFalling)
			{
				// Apply gravity
				character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
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
			myAnimator = GetComponentInChildren<Animator> ();

			// Set variables
			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				airSpeed = movementData[AirSpeedIndex].FloatValue;
				groundedLeeway = movementData[GroundedLeewayIndex].FloatValue;
			}
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
		/// Called when the movement loses control. Reset the jump count.
		/// </summary>
		override public void LosingControl()
		{
			jumpCount = 0;
			isJumping = false;
			jumpStart = false;
			isFalling = false;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (jumpStart || isJumping) return AnimationState.JUMP;
				return AnimationState.FALL;
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
		/// Does the X movement.
		/// </summary>
		override protected void MoveInX (float horizontalAxis, int horizontalAxisDigital, ButtonState runButton)
		{
			if (character.Input.HorizontalAxisDigital == 1)
			{
				character.SetVelocityX(character.IsGravityFlipped ? -airSpeed : airSpeed);
				character.Translate((character.IsGravityFlipped ? -airSpeed : airSpeed) * TimeManager.FrameTime, 0, false);
			}
			else if (character.Input.HorizontalAxisDigital == -1)
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
		///  Do the jump by translating and applying velocity.
		/// </summary>
		override public void DoJump()
		{
			jumpStart = true;
			jumpCount++;
		}

		/// <summary>
		/// Do the jump with overriden height and jumpCount.
		/// </summary>
		override public void DoOverridenJump(float newHeight, int newJumpCount, bool skipPowerUps = false)
		{
			Debug.LogWarning ("This movement does not support overriden jump");
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

			showDetails = EditorGUILayout.Foldout(showDetails, "Details");
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