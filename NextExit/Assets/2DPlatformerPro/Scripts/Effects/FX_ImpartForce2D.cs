using UnityEngine;
using System.Collections;
using PlatformerPro;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Simple FX class which adds a force to a rigidbody.
	/// </summary>
	/// </summary>
	[RequireComponent (typeof(Rigidbody2D))]
	public class FX_ImpartForce2D : FX_Base
	{
		/// <summary>
		/// How much force to apply.
		/// </summary>
		public Vector2 force;
		
		/// <summary>
		/// Force mode to use.
		/// </summary>
		public ForceMode2D forceMode;

		/// <summary>
		/// The relative to mob direction.
		/// </summary>
		[Tooltip ("If true then look for a mob in the parent and use its facing direction as a multiplier for the force in X.")]
		public bool relativeToMobDirection = true;

		/// <summary>
		/// Do the effect.
		/// </summary>
		override protected void DoEffect()
		{
			if (relativeToMobDirection)
			{
				IMob mob = (IMob) gameObject.GetComponentInParent(typeof(IMob));
				if (mob != null) 
				{
					GetComponent<Rigidbody2D>().AddForce (new Vector2(force.x * mob.LastFacedDirection, force.y), forceMode);
					return;
				}
			}
			GetComponent<Rigidbody2D>().AddForce (force, forceMode);
		}
	}
}