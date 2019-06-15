#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

namespace PlatformerPro.Experimental
{
	/// <summary>
	/// Air movement which floats when jump is pressed.
	/// </summary>
	public class AirMovement_Float: AirMovement
	{
		/// <summary>
		/// The float gravity.
		/// </summary>
		public float floatGravity;

		/// <summary>
		/// Track if a normal jump has started.
		/// </summary>
		protected bool jumpStarted;
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Float";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which floats when jump is pressed (you will also need another " +
			" Air Movement to handle jump).";
		
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
		/// The default float gravity.
		/// </summary>
		protected const float DefaultFloatGravity = 7.0f;

		/// <summary>
		/// The index of the float gravity.
		/// </summary>
		protected const int FloatGravityIndex = 0;
		
		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 1;

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
				return floatGravity;
			}
		}

		#endregion

		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			this.character = character;
			return this;
		}
		
		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			this.character = character;
			
			// Set variables
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				floatGravity = movementData[FloatGravityIndex].FloatValue;
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
			if (character.Input.JumpButton == ButtonState.HELD) return true;
			return false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// Use default X move
			character.DefaultAirMovement.DoOverridenMove (true, false, character.Input.HorizontalAxis, character.Input.RunButton);
			MoveInY();
		}

		/// Do the Y movement.
		/// </summary>
		override protected void MoveInY ()
		{
			// Apply gravity
			if (!character.Grounded)
			{
				character.AddVelocity(0, TimeManager.FrameTime * floatGravity, false);
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
				return AnimationState.FLOAT;
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

			if (movementData[FloatGravityIndex] == null) movementData[FloatGravityIndex] = new MovementVariable(DefaultFloatGravity);
			movementData[FloatGravityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Float Gravity", "Gravity to apply while floating."), movementData[FloatGravityIndex].FloatValue);
			if (movementData[FloatGravityIndex].FloatValue > 0) movementData[FloatGravityIndex].FloatValue = 0.0f;
			return movementData;
		}

		#endregion
#endif
	}
}
