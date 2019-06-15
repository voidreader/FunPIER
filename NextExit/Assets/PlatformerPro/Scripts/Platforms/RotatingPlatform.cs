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
	public class RotatingPlatform : ParentOnStandPlatform
	{
		/// <summary>
		/// The speed of rotation.
		/// </summary>
		public float rotationSpeed;

		
		//// <summary>
		/// Cached reference to the transform.
		/// </summary>
		protected Transform myTransform;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			base.PostInit();
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
		override protected void DoMove()
		{
			if (Activated)
			{
				transform.RotateAround (transform.position, new Vector3 (0, 0, 1), rotationSpeed * TimeManager.FrameTime);
			}
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