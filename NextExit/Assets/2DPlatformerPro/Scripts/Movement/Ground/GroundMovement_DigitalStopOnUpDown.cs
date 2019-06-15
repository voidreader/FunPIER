#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement digital which doesn't allow running if player is holding up or down.
	/// </summary>
	public class GroundMovement_DigitalStopOnUpDown : GroundMovement, IFlippableGravityMovement
	{

		#region members

		/// <summary>
		/// The speed the character moves at.
		/// </summary>
		[TaggedProperty ("speed")]
		[TaggedProperty ("maxSpeed")]
		[TaggedProperty ("groundSpeed")]
		public float speed;

		/// <summary>
		/// Does the user need to release the d-pad/arrows in order to start running after aiming.
		/// </summary>
		public bool mustReleaseKeysAfterAiming;

		/// <summary>
		/// Has the character started pressing up down? i.e. started aiming
		/// </summary>
		protected bool upDownStarted;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Digital/No Up or Down";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Digital ground movement that doesn't run when player holds up or down. Generally used for aiming games.";
		
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
		/// The index for the ignore horizontal time in the movement data.
		/// </summary>
		protected const int MustReleaseKeysAfterAimingIndex = 1;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 2;

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update()
		{
			// The user must let go of the d-pad/arrow keys before they can start running
			if (mustReleaseKeysAfterAiming && upDownStarted) 
			{
				if (character.Input.VerticalAxisDigital == 0 && character.Input.HorizontalAxisDigital == 0) upDownStarted = false;
			}
		}

		#endregion

		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			float frameSpeed = speed;
			if (character.Friction > 2.0f) speed *= (2.0f / character.Friction );
#if UNITY_EDITOR
			if (Character.Friction >= 0 && Character.Friction < 2.0f) Debug.LogError("A friction less than 2 has no affect on digitial movements.");
#endif

			if (character.Input.VerticalAxisDigital != 0) upDownStarted = true;
			if (!upDownStarted) 
			{
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
			else
			{
				character.SetVelocityX(0);
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
				mustReleaseKeysAfterAiming = movementData[MustReleaseKeysAfterAimingIndex].BoolValue;
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
				if (upDownStarted || character.Input.HorizontalAxisDigital == 0)
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

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			upDownStarted = false;
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

			// Walk speed
			if (movementData[SpeedIndex] == null) movementData[SpeedIndex] = new MovementVariable();
			movementData[SpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Speed", "How fast the character walks."), movementData[SpeedIndex].FloatValue);
			if (movementData[SpeedIndex].FloatValue < 0) movementData[SpeedIndex].FloatValue = 0.0f;

			// Must Release Keys After Aiming
			if (movementData[MustReleaseKeysAfterAimingIndex] == null) movementData[MustReleaseKeysAfterAimingIndex] = new MovementVariable(true);
			movementData[MustReleaseKeysAfterAimingIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Must Release After Aiming", "If the user starts aiming with up and down and this is true the user must let go of the arrows/d-pad in order to start running again."), movementData[MustReleaseKeysAfterAimingIndex].BoolValue);

			return movementData;
		}

		#endregion

#endif
	}

}

