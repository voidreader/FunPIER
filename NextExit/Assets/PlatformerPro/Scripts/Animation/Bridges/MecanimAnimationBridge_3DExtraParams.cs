using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// An animator that sends more parameters to the mecanim controller than the standard bridge. This is primarily
	/// here as an example of how you can extend the Mecanim 3D bridge.
	/// </summary>
	public class MecanimAnimationBridge_3DExtraParams : MecanimAnimationBridge_3D
	{
		/// <summary>
		/// Cached copy of character health. Or null if no character health found.
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init()
		{
			base.Init ();
			if (myCharacter is Character)
			{
				characterHealth = ((Character)myCharacter).GetComponentInChildren<CharacterHealth>();
			}
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		override protected void ActualUpdate()
		{
			base.ActualUpdate ();
			myAnimator.SetFloat("SlopeRotation", myCharacter.SlopeActualRotation);
			if (characterHealth != null)
			{
				// Percentage health we could for example use this to blend in limping animations as the character takes damage.
				myAnimator.SetFloat("Health", characterHealth.CurrentHealthAsPercentage);
			}
		}
	
	}
}
