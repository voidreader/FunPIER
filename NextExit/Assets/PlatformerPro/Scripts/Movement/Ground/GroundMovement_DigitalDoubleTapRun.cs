#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement_ digital.
	/// </summary>
	public class GroundMovement_DigitalDoubleTapRun : GroundMovement_Digital
	{
		
		#region members

		/// <summary>
		/// Should we cancel run on side collission or just play a PUSH animation.
		/// </summary>
		public bool cancelRunOnSideCollisions;

		/// <summary>
		/// The timer for the first tap (counts down).
		/// </summary>
		protected float tapTimer;
		
		/// <summary>
		/// Direction of first tap.
		/// </summary>
		protected int tapDirection;
		
		/// <summary>
		/// True if we have started running.
		/// </summary>
		protected bool isRunning;
		
		/// <summary>
		/// Should we auto start to run as soon as we next hit the ground
		/// </summary>
		protected bool autoStartRunning;
		
		/// <summary>
		/// Should we check for auto start?
		/// </summary>
		protected bool checkForAutoStart;

		/// <summary>
		/// Are we hitting the sides?
		/// </summary>
		protected bool hittingSides;

		#endregion
		
		#region constants
		
		/// <summary>
		/// How long before the second tap has to be entered for this to start.
		/// </summary>
		protected const float secondTapTime = 0.33f;
		
		/// <summary>
		/// How long before the switch direction tap has to be done before run stops
		/// </summary>
		protected const float switchTapTime = 0.1f;
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Digital/Double Tap Run";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which starts running at a constant speed when you double tap.";
		
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
		/// The index of the cancel run on side collisison.
		/// </summary>
		protected const int CancelRunOnSideCollisionIndex = 1;
		
		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 2;

		#endregion
		
		#region properties
		
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (hittingSides) return AnimationState.PUSH;
				return AnimationState.RUN;
			}
		}
		
		
		#endregion
		
		#region Unity hooks
		
		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update()
		{
			if (tapTimer > 0.0f)
			{
				tapTimer -= TimeManager.FrameTime;
				if (!isRunning && tapTimer <= 0.0f)
				{
					tapDirection = 0;
				}
			}
			// Check for any conditions that will stop our run (i.e. letting go of button)
			if (checkForAutoStart || autoStartRunning)
			{
				// Let go of key dont autostart
				if (character.Input.HorizontalAxisState != ButtonState.HELD) 
				{
					checkForAutoStart = false;
					autoStartRunning = false;
				}
				// We left the ground, next time we touch it start running
				else if (!autoStartRunning && !character.Grounded)
				{
					autoStartRunning = true;
				}
				// TODO We could add additional conditions like hitting a wall, doing a special movement, etc
			}
		}
		
		#endregion
		
		#region public methods

		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			base.Init (character, movementData);
			
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{

				cancelRunOnSideCollisions = movementData[CancelRunOnSideCollisionIndex].BoolValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}

			return this;
		}

		/// <summary>
		/// Called after initialisation to check if this movement is configured correctly. 
		/// Typically used to stop wrapper movements being the default and ending up in infinite loops.
		/// </summary>
		override public string PostInitValidation()
		{
#if UNITY_EDITOR
			// Errors 
			if (character.DefaultAirMovement == this) return "Double Tap Run movement cannot be the default GroundMovement.";

			// Warnings
			if (character.MovementHasPriorityOverDefaultAir(this)) Debug.LogWarning("If double tap run is higher priority than the default air movement you may not be able to jump while running.");
#endif
			return base.PostInitValidation();
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement on the ground.
		/// Default is false, with control falling back to default ground. Override if you want particular control.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		/// <returns><c>true</c>, if ground control was wantsed, <c>false</c> otherwise.</returns>
		override public bool WantsGroundControl()
		{
			// Don't run if not on the ground
			if (!character.Grounded) return false;

			// Already running
			if (isRunning) 
			{
				checkForAutoStart = false;
				// Pressed run again stop running
				if (character.Input.HorizontalAxisState == ButtonState.DOWN && character.Input.HorizontalAxisDigital == tapDirection)
				{
					return false;
				}
				// Still holding keep running
				if (character.Input.HorizontalAxisState == ButtonState.HELD && character.Input.HorizontalAxisDigital == tapDirection)
				{
					// TODO Does this need to be a separate variable
					tapTimer = switchTapTime;
					return true;
				}
				// Other direction timer expired - stop running
				else if (tapTimer <= 0)
				{
					return false;
				}
				// Other direction within timer speed
				else if (character.Input.HorizontalAxisDigital == -tapDirection)
				{
					tapDirection = character.Input.HorizontalAxisDigital;
					tapTimer = switchTapTime;
					return true;
				}
				return true;
			}

            // First tap
			if (tapDirection == 0 && character.Input.HorizontalAxisState == ButtonState.DOWN)
			{
				tapDirection = character.Input.HorizontalAxisDigital;
				tapTimer = secondTapTime;
				checkForAutoStart = false;
				return false;
			}
			
			// Second tap (or auto run)
			if ((autoStartRunning && character.Grounded && character.Velocity.y <= 0.0f) || 
			    (tapTimer > 0.0f && character.Input.HorizontalAxisState == ButtonState.DOWN && character.Input.HorizontalAxisDigital == tapDirection))
			{
				tapDirection = character.Input.HorizontalAxisDigital;
				isRunning = true;
				tapTimer = 0.0f;
				checkForAutoStart = false;
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			tapTimer = 0.0f;
			// If the user keeps holding the key we want to keep running
			checkForAutoStart = true;
			tapDirection = 0;
			isRunning = false;
			autoStartRunning = false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// Direction switch
			if (tapDirection == 1 && character.Input.HorizontalAxisState == ButtonState.DOWN && character.Input.HorizontalAxisDigital == -1) 
			{
				tapDirection = -1;
			}
			else if (tapDirection == -1 && character.Input.HorizontalAxisState == ButtonState.DOWN && character.Input.HorizontalAxisDigital == 1) 
			{
				tapDirection = 1;
			}
			
			// Check collisions
			hittingSides = false;
			if (CheckSideCollisions (character, 1, tapDirection == 1 ? RaycastType.SIDE_RIGHT : RaycastType.SIDE_LEFT))
			{

				if (cancelRunOnSideCollisions)
				{
					tapTimer = 0.0f;
					tapDirection = 0;
					isRunning = false;
					autoStartRunning = false;
				}
				else
				{
					hittingSides = true;
				}
			}
			
			// Set frame speed - if friction is bigger than 2 we will slow the character down.
			float frameSpeed = GetRunSpeed(speed);
			if (character.Friction > 2.0f) frameSpeed *= (2.0f / character.Friction );
			#if UNITY_EDITOR
			if (Character.Friction >= 0 && Character.Friction < 2.0f) Debug.LogError("A friction less than 2 has no affect on digitial movements.");
			#endif

			if (tapDirection == 1)
			{
				character.SetVelocityX(character.IsGravityFlipped ? -frameSpeed : frameSpeed);
				character.Translate((character.IsGravityFlipped ? -frameSpeed : frameSpeed) * TimeManager.FrameTime, 0, false);
			}
			else if (tapDirection == -1)
			{
				character.SetVelocityX(character.IsGravityFlipped ? frameSpeed : -frameSpeed);
				character.Translate((character.IsGravityFlipped ? frameSpeed : -frameSpeed) * TimeManager.FrameTime, 0, false);
			}
			else
			{
				character.SetVelocityX(0);
			}
		}

		
		/// <summary>
		/// Does any movement that MUST be done after collissions are calculated.
		/// </summary>
		override public void PostCollisionDoMove() {
			if (enabled && !character.rotateToSlopes) SnapToGround ();
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

			// Cancel Run On Side Collision
			if (movementData[CancelRunOnSideCollisionIndex] == null) movementData[CancelRunOnSideCollisionIndex] = new MovementVariable();
			movementData[CancelRunOnSideCollisionIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Cancel Run on Collision", " Should we cancel run on side collission or just play a PUSH animation."), movementData[CancelRunOnSideCollisionIndex].BoolValue);

			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = GroundMovement_Digital.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			return movementData;
		}

		#endregion
		
		#endif
	}
	
}