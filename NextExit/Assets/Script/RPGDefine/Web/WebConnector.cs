using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class WebConnector : RPGSingleton<WebConnector>
{

    /*
     * 사용방법 예)
     * 
     * WebConnector.Instance
     * setIsDebug : 디버깅모드로 작동할지 여부를 결정한다. 디버깅모드에서는 로그가 남는다.
     * setServerUrl : 서버의 주소를 등록한다.
     * CommonRequest : 서버로 항상 보내질 파라미터를 정의한다.
     * 위의 정보를 한번만 등록해두면 된다. 통신할때마다 적을 필요 없음.
     * 
     * 
     * WebObject
     * wo.WebEvent : 결과를 받은 delegate 를 등록한다.
     * setCommon : 명령어를 입력한다.
     * setData : 서버로 보낼 protobuf를 등록한다.
     * 
     * result delegate
     * 결과는 WebObject를 리턴 받는다.
     * WebObject.getResult 로 protobuf를 Deserialize 정보를 받는다.
     * 
     * 
    void request()
        WebConnector.Instance.setServerUrl("http://rpgames.co.kr/xcross/");
        WebConnector.Instance.setIsDebug(true);
        WebConnector.Instance.CommonRequest.uuid = "xxx-xxxx-xxxx-xxx";
        WebConnector.Instance.CommonRequest.user_id = "120728607814";
        WebConnector.Instance.CommonRequest.account_key = "062a402505d500771630459aa4b9e74820d1c146";
		// 위의 내용은 한번만 선언하면 된다.
        // 선언 이후에는 아래의 내용만 실행하면 된다.
        rpg.master.RequestFriendAdd request = new rpg.master.RequestFriendAdd();
        {
            request.request = WebConnector.Instance.CommonRequest;
            request.friend_id = "120726591300";
        }     
        WebObject wo = new WebObject(response);
        wo.setCommand("friend_add");
        wo.setData<rpg.master.RequestFriendAdd>(request);
        WebConnector.Instance.request(wo);
    }

    void response(WebObject result)
    {
        rpg.master.ResponseFriendAdd response = result.getResult<rpg.master.ResponseFriendAdd>();
        rpg.master.CommonResponse common = response.response;
        if (common.success)
        {
            // 성공

        }
        else
        {
            // 실패
            Debug.Log("error_code = " + common.error_code);
        }
    }

     */

    RPGSerializer m_Serializer = new RPGSerializer();
    rpgames.game.CommonRequest m_CommonEqeust = new rpgames.game.CommonRequest();
    public rpgames.game.CommonRequest CommonRequest { get { return m_CommonEqeust; } }

	private string SERVER_URL = "";
	private bool m_isDebug = false;
    private List<WebObject> m_request_list = new List<WebObject>();

    private bool m_IsSendPacket = false;

	/// <summary>
	/// 서버와 통신을 한번이라도 했는지 체크합니다.
	/// 한번 가져간 이후에는 자동으로 초기화 합니다.
	/// </summary>
	/// <value><c>true</c> if this instance is send packet; otherwise, <c>false</c>.</value>
	public bool IsSendPacket { get { bool packet = m_IsSendPacket; m_IsSendPacket = false; return packet; } }

	private Dictionary<string, string> staticParams = new Dictionary<string, string>();

    public override void Init()
    {
        base.Init();
        m_request_list.Clear();
    }

    public byte[] Serialize<T>(T proto)
    {
        byte[] datas;
        using (MemoryStream stream = new MemoryStream())
        {
            m_Serializer.Serialize(stream, proto);
            datas = stream.ToArray();
        }
        return datas;
    }

    public T Deserialize<T>(byte[] datas)
    {
        T deserializedObject = default(T);
        using (MemoryStream stream = new MemoryStream(datas))
        {
            deserializedObject = (T)m_Serializer.Deserialize(stream, null, typeof(T));
        }
        return deserializedObject;
    }

	public void setServerUrl(string url)
	{
		SERVER_URL = url;
	}

	public void setIsDebug(bool isdebug)
	{
		m_isDebug = isdebug;
	}

	public void setStaticParam(string key, string value)
	{
		staticParams[key] = value;
	}

    public void setStaticParam(string key, int value)
    {
        staticParams[key] = value.ToString();
    }



	public string descript()
	{
		string ret = "";
		foreach (string key in staticParams.Keys)
		{
			string value = staticParams[key].ToString();
			ret += "staticParam : key = "+key+" value = "+value+"\n";
		}
		return ret;
	}
	
	void Update()
	{
	}
	
    /// <summary>
    /// 서버로 전송.
    /// </summary>
    /// <param name="obj">Object.</param>
	public void request(WebObject obj)
	{
        //URL이 셋팅되기 전의 통신은 전부 무시합니다.
        if (SERVER_URL.Length == 0)
            return;

        m_request_list.Add(obj);
        if (m_request_list.Count == 1)
		    StartCoroutine(requestWithWebObject(obj));
	}
	
    /// <summary>
    /// 다음 전송할 명령이 있는지 체크하여 전송.
    /// </summary>
    private void nextRequest(bool IsSendResult = false)
    {
        if (m_request_list.Count > 0)
        {
            // 서버로부터 받은 결과가 있는 경우에만 처리합니다.
            if (m_request_list[0].IsResponse)
            {
                if (IsSendResult)
                    m_request_list[0].sendResult();
                m_request_list.RemoveAt(0);
            }
        }
        if (m_request_list.Count > 0)
        {
            WebObject obj = m_request_list[0];
            StartCoroutine(requestWithWebObject(obj));
        }
    }

    /// <summary>
    /// 접속 재시도.
    /// 현재 전송내용을 다시 전송합니다.
    /// </summary>
    private void reTryRequest()
    {
        WebObject obj = m_request_list[0];
        StartCoroutine(requestWithWebObject(obj));
    }

	private IEnumerator requestWithWebObject(WebObject obj)
	{
        // 로딩 시작.
        if (obj.IsShowLoading)
            RPGLoadingManager.Instance.startLoading();
        Debug.Log("requestWithWebObject");

        WWWForm wf = new WWWForm();

        string ServerURL = SERVER_URL;
        if (obj.IsURL)
            ServerURL = obj.URL;
        if (obj.IsUseProtoBuf)
        {
            // 버전 정보 추가.
            wf.AddField("version", CommonRequest.version);
            // protobuf 바이너리 데이터 추가.
            wf.AddBinaryData("data", obj.Data);
        }

		string param = "";
		foreach (string key in staticParams.Keys)
		{
            string value = staticParams[key].ToString();
            if (param.Length == 0)
                param = key+"="+value;
            else
                param = param+"&"+key+"="+value;

            wf.AddField(key, value);
		}
		
		Dictionary<string, string> Params = obj.getParam();
		foreach (string key in Params.Keys)
		{
            string value = Params[key].ToString();
            if (param.Length == 0)
                param = key+"="+value;
            else
                param = param+"&"+key+"="+value;

            wf.AddField(key, value);
		}
		if (m_isDebug)
            Debug.Log(ServerURL+"?"+param);
        /*
        wf.headers["Content-Encoding"] = "gzip";
        foreach (string key in wf.headers.Keys)
        {
            Debug.Log("key = " + key);
            Debug.Log("value = " + wf.headers[key]);
        }
        */
        using (WWW www = new WWW(ServerURL, wf))
        {
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                //Debug.LogError(string.Format("request error!\n{0}", www.error));
                Debug.Log(string.Format("request error!\n{0}", www.error));
                // reTry
                if (obj.IsShowLoading)
                    RPGLoadingManager.Instance.endLoading();
                showReTryPopup();                
            } else if (string.IsNullOrEmpty(www.text))
            {
                //Debug.LogError("www.text is null");
                Debug.Log("www.text is null");
                // reTry
                if (obj.IsShowLoading)
                    RPGLoadingManager.Instance.endLoading();
                showReTryPopup();
            } else
            {
                if (m_isDebug)
                    Debug.Log(www.text);
                obj.setResult(www);

                //if (!checkEvent(obj))
                {
                    // 서버와 동기화에 실패한 경우에 대한 처리를 추가합니다. state == 0 인경우에 false
                    if (checkResult(obj))
                    {
                        //obj.sendResult();
                        // 다음 명령이 대기중인지 체크.
                        nextRequest(true);
                        // 로딩 끝.
                        if (obj.IsShowLoading)
                            RPGLoadingManager.Instance.endLoading();
                    }
                }
            }
        }
	}

    /// <summary>
    /// 서버로부터 받은 메시지를 임시로 저장하는 공간.
    /// </summary>
    List<rpgames.game.CommonResponse.ServerMessage> m_ServerMessages = new List<rpgames.game.CommonResponse.ServerMessage>();
    
    /// <summary>
    /// 서버에서 보내는 메시지가 있는지 확인하여 있으면 처리합니다.
    /// </summary>
    /// <param name="result"></param>
    bool checkResult(WebObject result)
    {
		//result.
        // 프로토버퍼 사용이 아니라면 실행하지 않는다.
        if (!result.IsUseProtoBuf)
            return true;

        rpgames.game.Response response = result.getResult<rpgames.game.Response>();
        rpgames.game.CommonResponse common = response.response;


		//kjh:: 통신 여부 판단. 
		string command = result.GetCommand();
		if ( !command.Equals( "chat_send" ) && !command.Equals( "chat_info" ) && !command.Equals( "alert_check" )
			&& !command.Equals( "quest_info" ) )
			m_IsSendPacket = true;

        // 서버 시간 적용.
        long timestamp_server = long.Parse(common.server_time.ToString());
        RPGDefine.setServerTimeStamp(timestamp_server);

        // 통신 실패인 경우의 처리.
        if (!common.success)
        {
            Debug.Log("Error = " + common.error_code.ToString());
            switch (common.error_code)
            {
                case rpgames.game.ErrorCode.Money_NotEnoughGold:
                    {
                        MessageBox box = MessageBox.show();
                        box.setMessage(DefineMessage.getMsg(30008));
                        box.addYesButton((b) =>
                        {
                            b.Close();
                            UIShop.show();
                        });
                        box.addNoButton();
                        box.addCloseButton();
                        nextRequest(false);
                        if (result.IsShowLoading)
                            RPGLoadingManager.Instance.endLoading();
                    }
                    return false;
            }
        }

        /*
        if (common.messages.Count > 0)
        {
            // 서버에서 보내는 메시지가 있는 경우에 처리.
            if (result.IsShowLoading)
                RPGLoadingManager.Instance.endLoading();

            m_ServerMessages.Clear();
            m_ServerMessages.AddRange(common.messages);
            nextMessage();
            return false;
        }
        */
        return true;
    }
    
    void nextMessage(bool IsFirstDelete = false)
    {
        nextRequest(true); 

        /*
        if (IsFirstDelete && m_ServerMessages.Count > 0)
            m_ServerMessages.RemoveAt(0);
        if (m_ServerMessages.Count == 0)
        {
            // 메세지 박스의 호출이 끝나면 다음 request가 있는지 체크한다.
            nextRequest(true); 
            return;
        }
        
        rpgames.game.CommonResponse.ServerMessage msg = m_ServerMessages[0];
        Popup_Common popup = Popup_Common.show();
        RPGTextMesh.TEXT_ALIGNMENT align = RPGTextMesh.TEXT_ALIGNMENT.center;
        switch (msg.align)
        {
            case rpgames.game.CommonResponse.ServerMessage.Alignment.Left: align = RPGTextMesh.TEXT_ALIGNMENT.left; break;
            case rpgames.game.CommonResponse.ServerMessage.Alignment.Center: align = RPGTextMesh.TEXT_ALIGNMENT.center; break;
            case rpgames.game.CommonResponse.ServerMessage.Alignment.Right: align = RPGTextMesh.TEXT_ALIGNMENT.right; break;
        }

        popup.setMessage(msg.message, msg.font_size, new Vector2(msg.text_size.x, msg.text_size.y), align);
        if (msg.buttons.Count > 1)
        {
            popup.addYesButton(msg.buttons[0].button_name, popupBtnServerYes, convertColor(msg.buttons[0].image_color));
            popup.addNoButton(msg.buttons[1].button_name, popupBtnServerNo, convertColor(msg.buttons[1].image_color));
        }
        else if (msg.buttons.Count > 0)
            popup.addDoneButton(msg.buttons[0].button_name, popupBtnServerYes, convertColor(msg.buttons[0].image_color));
        else
            popup.addCancelButton(popupBtnServerCancel);
        */
    }

    /*
    /// <summary>
    /// 서버에서 보낸 컬러값을 팝업의 컬러값으로 변환합니다.
    /// </summary>
    /// <param name="imageColor"></param>
    /// <returns></returns>
    Popup_Common.eButtonColor convertColor(rpgames.game.CommonResponse.ServerMessage.Button.ImageColor imageColor)
    {
        Popup_Common.eButtonColor color = Popup_Common.eButtonColor.None;
        switch (imageColor)
        {
            case rpgames.game.CommonResponse.ServerMessage.Button.ImageColor.Blue: color = Popup_Common.eButtonColor.Blue; break;
            case rpgames.game.CommonResponse.ServerMessage.Button.ImageColor.Red: color = Popup_Common.eButtonColor.Red; break;
            case rpgames.game.CommonResponse.ServerMessage.Button.ImageColor.Green: color = Popup_Common.eButtonColor.Green; break;
            case rpgames.game.CommonResponse.ServerMessage.Button.ImageColor.Purple: color = Popup_Common.eButtonColor.Purple; break;
        }
        return color;
    }

    void popupBtnServerYes(Popup_Common popup)
    {
        executeServerButton(m_ServerMessages[0].buttons[0]);
    }

    void popupBtnServerNo(Popup_Common popup)
    {
        executeServerButton(m_ServerMessages[0].buttons[1]);
    }

    void popupBtnServerCancel(Popup_Common popup)
    {
        // 버튼에대한 처리가 없으므로 아무것도 하지 않고 창을 닫는다.
        nextMessage(true);
    }


    /// <summary>
    /// 서버 버튼 처리.
    /// </summary>
    /// <param name="button"></param>
    void executeServerButton(rpgames.game.CommonResponse.ServerMessage.Button button)
    {
        switch (button.connect_type)
        {
            case rpgames.game.CommonResponse.ServerMessage.Button.ConnectType.Url:
                Application.OpenURL(button.connect_id);
                break;
            case rpgames.game.CommonResponse.ServerMessage.Button.ConnectType.Scene:
                KSceneManager.Instance.pushLayer((eLayer_Index)int.Parse(button.connect_id));
                break;
                // 현재 구조에서 진행이 잘 되지 않으므로 추후에 추가하는걸로....
            //case rpgames.game.CommonResponse.ServerMessage.Button.ConnectType.Popup:
                //KPopupManager.Instance.pushPopup((ePopup_Index)int.Parse(button.connect_id));
                //break;
        }
        if (button.ignore_response)
        {
            // 서버로부터 받은 결과가 있는 경우에만 제거합니다.
            if (m_request_list.Count > 0 && m_request_list[0].IsResponse)
                m_request_list.RemoveAt(0);
        }
        nextMessage(true);
    }

    void showReTryPopup()
    {
        Popup_Common popup = Popup_Common.show();
        popup.setMessage(KDefine_UI.GetServerText(2405));
        popup.addDoneButton(KDefine_UI.GetServerText(2406), popupYesRetry);

    }

    void popupYesRetry(Popup_Common popup)
    {
        reTryRequest();
    }
    */

    void showReTryPopup()
    {
        MessageBox box = MessageBox.show();
        box.setMessage(DefineMessage.getMsg(40001));
        box.addDoneButton((b) =>
        {
            reTryRequest();
            b.Close();
        });
    }
	
}
