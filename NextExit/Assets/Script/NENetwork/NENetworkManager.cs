using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPG.AntiVariable;

public class NENetworkManager : RPGSingleton<NENetworkManager> {

    const string _KEY_USERID = "NE_USERID";
    const string _KEY_NICKNAME = "NE_NICKNAME";
    const string _KEY_PREFIX_TEST_NICKNAME = "_PTESTN_";

    string m_UserID = "";
    public string UserID
    {
        get { return m_UserID; }
        private set
        {
            m_UserID = value;
            HPlayerPrefs.SetString(_KEY_USERID, m_UserID);
            HPlayerPrefs.Save();
        }
    }

    string m_Nickname = "";
    public string NickName
    {
        get { return m_Nickname; }
        private set
        {
            m_Nickname = value;
            if (GameConfig.TEST_USERID.Length > 0)
                HPlayerPrefs.SetString(_KEY_PREFIX_TEST_NICKNAME + m_UserID, m_Nickname);
            else
                HPlayerPrefs.SetString(_KEY_NICKNAME, m_Nickname);
            HPlayerPrefs.Save();
        }
    }

    public bool IsLogin { get; private set; }

    public HInt64 Gold { get; set; }
    public HInt64 SlotCount { get; set; }

    public override void Init()
    {
        Debug.Log("NENetworkManager #1");

        base.Init();

        Gold = 0;
        SlotCount = 0;
        IsLogin = false;

        if (GameConfig.TEST_USERID.Length > 0)
        {
            m_UserID = GameConfig.TEST_USERID;
            NickName = HPlayerPrefs.GetString(_KEY_PREFIX_TEST_NICKNAME + m_UserID, "");            
        }
        else
        {
            UserID = HPlayerPrefs.GetString(_KEY_USERID, "");
            NickName = HPlayerPrefs.GetString(_KEY_NICKNAME, "");
        }
        if (UserID.Length == 0)
            UserID = SystemInfo.deviceUniqueIdentifier;

        // 통신정보 기본 셋팅.
        WebConnector.Instance.setIsDebug(true);
        WebConnector.Instance.setServerUrl(GameConfig.getServerInfo().GameURL);

        WebConnector.Instance.CommonRequest.user_id = UserID;
        WebConnector.Instance.CommonRequest.version = GameConfig.getServerInfo().ClientVersion;
        switch (GameConfig.Language)
        {
            case 0: WebConnector.Instance.CommonRequest.language = rpgames.game.CommonRequest.Language.Korean; break;
            case 1: WebConnector.Instance.CommonRequest.language = rpgames.game.CommonRequest.Language.English; break;
            case 2: WebConnector.Instance.CommonRequest.language = rpgames.game.CommonRequest.Language.Japanese; break;
            case 3: WebConnector.Instance.CommonRequest.language = rpgames.game.CommonRequest.Language.Chinese; break;
        }
        WebConnector.Instance.CommonRequest.platform = rpgames.game.CommonRequest.Platform.StandAlone;
#if UNITY_IPHONE
        WebConnector.Instance.CommonRequest.os = rpgames.game.CommonRequest.OS.IOS;
#elif UNITY_ANDROID
        WebConnector.Instance.CommonRequest.os = rpgames.game.CommonRequest.OS.Android;
#endif


        Debug.Log("NENetworkManager #2");
    }

    public void request_login(System.Action<WebObject> selector)
    {
        if (NickName.Length == 0)
        {
            // 임시로 닉네임 입력창을 띄웁니다.            
            MessageInput input = MessageInput.show();            
            input.setMessage("Input Nickname");
            input.addDoneButton((m) =>
            {
                if (m.Text.Length == 0)
                {
                    MessagePrint.show("require Nickname");
                    return;
                }
                m.Close();
                NickName = m.Text;
                request_login(selector);
            });
        }
        else
        {
            rpgames.game.RequestUserLogin request = new rpgames.game.RequestUserLogin();
            {
                request.nickname = NickName;
                request.request = WebConnector.Instance.CommonRequest;
            }
            WebObject wo = new WebObject((w) =>
            {
                rpgames.game.ResponseUserLogin response = w.getResult<rpgames.game.ResponseUserLogin>();
                if (response.response.success)
                {
                    NickName = response.account.nickname;
                    Gold = response.account.gold;
                    SlotCount = response.account.slot;
                    IsLogin = true;
                    if (selector != null)
                        selector(w);
                }
                else
                {
                    Debug.LogError("login Error = " + response.response.error_code.ToString());
                }
            });
            wo.setData<rpgames.game.RequestUserLogin>(request);
            wo.setCommand("login");
            WebConnector.Instance.request(wo);
        }

    }

}
