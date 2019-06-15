/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{ 
	/// <summary>
	/// Interface for anything that can get a character reference.
	/// </summary>
	public interface ICharacterReference
	{
		/// <summary>
		/// Get the character.
		/// </summary>
		/// <value>The character.</value>
		Character Character {
			get;
		}
	}
}
