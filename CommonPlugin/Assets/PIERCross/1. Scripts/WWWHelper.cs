using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using BestHTTP;
using System.Text;

public enum RequestID {
    request_crossinfo // 크로스 마케팅 정보 
}


public class WWWHelper : MonoBehaviour {

    public static WWWHelper main = null;

    public System.Action<JSONNode> PostCallback = delegate { };

    string _serverURL = "http://www.pier-showcase.com:8125/";

    HTTPRequest _previousRequest = null;
    private Hashtable _header = new Hashtable();
    private string _requestURL = null;
    [SerializeField] string _data; // 실제 서버로 날릴 string data 
    [SerializeField] JSONNode _dataForm; // 기초 폼 JsonNode

    readonly string DATA = "data";


    void Awake() {
        main = this;


        _header.Add("Content-Type", "application/json; charset=UTF-8");
    }

    void Start() {
        
    }

    /// <summary>
    /// 통신
    /// </summary>
    /// <param name="pID"></param>
    /// <param name="pCallback"></param>
    /// <param name="pNode"></param>
    public void Post(RequestID pID, System.Action<JSONNode> callback, JSONNode pNode=null) {


        if (Application.internetReachability == NetworkReachability.NotReachable)
            return;

        _dataForm = GetEmptyNode();
        _dataForm["cmd"] = pID.ToString();
        _requestURL = _serverURL + "com";
        PostCallback = callback;

        switch(pID) {
            case RequestID.request_crossinfo:
                _dataForm[DATA]["id"] = Application.identifier;
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

        switch(requestID) {
            case RequestID.request_crossinfo:
                PostCallback(result);
                break;
        }

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


}
