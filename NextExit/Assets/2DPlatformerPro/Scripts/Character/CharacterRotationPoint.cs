using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Describes ways of rotating the character to a slope.
	/// </summary>
	public enum CharacterRotationType
	{
		TRANSFORM_TO_AVERAGE,		// Rotate about the characters transform and rotate to the average position.
		COLLIDING_TO_AVERAGE,		// Rotate around the middle of the colliding feet and rotate to the average position.
		MIDDLE_TO_AVERAGE,			// Rotate about the middle of the feet and rotate to the average position.
	}

	public static class CharacterRotationTypeExtensions
	{
		public static string GetDescription(this CharacterRotationType me)
		{
			switch(me)
			{
				case CharacterRotationType.TRANSFORM_TO_AVERAGE: return "Rotate about the characters transform and rotate to the average position.";
				case CharacterRotationType.COLLIDING_TO_AVERAGE: return "Rotate around the middle of the colliding feet and rotate to the average position. Default.";
				case CharacterRotationType.MIDDLE_TO_AVERAGE: return "Rotate about the middle of the feet and rotate to the average position.";
			}
			return "No information available.";
		}
	}
}