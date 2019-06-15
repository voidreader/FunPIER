#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement that uses physics style movement.
	/// </summary>
	public class GroundMovement_Physics: GroundMovement, IPhysicsMovement
	{

		#region members

		/// <summary>
		/// The speed the character accelerates at.
		/// </summary>
		[TaggedProperty("agility")]
		[TaggedProperty("acceleration")]
		public float acceleration;

		/// <summary>
		/// The maximum travel speed.
		/// </summary>
		[TaggedProperty("speedLimit")]
		[TaggedProperty("agility")]
		[TaggedProperty ("groundSpeedLimit")]
		public float maxSpeed;

		/// <summary>
		/// The default friction. Platforms can override this.
		/// </summary>
		[TaggedProperty("friction")]
		public float friction;

		/// <summary>
		/// If true the acceleration will be multiplied via the analogue input value.
		/// </summary>
		public bool useAnalogueInput;

		/// <summary>
		/// If non-zero the character will have acceleration applied by slopes based on gravity.
		/// The actual force will be f = g sin() * slopeAccelerationFactor.
		/// </summary>
		public float slopeAccelerationFactor;

		/// <summary>
		/// If a character is moving slower than this stop them moving.
		/// </summary>
		public float quiesceSpeed;
		
		/// <summary>
		/// Forces smaller than this will be ignored (generally applies to, for example, slighlty sloped platforms or very small input values).
		/// </summary>
		public float ignoreForce;

		/// <summary>
		/// Should we stick to the ground when running on a slope.
		/// </summary>
		public bool stickToGround;

		/// <summary>
		/// Has a force been applied this frame? If so don't quiesce.
		/// </summary>
		protected bool hasAppliedForce;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Physics/Standard";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which uses drag and acceleration to give physics like movement.";
		
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
		/// The index for the friction value in the movement data.
		/// </summary>
		public const int FrictionIndex = 0;

		/// <summary>
		/// The index for the use analogue input value in the movement data.
		/// </summary>
		public const int UseAnalogueIndex = 1;

		/// <summary>
		/// The index for the use slope accelration factor value in the movement data.
		/// </summary>
		public const int SlopeAccelerationIndex = 2;

		/// <summary>
		/// The index for the quiesce value in the movement data.
		/// </summary>
		public const int QuiesceSpeedIndex = 3;

		/// <summary>
		/// The index for the ignore force value in the movement data.
		/// </summary>
		public const int IgnoreForceIndex = 4;

		/// <summary>
		/// The index for the acceleration value in the movement data.
		/// </summary>
		protected const int AccelerationIndex = 5;
		
		/// <summary>
		/// The index for the max speed in the movement data.
		/// </summary>
		protected const int MaxSpeedIndex = 6;

		/// <summary>
		/// The index for the stick to ground value in the movement data.
		/// </summary>
		protected const int StickToGroundIndex = 7;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 8;

		/// <summary>
		/// The default slope acceleration.
		/// </summary>
		public const float DefaultFriction = 5.0f;

		/// <summary>
		/// The default slope acceleration.
		/// </summary>
		public const float DefaultSlopeAcceleration = 0.0f;

		/// <summary>
		/// The default quiesce Speed.
		/// </summary>
		public const float DefaultQuiesceSpeed = 0.5f;

		/// <summary>
		/// The default ignore force.
		/// </summary>
		public const float DefaultIgnoreForce = 2f;

		/// <summary>
		/// The default ignore force.
		/// </summary>
		protected const float DefaultAcceleration = 40f;

		/// <summary>
		/// The default ignore force.
		/// </summary>
		protected const float DefaultMaxSpeed = 8f;


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
			if (character.rotateToSlopes && character.SlopeActualRotation != 0.0f)
			{
				float force = character.Gravity * Mathf.Sin(character.SlopeActualRotation * Mathf.Deg2Rad) * slopeAccelerationFactor;
				if (Mathf.Abs (force ) > ignoreForce)
				{
					character.AddVelocity(force * TimeManager.FrameTime, 0, false);
					hasAppliedForce = true;
				}
			}
			else if (!character.rotateToSlopes && character.calculateSlopes && character.SlopeTargetRotation != 0.0f)
			{
				float force = character.Gravity * Mathf.Sin(-character.SlopeTargetRotation * Mathf.Deg2Rad) * slopeAccelerationFactor;
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
			if (Mathf.Abs (character.Input.HorizontalAxisDigital * acceleration) > ignoreForce)
			{
				character.AddVelocity((useAnalogueInput ? character.Input.HorizontalAxis : (float)character.Input.HorizontalAxisDigital) * acceleration * TimeManager.FrameTime, 0, false);
				hasAppliedForce = true;
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
			character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, false);

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
				useAnalogueInput = movementData[UseAnalogueIndex].BoolValue;
				slopeAccelerationFactor = movementData[SlopeAccelerationIndex].FloatValue;
				quiesceSpeed = movementData[QuiesceSpeedIndex].FloatValue;
				ignoreForce = movementData[IgnoreForceIndex].FloatValue;
				stickToGround = movementData[StickToGroundIndex].BoolValue;
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
				if (character.Input.HorizontalAxisDigital == 0)
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
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{

			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Max Speed
			if (movementData[MaxSpeedIndex] == null) movementData[MaxSpeedIndex] = new MovementVariable(DefaultMaxSpeed);
			movementData[MaxSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Maximum Speed", "The characters peak speed."), movementData[MaxSpeedIndex].FloatValue);
			if (movementData[MaxSpeedIndex].FloatValue < 0) movementData[MaxSpeedIndex].FloatValue = 0.0f;

			// Acceleration
			if (movementData[AccelerationIndex] == null) movementData[AccelerationIndex] = new MovementVariable(DefaultAcceleration);
			movementData[AccelerationIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Acceleration", "How fast the character accelerates."), movementData[AccelerationIndex].FloatValue);
			if (movementData[AccelerationIndex].FloatValue < 0) movementData[AccelerationIndex].FloatValue = 0.0f;

			// Use Analog
			if (movementData[UseAnalogueIndex] == null) movementData[UseAnalogueIndex] = new MovementVariable(false);
			movementData[UseAnalogueIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Use Analogue Input", "Should the characters acceleration be affected by how hard/far the character presses the controller?"), movementData[UseAnalogueIndex].BoolValue);

			// Stick to ground
			if (movementData[StickToGroundIndex] == null) movementData[StickToGroundIndex] = new MovementVariable(false);
			movementData[StickToGroundIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Stick to Ground", "Should the character stick to the ground when running on slopes?"), movementData[StickToGroundIndex].BoolValue);

			// Stick to ground warning
			if (target != null && !target.rotateToSlopes && !movementData[StickToGroundIndex].BoolValue) 
			{
				EditorGUILayout.HelpBox("If your Character doesn't rotate to slopes you may want to enable stick to ground. If these settings are both off your cahracter will be able to 'run' off the slope.", MessageType.Info);
			}

			showDetails = EditorGUILayout.Foldout(showDetails, "Advanced Settings");
			DrawStandardPhysicsSettings(movementData, 0, showDetails);
			
			return movementData;
		}

		/// <summary>
		/// Draws the standard physics settings (used by many movements).
		/// </summary>
		/// <param name="movementData">Movement data.</param>
		/// <param name="movementDataIndexOffset">Movement data index offset. Because different movements might place this in different places 
		/// in the movement data array we need to offset the array position.</param>
		public static void DrawStandardPhysicsSettings(MovementVariable[] movementData, int movementDataIndexOffset, bool showDetails)
		{
			// Reset values if null
			if (movementData[FrictionIndex + movementDataIndexOffset] == null) movementData[FrictionIndex + movementDataIndexOffset] = new MovementVariable(DefaultFriction);
			if (movementData[SlopeAccelerationIndex + movementDataIndexOffset] == null) movementData[SlopeAccelerationIndex + movementDataIndexOffset] = new MovementVariable(DefaultSlopeAcceleration);
			if (movementData[QuiesceSpeedIndex + movementDataIndexOffset] == null) movementData[QuiesceSpeedIndex + movementDataIndexOffset] = new MovementVariable(DefaultQuiesceSpeed);
			if (movementData[IgnoreForceIndex + movementDataIndexOffset] == null) movementData[IgnoreForceIndex + movementDataIndexOffset] = new MovementVariable(DefaultIgnoreForce);

			// Do we need to draw, if so draw.
			if (showDetails)
			{
				GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
				GUILayout.Label ("Physics Settings", EditorStyles.boldLabel);

				// Friction
				movementData[FrictionIndex + movementDataIndexOffset].FloatValue = EditorGUILayout.FloatField(new GUIContent("FrictionCoefficient", "Default coefficient of friction. Platforms can override this."), movementData[FrictionIndex + movementDataIndexOffset].FloatValue);
				if (movementData[FrictionIndex + movementDataIndexOffset].FloatValue < 0) movementData[FrictionIndex].FloatValue = 0.0f;
				
				// Slope Acceleration
				movementData[SlopeAccelerationIndex + movementDataIndexOffset].FloatValue = EditorGUILayout.Slider(new GUIContent("Slope Acceleration", "If the character is standing on a slope should they start to slide down it?"), movementData[SlopeAccelerationIndex + movementDataIndexOffset].FloatValue , 0, 2);
				
				// Quiesce
				movementData[QuiesceSpeedIndex + movementDataIndexOffset].FloatValue = EditorGUILayout.FloatField(new GUIContent("QuiesceSpeed", "At what velocity should we sleep if no acceleration is applied?"), movementData[QuiesceSpeedIndex + movementDataIndexOffset].FloatValue);
				
				// Ignore Force
				movementData[IgnoreForceIndex + movementDataIndexOffset].FloatValue = EditorGUILayout.FloatField(new GUIContent("Max Ignored Force", "Forces smaller than this will be ignored. Use this to ignore slight slopes or very small input values."), movementData[IgnoreForceIndex + movementDataIndexOffset].FloatValue);

				// Reset button
				if (GUILayout.Button(new GUIContent("Reset Physics To Defaults", "Resets all physics settings to default values.")))
				{
					movementData[FrictionIndex + movementDataIndexOffset].FloatValue = DefaultFriction;
					movementData[SlopeAccelerationIndex + movementDataIndexOffset].FloatValue = DefaultSlopeAcceleration;
					movementData[QuiesceSpeedIndex + movementDataIndexOffset].FloatValue = DefaultQuiesceSpeed;
					movementData[IgnoreForceIndex + movementDataIndexOffset].FloatValue = DefaultIgnoreForce;
				}
			}
		}

		#endregion

#endif

	}

	// TODO Use this
	/// <summary>
	/// Options for controlling the direction the character faces.
	/// </summary>
	public enum FacingPreference
	{
		FACE_MOVEMENT_DIRECTION,	// Face the way the character is moving
		FACE_INPUT_DIRECTION,		// Face the way the user is holding
		FACE_INPUT_THEN_MOVEMENT	// Face the input direction, if no input face the movement direciton, if no movement face forward
	}
}

