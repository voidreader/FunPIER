#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PlatformerPro
{
	/// <summary>
	/// A wall movement class that lets you jump away from a wall only when sliding down.
	/// </summary>
	public class WallMovement_WallJump_V3: WallMovement_WallJump
	{
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Wall Jump/Standard (Jump on Slide)";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A wall movement that allows you to jump away from a wall with various controls, and requires you to be moving downwards to jump. " +
			"When you jump away you can only move in the opposite direction of wall until you start falling.";
		
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

		#endregion
		

		
		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (movingAwayFromWall || character.CurrentWall == null)
			{
				// Do air movement
				character.DefaultAirMovement.DoOverridenMove (true, true, (float) -cachedWallDirection, character.Input.RunButton);

				// Check for the end of the wall jump
				if (character.Grounded || character.Velocity.y <= speedWhereWallJumpEnds)
				{
					movingAwayFromWall = false;
				}
			}
			else
			{
				// Check for Jump
				if (character.Velocity.y < minSpeedForWallJump && CheckControls())
				{
					character.DefaultAirMovement.DoOverridenJump(jumpHeight, 1);
					movingAwayFromWall = true;
				}

				// Gravity
				if (character.Velocity.y > 0)
				{
					// Moving up - Apply normal gravity
					character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
				}
				else if (character.Velocity.y > clingTargetSpeed)
				{

					// Moving down - Apply reduced gravity
					character.AddVelocity(0, TimeManager.FrameTime * clingGravity, false);
					if (character.Velocity.y < clingTargetSpeed) character.SetVelocityY(clingTargetSpeed);
				}
				else if (character.Velocity.y < clingTargetSpeed)
				{
					// Moving too fast  - Apply 2 times -gravity 
					// TODO This could become a variable too
					character.AddVelocity(0, TimeManager.FrameTime * 2 * -clingGravity, false);
					if (character.Velocity.y > clingTargetSpeed) character.SetVelocityY(clingTargetSpeed);
				}

				// Translate
				character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
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
			return WallMovement_WallJump.DrawInspector(movementData, ref showDetails, target);
		}
		
		#endregion
		
#endif
		
	}
}

