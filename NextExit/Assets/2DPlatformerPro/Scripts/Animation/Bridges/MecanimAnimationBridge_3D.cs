using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An animator that sends sets state and previous state on a mecanim controller. Typically used for 3D models.
	/// </summary>
	public class MecanimAnimationBridge_3D : MonoBehaviour
	{
		
		#region members

		/// <summary>
		/// A work around for the very limited runtimeinformation available from Mecaim. If a clip is shorter
		/// than this it will play from the start when animators are changed instead of playing from the normalized time.
		/// </summary>
		protected const float OneShotLength = 0.25f;

		/// <summary>
		/// Check animator to ensure state plays for at least one frame.
		/// </summary>
		[Tooltip ("Check animator to ensure state plays for at least one frame.")]
		public bool ensureStatePlaysForOneFrame = true;

		/// <summary>
		/// Maps from states to animator overrides.
		/// </summary>
		[Tooltip ("Maps character animation override strings to a Mecanim AnimatorOverrideController.")]
		public List<AnimatorControllerMapping> mappings;
		
		/// <summary>
		/// Lookup table of states to animator overrides.
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
		protected AnimationState queuedState;

		/// <summary>
		/// Track if we have played an animation. If we play multiple animations faster than mecanim can handle it will sometimes not
		/// play the latest state.
		/// </summary>
		protected bool hasPlayed;

		/// <summary>
		/// Cached reference to an aimer.
		/// </summary>
		protected ProjectileAimer aimer;

		/// <summary>
		/// States that never show aiming animations. You could extend this list in your own class.
		/// </summary>
		protected static string[] nonAimingStates = {AnimationState.DEATH.AsString (), AnimationState.GAME_OVER.AsString ()};

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
			ActualUpdate ();
		}
		
		/// <summary>
		/// Unity OnDestory hook.
		/// </summary>
		void OnDestroy()
		{
			if (myCharacter != null) myCharacter.ChangeAnimationState -= AnimationStateChanged;
			if (myCharacter != null && myCharacter is Character) ((Character)myCharacter).Respawned -= HandleRespawned;
		}

				#endregion
		
		#region protected methods

		/// <summary>
		/// Implements the update code so it can be reused.
		/// </summary>
		virtual protected void ActualUpdate()
		{
			if (ensureStatePlaysForOneFrame)
			{
				// Ensure we played the state for at least one frame, this is to work around for Mecanim issue where calling Play isn't always playing the animation
				if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName(state.AsString()))
				{
					hasPlayed = true;
					myAnimator.SetBool("HasPlayed", true);
					// Now play the queued state
					if (queuedState != AnimationState.NONE)
					{
						myAnimator.SetInteger("PreviousState", (int)state);
						myAnimator.SetInteger("State", (int)queuedState);
						state = queuedState;
						queuedState = AnimationState.NONE;
						hasPlayed = false;
						myAnimator.SetBool("HasPlayed", false);
					}
					
				}
			}
			
			// If we have an aimer set the gun variables
			if (aimer != null)
			{
				Vector2 aimDirection = aimer.GetAimDirection((Character)myCharacter);
				myAnimator.SetFloat("GunPositionX",aimDirection.x);
				myAnimator.SetFloat("GunPositionY",aimDirection.y);
			}
			
			// Set velocity vars
			myAnimator.SetFloat("VelocityX", myCharacter.Velocity.x);
			myAnimator.SetFloat("VelocityY", myCharacter.Velocity.y);
			myAnimator.SetInteger("FacingDirection", myCharacter.FacingDirection);
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			// Get character reference
			myCharacter = (IMob) gameObject.GetComponent(typeof(IMob));
			if (myCharacter == null) myCharacter = (IMob) gameObject.GetComponentInParent(typeof(IMob));
			if (myCharacter == null) Debug.LogError ("Mecanim Animation Bridge (3D) unable to find Character or Enemy reference");

			if (myCharacter != null )
			{
				// Events
				myCharacter.ChangeAnimationState += AnimationStateChanged;
				if (myCharacter is Character) ((Character)myCharacter).Respawned += HandleRespawned;

				myAnimator = GetComponentInChildren<Animator>();
				if (myAnimator == null) Debug.LogError ("Platform Animator unable to find Unity Animator reference");
				defaultController = myAnimator.runtimeAnimatorController;

				// Get an aimer if one is present
				ProjectileAimer tmpAimer = ((Component)myCharacter).gameObject.GetComponent<ProjectileAimer> ();
				if (tmpAimer != null && myCharacter is Character && (tmpAimer.aimType == ProjectileAimingTypes.EIGHT_WAY || tmpAimer.aimType == ProjectileAimingTypes.SIX_WAY)) {
					aimer = tmpAimer;
				}

				// Set up animation overrides
				animationStateOverrideLookup = new Dictionary<string, AnimatorOverrideController> ();
				foreach (AnimatorControllerMapping mapping in mappings)
				{
					animationStateOverrideLookup.Add (mapping.overrrideState, mapping.controller);
				}
			}
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
				if (!editor_stateNames.Contains("IDLE")) Debug.LogWarning("Its recommended that you have a default state called 'IDLE' which can act as a hard reset state for actions like respawning");
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
				if (!editor_stateNames.Contains("IDLE")) Debug.LogWarning("Its recommended that you have a default state called 'IDLE' which can act as a hard reset state for actions like respawning");
			}
	#endif
