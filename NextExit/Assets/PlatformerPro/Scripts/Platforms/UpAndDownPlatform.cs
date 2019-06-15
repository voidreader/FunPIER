using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that moves up and down between two set points at a fixed speed.
	/// </summary>
	public class UpAndDownPlatform : ParentOnStandPlatform
	{
		/// <summary>
		/// The distance from starting position to the highest extent.
		/// </summary>
		public float topOffset;

		/// <summary>
		/// The distance from starting position to the lowest extent.
		/// </summary>
		public float bottomOffset;

		/// <summary>
		/// The speed the platform moves at.
		/// </summary>
		public float speed;

	
		//// <summary>
		/// Cached reference to the transform.
		/// </summary>
		protected Transform myTransform;

		/// <summary>
		/// The top extent.
		/// </summary>
		protected float topExtent;

		/// <summary>
		/// The bottom extent.
		/// </summary>
		protected float bottomExtent;

		/// <summary>
		/// The position the platform started at.
		/// </summary>
		protected Vector2 startingPosition;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			base.PostInit();
			if (transform.lossyScale != Vector3.one)
			{
				Debug.LogError("Moving platforms should have a scale of (1,1,1). " +
				               "If you wish to make them larger change the size of the collider and make the visual component a child of the platform.");
			}
			myTransform = transform;
			bottomExtent = transform.position.y - bottomOffset;
			topExtent = transform.position.y + topOffset;
		}



		/// <summary>
		/// Do the move.
		/// </summary>
		override protected void DoMove()
		{
			if (speed > 0)
			{
				// We have the additional check so we can beter support platforms starting at the wrong spot
				if (myTransform.position.y >= topExtent)
				{
					speed *= -1;
					if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_EXTENT) 
					{
						Activated = false;
						OnPlatformDeactivated(null);
					}
				}
				else
				{
					float distance = speed * TimeManager.FrameTime;
					myTransform.Translate(0, distance, 0);
					if (myTransform.position.y > topExtent)
					{
						float difference = distance - (myTransform.position.y - topExtent);
						myTransform.position = new Vector3(myTransform.position.x, topExtent - difference, myTransform.position.z);
						speed *= -1;
						if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_EXTENT) 
						{
							Activated = false;
							OnPlatformDeactivated(null);
						}
					}
				}
			}
			else if (speed < 0)
			{
				if (myTransform.position.y <= bottomExtent)
				{
					speed *= -1;
					if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_EXTENT) 
					{
						Activated = false;
						OnPlatformDeactivated(null);
					}
				}
				else
				{

					float distance = speed * TimeManager.FrameTime;
					myTransform.Translate(0, distance, 0);
					if (myTransform.position.y < bottomExtent)
					{
						float difference = distance - (myTransform.position.y - bottomExtent);
						myTransform.position = new Vector3(myTransform.position.x, bottomExtent - difference, myTransform.position.z);
						speed *= -1;
						if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_EXTENT) 
						{
							Activated = false;
							OnPlatformDeactivated(null);
						}
					}
				}
			}
		}


		/// <summary>
		/// Draw handles for showing extents
		/// </summary>
		void OnDrawGizmos() {
			float left =0.0f; float right = 0.0f;

			if (GetComponent<Collider2D>() is EdgeCollider2D)
			{
				EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
				for (int i = 0; i < edgeCollider.points.Length; i++)
				{
					if (edgeCollider.points[i].y > right) right = edgeCollider.points[i].y;
					if (edgeCollider.points[i].y < left) left = edgeCollider.points[i].y;
				}
			}
			else if (GetComponent<Collider2D>() is BoxCollider2D)
			{
				BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
				right = boxCollider.size.y / 2.0f;
				left = boxCollider.size.y / -2.0f;
			}

			Gizmos.color = Platform.GizmoColor;
			Gizmos.DrawLine(transform.position,  transform.position + new Vector3(0, topOffset + right,  0));
			Gizmos.DrawLine(transform.position + new Vector3(0.25f, topOffset + right, 0), transform.position + new Vector3( -0.25f, topOffset + right, 0));
			Gizmos.DrawLine(transform.position,  transform.position + new Vector3(0, -bottomOffset + left,  0));
			Gizmos.DrawLine(transform.position + new Vector3(0.25f, -bottomOffset + left,  0),  transform.position + new Vector3(-0.25f,-bottomOffset + left, 0));
		}
	}

}