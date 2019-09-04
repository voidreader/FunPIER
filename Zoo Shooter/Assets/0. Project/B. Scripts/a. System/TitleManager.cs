using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.Progress;
using Doozy.Engine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    public Progressor _progressor;

    public SceneLoader _loader;

    private void Awake() {
        Application.targetFrameRate = 60;
        // Screen.SetResolution(720, 1280, true);
    }

    IEnumerator Start() {

        Debug.Log(">>>>> Start Waiting in IAP Billing");

        // IAP 모듈 대기 
        while (!IAPControl.IsModuleLoaded)
            yield return null;


        Debug.Log(">>>>> End Waiting in IAP Billing");

        _loader.LoadSceneAsync();
        
    }
}
