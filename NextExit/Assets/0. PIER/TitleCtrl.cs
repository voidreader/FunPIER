using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleCtrl : MonoBehaviour
{

    public static TitleCtrl main = null;
    public Text _loadingText;
    string baseLoading = "Loading Dungeons";


    // 스테이지 맵 정보
    public List<TextAsset> listDeMaps;
    public List<ArrayList> StageList = new List<ArrayList>();
    bool isLoadingMap = false;


    private void Awake() {
        main = this;
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {

        /*
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
        */



        _loadingText.text = baseLoading;

        // 다음씬 미리 읽어오기 
        StartCoroutine(LoadingMainSceneAsync());

        StartCoroutine(LoadingText());
        




    }

    void SetLoadingText(int p) {
        _loadingText.text = baseLoading + "... " + p.ToString() + "%";
    }


    /// <summary>
    /// 텍스트 뿌려주기 
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadingText() {

        isLoadingMap = false;

        // 맵 데이터 로딩 
        TextAsset map;
        for (int i = 0; i < listDeMaps.Count; i++) {
            map = listDeMaps[i];
            StageList.Add(BlockManager.MapDateToListDecrypted(map.text));

            if(i<=100)
                SetLoadingText(i);

            yield return null;
        }

        isLoadingMap = true;
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

            if(_mainSceneProgress >= 0.9f && isLoadingMap) {
                _isMainSceneReady = true;
                _mainSceneOperation.allowSceneActivation = true;
            }
        }

        // _isMainSceneReady = true;
        // _mainSceneOperation.allowSceneActivation = true;

        /*
        while(!isLoadingMap) {
            yield return null;
        }
        */


        
    }

    #endregion
}
