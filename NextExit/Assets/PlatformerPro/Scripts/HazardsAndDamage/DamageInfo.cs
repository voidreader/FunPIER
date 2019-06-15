using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Wraps information about a (potentially) damage causing hit to the player.
	/// </summary>
	[System.Serializable]
	public class DamageInfo
	{

		/// <summary>
		/// The raw damage amount.
		/// </summary>
		virtual public int Amount
		{
			get; set;
		}

		/// <summary>
		/// The direction to the object causing the damage. You can use this to provide specific damage types.
		/// </summary>
		virtual public Vector2 Direction
		{
			get; set;
		}

	

		/// <summary>
		/// The type of the damage.
		/// </summary>
		virtual public DamageType DamageType
		{
			get; set;
		}

		/// <summary>
		/// Optional info about who caused the damage. Null if unknown or damage causer was not a character.
		/// </summary>
		public IMob DamageCauser
		{
			get;
			set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.DamageInfo"/> class.
		/// </summary>
		/// <param name="amount">Amount.</param>
		/// <param name="damageType">Damage type.</param>
		/// <param name="direction">Direction.</param>
		public DamageInfo(int amount, DamageType damageType, Vector2 direction)
		{
			Amount = amount;
			DamageType = damageType;
			Direction = direction;
		}
	
		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.DamageInfo"/> class.
		/// </summary>
		/// <param name="amount">Amount.</param>
		/// <param name="damageType">Damage type.</param>
		/// <param name="direction">Direction.</param>
		public DamageInfo(int amount, DamageType damageType, Vector2 direction, IMob damageCauser)
		{
			Amount = amount;
			DamageType = damageType;
			Direction = direction;
			DamageCauser = damageCauser;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.DamageInfo"/> class.
		/// </summary>
		public DamageInfo()
		{

		}
	}
}
