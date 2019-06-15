/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{ 
	/// <summary>
	/// Interface for anything that can get a Mob reference.
	/// </summary>
	public interface IRaycastColliderWithIMob
	{
		IMob Mob {
			get;
			set;
		}
	}
}
