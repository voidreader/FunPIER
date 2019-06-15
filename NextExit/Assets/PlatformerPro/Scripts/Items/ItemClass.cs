using UnityEngine;
using System.Collections;

namespace PlatformerPro 
{

	/// <summary>
	/// High level categorisatino of item type.
	/// </summary>
	public enum ItemClass 
	{
		NORMAL 			=  0, // A normal item that goes to your inventory
		NON_INVENTORY	=  1, // An item that is maintained in its own special 'stack'
		UNIQUE			=  2, // A unique item which a player may have only one of
		INSTANT			=  4  // An item that is intantly consumed
	}

}
