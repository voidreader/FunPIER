#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Movement for crouching that doesn't allow you to move forward.
	/// </summary>
	public class GroundMovement_CrouchNoCrawl : GroundMovement_Crouch
	{
		
		#region members
		
		/// <summary>
		/// If true the character can only start crouching if the press directly down not diagonally down.
		/// </summary>
		public bool crouchStartMustBeDown;

		/// <summary>
		/// Tracks if crouch has started.
		/// </summary>
		protected bool crouchStarted;

		// TODO Adjust head height

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Crouch/Digital No Crawl";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Crouch movement that doesn't allow you to move forward or backward while crouching.";
		
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
		protected const int CrouchStartMustBeDownIndex = 5;
		
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
			if (character.Grounded && CheckInput () && (!crouchStartMustBeDown || character.Input.HorizontalAxisDigital == 0))
			{
				return true;
			}
			return false;
		}
		
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			character.SetVelocityX(0);
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
			
			if (movementData != null && movementData.Length > CrouchStartMustBeDownIndex)
			{
				crouchStartMustBeDown = movementData[CrouchStartMustBeDownIndex].BoolValue;
			}
			else
			{
				Debug.LogError("No crouch start must be down value has been set.");
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
				return AnimationState.CROUCH;
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

			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = GroundMovement_Crouch.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			// Start crouch only by pressing down
			if (movementData[CrouchStartMustBeDownIndex] == null) movementData[CrouchStartMustBeDownIndex] = new MovementVariable();
			movementData[CrouchStartMustBeDownIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Start Crouch With Down", "If true the character can only start the crouch when pressing directly down, not diagonally down."), movementData[CrouchStartMustBeDownIndex].BoolValue);

			return movementData;
		}
		
		#endregion
		
		#endif
	}
	
}

