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

    private void Start() {

        _loader.LoadSceneAsync();
        
    }
}
