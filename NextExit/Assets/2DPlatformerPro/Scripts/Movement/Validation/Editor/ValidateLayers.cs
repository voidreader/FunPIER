using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Validation
{
	/// <summary>
	/// Validate layer settings
	/// </summary>
	public class ValidateLayers : IValidator
	{
		protected const int EXPECTED_MAX_LAYER = 15;

		/// <summary>
		/// Apply this validation to the scene.
		/// </summary>
		public List<ValidationResult> Validate()
		{
			List<ValidationResult> result = new List<ValidationResult> ();

			// Check Max layer
			result.AddRange (CheckMaxLayer ());

			// Get all characters
			Character[] allCharacters = GameObject.FindObjectsOfType<Character> ();
			foreach (Character c in allCharacters)
			{
				result.AddRange (CheckGeometryLayers(c));
			}
			return result;
		}

		
		/// <summary>
		/// Validates if the max layer is under 15.
		/// </summary>
		protected List<ValidationResult> CheckMaxLayer()
		{
			
			List<ValidationResult> result = new List<ValidationResult> ();

			if (LayerMask.LayerToName (EXPECTED_MAX_LAYER + 1) != null && LayerMask.LayerToName (EXPECTED_MAX_LAYER + 1) != "")
			{
#if UNITY_IPHONE || UNITY_ANDROID
				result.Add (new ValidationResult("The maximum layer is higher than " + EXPECTED_MAX_LAYER + " we recommend only " + (EXPECTED_MAX_LAYER + 1) + " layers for performance reasons.", MessageType.Warning));
#else
				result.Add (new ValidationResult("The maximum layer is higher than " + EXPECTED_MAX_LAYER + " we strongly recommend only " + (EXPECTED_MAX_LAYER + 1) + " layers for performance reasons.", MessageType.Info));
#endif
			}
			return result;
		}

		/// <summary>
		/// Checks that character layers arent not outside the bounds of the typical layer settings.
		/// </summary>
		protected List<ValidationResult> CheckGeometryLayers(Character c)
		{
			List<ValidationResult> result = new List<ValidationResult> ();
			int match = 0;
			for (int i = 0; i < 32; i++)
			{
				if ((c.layerMask & (1 << i)) == (1 << i))
				{
					match++;
					if (i >= NoAllocationRaycast.MAX_LAYER) 
					{
						result.Add (new ValidationResult("The default raycasts wont check layers higher than " + (NoAllocationRaycast.MAX_LAYER - 1) + ".", MessageType.Warning));
					}
				}
			}

			return result;
		}
	}
}