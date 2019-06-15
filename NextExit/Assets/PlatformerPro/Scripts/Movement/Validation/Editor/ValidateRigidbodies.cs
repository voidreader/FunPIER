using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Validation
{
	/// <summary>
	/// Validate hurt boxes and hitboxes ahve rigidbodies attached.
	/// </summary>
	public class ValidateRigidbodies : IValidator
	{
		protected const int EXPECTED_MAX_LAYER = 15;

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
            EnemyHitBox[] allEnemyHitBoxes = GameObject.FindObjectsOfType<EnemyHitBox>();
            foreach (EnemyHitBox c in allEnemyHitBoxes)
            {
                result.AddRange(CheckRigidbody(c, true));
                result.AddRange(CheckCollider(c, true));
            }
            // Get all hitboxes
            EnemyHazard[] allEnemyHurtboxes = GameObject.FindObjectsOfType<EnemyHazard>();
            foreach (EnemyHazard c in allEnemyHurtboxes)
            {
                result.AddRange(CheckRigidbody(c, true));
                result.AddRange(CheckCollider(c, false));
            }
            return result;
        }

        public List<ValidationResult> Validate(Character character) 
        {
            List<ValidationResult> result = new List<ValidationResult>();

            // Get all hitboxes
            CharacterHitBox[] allCharacterHitBoxes = character.GetComponentsInChildren<CharacterHitBox> ();
			foreach (CharacterHitBox c in allCharacterHitBoxes)
			{
				result.AddRange (CheckRigidbody(c, true));
				result.AddRange (CheckCollider(c, true));
			}

			// Get all hurtboxes
			CharacterHurtBox[] allCharacterHurtBoxes = character.GetComponentsInChildren<CharacterHurtBox> ();
			foreach (CharacterHurtBox c in allCharacterHurtBoxes)
			{
				result.AddRange (CheckRigidbody(c, true));
				result.AddRange (CheckCollider(c, false));
			}
			
			return result;
		}

		
		/// <summary>
		/// Validates  rigidbody settings.
		/// </summary>
		protected List<ValidationResult> CheckRigidbody(MonoBehaviour m, bool isWarning)
		{
			
			List<ValidationResult> result = new List<ValidationResult> ();

			if (m.GetComponent<Rigidbody2D>() == null)
			{
				result.Add (new ValidationResult("Found a  " + m.GetType().Name + " but it did not have a rigidbody attached.", isWarning ? MessageType.Warning : MessageType.Info));
			}
			else if (!m.GetComponent<Rigidbody2D>().isKinematic)
			{
				result.Add (new ValidationResult("Found a  " + m.GetType().Name  + " but the rigidboy was not kinematic.", isWarning ? MessageType.Warning : MessageType.Info));
			}
			return result;
		}

		/// <summary>
		/// Validates collider settings.
		/// </summary>
		protected List<ValidationResult> CheckCollider(MonoBehaviour m, bool checkTrigger)
		{
			
			List<ValidationResult> result = new List<ValidationResult> ();
			
			if (m.GetComponent<Collider2D>() == null)
			{
				result.Add (new ValidationResult("Found a  " + m.GetType().Name + " but it did not have a Collider2D attached.",  MessageType.Warning));
			}
			else if (checkTrigger && !m.GetComponent<Collider2D>().isTrigger)
			{
				result.Add (new ValidationResult("Found a  " + m.GetType().Name  + " but the collider2D was not a trigger.", MessageType.Info));
			}
			return result;
		}
	}
}