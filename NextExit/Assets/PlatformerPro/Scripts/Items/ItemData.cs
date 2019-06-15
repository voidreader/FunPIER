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
		virtual public void AddItem(string key, int amount)
		{
			stackableItemCountsIds.Add (key);
			stackableItemCountsCounts.Add (amount);
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
		    return result;
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


	}
}