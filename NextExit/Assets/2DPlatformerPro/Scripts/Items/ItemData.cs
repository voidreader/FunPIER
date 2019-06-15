using UnityEngine;
using System.Collections.Generic;

namespace PlatformerPro
{

	[System.Serializable]
	public class ItemData
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ItemData"/> class.
		/// </summary>
		public ItemData()
		{
			items = new List<string> ();
			stackableItemCountsIds = new List<string>();
			stackableItemCountsCounts = new List<int>();
		}

		/// <summary>
		/// Gets or sets the item data with the specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		virtual public int this [string key]
		{
			get
			{
				if (key == null)
				{
					throw new System.ArgumentNullException ("key");
				}
				int index = stackableItemCountsIds.IndexOf(key);
				if (index >= 0) return stackableItemCountsCounts[index];
				if (items.Contains(key)) return 1;
				throw new KeyNotFoundException();
			}
			set
			{
				if (key == null)
				{
					throw new System.ArgumentNullException ("key");
				}
				int index = stackableItemCountsIds.IndexOf(key);
				if (index >= 0) stackableItemCountsCounts[index] = value;
				else throw new KeyNotFoundException();
			}
		}

		/// <summary>
		/// Add the specified key and amount.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="amount">Amount.</param>
		virtual public void AddStackable(string key, int amount)
		{
			stackableItemCountsIds.Add (key);
			stackableItemCountsCounts.Add (amount);
		}

		/// <summary>
		/// Add the specified item to non-stackable items.
		/// </summary>
		/// <param name="key">Key.</param>
		virtual public void AddNonStackable(string key)
		{
			items.Add (key);
		}

		/// <summary>
		/// Returns true if the key is found.
		/// </summary>
		/// <returns><c>true</c>, if key was found, <c>false</c> otherwise.</returns>
		/// <param name="key">Key.</param>
		virtual public bool ContainsKey(string key)
		{
			bool result = false;
			if (stackableItemCountsIds.Contains (key)) result = true;
			if (items.Contains(key)) result = true;
		    return result;
		}

		/// <summary>
		/// Returns STACKABLE, SINGLE or NONE based on the type of the given item.
		/// </summary>
		/// <returns>The item class for key.</returns>
		/// <param name="key">Key.</param>
		virtual public ItemClass TypeForKey(string key)
		{
			if (stackableItemCountsIds.Contains (key)) return ItemClass.STACKABLE;
			if (items.Contains (key)) return ItemClass.SINGLE;
			return ItemClass.NONE;
		}

		/// <summary>
		/// Consume the item with specified key in the given amount.
		/// </summary>
		/// <returns>The actual number consumed.</returns>
		/// <param name="key">Key.</param>
		/// <param name="amount">Amount.</param>
		virtual public int Consume(string key, int amount)
		{
			int index = stackableItemCountsIds.IndexOf(key);
			if (index >= 0)
			{
				if (amount > stackableItemCountsCounts[index])
				{
					int consumedAmount = stackableItemCountsCounts[index];
					stackableItemCountsCounts[index] = 0;
					return consumedAmount;
				}
				else
				{
					stackableItemCountsCounts[index] -= amount;
					return amount;
				}
			}
			// Non-stackable
			if (items.Contains(key))
			{
				items.Remove (key);
				return 1;
			}
			return 0;
		}

		/// <summary>
		/// The stackable item counts identifiers.
		/// </summary>
		public List<string> stackableItemCountsIds;


		/// <summary>
		/// The stackable item counts counts.
		/// </summary>
		public List<int> stackableItemCountsCounts;

		/// <summary>
		/// A list of non-stakable items held by the player.
		/// </summary>
		public List<string> items;


	}
}