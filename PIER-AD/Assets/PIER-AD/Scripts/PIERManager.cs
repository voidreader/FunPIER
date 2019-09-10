using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using SimpleJSON;
using System;
using System.Text;
using System.IO;
using PIERAD;



public enum RequestID {
    request_version, // 버전 정보 조회 
    request_allsquaregifs // 사각 GIF 불러오기
}



public class PIERManager : MonoBehaviour {

    public PIERManager main = null;

    public AnimatedGifPlayer SquarePlayer;

    #region 통신 변수 
    [SerializeField] string _serverURL = "http://www.pier-showcase.com:8125/";
    HTTPRequest _previousRequest = null;
    private Hashtable _header = new Hashtable();
    private string _requestURL = null;

    [SerializeField] string _data; // 실제 서버로 날릴 string data 
    [SerializeField] JSONNode _dataForm; // 기초 폼 JsonNode
    [SerializeField] int ADVersion = 0;

    readonly string DATA = "data";
    readonly string KeyVersionPostFix = "ADVersion";
    readonly string KeySquarePostFix = "SQUAREGIF";

    public System.Action<JSONNode> PostCallback = delegate { };

    public JSONNode NodeSquareGIF; // 사각 GIF 노드
    public JSONNode NodeInterstitial; // 전면배너 노드 


    [SerializeField] string DebugSquareGIF;

    #endregion

    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 첫 실행 순서
        // 버전체크 → 사각 GIF 데이터 받아오기
        // 이미 다운받은 순서가 있으면 로컬데이터 불러오기 
        ADVersion = LoadClientVersion(); // 

