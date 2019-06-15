using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that is used for front or back of a loop. It works like a depth platform except it also
	/// prohibits jumping past a certian point which is useful for allow small/tight loops.
	/// </summary>
	public class LoopPlatform : DepthPlatform
	{
		/// <summary>
		/// The max angle player can be at before jumping is skipped.
		/// </summary>
		[Tooltip ("The maximum angle (in degrees) that the player can be at before jumping is ignored.")]
		[Range (0,90)]
		public float maxJumpAngle = 30.0f;

		/// <summary>
		/// Prevent air movement past a certain angle.
		/// </summary>
		/// <returns><c>true</c>, if movement should be skipped, <c>false</c> otherwise.</returns>
		/// <param name="character">Character being checked.</param>
		/// <param name="movement">Movement being checked.</param>
		override public bool SkipMovement(Character character, Movement movement)
		{
			if (movement is AirMovement && Mathf.Abs (character.SlopeActualRotation) >= maxJumpAngle) {
				return true;
			}
			return false;
		}
	}
}
