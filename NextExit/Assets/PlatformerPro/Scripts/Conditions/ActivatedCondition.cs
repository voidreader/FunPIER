using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition that is set when the given item is activated.
	/// </summary>
	public class ActivatedCondition : AdditionalCondition 
	{
		/// <summary>
		/// The group that contains the item. Can be empty in which case character will be searched for an activation group.
		/// </summary>
		[Tooltip ("The group that contains the item. Can be empty in which case character will be searched for an activation group.")]
		public ActivationGroup group;

		/// <summary>
		/// The item that must be active.
		/// </summary>
		[Tooltip ("The item that must be active.")]
		public string itemId;

		/// <summary>
		/// The deactivate when activated.
		/// </summary>
		[Tooltip ("If true the activated item will be deactivated when this triggers")]
		public bool deactivateWhenActivated;

		/// <summary>
		/// If only one character will ever use this, save a reference to the ActivationGroup
		/// </summary>
		[Tooltip ("If only one character will ever use this condition, save a reference to the item manager. Not rlevant if gorup is assigned.")]
		public bool cacheActivationGroup = true;

		/// <summary>
		/// Checks the condition. For example a check when entering a trigger.
		/// </summary>
		/// <returns><c>true</c>, if enter trigger was shoulded, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (group == null || !cacheActivationGroup)
			{
				foreach (ActivationGroup ag in character.GetComponentsInChildren<ActivationGroup>())
				{
					if (ag.items.Contains(itemId))
					{
						group = ag;
						break;
					}
				}
			}
			if (group == null) 
			{
				Debug.LogWarning("Conditions requires an ActivationGroup but a matching group could not be found.");
				return false;
			}
			if (group.IsActive (itemId)) return true;
			return false;
		}

		/// <summary>
		/// Applies any activation effects.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="other">Other object supporting the condition.</param>
		override public void Activated(Character character, object other)
		{
			// Group should have already been set in the call to CheckCondition
			if (deactivateWhenActivated) group.Deactivate (itemId);
		}

	}
}
