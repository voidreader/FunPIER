using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An animator that plays animations using the legacy 3D framework.
	/// </summary>
	public class LegacyAnimationBridge : MonoBehaviour, IAnimationBridge
	{
		#region members

		/// <summary>
		/// List of animation states and animations.
		/// </summary>
		public LegacyAnimation[] animationData;

		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		protected IMob myCharacter;
		
		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected Animation myAnimator;
		
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

		/// <summary>
		/// States stored as dictionary for quicker lookup.
		/// </summary>
		protected Dictionary <AnimationState, LegacyAnimation> animationLookup;

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			// Get character reference
			myCharacter = gameObject.GetComponentInParent<Character>();
			if (myCharacter == null) Debug.LogError ("Legacy Animation Bridge unable to find Character or Enemy reference");
			myCharacter.ChangeAnimationState += AnimationStateChanged;
			myAnimator = GetComponentInChildren<Animation>();
			if (myAnimator == null) Debug.LogError ("Legacy Animation Bridge unable to find Unity Animation reference");
			queuedStates = new Queue<AnimationState> ();
			queuedPriorities = new Queue<int> ();
			state = AnimationState.NONE;
			priority = -1;
			animationLookup = new Dictionary<AnimationState, LegacyAnimation>();
			foreach (LegacyAnimation a in animationData)
			{
				// Populate dictionary
				animationLookup.Add (a.state, a);
				// And add all clips
				myAnimator.AddClip(a.clip, a.state.AsString());
				// Set wrap mode
				a.clip.wrapMode = a.wrapMode;
			}
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

				if (animationLookup.ContainsKey(nextState)) 
				{
					if (nextPriority >= priority)
					{
						myAnimator.CrossFade(nextState.AsString(), animationLookup[nextState].fadeTime);
						state = nextState;
						priority = nextPriority;
						queuedStates.Dequeue ();
						queuedPriorities.Dequeue();
					}
					else 
					{
						bool hasFinished = false;
						if (!myAnimator.IsPlaying(state.AsString ())) 
						{
							hasFinished = true;
						} 
						else 
						{
							foreach (UnityEngine.AnimationState info in myAnimator)
							{
								if (info.name == state.AsString())
								{
									if (info.normalizedTime >= 1.0f) hasFinished = true;
								}
							}
						}
						if (hasFinished)	
						{
							myAnimator.CrossFade(nextState.AsString(), animationLookup[nextState].fadeTime);
							state = nextState;
							priority = nextPriority;
							queuedStates.Dequeue ();
							queuedPriorities.Dequeue();
						}
					}
				}
				else
				{
					Debug.LogWarning("Unable to find an animation for AnimationState: " + nextState);
					state = AnimationState.NONE;
					priority = -1;
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
		}

		#endregion

		#region protected methods

		/// <summary>
		/// Handles animation state changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		protected void AnimationStateChanged( object sender, AnimationEventArgs args)
		{
			queuedStates.Enqueue (args.State);
			queuedPriorities.Enqueue (args.Priority);
		}
		
		#endregion

		#region public methods and properties
		
		/// <summary>
		/// Returns null as no animator is being used.
		/// </summary>
		virtual public Animator Animator 
		{
			get { return null; }
		}
		
		/// <summary>
		/// Reset the animation state.
		/// </summary>
		virtual public void Reset() 
		{
		}
		
		#endregion

	}


}
