using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy hurt box.
	/// </summary>
	public class EnemyHurtBox : MonoBehaviour, IHurtable
	{
		//// <summary>
		/// Cached reference to the enemy script.
		/// </summary>
		protected Enemy enemy;
		
		#region properties
		
		/// <summary>
		/// Gets the enemy reference.
		/// </summary>
		/// <value>The enemy.</value>
		virtual public Enemy Enemy
		{
			get
			{
				return enemy;
			}
		}
		
		/// <summary>
		/// Is the enemy currently invulnerable?
		/// </summary>
		/// <value>The character.</value>
		virtual public bool IsInvulnerable
		{
			get
			{
				return enemy.IsInvulnerable;
			}
		}
		
		#endregion
		
		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start()
		{
			// Try looking for enemy
			if (enemy == null) enemy = gameObject.GetComponentInParent<Enemy>();
			if (enemy == null) Debug.LogError ("Unable to find Enemy for EnemyHurtBox");
		}
		
		/// <summary>
		/// Pass damage from the given hazard to the Enemy script.
		/// </summary>
		/// <param name="info">Information about the damage such as the hazard causing the damage.</param>
		virtual public void Damage(DamageInfo info)
		{
			enemy.Damage(info);
		}

		/// <summary>
		/// Get the mobile (charater) that this hurt box belongs too. Can return null.
		/// </summary>
		virtual public IMob Mob
		{
			get
			{
				return enemy;
			}
		}

	}
}