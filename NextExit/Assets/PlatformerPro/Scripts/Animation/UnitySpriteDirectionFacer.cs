using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Changes facing direction using transform.
	/// </summary>
	public class UnitySpriteDirectionFacer : MonoBehaviour
	{
		/// <summary>
		/// Should left = -1 or left = 1?
		/// </summary>
		[Tooltip ("Should left = -1 (false) or left = 1 (true)?")]
		public bool flipLeftAndRight;

		/// <summary>
		/// If the sprite x position is non zero should we inverse it on switch?
		/// </summary>
		[Tooltip ("If the sprite x position is non zero should we inverse it on switch?")]
		public bool flipSpriteOffset;

		/// <summary>
		/// The character reference.
		/// </summary>
		protected IMob animatable;

		/// <summary>
		/// The cached scale.
		/// </summary>
		protected Vector3 cachedScale;

		/// <summary>
		/// The cached offset.
		/// </summary>
		protected Vector3 cachedOffset;

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			// This is not elegant but its a simple and effective way to handle interfaces in Unity
			animatable = (IMob)gameObject.GetComponentInParent (typeof(IMob));
			if (animatable == null) Debug.LogError ("UnitySpriteDirectionFacer can't find the animatable reference");
			cachedScale = transform.localScale;
			cachedOffset = transform.localPosition;
			// if (gameObject.GetComponent<SpriteRenderer>() == null) Debug.LogError("The UnitySpriteDirectionFacer should be placed on the same game object as the SpriteRenderer");
		}

		/// <summary>
		/// Unity update hook, face correct direction.
		/// </summary>
		void Update ()
		{
			if (enabled && !TimeManager.Instance.Paused)
			{
				if (animatable.LastFacedDirection == 1)
				{
					transform.localScale = new Vector3((flipLeftAndRight ? -1 : 1) * cachedScale.x, cachedScale.y, cachedScale.z);
					if (flipSpriteOffset) transform.localPosition = new Vector3((flipLeftAndRight ? -1 : 1) * cachedOffset.x, cachedOffset.y, cachedOffset.z);
				}
				else if (animatable.LastFacedDirection == -1)
				{
					transform.localScale = new Vector3((flipLeftAndRight ? 1 : -1) * cachedScale.x, cachedScale.y, cachedScale.z);
					if (flipSpriteOffset) transform.localPosition = new Vector3((flipLeftAndRight ? 1 : -1) * cachedOffset.x, cachedOffset.y, cachedOffset.z);
				}
			}
		}

	}

}