using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Very simple parralax scroller.
	/// </summary>
	public class SimpleParallax : MonoBehaviour
	{

		/// <summary>
		/// The main camera that moves with character.
		/// </summary>
		public Camera scrollCamera;

		/// <summary>
		/// THow much the background moves with the camera.
		/// </summary>
		[Range(0,1)]
		[Tooltip("How much the background moves with the camera, from 0 (not at all) to 1 (track camera 1 for 1).")]
		public float factor = 0.33f;

		/// <summary>
		/// The last camera position.
		/// </summary>
		protected Vector3 lastCameraPosition;

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			if (scrollCamera == null) scrollCamera = Camera.main;
			lastCameraPosition = scrollCamera.transform.position;
		}

		/// <summary>
		/// Unity LateUpdate() hook.
		/// </summary>
		void LateUpdate()
		{
			Vector3 currentCameraPosition = scrollCamera.transform.position;
			float diff = lastCameraPosition.x - currentCameraPosition.x;
			transform.Translate (-diff + (diff * factor), 0, 0);
			lastCameraPosition = currentCameraPosition;
		}
	}
}
