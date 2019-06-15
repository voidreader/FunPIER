using UnityEngine;
using System.Collections;
using PlatformerPro.Tween;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// FX class which fades colour of an object.
	/// </summary>
	public class FX_FadeToColour : FX_Base
	{
		/// <summary>
		/// Target color.
		/// </summary>
		public Color fadeColour;

		/// <summary>
		/// How long to fade for.
		/// </summary>
		public float fadeTime;

		/// <summary>
		/// Target to fade.
		/// </summary>
		public Component fadeTarget;

		/// <summary>
		/// The type of the ease.
		/// </summary>
		[Tooltip ("Ease type to use for the move (if unsure use Linear).")]
		public TweenMode tweenMode;

		/// <summary>
		/// Tweener which handles fades.
		/// </summary>
		protected ColorTweener tweener;

		/// <summary>
		/// Do the effect.
		/// </summary>
		override protected void DoEffect()
		{
			tweener = GetComponent<ColorTweener> ();
			if (tweener == null) {
				tweener = gameObject.AddComponent<ColorTweener> ();
				tweener.UseGameTime = true;
			}
			tweener.TweenWithTime(tweenMode, fadeTarget, fadeColour, fadeTime, null);
		}
	}
}