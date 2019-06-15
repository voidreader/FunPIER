using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An item to quickly add health to the character.
	/// </summary>
	public class ItemHealth : Item
	{
		/// <summary>
		/// If true we add this item to the inventory for later use instead of healing immediately.
		/// </summary>
		public int amount = 1;

		/// <summary>
		/// Init this item.
		/// </summary>
		override public void Init()
		{
			base.Init();
			// Doesn't relaly matter but for consistency.
			if (itemClass != ItemClass.SINGLE)
			{
				itemClass = ItemClass.SINGLE;
			}
		}

		/// <summary>
		/// Do the collection.
		/// </summary>
		/// <param name="character">Character doing the collection.</param>
		override protected void DoCollect(Character character)
		{
			CharacterHealth health = character.GetComponentInChildren<CharacterHealth>();
			if (health == null)
			{
				Debug.LogWarning("Tried to add health but the character doesn't have a CharacterHealth");
			}
			else
			{
				health.Heal(amount);
			}
			DoEventForCollect (character);
		}
	}
}
