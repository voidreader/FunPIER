using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlatformerPro.Tween;

namespace PlatformerPro
{
	/// <summary>
	/// Platform class that locks player to the platform and allows them to control position along the rail.
	/// </summary>
	public class RailPlatform : ParentOnStandPlatform
	{

		/// <summary>
		/// The way points.
		/// </summary>
		[Tooltip ("The way points.")]
		public List<Vector2> waypoints;

		[Header ("Movement")]

		/// <summary>
		/// Max speed to move towards the next waypoint at.
		/// </summary>
		[Tooltip ("Max speed to move towards the next waypoint at. Note that if you use slope boosts this is not a hard limit.")]
		public float moveSpeed;

		/// <summary>
		/// Rate of acceleration up to moveSpeed.
		/// </summary>
		[Tooltip ("Rate of acceleration up to moveSpeed.")]
		public float acceleration;

		/// <summary>
		/// Rate of deceleration down to zero.
		/// </summary>
		[Tooltip ("Rate of deceleration down to zero.")]
		public float deceleration;

		/// <summary>
		/// The slope acceleration.
		/// </summary>
		[Tooltip ("Should we accelerate faster when slope is downwards, and slower when up. For example on a rail cart you might want to go faster up and slower down hill. 0 means no boost. This is not physically accurate, but generally a value a little higher than acceleration will work well.")]
		public float slopeAccelerationBoost;

		[Header ("Controls")]
		/// <summary>
		/// How do we move around the waypoints?
		/// </summary>
		[Tooltip ("How do we move along the rail?")]
		public RailMoveType moveType;

		/// <summary>
		/// Can we jump off the rail?
		/// </summary>
		[Tooltip ("Can we jump off the rail?")]
		public bool allowJump;

		/// <summary>
		/// Which action button needs to be held to enable movement. Use -1 for none (auto-lock).
		/// </summary>
		[Tooltip ("Which action button needs to be held to enable movement. Use -1 for none (auto-lock).")]
		public int requiredActionButton = -1;

		[Header ("Animation")]

		/// <summary>
		/// State to play when stationary on rail.
		/// </summary>
		[Tooltip ("State to play when stationary on rail.")]
		public AnimationState idleState = AnimationState.RAIL_IDLE;

		/// <summary>
		/// State to play when moving along rail.
		/// </summary>
		[Tooltip ("State to play when moving along rail.")]
		public AnimationState movingState = AnimationState.RAIL_MOVE;

		/// <summary>
		/// The facing direction.
		/// </summary>
		[Tooltip ("Facing direction while locked to rail.")]
		[Range(-1, 1)]
		public int facingDirection = 1;

		/// <summary>
		/// CurrentSpeed
		/// </summary>
		protected float currentSpeed;

		/// <summary>
		/// Direction we are currently heading.
		/// </summary>
		protected int currentAutoDirection;

		/// <summary>
		/// Index of the current way point to the right of current position.
		/// </summary>
		protected int currentWaypointRight = 1;

		/// <summary>
		/// Index of the current way point to the left of current position.
		/// </summary>
		protected int currentWaypointLeft = 0;

		/// <summary>
		/// Character reference.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Cached rail movement reference.
		/// </summary>
		protected SpecialMovement_Rail railMovement;

		/// <summary>
		/// Tracks if we have reached either end of the rail. -1 start, -1 end, 0 anything else. Starts at -1.
		/// </summary>
		protected int hasReachedEnd = -1;

		//// <summary>
		/// Cached reference to the transform.
		/// </summary>
		protected Transform myTransform;

		/// <summary>
		/// If we are this close to a waypoint we consider ourselves at the waypoint.
		/// </summary>
		protected const float MaxDistanceForNextWayPoint = 0.05f;

		/// <summary>
		/// Which way are we moving?
		/// </summary>
		protected int MoveDirection
		{
			get
			{
				switch (moveType) 
				{
				case RailMoveType.AUTOMATIC:
					return currentAutoDirection;
				case RailMoveType.LEFT_RIGHT:
					if (character == null) return 0;
					return character.Input.HorizontalAxisDigital;
				case RailMoveType.UP_DOWN:
					if (character == null) return 0;
					return character.Input.VerticalAxisDigital;
				case RailMoveType.FOLLOW_CURVE:
					return GetFollowCurveDirection();
				}
				// NOTE: Follow curve doesn't use MoveDirection.
				return 0;
			}
		}

