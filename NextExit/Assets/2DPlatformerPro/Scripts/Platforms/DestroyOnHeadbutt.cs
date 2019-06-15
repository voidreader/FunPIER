using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Platform that spawns (or adds) an item on headbutt.
	/// </summary>
	public class DestroyOnHeadbutt : Platform
	{
		[Header ("Destroy")]

		/// <summary>
		/// How long does the GameObject stay active before its destroyed.
		/// </summary>
		[Tooltip ("How long does the GameObject stay active before its destroyed. Allows time for particle effects, etc")]
		public float destroyDelay = 1.0f;

		/// <summary>
		/// The visible component, this will be disabled straight away.
		/// </summary>
		[Tooltip ("The visible component, this will be disabled straight away.")]
		public GameObject visibleComponent;

		/// <summary>
		/// If true object will be destoryed, if false it will just be set to inactive.
		/// </summary>
		[Tooltip ("If true object will be destroyed, if false it will just be set to inactive.")]
		public bool destroy;

		[Header ("Damage")]
		/// <summary>
		/// If this is non-null the attached collider will be briefly enabled on headbut causing damage to anything touching it.
		/// This can be used for the typical mario style headbut of the block to cause damage,
		/// </summary>
		[Tooltip ("If this is non-null the attached collider will be briefly enabled on headbut causing damage to anything touching it.")]
		public Collider2D damageCollider;

		/// <summary>
		/// Has the destory started?
		/// </summary>
		protected bool isbeingDestroyed;

		/// <summary>
		/// Cached copy of damage event args to save on allocations.
		/// </summary>
		protected DamageInfoEventArgs damageEventArgs;

		/// <summary>
		/// How long do we cause damage from this platform. A constant because its unlikely to need changing.
		/// </summary>
		const float DamageTime = 0.15f;

		/// <summary>
		/// Event for destory.
		/// </summary>
		public event System.EventHandler <DamageInfoEventArgs> Destroyed;
		
		/// <summary>
		/// Raises the Destroyed event.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual protected void OnDestroyed(DamageInfo info)
		{
			if (Destroyed != null)
			{
				damageEventArgs.UpdateDamageInfoEventArgs(info);
				Destroyed(this, damageEventArgs);
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init ()
		{
			base.Init ();
			damageEventArgs = new DamageInfoEventArgs();
		}

		/// <summary>
		/// Called when one of the characters colliders collides with this platform. This should be overriden for platform specific behaviour.
		/// </summary>
		/// <param name="PlatformCollisionArgs">Arguments describing a platform collision.</param>
		/// <returns>true if character should be parented to this platform, otherwise false.</returns>
		override protected bool CustomCollide(PlatformCollisionArgs args)
		{
			if (args.RaycastCollider.RaycastType == RaycastType.HEAD && args.Character is Character)
			{
				DoHeadbutt((Character)args.Character);
			}
			return false;
		}

		/// <summary>
		/// Do the headbutt
		/// </summary>
		virtual protected void DoHeadbutt(Character character)
		{
			if (!isbeingDestroyed) 
			{
				StartCoroutine (DoNudge (character));
			}
		}

		/// <summary>
		/// Do the nudge.
		/// </summary>
		/// <returns>The nudge.</returns>
		virtual protected IEnumerator DoNudge(Character character)
		{
			isbeingDestroyed = true;

			// Hide visible component
			if (visibleComponent != null) visibleComponent.SetActive (false);

			// Disable collider
			Collider2D myCollider = GetComponent<Collider2D> ();
			if (myCollider != null) myCollider.enabled = false;

			// Enable damage causer if we have one
			if (damageCollider != null) damageCollider.enabled = true;

			// Send event
			OnDestroyed(new DamageInfo(0, DamageType.PHYSICAL, Vector2.up, character));

			// Delay before destroying
			float timer = 0;

			// Force a delay if we have a damage collider
			if (destroyDelay < DamageTime && damageCollider != null) destroyDelay = 2.0f * DamageTime;

			while (timer < destroyDelay)
			{
				if (timer < DamageTime && damageCollider != null && damageCollider.enabled )
				{
					damageCollider.enabled = false;
				}
				timer += TimeManager.FrameTime;
				yield return true;
			}

			// Destroy
			if (destroy)
			{
				Destroy (gameObject);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}