using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Aimer for grapple which can shoot up or at 45 degrees (Left or Right).
	/// </summary>
	public class GrappleAimer : ProjectileAimer
	{

		/// <summary>
		/// How far offset is the bullet from the character position when throwing at 45.
		/// </summary>
		public Vector2 offsetFortyFive;

		/// <summary>
		/// When throwing at 45, how much does the character speed affect the angle?
		/// </summary>
		public float speedAffectsAngleFactor;

		/// <summary>
		/// Gets the aim direction.
		/// </summary>
		/// <returns>The aim direction.</returns>
		override public Vector2 GetAimDirection(Component character)
		{
			if (character is Character)
			{
				// If you hold up but not across OR if you are not moving at all then you fire upwards
				if (((Character)character).Input.HorizontalAxisDigital == 0 && (((Character)character).Input.VerticalAxisDigital == 1 || ((Character)character).Velocity.x == 0))
				{

					return new Vector2(0, 1);
				}
				float speedBoost = ((Character)character).Grounded ? 0 : ((Character)character).Velocity.x * speedAffectsAngleFactor;
				return new Vector2(((Character)character).LastFacedDirection + speedBoost, 1).normalized;
				// return new Vector2(((Character)character).LastFacedDirection, 1).normalized;
			}
			return Vector2.zero;
		}

	    /// <summary>
	    /// Offsets the projectile from character position.
	    /// </summary>
	    /// <returns>The aim offset.</returns>
	    override public Vector2 GetAimOffset(IMob character)
	    {
			if (character is Character)
			{
				// If you hold up but not across OR if you are not moving at all then you fire upwards
				if (((Character)character).Input.HorizontalAxisDigital == 0)
				{
					//return new Vector2(((Character)character).LastFacedDirection * offset.x, offset.y);
					return offset;
				}
				// return new Vector2(((Character)character).LastFacedDirection * offsetFortyFive.x, offsetFortyFive.y);
				return offsetFortyFive;
			}
			return Vector2.zero;
		}

	}
}