		/// <summary>
		/// Gets the current animation state.
		/// </summary>
		virtual public AnimationState AnimationState
		{
			get
			{
				if (MoveDirection != 0) return movingState;
				return idleState;
			}
		}

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
		/// Gets a value of -1, 1 or 0 based on the user following the curve direction with input.
		/// </summary>
		/// <returns>The follow curve direction.</returns>
		virtual protected int GetFollowCurveDirection()
		{
			// Early out, no input.
			if (character == null || character.Input.HorizontalAxisDigital == 0 && character.Input.VerticalAxisDigital == 0) return 0;

			Vector2 difference =  waypoints [currentWaypointRight] - waypoints [currentWaypointLeft];
			float sign = (waypoints [currentWaypointRight].y < waypoints [currentWaypointLeft].y)? -1.0f : 1.0f;
			float angle = Vector2.Angle(Vector2.right, difference) * sign;

			Vector2 inputDifference = new Vector2 (character.Input.HorizontalAxis, character.Input.VerticalAxis);
			float inputSign = character.Input.VerticalAxisDigital == 1 ? 1.0f : -1.0f;
			float inputAngle = Vector2.Angle(Vector2.right, inputDifference) * inputSign;

			float differenceTotal = Mathf.Abs (angle - inputAngle);

			if (differenceTotal < 33.0f) return 1;
			if (differenceTotal > (180.0f - 33.0f)) return -1;

			return 0;
		}

		/// <summary>
		/// Do the move.
		/// </summary>
		override protected void DoMove()
		{
			Vector2 direction = Vector2.zero;
			float distance = 0.0f;
			float remainingDistance = 0.0f;

			int currentMoveDirection = MoveDirection;

			// Slow down
			if (currentMoveDirection == 0 && currentSpeed != 0)
			{
				if (currentSpeed > 0) 
				{
					currentSpeed -= TimeManager.FrameTime * deceleration;
					if (currentSpeed < 0) currentSpeed = 0;
					// Update distance and direction
					direction = waypoints [currentWaypointRight] - (Vector2)myTransform.position;
					direction.Normalize ();
					distance = Vector2.Distance((Vector2)myTransform.position, waypoints [currentWaypointRight]);
				}
				else if (currentSpeed < 0) 
				{
					currentSpeed += TimeManager.FrameTime * deceleration;
					if (currentSpeed > 0) currentSpeed = 0;
					// Update distance and direction
					direction = (Vector2)myTransform.position - waypoints [currentWaypointLeft];
					direction.Normalize ();
					distance = Vector2.Distance((Vector2)myTransform.position, waypoints [currentWaypointLeft]);
				}
			}
			// Speed up right
			else if (currentMoveDirection == 1) 
			{
				if (currentSpeed > moveSpeed) 
				{
					// Deccelerate going too fast
					currentSpeed -= TimeManager.FrameTime * deceleration;
					if (currentSpeed < moveSpeed) currentSpeed = moveSpeed;
				}
				else
				{
					// Accelerate
					currentSpeed += TimeManager.FrameTime * acceleration;
					// Don't accelerate faster than move speed
					if (currentSpeed > moveSpeed) currentSpeed = moveSpeed;
				}
				// Update distance and direction
				direction = waypoints [currentWaypointRight] - (Vector2)myTransform.position;
				direction.Normalize ();
				distance = Vector2.Distance((Vector2)myTransform.position, waypoints [currentWaypointRight]);
				if (hasReachedEnd != 1) hasReachedEnd = 0;
			}
			// Speed up left
			else if (currentMoveDirection == -1) 
			{
				if (currentSpeed < -moveSpeed) 
				{
					// Deccelerate going too fast
					currentSpeed += TimeManager.FrameTime * deceleration;
					if (currentSpeed > moveSpeed) currentSpeed = -moveSpeed;
				}
				else
				{
					// Accelerate
					currentSpeed -= TimeManager.FrameTime * acceleration;
					// Don't accelerate faster than move speed
					if (currentSpeed < -moveSpeed) currentSpeed = -moveSpeed;
				}
				// Update distance and direction
				direction = (Vector2)myTransform.position - waypoints [currentWaypointLeft];
				direction.Normalize ();
				distance = Vector2.Distance((Vector2)myTransform.position, waypoints [currentWaypointLeft]);
				if (hasReachedEnd != -1) hasReachedEnd = 0;
			}
			// Slope acceleration
			if (slopeAccelerationBoost != 0)
			{
				Vector2 diference =  waypoints [currentWaypointRight] - waypoints [currentWaypointLeft];
				float sign = (waypoints [currentWaypointRight].y < waypoints [currentWaypointLeft].y)? -1.0f : 1.0f;
				float angle = Vector2.Angle(Vector2.right, diference) * sign;
				if (currentSpeed > 0)
				{
					currentSpeed -= Mathf.Sin(angle * Mathf.Deg2Rad) * TimeManager.FrameTime * slopeAccelerationBoost;
					if (currentSpeed < 0) currentSpeed = 0;
				}
				else if (currentSpeed < 0)
				{
					currentSpeed -= Mathf.Sin(angle * Mathf.Deg2Rad) * TimeManager.FrameTime * slopeAccelerationBoost;
					if (currentSpeed > 0) currentSpeed = 0;
				}
			}

			// Get distance to move, limit it to a max of distance, then move
			float distanceToMove = currentSpeed * TimeManager.FrameTime;
			if (currentSpeed > 0 && distanceToMove > distance)
			{
				remainingDistance = distanceToMove - distance;
				distanceToMove = distance;
			}
			if (currentSpeed < 0 && Mathf.Abs (distanceToMove) > distance)
			{
				remainingDistance = distanceToMove + distance;
				distanceToMove = -distance;
			}
			// The ends of the line are hard limits
			if (currentSpeed < 0 && hasReachedEnd == -1) currentSpeed = 0;
			if (currentSpeed > 0 && hasReachedEnd == 1) currentSpeed = 0;

			myTransform.Translate (direction * distanceToMove);


			// Check for next waypoint
			if (Mathf.Abs(distance) <= MaxDistanceForNextWayPoint)
			{
				if (currentSpeed > 0)
				{
					if (currentWaypointRight == waypoints.Count - 1) myTransform.position = new Vector3(waypoints [currentWaypointRight].x, waypoints [currentWaypointRight].y, myTransform.position.z);
					NextWaypoint(1);
				}
				else if (currentSpeed < 0)
				{
					if (currentWaypointLeft == 0) myTransform.position = new Vector3(waypoints [currentWaypointLeft].x, waypoints [currentWaypointLeft].y, myTransform.position.z);
					NextWaypoint(-1);
				}

				// Do any remaining movement
				if (Mathf.Abs (remainingDistance) > 0 && hasReachedEnd == 0)
				{
					if (remainingDistance > 0)
					{
						direction = waypoints [currentWaypointRight] - (Vector2)myTransform.position;
						direction.Normalize ();
					}
					else
					{
						direction = (Vector2)myTransform.position - waypoints[currentWaypointLeft];
						direction.Normalize ();
					}
					myTransform.Translate (direction * remainingDistance);
				}
			}
		}

