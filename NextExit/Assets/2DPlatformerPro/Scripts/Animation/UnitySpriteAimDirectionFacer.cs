using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Changes facing direction using transform scale and faces in the aiming direction.
	/// </summary>
	public class UnitySpriteAimDirectionFacer : UnitySpriteDirectionFacer
	{

		/// <summary>
		/// Cached aimer refernce.
		/// </summary>
		protected ProjectileAimer myAimer;

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			// This is not elegant but its a simple and effective way to handle interfaces in Unity
			animatable = (IMob)gameObject.GetComponentInParent (typeof(IMob));
			if (animatable == null) Debug.LogWarning ("UnitySpriteAimDirectionFacer can't find the animatable reference");
			cachedScale = transform.localScale;
			cachedOffset = transform.localPosition;
			myAimer = ((Component)animatable).GetComponentInChildren<ProjectileAimer> ();
			if (myAimer == null) Debug.LogWarning ("UnitySpriteAimDirectionFacer can't find the ProjectileAimer");
			if (gameObject.GetComponent<SpriteRenderer>() == null) Debug.LogWarning("The UnitySpriteDirectionFacer should be placed on the same game object as the SpriteRenderer");
		}

		/// <summary>
		/// Unity update hook, face correct direction.
		/// </summary>
		void Update ()
		{
			if (enabled && !TimeManager.Instance.Paused)
			{
				float dir = myAimer.GetAimDirection((Component)animatable).x;
				if (dir > 0)
				{
					transform.localScale = new Vector3((flipLeftAndRight ? -1 : 1) * cachedScale.x, cachedScale.y, cachedScale.z);
					if (flipSpriteOffset) transform.localPosition = new Vector3((flipLeftAndRight ? -1 : 1) * cachedOffset.x, cachedOffset.y, cachedOffset.z);
				}
				else if (dir < 0)
				{
					transform.localScale = new Vector3((flipLeftAndRight ? 1 : -1) * cachedScale.x, cachedScale.y, cachedScale.z);
					if (flipSpriteOffset) transform.localPosition = new Vector3((flipLeftAndRight ? 1 : -1) * cachedOffset.x, cachedOffset.y, cachedOffset.z);
				}
			}
		}

	}

}