using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleCtrl : MonoBehaviour
{
    public Text _loadingText;
    string baseLoading = "Loading";

    // Start is called before the first frame update
    IEnumerator Start()
    {

        _loadingText.text = baseLoading;
        yield return StartCoroutine(LoadingMainSceneAsync());
        


    }

    #region 씬 로드
    [SerializeField] float _mainSceneProgress = 0;
    public int frameCnt = 0;
    public int bigCnt = 0;
    string t;
    
    bool _isMainSceneReady = false;
    AsyncOperation _mainSceneOperation;

    IEnumerator LoadingMainSceneAsync() {
        yield return new WaitForSeconds(0.5f);
        frameCnt = 0;
        bigCnt = 0;

        _mainSceneOperation = SceneManager.LoadSceneAsync("game");
        _mainSceneOperation.allowSceneActivation = false;

        _mainSceneProgress = 0;
        _isMainSceneReady = false;

        

        while (!_mainSceneOperation.isDone) {
            yield return null;
            frameCnt++;
            _mainSceneProgress = _mainSceneOperation.progress;

            if (_mainSceneProgress >= 0.9f) {
                _isMainSceneReady = true;

                
                _mainSceneOperation.allowSceneActivation = true;
            }

            if(frameCnt % 12 == 0) {
                bigCnt++;
                t = baseLoading;
                for (int i=0;i<bigCnt;i++) {
                    t += ".";
                }

                _loadingText.text = t;
                if (bigCnt > 3)
                    bigCnt = 0;
            }
        }

        // _isMainSceneReady = true;
        // _mainSceneOperation.allowSceneActivation = true;
    }

    #endregion
}