		/// <summary>
		/// Update left and right waypoints as well as the has reached end variable.
		/// </summary>
		/// <param name="direction">Direction.</param>
		protected virtual void NextWaypoint(int direction)
		{
			if (direction == 1)
			{
				if (currentWaypointRight == (waypoints.Count - 1))
				{
					hasReachedEnd = 1;
					currentSpeed = 0;
					if (moveType == RailMoveType.AUTOMATIC) 
					{
						currentAutoDirection = -1;
					}
					if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_EXTENT)
					{
						Activated = false;
						OnPlatformDeactivated(character);
					}
				} 
				else
				{
					currentWaypointRight++;
					currentWaypointLeft++;
				}
			} 
			else if (direction == -1)
			{
				if (currentWaypointLeft == 0)
				{
					hasReachedEnd = -1;
					currentSpeed = 0;
					if (moveType == RailMoveType.AUTOMATIC) 
					{
						currentAutoDirection = 1;
					}
					if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_EXTENT)
					{
						Activated = false;
						OnPlatformDeactivated(character);
					}
				} 
				else
				{
					currentWaypointRight--;
					currentWaypointLeft--;
				}
			}
		}
		/// <summary>
		/// Checks the rail moement to see if we should activate.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="railMovement">Rail movement.</param>
		virtual public bool CheckForActivate(Character character, SpecialMovement_Rail railMovement)
		{
			if (railMovement.ShouldBeAttachedToRail(this))
			{
				this.character = character;
				this.railMovement = railMovement;
				if (moveType == RailMoveType.AUTOMATIC && currentAutoDirection == 0) currentAutoDirection = 1;
				return true;
			}
			else
			{
				railMovement = null;
				this.character = null;
			}
			return false;
		}

		/// <summary>
		/// Called when the character is unparented from this platform.
		/// </summary>
		override public void UnParent(IMob character)
		{
			base.UnParent (character);
			// Force off of rail
			if (character is Character && this.character == (Character)character)
			{
				this.character = null;
				railMovement = null;
			}
		}
	}

	public enum RailMoveType
	{
		AUTOMATIC,
		LEFT_RIGHT,
		UP_DOWN,
		FOLLOW_CURVE
	}
}