using UnityEngine;
using System.Collections;

namespace PlatformerPro 
{

	/// <summary>
	/// High level categorisatino of item type.
	/// </summary>
	public enum ItemClass 
	{
		NONE 			=  0,
		STACKABLE		=  1,
		SINGLE			=  2,
		POWER_UP		=  4,
		KEY				=  8
	}

}
