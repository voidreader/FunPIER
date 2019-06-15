using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An extension to BasicAttacks which does a block.
	/// </summary>
	public class BlockAttack : BasicAttacks
	{
		public BlockDirection blockDirection;

		virtual public bool IsBlocking (DamageInfo damageInfo)
		{
			// Not enabled allow damage
			if (!enabled) return false;

			// Not blocking - allow damage
			if (!IsAttacking) return false;

			// Check direction
			if (blockDirection == BlockDirection.ALL) return true;
			float angle = Mathf.Rad2Deg * Mathf.Atan2(damageInfo.Direction.x * character.LastFacedDirection, damageInfo.Direction.y);
			if (blockDirection == BlockDirection.FRONT && (angle > 0 && angle < 180)) return true;
			if (blockDirection == BlockDirection.BACK && (angle < 0 && angle > -180)) return true;
			if (blockDirection == BlockDirection.FRONT_NOT_HIGH && (angle > CharacterBlockingHurtBox.MIN_FOR_HIGH && angle < 180)) return true;
			if (blockDirection == BlockDirection.FRONT_NOT_LOW && (angle > 0 && angle < CharacterBlockingHurtBox.MAX_FOR_LOW)) return true;

			return false;
		}
	}


}