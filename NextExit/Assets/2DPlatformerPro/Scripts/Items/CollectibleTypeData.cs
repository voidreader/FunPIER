/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

namespace PlatformerPro
{
	/// <summary>
	/// Data about a collectible mapping a string (name) to details like the maximum amount allowed.
	/// </summary>
	[System.Serializable]
	public class StackableItemData
	{
		/// <summary>
		/// String name for the type of collectible.
		/// </summary>
		public string type;

		/// <summary>
		/// The maximum amount that can be collected.
		/// </summary>
		public int max;

		/// <summary>
		/// The amount the character starts with.
		/// </summary>
		public int startingCount;

	}
}
