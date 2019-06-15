using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy hurt box.
	/// </summary>
	public class EnemyHurtBoxWithDamageFilter : EnemyHurtBox
	{
		public List<DamageType> ignoredDamageTypes;

		/// <summary>
		/// Pass damage from the given hazard to the Enemy script.
		/// </summary>
		/// <param name="info">Information about the damage such as the hazard causing the damage.</param>
		override public void Damage(DamageInfo info)
		{
			if (ShouldDoDamage(info)) enemy.Damage(info);
		}

		/// <summary>
		/// Check if the given damage info should damage this enemy.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual public bool ShouldDoDamage (DamageInfo info)
		{
			if (ignoredDamageTypes == null) return false;
			for (int i = 0; i < ignoredDamageTypes.Count; i++)
			{
				if (info.DamageType == ignoredDamageTypes[i]) return false;
			}
			return true;
		}
	}
}