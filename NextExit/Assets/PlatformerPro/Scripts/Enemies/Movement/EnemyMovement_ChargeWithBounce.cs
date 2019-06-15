using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Same as a charge with an optional bounce.
	/// </summary>
	public class EnemyMovement_ChargeWithBounce : EnemyMovement_Charge
	{
		/// <summary>
		/// The speed to set when we jump.
		/// </summary>
		[Tooltip ("The speed to set when we jump.")]
		public float bounceSpeed;

		/// <summary>
		/// Track if we have left the ground so we don't apply bounce force more than once.
		/// </summary>
		protected bool hasLeftGround = true;

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public bool DoMove()
		{
			base.DoMove ();
			enemy.ApplyFeetCollisions ();
			if (!enemy.Grounded) hasLeftGround = true;
			if (enemy.Grounded && hasLeftGround) {
				enemy.SetVelocityY(bounceSpeed);
				hasLeftGround = false;
			}
			enemy.ApplyGravity ();
			return true;
		}

	}
}