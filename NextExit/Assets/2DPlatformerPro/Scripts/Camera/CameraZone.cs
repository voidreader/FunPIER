using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// An area which the camera can be moved to and within.
	/// </summary>
	public class CameraZone : MonoBehaviour
	{
		/// <summary>
		/// The cameras Z offset from this zones transform. 
		/// </summary>
		public float cameraZOffset;

		/// <summary>
		/// Width of the zone in world units.
		/// </summary>
		public float width;

		/// <summary>
		/// Height of the zone in world units.
		/// </summary>
		public float height;

		public bool updateDynamically;

		protected Vector2 min;

		protected Vector2 max;

		protected bool initialised;

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Checks bounds make sense (i.e. are not too small to fit main camera) and then stores them.
		/// </summary>
		virtual protected void Init()
		{
			max = Max (Camera.main);
			min = Min (Camera.main);
			if (min.x > max.x)
			{
				Debug.LogWarning ("Camera zone is smaller than cameras viewport");
				min = new Vector2(max.x - 1.0f, min.y);
			}
			if (min.y > max.y)
			{
				Debug.LogWarning ("Camera zone is smaller than cameras viewport");
				min = new Vector2(min.x, max.y - 1.0f);
			}
			initialised = true;
		}

		/// <summary>
		/// Gets the maximum bounds.
		/// </summary>
		virtual public Vector2 Max (Camera camera)
		{
			if (initialised && !updateDynamically) return max;
			max = new Vector2(transform.position.x + (width / 2.0f) - (camera.orthographicSize * camera.aspect), transform.position.y + (height / 2.0f) -camera.orthographicSize);
			return max;
		}

		/// <summary>
		/// Gets the minimum bounds.
		/// </summary>
		virtual public Vector2 Min  (Camera camera)
		{
			if (initialised && !updateDynamically) return min;
			min = new Vector2(transform.position.x - (width / 2.0f) + (camera.orthographicSize * camera.aspect), transform.position.y - (height / 2.0f) + camera.orthographicSize);
			return min;
		}

		/// <summary>
		/// Gets the position the camera should transition to.
		/// </summary>
		virtual public Vector3 CameraPosition
		{
			get
			{
				return transform.position + new Vector3(0, 0, cameraZOffset);
            }
		}

		/// <summary>
		/// Gets the actual zone that the camera should be in. Could be different to this
		/// for example in the case of a Transition Point.
		/// </summary>
		/// <value>The actual zone.</value>
		virtual public CameraZone ActualZone
		{
			get 
			{
				return this;
			}
		}

#if UNITY_EDITOR

		/// <summary>
		/// Unity gizmo hook, draw the full zone and default camera position.
		/// </summary>
		void OnDrawGizmos()
		{
			Camera camera = Camera.main;
			if (camera != null)
			{
				Matrix4x4 temp = Gizmos.matrix;
				Gizmos.color = new Color(0,0,1,0.25f);
				if (camera.orthographic) {
					// Draw initial camera pos
					Gizmos.matrix = Matrix4x4.TRS(transform.position + new Vector3(0, 0, cameraZOffset), transform.rotation, Vector3.one);
					// Not sure drawing this camera point helps much
					// float center = (camera.farClipPlane + camera.nearClipPlane)*0.5f;
					// Gizmos.DrawWireCube (new Vector3(0,0,center), new Vector3(camera.orthographicSize*2*camera.aspect, camera.orthographicSize*2, 0.1f));
					UnityEditor.Handles.color = new Color(0,0,1,0.25f);
					// Draw full extent of zone
					Vector3[] outerBounds = new Vector3[]{
						new Vector2 (transform.position.x + (width / 2.0f), transform.position.y - (height / 2.0f)),
						new Vector2 (transform.position.x + (width / 2.0f), transform.position.y + (height / 2.0f)),
						new Vector2 (transform.position.x - (width / 2.0f), transform.position.y + (height / 2.0f)),
						new Vector2 (transform.position.x - (width / 2.0f), transform.position.y - (height / 2.0f))
					};
					UnityEditor.Handles.DrawSolidRectangleWithOutline(outerBounds , new Color(0,0,1,0.125f), new Color(0,0,1,0.25f));
				} else {
					// TODO Calculate zones for non-ortho camera (assume 0 on z?)
					Gizmos.matrix = Matrix4x4.TRS(transform.position + new Vector3(0, 0, cameraZOffset), transform.rotation, Vector3.one);
	                      Gizmos.DrawFrustum(transform.position + new Vector3(0, 0, cameraZOffset), camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
				}
				Gizmos.matrix = temp;
			}
		}

#endif

	}

}