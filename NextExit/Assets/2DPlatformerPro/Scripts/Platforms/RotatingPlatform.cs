using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that rotates.
	/// </summary>
	public class RotatingPlatform : Platform
	{
		/// <summary>
		/// The speed of rotation.
		/// </summary>
		public float rotationSpeed;

		/// <summary>
		/// Should we parent when the head collides with this platform (used when you have hang from ceiling).
		/// </summary>
		public bool parentOnHeadCollission;
		
		//// <summary>
		/// Cached reference to the transform.
		/// </summary>
		protected Transform myTransform;

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
				Debug.LogError("Rotating platforms should have a scale of (1,1,1). " +
				               "If you wish to make them larger change the size of the collider and make the visual component a child of the platform.");
			}
			myTransform = transform;
		}
		
		/// <summary>
		/// Do the move.
		/// </summary>
		protected virtual void DoMove()
		{
			if (Activated)
			{
				transform.RotateAround (transform.position, new Vector3 (0, 0, 1), rotationSpeed * TimeManager.FrameTime);
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
		/// <summary>
		/// Does this platform want to prevent the given movement from moving. Generally implementations
		/// will use the movement.GetType() to restrict specific classes of movement. Only applied
		/// when character is parented to the platform.
		/// </summary>
		/// <returns><c>true</c>, if movement should be skipped, <c>false</c> otherwise.</returns>
		/// <param name="character">Character being checked.</param>
		/// <param name="movement">Movement being checked.</param>
		override public bool SkipMovement(Character character, Movement movement)
		{
			// Skip wall movements on rotating platforms
			if (movement is WallMovement) return true;
			return false;
		}

	}
	
}