        GetServerVersion();
    }


    /// <summary>
    /// 로컬에 저장된 데이터 조회 
    /// </summary>
    void GetADLocalData() {

        Debug.Log(">> GetADLocalData <<");

        try {
            // 사각 GIF 조회
            NodeSquareGIF = JSON.Parse(PlayerPrefs.GetString(Application.identifier + KeySquarePostFix));
            DebugSquareGIF = NodeSquareGIF.ToString();


            PickSqureGIF();
        }
        catch(Exception e) {
            // ... 예외 발생 처리 

            Debug.Log(">> GetADLocalData Exception :: " + e.StackTrace);
        }
    }



    /// <summary>
    /// 재생할 사각 GIF 픽
    /// </summary>
    public void PickSqureGIF() {
        //File.Exists(Application.persistentDataPath + "/" + NodeSquareGIF[i]["name"] + ".gif")
        int rand = UnityEngine.Random.Range(0, 1000);
        for(int i=0; i<NodeSquareGIF.Count;i++) {

            if (rand >= NodeSquareGIF[i]["ratemin"].AsInt && rand < NodeSquareGIF[i]["ratemax"].AsInt
                && File.Exists(Application.persistentDataPath + "/" + NodeSquareGIF[i]["name"])) {
                PlaySquareGIF(Application.persistentDataPath + "/" + NodeSquareGIF[i]["name"]);
            }


        }

    }

    /// <summary>
    /// 사각 GIF 재생 
    /// </summary>
    /// <param name="filepath"></param>
    public void PlaySquareGIF(string filepath) {
        SquarePlayer.gameObject.SetActive(true);
        SquarePlayer.FileName = filepath;
        SquarePlayer.Init();

    }
    
    
    #region 버전 체크

    /// <summary>
    /// 서버 버전 불러오기(통신)
    /// </summary>
    public void GetServerVersion() {
        Post(RequestID.request_version, OnCompleteVersionCheck, null);
    }


    /// <summary>
    /// 버전 체크 후 다운로드 여부 체크 
    /// </summary>
    /// <param name="result"></param>
    void OnCompleteVersionCheck(JSONNode result) {

        // 서버 버전이 높을때만 다운로드 진행 
        if (ADVersion >= result["version"].AsInt) {
            GetADLocalData();
            return;
        } // 로컬 처리 종료

        Debug.Log("Server version is higher than Client. Downloading starts");
        SaveClientVersion(result["version"].AsInt);

        GetSquareGifs();

    }


    int LoadClientVersion() {
        return PlayerPrefs.GetInt(Application.identifier + KeyVersionPostFix, 0);
    }

    void SaveClientVersion(int newver) {

        ADVersion = newver;
        PlayerPrefs.SetInt(Application.identifier + KeyVersionPostFix, ADVersion);
        PlayerPrefs.Save();

    }

    #endregion

    #region SQURE GIF 통신

    /// <summary>
    /// 모든 사각 Gifs 가져오기
    /// </summary>
    public void GetSquareGifs() {
        Post(RequestID.request_allsquaregifs, OnCompletedLoadingSquareGifs);
    }

    /// <summary>
    /// 리스트를 받아오고 다운로드 시작 
    /// </summary>
    /// <param name="node"></param>
    void OnCompletedLoadingSquareGifs(JSONNode node) {

        Debug.Log("OnCompletedLoadingSquareGifs : " + node.Count);

        for(int i=0; i<node.Count;i++) {

            // 이미 다운받은 파일인지 체크 
            if(File.Exists(Application.persistentDataPath + "/" + NodeSquareGIF[i]["name"])) {
                continue;
            }

            HTTPRequest req = new HTTPRequest(new Uri(node[i]["url"].Value), OnSquareGIFDownload);
            req.Tag = i;
            req.ConnectTimeout = TimeSpan.FromSeconds(30);
            req.Timeout = TimeSpan.FromSeconds(60);
            req.Send();
        }

        // 로컬로 저장 
        PlayerPrefs.SetString(Application.identifier + KeySquarePostFix, node.ToString());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 스퀘어 GIF 로컬 저장 
    /// </summary>
    /// <param name="req"></param>
    /// <param name="resp"></param>
    void OnSquareGIFDownload(HTTPRequest req, HTTPResponse resp) {
        if(req.State != HTTPRequestStates.Finished) {
            Debug.Log("Request Square GIF Download Fail : " + req.Tag.ToString() );
            return;
        }


        
        int dataIndex = (int)req.Tag;

        if (File.Exists(Application.persistentDataPath + "/" + NodeSquareGIF[dataIndex]["name"])) {
            Debug.Log("GIF Downloaded, " + NodeSquareGIF[dataIndex]["name"] + ".gif exists in storage");
            return;
        }

        byte[] bytes = resp.Data;

        // 서버에서 받아와야하는 데이터는 무조건 gif.. 
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + NodeSquareGIF[dataIndex]["name"], bytes);

    }


    /// <summary>
    /// 로컬에 저장된 사각 배너 불러오기 
    /// </summary>
    void LoadLocalSquareGIF() {
        Debug.Log(">>>>>> LoadLocalSquareGIF");
    }

    #endregion

    #region 서버 통신 


    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    void OnFinishedPost(HTTPRequest request, HTTPResponse response) {
        if (!CheckRequestState(request, response)) {
            return;
        }

        RequestID requestID = (RequestID)System.Enum.Parse(typeof(RequestID), request.Tag.ToString());
        JSONNode result = JSON.Parse(response.DataAsText);
        Debug.Log(">>> Finished Post :: " + result.ToString());

        result = result["data"];

        switch (requestID) {
            case RequestID.request_allsquaregifs:
                NodeSquareGIF = result;
                PostCallback(result);
                break;

            case RequestID.request_version:
                PostCallback(result);
                break;
        }

    }

    /// <summary>
    /// 통신
    /// </summary>
    /// <param name="pID"></param>
    /// <param name="pCallback"></param>
    /// <param name="pNode"></param>
    public void Post(RequestID pID, System.Action<JSONNode> callback, JSONNode pNode = null) {

        if (Application.internetReachability == NetworkReachability.NotReachable) {
            Debug.Log("No Access");
            return;
        }

        _dataForm = GetEmptyNode();
        _dataForm["cmd"] = pID.ToString();
        _requestURL = _serverURL + "com";
        PostCallback = callback;

        _dataForm[DATA]["id"] = Application.identifier;

        switch (pID) {
            case RequestID.request_allsquaregifs:
                
                break;
        }


        _data = _dataForm.ToString();
        HTTPRequest request = new HTTPRequest(new System.Uri(_requestURL), HTTPMethods.Post, OnFinishedPost);
        
        request.SetHeader("Content-Type", "application/json; charset=UTF-8");
        request.RawData = Encoding.UTF8.GetBytes(_data);

        request.ConnectTimeout = System.TimeSpan.FromSeconds(10);
        request.Timeout = System.TimeSpan.FromSeconds(30);
        request.Tag = pID;
        request.DisableRetry = false;
        _previousRequest = request;

        request.Send();
    }


    /// <summary>
    /// 타임아웃 체크
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    bool CheckRequestState(HTTPRequest request, HTTPResponse response) {

        if ((request.State == HTTPRequestStates.ConnectionTimedOut || request.State == HTTPRequestStates.TimedOut
            || request.State == HTTPRequestStates.Error || request.State == HTTPRequestStates.Aborted)
            || !response.IsSuccess) {

            Debug.Log("request state :: " + request.State);

            return false;
        }

        return true;
    }




    /// <summary>
    /// 빈 Node 생성 
    /// </summary>
    /// <returns></returns>
    JSONNode GetEmptyNode() {
        return JSON.Parse("{}");
    }


    #endregion
}

