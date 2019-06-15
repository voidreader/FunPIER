using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Applies an animator override to flip directions.
	/// </summary>
	public class LeftRightAnimatorOverrideDirectionFacer : MonoBehaviour
	{
		
		/// <summary>
		/// The override controller for direction flip.
		/// </summary>
		public AnimatorOverrideController controller;

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
		/// Store default controller.
		/// </summary>
		protected RuntimeAnimatorController defaultController;

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
		/// Cached reference o the animator.
		/// </summary>
		protected Animator myAnimator;

		protected IAnimationBridge bridge;

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			// This is not elegant but its a simple and effective way to handle interfaces in Unity
			animatable = (IMob)gameObject.GetComponentInParent (typeof(IMob));
			if (animatable == null) Debug.LogError ("LeftRightAnimatorOverrideDirectionFacer can't find the animatable reference");
			cachedScale = transform.localScale;
			cachedOffset = transform.localPosition;
			myAnimator = GetComponentInChildren<Animator>();
			if (myAnimator == null) Debug.LogError ("LeftRightAnimatorOverrideDirectionFacer unable to find Unity Animator reference");
			defaultController = myAnimator.runtimeAnimatorController;
			bridge = (IAnimationBridge)GetComponentInChildren(typeof(IAnimationBridge));
			if (bridge == null) Debug.LogError ("LeftRightAnimatorOverrideDirectionFacer unable to find Unity Animation Bridge");
		}

		/// <summary>
		/// Unity update hook, face correct direction.
		/// </summary>
		void Update ()
		{
			if (enabled && !TimeManager.Instance.Paused)
			{
				if (animatable.LastFacedDirection == (flipLeftAndRight ? -1 : 1)  && myAnimator.runtimeAnimatorController != defaultController)
				{
					myAnimator.runtimeAnimatorController = defaultController;
					bridge.Reset();
				}
				else if (animatable.LastFacedDirection == (flipLeftAndRight ? 1 : -1) && myAnimator.runtimeAnimatorController != controller)
				{
					myAnimator.runtimeAnimatorController = controller;
					bridge.Reset();
				}
				if (flipSpriteOffset) 
				{
					if (animatable.LastFacedDirection == 1)
					{
						transform.localPosition = new Vector3((flipLeftAndRight ? -1 : 1) * cachedOffset.x, cachedOffset.y, cachedOffset.z);
					}
					else if (animatable.LastFacedDirection == -1)
					{
						transform.localPosition = new Vector3((flipLeftAndRight ? 1 : -1) * cachedOffset.x, cachedOffset.y, cachedOffset.z);
					}
				}
			}
		}

	}

}