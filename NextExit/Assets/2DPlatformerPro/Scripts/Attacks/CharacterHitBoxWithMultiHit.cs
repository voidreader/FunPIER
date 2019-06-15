using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// The damage causing collider of a character or enemy, collides with hurt boxes to cause damage. This
	/// version can hit multiple enemies from one attack.
	/// </summary>
	public class CharacterHitBoxWithMultiHit : CharacterHitBox
	{
		/// <summary>
		/// A list of mobs this hit box has hit since it was last enabled.
		/// </summary>
		public List<IMob> hitMobs;

		/// <summary>
		/// Init this instance, this should be called by the attack system during Start();
		/// </summary>
		override public void Init(DamageInfo info)
		{
			base.Init (info);
			hitMobs = new List<IMob>();
		}

		/// <summary>
		/// Returns true if the hit box has hit the given mob since it was last enabled.
		/// </summary>
		virtual public bool HasHitMob(IMob mob)
		{
			return hitMobs.Contains (mob);
		}

		/// <summary>
		/// Start the hit with no timer.
		/// </summary>
		override public void Enable() 
		{
			hasHitCharacter = false;
			hitMobs.Clear ();
			myCollider.enabled = true;
		}
		
		/// <summary>
		/// Start the hit.
		/// </summary>
		override public void Enable(float enableTime, float disableTime)
		{
			// Disable then restart
			// TODO It may be faster to do this with physics by ignoring layers
			myCollider.enabled = false;
			hitMobs.Clear ();
			StopAllCoroutines();
			StartCoroutine(DoEnable (enableTime, disableTime));
		}

		/// <summary>
		/// Do the actual hit.
		/// </summary>
		/// <param name="other">Other.</param>
		/// <returns>true if a hit was done.</returns>
		override protected bool DoHit(Collider2D other)
		{
			IHurtable hurtBox = (IHurtable) other.gameObject.GetComponent(typeof(IHurtable));
			if (character == null) Debug.LogWarning("Tried to DoHit() but no character has been set");
			// Got a hurt box and its not ourselves
			if (hurtBox != null && !HasHitMob(hurtBox.Mob) && hurtBox.Mob != character )
			{
				damageInfo.Direction = transform.position - other.transform.position;
				damageInfo.DamageCauser = character;
				hurtBox.Damage(damageInfo);
				hasHitCharacter = true;
				hitMobs.Add (hurtBox.Mob);
				return true;
			}
			return false;
		}
	}
}