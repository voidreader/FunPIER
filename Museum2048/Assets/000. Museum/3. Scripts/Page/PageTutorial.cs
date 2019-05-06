using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTutorial : UILayer {

    public int _tutorialIndex = 0;
    public UISprite _image;
    public GameObject _btnLeft, _btnRight;

    public override UILayer Init(UILayerEnum type, Transform parent, Action pOpen, Action pClose) {


        CheckSideButton();
        SetTutorialImage();

        return base.Init(type, parent, pOpen, pClose);
    }



    public void OnClickLeft() {

        AudioAssistant.Shot("Possitive");

        _tutorialIndex = 0;
        CheckSideButton();
        SetTutorialImage();
    }

    public void OnClickRight() {

        AudioAssistant.Shot("Possitive");

        _tutorialIndex = 1;
        CheckSideButton();
        SetTutorialImage();
    }

    void SetTutorialImage() {
        if (_tutorialIndex == 0)
            _image.spriteName = "tut_block";
        else
            _image.spriteName = "tut_redmoon";

    }

    void CheckSideButton() {

        _btnLeft.SetActive(false);
        _btnRight.SetActive(false);

        if (_tutorialIndex == 0)
            _btnRight.SetActive(true);
        else
            _btnLeft.SetActive(true);

    }

}
