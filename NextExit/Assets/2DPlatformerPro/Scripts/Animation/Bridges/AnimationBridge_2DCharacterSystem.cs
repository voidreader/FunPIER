using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// An animator that plays animations on a SimpleCharacter from the 2D Character System.
	/// </summary>
	public class AnimationBridge_2DCharacterSystem : MonoBehaviour
	{
		/// <summary>
		/// Maps from states to animation names. Anything not in the list will use
		/// the Animation.AsString() method. Animations mapped to an empty state will be ignored.
		/// </summary>
		public List<AnimationMapping> mappings;

		#region members
		
		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		protected IMob myCharacter;
		
		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected CharacterSystem2D.SimpleCharacter myAnimator;
	
		/// <summary>
		/// Cached ditcionary of the mappings.
		/// </summary>
		protected Dictionary<AnimationState, string> mappingLookup;

		#endregion
		
		#region unity hooks
		
		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			// Get character reference
			if (myCharacter == null && transform.parent != null) myCharacter = transform.parent.gameObject.GetComponent<Character>();
			if (myCharacter == null && transform.parent != null) myCharacter = transform.parent.gameObject.GetComponent<Enemy>();
			if (myCharacter == null) myCharacter = gameObject.GetComponent<Character>();
			if (myCharacter == null) myCharacter = gameObject.GetComponent<Enemy>();
			if (myCharacter == null) Debug.LogError ("Platform Animator unable to find Character or Enemy reference");
			myCharacter.ChangeAnimationState += AnimationStateChanged;
			myAnimator = GetComponent<CharacterSystem2D.SimpleCharacter>();
			if (myAnimator == null) Debug.LogError ("Platform Animator unable to find Unity Animator reference");
			// Create a dictionary from the mappings
			mappingLookup = new Dictionary<AnimationState, string>();
			foreach (AnimationState state in System.Enum.GetValues(typeof(AnimationState)))
			{
				string name = null;
				AnimationMapping m = mappings.Where (am=>am.state == state).FirstOrDefault();
				if (m != null) name = m.animationName;
				if (name == null) mappingLookup.Add (state, state.AsString());
				else if (name.Length > 0) mappingLookup.Add (state, name);
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
			if (mappingLookup.ContainsKey(args.State))
			{

				myAnimator.SwitchAnimation(mappingLookup[args.State]);
			}
		}
		
		#endregion
	}
}