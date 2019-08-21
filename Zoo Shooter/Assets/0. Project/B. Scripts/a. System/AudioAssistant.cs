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

    

    const float maxMusicVolume = 0.6f;

    public bool BGM_Available {
        get => _BGM_Available;
        set {
            _BGM_Available = value;

            if (_BGM_Available)
                ChangeMusicVolume(maxMusicVolume);
            else
                ChangeMusicVolume(0);

        }


    }
    public bool SE_Available {
        get => _SE_Available;
        set { _SE_Available = value;
            if(_SE_Available) {
                ChangeSFXVolume(1);
            }
            else {
                ChangeSFXVolume(0);
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

    bool _BGM_Available = true;
    bool _SE_Available = true;
    
    internal string currentTrack;
		

    void Awake() {
        main = this;



        AudioSource[] sources = GetComponents<AudioSource>();
        music = sources[0];
        sfx = sources[1];
        loopingSFX = sources[2];
        lowSFX = sources[3];


        StartCoroutine(MixBufferRoutine());

        
    }

    private void Start() {
        SoundControlSystem.LoadData();
        SoundControlSystem.SetAudioAssistant();
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
                music.volume = delay * maxMusicVolume;
                delay -= Time.unscaledDeltaTime;
				yield return 0;
			}
		}
		music.clip = to;
        if (to == null || !BGM_Available) {
            music.Stop();
            yield break;
        }
        delay = 0;
		if (!music.isPlaying) music.Play();
		while (delay < 0.4f) {

            music.volume = delay * maxMusicVolume;
            delay += Time.unscaledDeltaTime;
			yield return 0;
		}

        music.volume = maxMusicVolume;
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
            // Debug.Log("Shot Audio ::" + clip);
            main.sfx.PlayOneShot(sound.clips[Random.Range(0, sound.clips.Count)]);
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



    public void ChangeMusicVolume(float v) {
        music.volume = v;

    }

    public void ChangeSFXVolume(float v) {
        
        sfx.volume = v;
        loopingSFX.volume = v / 2;
        lowSFX.volume = v / 2;
    }

    


    [System.Serializable]
    public class Sound {
        public string name;
        public List<AudioClip> clips = new List<AudioClip>();
    }
}

