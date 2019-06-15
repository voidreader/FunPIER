using UnityEngine;
using System.Collections;
using PlatformerPro.Tween;

namespace PlatformerPro
{

	/// <summary>
	/// A camera which shows a fixed zone, and then transitions to a new zone when a transition point is reached.
	/// </summary>
	public class FixedZoneCamera : PlatformCamera
	{
		/// <summary>
		/// How fast to move to the new camera zone.
		/// </summary>
		[Tooltip ("How fast to move to the new camera zone.")]
		public float transitionSpeed;

		/// <summary>
		/// How to mvoe to the new camera zone.
		/// </summary>
		[Tooltip ("How to move to the new camera zone.")]
		public TweenMode tweenMode;

		/// <summary>
		/// True while the camera is moving.
		/// </summary>
		protected bool isInTransition;

		/// <summary>
		/// Tweener which handles any moves.
		/// </summary>
		protected PositionTweener tweener;

		/// <summary>
		/// Zone we are moving to.
		/// </summary>
		protected CameraZone targetZone;

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public void Init()
		{
			base.Init ();
			tweener = GetComponent<PositionTweener> ();
			if (tweener == null) {
				tweener = gameObject.AddComponent<PositionTweener> ();
				tweener.UseGameTime = true;
			}
		}

	 	/// <summary>
		/// Changes the zone by smoothly animating to the new zone.
		/// </summary>
		/// <param name="newZone">The zone to move to.</param>
		override public void ChangeZone(CameraZone newZone)
		{
			if (tweener.Active) tweener.Stop();
			tweener.TweenWithRate(tweenMode, transform, newZone.CameraPosition, transitionSpeed, ZoneHasBeenChanged);
			currentZone = null;
			targetZone = newZone;
			isInTransition = true;
		}


		/// <summary>
		/// Called when the zones the been changed.
		/// </summary>
		virtual public void ZoneHasBeenChanged(Transform t, Vector3 p)
		{
			currentZone = ((CameraZone)targetZone).ActualZone;
			isInTransition = false;
		}
	}
}
