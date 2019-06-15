using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro {
	
	/// <summary>
	/// Attach to an enemy to create a spring board which also damages the underlying enemy. An alternative to 
	/// jump on head movement.
	/// </summary>
	public class EnemySpringBoard : SpringboardPlatform {

		Enemy enemy;
		DamageInfo damageInfo;

		override protected void PostInit()
		{
			base.PostInit();
			enemy = GetComponentInParent<Enemy>();	
			damageInfo = new DamageInfo(1, DamageType.PHYSICAL, Vector2.down);
		}

		override protected void DoSpring()
		{
			base.DoSpring ();
			enemy.Damage (damageInfo);
		}

	}
}