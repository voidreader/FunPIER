/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A power up is a collectible item that gives the character some kind of ability.
	/// </summary>
	public class PowerUp : Item
	{

		/// <summary>
		/// Do the collection.
		/// </summary>
		/// <param name="character">Character doing the collection.</param>
		override protected void DoCollect(Character character)
		{
			PowerUpResponder responder = character.GetComponent<PowerUpResponder>();
			if (responder) 
			{
				responder.Collect(this);
			}
			base.DoCollect (character);
		}
	}

}