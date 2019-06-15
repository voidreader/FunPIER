using UnityEngine;
using System.Collections;


namespace PlatformerPro
{
	/// <summary>
	/// Provides aiming using the controllers alternate axis.
	/// </summary>
	public class AlternateAxisAimer : ProjectileAimer
	{
		/// <summary>
		/// Gets the aim direction.
		/// </summary>
		/// <returns>The aim direction.</returns>
		override public Vector2 GetAimDirection(Component character)
		{
			if (!(character is Character))
			{
				Debug.LogWarning("The AlternateAxisAimer can only be attached to characters.");
				return Vector2.zero;
			}

			if (!canAimWhenCrouched && ((Character)character).AnimationState == AnimationState.CROUCH)
			{
				return new Vector2(((Character)character).LastFacedDirection, 0);
			}

			switch(aimType)
			{
			case ProjectileAimingTypes.ANGLED: 
				if (character is IMob) return new Vector2(((Character)character).Input.AltHorizontalAxis, ((Character)character).Input.AltVerticalAxis).normalized;
				break;
			case ProjectileAimingTypes.EIGHT_WAY: 
				return EightWayAimDirectionAlt(character);
			case ProjectileAimingTypes.SIX_WAY: 
				Vector2 direction = EightWayAimDirectionAlt (character);
				if (direction == Vector2.up) direction = new Vector2 (((Character)character).LastFacedDirection, 1);
				if (direction == -Vector2.up) direction = new Vector2 (((Character)character).LastFacedDirection, -1);
				return direction;
			}
			Debug.LogWarning("The AlternateAxisAimer only supports ANGLED, EIGHT_WAY or SIX_WAY aiming.");
			return Vector2.zero;
		}

		/// <summary>
		/// Get aim direction based on eight way directions using character input on the alternate axis.
		/// </summary>
		/// <returns>The aim direction.</returns>
		virtual protected Vector2 EightWayAimDirectionAlt(Component character)
		{
			Vector2 result = new Vector2(((Character)character).Input.AltHorizontalAxisDigital, ((Character)character).Input.AltVerticalAxisDigital);
			if (result == Vector2.zero) result = new Vector2(((Character)character).LastFacedDirection, 0); 
			return result;
		}
	}
}
