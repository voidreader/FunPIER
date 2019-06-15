using UnityEngine;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Condition which requires a specific item to be held.
	/// </summary>
	public class MustHaveItemCondition : AdditionalCondition
	{
        /// <summary>
        /// If this is not empty require the character to have an item with the matching type before triggering.
        /// </summary>
        [Tooltip ("If this is not empty require the character to have an item with the matching type to meet this condition.")]
        [ItemType]
        public List<string> requiredItemTypes;

        /// <summary>
        /// If true character must have all items, if false any oe will do.
        /// </summary>
        public bool mustHaveAll;

        /// <summary>
        /// If true the item must be equipped.
        /// </summary>
        [Tooltip("If true the item must be equipped.")]
        public bool mustBeEquipped;

        /// <summary>
        /// The number required.
        /// </summary>
        [Tooltip ("The number of item required to activate the condition.")]
        [DontShowWhen ("mustBeEquipped")]
		public int requiredItemCount = 1;

		/// <summary>
		/// The optional number of the item to consume when the effect is activated.
		/// </summary>
		[Tooltip ("The optional number of the item to consume when the effect is activated. If you specify multiple " +
			      "items and mustHaveAll is false, then only first match is consumed.")]
        [DontShowWhen("mustBeEquipped")]
        public int numberConsumed = 0;

        /// <summary>
        /// Checks the condition. For example a check when entering a trigger.
        /// </summary>
        /// <returns><c>true</c>, if enter trigger was shoulded, <c>false</c> otherwise.</returns>
        /// <param name="character">Character.</param>
        /// <param name="other">Other.</param>
        override public bool CheckCondition(Character character, object other)
        {
            if (character.ItemManager == null)
            {
                Debug.LogWarning("Conditions requires an item but the character has no item manager.");
                return false;
            }
            if (mustBeEquipped && character.EquipmentManager == null)
            {
                Debug.LogWarning("Conditions requires equipped item but the character has no equipment manager.");
                return false;
            }
            if (mustBeEquipped && requiredItemCount != 1)
            {
                Debug.LogWarning("A mustBeEquipped condition ignores item count. Set requiredItemCount to 1 to remove this warning.");
            }
            for (int i = 0; i < requiredItemTypes.Count; i++)
            {
                if (CheckItem(character, requiredItemTypes[i])) 
                {
                    if (!mustHaveAll) return true;
                }
                else
                {
                    if (mustHaveAll) return false;
                }
            }
            return mustHaveAll;
        }

        /// <summary>
        /// Checks presence and amount of an item.
        /// </summary>
        /// <returns><c>true</c>, if item was present in desired amount, <c>false</c> otherwise.</returns>
        /// <param name="character">Character ref.</param>
        /// <param name="requiredItemType">Required item type to check.</param>
        virtual public bool CheckItem(Character character, string requiredItemType) { 
			if (requiredItemType != null && requiredItemType != "")
			{
                if (mustBeEquipped && character.EquipmentManager.IsEquipped(requiredItemType)) return true;
                if (!mustBeEquipped && character.ItemManager.ItemCount(requiredItemType) >= requiredItemCount) return true;
				return false;
			}
			Debug.LogWarning("MustHaveItemCondition has empty item configured.");
			return false;
		}

		/// <summary>
		/// Applies any activation effects.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="other">Other object supporting the condition.</param>
		override public void Activated(Character character, object other)
		{
            if (mustBeEquipped && numberConsumed != 0)
            {
                Debug.LogWarning("A mustBeEquipped condition ignores number to consume . Set numberConsumed to 0 to remove this warning.");
            }
            // ItemManager should have already been set in the call to CheckCondition
            if (numberConsumed > 0)
            {
                for (int i = 0; i < requiredItemTypes.Count; i++)
                {
                    if (CheckItem(character, requiredItemTypes[i]))
                    {
                        character.ItemManager.UseItem(requiredItemTypes[i], numberConsumed);
                        if (!mustHaveAll) return;
                    }
                }
            }
            return;
        }

	}

}
