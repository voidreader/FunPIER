/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A class of item that stacks (for example coins).
	/// </summary>
	public class StackableItem : Item
	{

		/// <summary>
		/// How many are in this 'pile'
		/// </summary>
		public int amount = 1;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Init this item.
		/// </summary>
		override public void Init()
		{
			base.Init();
			if (itemClass != ItemClass.STACKABLE)
			{
				Debug.LogWarning("StackableItems should have the item class STACKABLE, updating");
				itemClass = ItemClass.STACKABLE;
			}
		}

		/// <summary>
		/// Do the collection.
		/// </summary>
		/// <param name="character">Character doing the collection.</param>
		override protected void DoCollect(Character character)
		{

			ItemManager itemManager = character.GetComponentInChildren<ItemManager> ();
			if (itemManager != null) itemManager.CollectItem (this);
			if (CollectHasListener()) OnCollectItem(itemClass, type, amount, character);
			else
			{
				// No responders lets do something so the user can tell the collected the item.
				gameObject.SetActive(false);
			}
		}
	}
}
