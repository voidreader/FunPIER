using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingView : MonoBehaviour {

    public Image _btnBGM, _btnSE;
    public Sprite _spriteOn, _spriteOff;
    public GameObject _btnRestore; // 구매 리스토어 (iOS)

    public void OnView() {

#if UNITY_IOS
        _btnRestore.SetActive(true);
#else
        _btnRestore.SetActive(false);
#endif

        SetStatus();
    }

    void SetStatus() {
        if (SoundControlSystem.BGM_Available)
            _btnBGM.sprite = _spriteOn;
        else
            _btnBGM.sprite = _spriteOff;

        if (SoundControlSystem.SE_Available)
            _btnSE.sprite = _spriteOn;
        else
            _btnSE.sprite = _spriteOff;
    }

    public void OnClickSupport() {
        Application.OpenURL("https://utplusinteractive.freshdesk.com/support/tickets/new");
    }

    public void OnClickSE() {
        SoundControlSystem.SetSE(!SoundControlSystem.SE_Available);
        SetStatus();
    }

    public void OnClickBGM() {
        SoundControlSystem.SetBGM(!SoundControlSystem.BGM_Available);
        SetStatus();
    }

}
