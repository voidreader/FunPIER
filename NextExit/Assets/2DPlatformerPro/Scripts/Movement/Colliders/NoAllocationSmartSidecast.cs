/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A raycast collider specifically for the sides. It uses the character reference to provide smarter collisions.
	/// </summary>
	[System.Serializable]
	public class NoAllocationSmartSidecast : BasicRaycast, IRaycastColliderWithIMob
	{
		/// <summary>
		/// The maximum number of raycasts to fire.
		/// </summary>
		protected const int MAX_HITS = 4;

		/// <summary>
		/// The maximum layer to check.
		/// </summary>
		protected const int MAX_LAYER = 16;
		
		/// <summary>
		/// We need some minimum length to avoid issues at zero velocity.
		/// </summary>
		protected const float minSideLength = 0.33f;
		
		/// <summary>
		/// Cached array of hits.
		/// </summary>
		protected RaycastHit2D[] hits;
		
		/// <summary>
		/// The character reference.
		/// </summary>
		protected IMob character;

		/// <summary>
		/// Store an empty raycast hit.
		/// </summary>
		protected RaycastHit2D emptyRaycastHit = new RaycastHit2D();

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
				if (!Application.isPlaying) return minSideLength;
				#endif
				if (Mob == null)
				{
					Debug.LogError("Smart Raycasts need a Mob (Character) Reference");
					return minSideLength;
				}
				else
				{
					if (RaycastType == RaycastType.SIDE_RIGHT)
					{
						if (Mob.Velocity.x > 0) return minSideLength + (Mob.Velocity.x * TimeManager.FrameTime);
					}
					else
					{
						if (Mob.Velocity.x < 0) return minSideLength + (-Mob.Velocity.x * TimeManager.FrameTime);
					}
					return minSideLength;
				}
			}
			set
			{
				
			}
		}
		
		/// <summary>
		/// Gets or sets the layer mask for the collision.
		/// </summary>
		/// <value>The layer mask.</value>
		override public int LayerMask
		{
			get
			{
				return layerMask;
			}
			set
			{
				layerMask = value;
				int bitCount = CountBits(layerMask);
				if (bitCount < MAX_HITS) hits = new RaycastHit2D[bitCount];
				else hits = new RaycastHit2D[MAX_HITS];
			}
		}
		
		/// <summary>
		/// Get the first raycast hit.
		/// </summary>
		override public RaycastHit2D GetRaycastHit()
		{
			if (Disabled) return new RaycastHit2D();
			Vector2 direction = Transform.rotation * GetDirection ();
			RaycastHit2D hit = Physics2D.Raycast(WorldPosition,  direction, Length + LookAhead, LayerMask);
			if (hit.fraction > 0.0f)
			{
				float deg = Vector3.Angle(hit.normal, direction) - 90 ;
				if (Mathf.Abs (deg) >= character.MinSideAngle) 
				{
					return hit;
				}
				else
				{
					Mob.IgnoredSlope = Mathf.Abs (deg);
					return new RaycastHit2D();
				}
			}
			return new RaycastHit2D();
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
			Vector2 direction = Transform.rotation * GetDirection();
			float length = Length + LookAhead;
			for (int i = 0; i < MAX_LAYER; i++)
			{
				// Skip the built-in layers except default and water
				if (i == 0 || i == 4 || i > 7 ) 
				{
					if (pos >= hits.Length) break;
					if ((LayerMask & 1 << i) == 1 << i)
					{
						hits[pos] = Physics2D.Raycast(worldPosition, direction, length, 1 << i);
						if (hits[pos].fraction > 0.0f)
						{
							float deg = 0;
							Vector2 surface = Vector2.zero;
							if (raycastType == RaycastType.SIDE_RIGHT) 
							{
								surface = Quaternion.Euler(0, 0, -90) * hits[pos].normal;
								deg = Vector2.Angle(direction, surface);
								Vector3 cross = Vector3.Cross(direction, surface);
								if (cross.z < 0) deg = 360 - deg;
							}
							else 
							{
								surface = Quaternion.Euler(0, 0, 90) * hits[pos].normal;
								deg = Vector2.Angle(direction, surface);
								Vector3 cross = Vector3.Cross(direction, surface);
								if (cross.z > 0) deg = 360 - deg;
								
							}
							if (deg >= character.MinSideAngle) 
							{
								pos++;
							}
							else
							{
								Mob.IgnoredSlope = deg;
							}
						}
					}
				}
			}
			for (int j = pos; j < hits.Length; j++)
			{
				hits[j] = emptyRaycastHit;
			}
			return hits;
		}
		
		#region Unity hooks
		
		void Awake()
		{
			Init();
		}
		
		#endregion
		
		#region protected methods
		
		/// <summary>
		/// Set up the raycasts.
		/// </summary>
		override protected void Init()
		{
			hits = new RaycastHit2D[MAX_HITS];
		}
		
		/// <summary>
		/// Counts the bits that are set to 1 in the LayerMask
		/// </summary>
		/// <returns>The number of bits set to 1.</returns>
		virtual protected int CountBits(int layerMask)
		{
			long uCount;
			
			uCount = LayerMask - ((LayerMask >> 1) & 033333333333) - ((LayerMask >> 2) & 011111111111);
			return (int)((uCount + (uCount >> 3)) & 030707070707) % 63;
		}
		
		#endregion
		
	}
	
}