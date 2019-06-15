using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WebObject {
	
	//public delegate void WebDelegate(WebObject result);
	//public event WebDelegate WebEvent = null;
    public System.Action<WebObject> WebEvent;
	
    private Dictionary<string, string> Params = new Dictionary<string, string>();
	//public Dictionary<string, object> Result = null;
    public byte[] Result = null;
    public byte[] Data = null;
    public string ResultText = "";
    /// <summary>
    /// protobuf 사용 여부.
    /// </summary>
    public bool IsUseProtoBuf = false;
    /// <summary>
    /// URL를 강제로 적용할 지 여부.
    /// </summary>
    public bool IsURL = false;
    /// <summary>
    /// 로딩바를 표시할지 여부.
    /// </summary>
    public bool IsShowLoading = true;
    /// <summary>
    /// 강제로 적용할 URL
    /// </summary>
    public string URL = "";
    /// <summary>
    /// 서버로부터 결과를 받은 인자인지 체크합니다.
    /// </summary>
    public bool IsResponse = false;

	public WebObject()
	{
	}
    
    //public WebObject(WebDelegate selector)
    public WebObject(System.Action<WebObject> selector)
    {
        WebEvent = selector;
    }
	
    public void setURL(string url)
    {
        IsURL = true;
        URL = url;
    }

	public void setParam(string key, string value)
	{
		Params[key] = value;
	}

    private void setParam(string key, byte[] value)
    {
        setParam(key, System.Text.Encoding.UTF8.GetString(value));
    }

    public void setCommand(string command)
    {
        setParam("command", command);
    }

    /// <summary>
    /// protobuf 의 request를 등록합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    public void setData<T>(T request)
    {
        IsUseProtoBuf = true;
        //setParam("data", WebConnector.Instance.Serialize<T>(request));
        Data = WebConnector.Instance.Serialize<T>(request);
    }

	public Dictionary<string, string> getParam()
	{
		return Params;
	}

	/// <summary>
	/// kjh::통신 컴엔트 정보. 
	/// </summary>
	/// <returns></returns>
	public string GetCommand()
	{
		return Params["command"].ToString();
	}
    
    public void setResult(WWW www)
    {
        IsResponse = true;
        Result = www.bytes;
        ResultText = www.text;
    }

    /// <summary>
    /// 결과를 리턴합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T getResult<T>()
    {
        return WebConnector.Instance.Deserialize<T>(Result);
    }

    /// <summary>
    /// 서버로부터 받은 Binary 데이터를 그대로 전달합니다.
    /// 서버로부터 받은 정보를 클라이언트에 캐싱할때 사용합니다.
    /// </summary>
    /// <returns></returns>
    public byte[] getBinaryResult()
    {
        return Result;
    }

    public string getTextResult()
    {
        return ResultText;
    }

	public void sendResult()
	{
		if (WebEvent != null && Result != null)
			WebEvent(this);
	}
	
}
