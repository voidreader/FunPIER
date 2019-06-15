using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Bridge which plays animations from overrides.
	/// </summary>
	public class MecanimOverrideAsAnimationBridge : MonoBehaviour, IAnimationBridge
	{
		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		protected IMob myCharacter;
		
		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected Animator myAnimator;

		/// <summary>
		/// Stores currently playing override.
		/// </summary>
		protected string currentOverride;

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Initialise this animation bridge.
		/// </summary>
		protected void Init()
		{
			// Get character reference
			myCharacter = (IMob)gameObject.GetComponent (typeof(IMob));
			if (myCharacter == null) myCharacter = (IMob)gameObject.GetComponentInParent (typeof(IMob));
			if (myCharacter == null) Debug.LogError (" Animation Bridge) unable to find Character or Enemy reference");
			myCharacter.ChangeAnimationState += AnimationStateChanged;
			myAnimator = GetComponentInChildren<Animator> ();
			if (myAnimator == null) Debug.LogError (" Animation Bridge unable to find Unity Animator reference");
	
			TimeManager.Instance.GamePaused += HandleGamePaused;
			TimeManager.Instance.GameUnPaused += HandleGameUnPaused;

		}
		
		/// <summary>
		/// Handles the game being unpaused.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandleGameUnPaused (object sender, System.EventArgs e)
		{
			myAnimator.enabled = true;
		}
		
		
		/// <summary>
		/// Handles the game being paused.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandleGamePaused (object sender, System.EventArgs e)
		{
			myAnimator.enabled = false;
		}

		/// <summary>
		/// Handles animation state changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		protected void AnimationStateChanged( object sender, AnimationEventArgs args)
		{
			// Stop warnings in Unity 2017
			if (!myAnimator.isActiveAndEnabled) return;
			if (currentOverride != args.OverrideState)
			{
				currentOverride = args.OverrideState;
				if (currentOverride == null || currentOverride == "") myAnimator.Play(AnimationState.IDLE.AsString());
				else myAnimator.Play(currentOverride);
			}
		}

#region public methods and properties

		/// <summary>
		/// Gets the associated animator. Returns null if an animator is not being used.
		/// </summary>
		/// <value>The animator.</value>
		virtual public Animator Animator 
		{
			get { return myAnimator; }
		}

		/// <summary>
		/// Reset the animation state.
		/// </summary>
		virtual public void Reset() 
		{
			if (myAnimator != null) 
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				myAnimator.Play("IDLE", 0, info.normalizedTime);
			}
		}

#endregion

	}
}
