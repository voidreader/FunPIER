using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// A projectile which can be cahrged up and gets faster and/or does more damage based on the charge.
	/// </summary>
	public class ChargableProjectile : Projectile
	{
		[Header ("Charge")]
		/// <summary>
		/// HOw does charge time convert to speed.  We multiply charge time in seconds by the attacks speed. 
		/// Use 0 for no effect.
		/// </summary>
		public float chargeToSpeedRatio = -1;

		/// <summary>
		/// The minimum speed the projectile can move at.
		/// </summary>
		public float minSpeed;

		/// <summary>
		/// The maximum speed the project can be moved at.
		/// </summary>
		public float maxSpeed;

		/// <summary>
		/// How does charge time convert to damage. We multiply charge time in seconds by the attacks damage amount. 
		/// Use 0 for no effect.
		/// </summary>
		public float chargeToDamageRatio = -1;

		/// <summary>
		/// Minimum amount of damage that can be done.
		/// </summary>
		public int minDamage;

		/// <summary>
		/// Maximum amount of damage that can be done.
		/// </summary>
		public int maxDamage;

		/// <summary>
		/// Calculates the actual damage amount.
		/// </summary>
		/// <returns>The damage amount.</returns>
		/// <param name="damageAmount">Damage amount from the attack.</param>
		override protected int CalculateDamageAmount(int damageAmount, float charge)
		{
			this.Charge = charge;
			if (chargeToDamageRatio == 0 ) return damageAmount;
			int actualDamage = (int)((float)damageAmount * charge * chargeToDamageRatio);
			if (actualDamage < minDamage) return minDamage;
			if (actualDamage > maxDamage) return maxDamage;
			return actualDamage;
		}

		/// <summary>
		/// Calculates the actual speed.
		/// </summary>
		/// <returns>The speed.</returns>
		override protected float CalculateSpeed(float charge)
		{
			if (chargeToSpeedRatio == 0 ) return speed;
			float actualSpeed = speed * charge * chargeToSpeedRatio;
			if (actualSpeed < minSpeed) return minSpeed;
			if (actualSpeed > maxSpeed) return maxSpeed;
			return actualSpeed;
		}

	}
}