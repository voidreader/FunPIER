using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that springs the character horizontally, requires a physics like ground movement.
	/// </summary>
	public class HorizontalSpringboardPlatform : Platform
	{
		/// <summary>
		/// How long to wait before the character is flung back.
		/// </summary>
		[Tooltip ("How long to wait before the character is flung back.")]
		[Header ("Springboard Settings")]
		public float springDelay = 0.2f;

		/// <summary>
		/// How long to wait before reseting the spring animation after flinging the character.
		/// </summary>
		[Tooltip ("How long to wait before reseting the spring animation after flinging the character.")]
		public float resetDelay = 0.05f;

		/// <summary>
		/// How high the character will spring.
		/// </summary>
		[Tooltip ("How fast the character will spring")]
		public float springVelocity = 25.0f;

		/// <summary>
		/// The sprite to use when the springboard has sprung.
		/// </summary>
		[Tooltip ("The sprite to use when the springboard has sprung.")]
		[Header ("Animation")]
		public Sprite sprungSprite;

		/// <summary>
		/// The character currently on the springboard.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Stores a copy of the springboards original (unsprung) sprite.
		/// </summary>
		protected Sprite originalSprite;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init()
		{
			base.Init();
			if (sprungSprite != null)
			{
				SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
				if (spriteRenderer != null) originalSprite = spriteRenderer.sprite;
			}
		}

		/// <summary>
		/// If the collision is a foot and character is moving down get ready to launch.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="type">Type of raycast.</param>
		/// <returns>true if character should be parented to this platform, otherwise false.</returns>
		override protected bool CustomCollide(PlatformCollisionArgs args)
		{
			if (args.Character is Character &&
			    (args.RaycastCollider.RaycastType == RaycastType.SIDE_RIGHT && springVelocity < 0) ||
			    (args.RaycastCollider.RaycastType == RaycastType.SIDE_LEFT && springVelocity > 0))
			{
				if (character == args.Character) return true;

				if (character == null)
				{
					character = (Character) args.Character;
					StopCoroutine(DelaySpring());
					StartCoroutine(DelaySpring());
					return true;
                }
			}
			return false;
		}
	
		/// <summary>
		/// Called when the character is unparented from this platform.
		/// </summary>
		override public void UnParent()
		{
			StopCoroutine (DelaySpring ());
			character = null;
		}

		/// <summary>
		/// Delays the spring until ready.
		/// </summary>
		protected virtual IEnumerator DelaySpring()
		{
			float elapsedTime = 0.0f;
			while (elapsedTime < springDelay)
			{
				elapsedTime += TimeManager.FrameTime;
				yield return true;
			}
			if (character != null)
			{
				DoSpring();
			}

		}

		/// <summary>
		/// Resets the spring after a delay.
		/// </summary>
		/// <returns>The spring.</returns>
		protected virtual IEnumerator ResetSpring()
		{
			float elapsedTime = 0.0f;
			while (elapsedTime < resetDelay)
			{
				elapsedTime += TimeManager.FrameTime;
				yield return true;
			}
			DoReset ();
		}

		/// <summary>
		/// Does the actual springing.
		/// </summary>
		protected virtual void DoSpring()
		{
			character.SetVelocityX(springVelocity);
			character = null;
			if (sprungSprite != null)
			{
				SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
				if (spriteRenderer != null) spriteRenderer.sprite = sprungSprite;
			}
			StartCoroutine (ResetSpring ());
		}

		/// <summary>
		/// Does the spring reset.
		/// </summary>
		protected virtual void DoReset()
		{
			if (sprungSprite != null)
			{
				SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
				if (spriteRenderer != null) spriteRenderer.sprite = originalSprite;
			}
		}

		/// <summary>
		/// Skip air movements as this platform wants to control the jump button.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">The character.</param>
		/// <param name="movement">Movement being checked.</param>
		override public bool SkipMovement(Character character, Movement movement)
		{
			if (movement is AirMovement)
			{
				return true;
			}
			return false;
		}

	}

}