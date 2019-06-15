using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A collider that can register damage agaisnt the character but also supports blocking damage via a BlockAttack movement.
	/// </summary>
	public class CharacterBlockingHurtBox : CharacterHurtBox
	{
		/// <summary>
		/// Action button to use for block.
		/// </summary>
		public int actionButton;


		public BlockDirection blockDirection;

		/// <summary>
		/// The block to use. Only used if ActionButton = -1;
		/// </summary>
		public BlockAttack block;

		public List<DamageType> unblockableTypes;

		/// <summary>
		/// Angle at which an attack is considered high.
		/// </summary>
		public const float MIN_FOR_HIGH = 50.0f;

		/// <summary>
		/// Angle at which attack is considered low.
		/// </summary>
		public const float MAX_FOR_LOW = 130.0f;

		/// <summary>
		/// Unity OnEnable hook.
		/// </summary>
		void Start()
		{
			// Try looking for character health in self then parents
			if (health == null) health = gameObject.GetComponentInParent<CharacterHealth>();
			if (health == null) Debug.LogError ("Unable to find CharacterHealth for CharacterHurtBox");
			if (block == null) block = Character.GetComponentInChildren<BlockAttack>();
		}

		/// <summary>
		/// Pass damage from the given hazard to the CharacterHealth script.
		/// </summary>
		/// <param name="info">Information about the damage such as the hazard causing the damage.</param>
		override public void Damage(DamageInfo info)
		{
			if (enabled)
			{
				if (
					// Can't be blocked
					(unblockableTypes != null && unblockableTypes.Contains(info.DamageType)) ||
					// OR Didn't block
					(actionButton > -1 && !IsBlocking(info) || block == null || !block.IsBlocking(info)))
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
		}

		virtual protected bool IsBlocking(DamageInfo damageInfo)
		{
			if (Character.Input.GetActionButtonState(actionButton) == ButtonState.NONE) return false;

			// Check direction
			float angle = Mathf.Rad2Deg * Mathf.Atan2(damageInfo.Direction.x * Character.LastFacedDirection, damageInfo.Direction.y);

			// Check direction
			if (blockDirection == BlockDirection.ALL) return true;
			if (blockDirection == BlockDirection.FRONT && (angle > 0 && angle < 180)) return true;

			if (blockDirection == BlockDirection.BACK && (angle < 0 && angle > -180)) return true;

			if (blockDirection == BlockDirection.FRONT_WITH_LOW_HIGH)
			{
				if (Character.Input.VerticalAxisDigital == -1 && (angle > MIN_FOR_HIGH && angle < 180)) return true;
				if (Character.Input.VerticalAxisDigital > -1 && (angle > 0 && angle < MAX_FOR_LOW)) return true;

			}
			if (blockDirection == BlockDirection.FRONT_NOT_HIGH && (angle > MIN_FOR_HIGH && angle < 180)) return true;
			if (blockDirection == BlockDirection.FRONT_NOT_LOW && (angle > 0 && angle < MAX_FOR_LOW)) return true;

			return false;
		}
	}

	public enum BlockDirection
	{
		ALL,
		FRONT,
		BACK,
		FRONT_WITH_LOW_HIGH,
		FRONT_NOT_HIGH,
		FRONT_NOT_LOW
	}
}