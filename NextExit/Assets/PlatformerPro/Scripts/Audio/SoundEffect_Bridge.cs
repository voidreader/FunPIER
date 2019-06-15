using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An 'animator' that plays sounds when the character enters certain states.
	/// </summary>
	public class SoundEffect_Bridge : MonoBehaviour, IAnimationBridge
	{
		
		#region members
		
		/// <summary>
		/// Maps from states to animator overrides.
		/// </summary>
		public List<SoundEffectMapping> mappings;
		
		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		protected IMob myCharacter;
		
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
		/// Initialise this animation bridge.
		/// </summary>
		protected void Init()
		{
			// Get character reference
			myCharacter = (IMob) gameObject.GetComponent(typeof(IMob));
			if (myCharacter == null) myCharacter = (IMob) gameObject.GetComponentInParent(typeof(IMob));
			if (myCharacter == null) Debug.LogError ("Sound Effect Animation Bridge unable to find Character or Enemy reference");
			myCharacter.ChangeAnimationState += AnimationStateChanged;
			
		}
		
		/// <summary>
		/// Handles animation state changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		protected void AnimationStateChanged( object sender, AnimationEventArgs args)
		{
			StopAllCoroutines ();
			foreach (SoundEffectMapping se in mappings)
			{
				// Check state and layer matches
				if (se.state == args.State && (se.previousState == AnimationState.NONE || se.previousState == args.PreviousState) && (se.groundLayer == 0 ||  ((1 << myCharacter.GroundLayer) & se.groundLayer) == (1 << myCharacter.GroundLayer)))
				{
					if (se.repeatInterval > 0) StartCoroutine(RepeatSound(se));
					else se.soundEffect.Play ();
				}
			}
		}
		
		/// <summary>
		/// Play the sound at an interval.
		/// </summary>
		/// <param name="effect">Effect to play.</param>
		/// <param name="time">The interval.</param>
		protected IEnumerator RepeatSound(SoundEffectMapping se)
		{
			yield return true;
			se.soundEffect.Play ();
			while (true)
			{
				if (TimeManager.Instance.Paused) 
				{
					yield return true;
				}
				else 
				{
					yield return new WaitForSeconds(se.repeatInterval);
					// Recheck state
					if (!TimeManager.Instance.Paused && se.state == myCharacter.AnimationState  && (se.groundLayer == 0 ||  ((1 << myCharacter.GroundLayer) & se.groundLayer) == (1 << myCharacter.GroundLayer)))
					{
						se.soundEffect.Play ();
					}
				}
			}
		}
		
		#endregion
		
		#region public methods and properties
		
		public void Reset()
		{
			
		}
		
		/// <summary>
		/// Gets the associated animator. Returns null if an animator is not being used.
		/// </summary>
		public Animator Animator
		{
			get {return null; }
		}
		
		#endregion
	}
	
	/// <summary>
	/// Maps ainmation states to sound effects.
	/// </summary>
	[System.Serializable]
	public class SoundEffectMapping
	{
		
		/// <summary>
		/// The animation state.
		/// </summary>
		public AnimationState state;
		
		/// <summary>
		/// The sound effect.
		/// </summary>
		public SoundEffect soundEffect;
		
		/// <summary>
		/// How often we should repeat this sound or 0 for a one shot.
		/// </summary>
		public float repeatInterval;
		
		/// <summary>
		/// Only play this animation if the ground layer is in the layer mask. Note that NOTHING means always match.
		/// </summary>
		public LayerMask groundLayer;
		
		/// <summary>
		/// The required previous state. Use NONE to ignore previous state.
		/// </summary>
		public AnimationState previousState;
	}
	
}