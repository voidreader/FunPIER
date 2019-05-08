using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionCtrl : MonoBehaviour
{
    void Awake() {

        Application.targetFrameRate = 30;
        InitScreenResolution();
    }


    /// <summary>
    /// 스크린 해상도 설정 
    /// </summary>
    public void InitScreenResolution() {

        Debug.Log("!!!! InitScreenResolution !!! :: " + Screen.width + " / " + Screen.height);
        float t = (float)Screen.width / (float)Screen.height;

        Debug.Log("!! Aspect Ratio :: " + t);



        if (t < 0.49f) { // 9:16 이상의 세로 해상도들. 

            Debug.Log("This is a portrait tall device");
#if UNITY_ANDROID

            Screen.SetResolution(720, 1280, true);
#endif


        }

        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

}
