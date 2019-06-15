using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An animator that plays animations directly on a mecanim controller. Typically used for 2D sprites.
	/// </summary>
	public class AnimationBridge_DebugLog : MonoBehaviour, IAnimationBridge
	{
		
		protected IMob myCharacter;

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}
		/// <summary>
		/// Unity OnDestory hook.
		/// </summary>
		void OnDestroy()
		{
			if (myCharacter != null) myCharacter.ChangeAnimationState -= AnimationStateChanged;
		}


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

		}

		/// <summary>
		/// Handles animation state changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		protected void AnimationStateChanged( object sender, AnimationEventArgs args)
		{
			if (enabled) Debug.Log (args.State + " (" + args.Priority + ")");
		}
	
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

	}
}