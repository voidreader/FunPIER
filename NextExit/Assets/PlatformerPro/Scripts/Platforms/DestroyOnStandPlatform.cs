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
	public class DestroyOnStandPlatform : ParentOnStandPlatform
	{

		/// <summary>
		/// How much health the platform has.
		/// </summary>
		[Range (1,10)]
		[Tooltip ("How much health the platform has.")]
		public int health;

		/// <summary>
		/// The time in seconds before the platform takes damage.
		/// </summary>
		[Tooltip ("The time in seconds before the platform takes damage.")]
		public float damageDelay;

		/// <summary>
		/// If populated this will indicate damage by changing sprites automatically. Leave empty if you don't want to do this. 
		/// The array index corresponds to the sprite. You can leave sprite at position 0 as null if you want platform to disappear.
		/// </summary>
		[Tooltip ("If populated this will indicate damage by changing sprites automatically. Leave empty if you don't want to do this.")]
		public Sprite[] sprites;

		/// <summary>
		/// Controls what happens when the player leaves the platform. Does it reset the timer causing 0 damage, cause 1 damage, or always completely destroy platform.
		/// </summary>
		[Tooltip ("Controls what happens when the player leaves the platform. Does it reset the timer causing 0 damage, cause 1 damage, or always completely destroy platform.")]
		public DamageMode damageCancelMode = DamageMode.INSTANT_CANCEL;

		//// <summary>
		/// Cached reference to the renderer.
		/// </summary>
		protected SpriteRenderer myRenderer;

		/// <summary>
		/// Cached reference to platform collider.
		/// </summary>
		protected Collider2D myCollider;

		/// <summary>
		/// Have we started the count down>
		/// </summary>
		protected bool timerHasStarted;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			base.PostInit();
			if (sprites.Length > 0)
			{
				myRenderer = GetComponentInChildren<SpriteRenderer> ();
				if (sprites.Length != health) Debug.LogWarning ("Sprite list should be the same size as health");
			}
			myCollider = GetComponentInChildren<Collider2D> ();
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
		override public void Parent(IMob character)
		{
			StopAllCoroutines ();
			StartCoroutine(StartDamageTimer());
		}

		override public void UnParent(IMob character)
		{
			if (damageCancelMode == DamageMode.INSTANT_CANCEL) StopAllCoroutines ();
		}

		/// <summary>
		/// Starts the damage timer, then once it expires applies 1 damage.
		/// </summary>
		/// <returns>The fall timer.</returns>
		protected IEnumerator StartDamageTimer() 
		{
			timerHasStarted = true;
			yield return new WaitForSeconds(damageDelay);
			health -= 1;
			if (myRenderer != null && sprites.Length > 0)
			{
				if (sprites [health] == null) myRenderer.enabled = false;
				else myRenderer.sprite = sprites [health];
			}

			if (health == 0)
			{
				myCollider.enabled = false;
			}
			else if (damageCancelMode == DamageMode.ALWAYS_DESTROY) 
			{
				StartCoroutine (StartDamageTimer());	
			}
			OnFired (null);
		}

		public enum DamageMode 
		{
			ALWAYS_DESTROY = 0,
			ALWAYS_DAMAGE = 1,
			INSTANT_CANCEL = 2
		}
	}


}