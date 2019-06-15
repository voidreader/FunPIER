using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// The damage causing collider of a character or enemy, collides with hurt boxes to cause damage.
	/// </summary>
	public class CharacterHitBox : MonoBehaviour, ICharacterReference
	{

		/// <summary>
		/// The character this hit box is for.
		/// </summary>
		protected IMob character;

		/// <summary>
		/// The actual collider.
		/// </summary>
		protected Collider2D myCollider;

		/// <summary>
		/// Tracks the time for enalbing and disabling the hit box.
		/// </summary>
		protected float hitTimer;

		/// <summary>
		/// Tracks if this attack instance has hit an enemy.
		/// </summary>
		protected bool hasHitCharacter;

		/// <summary>
		/// Cached damage info.
		/// </summary>
		protected DamageInfo damageInfo;

		/// <summary>
		/// Gets the character.
		/// </summary>
		virtual public Character Character {
			get
			{
				return character as Character;
			}
			set
			{
				character = value;
			}
		}

		/// <summary>
		/// Returns true if the hit box has hit something since it was last enabled.
		/// </summary>
		virtual public bool HasHit
		{
			get
			{
				return hasHitCharacter;
			}
		}

		/// <summary>
		/// Init this instance, this should be called by the attack system during Start();
		/// </summary>
		virtual public void Init(DamageInfo info)
		{
			character = (IMob) gameObject.GetComponentInParent (typeof(IMob));
			myCollider = GetComponent<Collider2D>();
			if (myCollider == null)
			{
				Debug.LogError("A CharacterHitBox must be on the same GameObject as a Collider2D");
			}
			this.damageInfo = new DamageInfo (info.Amount, info.DamageType, Vector2.zero, character);
		}

		/// <summary>
		/// Updates the damage info with new values.
		/// </summary>
		/// <param name="amount">Amount.</param>
		/// <param name="damageType">Damage type.</param>
		virtual public void UpdateDamageInfo(int amount, DamageType damageType)
		{
			this.damageInfo.Amount = amount;
			this.damageInfo.DamageType = damageType;
		}

		/// <summary>
		/// Start the hit with no timer.
		/// </summary>
		virtual public void Enable() 
		{
			hasHitCharacter = false;
			myCollider.enabled = true;
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
			hasHitCharacter = false;
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

		/// <summary>
		/// Unity 2D trigger hook.
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerEnter2D(Collider2D other)
		{
			DoHit(other);
		}

		/// <summary>
		/// Do the actual hit.
		/// </summary>
		/// <param name="other">Other.</param>
		/// <returns>true if a hit was done.</returns>
		virtual protected bool DoHit(Collider2D other)
		{
			IHurtable hurtBox = (IHurtable) other.gameObject.GetComponent(typeof(IHurtable));
			if (character == null) Debug.LogWarning("Tried to DoHit() but no character has been set");
			// Got a hurt box and its not ourselves
			if (hurtBox != null && !hasHitCharacter && hurtBox.Mob != character )
			{
				damageInfo.Direction = transform.position - other.transform.position;
				damageInfo.DamageCauser = character;
				hurtBox.Damage(damageInfo);
				hasHitCharacter = true;
				return true;
			}
			return false;
		}
	}
}