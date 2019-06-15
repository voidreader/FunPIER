#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Movement for crouching that will slide if moving at speed.
	/// </summary>
	public class GroundMovement_CrouchWithSlide : GroundMovement_Crouch
	{
		
		#region members
		
		/// <summary>
		/// How fast the character has to be moving for slide to start.
		/// </summary>
		public float minimumSpeedForSlide;

		/// <summary>
		/// The maximum travel speed.
		/// </summary>
		[TaggedProperty ("speed")]
		[TaggedProperty ("maxSpeed")]
		public float maxSpeed;

		/// <summary>
		/// The default friction. Platforms can override this.
		/// </summary>
		[TaggedProperty("friction")]
		public float friction;
		
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
		/// Tracks if crouch has started.
		/// </summary>
		protected bool crouchStarted;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Crouch/With Slide";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Crouch movement that will slide if you crouch while running.";
		
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
		/// The default minimum speed for slide.
		/// </summary>
		protected const float DefaultMinimumSpeedForSlide = 1.0f;

		/// <summary>
		/// The index for the minimum speed for slide value in the movement data.
		/// </summary>
		protected const int MinimumSpeedForSlideIndex = 10;

		/// <summary>
		/// The index of the max speed.
		/// </summary>
		protected const int MaxSpeedIndex = 11;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 12;

		/// <summary>
		/// Offset for where we will place the standard physics data in the movement data.
		/// </summary>
		private const int PhysicsMovementDataIndexOffset = 5;

		#endregion
		
		#region public methods
		
		/// <summary>
		/// If this is true then the movement wants to control the object on thre ground.
		/// </summary>
		/// <returns><c>true</c> if control is wanted, <c>false</c> otherwise.</returns>
		override public bool WantsGroundControl()
		{
			if (!enabled) return false;
			if (character.Grounded && CheckInput () && 
			    (crouchStarted || Mathf.Abs (character.Velocity.x) >= minimumSpeedForSlide))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't.
		/// </summary>
		override public bool WantsControl()
		{
			if (character.WouldHitHeadThisFrame) return true;
			return false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			float actualFriction = character.Friction;
			if (actualFriction == -1) actualFriction = friction;
			
			// Apply drag
			if (!character.WouldHitHeadThisFrame || Mathf.Abs (character.Velocity.x) > (maxSpeed / 2.0f)) 
			{
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
			if (character.Velocity.x > 0 && character.Velocity.x < quiesceSpeed ) 
			{
				character.SetVelocityX(0);
			}
			else if (character.Velocity.x  < 0 && character.Velocity.x > -quiesceSpeed)
			{
				character.SetVelocityX(0);
			}

			// Translate
			character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, false);

		}
		
		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			base.Init (character);
			return this;
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
				minimumSpeedForSlide = movementData[MinimumSpeedForSlideIndex].FloatValue;
				friction = movementData[GroundMovement_Physics.FrictionIndex + PhysicsMovementDataIndexOffset].FloatValue;
				slopeAccelerationFactor = movementData[GroundMovement_Physics.SlopeAccelerationIndex + PhysicsMovementDataIndexOffset].FloatValue;
				quiesceSpeed = movementData[GroundMovement_Physics.QuiesceSpeedIndex + PhysicsMovementDataIndexOffset].FloatValue;
				ignoreForce = movementData[GroundMovement_Physics.IgnoreForceIndex + PhysicsMovementDataIndexOffset].FloatValue;
				maxSpeed = movementData[MaxSpeedIndex].FloatValue;
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
				if (character.Velocity.x != 0) return AnimationState.CROUCH_SLIDE;
				return AnimationState.CROUCH;
			}
		}
		
		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (character.Velocity.x > 0) return 1;
				if (character.Velocity.x < 0) return -1;
				return 0;
			}
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			Shrink ();
			crouchStarted = true;
		}
		
		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions. You should
		/// ensure that character velocity is reset back to world-relative velocity here.
		/// </summary>
		override public void LosingControl()
		{
			Grow ();
			crouchStarted = false;
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

			// Start crouch only by pressing down
			if (movementData[MinimumSpeedForSlideIndex] == null) movementData[MinimumSpeedForSlideIndex] = new MovementVariable(DefaultMinimumSpeedForSlide);
			movementData[MinimumSpeedForSlideIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Minimum Speed for Slide", "How fast does the character have to go before they will go in to crouch slide instead of just crouching?"), movementData[MinimumSpeedForSlideIndex].FloatValue);
			if (movementData[MinimumSpeedForSlideIndex].FloatValue < 0) movementData[MinimumSpeedForSlideIndex].FloatValue = 0.0f;

			// Max Speed
			if (movementData[MaxSpeedIndex] == null) movementData[MaxSpeedIndex] = new MovementVariable();
			movementData[MaxSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Maximum Speed", "The characters peak speed, required as we may accelerate down a hill."), movementData[MaxSpeedIndex].FloatValue);
			if (movementData[MaxSpeedIndex].FloatValue < 0) movementData[MaxSpeedIndex].FloatValue = 0.0f;

			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = GroundMovement_Crouch.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			GroundMovement_Physics.DrawStandardPhysicsSettings (movementData, PhysicsMovementDataIndexOffset, showDetails);

			return movementData;
		}
		
		#endregion
		
		#endif
	}
	
}

