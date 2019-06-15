#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Movement for crouching and creeping while crouched.
	/// </summary>
	public class GroundMovement_CrouchWithCrawl : GroundMovement_Crouch
	{
		
		#region members
		
		/// <summary>
		/// The speed the character moves at.
		/// </summary>
		[TaggedProperty ("agility")]
		[TaggedProperty ("speedLimit")]
		[TaggedProperty ("groundSpeedLimit")]
		public float speed;

		// TODO Adjust head height

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Crouch/Digital";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Crouch movement which allows creeping back and forwards at a constant speed.";
		
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
		private const int SpeedIndex = 5;
		
		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 6;
		
		#endregion
		
		#region public methods
		
		/// <summary>
		/// If this is true then the movement wants to control the object on thre ground.
		/// </summary>
		/// <returns><c>true</c> if control is wanted, <c>false</c> otherwise.</returns>
		override public bool WantsGroundControl()
		{
			if (!enabled) return false;
			if (character.Grounded && CheckInput())
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			Shrink ();
		}
		
		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions. You should
		/// ensure that character velocity is reset back to world-relative velocity here.
		/// </summary>
		override public void LosingControl()
		{
			Grow ();
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
			if (character.Input.HorizontalAxisDigital == 1)
			{
				character.SetVelocityX(speed);
				character.Translate(speed * TimeManager.FrameTime, 0, false);
			}
			else if (character.Input.HorizontalAxisDigital == -1)
			{
				character.SetVelocityX(-speed);
				character.Translate(-speed * TimeManager.FrameTime, 0, false);
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
			if (movementData != null && movementData.Length > SpeedIndex)
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
				if (character.Input.HorizontalAxisDigital == 0 || speed == 0)
				{
					return AnimationState.CROUCH;
				}
				else
				{
					return AnimationState.CROUCH_WALK;
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
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}
			
			// Walk speed
			if (movementData[SpeedIndex] == null) movementData[SpeedIndex] = new MovementVariable();
			movementData[SpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Speed", "How fast the character walks."), movementData[SpeedIndex].FloatValue);
			if (movementData[SpeedIndex].FloatValue < 0) movementData[SpeedIndex].FloatValue = 0.0f;

			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = GroundMovement_Crouch.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);


			return movementData;
		}
		
		#endregion
		
		#endif
	}
	
}

