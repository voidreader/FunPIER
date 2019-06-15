using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A collider that can register damage agaisnt the character.
	/// </summary>
	public class CharacterHurtBox : MonoBehaviour, IHurtable, ICharacterReference
	{

		//// <summary>
		/// Cached reference to the characters health script.
		/// </summary>
		protected CharacterHealth health;

		#region properties
		
		/// <summary>
		/// Gets the character reference.
		/// </summary>
		/// <value>The character.</value>
		virtual public Character Character
		{
			get
			{
				return health.Character;
			}
		}

		/// <summary>
		/// Is the character currently invulnerable?
		/// </summary>
		/// <value>The character.</value>
		virtual public bool IsInvulnerable
		{
			get
			{
				return health.IsInvulnerable;
			}
		}

		#endregion

		/// <summary>
		/// Unity OnEnable hook.
		/// </summary>
		void Start()
		{
			// Try looking for character health in self then parents
			if (health == null) health = gameObject.GetComponentInParent<CharacterHealth>();
			if (health == null) Debug.LogError ("Unable to find CharacterHealth for CharacterHurtBox");
		}

		/// <summary>
		/// Pass damage of the given amount to the CharacterHealth script.
		/// </summary>
		/// <param name="amount">Amount.</param>
		virtual public void Damage(int amount)
		{
			if (enabled) health.Damage(amount);
		}

		/// <summary>
		/// Pass damage from the given hazard to the CharacterHealth script.
		/// </summary>
		/// <param name="info">Information about the damage such as the hazard causing the damage.</param>
		virtual public void Damage(DamageInfo info)
		{
			if (enabled)
			{
				if (info.DamageType == DamageType.AUTO_KILL)
				{
					health.Kill();
				}
				else
				{
					health.Damage(info);
				}
			}
		}

		/// <summary>
		/// Get the mobile (charater) that this hurt box belongs too. Can return null.
		/// </summary>
		virtual public IMob Mob
		{
			get
			{
				return Character;
			}
		}

	}

}