using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlatformerPro.Tween;

namespace PlatformerPro
{
	/// <summary>
	/// An enemy movement that moves through a set of waypoints.
	/// </summary>
	public class EnemyMovement_WaypointMover : EnemyMovement, ICompletableMovement
	{
		
		/// <summary>
		/// The way points.
		/// </summary>
		[Tooltip ("The way points.")]
		public List<Vector2> wayPoints;

		/// <summary>
		/// Speed to move towards the next waypoint at.
		/// </summary>
		[Tooltip ("Speed to move towards the next waypoint at.")]
		public float moveSpeed;

		/// <summary>
		/// Should we loop or stop we reach the last position?
		/// </summary>
		[Tooltip ("Should we loop or stop we reach the last position?")]
		public bool loop;

		/// <summary>
		/// Any tweening to apply.
		/// </summary>
		[Tooltip ("Any tweening to apply.")]
		public TweenMode tweenMode;

		/// <summary>
		/// Tweener which handles any moves.
		/// </summary>
		protected IMobPositionTweener tweener;

		/// <summary>
		/// Has tween started?
		/// </summary>
		protected bool tweenStarted;

		/// <summary>
		/// Index of the current way point.
		/// </summary>
		protected int currentWayPoint;

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Waypoint mover";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that moves through a set of waypoints.";
		
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
		/// Initialise this movement and return a reference to the ready to use movement.
		/// </summary>
		override public EnemyMovement Init(Enemy enemy)
		{
			base.Init (enemy);
			currentWayPoint = 0;
			if (wayPoints.Count < 1) 
			{
				Debug.LogWarning("Waypoint mover needs at least one waypoint");
				enabled = false;
			}
			return this;
		}

		/// <summary>
		/// Does the movement.
		/// </summary>
		override public bool DoMove()
		{
			// Early out don't move if not enabled
			if (!enabled) return true;

			// Early out if we have reached the last way point
			if (!loop && currentWayPoint >= wayPoints.Count) return true;

			// Loop
			if (loop && currentWayPoint >= wayPoints.Count) currentWayPoint = 0;


			if (!tweenStarted)
			{
				// Special case We use speed like a delay if the mode is set to SNAP
				if (tweenMode == TweenMode.SNAP)
				{
					StartCoroutine(SnapAfterDelay(wayPoints[currentWayPoint]));
				}
				else
				{
					Vector2 targetPosition = wayPoints[currentWayPoint];
					tweener = GetComponent<IMobPositionTweener> ();
					if (tweener == null)
					{
						tweener = gameObject.AddComponent<IMobPositionTweener> ();
						tweener.UseGameTime = true;
					}
					tweener.TweenWithRate(tweenMode, enemy, targetPosition, moveSpeed, MoveComplete);
					tweenStarted = true;
				}
			}
			return true;
		}

		/// <summary>
		/// Moves after a delay.
		/// </summary>
		/// <returns>The after delay.</returns>
		virtual protected IEnumerator SnapAfterDelay(Vector2 targetPosition)
		{
			tweenStarted = true;
			yield return new WaitForSeconds (moveSpeed);
			tweener = GetComponent<IMobPositionTweener> ();
			if (tweener == null)
			{
				tweener = gameObject.AddComponent<IMobPositionTweener> ();
				tweener.UseGameTime = true;
			}
			tweener.TweenWithRate(TweenMode.SNAP, enemy, targetPosition, moveSpeed, MoveComplete);
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
			// NOTE Even if loop is true the movement will be considered complete when it reaches the last way point
			if (currentWayPoint >= wayPoints.Count - 1) enemy.MovementComplete ();
			currentWayPoint++;
			tweenStarted = false;
		}
	}

}