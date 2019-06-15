using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{

	/// <summary>
	/// A hazard causes damage to a player. The default hazard is a trigger and requires the player
	/// to have a collider.
	/// </summary>
	public class Hazard : MonoBehaviour {

		/// <summary>
		/// Amount of damage dealt by this object.
		/// </summary>
		[Tooltip ("The amount of damage dealt by this hazard.")]
		public int damageAmount;

		/// <summary>
		/// What type of damage does this hazard cause?
		/// </summary>
		[Tooltip ("The type of damage dealt by this hazard.")]
		public DamageType damageType;

		/// <summary>
		/// Should the damage be caused once, or should it be sent every frame.
		/// </summary>
		[Tooltip ("Should the damage be sent once, or should it be sent every frame.")]
		public bool oneShot;

		/// <summary>
		/// Does this hazard cause damage to characters?
		/// </summary>
		[Tooltip ("Does this hazard cause damage to characters?")]
		public bool damageCharacters = true;

		/// <summary>
		/// Does this hazard also cause damage to enemies?
		/// </summary>
		[Tooltip ("Does this hazard cause damage to enemies?")]
		public bool damageEnemies;

		/// <summary>
		/// Has the damage happened (for one shot types).
		/// </summary>
		protected bool hasFired;

		/// <summary>
		/// Cache the damage info to avoid allocation.
		/// </summary>
		protected DamageInfo damageInfo;

		/// <summary>
		/// Unity awake hook.
		/// </summary>
		void Awake()
		{
			Init ();
		}

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		virtual protected void Init()
		{
			damageInfo = new DamageInfo(damageAmount, damageType, transform.position);
#if UNITY_EDITOR
			if (!damageCharacters && !damageEnemies) Debug.LogWarning("Hazard does not damage enemies or characters.");
#endif
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
		/// Unity 2D trigger stay hook.
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerStay2D(Collider2D other)
		{
			DoHit(other);
		}

		/// <summary>
		/// Do the actual hit.
		/// </summary>
		/// <param name="other">Other.</param>
		virtual protected void DoHit(Collider2D other)
		{
			// If we are a one shot and have already fired we can't cause damage any more, if not continue
			if (!oneShot || !hasFired)
			{
				if (damageCharacters)
				{
					CharacterHurtBox hurtBox = other.gameObject.GetComponent<CharacterHurtBox>();
					if (hurtBox != null)
					{
						damageInfo.Direction = transform.position - other.transform.position;
						damageInfo.DamageType = damageType;
						damageInfo.Amount = damageAmount;
						hurtBox.Damage(damageInfo);
						hasFired = true;
					}
				}
				if (damageEnemies)
				{
					EnemyHurtBox enemyHurtBox = other.gameObject.GetComponent<EnemyHurtBox>();
					if (enemyHurtBox != null)
					{
						damageInfo.Direction = transform.position - other.transform.position;
						damageInfo.DamageType = damageType;
						damageInfo.Amount = damageAmount;
						enemyHurtBox.Damage(damageInfo);
						hasFired = true;
					}
				}
			}
		}

#if UNITY_EDITOR
		
		/// <summary>
		/// Unity gizmo hook.
		/// </summary>
		void OnDrawGizmos()
		{
			// We don't do anything but having this here allows us to assign a colored icon to the script.
		}
#endif
	}
}
