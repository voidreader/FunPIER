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
	}
}
