using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBoxCtrl : MonoBehaviour {


    [SerializeField]
    tk2dTiledSprite _letterBox;

    void Start() {
        SetSize();
    }

    public void SetSize() {

        Debug.Log("SetSize!");
        _letterBox.dimensions.Set(Screen.width, Screen.height);
    }
}

