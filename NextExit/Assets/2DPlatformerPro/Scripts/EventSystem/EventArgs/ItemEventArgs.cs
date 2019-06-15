using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Item event arguments.
	/// </summary>
	public class ItemEventArgs : CharacterEventArgs
	{
		/// <summary>
		/// Gets the item class.
		/// </summary>
		public ItemClass ItemClass
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the item type.
		/// </summary>
		public string Type
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the number of items collected.
		/// </summary>
		virtual public int Amount 
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the  int value.
		/// </summary>
		/// <value>The previous scene.</value>
		virtual public int IntValue
		{
			get {return Amount;}
			protected set {}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ItemEventArgs"/> class assuming
		/// the amount = 1.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="amount">The number of items collected.</param>
		/// <param name="character">Character.</param>
		/// <param name="totalAmount">Total Number of items of this type the player has.</param>
		public ItemEventArgs(ItemClass itemClass, string type, Character character) : base (character)
		{
			ItemClass = itemClass;
			Type = type;
			Character = character;
			Amount = 1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ItemEventArgs"/> class.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="amount">The number of items collected.</param>
		/// <param name="character">Character.</param>
		/// <param name="totalAmount">Total Number of items of this type the player has.</param>
		public ItemEventArgs(ItemClass itemClass, string type, int amount, Character character)  : base (character)
		{
			ItemClass = itemClass;
			Type = type;
			Character = character;
			Amount = amount;
		}

	}
	
}
