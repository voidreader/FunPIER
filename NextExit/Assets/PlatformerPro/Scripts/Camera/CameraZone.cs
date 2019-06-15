using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        /// Track if the zone is smaller than camera viewport. If it is we wont snap to bounds.
        /// </summary>
        public bool ZoneSmallerThanView 
        {
            get; protected set;
        }

		/// <summary>
		/// Static list of all camera zones.
		/// </summary>
		public static List<CameraZone> cameraZones = new List<CameraZone>();

		/// <summary>
		/// Current camera
		/// </summary>
		Camera bestCamera;
		
		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init ();
			// TODO Listen for will exit scene to clear up cameraZones
		}

        /// <summary>
        /// Make sure we remove reference when we destroy this.
        /// </summary>
        void OnDestroy()
        {
            cameraZones.Remove(this);
        }

        /// <summary>
        /// Returns true if transform is in zone.
        /// </summary>
        /// <returns><c>true</c> if this instance is in zone the specified t; otherwise, <c>false</c>.</returns>
        /// <param name="t">Transform.</param>
        public bool IsInZone(Transform t)
		{
			if ((Mathf.Abs(t.position.x - transform.position.x) < (width / 2.0f)) &&
				(Mathf.Abs(t.position.y - transform.position.y) < (height / 2.0f)))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Checks bounds make sense (i.e. are not too small to fit main camera) and then stores them.
		/// </summary>
		virtual protected void Init()
		{
            ZoneSmallerThanView = false;
            if (bestCamera == null) GetBestCamera();
			max = Max (bestCamera);
			min = Min (bestCamera);
			if (min.x > max.x)
			{
                ZoneSmallerThanView = true;
                #if UNITY_EDITOR
                Debug.LogWarning ("Camera zone is smaller than cameras viewport");
				#endif
			}
			if (min.y > max.y)
			{
                ZoneSmallerThanView = true;
                #if UNITY_EDITOR
                Debug.LogWarning ("Camera zone is smaller than cameras viewport");
				#endif
			}
			initialised = true;
			cameraZones.Add (this);
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

		public Vector3 GetBestPositionForCharacter(Camera camera, Character character)
		{
			Vector3 pos = CameraPosition;
			if (character == null)  return CameraPosition;
			// X
			if (character.transform.position.x >= Min (camera).x && character.transform.position.x <= Max (camera).x)
				pos.x = character.transform.position.x;
			else if (character.transform.position.x < Min (camera).x) pos.x = Min (camera).x;
			else if (character.transform.position.x > Max (camera).x) pos.x = Max (camera).x;
			// Y
			if (character.transform.position.y >= Min (camera).y && character.transform.position.y <= Max (camera).y)
				pos.y = character.transform.position.y;
			else if (character.transform.position.y < Min (camera).y) pos.y = Min (camera).y;
			else if (character.transform.position.y > Max (camera).y) pos.y = Max (camera).y;
			return pos;
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

		/// <summary>
		/// Try to find best camera for drawing camera zones.
		/// </summary>
		void GetBestCamera()
		{
			PlatformerProStandardCamera[] ppCameras = FindObjectsOfType<PlatformerProStandardCamera>();
			PlatformerProStandardCamera gizmoCamPp = null;
			for (int i = 0; i < ppCameras.Length; i++)
			{
				if (gizmoCamPp == null)
				{
					gizmoCamPp = ppCameras[i];
				}
				else if (ppCameras[i].isDefaultCamera)
				{
					gizmoCamPp = ppCameras[i];
					break;
				}
			}
			if (gizmoCamPp != null)
			{
				bestCamera = gizmoCamPp.GetComponent<Camera>();
				if (bestCamera == null) bestCamera = gizmoCamPp.GetComponentInChildren<Camera>();
			}
			if (bestCamera == null) bestCamera = Camera.main;
		}

#if UNITY_EDITOR
		
		/// <summary>
		/// Unity gizmo hook, draw the full zone and default camera position.
		/// </summary>
		void OnDrawGizmos()
		{
			if (bestCamera == null) GetBestCamera();
			Camera camera = (bestCamera != null) ? bestCamera : Camera.main;			
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