using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Music player which provides basic operations for playing/pausing/stopping music.
	/// </summary>
	public class MusicPlayer : MonoBehaviour
	{

		/// <summary>
		/// The song data.
		/// </summary>
		[Tooltip ("List of songs to play, relates a name to a clip")]
		public List<SongData> playlist;

		/// <summary>
		/// Should we play the first song straight away?
		/// </summary>
		[Tooltip ("Should we play the first song straight away?")]
		public bool playOnStart;

		/// <summary>
		/// Should we start playing the next song automatically.
		/// </summary>
		 [Tooltip ("Should we start playing the next song as soon as the current finishes?")]
		public bool autoPlayNextSong;

		/// <summary>
		/// Cached reference to the audioSource;
		/// </summary>
		protected AudioSource audioSource;

		/// <summary>
		/// The current song index.
		/// </summary>
		protected int currentSong = 0;

		/// <summary>
		/// How long to wait before playing the next song.
		/// </summary>
		protected const float nextSongDelay = 0.33f;

		#region Unity hooks

		/// <summary>
		/// Unity Start hook.
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
			if (autoPlayNextSong && audioSource.isPlaying && audioSource.time >= audioSource.clip.length) PlayNext();
		}

		#endregion

		virtual protected void Init()
		{
			audioSource = GetComponent<AudioSource>();
			if (audioSource == null)
			{
					Debug.LogWarning ("No audio source found on the music player.");
			}
			else
			{
				audioSource.volume = AudioManager.Instance.MusicVolume;
				if (playOnStart)
				{
					Play ();
				}
				else
				{
					Stop ();
				}
				
			}
		}

		/// <summary>
		/// Updates the volume.
		/// </summary>
		virtual public void UpdateVolume()
		{
			audioSource.volume = AudioManager.Instance.MusicVolume;
		}

		/// <summary>
		/// Play the first song in the playlist.
		/// </summary>
		virtual public void Play()
		{
			if (playlist == null || playlist.Count == 0)
			{
				audioSource.Play();
			}
			else
			{
				audioSource.clip = playlist[0].clip;
				audioSource.Play();
			}
			audioSource.volume = AudioManager.Instance.MusicVolume;
		}

		/// <summary>
		/// Play the specified song.
		/// </summary>
		/// <param name="songName">Song name.</param>
		virtual public void Play(string songName)
		{
			bool songFound = false;
			if (playlist != null &&playlist.Count > 0)
			{
				foreach (SongData s in playlist)
				{
					if (s.name == songName)
					{
						songFound = true;
						audioSource.clip = s.clip;
						audioSource.Play();
					}
				}
			}
			if (!songFound)
			{
				Debug.LogWarning("Song '" + songName + "' not found");
			}
			else
			{
				audioSource.volume = AudioManager.Instance.MusicVolume;
			}
		}

		/// <summary>
		/// Play the next song.
		/// </summary>
		virtual public void PlayNext()
		{
			if (playlist != null && playlist.Count > 0)
			{
				currentSong ++;
				if (currentSong > playlist.Count) currentSong = 0;
				audioSource.clip = playlist[currentSong].clip;
				audioSource.Play();
				audioSource.volume = AudioManager.Instance.MusicVolume;
			}
			else
			{
				audioSource.Play();
				audioSource.volume = AudioManager.Instance.MusicVolume;
			}
		}

		/// <summary>
		/// Pause the music.
		/// </summary>
		virtual public void Pause()
		{
			audioSource.Pause();
		}

		/// <summary>
		/// Stop the music.
		/// </summary>
		virtual public void Stop() 
		{
			audioSource.Stop ();
		}


		/// <summary>
		/// Fade the music out over time.
		/// </summary>
		/// <param name="time">Time taken to fade to 0.</param>
		virtual public void FadeOut(float time)
		{
			StartCoroutine (DoFadeOut (time));
		}

		/// <summary>
		/// Fade the music in over time.
		/// </summary>
		/// <param name="time">Time.</param>
		virtual public void FadeIn(float time)
		{
			StartCoroutine (DoFadeIn (time));
		}

		/// <summary>
		/// Fades out.
		/// </summary>
		virtual protected IEnumerator DoFadeOut(float time)
		{
			float ratePerSecond = audioSource.volume / time;
			while (audioSource.volume > 0)
			{
				audioSource.volume -= ratePerSecond * Time.deltaTime;
				yield return true;
			}
		}

		/// <summary>
		/// Fades in.
		/// </summary>
		virtual protected IEnumerator DoFadeIn(float time)
		{
			float ratePerSecond = (AudioManager.Instance.MusicVolume - audioSource.volume) / time;
			while (audioSource.volume < AudioManager.Instance.MusicVolume )
			{
				audioSource.volume += ratePerSecond * Time.deltaTime;
				if  (audioSource.volume > AudioManager.Instance.MusicVolume) audioSource.volume = AudioManager.Instance.MusicVolume ;
				yield return true;
			}
		}
	}

	/// <summary>
	/// Assocaites a music clip with a name.
	/// </summary>
	[System.Serializable]
	public class SongData
	{
		/// <summary>
		/// The name.
		/// </summary>
		public string name;

		/// <summary>
		/// The clip.
		/// </summary>
		public AudioClip clip;

	}
}