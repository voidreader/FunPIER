using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A little class to rotate towars the direction of swim.
	/// </summary>
	public class SwimDirectionRotator : MonoBehaviour 
	{

		/// <summary>
		/// Speed we rotate at for air movement.
		/// </summary>
		public float rotationSpeed;

		/// <summary>
		/// If on the surface of water should we still rotate?
		/// </summary>
		public bool rotateOnSurface;

		/// <summary>
		/// Speed we rotate at when resetting back to 0.
		/// </summary>
		[Tooltip ("If movement is not a swim should be snap immedaitely back to 0?")]
		public bool snapImmediately = true;
		
		/// <summary>
		/// point we rotate around.
		/// </summary>
		public Vector3 rotationPoint;
		
		/// <summary>
		/// Cached reference to character.
		/// </summary>
		protected IMob character;
		
		/// <summary>
		/// Desired rotation.
		/// </summary>
		protected float targetRotation;
		
		void Start()
		{
			character = GetComponentInParent<Character> ();
			if (character == null) character = GetComponentInParent<Enemy> ();
			if (character == null) Debug.LogWarning ("Unable to find a character or enemy for velocity driven rotator");
		}
		
		void Update()
		{
			SetTargetRotation ();
			RotateTowardsTarget ();
		}
		
		protected void SetTargetRotation()
		{
			targetRotation = 0.0f;

			if (character is Character && ((Character)character).ActiveMovement is SpecialMovement_DirectionalSwim)
			{
				// Rotate on surface?
				if (!rotateOnSurface && ((SpecialMovement_DirectionalSwim)((Character)character).ActiveMovement).OnSurface) 
				{
					targetRotation = 0;
					return;
				}

				Vector2 swimDirection = ((SpecialMovement_DirectionalSwim)((Character)character).ActiveMovement).SwimFacingDirection;
				float deg = Mathf.Rad2Deg * Mathf.Atan2(swimDirection.x, swimDirection.y);
				if (targetRotation < 0 && deg >= 180.0f) deg = -180.0f;
				targetRotation = deg;
			}
		}
		
		/// <summary>
		/// Rotates towrds target rotation.
		/// </summary>
		protected void RotateTowardsTarget()
		{
			// Grounded instantly set rotation
			if (snapImmediately && !(((Character)character).ActiveMovement is SpecialMovement_DirectionalSwim))  {
				transform.localRotation = Quaternion.identity;
			}
			else
			{
				float speed = rotationSpeed;
				float difference  = -targetRotation - transform.localEulerAngles.z;
				// Shouldn't really happen but just in case
				if (difference > 180) difference = difference - 360;
				if (difference < -180) difference = difference + 360;
				Vector3 rotateAround = transform.position + rotationPoint;
				
				if (targetRotation == 0 && difference > rotationSpeed * TimeManager.FrameTime) difference = rotationSpeed * TimeManager.FrameTime;
				if (targetRotation == 0 && difference < -rotationSpeed * TimeManager.FrameTime) difference = -rotationSpeed * TimeManager.FrameTime;
				if (targetRotation != 0 && difference > speed * TimeManager.FrameTime) difference = rotationSpeed * TimeManager.FrameTime;
				if (targetRotation != 0 && difference < -speed * TimeManager.FrameTime) difference = -rotationSpeed * TimeManager.FrameTime;
				transform.RotateAround(rotateAround, new Vector3(0,0,1), difference);
			}
		}
	}
}
