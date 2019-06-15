using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that crumbles after a delay.
	/// </summary>
	[RequireComponent (typeof (Collider2D))]
	public class CrumblingPlatform : Platform
	{
		/// <summary>
		/// The time in seconds before the platform starts to fall.
		/// </summary>
		public float crumbleTime;

		//// <summary>
		/// Cached reference to the collider.
		/// </summary>
		protected Collider2D myCollider;

		/// <summary>
		/// Have we started the count down?
		/// </summary>
		protected bool timerHasStarted;

		/// <summary>
		/// have we finished crumbling
		/// </summary>
		protected bool isCrumbled;

		/// <summary>
		/// Unit update hook.
		/// </summary>
		void Update()
		{

		}
		
		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			base.PostInit();
			myCollider = GetComponent<Collider2D> ();
		}

		/// <summary>
		/// If the collission is a foot try to parent.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="type">Type of raycast.</param>
		/// <returns>true if character should be parented to this platform, otherwise false.</returns>
		override protected bool CustomCollide(PlatformCollisionArgs args)
		{
			// Shouldn't happen but just in case
			if (isCrumbled) return false;
			if (args.RaycastCollider.RaycastType == RaycastType.FOOT)
			{
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Called when the character is parented to this platform.
		/// </summary>
		override public void Parent(IMob character)
		{
			// Ignore activation, this is always activated.
			if (!timerHasStarted) StartCoroutine(StartCrumbleTimer());
		}

		/// <summary>
		/// Starts the crumble timer, then once it expires remove the collider.
		/// </summary>
		/// <returns>The fall timer.</returns>
		protected IEnumerator StartCrumbleTimer() 
		{
			timerHasStarted = true;
			OnFired (null);
			yield return new WaitForSeconds(crumbleTime);
			myCollider.enabled = false;
			isCrumbled = true;
		}
	}

}