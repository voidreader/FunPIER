using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Simple wrapper for sound effects.
	/// </summary>
	public class SoundEffect : MonoBehaviour
	{
		/// <summary>
		/// Multiply source volume?
		/// </summary>
		[Tooltip ("If true the sources current volume will be multiplied by the AudioManager SFXVolume to make the final volume. Allows easy tweaking of clip volumes. ")]
		public bool multiplyVolume = true;

		/// <summary>
		/// The audio clip to play.
		/// </summary>
		[Tooltip ("The audio clip to play. If null the clip from the audio source will be used")]
		public AudioClip clip;
		
		/// <summary>
		/// Should we play the sound straight away?
		/// </summary>
		[Tooltip ("Should we play the sound straight away?")]
		public bool playOnStart;

		/// <summary>
		/// Store the original volume of the clip;
		/// </summary>
		protected float originalVolume;

		/// <summary>
		/// Cached reference to the audioSource;
		/// </summary>
		protected AudioSource audioSource;

		#region Unity hooks
		
		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start() 
		{
			Init ();
		}
		#endregion

		virtual protected void Init()
		{
			audioSource = GetComponent<AudioSource>();
			if (audioSource == null)
			{
				Debug.LogWarning ("No audio source found on the sound effect.");
			}
			else
			{
				originalVolume = audioSource.volume;
				if (multiplyVolume)
				{
					audioSource.volume = originalVolume * AudioManager.Instance.SfxVolume;
				}
				else
				{
					audioSource.volume = AudioManager.Instance.SfxVolume;
				}
				if (playOnStart)
				{
					Play ();
				}
			}
		}

		/// <summary>
		/// Updates the volume.
		/// </summary>
		virtual public void UpdateVolume()
		{
			if (multiplyVolume)
			{
				audioSource.volume = originalVolume * AudioManager.Instance.SfxVolume;
			}
			else
			{
				audioSource.volume = AudioManager.Instance.SfxVolume;
			}
		}

		/// <summary>
		/// Play the effect.
		/// </summary>
		virtual public void Play()
		{
			if (clip != null && audioSource.clip != clip) 
			{
				audioSource.Stop ();
				audioSource.clip = clip;
			}
			audioSource.Play();
		}

	}
}