/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

namespace PlatformerPro
{
	/// <summary>
	/// Data about a collectible mapping a string (name) to detils like the maximum amount allowed.
	/// </summary>
	[System.Serializable]
	public class ItemAndCount
	{
		/// <summary>
		/// String name for the type of collectible.
		/// </summary>
		public string type;
		
		/// <summary>
		/// The number of this item held.
		/// </summary>
		public int count;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ItemAndCount"/> class.
		/// </summary>
		public ItemAndCount()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ItemAndCount"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="count">Count.</param>
		public ItemAndCount(string type, int count)
		{
			this.type = type;
			this.count = count;
		}
	}
}
