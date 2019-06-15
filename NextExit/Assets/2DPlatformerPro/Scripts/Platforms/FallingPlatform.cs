using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that falls off the screen after a given number of seconds.
	/// </summary>
	public class FallingPlatform : Platform
	{
		/// <summary>
		/// The time in seconds before the platform starts to fall.
		/// </summary>
		public float fallDelay;

		/// <summary>
		/// If true the platform turns in to a rigidbody and falls using unity physics.
		/// </summary>
		public bool fallAsRigidbody;

		//// <summary>
		/// Cached reference to the transform.
		/// </summary>
		protected Transform myTransform;

		/// <summary>
		/// Have we started the count down>
		/// </summary>
		protected bool timerHasStarted;

		/// <summary>
		/// Have we started falling?
		/// </summary>
		protected float fallHasStarted;

		
		/// <summary>
		/// Reference to the rigidbody 2D.
		/// </summary>
		new protected Rigidbody2D rigidbody2D;

		/// <summary>
		/// Unit update hook.
		/// </summary>
		void Update()
		{

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
			rigidbody2D = GetComponent<Rigidbody2D> ();
			if (fallAsRigidbody && rigidbody2D == null) 
			{
				Debug.LogError("A FallingPlatform that falls as a rigidbody must have rigidbody attached!");
			}
			myTransform = transform;
		}

		/// <summary>
		/// If the collission is a foot try to parent.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="type">Type of raycast.</param>
		/// <returns>true if character should be parented to this platform, otherwise false.</returns>
		override protected bool CustomCollide(PlatformCollisionArgs args)
		{
			if (args.RaycastCollider.RaycastType == RaycastType.FOOT)
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
			// Ignore activation, this is always activated.
			if (!timerHasStarted) StartCoroutine(StartFallTimer());
		}

		/// <summary>
		/// Starts the fall timer, then once it expires activates the fall.
		/// </summary>
		/// <returns>The fall timer.</returns>
		protected IEnumerator StartFallTimer() 
		{
			timerHasStarted = true;
			yield return new WaitForSeconds(fallDelay);
			if (fallAsRigidbody) 
			{
				rigidbody2D.isKinematic = false;
				rigidbody2D.gravityScale = 1.0f;
			}
			else
			{

			}
		}
	}

}