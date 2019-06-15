using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Class which ensures whatver is being moved is snapped to a pixel position.
	/// </summary>
	public class QuantizeMovementToPixels : MonoBehaviour
	{
		/// <summary>
		/// Allow sub-pixel movement in smoothingFactor increments.
		/// </summary>
		[Range (1,16)]
		[Tooltip ("Allow sub pixel movement, 1 means no sub-pixel movement, 16 means 1/16th of a pixel. When larger values are" +
		          "combined with a pixel snapping shader this will help smooth movement.")]
		public int smoothingFactor = 8;

		/// <summary>
		/// Ortho size (assume camera does not zoom).
		/// </summary>
		protected float orthoUnits;

		/// <summary>
		/// Store actual position so we can revert after render.
		/// </summary>
		protected Vector3 preRenderPosition;

		/// <summary>
		/// Has this been initialised?
		/// </summary>
		protected bool initialised;

		/// <summary>
		/// Unity PreRender hook.
		/// </summary>
		void OnPreRender () {
			if (!initialised)
			{
				Camera myCamera = (PlatformCamera.DefaultCamera != null) ? PlatformCamera.DefaultCamera.GetComponent<Camera>() : Camera.main;
				orthoUnits = Screen.height / (myCamera.orthographicSize);
				initialised = true;
			}
			preRenderPosition = transform.position;
			transform.position = new Vector3(Quantize(transform.position.x), Quantize(transform.position.y), transform.position.z);
		}

		/// <summary>
		/// Unity PostRender hook.
		/// </summary>
		void OnPostRender ()
		{
			transform.position = preRenderPosition;
		}

		/// <summary>
		/// Quantize the specified position to a pixel position
		/// </summary>
		/// <param name="position">Position.</param>
		protected float Quantize(float position)
		{
			float valueInPixels = orthoUnits * (float)smoothingFactor * position;
			valueInPixels = Mathf.Round(valueInPixels);
			return (valueInPixels / (orthoUnits * (float)smoothingFactor));
		}
	}

}