using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro 
{
	[System.Flags]
	public enum ItemBehaviour  {
		NONE 			=  0, // Item doesn't do anything to stats or abilities
		EQUIPPABLE		=  1, // Item upgrades stats or abilities when put in a slot
		UPGRADE			=  2, // Item provides an upgrade to stats or abilities just by being posessed
		CONSUMABLE		=  4, // Item does something when consumed
		POWER_UP		=  8  // Item does something when used or collected and the effect expires over time (or at death)
	}
}