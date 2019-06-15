using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	
	/// <summary>
	/// Like a normal hazard but it has an enemy reference and tells the enemy when it hit the character.
	/// </summary>
	public class EnemyHazard : Hazard
	{
		protected Enemy enemy;

		/// <summary>
		/// Store the hit ready for processing at end of frame (note wont hit multiple enemies).
		/// </summary>
		protected CharacterHurtBox deferredHit;

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override protected void Init()
		{
			base.Init ();
			enemy = GetComponentInParent<Enemy>();
			if (enemy == null) Debug.LogWarning ("Unable to find the Enemy that EnemyHazard is attached to. Make sure your hazard is a direct child of the enemy.");
		}

		/// <summary>
		/// Do the actual hit.
		/// </summary>
		/// <param name="other">Other.</param>
		override protected void DoHit(Collider2D other)
		{
			CharacterHurtBox hurtBox = other.gameObject.GetComponent<CharacterHurtBox>();
			// Don't trigger hazards from a dead enemy
			if (hurtBox != null && deferredHit == null && (enemy.State != EnemyState.DEAD) && !oneShot)
			{
				deferredHit = hurtBox;
			}
		}

		/// <summary>
		/// Unity late update hook.
		/// </summary>
		void LateUpdate()
		{
			// We process hits at the end of the frame so if physics calls multiple hitboxes we process character damage last
			// In other words if the cahracter both kills and gets killed by the enemy... the character "wins"
			if (deferredHit != null)
			{
				if ((enemy.State != EnemyState.DEAD) && (!oneShot || !hasFired))
				{
					damageInfo.Direction = transform.position - deferredHit.Character.transform.position;
					damageInfo.DamageType = damageType;
					damageInfo.Amount = damageAmount;
					damageInfo.DamageCauser = enemy;
					// Tell enemy we hit character - we need to do invulnerable check before we call Damage else the character may become invulnerable 
					if (enemy != null && !deferredHit.IsInvulnerable) enemy.HitCharacter(deferredHit.Character, damageInfo);
					deferredHit.Damage(damageInfo);
					hasFired = true;
				}
			}
			deferredHit = null;
		}
	}
}

