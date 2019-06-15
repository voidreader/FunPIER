using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Interface implemented by a hurt box (or anything else that can be hurt).
	/// </summary>
	public interface IHurtable
	{
		/// <summary>
		/// Deal damage to the hurtable.
		/// </summary>
		/// <param name="info">Info.</param>
		void Damage(DamageInfo info);

		/// <summary>
		/// Get the mobile (charater or enemy) that this hurt box belongs too. Can return null.
		/// </summary>
		IMob Mob
		{
			get;
		}
	}

}

