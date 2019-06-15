using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A simple raycast collider wrapping a standard 2D raycast.
	/// </summary>
	[System.Serializable]
	public class BasicRaycast
	{
		#region protected members

		/// <summary>
		/// Stores the transform from which the ray position is relative to. Most collider will use
		/// the characters transform which is set on Init().
		/// </summary>
		[SerializeField]
		protected Transform transform;

		/// <summary>
		/// The length of the ray.
		/// </summary>
		[SerializeField]
		protected float length;

		/// <summary>
		/// The type of raycast.
		/// </summary>
		[SerializeField]
		protected RaycastType raycastType;

		/// <summary>
		/// The additional length to add to the ray to "look ahead" for special conditions.
		/// </summary>
		[SerializeField]
		protected float lookAhead;

		/// <summary>
		/// The position the ray is cast from (relative to transform).
		/// </summary>
		[SerializeField]
		protected Vector2 extent;
		
		/// <summary>
		/// The layer mask.
		/// </summary>
		[SerializeField]
		protected int layerMask;

		#endregion

		/// <summary>
		/// Enables or disables the collider.
		/// </summary>
		/// <value>The layer mask.</value>
		virtual public bool Disabled
		{
			get; set;
		}

		/// <summary>
		/// Stores the transform from which the ray position is relative to. Usually
		/// the position of the character or one of the characters limbs.
		/// </summary>
		virtual public Transform Transform
		{
			get
			{
				return transform;
			}
			set
			{
				transform = value;
			}
		}

		/// <summary>
		/// The length of the ray.
		/// </summary>
		virtual public float Length
		{
			get
			{
				return length;
			}
			set
			{
				length = value;
			}
		}

		/// <summary>
		/// The additional length to add to the ray to "look ahead" for special conditions.
		/// </summary>
		virtual public float LookAhead
		{
			get
			{
				return lookAhead;
			}
			set
			{
				lookAhead = value;
			}
		}

		/// <summary>
		/// The type of raycast.
		/// </summary>
		virtual public RaycastType RaycastType
		{
			get
			{
				return raycastType;
			}
			set
			{
				raycastType = value;
			}
		}

		/// <summary>
		/// The position the ray is cast from (relative to transform).
		/// </summary>
		virtual public Vector2 WorldPosition
		{
			get
			{
				return WorldExtent - (Vector2)(Transform.rotation * GetDirection() * Length);
			}
		}

		/// <summary>
		/// Gets or sets the layer mask for the collision.
		/// </summary>
		/// <value>The layer mask.</value>
		virtual public int LayerMask
		{
			get
			{
				return layerMask;
			}
			set
			{
				layerMask = value;
			}
		}

		
		/// <summary>
		/// Gets the extent of this collider relative to character position and rotation. For feet colliders the 
		/// extent would be the offset to add to the character to find the bottom of the foot.
		/// </summary>
		/// <value>The extent.</value>
		virtual public Vector2 Extent
		{
			get
			{
				return extent;
			}
			set
			{
				extent = value;
			}

		}

		
		/// <summary>
		/// Gets the extent of this collider in world space (i.e. with character transform applied).
		/// </summary>
		/// <value>The extent.</value>
		virtual public Vector2 WorldExtent
		{
			get
			{
				if (Transform != null) return Transform.position + Transform.rotation * extent;
				return Vector2.zero;
			}
		}

		/// <summary>
		/// Get the first raycast hit.
		/// </summary>
		virtual public RaycastHit2D GetRaycastHit()
		{
			if (Disabled) return new RaycastHit2D();
			return Physics2D.Raycast(WorldPosition,  Transform.rotation * GetDirection(), Length + LookAhead, LayerMask);
		}
		
		/// <summary>
		/// Get all raycast hits.
		/// </summary>
		virtual public RaycastHit2D[] GetRaycastHits()
		{
			if (Disabled) return new RaycastHit2D[0];
			return Physics2D.RaycastAll(WorldPosition,  Transform.rotation * GetDirection(), Length + LookAhead, LayerMask);
		}

#if UNITY_EDITOR

		/// <summary>
		/// Draws gizmos used to debug the raycast.
		/// </summary>
		virtual public void DrawGizmos(Character character) 
		{
			UnityEditor.Handles.color = Disabled ? new Color (1, 1, 1, 0.5f) : Color.white;
			Quaternion baseRotation = Quaternion.identity;
			switch (RaycastType)
			{
				case RaycastType.SIDE_LEFT:
					baseRotation = Quaternion.Euler(0,0,270);
					break;
				case RaycastType.SIDE_RIGHT:
					baseRotation = Quaternion.Euler(0,0,90);
					break;
				case RaycastType.HEAD:
					baseRotation = Quaternion.Euler(0,0,180);
					break;
			}

			// Arrow
			UnityEditor.Handles.DrawLine (WorldExtent, WorldExtent + (Vector2)(Transform.rotation * baseRotation * new Vector2(0.02f, 0.025f)));
			UnityEditor.Handles.DrawLine (WorldExtent, WorldExtent + (Vector2)(Transform.rotation * baseRotation * new Vector2(-0.02f, 0.025f)));

			// Base
			UnityEditor.Handles.DrawLine (WorldPosition + (Vector2)(Transform.rotation * baseRotation * new Vector2(-0.025f, 0.0f)), WorldPosition + (Vector2)(Transform.rotation * baseRotation * new Vector2(0.025f, 0.0f)));

			// Length
			UnityEditor.Handles.DrawLine (WorldPosition, WorldExtent);

			// LookAhead Ahead
			if (LookAhead != 0)
			{
				UnityEditor.Handles.color = Color.grey;
				Vector2 lookAheadPoint = WorldExtent + (Vector2)(Transform.rotation * GetDirection() * LookAhead);
				UnityEditor.Handles.DrawLine (WorldExtent, lookAheadPoint);

				// LookAhead Arrow
				UnityEditor.Handles.DrawLine (lookAheadPoint, lookAheadPoint + (Vector2)(Transform.rotation * baseRotation * new Vector2(0.02f, 0.025f)));
				UnityEditor.Handles.DrawLine (lookAheadPoint, lookAheadPoint + (Vector2)(Transform.rotation * baseRotation * new Vector2(-0.02f, 0.025f)));
			}

			// Collision point
			if (!Disabled)
			{
				RaycastHit2D hit = Physics2D.Raycast(WorldPosition,  Transform.rotation * GetDirection(), Length + LookAhead, LayerMask);
				if (hit.collider != null) 
				{
					if (hit.fraction * (Length + LookAhead) <= Length &&  (1 << hit.collider.gameObject.layer & character.geometryLayerMask) == 1 << hit.collider.gameObject.layer)
					{
						UnityEditor.Handles.color = Color.red;
					} 
					else
					{
						UnityEditor.Handles.color = Color.blue;
					}
					UnityEditor.Handles.DrawWireDisc(hit.point, new Vector3(0,0,1), 0.05f * UnityEditor.HandleUtility.GetHandleSize(hit.point));
				}
			}
		}
#endif

		
		/// <summary>
		/// Gets the direction for the raycast.
		/// </summary>
		/// <returns>The direction.</returns>
		virtual public Vector2 GetDirection()
		{
			switch (RaycastType)
			{
			case RaycastType.FOOT:
				return new Vector2(0,-1);
			case RaycastType.HEAD:
				return new Vector2(0, 1);
			case RaycastType.SIDE_RIGHT:
				return new Vector2(1, 0);
			case RaycastType.SIDE_LEFT:
				return new Vector2(-1, 0);
			}
			Debug.LogError("Trying to get a raycast direction for an invalid raycast type: " + RaycastType);
			return Vector2.zero;
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
		virtual protected void Init()
		{

		}


		#endregion
	}

}