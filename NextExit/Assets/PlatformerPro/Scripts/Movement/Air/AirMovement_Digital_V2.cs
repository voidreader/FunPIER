#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Air movement digital.
	/// </summary>
	public class AirMovement_Digital_V2 : AirMovement_Digital
	{
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Digital/Basic V2";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which has a fixed jump height and fixed air speed. This version sends the DOUBLE_JUMP state on double jump.";
		
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
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (showJumpStartedAnimation)
				{
					showJumpStartedAnimation = false;
					if (jumpCount > 1) return AnimationState.DOUBLE_JUMP;
					return AnimationState.JUMP;
				}
				else if (character.Velocity.y >= 0)
				{
					return AnimationState.AIRBORNE;
				}
				else
				{
					return AnimationState.FALL;
				}
			}
		}


		#endregion

#if UNITY_EDITOR
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			return AirMovement_Digital.DrawInspector(movementData, ref showDetails, target);
		}
#endif
	}

}