// To use first import and install 2D Tool Kit then uncomment the line below
// #define TK2D_AVAILABLE

#if TK2D_AVAILABLE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using tk2dRuntime;


namespace PlatformerPro
{
	/// <summary>
	/// An animator that plays animations directly on a mecanim controller. Typically used for 2D sprites.
	/// </summary>
	public class AnimationBridge_2DTK : MonoBehaviour, IAnimationBridge
	{
		
		#region members

		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		protected IMob myCharacter;
		
		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected tk2dSpriteAnimator myAnimator;
		
		/// <summary>
		/// The state currently playing.
		/// </summary>
		protected AnimationState state;
		
		/// <summary>
		/// The animation state that should play next.
		/// </summary>
		protected Queue<AnimationState> queuedStates;

		/// <summary>
		/// The current states priority.
		/// </summary>
		protected int priority;
		
		/// <summary>
		/// The queued states priority.
		/// </summary>
		protected Queue<int> queuedPriorities;

		#endregion
		
		#region unity hooks
		
		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}
		
		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			// If we have a new animation to play
			if (queuedStates.Count > 0)
			{
				AnimationState nextState = queuedStates.Peek ();
				int nextPriority = queuedPriorities.Peek ();

#if UNITY_EDITOR
				if (myAnimator.GetClipByName(nextState.AsString()) == null)
				{
					Debug.LogWarning("Clip with the name " + nextState.AsString() + " not found in animator. Clip names must be an exact uppercase match for state names");
					state = nextState;
					priority = nextPriority;
					queuedStates.Dequeue ();
					queuedPriorities.Dequeue();
					return;
				}
#endif
				// TODO Handle priority -> how do we get normalized time in 2DTK
				if (nextPriority >= priority || ((myAnimator.CurrentFrame + 1) >= myAnimator.CurrentClip.frames.Length ))
				{
					myAnimator.Play(nextState.AsString());
					state = nextState;
					priority = nextPriority;
					queuedStates.Dequeue ();
					queuedPriorities.Dequeue();
				}
			}
		}
		
		/// <summary>
		/// Unity OnDestory hook.
		/// </summary>
		void OnDestroy()
		{
			if (myCharacter != null) myCharacter.ChangeAnimationState -= AnimationStateChanged;
			if (TimeManager.SafeInstance != null) TimeManager.SafeInstance.GamePaused -= HandleGamePaused;
			if (TimeManager.SafeInstance != null) TimeManager.SafeInstance.GameUnPaused -= HandleGameUnPaused;
		}
		
		#endregion
		
		#region protected methods

		/// <summary>
		/// Initialise this animation bridge.
		/// </summary>
		protected void Init()
		{
			// Get character reference
			myCharacter = (IMob) gameObject.GetComponent(typeof(IMob));
            if (myCharacter == null) myCharacter = (IMob) gameObject.GetComponentInParent(typeof(IMob));
			if (myCharacter == null) Debug.LogError ("2DTK Animation Bridge unable to find Character or Enemy reference");
			myCharacter.ChangeAnimationState += AnimationStateChanged;
			myAnimator = GetComponentInChildren<tk2dSpriteAnimator>();
			if (myAnimator == null) Debug.LogError ("2DTK Animation Bridge unable to find 2DTK Animator reference");

			queuedStates = new Queue<AnimationState> ();
			queuedPriorities = new Queue<int> ();
			state = AnimationState.NONE;
			priority = -1;

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

			// Don't queue states that are already queued
			if (queuedStates.Count < 1 || queuedStates.Peek() != args.State)
			{
				queuedStates.Enqueue (args.State);
				queuedPriorities.Enqueue (args.Priority);
			}
		}
		
		#endregion

		#region public methods and properties

		virtual public Animator Animator 
		{
			get { return null; }
		}

		/// <summary>
		/// Reset the animation state.
		/// </summary>
		virtual public void Reset() 
		{
			if (myAnimator != null)
			{
#if UNITY_EDITOR
				if (myAnimator.GetClipByName (state.AsString ()) == null)
				{
					Debug.LogWarning ("Clip with the name " + state.AsString () + " not found in animator. Clip names must be an exact uppercase match for state names");
						return;
				}
#endif
				myAnimator.Play (state.AsString ());
			}
		}

		#endregion
	}
}
#endif