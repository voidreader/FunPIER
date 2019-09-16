using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingView : MonoBehaviour {

    public Image _btnBGM, _btnSE;
    public Sprite _spriteOn, _spriteOff;
    public GameObject _btnRestore; // 구매 리스토어 (iOS)

    // 치트
    public int cheatValue1, cheatValue2, cheatValue3;

    public void OnView() {

#if UNITY_IOS
        _btnRestore.SetActive(true);
#else
        _btnRestore.SetActive(false);
#endif

        SetStatus();
        InitCheatValue();
    }

    #region Cheat 

    void InitCheatValue() {
        cheatValue1 = 0;
        cheatValue2 = 0;
        cheatValue3 = 0;
    }

    void CheatCheck() {
        // 모든 무기 개방 
        if (cheatValue1 == 3 && cheatValue2 == 3 && cheatValue3 == 4) {
            PIER.main.AddEveryGun();
        }


        if (cheatValue1 == 7 && cheatValue3 == 5) {
            PIER.main.ClearLevelCheat();
        }
    }

    public void OnClickCheat1() {
        cheatValue1++;
    }
    public void OnClickCheat2() {
        cheatValue2++;
    }
    public void OnClickCheat3() {
        cheatValue3++;
    }

    #endregion




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

        CheatCheck();

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
