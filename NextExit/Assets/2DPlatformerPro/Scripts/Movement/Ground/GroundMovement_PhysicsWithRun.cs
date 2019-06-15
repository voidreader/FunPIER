#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement that uses physics style movement and includes run.
	/// </summary>
	public class GroundMovement_PhysicsWithRun: GroundMovement_Physics
	{

		#region members

		/// <summary>
		/// Maximum speed when run is not pressed.
		/// </summary>
		[TaggedProperty ("agility")]
		public float walkSpeed;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Physics/With Run";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which uses drag and acceleration to give physics like movement and alos includes running.";
		
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
		/// The index for the walk speed.
		/// </summary>
		protected const int WalkSpeedIndex = 8;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 9;

		#endregion

		#region properties

		
		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> can
		/// support automatic sliding based on the characters slope. This physics based movement can do this.
		/// </summary>
		override public bool SupportsSlidingOnSlopes
		{
			get
			{
				return true;
			}
		}


		#endregion

		#region public methods

		/// <summary>
		/// Applies the slope force by setting velocity. Note this WILL NOT move the character that happens in DoMove.
		/// </summary>
		override public void ApplySlopeForce()
		{
			if (character.SlopeActualRotation != 0.0f)
			{
				float force = character.Gravity * Mathf.Sin(character.SlopeActualRotation * Mathf.Deg2Rad) * slopeAccelerationFactor;
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
			float actualFriction = character.Friction;
			if (actualFriction == -1) actualFriction = friction;

			// Apply drag
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

			// Apply acceleration 
			if (Mathf.Abs (character.Input.HorizontalAxisDigital * acceleration) > ignoreForce )
			{
			    // If already at walk speed limit speed to walk
			    if (character.Input.RunButton != ButtonState.DOWN && character.Input.RunButton != ButtonState.HELD)
				{
					if (Mathf.Abs (character.Velocity.x) < walkSpeed)
					{
						character.AddVelocity((float)character.Input.HorizontalAxisDigital * acceleration * TimeManager.FrameTime, 0, false);
						hasAppliedForce = true;
						// Limit to walk speed
						if (character.Velocity.x > walkSpeed) 
						{
							character.SetVelocityX(walkSpeed);
						}
						else if (character.Velocity.x < -walkSpeed) 
						{
							character.SetVelocityX(-walkSpeed);
						}
					}
					else
					{
						// Slow down
						// character.AddVelocity((float)character.Input.HorizontalAxisDigital * -acceleration * TimeManager.FrameTime, 0, false);
					}
				}
				else
				{
					character.AddVelocity((float)character.Input.HorizontalAxisDigital * acceleration * TimeManager.FrameTime, 0, false);
					hasAppliedForce = true;
				}
			    

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
			if (Mathf.Abs (character.Velocity.x) > quiesceSpeed) character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, false);

			hasAppliedForce = false;
		}

		/// <summary>
		/// Does any movement that MUST be done after collissions are calculated.
		/// </summary>
		override public void PostCollisionDoMove() {
			if (enabled && stickToGround) SnapToGround ();
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
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			this.character = character;

			// Set variables
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				acceleration = movementData[AccelerationIndex].FloatValue;
				maxSpeed = movementData[MaxSpeedIndex].FloatValue;
				friction = movementData[FrictionIndex].FloatValue;
				slopeAccelerationFactor = movementData[SlopeAccelerationIndex].FloatValue;
				quiesceSpeed = movementData[QuiesceSpeedIndex].FloatValue;
				ignoreForce = movementData[IgnoreForceIndex].FloatValue;
				stickToGround = movementData[StickToGroundIndex].BoolValue;
				walkSpeed = movementData[WalkSpeedIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
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
				if (Mathf.Abs (character.Velocity.x) < quiesceSpeed )
				{
					return AnimationState.IDLE;
				}
				else if (character.Input.HorizontalAxisDigital == 0)
				{
					return AnimationState.SLIDE;
				}
				else
				{
					if ((character.Input.HorizontalAxisDigital == 1 && character.Velocity.x < 0) ||
					    (character.Input.HorizontalAxisDigital == -1 && character.Velocity.x > 0))
					{
						return AnimationState.SLIDE_DIR_CHANGE;
					}
					if (Mathf.Abs(character.Velocity.x) > walkSpeed) return AnimationState.RUN;
					return AnimationState.WALK;
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
			
			// Max Speed
			if (movementData[MaxSpeedIndex] == null) movementData[MaxSpeedIndex] = new MovementVariable();
			movementData[MaxSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Maximum Speed", "The characters peak speed."), movementData[MaxSpeedIndex].FloatValue);
			if (movementData[MaxSpeedIndex].FloatValue < 0) movementData[MaxSpeedIndex].FloatValue = 0.0f;

			// Walk Speed
			if (movementData[WalkSpeedIndex] == null) movementData[WalkSpeedIndex] = new MovementVariable();
			movementData[WalkSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Walk Speed", "The characters walk speed."), movementData[WalkSpeedIndex].FloatValue);
			if (movementData[WalkSpeedIndex].FloatValue < 0) movementData[WalkSpeedIndex].FloatValue = 0.0f;

			// Acceleration
			if (movementData[AccelerationIndex] == null) movementData[AccelerationIndex] = new MovementVariable();
			movementData[AccelerationIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Acceleration", "How fast the character accelerates."), movementData[AccelerationIndex].FloatValue);
			if (movementData[AccelerationIndex].FloatValue < 0) movementData[AccelerationIndex].FloatValue = 0.0f;
			
			// Stick to ground
			if (movementData[StickToGroundIndex] == null) movementData[StickToGroundIndex] = new MovementVariable(false);
			movementData[StickToGroundIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Stick to Ground", "Should the character stick to the ground when running on slopes?"), movementData[StickToGroundIndex].BoolValue);
			
			// Stick to ground warning
			if (!target.rotateToSlopes && !movementData[StickToGroundIndex].BoolValue) 
			{
				EditorGUILayout.HelpBox("If your Character doesn't rotate to slopes you may want to enable stick to ground. If these settings are both off your cahracter will be able to 'run' off the slope.", MessageType.Info);
			}
			
			showDetails = EditorGUILayout.Foldout(showDetails, "Advanced Settings");
			DrawStandardPhysicsSettings(movementData, 0, showDetails);
			
			return movementData;
		}


		#endregion

#endif

	}
}

