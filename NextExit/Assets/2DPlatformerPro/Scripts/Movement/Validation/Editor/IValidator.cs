using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Validation
{
	public interface IValidator
	{
		/// <summary>
		/// Apply this validation to the scene.
		/// </summary>
		List<ValidationResult> Validate();
	}
}
