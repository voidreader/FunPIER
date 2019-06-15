using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A simple raycast collider wrapping a standard 2D raycast in a way that ensures no per frame allocation.
	/// </summary>
	public class NoAllocationRaycast : BasicRaycast
	{
		/// <summary>
		/// The maximum number of raycasts to fire.
		/// </summary>
		public const int MAX_HITS = 4;
		
		/// <summary>
		/// The maximum layer to check.
		/// </summary>
		public const int MAX_LAYER = 16;
		
		/// <summary>
		/// Cached array of hits.
		/// </summary>
		protected RaycastHit2D[] hits;

		/// <summary>
		/// Store an empty raycast hit.
		/// </summary>
		public static RaycastHit2D EmptyRaycastHit = new RaycastHit2D();

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
						if (hits[pos].collider != null) pos++;
					}
				}
			}
			for (int j = pos; j < hits.Length; j++)
			{
				hits[j] = EmptyRaycastHit;
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
			if (RaycastType == RaycastType.SIDE_RIGHT)
			{
				
			}
			else if (RaycastType == RaycastType.SIDE_LEFT)
			{
				
			}
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