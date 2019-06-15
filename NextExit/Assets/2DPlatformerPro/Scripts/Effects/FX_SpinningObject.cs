using UnityEngine;
using System.Collections;
using PlatformerPro.Tween;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Simple FX class which spins an object a fixed amount in a fixed time.
	/// </summary>
	public class FX_SpinningObject : FX_Base
	{
		/// <summary>
		/// How fast to rotate in x,y,z.
		/// </summary>
		public Vector3 rotationSpeed;

		/// <summary>
		/// How long to rotate for.
		/// </summary>
		public float rotationTime;

		/// <summary>
		/// The type of the ease.
		/// </summary>
		[Tooltip ("Ease type to use for the move (if unsure use Linear).")]
		public TweenMode tweenMode;

		
		/// <summary>
		/// Tweener which hnadles any moves.
		/// </summary>
		protected RotationBySpeedTweener tweener;

		/// <summary>
		/// Do the effect.
		/// </summary>
		override protected void DoEffect()
		{
			tweener = GetComponent<RotationBySpeedTweener> ();
			if (tweener == null) {
				tweener = gameObject.AddComponent<RotationBySpeedTweener> ();
				tweener.UseGameTime = true;
			}
			tweener.TweenWithTime(tweenMode, transform, rotationSpeed, rotationTime, null);
		}
	}
}