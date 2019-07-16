using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent (typeof (AudioListener))]
[RequireComponent (typeof (AudioSource))]
public class AudioAssistant : MonoBehaviour {

	public static AudioAssistant main;

	AudioSource music;
	AudioSource sfx;
    AudioSource loopingSFX;
    AudioSource lowSFX;

	public float musicVolume {
        get {
            if (!PlayerPrefs.HasKey("Music Volume"))
                return 1f;
            return PlayerPrefs.GetFloat("Music Volume");
        }
        set {
            PlayerPrefs.SetFloat("Music Volume", value);
            PlayerPrefs.Save();

            if (main != null) {

                if (musicVolume <= 0)
                    music.volume = 0;
                else
                    music.volume = 1;
            }
        }
    }

    public float sfxVolume {
        get {
            if (!PlayerPrefs.HasKey("SFX Volume"))
                return 1f;
            return PlayerPrefs.GetFloat("SFX Volume");
        }
        set {
            PlayerPrefs.SetFloat("SFX Volume", value);
            PlayerPrefs.Save();

            if (main != null) {
                if (sfxVolume <= 0) {
                    sfx.volume = 0;
                    loopingSFX.volume = 0;
                    lowSFX.volume = 0;
                }
                else {
                    sfx.volume = 1;
                    loopingSFX.volume = 1;
                    lowSFX.volume = 0.5f;
                }
            }

        }
    }

    public List<Sound> tracks = new List<Sound>();
    public List<Sound> sounds = new List<Sound>();
    Sound GetSoundByName(string name) {
        return sounds.Find(x => x.name == name);
    }

	static List<string> mixBuffer = new List<string>();
	static float mixBufferClearDelay = 0.05f;

    public bool mute = false;
    public bool quiet_mode = false;
    
    internal string currentTrack;
		

    void Awake() {
        main = this;

        AudioSource[] sources = GetComponents<AudioSource>();
        music = sources[0];
        sfx = sources[1];
        loopingSFX = sources[2];
        lowSFX = sources[3];

        // Initialize
        sfxVolume = sfxVolume;
        musicVolume = musicVolume;

        ChangeMusicVolume(musicVolume);
        ChangeSFXVolume(sfxVolume);

        StartCoroutine(MixBufferRoutine());

        mute = PlayerPrefs.GetInt("Mute") == 1;
    }

	// Coroutine responsible for limiting the frequency of playing sounds
	IEnumerator MixBufferRoutine() {
        float time = 0;

		while (true) {
            time += Time.unscaledDeltaTime;
            yield return 0;
            if (time >= mixBufferClearDelay) {
			    mixBuffer.Clear();
                time = 0;
            }
		}
	}


    // Launching a music track
    public void PlayMusic(string trackName, bool loop = true) {

        music.loop = loop;

        if (trackName != "")
            currentTrack = trackName;
		AudioClip to = null;
        foreach (Sound track in tracks)
            if (track.name == trackName)
                to = track.clips[0];
        StartCoroutine(main.CrossFade(to));
	}




    public void StopLoopingSFX() {
        loopingSFX.Stop();
    }

    public void PlayLoopingSFX(string trackName) {

        Sound sound = main.GetSoundByName(trackName);
        loopingSFX.clip = sound.clips[0];
        loopingSFX.loop = true;
        loopingSFX.Play();
    }

	// A smooth transition from one to another music
	IEnumerator CrossFade(AudioClip to) {
		float delay = 0.4f;
		if (music.clip != null) {
			while (delay > 0) {
                music.volume = delay * musicVolume;
                delay -= Time.unscaledDeltaTime;
				yield return 0;
			}
		}
		music.clip = to;
        if (to == null || mute) {
            music.Stop();
            yield break;
        }
        delay = 0;
		if (!music.isPlaying) music.Play();
		while (delay < 0.4f) {

            music.volume = delay * musicVolume;
            delay += Time.unscaledDeltaTime;
			yield return 0;
		}

        music.volume = musicVolume;
    }


    Sound inGameSound;
    public void ShotInGame(string clip, bool force = false) {


        inGameSound = GetSoundByName(clip);

        if (inGameSound != null && !mixBuffer.Contains(clip)) {

            if (inGameSound.clips.Count == 0)
                return;
            mixBuffer.Add(clip);
            sfx.PlayOneShot(inGameSound.clips[0]);
        }
    }

	// A single sound effect
	public static void Shot(string clip) {
        Sound sound = main.GetSoundByName(clip);

        /*
        if (sound != null && !mixBuffer.Contains(clip)) {
            if (sound.clips.Count == 0) return;
			mixBuffer.Add(clip);
            main.sfx.PlayOneShot(sound.clips[0]);
		}
        */

        if (sound != null) {
            if (sound.clips.Count == 0)
                return;

            // mixBuffer.Add(clip);
            main.sfx.PlayOneShot(sound.clips[0]);
        }
    }

    public static void Shot(AudioClip clip) {

        if(clip)
            main.sfx.PlayOneShot(clip);
    }

    public static void LowShot(string clip) {
        Sound sound = main.GetSoundByName(clip);
        if (sound != null) {
            if (sound.clips.Count == 0)
                return;

            // mixBuffer.Add(clip);
            main.lowSFX.PlayOneShot(sound.clips[0]);
        }
    }


    // Turn on/off music
    public void MuteButton() {
        mute = !mute;
        PlayerPrefs.SetInt("Mute", mute ? 1 : 0);
        PlayerPrefs.Save();
        PlayMusic(mute ? "" : currentTrack);
    }

    public void ChangeMusicVolume(float v) {
        musicVolume = v;
        // music.volume = musicVolume * ProjectParameters.main.music_volume_max;
        music.volume = musicVolume;

    }

    public void ChangeSFXVolume(float v) {
        sfxVolume = v;
        sfx.volume = sfxVolume;
        loopingSFX.volume = sfxVolume;
    }

    [System.Serializable]
    public class Sound {
        public string name;
        public List<AudioClip> clips = new List<AudioClip>();
    }
}