#endif
		}

		/// <summary>
		/// Handles animation state changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		virtual protected void AnimationStateChanged( object sender, AnimationEventArgs args)
		{
			float normalizedTime = 0.0f;

			if (!ensureStatePlaysForOneFrame || hasPlayed)
			{
				myAnimator.SetInteger("PreviousState", (int)args.PreviousState);
				myAnimator.SetInteger("State", (int)args.State);
				state = args.State;
				queuedState = AnimationState.NONE;
				hasPlayed = false;
				myAnimator.SetBool("HasPlayed", false);
			}
			else
			{
				queuedState = args.State;
			}

			if (args.OverrideState != null && animationStateOverrideLookup.ContainsKey(args.OverrideState) && myAnimator.runtimeAnimatorController != animationStateOverrideLookup[args.OverrideState])
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				myAnimator.runtimeAnimatorController = animationStateOverrideLookup[args.OverrideState];
				myAnimator.SetInteger("PreviousState", (int)state);
				myAnimator.SetInteger("State", (int)state);
				myAnimator.SetBool("HasPlayed", true);

				normalizedTime = (info.length < OneShotLength) ? 0 :  info.normalizedTime;

				if (aimer != null)
				{
					Vector2 aimDirection = aimer.GetAimDirection((Character)myCharacter);
					myAnimator.SetFloat("GunPositionX",aimDirection.x);
					myAnimator.SetFloat("GunPositionY",aimDirection.y);

#if UNITY_EDITOR
	#if UNITY_5
					// In editor mode check that the state is present
					if (myAnimator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
					{
	#else
					// In editor mode check that the state is present
					if (myAnimator.runtimeAnimatorController is UnityEditorInternal.AnimatorController)
					{
	#endif

						if (nonAimingStates.Contains(state.AsString()) && editor_stateNames.Contains(state.AsString()))
					    {
							Debug.LogError("The state " + args.State.AsString() + " is not in the animator, when you use overrides and aimers, state names must be an exact UPPERCASE match for the Animation State. This is being skipped in editor mode but this will lead to errors in release builds.");
							return;
						}
						else if (aimDirection.y > 0 && !editor_stateNames.Contains(state.AsString() +  "_UP"))
						{
							Debug.LogError("The state " + args.State.AsString() + "_UP is not in the animator, when you use overrides and aimers, state names must be an exact UPPERCASE match for the Animation State with suffixes of _UP and _DOWN. This is being skipped in editor mode but this will lead to errors in release builds.");
							return;
						}
						else if (aimDirection.y < 0 && !editor_stateNames.Contains(state.AsString() +  "_DOWN"))
						{
							Debug.LogError("The state " + args.State.AsString() + "_DOWN is not in the animator, when you use overrides and aimers, state names must be an exact UPPERCASE match for the Animation State with suffixes of _UP and _DOWN. This is being skipped in editor mode but this will lead to errors in release builds.");
							return;
						}
						else if (aimDirection.y == 0 && !editor_stateNames.Contains(state.AsString()))
						{

							Debug.LogError("The state " + args.State.AsString() + " is not in the animator, when you use overrides and aimers, state names must be an exact UPPERCASE match for the Animation State. This is being skipped in editor mode but this will lead to errors in release builds.");
							return;
						}
					}
#endif
					if (nonAimingStates.Contains(state.AsString())) myAnimator.Play(state.AsString(), 0, info.normalizedTime);
					else if (aimDirection.y > 0) myAnimator.Play(state.AsString() + "_UP", 0, normalizedTime);
					else if (aimDirection.y < 0) myAnimator.Play(state.AsString() + "_DOWN", 0, normalizedTime);
					else myAnimator.Play(state.AsString(), 0, info.normalizedTime);
				}
				else
				{
#if UNITY_EDITOR
	#if UNITY_5
					// In editor mode check that the state is present
					if (myAnimator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
					{
	#else
						// In editor mode check that the state is present
						if (myAnimator.runtimeAnimatorController is UnityEditorInternal.AnimatorController)
						{
	#endif
						if (!editor_stateNames.Contains(state.AsString()))
						{
							Debug.LogError("The state " + args.State.AsString() + " is not in the animator, when you use overrides state names must be an exact UPPERCASE match for the Animation State. This is being skipped in editor mode but this will lead to errors in release builds.");
							return;
						}
					}
#endif
					myAnimator.Play(state.AsString(), 0, normalizedTime);
				}
				info = myAnimator.GetCurrentAnimatorStateInfo(0);
			}
			else if (args.OverrideState == null && myAnimator.runtimeAnimatorController != defaultController)
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				myAnimator.runtimeAnimatorController = defaultController;
				normalizedTime = (info.length < OneShotLength) ? 0 :  info.normalizedTime;
				if (state != AnimationState.NONE)
				{
					myAnimator.SetInteger("PreviousState", (int)state);
					myAnimator.SetInteger("State", (int)state);
					myAnimator.SetBool("HasPlayed", true);
					if (aimer != null)
					{
						Vector2 aimDirection = aimer.GetAimDirection((Character)myCharacter);
						myAnimator.SetFloat("GunPositionX",aimDirection.x);
						myAnimator.SetFloat("GunPositionY",aimDirection.y);
#if UNITY_EDITOR
	#if UNITY_5
						// In editor mode check that the state is present
						if (myAnimator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
						{
	#else
						// In editor mode check that the state is present
						if (myAnimator.runtimeAnimatorController is UnityEditorInternal.AnimatorController)
						{
	#endif
							if (nonAimingStates.Contains(state.AsString()) && editor_stateNames.Contains(state.AsString()))
							{
								Debug.LogError("The state " + args.State.AsString() + " is not in the animator, when you use overrides and aimers, state names must be an exact UPPERCASE match for the Animation State. This is being skipped in editor mode but this will lead to errors in release builds.");
								return;
							}
							else if (aimDirection.y > 0 && !editor_stateNames.Contains(state.AsString() +  "_UP"))
							{
								Debug.LogError("The state " + args.State.AsString() + "_UP is not in the animator, when you use overrides and aimers, state names must be an exact UPPERCASE match for the Animation State with suffixes of _UP and _DOWN. This is being skipped in editor mode but this will lead to errors in release builds.");
								return;
							}
							else if (aimDirection.y < 0 && !editor_stateNames.Contains(state.AsString() +  "_DOWN"))
							{
								Debug.LogError("The state " + args.State.AsString() + "_DOWN is not in the animator, when you use overrides and aimers, state names must be an exact UPPERCASE match for the Animation State with suffixes of _UP and _DOWN. This is being skipped in editor mode but this will lead to errors in release builds.");
								return;
							}
							else if (aimDirection.y == 0 && !editor_stateNames.Contains(state.AsString()))
							{
								
								Debug.LogError("The state " + args.State.AsString() + " is not in the animator, when you use overrides and aimers, state names must be an exact UPPERCASE match for the Animation State. This is being skipped in editor mode but this will lead to errors in release builds.");
								return;
							}
						}
#endif
						if (nonAimingStates.Contains(state.AsString())) myAnimator.Play(state.AsString(), 0, info.normalizedTime);
						else if (aimDirection.y > 0) myAnimator.Play(state.AsString() + "_UP", 0, normalizedTime);
						else if (aimDirection.y < 0) myAnimator.Play(state.AsString() + "_DOWN", 0, normalizedTime);
						else myAnimator.Play(state.AsString(), 0, normalizedTime);
					}
					else
					{
#if UNITY_EDITOR
	#if UNITY_5
						// In editor mode check that the state is present
						if (myAnimator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
						{
	#else
						// In editor mode check that the state is present
						if (myAnimator.runtimeAnimatorController is UnityEditorInternal.AnimatorController)
						{
	#endif
							if (!editor_stateNames.Contains(state.AsString()))
							{
								Debug.LogError("The state " + args.State.AsString() + " is not in the animator, when you use overrides state names must be an exact UPPERCASE match for the Animation State. This is being skipped in editor mode but this will lead to errors in release builds.");
								return;
							}
						}
#endif
						myAnimator.Play(state.AsString(), 0, normalizedTime);						
					}
				}
			}
		}

		/// <summary>
		/// Handle character respawns by resetting animation state.
		/// </summary>
		virtual protected void HandleRespawned (object sender, CharacterEventArgs e)
		{
			myAnimator.runtimeAnimatorController = defaultController;
			myAnimator.SetInteger("PreviousState", 0);
			myAnimator.SetInteger("State", 0);
			myAnimator.SetBool("HasPlayed", false);
			if (aimer != null)
			{
				myAnimator.SetFloat("GunPositionX",0);
				myAnimator.SetFloat("GunPositionY",0);
			}
			myAnimator.Play ("IDLE");
		}
		


		#endregion
	}
}

