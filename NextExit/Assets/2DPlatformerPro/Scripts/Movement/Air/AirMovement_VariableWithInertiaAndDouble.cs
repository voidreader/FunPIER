#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An air movement that has a variable height jump and in which the horizontal speed 
	/// has some inertia before coming to a stop (i.e. is not instantly changed). Similar to SMB.
	/// </summary>
	public class AirMovement_VariableWithInertiaAndDouble : AirMovement_VariableWithDoubleJump
	{
		
		#region members
		

		/// <summary>
		/// The (horizontal) speed the character moves at in the air while run is depressed.
		/// </summary>
		[TaggedProperty ("speedLimit")]
		[TaggedProperty ("agility")]
		public float airSpeedRun;

		/// <summary>
		/// Rate at which the air speed in X increases when holding left or right.
		/// </summary>
		[TaggedProperty ("agility")]
		[TaggedProperty ("acceleration")]
		public float airSpeedAcceleration;

		/// <summary>
		/// Rate at which the air speed in X slows down when not holding left or right.
		/// </summary>
		[TaggedProperty ("agility")]
		[TaggedProperty ("acceleration")]
		public float airSpeedDeceleration;

		
		/// <summary>
		/// If the previous movement was a run movement, then this should be a run too.
		/// </summary>
		protected bool isForceRun;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Physics/Variable Height Jump with Double";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An air movement that has a variable height jump and in which the horizontal speed " +
						"has some inertia before coming to a stop (i.e. is not instantly changed). Similar to Super Meat boy. This " +
						"version separates acceleration and drag and also includes a FIXED height double jumo.";
		
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
		/// The default air speed while run is depressed.
		/// </summary>
		protected const float DefaultAirSpeedRun = 8.0f;

		/// <summary>
		/// The default air speed acceleration.
		/// </summary>
		protected const float DefaultAirSpeedAcceleration = 45.0f;

		/// <summary>
		/// The default air speed deceleration.
		/// </summary>
		protected const float DefaultAirSpeedDeceleration = 25.0f;

		/// <summary>
		/// The index of the air speed run in the movement settings.
		/// </summary>
		protected const int AirSpeedRunIndex = 9; 

		/// <summary>
		/// The index of the air speed acceleration in the movement settings.
		/// </summary>
		protected const int AirSpeedAccelerationIndex = 10;
		
		/// <summary>
		/// The index of the air speed acceleration in the movement settings.
		/// </summary>
		protected const int AirSpeedDecelerationIndex = 11;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 12;

		
		#endregion

		
		#region public methods

		
		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			this.character = character;

			// Calculate initial velocity and acceleration
			initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * relativeJumpGravity * minJumpHeight);
			return this;
		}
		
		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			base.Init (character, movementData);

			// Set variables
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				airSpeedRun = movementData[AirSpeedRunIndex].FloatValue;
				airSpeedAcceleration = movementData[AirSpeedAccelerationIndex].FloatValue;
				airSpeedDeceleration = movementData[AirSpeedDecelerationIndex].FloatValue;
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
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			isForceRun = false;
		}
		
		/// <summary>
		///  Do the jump by translating and applying velocity.
		/// </summary>
		override public void DoJump()
		{
			GainControl ();
			base.DoJump ();
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
				if (runButton == ButtonState.DOWN || runButton == ButtonState.HELD || isForceRun)
				{
					character.AddVelocity(airSpeedAcceleration * TimeManager.FrameTime, 0, false);
					if (character.Velocity.x > airSpeedRun) character.SetVelocityX(airSpeedRun);
				} 
				else 
				{
					if (character.Velocity.x > airSpeed) 
					{
						character.AddVelocity(-airSpeedAcceleration * TimeManager.FrameTime, 0, false);
						if (character.Velocity.x < airSpeed) character.SetVelocityX(airSpeed);
					}
					else
					{
						character.AddVelocity(airSpeedAcceleration * TimeManager.FrameTime, 0, false);
						if (character.Velocity.x > airSpeed) character.SetVelocityX(airSpeed);
					}
				}
			}
			else if (horizontalAxisDigital == -1)
			{
				if (runButton == ButtonState.DOWN || runButton == ButtonState.HELD || isForceRun)
				{
					character.AddVelocity(-airSpeedAcceleration * TimeManager.FrameTime, 0, false);
					if (character.Velocity.x < -airSpeedRun) character.SetVelocityX(-airSpeedRun);
				} 
				else 
				{
					if (character.Velocity.x < -airSpeed) 
					{
						character.AddVelocity(airSpeedAcceleration * TimeManager.FrameTime, 0, false);
						if (character.Velocity.x > -airSpeed) character.SetVelocityX(-airSpeed);
					}
					else
					{
						character.AddVelocity(-airSpeedAcceleration * TimeManager.FrameTime, 0, false);
						if (character.Velocity.x < -airSpeed) character.SetVelocityX(-airSpeed);
					}
				}
			}
			else
			{
				if (character.Velocity.x > 0)
				{
					character.AddVelocity(-airSpeedDeceleration * TimeManager.FrameTime, 0, false);
					if (character.Velocity.x < 0) character.SetVelocityX(0);
				}
				else if (character.Velocity.x < 0)
				{
					character.AddVelocity(airSpeedDeceleration * TimeManager.FrameTime, 0, false);
					if (character.Velocity.x > 0) character.SetVelocityX(0);
				}
			}
			character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, true);
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

			// Air acceleration
			if (movementData[AirSpeedAccelerationIndex] == null) movementData[AirSpeedAccelerationIndex] = new MovementVariable(DefaultAirSpeedAcceleration);
			movementData[AirSpeedAccelerationIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Air Speed Acceleration(x)", "Rate at which the character moves to the desired air speed when left or right is held."), movementData[AirSpeedAccelerationIndex].FloatValue);
			if (movementData[AirSpeedAccelerationIndex].FloatValue < 0) movementData[AirSpeedAccelerationIndex].FloatValue = 0.0f;

			// Air Deceleration
			if (movementData[AirSpeedDecelerationIndex] == null) movementData[AirSpeedDecelerationIndex] = new MovementVariable(DefaultAirSpeedDeceleration);
			movementData[AirSpeedDecelerationIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Air Speed Deceleration(x)", "Rate at which the character slows down to 0 when left and right not held."), movementData[AirSpeedDecelerationIndex].FloatValue);
			if (movementData[AirSpeedDecelerationIndex].FloatValue < 0) movementData[AirSpeedDecelerationIndex].FloatValue = 0.0f;

			// Run speed
			if (movementData[AirSpeedRunIndex] == null) movementData[AirSpeedRunIndex] = new MovementVariable(DefaultAirSpeedRun);
			movementData[AirSpeedRunIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Air Speed Run(x)", "How fast the character moves in the X direction whle in the air when run is depressed."), movementData[AirSpeedRunIndex].FloatValue);
			if (movementData[AirSpeedRunIndex].FloatValue < 0) movementData[AirSpeedRunIndex].FloatValue = 0.0f;

			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = AirMovement_VariableWithDoubleJump.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			return movementData;
		}
		
		#endregion

#endif
	}

}