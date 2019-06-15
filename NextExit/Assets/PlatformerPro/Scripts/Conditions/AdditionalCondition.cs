using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Additional trigger conditions. Add a subclass of this this to the same GameObject as a Trigger to add new contions.
	/// </summary>
	public abstract class AdditionalCondition : MonoBehaviour
	{

		/// <summary>
		/// If true condition will be applied on inverse (for example trigger exit as well as enter).
		/// </summary>
		[Tooltip ("If true condition will be applied on inverse (for example trigger exit as well as enter).")]
		public bool applyOnInverse;

		/// <summary>
		/// Checks the condition. For example a check when entering a trigger.
		/// </summary>
		/// <returns><c>true</c>, if enter trigger was shoulded, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		virtual public bool CheckCondition(Character character, object other)
		{
			return true;
		}

		/// <summary>
		/// Checks the inverse condition. For example a check when leaving a trigger.
		/// </summary>
		/// <returns><c>true</c>, if inverse condition is met, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="other">Other object supporting the condition.</param>
		virtual public bool CheckInverseCondition(Character character, object other)
		{
			if (!applyOnInverse) return true;
			return CheckCondition(character, other);
		}

		/// <summary>
		/// Applies any activation effects.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="other">Other object supporting the condition.</param>
		virtual public void Activated(Character character, object other)
		{
			
		}
	}
}