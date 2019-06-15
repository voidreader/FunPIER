using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An 'animator' that plays animations when the character enters certain states.
	/// </summary>
	public class ParticleEffect_Bridge : MonoBehaviour, IAnimationBridge
	{
		
		#region members
		
		/// <summary>
		/// Maps from states to animator overrides.
		/// </summary>
		public List <ParticleEffectMapping> mappings;
		
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
			if (myCharacter == null) Debug.LogError ("Particle Effect Animation Bridge unable to find Character or Enemy reference");
			myCharacter.ChangeAnimationState += AnimationStateChanged;
			foreach (ParticleEffectMapping p in mappings)
			{
				if (p.particleSystem == null)
				{
					Debug.LogWarning("No particle system attached");
				}
				else
				{
					if (p.repeatInterval != 0 && p.isStop) Debug.LogWarning("It doesnt make sense for a STOP particle system effect to be repeated");
					if (p.isStop && p.effectType == ParticleEffectType.INSTANTIATE_NEW_INSTANCE) Debug.LogWarning("It doesnt make sense for a STOP particle system effect to also INSTANTIATE_NEW_INSTANCE. ");
					p.offset = p.particleSystem.transform.position - ((Component)myCharacter).transform.position;
				}
			}
		}
		
		/// <summary>
		/// Handles animation state changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		protected void AnimationStateChanged( object sender, AnimationEventArgs args)
		{
			StopAllCoroutines();
			foreach (ParticleEffectMapping p in mappings)
			{
				// Check state and layer matches
				if (p.state == args.State && (p.previousState == AnimationState.NONE || p.previousState == args.PreviousState) && (p.groundLayer == 0 ||  ((1 << myCharacter.GroundLayer) & p.groundLayer) == (1 << myCharacter.GroundLayer)))
				{
					if (p.repeatInterval > 0) StartCoroutine(RepeatEffect(p));
					else 
					{
						DoPlay (p);
					}
				}
			}
		}

		/// <summary>
		/// Plays the particle effect
		/// </summary>
		protected void DoPlay(ParticleEffectMapping p)
		{
			switch (p.effectType)
			{
				case ParticleEffectType.ATTACHED_TO_CHARACTER:
					p.particleSystem.Stop ();
					if (!p.isStop) p.particleSystem.Play ();
					break;
				case ParticleEffectType.DETACHED_FROM_CHARACTER:
					p.particleSystem.transform.parent = null;
					p.particleSystem.transform.position = ((Component)myCharacter).transform.position + new Vector3 (myCharacter.LastFacedDirection * p.offset.x, p.offset.y, p.offset.z);
					p.particleSystem.Stop ();
					if (!p.isStop) p.particleSystem.Play ();
					break;
				case ParticleEffectType.INSTANTIATE_NEW_INSTANCE:
					GameObject go = (GameObject)GameObject.Instantiate(p.particleSystem.gameObject, p.particleSystem.transform.position, p.particleSystem.transform.rotation);
					ParticleSystem newPs = go.GetComponent<ParticleSystem>();
					// NOTE Note we don't pool here or do anything to force this particle system to destroy or disable itself
					if (newPs != null) newPs.Play();
					break;
			}

		}

		/// <summary>
		/// Play the sound at an interval.
		/// </summary>
		protected IEnumerator RepeatEffect(ParticleEffectMapping p)
		{
			yield return true;
			DoPlay (p);
			while (true)
			{
				if (TimeManager.Instance.Paused) 
				{
					yield return true;
				}
				else 
				{
					yield return new WaitForSeconds(p.repeatInterval);
					// Recheck state
					if (!TimeManager.Instance.Paused && p.state == myCharacter.AnimationState  && (p.groundLayer == 0 ||  ((1 << myCharacter.GroundLayer) & p.groundLayer) == (1 << myCharacter.GroundLayer)))
					{
						DoPlay (p);
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
	/// Maps animation states to particle effects.
	/// </summary>
	[System.Serializable]
	public class ParticleEffectMapping
	{
		
		/// <summary>
		/// The animation state.
		/// </summary>
		public AnimationState state;
		
		/// <summary>
		/// The particles.
		/// </summary>
		public ParticleSystem particleSystem;
		
		/// <summary>
		/// How often we should repeat this effect or 0 for a one shot.
		/// </summary>
		public float repeatInterval;
		
		/// <summary>
		/// Only play this effect if the ground layer is in the layer mask. Note that NOTHING means always match.
		/// </summary>
		public LayerMask groundLayer;
		
		/// <summary>
		/// The required previous state. Use NONE to ignore previous state.
		/// </summary>
		public AnimationState previousState;

		/// <summary>
		/// If true instead of starting the referenced system we stop it.
		/// </summary>
		public bool isStop;

		/// <summary>
		/// How does this effect work.
		/// </summary>
		public ParticleEffectType effectType;

		[System.NonSerialized]
		/// <summary>
		/// The offset from character position (used for detaached and instantiated particle effects).
		/// </summary>
		public Vector3 offset;

	}

	/// <summary>
	/// Types of particle effect.
	/// </summary>
	public enum ParticleEffectType
	{
		ATTACHED_TO_CHARACTER,
		DETACHED_FROM_CHARACTER,
		INSTANTIATE_NEW_INSTANCE
	}
}