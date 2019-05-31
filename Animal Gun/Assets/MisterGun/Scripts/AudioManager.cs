using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; set; }

    public AudioClip Headshot;

    public AudioSource SoundSource;
    public AudioSource MusicSource;

    public Sprite[] SoundsSprites;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        SoundSource.PlayOneShot(clip);
    }

    public void StopAudio()
    {
        SoundSource.Stop();
    }

    public void SoundButton(Image CurrentImage)
    {
        if (PlayerPrefs.GetInt("Sounds") == 0)
        {
            PlayerPrefs.SetInt("Sounds", 1);
            SoundSource.mute = true;
            MusicSource.mute = true;
            CurrentImage.sprite = SoundsSprites[PlayerPrefs.GetInt("Sounds")];
        }
        else if (PlayerPrefs.GetInt("Sounds") == 1)
        {
            PlayerPrefs.SetInt("Sounds", 0);
            SoundSource.mute = false;
            MusicSource.mute = false;
            CurrentImage.sprite = SoundsSprites[PlayerPrefs.GetInt("Sounds")];
        }
    }
}
