using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	public class ProjectileHitBox : CharacterHitBox
	{
		/// <summary>
		/// If true we can hit more than one thing
		/// </summary>
		public bool allowMultiHit;

		protected bool destroyOnEnemyHit;
		protected bool destroyOnSceneryHit;
		protected Projectile projectile;

		/// <summary>
		/// Init this instance, used for projectiles or hit boxes which are not children of a character.
		/// </summary>
		virtual public void Init(DamageInfo info, IMob character, Projectile projectile,  bool destroyOnEnemyHit, bool destroyOnSceneryHit)
		{
			this.character = character;
			if (character == null) 
			{
				Debug.LogError ("A ProjectileHitBox (CharacterHitBox) must have a character");
			}
			myCollider = GetComponent<Collider2D>();
			if (myCollider == null)
			{
				Debug.LogError("A ProjectileHitBox (CharacterHitBox) must be on the same GameObject as a Collider2D");
			}
			myCollider.enabled = true;
			this.damageInfo = info;
			this.projectile = projectile;
			this.destroyOnSceneryHit = destroyOnSceneryHit;
			this.destroyOnEnemyHit = destroyOnEnemyHit;
		}

		/// <summary>
		/// Do the actual hit.
		/// </summary>
		/// <param name="other">Other.</param>
		override protected bool DoHit(Collider2D other)
		{
			// Simple projectiles can hit only one thing
			if (!hasHitCharacter && enabled)
			{
				IHurtable hurtBox = (IHurtable) other.gameObject.GetComponent(typeof(IHurtable));
				// Got a hurt box and its not ourselves
				if (hurtBox != null && !hasHitCharacter && hurtBox.Mob  != character)
				{
					if (projectile != null && destroyOnEnemyHit) projectile.DestroyProjectile(true);
					damageInfo.Direction = transform.position - other.transform.position;
					hurtBox.Damage(damageInfo);
					if (!allowMultiHit) hasHitCharacter = true;
					return true;
				}
				else
				{
					if (hurtBox == null && projectile != null && destroyOnSceneryHit) projectile.DestroyProjectile(false);
				}
			}
			return false;
		}

		/// <summary>
		/// Unity 2D trigger hook.
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerStay2D(Collider2D other)
		{
			DoHit(other);
		}
	}

}