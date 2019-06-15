using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Provides aiming details for a projectile.
	/// </summary>
	public class ProjectileAimer : MonoBehaviour
	{
		/// <summary>
		/// How is the projectile aimed?
		/// </summary>
		[Tooltip ("How is the projectile aimed?")]
		public ProjectileAimingTypes aimType;

		/// <summary>
		/// How far offset is the bullet from the character position.
		/// </summary>
		[Tooltip ("How far offset is the bullet from the character position.")]
		public Vector2 offset;

		/// <summary>
		/// What is the default angle to shoot at when facing right (used for ANGLED type shooting only).
		/// </summary>
		[Tooltip ("What is the default angle to shoot at when facing right (used for ANGLED type shooting only). X value will be * -1 when facing left.")]
		public Vector2 angle;

		/// <summary>
		/// If true aiming function as normal when crouched. If false crouch shooting is always in facing direction.
		/// </summary>
		[Tooltip ("If true aiming function as normal when crouched. If false crouch shooting is always in facing direction.")]
		public bool canAimWhenCrouched;

		/// <summary>
		/// Alternate offset to use when crouching.
		/// </summary>
		[Tooltip ("How far offset is the bullet from the character position when crouching.")]
		public Vector2 crouchOffset;

		/// <summary>
		/// Gets the aim direction.
		/// </summary>
		/// <returns>The aim direction.</returns>
		virtual public Vector2 GetAimDirection(Component character)
		{
			switch(aimType)
			{
				case ProjectileAimingTypes.NONE: 
					if (character is IMob) return new Vector2(((IMob)character).LastFacedDirection, 0);
					break;
				case ProjectileAimingTypes.ANGLED: 
					if (character is IMob) return new Vector2(angle.x * ((IMob)character).LastFacedDirection, angle.y).normalized;
					break;
				case ProjectileAimingTypes.MOUSE: 
					return MouseAimDirection(character);
				case ProjectileAimingTypes.EIGHT_WAY: 
					return EightWayAimDirection(character);
				case ProjectileAimingTypes.SIX_WAY: 
					return SixWayAimDirection(character);
				case ProjectileAimingTypes.FOUR_WAY: 
					return FourWayAimDirection(character);
			}
			return Vector2.zero;
		}

		/// <summary>
		/// Offsets the projectile from character position.
		/// </summary>
		/// <returns>The aim offset.</returns>
		virtual public Vector2 GetAimOffset(IMob character)
		{
			if (character.AnimationState == AnimationState.CROUCH)
			{
				if (character.LastFacedDirection == -1) return new Vector2(-crouchOffset.x, crouchOffset.y);
				return crouchOffset;
			}
			if (character.LastFacedDirection == -1) return new Vector2(-offset.x, offset.y);
			return offset;
		}

		/// <summary>
		/// Get aim direction based on character position relative to mouse
		/// </summary>
		/// <returns>The aim direction.</returns>
		virtual protected Vector2 MouseAimDirection(Component character)
		{
			Vector3 pos = Camera.main.WorldToScreenPoint(character.transform.position);
			return (UnityEngine.Input.mousePosition - pos).normalized;
		}

		/// <summary>
		/// Get aim direction based on eight way directions using character input.
		/// </summary>
		/// <returns>The aim direction.</returns>
		virtual protected Vector2 EightWayAimDirection(Component character)
		{
			if (character is Character)
			{
				if (!canAimWhenCrouched && ((Character)character).AnimationState == AnimationState.CROUCH)
				{
					return new Vector2(((Character)character).LastFacedDirection, 0);
				}
				Vector2 result = new Vector2(((Character)character).Input.HorizontalAxisDigital, ((Character)character).Input.VerticalAxisDigital);
				if (result == Vector2.zero) result = new Vector2(((Character)character).LastFacedDirection, 0); 
				return result;
			}
			Debug.LogError("Tried to get eight-way input but provided component was not a Character!");
			return Vector2.zero;
		}

		/// <summary>
		/// Get aim direction based on six way directions using character input.
		/// </summary>
		/// <returns>The aim direction.</returns>
		virtual protected Vector2 SixWayAimDirection(Component character)
		{
			if (character is Character)
			{
				Vector2 direction = EightWayAimDirection (character);
				if (direction == Vector2.up) direction = new Vector2 (((Character)character).LastFacedDirection, 1);
				if (direction == -Vector2.up) direction = new Vector2 (((Character)character).LastFacedDirection, -1);
				return direction;
			}
			Debug.LogError("Tried to get six-way input but provided component was not a Character!");
			return Vector2.zero;
		}

		/// <summary>
		/// Get aim direction based on four way directions using character input.
		/// </summary>
		/// <returns>The aim direction.</returns>
		virtual protected Vector2 FourWayAimDirection(Component character)
		{
			if (character is Character)
			{
				Vector2 direction = EightWayAimDirection (character);
				if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x)) 
				{
					direction = (new Vector2 (0, direction.y)).normalized;
				}
				else
				{
					direction = (new Vector2 (direction.x, 0)).normalized;
				}
				return direction;
			}
			Debug.LogError("Tried to get six-way input but provided component was not a Character!");
			return Vector2.zero;
		}

	}

	/// <summary>
	/// Different types of projectile aiming
	/// </summary>
	public enum ProjectileAimingTypes
	{
		NONE,
		MOUSE,
		EIGHT_WAY,
		SIX_WAY,
		ANGLED,
		FOUR_WAY
	}

}