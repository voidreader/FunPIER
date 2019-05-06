using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LangButtonCtrl : MonoBehaviour
{
    public SystemLanguage _lang;
    public GameObject _activeSign;

    public void SetCurrentLanguage() {

        if (_lang == PierSystem.main.currentLang)
            _activeSign.SetActive(true);
        else
            _activeSign.SetActive(false);

    }
}
