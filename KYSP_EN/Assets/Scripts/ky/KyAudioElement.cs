using UnityEngine;
using System.Collections;

public class KyAudioElement : MonoBehaviour {

	void Awake() {
		mAudioSource = gameObject.AddComponent<AudioSource>();
	}
	
	void Update () {
		if (mFadeRunning) {
			mElapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(mElapsedTime / mFadeTime);
			float volume = Mathf.Lerp(mStartVolume, mEndVolume, t);
			mAudioSource.volume = volume;
			if (t == 1.0f) {
				mFadeRunning = false;
				if (volume == 0.0f && mStopWhenVolumeZero) {
					mAudioSource.Stop();
					mStopWhenVolumeZero = false;
				}
			}
		}
	}

	public void Play(string name, bool looping, float volume, float pitch, float fadetime) {
		if (name != mPlayingAudio) {
			AudioClip clip = (AudioClip)Resources.Load("GameAudioClips/" + name, typeof(AudioClip));
			mAudioSource.clip = clip;
			mAudioSource.pitch = pitch;
			mAudioSource.loop = looping;
			mAudioSource.Play();
			mPlayingAudio = name;
			if (fadetime == 0) {
				mFadeRunning = false;
				mFadeTime = 0;
				mAudioSource.volume = volume * AudioManager.MasterVolume;
			} else {
				mElapsedTime = 0.0f;
				mFadeRunning = true;
				mFadeTime = fadetime;
				mStartVolume = 0;
				mEndVolume = volume * AudioManager.MasterVolume;
				mStopWhenVolumeZero = false;
				mAudioSource.volume = 0;
			}
		} else {
			mAudioSource.volume = volume * AudioManager.MasterVolume;
		}
	}

	public void Stop(float fadetime) {
		mFadeTime = fadetime;
		mElapsedTime = 0.0f;
		if (fadetime == 0.0f) {
			mFadeRunning = false;
			mAudioSource.Stop();
		} else {
			mFadeRunning = true;
			mStartVolume = mAudioSource.volume;
			mEndVolume = 0.0f;
			mStopWhenVolumeZero = true;
		}
		mPlayingAudio = "";
	}

	public KyAudioManager AudioManager {
		get { return mAudioManager; }
		set { mAudioManager = value; }
	}

	public AudioSource AudioSource {
		get { return mAudioSource; }
	}

	private KyAudioManager mAudioManager = null;	//	オーディオマネージャ
	private AudioSource mAudioSource = null;		//	音源コンポーネント
	private float mFadeTime = 0.0f;
	private float mElapsedTime = 0.0f;
	private float mStartVolume = 0.0f;
	private float mEndVolume = 0.0f;
	private bool mFadeRunning = false;
	private bool mStopWhenVolumeZero = false;
	private string mPlayingAudio;
}
