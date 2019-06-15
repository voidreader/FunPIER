#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement that lets mecanim do the movement.
	/// </summary>
	public class GroundMovement_MecanimAnimationDriven: GroundMovement_Physics
	{

		#region members

		#endregion

		#region constants

		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Mecanim/Animation Driven/Physics";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement that lets mecanim do the movement with a physcis style movement control";
		
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
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 8;

		#endregion 

		/// <summary>
		/// Sets the character speed.
		/// </summary>
		override public void DoMove()
		{
			float actualFriction = character.Friction;
			if (actualFriction == -1) actualFriction = friction;
			
			// Apply drag
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
			
			// Apply acceleration
			if (Mathf.Abs (character.Input.HorizontalAxisDigital * acceleration) > ignoreForce)
			{
				character.AddVelocity((useAnalogueInput ? character.Input.HorizontalAxis : (float)character.Input.HorizontalAxisDigital) * acceleration * TimeManager.FrameTime, 0, false);
				hasAppliedForce = true;
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
			if (!hasAppliedForce)
			{
				if (character.Velocity.x > 0 && character.Velocity.x < quiesceSpeed ) 
				{
					character.SetVelocityX(0);
				}
				else if (character.Velocity.x  < 0 && character.Velocity.x > -quiesceSpeed)
				{
					character.SetVelocityX(0);
				}
			}
			
			// Translate
			// character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, false);
			
			hasAppliedForce = false;
		}
		
		/// <summary>
		/// Does any movement that MUST be done after collissions are calculated.
		/// </summary>
		override public void PostCollisionDoMove() {
			if (enabled && stickToGround) SnapToGround ();
		}

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
			MovementVariable[] baseMovementData = GroundMovement_Physics.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);
			
			
			return movementData;
		}

		#endregion

#endif

	}

}

