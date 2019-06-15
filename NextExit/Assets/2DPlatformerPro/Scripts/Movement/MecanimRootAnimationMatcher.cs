#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Matches character position to the root animators position.
	/// </summary>
	public class MecanimRootAnimationMatcher : MonoBehaviour
	{
		public Character character;

		protected Animator myAnimator;

		void Start()
		{
			myAnimator = GetComponent<Animator> ();
			if (myAnimator == null) Debug.LogWarning("MecanimRootAnimationMatcher couldn't find an Animator.");
		}

		// Update is called once per frame
		void Update ()
		{
			// Only do this if applying root motion, this way we can combine both animation and code driven animations
			if (myAnimator.applyRootMotion)
			{
				Vector3 diff = transform.position - character.Transform.position;
				character.Translate (diff.x, diff.y, true);
				transform.localPosition = Vector3.zero;
			}
		}
	}
}