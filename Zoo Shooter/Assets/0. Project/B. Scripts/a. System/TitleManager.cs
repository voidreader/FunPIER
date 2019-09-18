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


        Debug.Log(">> LoadSceneAsync <<");
        _loader.LoadSceneAsync();

        Debug.Log(">>>>> Start Waiting in IAP Billing");

        // IAP 모듈 초기화 완료 대기 
        while (!IAPControl.IsModuleLoaded)
            yield return null;

        yield return new WaitForSeconds(0.1f);
        Debug.Log(">>>>> End Waiting in IAP Billing");


        Debug.Log(">>>>> SetAllowSceneActivation");
        _loader.SetAllowSceneActivation(true);




    }
}
