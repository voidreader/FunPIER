using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Trigger that should be attached to a TriggerPlatform.
	/// </summary>
	public class TriggerPlatformTrigger : Trigger
	{
		/// <summary>
		/// Called when character enters a trigger.
		/// </summary>
		/// <param name="character">Character.</param>
		public void CharacterEnteredTrigger(Character character)
		{
			EnterTrigger (character);
		}

		/// <summary>
		/// Called when character leaves a trigger.
		/// </summary>
		/// <param name="character">Character.</param>
		public void CharacterLeftTrigger(Character character)
		{
			LeaveTrigger (character);
		}
	}
}