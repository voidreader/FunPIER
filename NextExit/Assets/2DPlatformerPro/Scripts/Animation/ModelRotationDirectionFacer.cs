using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Changes facing direction by rotating the model.
	/// </summary>
	public class ModelRotationDirectionFacer : MonoBehaviour
	{

		/// <summary>
		/// An extra rotation applied to all rotations (use this to easily account for backed in model rotations).
		/// </summary>
		[Range (-180, 180)]
		public float rotationOffset;

		/// <summary>
		/// If true offset the parent animation so the sprite doesn't rotate in the z direction
		/// </summary>
		public bool dontRotateInZ;

		/// <summary>
		/// List of animation states in which the character should face backwards (0 degrees).
		/// </summary>
		public AnimationState[] faceBackAnimations;

		/// <summary>
		/// List of animation states in which the character should face forwards (180 degrees).
		/// </summary>
		public AnimationState[] faceForwardAnimations;

		/// <summary>
		/// List of animation states in which the character should face right (90 degrees).
		/// </summary>
		public AnimationState[] faceRightAnimations;

		/// <summary>
		/// List of animation states in which the character should face left (-90 degrees).
		/// </summary>
		public AnimationState[] faceLeftAnimations;

		/// <summary>
		/// How fast the character rotates.
		/// </summary>
		public float rotationSpeed;

		/// <summary>
		/// The character reference.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Desired rotation.
		/// </summary>
		protected Quaternion targetRotation;

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			if (GetComponent<Character>() != null) Debug.LogError ("ModelRotationDirectionFacer should not be palced on the character, put it on the root of model (which should be a descendant of the character).");
			character = GetComponentInParent<Character>();
			if (character == null) Debug.LogError ("Direction facer can't find the character reference");
		}

		/// <summary>
		/// Unity update hook, face correct direction.
		/// </summary>
		void Update ()
		{
			SetTargetRotation ();
			RotateToTarget ();
		}

		/// <summary>
		/// Sets the target rotation based on character state and direction.
		/// </summary>
		protected void SetTargetRotation()
		{
			foreach (AnimationState state in faceBackAnimations)
			{
				if (character.AnimationState == state)
				{
					targetRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
					return;
				}
			}
			foreach (AnimationState state in faceForwardAnimations)
			{
				if (character.AnimationState == state)
				{
					targetRotation = Quaternion.Euler (0.0f, 180.0f, 0.0f);
					return;
				}
			}
			foreach (AnimationState state in faceRightAnimations)
			{
				if (character.AnimationState == state)
				{
					targetRotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
					return;
				}
			}
			foreach (AnimationState state in faceLeftAnimations)
			{
				if (character.AnimationState == state)
				{
					targetRotation = Quaternion.Euler (0.0f, -90.0f, 0.0f);
					return;
				}
			}
			if (character.LastFacedDirection == 1)
			{
				targetRotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
			}
			else if (character.LastFacedDirection == -1)
			{
				targetRotation = Quaternion.Euler (0.0f, -90.0f, 0.0f);
			}
			else if (character.FacingDirection == 0)
			{
				targetRotation =  Quaternion.Euler (0.0f, 180.0f, 0.0f);
			}
		}

		/// <summary>
		/// Does the rotation.
		/// </summary>
		protected void RotateToTarget()
		{
			Quaternion actualTarget = targetRotation;

			if (rotationOffset != 0.0f)  actualTarget *= Quaternion.Euler(0.0f, rotationOffset, 0.0f);
			// Quaternion.Inverse(transform.parent.rotation) * 
			// Debug.Log (((Mathf.Abs (transform.localRotation.eulerAngles.z - actualTarget.eulerAngles.z) >= 180) ? Quaternion.Euler (0, 180, 0) : actualTarget).eulerAngles.z);
			if (dontRotateInZ)
			{
				actualTarget = Quaternion.Inverse(transform.parent.rotation) * actualTarget;
			}
			transform.localRotation = Quaternion.RotateTowards (transform.localRotation,
				 (Quaternion.Angle (transform.localRotation, actualTarget) >= 180) ? (Quaternion.Euler(0, 180, 0) * Quaternion.Euler(0.0f, rotationOffset, 0.0f)) : actualTarget, TimeManager.FrameTime * rotationSpeed);
		}

	}

}