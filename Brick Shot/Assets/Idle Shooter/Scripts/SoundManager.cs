using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

    public static SoundManager _instance;
    public bool hasSound = true;

    public Image soundImage;
    public Sprite spSound, spNoSound;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        if (PlayerPrefs.GetInt("SOUND") == 1) { // 0 = has sound, 1 = no sound
            hasSound = false;
            soundImage.sprite = spNoSound;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TurnOnOffSound() {
        if (hasSound) {
            hasSound = false;
            soundImage.sprite = spNoSound;
            PlayerPrefs.SetInt("SOUND", 1);
        } else {
            hasSound = true;
            soundImage.sprite = spSound;
            PlayerPrefs.SetInt("SOUND", 0);
        }
    }
}
