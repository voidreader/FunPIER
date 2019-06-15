#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement with five state digital movement.
	/// </summary>
	public class GroundMovement_DigitalWithRun : GroundMovement_Digital
	{

		#region members
		
		/// <summary>
		/// The speed the character runs at.
		/// </summary>
		[TaggedProperty ("agility")]
		[TaggedProperty ("speedLimit")]
		[TaggedProperty ("groundSpeedLimit")]
		public float runSpeed;
		
		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Digital/With Run";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which has 5 speeds: NONE, LEFT, LEFT RUN, RIGHT and RIGHT RUN";
		
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
		/// The index for the run speed value in the movement data.
		/// </summary>
		protected const int RunSpeedIndex = 1;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		protected const int MovementVariableCount = 2;
		
		#endregion

		#region public methods
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// Set frame speed - if friction is bigger than 2 we will slow the character down.
			float frameSpeed = (character.Input.RunButton == ButtonState.HELD) ? runSpeed : speed;
			if (character.Friction > 2.0f) speed *= (2.0f / character.Friction );
#if UNITY_EDITOR
			if (Character.Friction >= 0 && Character.Friction < 2.0f) Debug.LogError("A friction less than 2 has no affect on digitial movements.");
#endif

			if (character.Input.HorizontalAxisDigital == 1)
			{
				if (character.IsGravityFlipped) frameSpeed *= -1;
				character.SetVelocityX(frameSpeed);
				character.Translate(frameSpeed * TimeManager.FrameTime, 0, false);
			}
			else if (character.Input.HorizontalAxisDigital == -1)
			{
				frameSpeed *= -1;
				character.SetVelocityX(frameSpeed);
				character.Translate(frameSpeed * TimeManager.FrameTime, 0, false);
			}
			else
			{
				character.SetVelocityX(0);
			}
		}

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
				runSpeed = movementData[RunSpeedIndex].FloatValue;
			}
			else
			{
				Debug.LogError("No run speed has been set");
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
				if (character.Input.HorizontalAxisDigital == 0)
				{
					return AnimationState.IDLE;
				}
				else if (character.Input.RunButton == ButtonState.HELD)
				{
					return AnimationState.RUN;
				}
				else
				{
					return AnimationState.WALK;
				}
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
			MovementVariable[] baseMovementData = GroundMovement_Digital.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			// Run speed
			if (movementData[RunSpeedIndex] == null) movementData[RunSpeedIndex] = new MovementVariable();
			movementData[RunSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Run Speed", "How fast the character runs."), movementData[RunSpeedIndex].FloatValue);
			if (movementData[RunSpeedIndex].FloatValue < 0) movementData[RunSpeedIndex].FloatValue = 0.0f;
			
			return movementData;
		}
		
		#endregion
		
#endif

	}
}