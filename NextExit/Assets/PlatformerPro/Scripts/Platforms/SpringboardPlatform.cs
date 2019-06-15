using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that springs the character in to the air using the characters DefaultAirMovement.
	/// </summary>
	public class SpringboardPlatform : Platform
	{
		/// <summary>
		/// How long to wait before the character is flung in to the air.
		/// </summary>
		[Tooltip ("How long to wait before the character is flung in to the air.")]
		[Header ("Springboard Settings")]
		public float springDelay = 0.2f;

		/// <summary>
		/// How long to wait before reseting the spring animation after flinging the character.
		/// </summary>
		[Tooltip ("How long to wait before reseting the spring animation after flinging the character.")]
		public float resetDelay = 0.05f;

		/// <summary>
		/// How high the character will spring if they do not press jump.
		/// </summary>
		[Tooltip ("How high the character will spring if they do not press jump.")]
		public float springHeight = 2.0f;

		/// <summary>
		/// How high the character will spring if they do press jump.
		/// </summary>
		[Tooltip ("How high the character will spring if they do press jump.")]
		public float springPlusJumpHeight = 4.0f;

		/// <summary>
		/// Does the character have to press jump to get the high jump or can they just hold the jump button down?
		/// </summary>
		[Tooltip ("Does the character have to press jump (i.e. time the jump to fall within the delay) to get the high jump or can they just hold the jump button down?")]
		public bool pressJumpToGetHighJump = true;

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
		/// Track if we have pressed/held jump while on the platform.
		/// </summary>
		protected bool jumpHasBeenPressed;

		/// <summary>
		/// Stores a copy of the springboards original (unsprung) sprite.
		/// </summary>
		protected Sprite originalSprite;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			base.PostInit();
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
			if (args.RaycastCollider.RaycastType == RaycastType.FOOT && args.Character is Character)
			{
				if (character == (Character)args.Character) return true;
				if (args.Character.Velocity.y <= 0.0f)
				{
					if (character == null)
					{
						character = (Character) args.Character;
						StopCoroutine(DelaySpring());
						StartCoroutine(DelaySpring());
						return true;
	                }
				}
			}
			return false;
		}
	
		/// <summary>
		/// Called when the character is unparented from this platform.
		/// </summary>
		override public void UnParent(IMob character)
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
			jumpHasBeenPressed = false;
			while (elapsedTime < springDelay)
			{
				if (character != null && (character.Input.JumpButton == ButtonState.DOWN || (!pressJumpToGetHighJump && character.Input.JumpButton == ButtonState.HELD)))
				{
					jumpHasBeenPressed = true;
				}
				if (character == null)
				{
					jumpHasBeenPressed = false;
				}
				elapsedTime += TimeManager.FrameTime;
				yield return true;
			}
			if (character != null)
			{
				DoSpring();
				OnFired (character);
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
			character.DefaultAirMovement.DoOverridenJump (jumpHasBeenPressed ? springPlusJumpHeight : springHeight, 1);
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