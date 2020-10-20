using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {

    public GameObject copyright;
    public UISprite LocalizedTitleSprite;
    bool isTallScreen = false;
    public SystemLanguage titleLang = SystemLanguage.Korean;

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

            Debug.Log("Application.systemLanguage : " + Application.systemLanguage.ToString());


            if (Application.systemLanguage == SystemLanguage.Korean)
                LocalizedTitleSprite.spriteName = "title_kor";
            else if (Application.systemLanguage == SystemLanguage.Japanese)
                LocalizedTitleSprite.spriteName = "title_jp";
            else
                LocalizedTitleSprite.spriteName = "title_eng";

            if (Application.systemLanguage == SystemLanguage.Korean)
                titleLang = SystemLanguage.Korean;
            else if (Application.systemLanguage == SystemLanguage.Japanese)
                titleLang = SystemLanguage.Japanese;
            else
                titleLang = SystemLanguage.English;

            PierSystem.SaveLanguage(titleLang.ToString());

        }
        else
        {
            Debug.Log("LoadLanguage : " + PierSystem.LoadLanguage());

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
