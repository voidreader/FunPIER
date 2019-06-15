using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Special "platform" that handles stomping on characters heads.
	/// </summary>
	public class StompOnHead : Platform
	{
		/// <summary>
		/// How much to bobble the character
		/// </summary>
		public float characterBobble = 6.0f;

		/// <summary>
		/// Cached damage info.
		/// </summary>
		protected DamageInfo damageInfo;

		/// <summary>
		/// Cached enemy reference
		/// </summary>
		protected Enemy enemy;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init()
		{
			base.Init();
			damageInfo = new DamageInfo(1, DamageType.HEAD_STOMP, Vector3.zero);
			enemy = transform.parent.gameObject.GetComponent<Enemy>();
			if (enemy == null) enemy = GetComponent<Enemy>();
			if (enemy == null) Debug.LogWarning ("Unable to find the Enemy that this StompOnHead is attached to. Make sure your StompOnHead is a direct child of the enemy.");
		}

		/// <summary>
		/// Called when one of the characters colliders collides with this enemys head.
		/// </summary>
		/// <param name="PlatformCollisionArgs">Arguments describing a platform collision.</param>
		/// <returns>Always returns false.</returns>
		override public bool Collide(PlatformCollisionArgs args)
		{
			if (args.RaycastCollider.RaycastType == RaycastType.FOOT)
			{
				// Apply upwards movement, this will work for most air movements, but in some cases you may need to provide a movement override
				args.Character.SetVelocityY(characterBobble);
				damageInfo.DamageCauser = args.Character;
				Debug.Log (damageInfo.DamageCauser);
				enemy.Damage(damageInfo);
				if (enemy.health <= 0) 
				{
					// If the character is dead, turn everything off
					GetComponent<Collider2D>().enabled = false;
					enabled = false;
				}
			}
			return false;
		}

	}
}