using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WebDownloader : RPGSingleton<WebDownloader>
{

    /*
	// Use this for initialization
	void Start () {
	
	}
	*/
    /*
	// Update is called once per frame
	void Update () {
	
	}
    */
    public class WebDownloaderObject
    {
        public System.Action<WebDownloaderObject> WebEvent;

        byte[] m_ResultBytes = null;
        string m_ResultString;
        public string Filename { get; set; }

        public WebDownloaderObject(System.Action<WebDownloaderObject> selector)
        {
            WebEvent = selector;
        }

        public void setResult(WWW www)
        {
            m_ResultBytes = www.bytes;
            m_ResultString = www.text;
        }

        /// <summary>
        /// 서버로부터 받은 www.bytes를 전달합니다.
        /// </summary>
        /// <returns></returns>
        public byte[] getBytes()
        {
            return m_ResultBytes;
        }

        /// <summary>
        /// 서버로부터 받은 www.text를 전달합니다.
        /// </summary>
        /// <returns></returns>
        public string getString()
        {
            return m_ResultString;
        }

        public void sendResult()
        {
            if (WebEvent != null)
                WebEvent(this);
        }
    }

    private string SERVER_URL = "";
    List<WebDownloaderObject> m_request_list = new List<WebDownloaderObject>();

    //private Dictionary<string, string> staticParams = new Dictionary<string, string>();

    public override void Init()
    {
        base.Init();
        m_request_list.Clear();
    }

    public void setServerUrl(string url)
    {
        SERVER_URL = url;
    }

    /// <summary>
    /// 서버로 전송.
    /// </summary>
    /// <param name="obj">Object.</param>
    public void request(WebDownloaderObject obj)
    {
        m_request_list.Add(obj);
        if (m_request_list.Count == 1)
            StartCoroutine(requestWithWebObject(obj));
    }

    public void request(string filename, System.Action<WebDownloaderObject> selector)
    {
        WebDownloaderObject wdo = new WebDownloaderObject(selector);
        wdo.Filename = filename;
        request(wdo);
    }

    /// <summary>
    /// 다음 전송할 명령이 있는지 체크하여 전송.
    /// </summary>
    private void nextRequest()
    {
		Debug.Log( "m_request_list.Count : " + m_request_list.Count );
        if (m_request_list.Count > 0)
            m_request_list.RemoveAt(0);
        if (m_request_list.Count > 0)
        {
            WebDownloaderObject obj = m_request_list[0];
            StartCoroutine(requestWithWebObject(obj));
        }
		//else

    }

    private IEnumerator requestWithWebObject(WebDownloaderObject obj)
    {
        // 로딩 시작.
        //SYLoadingManager.Instance.startLoading ();
        string ServerURL = SERVER_URL + obj.Filename;

        Debug.Log("requestWithWebObject : " + ServerURL);

        using (WWW www = new WWW(ServerURL))
        {
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                //Debug.LogError(string.Format("request error!\n{0}", www.error));
                Debug.Log(string.Format("request error!\n{0}", www.error));
                // reTry
                showReTryPopup();
            }
            else if (string.IsNullOrEmpty(www.text))
            {
                //Debug.LogError("www.text is null");
                Debug.Log("www.text is null");
                // reTry
                showReTryPopup();
            }
            else
            {
                Debug.Log(www.text);
                obj.setResult(www);
                obj.sendResult();
                // 로딩 끝.
                //SYLoadingManager.Instance.endLoading();
                // 다음 명령이 대기중인지 체크.
                nextRequest();
            }
        }
    }

    /// <summary>
    /// 접속 재시도.
    /// 현재 전송내용을 다시 전송합니다.
    /// </summary>
    private void reTryRequest()
    {
        WebDownloaderObject obj = m_request_list[0];
        StartCoroutine(requestWithWebObject(obj));
    }

    /// <summary>
    /// 서버와의 통신을 재시도 합니다.
    /// </summary>
    void showReTryPopup()
    {
        MessageBox box = MessageBox.show();
        box.setMessage("Re Try?");
        box.addDoneButton((b) =>
        {
            reTryRequest();
            b.Close();
        });
    }


    


}
