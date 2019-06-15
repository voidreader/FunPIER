using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Pauses animation when the TimeManager pauses (regardless of time scale).
	/// </summary>
	[RequireComponent (typeof(Animator))]
	public class AnimationPauser : MonoBehaviour 
	{
		protected Animator myAnimator;

		void Start()
		{
			Init ();
		}

		void OnDestroy()
		{
			DoDestroy ();
		}

		protected void Init()
		{
			myAnimator = GetComponent<Animator> ();
			TimeManager.Instance.GamePaused += HandleGamePaused;
			TimeManager.Instance.GameUnPaused += HandleGameUnPaused;
		}

		protected void DoDestroy()
		{
			if (TimeManager.SafeInstance != null) TimeManager.SafeInstance.GamePaused -= HandleGamePaused;
			if (TimeManager.SafeInstance != null) TimeManager.SafeInstance.GameUnPaused -= HandleGameUnPaused;
		}


		void HandleGameUnPaused (object sender, System.EventArgs e)
		{
			myAnimator.StopPlayback ();
		}

		void HandleGamePaused (object sender, System.EventArgs e)
		{
			myAnimator.StartPlayback ();
		}

	}
}