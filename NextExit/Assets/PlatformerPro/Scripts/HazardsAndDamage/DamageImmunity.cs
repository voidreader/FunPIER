using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Used to define damage immunity in character health.
	/// </summary>
	[System.Serializable]
	public class DamageImmunity
	{
		[Tooltip ("Type of damage.")]
		public DamageType damageType;
		[Tooltip ("Immunity from 0 (none to 1 (100%).")]
		[Range (0,1)]
		public float immunity;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.DamageImmunity"/> class.
		/// </summary>
		public DamageImmunity() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.DamageImmunity"/> class by cloning an existing instance.
		/// </summary>
		/// <param name="original">Object to clone..</param>
		public DamageImmunity(DamageImmunity original) 
		{
			damageType = original.damageType;
			immunity = original.immunity;
		}

	}
}
