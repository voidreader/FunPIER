using UnityEngine;
using System.Collections;
using PlatformerPro.Tween;

namespace PlatformerPro
{
	/// <summary>
	/// An enemy movement that moves directly to a position.
	/// </summary>
	public class EnemyMovement_MoveToPosition : EnemyMovement, ICompletableMovement
	{
		
		/// <summary>
		/// TTarget position in world space.
		/// </summary>
		public Vector2 targetPosition;

		/// <summary>
		/// Speed to move towards the target at.
		/// </summary>
		public float moveSpeed;

		/// <summary>
		/// Any tweening to apply.
		/// </summary>
		public TweenMode tweenMode;

		/// <summary>
		/// Tweener which handles any moves.
		/// </summary>
		protected IMobPositionTweener tweener;

		/// <summary>
		/// Has tween started?
		/// </summary>
		protected bool tweenStarted;

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Move to Position";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that tweens character to given target position.";
		
		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		new public static MovementInfo Info
		{
			get
			{
				return new MovementInfo(Name, Description);
			}
		}
		
		#endregion

		/// <summary>
		/// Does the movement.
		/// </summary>
		override public bool DoMove()
		{
			if (!tweenStarted)
			{
				tweener = GetComponent<IMobPositionTweener> ();
				if (tweener == null) {
					tweener = gameObject.AddComponent<IMobPositionTweener> ();
					tweener.UseGameTime = true;
				}
				tweener.TweenWithRate(tweenMode, enemy, targetPosition, moveSpeed, MoveComplete);
				tweenStarted = true;
			}
			return true;
		}

		/// <summary>
		/// Called when this movement is gaining control.
		/// </summary>
		override public void GainingControl()
		{
			if (tweener != null) tweener.Stop ();
			tweenStarted = false;
		}
		
		/// <summary>
		/// Called when this movement is losing control.
		/// </summary>
		/// <returns><c>true</c>, if a final animation is being played and control should not revert <c>false</c> otherwise.</returns>
		override public bool LosingControl()
		{
			tweener.Stop ();
			tweenStarted = false;
			return false;
		}

		/// <summary>
		/// Callback for when move is done.
		/// </summary>
		virtual public void MoveComplete(IMob target, Vector3 finalPosition)
		{
			enemy.MovementComplete ();
			tweenStarted = false;
		}
	}

}