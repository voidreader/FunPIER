#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Air movement which tries to hit a target speed by slowing down or speeding up. Simulates a parachute.
	/// </summary>
	public class AirMovement_Parachute: AirMovement
	{

		/// <summary>
		/// The target speed.
		/// </summary>
		public float targetSpeed;

		/// <summary>
		/// The acceleration used to speed up to target speed.
		/// </summary>
		public float gravity;

		/// <summary>
		/// The deceleration, used to slow down to target speed if going faster than gravity when parachute is engaged.
		/// </summary>
		public float deceleration;

		/// <summary>
		/// How fast we move in x.
		/// </summary>
		public float xSpeed;

		/// <summary>
		/// The action buttion. If its -1 parachute is always engaged.
		/// </summary>
		public int actionButton;

		/// <summary>
		/// Track if a normal jump has started.
		/// </summary>
		protected bool jumpStarted;
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Parachute";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which tries to hit a target speed by slowing down or speeding up. " +
			"Simulates a parachute.";
		
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
		/// Default target speed.
		/// </summary>
		protected const float DefaultTargetSpeed = -5.0f;

		/// <summary>
		/// The default acceleration.
		/// </summary>
		protected const float DefaultGravity = -5.0f;

		/// <summary>
		/// The default decceleration.
		/// </summary>
		protected const float DefaultDeceleration = 5.0f;

		/// <summary>
		/// The default x speed.
		/// </summary>
		protected const float DefaultXSpeed = 1.0f;

		/// <summary>
		/// The index of the target speed
		/// </summary>
		protected const int TargetSpeedIndex = 0;

		/// <summary>
		/// The index of the acceleration.
		/// </summary>
		protected const int GravityIndex = 1;

		/// <summary>
		/// The index of the deceleration.
		/// </summary>
		protected const int DecelerationIndex = 2;

		/// <summary>
		/// The index of the x speed.
		/// </summary>
		protected const int XSpeedIndex = 3;

		/// <summary>
		/// The index of the action button.
		/// </summary>
		protected const int ActionButtonIndex = 4;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 5;

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
				return gravity;
			}
		}

		#endregion

		
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
				targetSpeed = movementData[TargetSpeedIndex].FloatValue;
				gravity = movementData[GravityIndex].FloatValue;
				deceleration = movementData[DecelerationIndex].FloatValue;
				xSpeed = movementData [XSpeedIndex].FloatValue;
				actionButton = movementData[ActionButtonIndex].IntValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
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
			if (!enabled) return false;
			if (character.Grounded) return false;
			// Moving up don't take control
			if (character.Velocity.y > 0) return false;
			// Else check button 
			if (actionButton == -1 || character.Input.GetActionButtonState(actionButton) == ButtonState.HELD) return true;
			return false;
		}

		override public void DoMove() {
			MoveInX (character.Input.HorizontalAxis, character.Input.HorizontalAxisDigital, ButtonState.NONE);
			MoveInY ();
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override protected void MoveInX (float horizontalAxis, int horizontalAxisDigital, ButtonState runButton)
		{
			if (horizontalAxisDigital  == 1)
			{
				character.SetVelocityX(character.IsGravityFlipped ? -xSpeed : xSpeed);
				character.Translate((character.IsGravityFlipped ? -xSpeed : xSpeed) * TimeManager.FrameTime, 0, false);
			}
			else if (horizontalAxisDigital == -1)
			{
				character.SetVelocityX(character.IsGravityFlipped ? xSpeed : -xSpeed);
				character.Translate((character.IsGravityFlipped ? xSpeed : -xSpeed) * TimeManager.FrameTime, 0, false);
			}
			else
			{
				character.SetVelocityX(0);
			}
			character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, true);
		}

		/// Do the Y movement.
		/// </summary>
		override protected void MoveInY ()
		{
			// Apply gravity
			if (!character.Grounded)
			{
				if (character.Velocity.y > targetSpeed)
				{
					character.AddVelocity (0, TimeManager.FrameTime * gravity, false);
					if (character.Velocity.y < targetSpeed) character.SetVelocityY (targetSpeed);
				} 
				else
				{
					character.AddVelocity (0, TimeManager.FrameTime * deceleration, false);
					if (character.Velocity.y > targetSpeed) character.SetVelocityY (targetSpeed);
				}

			}
			// Translate
			character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				return AnimationState.PARACHUTE;
			}
		}


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

			if (movementData[TargetSpeedIndex] == null) movementData[TargetSpeedIndex] = new MovementVariable(DefaultTargetSpeed);
			movementData[TargetSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Target Speed", "Termainl velocity while parachute is engaged."), movementData[TargetSpeedIndex].FloatValue);
			if (movementData[TargetSpeedIndex].FloatValue > 0) movementData[TargetSpeedIndex].FloatValue = 0.0f;

			if (movementData[GravityIndex] == null) movementData[GravityIndex] = new MovementVariable(DefaultGravity);
			movementData[GravityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Gravity", "Gravity to apply while parachute engaged."), movementData[GravityIndex].FloatValue);
			if (movementData[GravityIndex].FloatValue > 0) movementData[GravityIndex].FloatValue = 0.0f;

			if (movementData[DecelerationIndex] == null) movementData[DecelerationIndex] = new MovementVariable(DefaultDeceleration);
			movementData[DecelerationIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Deceleration", "The deceleration, used to slow down to target speed if going faster than gravity when parachute is engaged."), movementData[DecelerationIndex].FloatValue);
			if (movementData[DecelerationIndex].FloatValue < 0) movementData[DecelerationIndex].FloatValue = 0.0f;

			if (movementData[XSpeedIndex] == null) movementData[XSpeedIndex] = new MovementVariable(DefaultXSpeed);
			movementData[XSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("X Speed", "Speed to move in in X direction."), movementData[XSpeedIndex].FloatValue);

			if (movementData[ActionButtonIndex] == null) movementData[ActionButtonIndex] = new MovementVariable(-1);
			movementData[ActionButtonIndex].IntValue = EditorGUILayout.IntField(new GUIContent("Action Button", "Action Button to use to engage parachute or -1 for always engaged."), movementData[ActionButtonIndex].IntValue);

			return movementData;
		}

		#endregion
#endif
	}
}
