using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageLang : UILayer {

    public List<LangButtonCtrl> _langButtonList;

    public override UILayer Init(UILayerEnum type, Transform parent, Action pOpen, Action pClose) {

        CheckCurrentLang();
        

        return base.Init(type, parent, pOpen, pClose);
    }

    void CheckCurrentLang() {
        for(int i=0; i<_langButtonList.Count;i++) {
            _langButtonList[i].SetCurrentLanguage();
        }
    }


    void GoTitle() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }


    public void OnClickKorean() {
        PierSystem.main.currentLang = SystemLanguage.Korean;
        PierSystem.main.SaveLanguage();
        GoTitle();
    }

    public void OnClickEnglish() {
        PierSystem.main.currentLang = SystemLanguage.English;
        PierSystem.main.SaveLanguage();
        GoTitle();
    }

    public void OnClickJapanese()
    {
        PierSystem.main.currentLang = SystemLanguage.Japanese;
        PierSystem.main.SaveLanguage();
        GoTitle();
    }


}
