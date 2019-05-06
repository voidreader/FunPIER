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


}
