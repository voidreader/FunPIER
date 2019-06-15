#if UNITY_EDITOR
using UnityEditor;
using PlatformerPro.Validation;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement_ digital.
	/// </summary>
	public class GroundMovement_Digital : GroundMovement, IFlippableGravityMovement
	{

		#region members

		/// <summary>
		/// The speed the character moves at.
		/// </summary>
		[TaggedProperty ("agility")]
		[TaggedProperty ("speedLimit")]
		[TaggedProperty ("groundSpeedLimit")]
		public float speed;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Digital/Basic";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which has only 3 speeds: NONE, LEFT and RIGHT";
		
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
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 1;

		/// <summary>
		/// The default speed.
		/// </summary>
		protected const float DefaultSpeed = 5.0f;

		#endregion

		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// Set frame speed - if friction is bigger than 2 we will slow the character down.
			float frameSpeed = GetSpeed(speed);
			frameSpeed = ApplySlopeSpeedModifier(frameSpeed);
			if (character.Friction > 2.0f) frameSpeed *= (2.0f / character.Friction );
#if UNITY_EDITOR
			if (character.Friction >= 0 && Character.Friction < 2.0f) Debug.LogError("A friction less than 2 has no affect on digitial movements.");
#endif

			if (character.Input.HorizontalAxisDigital == 1)
			{
				character.SetVelocityX(character.IsGravityFlipped ? -frameSpeed : frameSpeed);
				character.Translate((character.IsGravityFlipped ? -frameSpeed : frameSpeed) * TimeManager.FrameTime, 0, false);
			}
			else if (character.Input.HorizontalAxisDigital == -1)
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
        /// Snap to ground if set.
        /// </summary>
		override public void PostCollisionDoMove() {
			if (enabled && !character.rotateToSlopes) SnapToGround ();
		}

		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			if (character != null) AssignReferences (character);
	
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				speed = movementData[SpeedIndex].FloatValue;
			}
			else
			{
				Debug.LogError("No speed has been set");
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
				else
				{
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
				if (character.IsGravityFlipped) return -character.Input.HorizontalAxisDigital;
				return character.Input.HorizontalAxisDigital;
			}
		}

		#endregion

#if UNITY_EDITOR

		/// <summary>
		/// Custom movement validation.
		/// </summary>
		/// <returns>A list of validation errors</returns>
		/// <param name="c">Character to validate against.</param>
		override public List<ValidationResult> ValidateMovement(Character c)
		{
			// Speed
			List<ValidationResult> result = new List<ValidationResult> ();
            Debug.Log(speed);
			if (speed == 0) result.Add(new ValidationResult("Ground movement speed is 0.", MessageType.Warning));

			// Physics things that wont work with this movement
			if (FindObjectOfType(typeof(WindAffector)) != null)
			{
				result.Add(new ValidationResult("Note that WindAffectors wont be able to push characters with a digital ground movement. You may want to use a physics movement instead.", MessageType.Info));
			}

			// Check stick to ground
			if (speed > 0)
			{
				float slope = Mathf.Atan (c.groundedLookAhead / (TimeManager.DefaultMaximumFrameTime * speed)) * Mathf.Rad2Deg;
				if (slope < 45.0f)
				{
					result.Add(new ValidationResult(string.Format("Your grounded lookahead and speed mean you will only stick to slopes with an angle of {0} degrees or less.", Mathf.RoundToInt(slope)), MessageType.Info));	
				}
			}

			return result;
		}

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

			// Walk speed
			if (movementData[SpeedIndex] == null) movementData[SpeedIndex] = new MovementVariable(DefaultSpeed);
			movementData[SpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Speed", "How fast the character walks."), movementData[SpeedIndex].FloatValue);
			if (movementData[SpeedIndex].FloatValue < 0) movementData[SpeedIndex].FloatValue = 0.0f;

			return movementData;
		}

		#endregion

#endif
	}

}

