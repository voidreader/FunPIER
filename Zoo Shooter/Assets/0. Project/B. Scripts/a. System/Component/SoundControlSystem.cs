using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.Soundy;


/// <summary>
/// 사운드 컨트롤 시스템 - AudioAssistant와 연계
/// </summary>
public class SoundControlSystem : MonoBehaviour
{
    public static SoundControlSystem main = null;
    const string keyBGM = "KeyBGM_Setting";
    const string keySE = "KeySE_Setting";

    public static bool BGM_Available = false;
    public static bool SE_Available = false;


    private void Awake() {
        main = this;
        LoadData();
    }

    public static void LoadData() {

        BGM_Available = true;
        SE_Available = true;

        if (!PlayerPrefs.HasKey(keyBGM))
            BGM_Available = true;
        else {
            if (PlayerPrefs.GetInt(keyBGM) > 0)
                BGM_Available = true;
            else
                BGM_Available = false;
        }


        if (!PlayerPrefs.HasKey(keySE))
            SE_Available = true;
        else {
            if (PlayerPrefs.GetInt(keySE) > 0)
                SE_Available = true;
            else
                SE_Available = false;
        }
    }

    private void Start() {

        SetAudioAssistant();


    }

    public static void SetAudioAssistant() {
        AudioAssistant.main.BGM_Available = BGM_Available;
        AudioAssistant.main.SE_Available = SE_Available;
    }

    public static void SetBGM(bool flag) {

        if (flag) {
            PlayerPrefs.SetInt(keyBGM, 1);
            AudioAssistant.main.PlayMusic("Main");
        }
        else
            PlayerPrefs.SetInt(keyBGM, 0);

        PlayerPrefs.Save();

        BGM_Available = flag;
        SetAudioAssistant();
    }


    public static void SetSE(bool flag) {

        if (flag) {
            PlayerPrefs.SetInt(keySE, 1);
            SoundyManager.UnmuteAllSounds();
        }
        else {
            PlayerPrefs.SetInt(keySE, 0);
            // mute
            SoundyManager.MuteAllSounds();
            
        }

        PlayerPrefs.Save();

        SE_Available = flag;
        SetAudioAssistant();
    }
}
