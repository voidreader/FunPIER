#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement that uses physics style movement and can support 360 degree loops.
	/// </summary>
	public class GroundMovement_PhysicsWithLoop: GroundMovement_Physics
	{

		#region members

		/// <summary>
		/// How fast we need to be going to loop.
		/// </summary>
		public float loopSpeed = 14.0f;

		/// <summary>
		/// How long the character slides down if they aren't looping fast enough.
		/// </summary>
		public float slideDownTime = 1.0f;

		/// <summary>
		/// Track  if the character has hit the loop at loop velocity.
		/// </summary>
		protected bool isLooping;

		/// <summary>
		/// While true ignore character horizontal input and let the character slide down.
		/// </summary>
		protected float slideDownTimer;

		/// <summary>
		/// When we start falling we may still be grounded due to rotation. Accordingly we can't rely
		/// on the characters y velocity so we temporarily track it here until we fall far enough for the air
		/// control to take over.
		/// </summary>
		protected float localFallVelocity;

		/// <summary>
		/// Should we show loop settings in editor.
		/// </summary>
		protected bool showLoopSettings;

		/// <summary>
		/// The auto loop direction. 0 for NONE.
		/// </summary>
		protected int autoLoopDirection;

		/// <summary>
		/// The direciton we are sliding down in.
		/// </summary>
		protected int slideDownDirection;


		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Physics/With Loops";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which uses drag and acceleration to give physics like movement and can support 360 degree loops.";
		
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
		/// The index for the loop speed value in the movement data.
		/// </summary>
		protected const int LoopSpeedIndex = 8;

		/// <summary>
		/// The index for the slide time value in the movement data.
		/// </summary>
		protected const int SlideTimeIndex = 9;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 10;

		/// <summary>
		/// The default loop speed.
		/// </summary>
		protected const float DefaultLoopSpeed = 14.0f;
		
		/// <summary>
		/// The default slide time.
		/// </summary>
		protected const float DefaultSlideTime = 1.5f;

		/// <summary>
		/// How much do we accentuate friciton in the opposite of the slide down direction
		/// </summary>
		protected const float LoopCutOffPoint = 60.0f;

		#endregion

		#region Unity hooks

		void Update()
		{
			if (slideDownTimer > 0.0f) slideDownTimer -= TimeManager.FrameTime;
		}

		#endregion

		#region public methods

		/// <summary> 
		/// Applies the slope force by setting velocity. Note this WILL NOT move the character that happens in DoMove.
		/// </summary>
		override public void ApplySlopeForce()
		{
			// Note: we use target instead of actual slope in this movement
			if (character.SlopeTargetRotation != 0.0f && autoLoopDirection == 0)
			{
				float force = 0.0f;
				if (character.SlopeActualRotation > 90)
				{
					// Assume over 90 degrees that slope force is constant
					force = character.DefaultGravity * slopeAccelerationFactor;
				}
				else if (character.SlopeActualRotation < -90)
				{
					// Assume over 90 degrees that slope force is constant
					force = character.DefaultGravity * -slopeAccelerationFactor;
				}
				else
				{
					force = character.DefaultGravity * Mathf.Sin(character.SlopeActualRotation * Mathf.Deg2Rad) * slopeAccelerationFactor;
				}
				if (Mathf.Abs (force ) > ignoreForce)
				{
					character.AddVelocity(force * TimeManager.FrameTime, 0, false);
					hasAppliedForce = true;
				}
			}
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			
			bool goingAboveMax = false;

			// Apply gravity ourselves
			character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
			character.Translate(0, character.Velocity.y * TimeManager.FrameTime, false);

			float actualFriction = character.Friction;
			if (actualFriction == -1) actualFriction = friction;

			// We are grounded, y should be zero
			// character.SetVelocityY(0);

			// Apply Friction and input acceleration if not auto looping
			if (autoLoopDirection == 0)
			{
				// If sliding
				if (slideDownTimer > 0.0f)
				{
					float factor = 1;
					// As the angle gets closer to 90 get more and more aggressive about arresting motion
					if (Mathf.Abs (character.SlopeTargetRotation) >= 90)
					{
						// Hard stop
						if ((slideDownDirection == 1 && character.Velocity.x > 0) || slideDownDirection == -1 && character.Velocity.x < 0) character.SetVelocityX(0);
					}
					else if (Mathf.Abs (character.SlopeTargetRotation) > 80)
					{
						factor = 20;
					}
					else if (Mathf.Abs (character.SlopeTargetRotation) > 70)
					{
						factor = 10;
					}
					else if (Mathf.Abs (character.SlopeTargetRotation) > LoopCutOffPoint)
					{
						factor = 5;
					}
					// Friction only in slide down direction
					if (character.Velocity.x > 0  && slideDownDirection == 1) 
					{
						character.AddVelocity(-character.Velocity.x * factor * actualFriction * TimeManager.FrameTime, 0, false);
						if (character.Velocity.x < 0) character.SetVelocityX(0);
					}
					else if (character.Velocity.x < 0 && slideDownDirection == -1) 
					{
						character.AddVelocity(-character.Velocity.x * actualFriction * TimeManager.FrameTime, 0, false);
						if (character.Velocity.x > 0) character.SetVelocityX(0);
					}
				}
				else
				{

					if (Mathf.Abs (character.Velocity.x) > maxSpeed) goingAboveMax = true;

					// Friction
					if (character.Velocity.x > 0) 
					{
						character.AddVelocity(-character.Velocity.x * actualFriction * TimeManager.FrameTime, 0, false);
						if (character.Velocity.x < 0) character.SetVelocityX(0);
					}
					else if (character.Velocity.x < 0) 
					{
						character.AddVelocity(-character.Velocity.x * actualFriction * TimeManager.FrameTime, 0, false);
						if (character.Velocity.x > 0) character.SetVelocityX(0);
					}
			
					// Input acceleration
					if (slideDownTimer <= 0.0f && Mathf.Abs (character.Input.HorizontalAxisDigital * acceleration) > ignoreForce)
					{
						if (!goingAboveMax ||
						    (character.Velocity.x > 0 && character.Input.HorizontalAxisDigital == -1) ||
						    (character.Velocity.x < 0 && character.Input.HorizontalAxisDigital == 1))
						{
							character.AddVelocity((useAnalogueInput ? character.Input.HorizontalAxis : (float)character.Input.HorizontalAxisDigital) * acceleration * TimeManager.FrameTime, 0, false);
							hasAppliedForce = true;
						}
					}
				}

			}
			// Else apply acceleration in auto loop direction
			else 
			{
				//character.AddVelocity(((float)autoLoopDirection) * acceleration * TimeManager.FrameTime, 0, false);
				hasAppliedForce = true;
			}

			// Limit to max speed (only if we were going slower than max)
			if (!goingAboveMax)
			{
				if (character.Velocity.x > maxSpeed) 
				{
					character.SetVelocityX(maxSpeed);
				}
				else if (character.Velocity.x < -maxSpeed) 
				{
					character.SetVelocityX(-maxSpeed);
				}
			}

			// Quiesce if moving very slowly and no force applied
			if (!hasAppliedForce)
			{
				if (character.Velocity.x > 0 && character.Velocity.x < quiesceSpeed ) 
				{
					character.SetVelocityX(0);
				}
				else if (character.Velocity.x  < 0 && character.Velocity.x > -quiesceSpeed)
				{
					character.SetVelocityX(0);
				}
			}

			// Translate
			character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, false);

			// Update state
			// Looping
			if (autoLoopDirection != 0)
			{
				if (Mathf.Abs(character.SlopeTargetRotation) < 45)
				{
					autoLoopDirection = 0;
				}
			}
			// Already sliding down
			else if (slideDownTimer > 0.0f)
			{
				// Don't slide past 0
				// Stop sliding if character pressed in slide direction
				if (character.SlopeTargetRotation == 0 || (character.Input.HorizontalAxisDigital == -slideDownDirection)) 
				{
					slideDownTimer = 0.0f;
					slideDownDirection = 0;
				}
			}
			// On a steep slope
			else if (Mathf.Abs(character.SlopeTargetRotation) > LoopCutOffPoint)
			{
				// Check for slide down conditions (not fast enough, always loop if you make it to 90)
				if (Mathf.Abs (character.Velocity.x) < loopSpeed && Mathf.Abs(character.SlopeTargetRotation) < 90 )
				{
					slideDownTimer = slideDownTime;
					slideDownDirection = character.SlopeTargetRotation > 0 ? -1 : 1;
					autoLoopDirection = 0;
				}
				// Check for loop conditions
				else
				{
					autoLoopDirection = character.Velocity.x > 0 ? 1 : -1;
					slideDownTimer = 0.0f;
				}
			}

			hasAppliedForce = false;
		}



		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{

			base.Init (character, movementData);

			// Set variables
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				slideDownTime = movementData[SlideTimeIndex].FloatValue;
				loopSpeed = movementData[LoopSpeedIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}

			return this;
		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't. In this case we want to control 
		/// the initial part of the fall from the top of a loop.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		override public bool ForceMaintainControl()
		{
			if (autoLoopDirection != 0) return true;
			if (slideDownTimer > 0) return true;
			// Don't allow jump past LoopCutOffPoint degrees
			if (Mathf.Abs(character.SlopeTargetRotation) > LoopCutOffPoint) return true;
			return false;
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			if (character.ActiveMovement.VelocityType != VelocityType.RELATIVE_X_WORLD_Y)
			{
				Vector2 newVelocity = Quaternion.Euler(0,0, -character.SlopeActualRotation) * character.Velocity;
				character.SetVelocityX(newVelocity.x);
			}
			character.SetVelocityY(0);
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions. You should
		/// ensure that character velocity is reset back to world-relative velocity here.
		/// </summary>
		override public void LosingControl() 
		{
			isLooping = false;
			slideDownTimer = 0.0f;
			localFallVelocity = 0.0f;
			autoLoopDirection = 0;
		}

		
		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// base collisions to be executed after its movement finishes.
		/// </summary>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				if (autoLoopDirection != 0 || Mathf.Abs (character.SlopeTargetRotation) > 90)  return RaycastType.FOOT | RaycastType.HEAD;
				return RaycastType.ALL;
			}
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (autoLoopDirection != 0) return AnimationState.ROLL;
				if (Mathf.Abs (character.Velocity.x) >= (0.99f * maxSpeed) ) return AnimationState.RUN;
				return base.AnimationState;
			}
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

