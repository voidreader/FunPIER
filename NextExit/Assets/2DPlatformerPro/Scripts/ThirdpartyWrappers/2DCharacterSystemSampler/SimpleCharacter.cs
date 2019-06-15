using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CharacterSystem2D
{
	/// <summary>
	/// A simple character which allows for animation switching only.
	/// </summary>
	public class SimpleCharacter : MonoBehaviour {

		public Animator myAnimator;

		protected string currentAnimation;
		protected string queuedAnimation;
		protected bool hasPlayed;

		void Start()
		{
			if (myAnimator == null) myAnimator = GetComponentInChildren<Animator>();
			currentAnimation = "IDLE";
			myAnimator.Play(currentAnimation);
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			// Ensure we played the state for at least one frame, this is to work around for Mecanim issue where calling Play isn't always playing the animation
			if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName(currentAnimation))
			{
				hasPlayed = true;
				// Now play the queued state
				if (queuedAnimation != null)
				{
					myAnimator.Play(queuedAnimation);
					currentAnimation = queuedAnimation;
					queuedAnimation = null;
					hasPlayed = false;
				}
			}
		}

		/// <summary>
		/// Switches the character current animation.
		/// </summary>
		/// <param name="animationName">Animation name.</param>
		public void SwitchAnimation(string animationName)
		{
			if (hasPlayed)
			{
				myAnimator.Play(animationName);
				currentAnimation = animationName;
				queuedAnimation = null;
				hasPlayed = false;
			}
			else
			{
				queuedAnimation = animationName;
			}
		}
	}

}