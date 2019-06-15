using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Stores data about a basic attack such as its animation, key, and hit box.
	/// </summary>
	[System.Serializable]
	public class BasicAttackData
	{
		/// <summary>
		/// Human readable name (optional).
		/// </summary>
		public string name;

		/// <summary>
		/// The type of the attack.
		/// </summary>
		public AttackType attackType;

		/// <summary>
		/// Where the character needs to be to do the attack.
		/// </summary>
		public AttackLocation attackLocation;

		/// <summary>
		/// The animation state to set when character is attacking.
		/// </summary>
		public AnimationState animation;

		/// <summary>
		/// How long the attack lasts for.
		/// </summary>
		public float attackTime;

		/// <summary>
		/// After the attack finishes, the cool down blocks this attack from being done again until it expires
		/// </summary>
		public float coolDown;

		/// <summary>
		/// When the projectile fires (must be smaller than attackTime).
		/// </summary>
		public float projectileDelay;

		/// <summary>
		/// When the hitbox becomes active as a normalized time (0 to 1). Only used for melee attacks.
		/// </summary>
		public float attackHitBoxStart;

		/// <summary>
		/// When the hitbox becomes deactive as a normalized time (0 to 1). Only used for melee attacks.
		/// </summary>
		public float attackHitBoxEnd;

		/// <summary>
		/// Which action button triggers this attack.
		/// </summary>
		public int actionButtonIndex;

		/// <summary>
		/// This attacks hit box (only used for melee attacks).
		/// </summary>
		public CharacterHitBox hitBox;

		/// <summary>
		/// Type of damage done by this attack.
		/// </summary>
		public DamageType damageType;

		/// <summary>
		/// Amount of damage done by this attack.
		/// </summary>
		public int damageAmount;

		/// <summary>
		/// The projectile prefab (only used for projectile attacks). Typically will have a Procectile behaviour attached.
		/// </summary>
		public GameObject projectilePrefab;


		/// <summary>
		/// String representing the stackable item that is used for ammo.
		/// </summary>
		public string ammoType;

		/// <summary>
		/// If true don't allow a jump to be triggered whilst this attack is active.
		/// </summary>
		public bool blockJump;

		/// <summary>
		/// If true don't allow a wall cling to be triggered whilst this attack is active.
		/// </summary>
		public bool blockWall;

		/// <summary>
		/// If true don't allow a climb to be triggered whilst this attack is active.
		/// </summary>
		public bool blockClimb;

		/// <summary>
		/// If true don't allow a special movement to be triggered whilst this attack is active.
		/// </summary>
		public bool blockSpecial;

		/// <summary>
		/// If true reset X velocity if this attack gains control.
		/// </summary>
		public bool resetVelocityX;

		/// <summary>
		/// If true reset Y velocity if this attack gains control.
		/// </summary>
		public bool resetVelocityY;

		/// <summary>
		/// Should we apply gravity during this attack (only used if this attacks overrides movement).
		/// </summary>
		public bool applyGravity;
	}
}