#if UNITY_EDITOR

		#region draw inspector
		
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}
			
			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = GroundMovement_Physics.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			if (showDetails)
			{
				// Loop Speed
				if (movementData[LoopSpeedIndex] == null) movementData[LoopSpeedIndex] = new MovementVariable(DefaultLoopSpeed);
				movementData[LoopSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Loop Speed", "Speed required to be able to run the loop."), movementData[LoopSpeedIndex].FloatValue );
				if (movementData[LoopSpeedIndex].FloatValue < 0) movementData[LoopSpeedIndex].FloatValue = 0;

				// Slide Time
				if (movementData[SlideTimeIndex] == null) movementData[SlideTimeIndex] = new MovementVariable(DefaultSlideTime);
				movementData[SlideTimeIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("SlideTime", "How long will the character slide down a slope if they get to 90 degrees without required speed."), movementData[SlideTimeIndex].FloatValue);
				if (movementData[SlideTimeIndex].FloatValue < 0) movementData[SlideTimeIndex].FloatValue = 0;
			}

			// Not rotate warning
			if (!target.rotateToSlopes) 
			{
				EditorGUILayout.HelpBox("A character with a Loop movement must use rotate to slopes.", MessageType.Error);
			}
			else if (target.maxSlopeRotation < 90) 
			{
				EditorGUILayout.HelpBox("If your character cannot rotate to an angle of 90 degrees you don't need a Loop movement.", MessageType.Warning);
			}
			if (!target.rotateToSlopes || target.maxSlopeRotation < 90) 
			{
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Fix Now"))
				{
					target.rotateToSlopes = true;
					target.maxSlopeRotation = 180;
				}
				GUILayout.EndHorizontal();
			}


			return movementData;
		}

		#endregion

#endif

	}
	
}

