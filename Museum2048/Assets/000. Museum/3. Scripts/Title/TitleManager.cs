using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {

    public GameObject copyright;
    public UISprite LocalizedTitleSprite;
    bool isTallScreen = false;

    // Start is called before the first frame update
    void Start() {

        SetAppsFlyer();


        float screenw = (float)Screen.width;
        float screenh = (float)Screen.height;

        float ratio = screenw / screenh;

        isTallScreen = false;
        if (ratio < 0.56f)
            isTallScreen = true;


        if (isTallScreen)
            copyright.transform.localPosition = new Vector3(0, -710, 0);
        else
            copyright.transform.localPosition = new Vector3(0, -620, 0);


        //언어 설정이 아직 안된경우. (첫실행)
        if (!ES2.Exists(ConstBox.KeySavedLang))
        {

            if (Application.systemLanguage == SystemLanguage.Korean)
                LocalizedTitleSprite.spriteName = "title_kor";
            else if (Application.systemLanguage == SystemLanguage.Japanese)
                LocalizedTitleSprite.spriteName = "title_jp";
            else
                LocalizedTitleSprite.spriteName = "title_eng";
        }
        else
        {
            if (PierSystem.LoadLanguage() == SystemLanguage.Korean)
                LocalizedTitleSprite.spriteName = "title_kor";
            else if (PierSystem.LoadLanguage() == SystemLanguage.Japanese)
                LocalizedTitleSprite.spriteName = "title_jp";
            else
                LocalizedTitleSprite.spriteName = "title_eng";
        }


        StartCoroutine(TitleRoutine());

        ScissorCtrl.Instance.UpdateResolution();
    }

    IEnumerator TitleRoutine() {

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Main");
    }

    void SetAppsFlyer() {



    }
}
