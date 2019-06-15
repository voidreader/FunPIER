using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An area which triggers a transition of the camera.
	/// </summary>
	public class CameraTransitionPoint : CameraZone
	{
		/// <summary>
		/// The parent camera zone.
		/// </summary>
		protected CameraZone parentZone;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Gets the actual zone that the camera should be in (i.e. the parent zone).
		/// </summary>
		/// <value>The actual zone.</value>
		override public CameraZone ActualZone
		{
			get 
			{
				return parentZone;
			}
		}

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override protected void Init()
		{
			if (transform.parent != null) 
			{
				parentZone = transform.parent.GetComponentInParent<CameraZone> ();
			}
			if (parentZone == null) Debug.LogError ("A CameraTransitionPoint must be the child of a CameraZone");
		}
	
#if UNITY_EDITOR

		/// <summary>
		/// Unity gizmo hook, draw the camera zone.
		/// </summary>
		void OnDrawGizmos()
		{

		}

#endif

	}
}