using UnityEngine;
using System.Collections;
using PlatformerPro.Tween;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// FX class which tweens position of a UI element.
	/// </summary>
	public class FX_UIMoveTo : FX_Base
	{
		/// <summary>
		/// Target position.
		/// </summary>
		[Tooltip ("Position to move to.")]
		public Vector3 targetPosition;
		
		/// <summary>
		/// How long to fade for.
		/// </summary>
		[Tooltip ("How fast to move.")]
		public float moveSpeed;
		
		/// <summary>
		/// The type of the ease.
		/// </summary>
		[Tooltip ("Ease type to use for the move (if unsure use Linear).")]
		public TweenMode tweenMode;
		
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
		/// Tweener which hnadles any moves.
		/// </summary>
		protected PositionTweener tweener;
		
		protected bool hasSetToPosition = false;
		
		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Awake()
		{
			if (playOnAwake) DoEffect();
		}
		
		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			EnsureToPosition ();
		}
		
		/// <summary>
		/// Do the effect.
		/// </summary>
		override protected void DoEffect()
		{
			EnsureToPosition ();
			tweener = GetComponent<PositionTweener> ();
			if (tweener == null) {
				tweener = gameObject.AddComponent<PositionTweener> ();
				tweener.UseGameTime = false;
			}
			tweener.TweenWithRate(tweenMode, transform, toPosition, moveSpeed, null);
		}
		
		/// <summary>
		/// Ensure we have set the to position.
		/// </summary>
		virtual protected void EnsureToPosition()
		{
			if (!hasSetToPosition)
			{
				toPosition = isRelative ? ((RectTransform)transform).position + targetPosition : targetPosition;
				hasSetToPosition = true;
			}
		}
		
	}
}