using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Validation
{
	/// <summary>
	/// Validate layer settings
	/// </summary>
	public class ValidateBasicMovements : IValidator
	{
		protected const float MINIMUM_GROUND_LOOKAHEAD = 0.05f;

		protected const float MINIMUM_FEET_LOOKAHEAD_FOR_SLOPES = 0.2f;

		protected const float EXPECTED_STICK_GROUND_LOOKAHEAD_RATIO = 1.0f;

        /// <summary>
        /// Apply this validation to the scene.
        /// </summary>
        public List<ValidationResult> Validate()
        {
            List<ValidationResult> result = new List<ValidationResult>();

            // Get all characters
            Character[] allCharacters = GameObject.FindObjectsOfType<Character>();
            foreach (Character c in allCharacters)
            {
                result.AddRange(Validate(c));
            }
            return result;
        }

        public List<ValidationResult> Validate(Character c)
        {
            List<ValidationResult> result = new List<ValidationResult>();

            result.AddRange (CheckForGround(c));
			result.AddRange (CheckForAir(c));
			
			return result;
		}

		/// <summary>
		/// Checks that character has a ground movement.
		/// </summary>
		protected List<ValidationResult> CheckForGround(Character c)
		{
			List<ValidationResult> result = new List<ValidationResult> ();

            // Look aheads
            if (c.GetComponentInChildren<GroundMovement>() == null)
            {
                ValidationResult r = new ValidationResult("You must have a ground movement as a child of your character!", MessageType.Error);
                result.Add(r);
            }
            return result;
		}

        /// <summary>
        /// Checks that character has an air movement.
        /// </summary>
        protected List<ValidationResult> CheckForAir(Character c)
        {
            List<ValidationResult> result = new List<ValidationResult>();

            // Look aheads
            if (c.GetComponentInChildren<AirMovement>() == null)
            {
                ValidationResult r = new ValidationResult("You must have an air movement as a child of your character!", MessageType.Error);
                result.Add(r);
            }
            return result;
        }
    }
}