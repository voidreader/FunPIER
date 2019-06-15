#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement roll on land.
	/// </summary>
	public class GroundMovement_RollOnLand : GroundMovement
	{

		#region members

		/// <summary>
		/// The speed the character rolls at.
		/// </summary>
		[TaggedProperty ("maxSpeed")]
		[TaggedProperty ("speed")]
		public float speed;

		/// <summary>
		/// How long to roll for if no interrupt is given.
		/// </summary>
		public float rollTime;
		
		/// <summary>
		/// How fast to be falling before we have to roll?
		/// </summary>
		public float yVelocityForRoll;

		/// <summary>
		/// Stores the direction the roll started in.
		/// </summary>
		protected int currentRollDirection;

		/// <summary>
		/// How long till we stop rolling
		/// </summary>
		protected float rollTimer;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Roll on Land";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which rolls when the cahracter lands after a large fall. Should NOT be the only ground movement.";
		
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
		/// The index for the speed value in the movement data.
		/// </summary>
		protected const int SpeedIndex = 0;

		/// <summary>
		/// The index for the role time in the data.
		/// </summary>
		protected const int RollTimeIndex = 1;

		/// <summary>
		/// The index for the role time in the data.
		/// </summary>
		protected const int YVelocityForRollIndex = 2;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 3;

		/// <summary>
		/// The default speed.
		/// </summary>
		protected const float DefaultSpeed = 4.0f;
		
		/// <summary>
		/// The default roll time.
		/// </summary>
		protected const float DefaultRollTime = 1.0f;

		/// <summary>
		/// The default Y velocity for roll.
		/// </summary>
		protected const float DefaultYVelocityForRoll = -10.0f;

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (rollTimer > 0.0f) 
			{
				rollTimer -= TimeManager.FrameTime;
				if (character.IsAttacking) rollTimer = 0.0f;
			}
		}

		#endregion

		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (rollTimer > 0.0f)
			{
				if (character.Input.HorizontalAxisDigital == 0 || character.Input.HorizontalAxisDigital == currentRollDirection)
				{
					character.SetVelocityX(speed * currentRollDirection);
					character.Translate(speed * currentRollDirection * TimeManager.FrameTime, 0, false);
				}
			}
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

			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				speed = movementData[SpeedIndex].FloatValue;
				rollTime = movementData[RollTimeIndex].FloatValue;
				yVelocityForRoll = movementData[YVelocityForRollIndex].FloatValue;
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
				if (rollTimer > 0.0f)
				{
					return AnimationState.ROLL;
				}
				return AnimationState.IDLE;
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
				return currentRollDirection;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement on the ground.
		/// We want this if roll timer is set and user hasn't put in a counter input.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsGroundControl()
		{
			if (!enabled) return false;
			// Existing roll
			if (rollTimer > 0.0f && (character.Input.HorizontalAxisDigital == 0 || character.Input.HorizontalAxisDigital == currentRollDirection))
			{
				return true;
			}
			// New roll
			if (character.Grounded && character.PreviousVelocity.y <= yVelocityForRoll)
			{
				if (character.Velocity.x > 0.0f)
				{
					currentRollDirection = 1;
				}
				else if (character.Velocity.x < 0.0f)
				{
					currentRollDirection = -1;
				}
				else
				{
					currentRollDirection = character.LastFacedDirection;
				}
				rollTimer = rollTime;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			rollTimer = 0.0f;
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

			// Roll speed
			if (movementData[SpeedIndex] == null) movementData[SpeedIndex] = new MovementVariable(DefaultSpeed);
			movementData[SpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Speed", "How fast the character walks."), movementData[SpeedIndex].FloatValue);
			if (movementData[SpeedIndex].FloatValue < 0) movementData[SpeedIndex].FloatValue = 0.0f;

			//Roll time
			if (movementData[RollTimeIndex] == null) movementData[RollTimeIndex] = new MovementVariable(DefaultRollTime);
			movementData[RollTimeIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Roll Time", "How long the character will roll for if not interrupted."), movementData[RollTimeIndex].FloatValue);
			if (movementData[RollTimeIndex].FloatValue < 0) movementData[RollTimeIndex].FloatValue = 0.0f;

			// Y velocity for roll
			if (movementData[YVelocityForRollIndex] == null) movementData[YVelocityForRollIndex] = new MovementVariable(DefaultYVelocityForRoll);
			movementData[YVelocityForRollIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Fall Speed", "How fast the character needs to be falling before roll starts."), movementData[YVelocityForRollIndex].FloatValue);
			if (movementData[YVelocityForRollIndex].FloatValue > 0) movementData[YVelocityForRollIndex].FloatValue = 0.0f;

			return movementData;
		}

		#endregion

#endif
	}

}

