using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An animator that plays animations directly on a mecanim controller. Typically used for 2D sprites.
	/// </summary>
	public class MecanimAnimationBridge_2D : MonoBehaviour, IAnimationBridge
	{
		
		#region members

		/// <summary>
		/// Maps from states to animator overrides.
		/// </summary>
		public List<AnimatorControllerMapping> mappings;

		/// <summary>
		/// Lookup table of attack states to animator overrides.
		/// </summary>
		protected Dictionary<string, AnimatorOverrideController> animationStateOverrideLookup;

		/// <summary>
		/// Store default controller.
		/// </summary>
		protected RuntimeAnimatorController defaultController;

		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		protected IMob myCharacter;
		
		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected Animator myAnimator;
		
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
		protected PriorityQueue queuedPriorities;


#if UNITY_EDITOR
		/// <summary>
		/// In the editor track state names so we can show an error message if a state is missing.
		/// </summary>
		protected List<string> editor_stateNames;
#endif

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
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				// Ensure we played the current state for at least one frame, this is to work around for Mecanim issue where calling Play isn't always playing the animation
				if (state == AnimationState.NONE || info.IsName(state.AsString()) || info.IsName(nextState.AsString()))
				{
					if (info.IsName(nextState.AsString()))
					{
						state = nextState;
						priority = nextPriority;
						queuedStates.Dequeue ();
						queuedPriorities.Dequeue();
					}
					// Next animation has higher priority, play it now
					else if (nextPriority >= priority || info.normalizedTime >= 1.0f ) // || info.loop)
					{
						myAnimator.Play(nextState.AsString());
						state = nextState;
						priority = nextPriority;
						queuedStates.Dequeue ();
						queuedPriorities.Dequeue();
					}
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
			if (myCharacter == null) Debug.LogError ("Mecanim Animation Bridge (2D) unable to find Character or Enemy reference");
			myCharacter.ChangeAnimationState += AnimationStateChanged;
			myAnimator = GetComponentInChildren<Animator>();
			if (myAnimator == null) Debug.LogError ("Platform Animator unable to find Unity Animator reference");
			defaultController = myAnimator.runtimeAnimatorController;


			animationStateOverrideLookup = new Dictionary<string, AnimatorOverrideController> ();
			foreach (AnimatorControllerMapping mapping in mappings)
			{
				animationStateOverrideLookup.Add (mapping.overrrideState, mapping.controller);
			}

			queuedStates = new Queue<AnimationState> ();
			queuedPriorities = new PriorityQueue ();
			state = AnimationState.NONE;
			priority = -1;

			TimeManager.Instance.GamePaused += HandleGamePaused;
			TimeManager.Instance.GameUnPaused += HandleGameUnPaused;

#if UNITY_EDITOR
	#if UNITY_5
			
			// In editor mode build a list of handled states for error messaging and the like
			if (myAnimator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
			{
				editor_stateNames = new List<string>();
				UnityEditor.Animations.AnimatorStateMachine stateMachine = ((UnityEditor.Animations.AnimatorController)defaultController).layers[0].stateMachine;
				for (int i = 0; i < stateMachine.states.Length; i++)
				{
					editor_stateNames.Add (stateMachine.states[i].state.name);
				}
			}

	#else

			// In editor mode build a list of handled states for error messaging and the like
			if (myAnimator.runtimeAnimatorController is UnityEditorInternal.AnimatorController)
			{
				editor_stateNames = new List<string>();
				UnityEditorInternal.StateMachine stateMachine = ((UnityEditorInternal.AnimatorController)defaultController).GetLayer(0).stateMachine;
				for (int i = 0; i < stateMachine.stateCount; i++)
				{
					editor_stateNames.Add (stateMachine.GetState(i).name);
				}

			}
	#endif
#endif
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
#if UNITY_EDITOR
	#if UNITY_5
			// In editor mode check that the state is present
			if (myAnimator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
			{
				if (!editor_stateNames.Contains(args.State.AsString()))
				{
					Debug.LogError("The state " + args.State.AsString() + " is not in the animator, state names must be an exact UPPERCASE match for the Animation State. This is being skipped in editor mode but this will lead to errors in release builds.");
					return;
				}
			}
	#else

			// In editor mode check that the state is present
			if (myAnimator.runtimeAnimatorController is UnityEditorInternal.AnimatorController)
			{
				if (!editor_stateNames.Contains(args.State.AsString()))
				{
					Debug.LogError("The state " + args.State.AsString() + " is not in the animator, state names must be an exact UPPERCASE match for the Animation State. This is being skipped in editor mode but this will lead to errors in release builds.");
					return;
				}
			}
	#endif
#endif
			// Don't queue states that are already queued
			if (queuedStates.Count < 1 || queuedStates.Peek() != args.State)
			{
				queuedStates.Enqueue (args.State);
				queuedPriorities.EnqueueAndPromote (args.Priority);
			}
			if (animationStateOverrideLookup.Count > 0 && args.OverrideState != null && animationStateOverrideLookup.ContainsKey(args.OverrideState))
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				myAnimator.runtimeAnimatorController = animationStateOverrideLookup[args.OverrideState];
				if (state != AnimationState.NONE) 
				{
					myAnimator.Play(state.AsString(), 0, info.normalizedTime);
				}
			}
			else if (args.OverrideState == null && animationStateOverrideLookup.Count > 0)
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				myAnimator.runtimeAnimatorController = defaultController;
				if (state != AnimationState.NONE)
				{
					myAnimator.Play(state.AsString(), 0, info.normalizedTime);
				}
			}
		}
		
		#endregion

		#region public methods and properties

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
				myAnimator.Play(state.AsString(), 0, info.normalizedTime);
			}
		}

		#endregion
	}
}