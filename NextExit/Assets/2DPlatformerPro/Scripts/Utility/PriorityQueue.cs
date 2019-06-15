using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A list that provides a queue like interface specifically for animation priorities.
	/// </summary>
	public class PriorityQueue : List <int>
	{
		/// <summary>
		/// Look at the first item in the queue.
		/// </summary>
		/// <returns> The item at front of queue or 0 if queue is empty.</returns>
		public int Peek ()
		{
			if (Count > 0) return this [0];
			return 0;
		}

		/// <summary>
		/// Dequeues an item from the queue.
		/// </summary>
		/// <returns>the item at front of queue or null (default(T)) if queue is empty.</returns>
		public int Dequeue ()
		{
			if (Count > 0)
			{
				int result = this [0];
				RemoveAt(0);
				return result;
			}
			return 0;
		}

		/// <summary>
		/// Enqueue the specified item.
		/// </summary>
		/// <param name="item">Item to add to queue.</param>
		public void Enqueue(int item) 
		{
			this.Add (item);
		}

		/// <summary>
		/// Enqueue the specified item and update all items that have a lower value to match this item (except the first).
		/// </summary>
		/// <param name="item">Item to add to queue.</param>
		public void EnqueueAndPromote(int item) 
		{
			for (int i = Count - 1; i >= 0; i--)
			{
				if (item > this[i])
				{
					this[i] = item - 1;
				}
				else
				{
					break;
				}
			}
			this.Add (item);
		}

	}
}
