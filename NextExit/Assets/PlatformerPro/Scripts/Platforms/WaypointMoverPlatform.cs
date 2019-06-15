using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlatformerPro.Tween;

namespace PlatformerPro
{
	/// <summary>
	/// Platform class that moves between waypoints.
	/// </summary>
	public class WaypointMoverPlatform : ParentOnStandPlatform
	{

		/// <summary>
		/// The way points.
		/// </summary>
		[Tooltip ("The way points.")]
		public List<Vector2> waypoints;

		/// <summary>
		/// Speed to move towards the next waypoint at.
		/// </summary>
		[Tooltip ("Speed to move towards the next waypoint at.")]
		public float moveSpeed;

		/// <summary>
		/// How long we pause for when we reach a waypoint.
		/// </summary>
		[Tooltip ("How long we pause for when we reach a waypoint.")]
		public float pauseTime;

		/// <summary>
		/// How do we move around the waypoints?
		/// </summary>
		[Tooltip ("How do we move around the waypoints?")]
		public WaypointMoveType moveType;

		/// <summary>
		/// Pause timer.
		/// </summary>
		protected float pauseTimer;

		/// <summary>
		/// Index of the current way point.
		/// </summary>
		protected int currentWaypoint;

		/// <summary>
		/// Which way are we moving in a ping-pong.
		/// </summary>
		protected int pingPongDir = 1;

		//// <summary>
		/// Cached reference to the transform.
		/// </summary>
		protected Transform myTransform;

		protected const float MaxDistanceForNextWayPoint = 0.05f;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			base.PostInit();
			if (transform.lossyScale != Vector3.one)
			{
				Debug.LogError("Moving platforms should have a scale of (1,1,1). " +
				               "If you wish to make them larger change the size of the collider and make the visual component a child of the platform.");
			}
			myTransform = transform;
		}

		/// <summary>
		/// Do the move.
		/// </summary>
		override protected void DoMove()
		{
			// Early out if we reached the end of a ONE_OFF we can't go back
			if (moveType == WaypointMoveType.ONE_OFF && currentWaypoint >= waypoints.Count) return;
			
			if (pauseTimer > 0) 
			{
				pauseTimer -= TimeManager.FrameTime;
				return;
			}
			
			float distance = Vector2.Distance(waypoints[currentWaypoint], myTransform.position);
			Vector2 direction = waypoints [currentWaypoint] - (Vector2)myTransform.position;
			float distanceToMove = moveSpeed * TimeManager.FrameTime;

			if (distanceToMove > distance) distanceToMove = distance;
			direction.Normalize ();

			myTransform.Translate(direction * distanceToMove);

			if (distance <= MaxDistanceForNextWayPoint)
			{
				myTransform.position = new Vector3(waypoints [currentWaypoint].x, waypoints [currentWaypoint].y, myTransform.position.z);
				NextWaypoint();
			}
		}

		protected virtual void NextWaypoint()
		{
			switch (moveType)
			{
			case WaypointMoveType.RANDOM:
				int waypoint = Random.Range(0, waypoints.Count);
				if (waypoint == currentWaypoint) waypoint += (waypoint == 0) ? 1 : -1;
				currentWaypoint = waypoint;
				break;
			case WaypointMoveType.ONE_OFF:
				currentWaypoint += 1;
				if (currentWaypoint >= waypoints.Count) {
					Activated = false;
				}
				break;
			case WaypointMoveType.PING_PONG:
				if ((currentWaypoint == (waypoints.Count - 1) && pingPongDir == 1) || (currentWaypoint == 0 && pingPongDir == -1))
				{
					pingPongDir *= -1;
					if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_EXTENT) Activated = false;
				}
				currentWaypoint += pingPongDir;
				break;
			case WaypointMoveType.LOOP:
				currentWaypoint += 1;
				if (currentWaypoint >= waypoints.Count) {
					currentWaypoint = 0;
					if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_EXTENT) Activated = false;
				}
				break;
			}
		}
	}

	public enum WaypointMoveType
	{
		ONE_OFF,
		PING_PONG,
		LOOP,
		RANDOM
	}
}