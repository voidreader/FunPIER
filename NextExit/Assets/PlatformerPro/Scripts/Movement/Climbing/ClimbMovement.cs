using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A wrapper class for handling moving on ladders and other climbables that proxies the movement function
	/// to the desired implementation.
	/// </summary>
	public class ClimbMovement : BaseMovement <ClimbMovement>
	{

		#region ladder specific properties and methods
		
		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate the climbing.
		/// </summary>
		virtual public bool WantsClimb()
		{
			return false;
		}

		/// <summary>
		/// Should we find ladders by tag or by layer. If true we will use tags, if false we will use layers. Tags are easier to use
		/// but come with an allocation cost.
		/// </summary>
		virtual public bool DetectLaddersByTag
		{
			get; protected set;
		}
		
		/// <summary>
		/// The name of the ladder lyaer if we find by layers, or the tag name if we find by tags.
		/// </summary>
		virtual public string LadderLayerOrTagName
		{
			get; protected set;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// rotations to be calculated and applied by the character. By default ladders don't do this.
		/// </summary>
		override public bool ShouldDoRotations
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Climb Movement";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "The base ladder movement class, you shouldn't be seeing this did you forget to create a new MovementInfo?";

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
	}

	/// <summary>
	/// Ways that a ladder can be dismounted.
	/// </summary>
	[System.Flags]
	public enum LadderDismountType
	{
		NONE = 0,
		TOP_BOTTOM = 1, 		// Climbing beyond bounds dismounts ladder
		LEFT_RIGHT = 2,			// Pressing left or right dismounts ladder
		JUMP = 4,				// Jumping dismounts ladder

		TOP_BOTTOM_AND_LEFT_RIGHT = TOP_BOTTOM | LEFT_RIGHT,
		JUMP_AND_LEFT_RIGHT = JUMP | LEFT_RIGHT,
		TOP_BOTTOM_AND_JUMP = TOP_BOTTOM | JUMP,
		TOP_BOTTOM_AND_JUMP_AND_LEFT_RIGHT = TOP_BOTTOM | JUMP | LEFT_RIGHT

	}
}