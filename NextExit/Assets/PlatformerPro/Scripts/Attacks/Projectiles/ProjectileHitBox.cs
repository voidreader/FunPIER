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
					damageInfo.Direction = transform.position - other.transform.position;
					hurtBox.Damage(damageInfo);
					// TODO: Ideally this shouldn't be here but on something related to grappling hook projectile
					if (projectile != null && projectile is GrapplingHookProjectile && other.gameObject.GetComponent<DestructiblePlatform>() != null)
					{
						((GrapplingHookProjectile)projectile).SetCollider (other);
						// For grapple hitting a platform we need to stop damage after first hit no matter what
						damageInfo = new DamageInfo(0, damageInfo.DamageType, Vector2.zero, damageInfo.DamageCauser);
						projectile.DestroyProjectile (false);
					} else if (projectile != null && destroyOnEnemyHit)
					{
						projectile.DestroyProjectile (true);
					}

					if (!allowMultiHit) hasHitCharacter = true;
					if (character is Character) ((Character)character).HitEnemy(hurtBox.Mob, damageInfo);
					return true;
				}
				else
				{
					if (hurtBox == null && projectile != null && destroyOnSceneryHit) projectile.DestroyProjectile(false);
					// TODO: Ideally this shouldn't be here but on something related to grappling hook projectile
					if (projectile is GrapplingHookProjectile) 
					{
						((GrapplingHookProjectile)projectile).SetCollider (other);
						if (((GrapplingHookProjectile)projectile).shouldParent) 
						{
							projectile.gameObject.transform.parent = other.gameObject.transform;
#if UNITY_EDITOR
							if (other.gameObject.transform.lossyScale != Vector3.one) 
								Debug.LogWarning("Grapple with shouldParent==true requires that all the colliders it collides with have a GameObject scale of (1,1,1) due to Unity bug with parenting to non-uniform colliders. Instead of scaling your game objects set the size on the collider and leave the scale at (1,1,1).");
#endif
						}
					}
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