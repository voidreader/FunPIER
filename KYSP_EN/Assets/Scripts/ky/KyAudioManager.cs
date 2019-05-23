using UnityEngine;
using System.Collections;

public class KyAudioManager : MonoBehaviour {

	enum FadeState {
		None = 0,
		FadeIn,
		FadeOut
	}

	public void Awake() {
		for (int i = 0; i < AudioElementCount; ++i) {
			GameObject go = new GameObject("audio" + i);
			go.transform.parent = transform;
			mAudioElements[i] = go.AddComponent<KyAudioElement>();
			mAudioElements[i].AudioManager = this;
		}
	}

	public void Update() {

	}

	public AudioSource PlayOneShot(AudioClip clip, Vector3 point, float volume, float pitch) {
		GameObject instance = new GameObject("Audio: " + clip.name);
		instance.transform.position = point;
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.clip = clip;
		source.volume = volume * MasterVolume;
		source.pitch = pitch;
		source.Play();
		Destroy(source, clip.length);
		return source;
	}

	public void PlayOneShot(string name) {
		PlayOneShot(name, 1.0f, 1.0f);
	}

	public void PlayOneShot(string name, float volume, float pitch) {
		DebugUtil.Log("audio play one shot:" + name);
		AudioClip clip = (AudioClip)Resources.Load("GameAudioClips/" + name, typeof(AudioClip));
		if (clip == null) {
			return;
		}
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.clip = clip;
		source.volume = volume * MasterVolume;
		source.pitch = pitch;
		source.Play();
		Destroy(source, clip.length/pitch);
	}

	public void Play(string name, bool looping, float volume, float pitch, float fadetime) {
		KyAudioElement elem = mAudioElements[0];
		elem.Play(name, looping, volume, pitch, fadetime);
	}

	public void Play(string name, bool looping) {
		KyAudioElement elem = mAudioElements[0];
		elem.Play(name, looping, 1.0f, 1.0f, 0.0f);
	}

	public void Pause(bool pause) {
		foreach (KyAudioElement elem in mAudioElements) {
			if (pause) {
				elem.AudioSource.Pause();
			} else {
				elem.AudioSource.Play();
			}
		}
	}

	public void Stop() {
		foreach (KyAudioElement elem in mAudioElements) {
			elem.Stop(DefaultFadeTime);
		}
	}

	public void Stop(float fadetime) {
		foreach (KyAudioElement elem in mAudioElements) {
			elem.Stop(fadetime);
		}
	}

	public void StopAll() {
		Stop(0);
		foreach (AudioSource source in GetComponents<AudioSource>()) {
			Destroy(source);
		}
	}

	public static KyAudioManager Instance {
		get {
			if (mInstance == null) {
				mInstance = new GameObject("KyAudioManager").AddComponent<KyAudioManager>();
			}
			return mInstance;
		}
	}

	public float MasterVolume {
		get { return mMasterVolume; }
		set {
			mMasterVolume = value;
			foreach (KyAudioElement elem in mAudioElements) {
				if (elem.AudioSource != null) {
					elem.AudioSource.volume = mMasterVolume;
				}
			}
			/*AudioSource source = mAudioSources[mMainAudioIndex];
			if (source != null) {
				source.volume = mMasterVolume;
			}*/
		}
	}

	public KyAudioElement[] AudioElements {
		get { return mAudioElements; }
	}

	public const int AudioElementCount = 2;
	public float DefaultFadeTime = 1.0f;

	private float mMasterVolume = 1.0f;
	private KyAudioElement[] mAudioElements = new KyAudioElement[AudioElementCount];

	private static KyAudioManager mInstance;
}
