using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform defining a set of steps.
	/// </summary>
	[RequireComponent (typeof(EdgeCollider2D))]
	public class Stairs : Platform
	{
		/// <summary>
		/// The number of steps that are on these stairs.
		/// </summary>
		[Tooltip ("The number of steps that are on these stairs.")]
		[SerializeField]
		[Range (3,99)]
		protected int stepCount = 5;

		/// <summary>
		/// Ignore the charater unless they are this deep in to the ocllider (stops character climbing the starts of the stairs as if its a slope).
		/// </summary>
		[Tooltip ("Ignore the character unless they are this deep in to the ocllider (stops character climbing the starts of the stairs as if its a slope).")]
		[SerializeField]
		protected float ignoreColliderDistance = 0.25f;

		
		///<summary>
		/// How we determine if we should start descending the stairs.
		/// </summary>
		[Tooltip ("How we determine if we should start descending the stairs.")]
		[SerializeField]
		protected StairCollisionType topMountType;

		///<summary>
		/// How we determine if we should start ascending the stairs.
		/// </summary>
		[Tooltip ("How we determine if we should start ascending the stairs.")]
		[SerializeField]
		protected StairCollisionType bottomMountType;

		///<summary>
		/// How we determine if we should mount stairs when in the air.
		/// </summary>
		[Tooltip ("How we determine if we should mount stairs when in the air and we fall on to the stairs.")]
		[SerializeField]
		protected StairAirType airMountType;

		/// <summary>
		/// The calculated size of each step 
		/// </summary>
		protected Vector2 stepSize;

		/// <summary>
		/// The step direction. We use a float becasue we are frequently using this in
		/// multiplication and divison with other floats.
		/// </summary>
		protected float stepDirection;

		/// <summary>
		/// The leftmost stair point.
		/// </summary>
		protected Vector2 leftPoint;

		/// <summary>
		/// The right most stair point.
		/// </summary>
		protected Vector2 rightPoint;

		/// <summary>
		/// The size of a step.
		/// </summary>
		public Vector2 StepSize
		{
			get { return stepSize; }
		}

		/// <summary>
		/// Direction of the stair case, 1 means ascending to the right, -1 means ascending to the left.
		/// </summary>
		public float StepDirection
		{
			get { return stepDirection; }
		}

		/// <summary>
		/// Called to determine if collision should be ignored. 
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character.</param>
		/// <param name="collider">Collider.</param>
		override public bool IgnoreCollision(Character character, BasicRaycast collider)
		{
			if (base.IgnoreCollision(character, collider)) return true;
			if (collider.RaycastType == RaycastType.FOOT && 
				Mathf.Abs (collider.WorldExtent.x - character.transform.position.x) < ignoreColliderDistance
				&& ShouldStartClimb(character, collider) && ShouldStartDescent(character, collider))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Gets the last step/total number of steps.
		/// </summary>
		/// <value>The last step.</value>
		public int LastStep
		{
			get { return stepCount; }
		}

		/// <summary>
		/// Unity start hook. Init this instance.
		/// </summary>
		void Start()
		{
			PostInit ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			EdgeCollider2D edge = GetComponent<EdgeCollider2D> ();
			if (edge.pointCount != 2)
			{
				Debug.LogWarning ("Stairs are expected to consist of an edge collider with two points");
			}
			else
			{
				// Make sure left most point is in position 0
				if (transform.TransformPoint (edge.points [0]).x > transform.TransformPoint (edge.points [1]).x)
				{
					leftPoint = transform.TransformPoint (edge.points [1]);
					rightPoint = transform.TransformPoint (edge.points [0]);
				}
				else
				{
					leftPoint = transform.TransformPoint (edge.points [0]);
					rightPoint = transform.TransformPoint (edge.points [1]);
				}
				if (leftPoint.y > rightPoint.y)
				{
					stepDirection = -1f;
					stepSize = new Vector2((rightPoint.x - leftPoint.x) / (float) stepCount, 
										   (leftPoint.y - rightPoint.y) / (float)stepCount); 
				}
				else if (leftPoint.y < rightPoint.y) 
				{
					stepDirection = 1f;
					stepSize = new Vector2((rightPoint.x - leftPoint.x) / (float) stepCount, 
										   (rightPoint.y - leftPoint.y) / (float)stepCount); 
				}
				else
				{
					Debug.LogWarning ("The stairs collider doesn't look to define a valid set of stiars, both points have the same y value");
				}
			}

			base.PostInit ();
		}

		/// <summary>
		/// Gets the step number for a given world position.
		/// </summary>
		public int GetStepForPosition(Vector2 position)
		{
			int step = 1;
			float xPosition = leftPoint.x + stepSize.x;

			// Too far to left
			if (position.x < leftPoint.x) return -1;
			// Or right
			if (position.x > rightPoint.x) return -1;

			while (xPosition < position.x)
			{
				xPosition += stepSize.x;
				step++;
			}
			return step;
		}

		/// <summary>
		/// Gets the position for the middle top of the given step. Doesn't check step bounds.
		/// </summary>
		/// <returns>The position of the step step.</returns>
		/// <param name="step">Step.</param>
		public Vector2 GetPositionForStep(int step) 
		{
			Vector2 stepPoint = leftPoint;
			stepPoint += ((float)step) * new Vector2(stepSize.x, stepSize.y * stepDirection);
			stepPoint.x -= stepSize.x / 2.0f;
			if (stepDirection < 0) stepPoint.y += stepSize.y;
			return stepPoint;
		}

		/// <summary>
		/// Should we start descending stairs.
		/// </summary>
		/// <returns><c>true</c>, if we should start descending, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="collider">Collider.</param>
		protected bool ShouldStartDescent(Character character, BasicRaycast collider) {
			
			// Already on stairs
			if (character.ActiveMovement is GroundMovement_Stairs) return true;

			// Check air mount
			if (character.ActiveMovement is AirMovement) return ShouldAirMount (character, collider);

			// Always type
			if (topMountType == StairCollisionType.ALWAYS) return true;
			
			int step = GetStepForPosition (character.transform.position);
			
			// At bottom of stairs, always true
			if (step == LastStep && stepDirection < 0) return true;
			if (step == 1 && stepDirection > 0) return true;
			
			// Hold up means dont climb down steps
			if (character.Input.VerticalAxisDigital == 1) return false;

			if (step == 1 && stepDirection < 0)
			{
				if (character.FacingDirection != 1) return false;
				// Conditions met
				if (topMountType == StairCollisionType.WALK_DIRECTION || character.Input.VerticalAxisDigital == -1 ) return true;
			}
			else if (step == LastStep && stepDirection > 0)
			{
				// Facing direction 
				if (character.FacingDirection != -1) return false;
				// Conditions met
				if (topMountType == StairCollisionType.WALK_DIRECTION || character.Input.VerticalAxisDigital == -1 ) return true;
			}
			return false;
		}

		/// <summary>
		/// Should we start climbing stairs.
		/// </summary>
		/// <returns><c>true</c>, if we should start climbing, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="collider">Collider.</param>
		protected bool ShouldStartClimb(Character character, BasicRaycast collider) {
			
			// Already on stairs
			if (character.ActiveMovement is GroundMovement_Stairs) return true;

			// Check air mount
			if (character.ActiveMovement is AirMovement) return ShouldAirMount (character, collider);

			// Always type
			if (bottomMountType == StairCollisionType.ALWAYS) return true;

			int step = GetStepForPosition (character.transform.position);

			// At top of stairs, always true
			if (step == LastStep && stepDirection > 0) return true;
			if (step == 1 && stepDirection < 0) return true;

			// Hold down means dont climb up steps
			if (character.Input.VerticalAxisDigital == -1) return false;

			if (step == 1 && stepDirection > 0)
			{
				// Facing direction 
				if (character.FacingDirection != 1) return false;
				// Conditions met
				if (bottomMountType == StairCollisionType.WALK_DIRECTION || character.Input.VerticalAxisDigital == 1 ) return true;
			}
			else if (step == LastStep && stepDirection < 0)
			{
				// Facing direction 
				if (character.FacingDirection != -1) return false;
				// Conditions met
				if (bottomMountType == StairCollisionType.WALK_DIRECTION || character.Input.VerticalAxisDigital == 1 ) return true;
			}
			return false;
		}

		/// <summary>
		/// Determines if we should mount the stairs when we fall on them.
		/// </summary>
		/// <returns><c>true</c>, if we should mount stairs.
		/// <param name="character">Character.</param>
		/// <param name="collider">Collider.</param>
		protected bool ShouldAirMount(Character character, BasicRaycast collider) {

			// Rising up don't meount
			if (character.Velocity.y >= 0) return false;

			// Falling down - Check conditions
			switch (airMountType)
			{
			case StairAirType.ALWAYS:
				return true;
			case StairAirType.MUST_HOLD_UP:
				if (character.Input.VerticalAxisDigital == 1) return true;
				break;
			case StairAirType.MUST_HOLD_UP_OR_DOWN:
				if (character.Input.VerticalAxisDigital != 0) return true;
				break;
			case StairAirType.MUST_NOT_HOLD_DOWN:
				if (character.Input.VerticalAxisDigital != -1) return true;
				break;
			}
			return false;
		}

		/// <summary>
		/// Draw handles for showing extents/stairs.
		/// </summary>
		void OnDrawGizmos() {
			PostInit ();
			if (stepSize.x > 0 && stepSize.y > 0)
			{
				Gizmos.color = Color.green;
				Vector2 currentPos = leftPoint;
				if (stepDirection > 0) 
				{
					for (int i = 0; i < stepCount; i++)
					{
						Gizmos.DrawLine (currentPos, currentPos + new Vector2 (0, stepSize.y * stepDirection));
						currentPos += new Vector2 (0, stepSize.y * stepDirection);
						Gizmos.DrawLine (currentPos, currentPos + new Vector2 (stepSize.x, 0));
						currentPos += new Vector2 (stepSize.x, 0);
					}
				}
				else 
				{
					for (int i = 0; i < stepCount; i++)
					{
						Gizmos.DrawLine (currentPos, currentPos + new Vector2 (stepSize.x, 0));
						currentPos += new Vector2 (stepSize.x, 0);
						Gizmos.DrawLine (currentPos, currentPos + new Vector2 (0, stepSize.y * stepDirection));
						currentPos += new Vector2 (0, stepSize.y * stepDirection);
					}
				}
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Gets the top mount description.
		/// </summary>
		/// <returns>The top mount description.</returns>
		public string GetTopMountDescription()
		{
			return topMountType.GetDescription ();
		}

		/// <summary>
		/// Gets the bottom mount description.
		/// </summary>
		/// <returns>The bottom mount description.</returns>
		public string GetBottomMountDescription()
		{
			return bottomMountType.GetDescription ();
		}

		/// <summary>
		/// Gets the air mount description.
		/// </summary>
		/// <returns>The air mount description.</returns>
		public string GetAirMountDescription()
		{
			return airMountType.GetDescription ();
		}

#endif

	}

	/// <summary>
	/// Methods for determining if we mount the stairs.
	/// </summary>
	public enum StairCollisionType 
	{
		ALWAYS,
		WALK_DIRECTION,
		WALK_DIRECTION_PLUS_UP_DOWN
	}

	/// <summary>
	/// Extension class for stair collission type, provides description.
	/// </summary>
	public static class StairCollisionTypeExtensions
	{
		public static string GetDescription(this StairCollisionType me)
		{
			switch(me)
			{
			case StairCollisionType.ALWAYS: return "Treat this stair case likes its solid. Character will always start stair walking if they collide with the edge collider.";
			case StairCollisionType.WALK_DIRECTION: return "The character will only mount the stair case if they walk in the direction of the stairs.";
			case StairCollisionType.WALK_DIRECTION_PLUS_UP_DOWN: return "The character will only mount the stair case if the player holds UP/DOWN while they walk in the direction of the stairs.";
			}
			return "No information available.";
		}

	}

	/// <summary>
	/// Methods for determining if we mount the stairs while airborne.
	/// </summary>
	public enum StairAirType 
	{
		ALWAYS,
		MUST_HOLD_UP,
		MUST_HOLD_UP_OR_DOWN,
		MUST_NOT_HOLD_DOWN
	}

	/// <summary>
	/// Extension class for stair collission type, provides description.
	/// </summary>
	public static class StairAirTypeExtensions
	{
		public static string GetDescription(this StairAirType me)
		{
			switch(me)
			{
			case StairAirType.ALWAYS: return "Always latch to the stairs when you fall on to them.";
			case StairAirType.MUST_HOLD_UP: return "Only latch to the stairs when you fall on to them while holding up.";
			case StairAirType.MUST_HOLD_UP_OR_DOWN: return "Only latch to the stairs when you fall on to them while holding up or down.";
			case StairAirType.MUST_NOT_HOLD_DOWN: return "Latch to the stairs when you fall on to them unless you are holding down.";
			}
			return "No information available.";
		}

	}
}
