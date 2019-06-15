using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy which can rotate.
	/// </summary>
	public class RotatingEnemy : Enemy 
	{
		[Header ("Rotating settings")]

		/// <summary>
		/// Characters rotation speed.
		/// </summary>
		public float rotationSpeed = 360.0f;

		/// <summary>
		/// The max slope rotation in degrees.
		/// </summary>
		public float maxSlopeRotation = 180.0f;
		
		[Header ("Other settings")]

		/// <summary>
		/// Minimum angle that will be considered a wall.
		/// </summary>
		public float minSideAngle = 60.0f;


		/// <summary>
		/// The middle foot collider.
		/// </summary>
		protected NoAllocationSmartFeetcast middleFoot;


		/// <summary>
		/// Current slope angle.
		/// </summary>
		protected float slope;

		/// <summary>
		/// The left foot collision.
		/// </summary>
		protected RaycastHit2D leftFootCollision;

		/// <summary>
		/// The right foot collision.
		/// </summary>
		protected RaycastHit2D rightFootCollision;

		protected RaycastHit2D middleFootCollision;

		protected int rotationPoint;

		protected Collider2D currentGround;

		/// <summary>
		/// If slopes are on this is the rotation we are currently on.
		/// </summary>
		override public float SlopeActualRotation
		{
			get
			{
				float result = myTransform.rotation.eulerAngles.z;
				if (result > 180.0f) result -= 360;
				return result;
			}
		}

		
		/// <summary>
		/// Gets the minimum angle at which geometry is considered a wall.
		/// </summary>
		override public float MinSideAngle
		{
			get
			{
				return minSideAngle;
			}
		}

		/// <summary>
		/// Set up the character
		/// </summary>
		override protected void PostInit()
		{
			base.PostInit ();

			// An an extra foot to smooth slopes
			middleFoot = new NoAllocationSmartFeetcast ();
			middleFoot.Mob = this;
			middleFoot.RaycastType = RaycastType.FOOT;
			middleFoot.Extent = new Vector3(0, feetHeight);
			middleFoot.Transform = transform;
			middleFoot.LayerMask = geometryLayerMask;
			middleFoot.LookAhead = groundedLookAhead;
		}

		/// <summary>
		/// Run each frame to determine and execute move.
		/// </summary>
		override protected void DoUpdate()
		{
			DoRotation ();
			base.DoUpdate ();
			CalculateTargetRotation ();
		}

		
		/// <summary>
		/// Applies the feet collisions.
		/// </summary>
		override public void ApplyFeetCollisions()
		{
			float deepestFootPenetration = 0.0f;
			float smallestFraction = float.MaxValue;
			int closest = -1;
			float distanceToGround = float.MaxValue;
			int feetCount = 0;
			float leftFootSlope = 0;
			float rightFootSlope = 0;
			float middleFootSlope = 0;
			float deg;
			bool hasParented = false;

			grounded = false;
			currentGround = null;
			rightFootCollision = NoAllocationRaycast.EmptyRaycastHit;
			leftFootCollision = NoAllocationRaycast.EmptyRaycastHit;

			// Get collisions
			RaycastHit2D[] leftFeetCollisions = leftFoot.GetRaycastHits();
			RaycastHit2D[] rightFeetCollisions = rightFoot.GetRaycastHits();
			RaycastHit2D[] middleFeetCollisions = middleFoot.GetRaycastHits();
			// Left foot
			for (int i = 0; i < leftFeetCollisions.Length; i++)
			{	
				// Make sure we have a collision to support raycast colliders which use statically allocated hits arrays 
				if (leftFeetCollisions[i].collider != null)
				{
					if (leftFeetCollisions[i].fraction > 0 && leftFeetCollisions[i].fraction < smallestFraction)
					{
						smallestFraction = leftFeetCollisions[i].fraction;
						closest = i;
						leftFootCollision = leftFeetCollisions[i];
					}
				} 
			}
			// Found a collision
			if (closest != -1)
			{
				// float penetration = leftFoot.Length - (leftFeetCollisions[closest].fraction * (leftFoot.Length + leftFoot.LookAhead));
				float penetration = (leftFeetCollisions[closest].fraction * (leftFoot.Length + leftFoot.LookAhead)) - leftFoot.Length;
				if (penetration < deepestFootPenetration)
				{
					deepestFootPenetration = penetration;
				}
				// Check for platforms
				// The memory allocation only occurs in the editor, its not the GC trap that is seems
				if (enemyInteractsWithPlatforms)
				{
					Platform platform = leftFeetCollisions[closest].collider.GetComponent<Platform>();
					if (platform != null) 
					{
						platformCollisionArgs.RaycastCollider = leftFoot;
						platformCollisionArgs.Penetration = penetration;
						bool parent = platform.Collide(platformCollisionArgs);
						if (parent && !hasParented) 
						{
							if (parentPlatform != null)
							{
								parentPlatform.UnParent(this);
							}
							parentPlatform = platform;
							transform.parent = platform.transform;
							platform.Parent(this);
							hasParented = true;
						}
					}
				}
				if (penetration <= groundedLookAhead)
				{
					// Check for grounded
					grounded = true;
					currentGround = leftFeetCollisions[closest].collider;
					if (penetration < distanceToGround) distanceToGround = penetration;
					feetCount++;
					deg = Mathf.Rad2Deg * Mathf.Atan2(leftFeetCollisions[closest].normal.x, leftFeetCollisions[closest].normal.y);
					if (deg <= MAX_ENEMY_SLOPE && deg >= -MAX_ENEMY_SLOPE) 
					{
						leftFootSlope = deg;
					}
				}
			}
			
			// Right foot
			closest = -1;
			smallestFraction = float.MaxValue;
			for (int i = 0; i < rightFeetCollisions.Length; i++)
			{	
				// Make sure we have a collision to support raycast colliders which use statically allocated hits arrays 
				if (rightFeetCollisions[i].collider != null)
				{
					if (rightFeetCollisions[i].fraction > 0 && rightFeetCollisions[i].fraction < smallestFraction)
					{
						smallestFraction = rightFeetCollisions[i].fraction;
						closest = i;
						rightFootCollision = rightFeetCollisions[i];
					}
				}
			}
			// Found a collision
			if (closest != -1)
			{				
				// float penetration = rightFoot.Length - (rightFeetCollisions[closest].fraction * (rightFoot.Length + rightFoot.LookAhead));
				float penetration = (rightFeetCollisions[closest].fraction * (rightFoot.Length + rightFoot.LookAhead)) - rightFoot.Length;
				if (penetration < deepestFootPenetration)
				{
					deepestFootPenetration = penetration;
				}
				// Check for platforms
				// The memory allocation only occurs in the editor, its not the GC trap that is seems
				if (enemyInteractsWithPlatforms)
				{
					Platform platform = rightFeetCollisions[closest].collider.GetComponent<Platform>();
					if (platform != null) 
					{
						platformCollisionArgs.RaycastCollider = rightFoot;
						platformCollisionArgs.Penetration = penetration;
						bool parent = platform.Collide(platformCollisionArgs);
						if (parent && !hasParented) 
						{
							if (parentPlatform == platform)
							{
								hasParented = true;
							}
							else 
							{
								if (parentPlatform != null)
								{
									parentPlatform.UnParent(this);
								}
								parentPlatform = platform;
								transform.parent = platform.transform;
								platform.Parent(this);
								hasParented = true;
							}
						}
					}
				}
				if (penetration <= groundedLookAhead)
				{
					// Check for grounded
					if (grounded)
					{
						// If already ground Prefer collider we are moving on to
						if (rightFeetCollisions[closest].collider != currentGround && LastFacedDirection == 1)
						{
							currentGround = rightFeetCollisions[closest].collider;
						}
					}
					grounded = true;
					currentGround = rightFeetCollisions[closest].collider;
					if (penetration < distanceToGround) distanceToGround = penetration;
					feetCount++;
					deg = Mathf.Rad2Deg * Mathf.Atan2(rightFeetCollisions[closest].normal.x, rightFeetCollisions[closest].normal.y);
					if (deg <= MAX_ENEMY_SLOPE && deg >= -MAX_ENEMY_SLOPE) 
					{
						rightFootSlope = deg;
					}
				}
			}

			// middle foot
			for (int i = 0; i < middleFeetCollisions.Length; i++)
			{	
				// Make sure we have a collision to support raycast colliders which use statically allocated hits arrays 
				if (middleFeetCollisions[i].collider != null)
				{
					if (middleFeetCollisions[i].fraction > 0 && middleFeetCollisions[i].fraction < smallestFraction)
					{
						smallestFraction = middleFeetCollisions[i].fraction;
						closest = i;
						middleFootCollision = middleFeetCollisions[i];
					}
				} 
			}
			// Found a collision
			if (closest != -1)
			{
				// float penetration = middleFoot.Length - (middleFeetCollisions[closest].fraction * (middleFoot.Length + middleFoot.LookAhead));
				float penetration = (middleFeetCollisions[closest].fraction * (middleFoot.Length + middleFoot.LookAhead)) - middleFoot.Length;
				if (penetration < deepestFootPenetration)
				{
					deepestFootPenetration = penetration;
				}
				// Check for platforms
				// The memory allocation only occurs in the editor, its not the GC trap that is seems
				if (enemyInteractsWithPlatforms)
				{
					Platform platform = middleFeetCollisions[closest].collider.GetComponent<Platform>();
					if (platform != null) 
					{
						platformCollisionArgs.RaycastCollider = middleFoot;
						platformCollisionArgs.Penetration = penetration;
						bool parent = platform.Collide(platformCollisionArgs);
						if (parent && !hasParented) 
						{
							if (parentPlatform != null)
							{
								parentPlatform.UnParent(this);
							}
							parentPlatform = platform;
							transform.parent = platform.transform;
							platform.Parent(this);
							hasParented = true;
						}
					}
				}
				if (penetration <= groundedLookAhead)
				{
					// Check for grounded
					grounded = true;
					currentGround = middleFeetCollisions[closest].collider;
					if (penetration < distanceToGround) distanceToGround = penetration;
					feetCount++;
					deg = Mathf.Rad2Deg * Mathf.Atan2(middleFeetCollisions[closest].normal.x, middleFeetCollisions[closest].normal.y);
					if (deg <= MAX_ENEMY_SLOPE && deg >= -MAX_ENEMY_SLOPE) 
					{
						middleFootSlope = deg;
					}
				}
			}

			// On the ground so set y velocity to 0
			if (grounded && Velocity.y <= 0 && distanceToGround < 0) SetVelocityY(0);
			
			// Translate above ground
			if (deepestFootPenetration < 0.0f)
			{
				Translate(0, -deepestFootPenetration, false);
			}
			// Most ground movements will want to keep us aligned with ground, so do that.
			else if (grounded && movement.ShouldSnapToGround && State != EnemyState.DEAD && State != EnemyState.DAMAGED) 
			{
				Translate(0, -distanceToGround, false);
			}
			// Set slope rotation
			if (feetCount > 0) SlopeTargetRotation = (rightFootSlope + leftFootSlope + middleFootSlope) / feetCount;
			
			// Unparent if we aren't on platform any longer
			if (enemyInteractsWithPlatforms && !hasParented)
			{
				transform.parent = null;
				if (parentPlatform !=null) 
				{
					parentPlatform.UnParent(this);
					parentPlatform = null;
				}
			}
		}

		/// <summary>
		/// Use the feet colliders to determine the target rotation.
		/// </summary>
		virtual protected void CalculateTargetRotation()
		{
			float rightFootWeight = 0.0f;
			float leftFootWeight = 0.0f;
			float rightFootSlope = 0.0f;
			float leftFootSlope = 0.0f;
			
			// TODO Check that we have applied feet 

			// Left Foot hit something - assumes left foot is the first collider
			if (leftFootCollision.collider != null)
			{
				float deg = Mathf.Rad2Deg * Mathf.Atan2(leftFootCollision.normal.x, leftFootCollision.normal.y);
				if (deg <= maxSlopeRotation && deg >= -maxSlopeRotation) 
				{
					leftFootSlope = deg;
					leftFootWeight = 1.0f;
				}
			}
			
			// Right Foot hit something - assumes right foot is the collider at feetColliders.Length - 1
			if (rightFootCollision.collider != null)
			{
				// Right layer
			
				float deg = Mathf.Rad2Deg * Mathf.Atan2(rightFootCollision.normal.x, rightFootCollision.normal.y);
				if (deg <= maxSlopeRotation && deg >= -maxSlopeRotation) 
				{
					rightFootSlope = deg;
					rightFootWeight = 1.0f;
				}
			}
			
			// If only one foot hit, just use that one
			if (rightFootWeight == 0.0f && leftFootWeight > 0.0f)
			{
				slope = leftFootSlope;
				rotationPoint = 0;
			}
			else if (leftFootWeight == 0.0f && rightFootWeight > 0.0f)
			{
				slope = rightFootSlope;
				rotationPoint = 1;
			}
			// Else if both hit
			else if (leftFootWeight > 0.0f && rightFootWeight > 0.0f)
			{
				// Early out if they are the same
				if (Mathf.Approximately(leftFootSlope, rightFootSlope))
				{
					slope = leftFootSlope;
					rotationPoint = -1;
				}
				// Else cycle through other feet and get the average
				else
				{
					slope = (leftFootSlope + rightFootSlope) / 2.0f;
					rotationPoint = -1;
				}
			}
			// Else if none hit
			else
			{
				slope = 0.0f;
			}
		}


		/// <summary>
		/// Rotate towards the target rotation.
		/// </summary>
		virtual protected void DoRotation()
		{
			float difference  = -slope - myTransform.eulerAngles.z;
			// Shouldn't really happen but just in case
			if (difference > 180) difference = difference - 360;
			if (difference < -180) difference = difference + 360;
			Vector3 rotateAround = transform.position;
			if (difference > rotationSpeed * TimeManager.FrameTime) difference = rotationSpeed * TimeManager.FrameTime;
			if (difference < -rotationSpeed * TimeManager.FrameTime) difference = -rotationSpeed * TimeManager.FrameTime;
			myTransform.RotateAround(rotateAround, new Vector3(0,0,1), difference);
		}


	}
}