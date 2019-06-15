/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A raycast collider wrapping a standard 2D raycast in a way that ensures no per frame allocation. That also changes the length of the 
	/// raycast based on the characters y speed. The default feet collider.
	/// </summary>
	public class NoAllocationSmartFeetcast : NoAllocationRaycast, IRaycastColliderWithIMob
	{
		/// <summary>
		/// We need some minimum length to avoid issues at zero velocity (for example on ladders).
		/// </summary>
		protected const float minFeetLength = 0.33f;

		/// <summary>
		/// The character reference.
		/// </summary>
		protected IMob character;
		
		/// <summary>
		///  Reference to the chracter
		/// </summary>
		/// <value>The character.</value>
		public IMob Mob {
			get
			{
				return character;
			}
			set
			{
				character = value;
			}
		}

		/// <summary>
		/// Calculate length based on characters velocity
		/// </summary>
		/// <value>The length.</value>
		override public float Length
		{
			get
			{
#if UNITY_EDITOR
				if (!Application.isPlaying) return minFeetLength;
#endif
				if (character == null)
				{
					Debug.LogError("Smart Raycasts need a Mob (Character) Reference");
					return minFeetLength;
				}
				else
				{
					float actualMinFeetLength = minFeetLength;
					if (character.IgnoredSlope > 0) 
					{
						// actualMinFeetLength += Mathf.Sin (character.IgnoredSlope * Mathf.Deg2Rad);
					}
					if (character.Velocity.y >= 0 && character.SlopeActualRotation == 0.0f) return actualMinFeetLength;
					return actualMinFeetLength + ((-character.Velocity.y +
					                  Mathf.Abs(Mathf.Sin (character.SlopeActualRotation * Mathf.Deg2Rad) * character.Velocity.x)) * TimeManager.FrameTime);
				}
			}
			set
			{
				length = value;
			}
		}

		/// <summary>
		/// Get all raycast hits.
		/// </summary>
		override public RaycastHit2D[] GetRaycastHits()
		{
			if (Disabled) 
			{
				for (int j = 0; j < hits.Length; j++)
				{
					hits[j] = new RaycastHit2D();
				}
				return hits;
			}
			int pos = 0;
			Vector3 worldPosition = WorldPosition;
			Vector3 direction = Transform.rotation * GetDirection();
			float length = Length + LookAhead;
			for (int i = 0; i < MAX_LAYER; i++)
			{
				// Skip the built-in layers except default and water
				if (i == 0 || i == 4 || i > 7 ) 
				{
					if (pos >= hits.Length) break;
					if ((LayerMask & (1 << i)) == 1 << i)
					{
						hits[pos] = Physics2D.Raycast(worldPosition,  direction, length, 1 << i);
						if (hits[pos].collider != null && !character.IsColliderIgnored(hits[pos].collider)) pos++;
					}
				}
			}
			for (int j = pos; j < hits.Length; j++)
			{
				hits[j] = EmptyRaycastHit;
			}
			return hits;
		}

	}

}