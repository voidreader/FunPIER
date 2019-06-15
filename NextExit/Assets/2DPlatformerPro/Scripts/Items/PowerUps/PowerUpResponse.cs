/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Contains details about a power up response.
	/// </summary>
	[System.Serializable]
	public class PowerUpResponse {
		
		/// <summary>
		/// The type of power up this response is for.
		/// </summary>
		public string type;

		/// <summary>
		/// Number of seconds the power up stays active for (0 means forever).
		/// </summary>
		public int time;

		/// <summary>
		/// If true the power up will reset when character is damaged.
		/// </summary>
		public bool resetOnDamage;

		/// <summary>
		/// What to do when the power up is collected.
		/// </summary>
		public EventResponse[] actions;

		/// <summary>
		/// Which power up response to run when this power up expires. Only set if timer > 0.
		/// </summary>
		public string powerUpReset;

		/// <summary>
		/// rack the damage reset listener.
		/// </summary>
		[System.NonSerialized]
		public System.EventHandler<DamageInfoEventArgs> damageResetListener;
	}
}