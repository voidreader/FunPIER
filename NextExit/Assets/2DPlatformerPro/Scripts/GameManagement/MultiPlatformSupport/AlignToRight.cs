using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Aligns a GO to the right edge of the camera.
	/// </summary>
	public class AlignToRight : MonoBehaviour 
	{
		/// <summary>
		/// The align camera.
		/// </summary>
		public Camera alignCamera;

		/// <summary>
		/// The offset from left edge.
		/// </summary>
		public float offset = 0.0f;

		// Use this for initialization
		void Awake () {
			DoAlign ();		
		}

		/// <summary>
		/// Does the align.
		/// </summary>
		virtual protected void DoAlign()
		{
			if (alignCamera == null) alignCamera = Camera.main;
			// Just assume y value is centre, we could refine this for 3D with perspective by translation objects y position to scren position.
			Vector3 point = alignCamera.ScreenToWorldPoint (new Vector3(alignCamera.pixelWidth, alignCamera.pixelHeight / 2.0f, -alignCamera.transform.position.z));
			transform.position = new Vector3 (point.x - offset, transform.position.y, transform.position.z);
		}
	}
}
