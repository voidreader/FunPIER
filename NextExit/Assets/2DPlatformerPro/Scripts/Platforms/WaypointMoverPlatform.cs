using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlatformerPro.Tween;

namespace PlatformerPro
{

	public class WaypointMoverPlatform : Platform {

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
		/// Should we parent when the head collides with this platform (used when you have hang from ceiling).
		/// </summary>
		[Tooltip ("Should we parent when the head collides with this platform (used when you have hang from ceiling).")]
		public bool parentOnHeadCollission;

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
		/// Unit update hook.
		/// </summary>
		void Update()
		{
			if (Activated) DoMove();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init()
		{
			base.Init();
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
		protected virtual void DoMove()
		{
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

		/// <summary>
		/// If the collission is a foot try to parent.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="type">Type of raycast.</param>
		override protected bool CustomCollide(PlatformCollisionArgs args)
		{
			if (args.RaycastCollider.RaycastType == RaycastType.FOOT)
			{
				return true;
			}
			if (parentOnHeadCollission && args.RaycastCollider.RaycastType == RaycastType.HEAD)
			{
				return true;
			}
			
			return false;
		}


		/// <summary>
		/// Called when the character is parented to this platform.
		/// </summary>
		override public void Parent()
		{
			if (automaticActivation == PlatformActivationType.ACTIVATE_ON_STAND) Activated = true;
			if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_STAND) Activated = false;
		}
		
		/// <summary>
		/// Called when the character is unparented from this platform.
		/// </summary>
		override public void UnParent()
		{
			if (automaticActivation == PlatformActivationType.ACTIVATE_ON_LEAVE) Activated = true;
			if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_LEAVE) Activated = false;
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
					if (currentWaypoint >= waypoints.Count) Activated = false;
					break;
				case WaypointMoveType.PING_PONG:
					if ((currentWaypoint == (waypoints.Count - 1) && pingPongDir == 1) || (currentWaypoint == 0 && pingPongDir == -1))
					{
						pingPongDir *= -1;
					}
					currentWaypoint += pingPongDir;
					break;
				case WaypointMoveType.LOOP:
					currentWaypoint += 1;
					if (currentWaypoint >= waypoints.Count) currentWaypoint = 0;
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