using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Like a normal enemy hazard but it can be enabled and disabled.
	/// </summary>
	public class EnemyHitBox : EnemyHazard
	{

		/// <summary>
		/// Reference to collider.
		/// </summary>
		public Collider2D myCollider;

		/// <summary>
		/// The hit timer.
		/// </summary>
		protected float hitTimer;

		/// <summary>
		/// Do the actual hit.
		/// </summary>
		/// <param name="other">Other.</param>
		override protected void DoHit(Collider2D other)
		{
			CharacterHurtBox hurtBox = other.gameObject.GetComponent<CharacterHurtBox>();
			// Don't trigger hazards from a dead enemy
			if (hurtBox != null && (enemy.State != EnemyState.DEAD) && (!hasFired))
			{
				damageInfo.Direction = transform.position - other.transform.position;
				damageInfo.DamageType = damageType;
				damageInfo.Amount = damageAmount;
				damageInfo.DamageCauser = enemy;
				// Tell enemy we hit character - we need to do invulnerable check before we call Damage else the character may become invulnerable 
				if (enemy != null && !hurtBox.IsInvulnerable) enemy.HitCharacter(hurtBox.Character, damageInfo);
				hurtBox.Damage(damageInfo);
				hasFired = true;
			}
		}

		/// <summary>
		/// Start the hit.
		/// </summary>
		virtual public void Enable(float enableTime, float disableTime)
		{
			// Disable then restart
			// TODO It may be faster to do this with physics by ignoring layers
			myCollider.enabled = false;
			StopAllCoroutines();
			StartCoroutine(DoEnable (enableTime, disableTime));
		}
		
		/// <summary>
		/// Forces the attack to finish.
		/// </summary>
		virtual public void ForceStop()
		{
			StopAllCoroutines();
			myCollider.enabled = false;
			hitTimer = 0.0f;
		}
		
		/// <summary>
		/// Turn on the hit box.
		/// </summary>.
		/// <returns>The enable.</returns>
		/// <param name="enableTime">Enable time.</param>
		/// <param name="disableTime">Disable time.</param>
		virtual protected IEnumerator DoEnable(float enableTime, float disableTime)
		{
			hasFired = false;
			hitTimer = 0.0f;
			// Handle the timing, we don't use WaitForSeconds as we want to align with the internal frame time
			while (hitTimer < enableTime)
			{
				hitTimer += TimeManager.FrameTime;
				yield return true;
			}
			myCollider.enabled = true;
			while (hitTimer < disableTime)
			{
				hitTimer += TimeManager.FrameTime;
				yield return true;
			}
			myCollider.enabled = false;
		}

	}
}

