using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Validation
{
	/// <summary>
	/// Validate raycast colliders.
	/// </summary>
	public class ValidateRaycastColliders : IValidator
	{
		protected const float EXPECTED_FEET_SPACING_WARNING = 0.75f;

		protected const float EXPECTED_FEET_SPACING_INFO = 0.25f;

		/// <summary>
		/// Apply this validation to the scene.
		/// </summary>
		public List<ValidationResult> Validate()
		{
			List<ValidationResult> result = new List<ValidationResult> ();

			// Get all characters
			Character[] allCharacters = GameObject.FindObjectsOfType<Character> ();

			foreach (Character c in allCharacters)
			{
                result.AddRange(Validate(c));

			}
			return result;
		}

        public List<ValidationResult> Validate(Character c)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            result.AddRange(CheckFeet(c));
            return result;
        }

        protected List<ValidationResult> CheckFeet(Character character)
		{

			List<ValidationResult> result = new List<ValidationResult> ();
			if (character.feetColliders.Length < 2)
			{
				result.Add (new ValidationResult("You MUST have at least two feet colliders", MessageType.Error));
				return result;
			}
			float feetDistance = Mathf.Abs (character.feetColliders [character.feetColliders.Length - 1].Extent.x - character.feetColliders [0].Extent.x) / (float)character.feetColliders.Length; 

			if (feetDistance > EXPECTED_FEET_SPACING_WARNING) 
			{
				result.Add (new ValidationResult("The spacing between your feet is quite large. You probably want to add more feet colldiers.", MessageType.Warning));
			}
			else if (feetDistance > EXPECTED_FEET_SPACING_INFO) 
			{
				result.Add (new ValidationResult("The spacing between your feet may be a little large. You may want to add more feet colldiers.", MessageType.Info));
			}
			return result;
		}
	}
}