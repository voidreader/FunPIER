using UnityEngine;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// FX class which tweens position of a UI element.
	/// </summary>
	public class FX_SnapTo : FX_Base
	{
		/// <summary>
		/// Target position.
		/// </summary>
		[Tooltip ("Position to snap to.")]
		public Vector3 targetPosition;

		/// <summary>
		/// The is relative.
		/// </summary>
		[Tooltip ("If true the targetPosition is relative to the starting position")]
		public bool isRelative;
		
		/// <summary>
		/// To position.
		/// </summary>
		protected Vector3 toPosition;
		
		/// <summary>
		/// The tween arguments.
		/// </summary>
		protected Hashtable tweenArgs;
		
		/// <summary>
		/// Unity start hook.
		/// </summary>
		public void Awake()
		{
			if (playOnAwake) DoEffect();
		}

		/// <summary>
		/// Do the effect.
		/// </summary>
		override protected void DoEffect()
		{
			transform.position = isRelative ? ((Transform)transform).position + targetPosition : targetPosition;
		}
	}
}