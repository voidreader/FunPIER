using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An extension to BasicAttacks which does a dash attack.
	/// </summary>
	public class DashAttack : BasicAttacks
	{
		/// <summary>
		/// How fast the character dashes.
		/// </summary>
		public float dashSpeed = 5.0f;

		/// <summary>
		/// Stores direction character is dashing in once a dash starts.
		/// </summary>
		protected int dashDirection;

		/// <summary>
		/// Used by the inspector to determine if a given attack can have multiple attacks defined in it.
		/// </summary>
		override public bool CanHaveMultipleAttacks
		{
			get { return false; }
		}

		/// <summary>
		/// Used by the inspector to determine if a given attack allows user to change attack type.
		/// </summary>
		override public bool CanUserSetAttackType
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects
		/// gravity to be applied after its movement finishes.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get 
			{
				return false;
			}
		}

		/// <summary>
		/// Do whichever attack is available.
		/// </summary>
		/// <returns>true if this movement wants to main control of movement, false otherwise</returns>
		override public bool Attack()
		{
			bool result = base.Attack ();
			if (result) 
			{
				dashDirection = character.LastFacedDirection;
				return true;
			}
			return false;
		}


		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (dashDirection == 1)
			{
				character.SetVelocityX(dashSpeed);
				character.Translate(dashSpeed * TimeManager.FrameTime, 0, false);
			}
			else if (dashDirection == -1)
			{
				character.SetVelocityX(-dashSpeed);
				character.Translate(-dashSpeed * TimeManager.FrameTime, 0, false);
			}
		}

	}
}