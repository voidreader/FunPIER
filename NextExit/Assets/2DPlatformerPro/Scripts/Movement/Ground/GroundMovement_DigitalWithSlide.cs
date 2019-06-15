#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement with three state digital movement ans liding.
	/// </summary>
	public class GroundMovement_DigitalWithSlide : GroundMovement_Digital
	{

		#region members
		
		/// <summary>
		/// Characters friction.
		/// </summary>
		[TaggedProperty ("friction")]
		public float friction;

		/// <summary>
		/// The quiesce speed.
		/// </summary>
		public float quiesceSpeed;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Digital/With Slide";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which has 3 speeds plus sliding.";
		
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
		/// The index for the drag value in the movement data.
		/// </summary>
		protected const int FrictionIndex = 1;

		/// <summary>
		/// The index of the queisce speed in the movement data.
		/// </summary>
		protected const int QuiesceSpeedIndex = 2;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		protected const int MovementVariableCount = 3;

		/// <summary>
		/// The default friction.
		/// </summary>
		public const float DefaultFriction = 5.0f;

		/// <summary>
		/// The default quiesce speed.
		/// </summary>
		protected const float DefaultQuiesceSpeed = 0.5f;

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
				friction = movementData[FrictionIndex].FloatValue;
				quiesceSpeed = movementData[QuiesceSpeedIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid mvoement data");
			}
			
			return this;
		}


		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// Set frame speed - if friction is bigger than 2 we will slow the character down.
			float frameSpeed = speed;
			if (character.Friction > 2.0f) speed *= (2.0f / character.Friction );

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
				float actualFriction = character.Friction;
				if (actualFriction == -1) actualFriction = friction;
				if (character.Velocity.x > 0) 
				{
					character.AddVelocity(-character.Velocity.x * actualFriction * TimeManager.FrameTime, 0, false);
					if (character.Velocity.x < quiesceSpeed) character.SetVelocityX(0);
				}
				else if (character.Velocity.x < 0) 
				{
					character.AddVelocity(-character.Velocity.x * actualFriction * TimeManager.FrameTime, 0, false);
					if (character.Velocity.x > -quiesceSpeed) character.SetVelocityX(0);
				}
				if (character.Velocity.x != 0)
				{
					character.Translate((character.IsGravityFlipped ? -character.Velocity.x : character.Velocity.x ) * TimeManager.FrameTime, 0, false);
				}
			}
		}

		
		/// <summary>
		/// Does any movement that MUST be done after collissions are calculated.
		/// </summary>
		override public void PostCollisionDoMove() {
			if (enabled && !character.rotateToSlopes) SnapToGround ();
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
					if (character.Velocity.x != 0) return AnimationState.SLIDE;
					return AnimationState.IDLE;
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

			// Friction 
			if (movementData[FrictionIndex] == null) movementData[FrictionIndex] = new MovementVariable(DefaultFriction);
			movementData[FrictionIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Friction", "Controls how quickly the character comes to a stop."), movementData[FrictionIndex].FloatValue);

			// Quiesce Speed 
			if (movementData[QuiesceSpeedIndex] == null) movementData[QuiesceSpeedIndex] = new MovementVariable(DefaultQuiesceSpeed);
			movementData[QuiesceSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Quiesce Speed", "Speed at which slide turns to stop."), movementData[QuiesceSpeedIndex].FloatValue);
			if (movementData[QuiesceSpeedIndex].FloatValue < 0) movementData[QuiesceSpeedIndex].FloatValue = 0.0f;


			return movementData;
		}
		
		#endregion
		
#endif

	}
}