using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Where the character needs to be to do the attack.
	/// </summary>
	public enum AttackLocation
	{
		GROUNDED,
		AIRBORNE,
		ANY_BUT_SPECIAL,
		ANY
	}
}
