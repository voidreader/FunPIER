using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An animator that plays animations directly on a mecanim controller. Typically used for 2D sprites.
	/// </summary>
	public class MecanimAnimationBridge_2DWithModifiers : MonoBehaviour, IAnimationBridge
	{
		
		#region members

		/// <summary>
		/// Maps from states to animator overrides.
		/// </summary>
		[Tooltip ("Maps from override names to animator controller overrides. Can be used to override animations sets for shooting, weapons, power-ups, etc.")]
		public List<AnimatorControllerMapping> mappings;

		/// <summary>
		/// A list of states that have the _UP modifier added to the end if the character is aiming upwards.
		/// </summary>
		[Tooltip ("A list of states that have the _UP modifier added to the end if the character is aiming upwards.")]
		public List<AnimationState> statesWithAimUpModifer;

		/// <summary>
		/// A list of states that have the _DOWN modifier added to the end if the character is aiming downwards."
		/// </summary>
		[Tooltip ("A list of states that have the _DOWN modifier added to the end if the character is aiming downwards.")]
		public List<AnimationState> statesWithAimDownModifer;

		/// <summary>
		/// A list of states that have the _RIGHT and _LEFT modifiers added based on characters facing direction.
		/// </summary>
		[Tooltip ("A list of states that have the _RIGHT and _LEFT modifiers added based on characters facing direction.")]
		public List<AnimationState> statesWithLeftRightModifer;

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
		protected IMob myMob;
		
		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected Animator myAnimator;
		
		/// <summary>
		/// The state currently playing.
		/// </summary>
		protected string state;
		
		/// <summary>
		/// The animation state that should play next.
		/// </summary>
		protected Queue<string> queuedStates;

		/// <summary>
		/// The current states priority.
		/// </summary>
		protected int priority;
		
		/// <summary>
		/// The queued states priority.
		/// </summary>
		protected Queue<int> queuedPriorities;

		/// <summary>
		/// Keep a reference to the character too.
		/// </summary>
		protected Character myCharacter;

		/// <summary>
		/// Keep a reference to the aimer.
		/// </summary>
		protected ProjectileAimer aimer;

		/// <summary>
		/// The previous frames aim direction (UP = 1, NEUTRAL = 0, DOWN = 1);
		/// </summary>
		protected int previousAimDirection;

		/// <summary>
		/// Cached animation event args for triggering local update.
		/// </summary>
		protected AnimationEventArgs animationEventArgs;

		/// <summary>
		/// Track this so we only ever change state once per frame.
		/// </summary>
		protected bool stateChangedThisFrame;

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
			stateChangedThisFrame = false;
			// If we have a new animation to play
			if (queuedStates.Count > 0)
			{
				string nextState = queuedStates.Peek ();
				int nextPriority = queuedPriorities.Peek ();
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				// Ensure we played the current state for at least one frame, this is to work around for Mecanim issue where calling Play isn't always playing the animation
				if (state == AnimationState.NONE.AsString() || info.IsName(state))
				{
					// Next animation has higher priority, play it now
					if (nextPriority >= priority || info.normalizedTime >= 1.0f ) // || info.loop)
					{
						myAnimator.Play(nextState);
						state = nextState;
						priority = nextPriority;
						queuedStates.Dequeue ();
						queuedPriorities.Dequeue();
						stateChangedThisFrame = true;
					}
				}
			}

			// If we have an aimer set the gun variables
			if (aimer != null)
			{
				Vector2 aimDirection = aimer.GetAimDirection((Character)myCharacter);
				int aimDirectionInt = GetYAimAsInt(aimDirection);
				if (aimDirectionInt != previousAimDirection) 
				{
					animationEventArgs.UpdateAnimationEventArgs(myCharacter.AnimationState, myCharacter.AnimationState, myCharacter.OverrideState);
					if (!stateChangedThisFrame) AnimationStateChanged(this, animationEventArgs);
				}
				previousAimDirection = aimDirectionInt;
			}
			animationEventArgs = new AnimationEventArgs(AnimationState.NONE, AnimationState.NONE, null);
		}
		
		/// <summary>
		/// Unity OnDestory hook.
		/// </summary>
		void OnDestroy()
		{
			if (myMob != null) myMob.ChangeAnimationState -= AnimationStateChanged;
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
			myMob = (IMob) gameObject.GetComponent(typeof(IMob));
            if (myMob == null) myMob = (IMob) gameObject.GetComponentInParent(typeof(IMob));
			if (myMob == null) Debug.LogError ("Mecanim Animation Bridge (2D) unable to find Character or Enemy reference");
			myMob.ChangeAnimationState += AnimationStateChanged;
			myAnimator = GetComponentInChildren<Animator>();
			if (myAnimator == null) Debug.LogError ("Platform Animator unable to find Unity Animator reference");
			defaultController = myAnimator.runtimeAnimatorController;

			if (myMob is Character && (statesWithAimUpModifer.Count > 0 || statesWithAimDownModifer.Count > 0)) 
			{
				myCharacter = (Character) myMob;
				aimer = myCharacter.GetComponentInChildren<ProjectileAimer>();
			}
			if ((statesWithAimUpModifer.Count > 0 || statesWithAimDownModifer.Count > 0) && aimer == null) Debug.LogWarning ("Can't use UP or DOWN modifiers as no aimer could be found");

			animationStateOverrideLookup = new Dictionary<string, AnimatorOverrideController> ();
			foreach (AnimatorControllerMapping mapping in mappings)
			{
				animationStateOverrideLookup.Add (mapping.overrrideState, mapping.controller);
			}

			queuedStates = new Queue<string> ();
			queuedPriorities = new Queue<int> ();
			state = AnimationState.NONE.AsString();
			priority = -1;

			TimeManager.Instance.GamePaused += HandleGamePaused;
			TimeManager.Instance.GameUnPaused += HandleGameUnPaused;

#if UNITY_EDITOR
	#if UNITY_5
			// TODO also check for up and down states
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
			// TODO also check for up and down states
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
		/// Gets the Y aim direciton as int.
		/// </summary>
		/// <returns>The Y aim as an int.</returns>
		/// <param name="aimDirection">Aim direction.</param>
		virtual protected int GetYAimAsInt(Vector2 aimDirection)
		{
			// TODO Make these thresholds a variable
			if (aimDirection.y >= 0.33f) return 1;
			if (aimDirection.y <= -0.33f) return -1;
			return 0;
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
			string newStateString = GetNewStateString (args);
#if UNITY_EDITOR
	#if UNITY_5
			// In editor mode check that the state is present
			if (myAnimator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
			{
				if (!editor_stateNames.Contains(newStateString))
				{
					Debug.LogError("The state " + newStateString + " is not in the animator, state names must be an exact UPPERCASE match for the Animation State. This is being skipped in editor mode but this will lead to errors in release builds.");
					return;
				}
			}
	#else

			// In editor mode check that the state is present
			if (myAnimator.runtimeAnimatorController is UnityEditorInternal.AnimatorController)
			{
				if (!editor_stateNames.Contains(newStateString))
				{
					Debug.LogError("The state " + newStateString + " is not in the animator, state names must be an exact UPPERCASE match for the Animation State. This is being skipped in editor mode but this will lead to errors in release builds.");
					return;
				}
			}
	#endif
#endif
			// Don't queue states that are already queued
			if (queuedStates.Count < 1 || queuedStates.Peek() != args.State.AsString())
			{
				queuedStates.Enqueue (newStateString);
				queuedPriorities.Enqueue (args.Priority);
			}
			if (args.OverrideState != null && animationStateOverrideLookup.ContainsKey(args.OverrideState))
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				myAnimator.runtimeAnimatorController = animationStateOverrideLookup[args.OverrideState];
				if (state != AnimationState.NONE.AsString()) 
				{
					myAnimator.Play(state, 0, info.normalizedTime);
					stateChangedThisFrame = true;
				}
			}
			else if (args.OverrideState == null)
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				myAnimator.runtimeAnimatorController = defaultController;
				if (state != AnimationState.NONE.AsString())
				{
					myAnimator.Play(state, 0, info.normalizedTime);
					stateChangedThisFrame = true;
				}
			}
		}

		/// <summary>
		/// Gets a string representation of the animation to play.
		/// </summary>
		/// <returns>The new state string.</returns>
		/// <param name="args">Animation event arguments.</param>
		virtual protected string GetNewStateString(AnimationEventArgs args)
		{
			//TODO Is it worth using a buffer?
			string result = args.State.AsString ();
			if (aimer != null)
			{
				int aimDir = GetYAimAsInt(aimer.GetAimDirection(myCharacter));
				if (aimDir == 1 && statesWithAimUpModifer.Contains (args.State)) result += "_UP";
				if (aimDir == -1 && statesWithAimDownModifer.Contains (args.State)) result += "_DOWN";
			}
			if (statesWithLeftRightModifer.Contains(args.State))
			{
				if (myMob.LastFacedDirection == 1)
				{
					result += "_RIGHT";
				}
				else
				{
					result += "_LEFT";
				}
			}
			return result;
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
			if (myAnimator != null) myAnimator.Play (state);
		}

		#endregion
	}
}