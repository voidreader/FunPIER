using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageOption : UILayer {

    public GameObject _sfxInactive, _bgmInactive;
    public UILabel _lblSFX, _lblBGM;

    public GameObject _btnRestore;

    public int lockCount = 0;


    public override UILayer Init(UILayerEnum type, Transform parent, Action pOpen, Action pClose) {

        _btnRestore.SetActive(false);
#if UNITY_IOS
        
        _btnRestore.SetActive(true);
#endif


        CheckVolumeState();

        lockCount = 0;

        return base.Init(type, parent, pOpen, pClose);
    }


    /// <summary>
    /// 볼륨 상태 처리 
    /// </summary>
    void CheckVolumeState() {
        
        // 효과음
        if(AudioAssistant.main.sfxVolume > 0) {
            _lblSFX.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT11);
            _sfxInactive.SetActive(false);
        }
        else {
            _lblSFX.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT12);
            _sfxInactive.SetActive(true);
        }

        // 배경음 
        if (AudioAssistant.main.musicVolume > 0) {
            _lblBGM.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT13);
            _bgmInactive.SetActive(false);
        }
        else {
            _lblBGM.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT14);
            _bgmInactive.SetActive(true);
        }
    }


    public void OnClickLang() {
        AudioAssistant.Shot("Possitive");
        PageManager.main.OpenLang();
    }

    public void OnClickTutorial() {
        AudioAssistant.Shot("Possitive");
        PageManager.main.OpenTutorial();

        if(lockCount == 7) {
            UnlockContents();
        }

    }


    public void OnClickSFX() {
        AudioAssistant.Shot("Possitive");
        if (AudioAssistant.main.sfxVolume > 0) {
            AudioAssistant.main.sfxVolume = 0;
        }
        else {
            AudioAssistant.main.sfxVolume = 1;
        }

        CheckVolumeState();
    }

    public void OnClickBGM() {
        AudioAssistant.Shot("Possitive");
        if (AudioAssistant.main.musicVolume > 0) {
            AudioAssistant.main.musicVolume = 0;
        }
        else {
            AudioAssistant.main.musicVolume = 1;
        }

        CheckVolumeState();
    }

    public void OnClickPolicy() {
        AdsControl.main.IsCoolingPauseAds = true;
        Application.OpenURL("http://pier-showcase.com/policy/m2048_privacy_policy.html");
    }


    public void OnClickTitle() {
        lockCount++;


    }

    /// <summary>
    /// 
    /// </summary>
    public void UnlockContents() {
        Debug.Log(">> UnlockContents <<");

        /*
        carMuseumStep = node["carstep"].AsInt;
        wineMuseumStep = node["winestep"].AsInt;
        vikingMuseumStep = node["vikingstep"].AsInt;
        iceMuseumStep = node["icestep"].AsInt;
        */

        PierSystem.main.carMuseumStep = PierSystem.main.MaxCarMuseumStep;
        PierSystem.main.wineMuseumStep = PierSystem.main.MaxWineMuseumStep;
        PierSystem.main.vikingMuseumStep = PierSystem.main.MaxVikingMuseumStep;
        PierSystem.main.iceMuseumStep = PierSystem.main.MaxIceMuseumStep;

        PierSystem.main.SaveProfile();
    }
}
