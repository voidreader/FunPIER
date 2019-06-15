using UnityEngine;
using System.Collections;


namespace PlatformerPro
{
	/// <summary>
	/// How do we detect that ther eis enough gap (i.e. a place for the character to be) for a ledge climb to start.
	/// </summary>
	public enum GapDetectionType
	{
		NONE,					// The ledge will be detected by a tag/layer and it will be assumed there is a gap.
		RAYCAST_SIDE_COLLIDERS	// A ray will be projected for each side collider as if the chracter were standing on the ledge position, if nothing is hit there is a gap.

	}

	/// <summary>
	/// How do we dectect where the edge of the ledge is (i.e. where the characters hands should be).
	/// </summary>
	public enum LedgeDetectionType
	{
		NONE,					// The ledge will be detected by a tag/layer and the position of the transform will be the ledge position.
		BOX,					// The edge of the box collider 2D will be considered the ledge position.
		CIRCLE_CAST				// A circle will be cast from above the character and the first point hit on the ledge collider will be considered the ledge position.
	}

	/// <summary>
	/// Ledge climb state.
	/// </summary>
	public enum LedgeClimbState 
	{
		NONE,
		REACHING,
		GRASPING,
		HANGING,
		FALLING,
		CLIMBING,
		CLIMB_FINISHED,
		DISMOUNTING,
		DISMOUNT_FINISHED
	}

	/// <summary>
	/// How do we target the character at the ledge.
	/// </summary>
	public enum LedgeClimbAnimationTargetting 
	{
		SPRITE_PIVOT,		// Use the sprite pivot.
		BAKED				// Transform is baked in to the animation.
	}
	
	public static class LedgeClimbEnumExtensions
	{
		public static string GetDescription(this GapDetectionType me)
		{
			switch(me)
			{
			case GapDetectionType.NONE: return "The ledge will be detected by a tag/layer and it will be assumed there is a gap.";
			case GapDetectionType.RAYCAST_SIDE_COLLIDERS: return "A ray will be projected for each side collider as if the chracter were standing on the ledge position, if nothing is hit there is a gap.";
			}
			return "No information available.";
		}

		public static string GetDescription(this LedgeDetectionType me)
		{
			switch(me)
			{
			case LedgeDetectionType.NONE: return "The ledge will be detected by a tag/layer and the position of the transform will be the ledge position.";
			case LedgeDetectionType.BOX: return "The edge of the BoxCollider2D (plus the ledge offset) will be considered the ledge position.";
			case LedgeDetectionType.CIRCLE_CAST: return "A circle will be cast from above the character and the first point hit on the ledge collider will be considered the ledge position.";
			}
			return "No information available.";
		}

		public static string GetDescription(this LedgeClimbAnimationTargetting me)
		{
			switch(me)
			{
			case LedgeClimbAnimationTargetting.SPRITE_PIVOT: return "The characters pivot point will be moved towards the target position.";
			case LedgeClimbAnimationTargetting.BAKED: return "Move the select bone to the target position then play the baked animation.";
			}
			return "No information available.";
		}

	}

}
