//#define DEVELOPER_SERVER
//#define GLAMBOX_SERVER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameConfig {

    public const float PixelsPerMeter = 20.0f;

    public const string _SaveFileAssetPath = "Assets/Resources/";
    public const string _SaveFileResPath = "MapData/";
    public const string _SaveFileFullPath = _SaveFileAssetPath + _SaveFileResPath;
    public const string _Extention = ".txt";
    public const string _StageFilePath = "Stage/Stage";


    /// <summary>
    /// 개발모드인지? 개발모드인경우에는 로그인 UI에서 로그인을 한다.
    /// 서버선택, 언어선택이 존재한다.
    /// </summary>
    public const bool IS_DEVELOPER = true;
	/// <summary>
    /// 기본언어. 한글(0), 영어(1), 일본어(2), 중국어(3)
    /// </summary>
    public static int Language = 0;
    /// <summary>
    /// 기본적으로 접속할 서버. 개발모드에서는 서버선택이 가능하기때문에 의미 없음.
    /// </summary>
    private static eServerType m_ServerType = eServerType.DevServer;
    /// <summary>
    /// 임시로 아이디를 박아야 하는 경우만 사용합니다. 비어있는경우에는 자신의 아이디를 사용합니다.
    /// </summary>
    public const string TEST_USERID = "";
    /// <summary>
    /// 네트워크 상태를 디버깅할지?
    /// </summary>
    public const bool IS_NETWORKDEBUG = true;
    
    /// <summary>
    /// 
    /// </summary>
    public const string MapDataVersion = "1.0.0";

    public static string TestClientVersion = "1.0.0";

    /// <summary>
    /// ios(0), android(1), web(2).
    /// </summary>
    #if UNITY_IPHONE
    public static string OS = "I";
    #elif UNITY_ANDROID
    public static string OS = "A";
    #else
    public static string OS = "A";
    #endif
    
    /// <summary>
    /// 테스트용 CDN 동기화 필요없는 URL.
    /// </summary>
    #if UNITY_IPHONE
    public const string CDN_TEST_URL = "http://admin.starduststory.com/nf_cdn/data/IOS/test";
    #elif UNITY_ANDROID
    public const string CDN_TEST_URL = "http://admin.starduststory.com/nf_cdn/data/Android/test";
    #elif UNITY_WEBPLAYER
    public const string CDN_TEST_URL = "http://admin.starduststory.com/nf_cdn/data/WebPlayer/test";
    #else
    public const string CDN_TEST_URL = "http://admin.starduststory.com/nf_cdn/data/Android/test";
    #endif
    
    /// <summary>
    /// 앱스토어(0), 구글스토어(1), 티스토어(2), 올레마켓(3), 유플러스(4), 페이스북(5).
    /// </summary>
    #if UNITY_IPHONE
    public static eMarketType Market = eMarketType.Apple;
    #elif UNITY_ANDROID
    public static eMarketType Market = eMarketType.Google;
	//YSY::20170824::원스토어로 변경.
	//public static eMarketType Market = eMarketType.OnStore;
#else
    public static eMarketType Market = eMarketType.Web;
    #endif
    
    public enum eServerType
    {
        DevServer,
        TestServer,
        RealServer,
    }
    
    private static Dictionary<eServerType, cServerURL> ServerList = new Dictionary<eServerType, cServerURL>()
    {
        {eServerType.DevServer , new cServerURL(eServerType.DevServer)},
        {eServerType.TestServer, new cServerURL(eServerType.TestServer)},
        {eServerType.RealServer, new cServerURL(eServerType.RealServer)},
    };
    
    public enum eMarketType
    {
        Apple = 0,
        Google,
        Web,
		OnStore,
    }
    
    public class cServerURL
    {
		/// <summary>
		/// 게임서버 주소.
		/// </summary>
		public string GameURL = "";
		/// <summary>
        /// 게임정보 시트를 담고 있는 URL.
        /// </summary>
        public string SheetURL = "";
        /// <summary>
        /// 웹페이지 URL
        /// </summary>
        public string PageURL = "";
        /// <summary>
        /// 소켓서버 URL
        /// </summary>
        public string SocketURL = "";

        /// <summary>
        /// 클라이언트 버전관리를 위한 키.
        /// </summary>
        private string ClientVersionKey = "";
        /// <summary>
        /// 클라이언트 버전.
        /// </summary>
        public string ClientVersion = "";
        
        public eServerType Type = eServerType.RealServer;
        public cServerURL(eServerType type)
        {
            Type = type;
            /*
            switch (type)
            {
                case eServerType.DevServer:
                    GameURL = "http://rpgames.co.kr/xcross/";
                    SheetURL = "http://rpgames.co.kr/xcross_data/gz/";
                    PageURL = "http://www.pytgame.cn/";
                    PytURL = "http://www.pytgame.cn/";
                    break;
                case eServerType.TestServer:
                    GameURL = "http://rpgames.co.kr/xcross/";
                    SheetURL = "http://rpgames.co.kr/xcross_data/gz/";
                    PageURL = "http://www.pytgame.cn/";
                    PytURL = "http://www.pytgame.cn/";
                    break;
                case eServerType.RealServer:
                    GameURL = "http://rpgames.co.kr/xcross/";
                    SheetURL = "http://rpgames.co.kr/xcross_data/gz/";
                    PageURL = "http://www.pytgame.cn/";
                    PytURL = "http://www.pytgame.cn/";
                    break;
            }
            */
            GameURL = "http://monocatpunch.iptime.org/giveup/";
            SheetURL = "http://monocatpunch.iptime.org/giveup_master_data/gz/";
            PageURL = "";
            SocketURL = "rpgames.co.kr";

            switch (Market)
            {
                case eMarketType.Apple:
                    ClientVersionKey = "AppleClientKey";
                    ClientVersion = "1.0.0";
                    break;
                case eMarketType.Google:
                    ClientVersionKey = "GoogleClientKey";
                    ClientVersion = "1.0.0";
                    break;
                default: // web
                    ClientVersionKey = "WebClientKey";
                    ClientVersion = "1.0.0";
                    break;
            }
            //changeVersion("1.3.0");
            
            string beforeVersion = PlayerPrefs.GetString(ClientVersionKey, "1.0.0");
            if (checkClientUpdateVersion(beforeVersion))
                changeVersion(ClientVersion);
            else
                ClientVersion = beforeVersion;
            
            Debug.Log(Type.ToString()+" ClientVersion = "+ClientVersion);
            //ClientVersion = PlayerPrefs.GetString(ClientVersionKey, "1.0.0");
        }
        
        /// <summary>
        /// 리소스 다운로드용 버전 코드.
        /// </summary>
        /// <returns>The resource version.</returns>
        public string getResourceVersion()
        {
            string[] ClientVersionList = ClientVersion.Split(".".ToCharArray(), System.StringSplitOptions.None);
            int ClientVersion_1 = int.Parse(ClientVersionList [0]);
            int ClientVersion_2 = int.Parse(ClientVersionList [1]);
            return ClientVersion_1+"."+ClientVersion_2+".0";
        }
        
        /// <summary>
        /// Checks the client update version.
        /// </summary>
        /// <returns><c>true</c>, if client update version was checked, <c>false</c> otherwise.</returns>
        /// <param name="beforeVersion">Before version.</param>
        public bool checkClientUpdateVersion(string beforeVersion)
        {
            string[] ClientVersionList = ClientVersion.Split(".".ToCharArray(), System.StringSplitOptions.None);
            int ClientVersion_1 = int.Parse(ClientVersionList [0]);
            int ClientVersion_2 = int.Parse(ClientVersionList [1]);
            string[] ServerVersionList = beforeVersion.Split(".".ToCharArray(), System.StringSplitOptions.None);
            int ServerVersion_1 = int.Parse(ServerVersionList [0]);
            int ServerVersion_2 = int.Parse(ServerVersionList [1]);
            
            if (ClientVersion_1 > ServerVersion_1)
                return true;
            if (ClientVersion_2 > ServerVersion_2)
                return true;
            return false;
        }
        
        /// <summary>
        /// 클라이언트의 업데이트가 필요한 버전 체크.
        /// 클라이언트의 업데이트가 필요하면 true.
        /// </summary>
        /// <returns><c>true</c>, if patch version was checked, <c>false</c> otherwise.</returns>
        /// <param name="ServerVersion">Server version.</param>
        public bool checkPatchVersion(string ServerVersion)
        {
            string[] ClientVersionList = ClientVersion.Split(".".ToCharArray(), System.StringSplitOptions.None);
            int ClientVersion_1 = int.Parse(ClientVersionList [0]);
            int ClientVersion_2 = int.Parse(ClientVersionList [1]);
            string[] ServerVersionList = ServerVersion.Split(".".ToCharArray(), System.StringSplitOptions.None);
            int ServerVersion_1 = int.Parse(ServerVersionList [0]);
            int ServerVersion_2 = int.Parse(ServerVersionList [1]);
            
            if (ClientVersion_1 < ServerVersion_1)
                return true;
            if (ClientVersion_2 < ServerVersion_2)
                return true;
            return false;
        }
        /// <summary>
        /// 정보만 업데이트 하는 버전 체크.
        /// 정보 업데이트가 필요하면 true.
        /// </summary>
        /// <returns><c>true</c>, if info version was checked, <c>false</c> otherwise.</returns>
        /// <param name="ServerVersion">Server version.</param>
        public bool checkInfoVersion(string ServerVersion)
        {
            string[] ClientVersionList = ClientVersion.Split(".".ToCharArray(), System.StringSplitOptions.None);
            int ClientVersion_3 = int.Parse(ClientVersionList [2]);
            string[] ServerVersionList = ServerVersion.Split(".".ToCharArray(), System.StringSplitOptions.None);
            int ServerVersion_3 = int.Parse(ServerVersionList [2]);
            
            if (ClientVersion_3 < ServerVersion_3)
                return true;
            return false;
        }
        
        /// <summary>
        /// 클라이언트 버전 변경.
        /// </summary>
        /// <param name="ServerVersion">Server version.</param>
        public void setVersion(string ServerVersion)
        {
            // 클라이언트의 패치가 필요한 경우에는 튕겨내야 한다.
            if (checkPatchVersion(ServerVersion))
                return;
            // 버전 교체가 필요하지 않으면 튕겨낸다.
            if (!checkInfoVersion(ServerVersion))
                return;
            changeVersion(ServerVersion);
        }
        
        /// <summary>
        /// 버전을 무조건 교체합니다.
        /// </summary>
        /// <param name="ServerVersion">Server version.</param>
        private void changeVersion(string ServerVersion)
        {
            PlayerPrefs.SetString(ClientVersionKey, ServerVersion);
            PlayerPrefs.Save();
            ClientVersion = ServerVersion;
            // 서버로 보내는 버전도 변경해야 한다.
            //SYGameInfoConnector.Instance.setStaticParam("version", ClientVersion);
            WebConnector.Instance.setStaticParam("version", ClientVersion);
        }
    }
    
    //    public static eServerType ServerType = eServerType.TestServer;
    public static eServerType ServerType 
    { 
        get 
        { 
            return m_ServerType; 
        } 
        set 
        { 
            m_ServerType = value; 
            /*
            if (Application.isWebPlayer || Application.isEditor) {
                if (!Security.PrefetchSocketPolicy(GameConfig.getServerInfo().SFSURL, 843, 500)) {
                    Debug.LogError("Security Exception. Policy file loading failed!");
                }
            }
            */
        } 
    }
    
    public static cServerURL getServerInfo()
    {
        return ServerList [ServerType];
    }
    
    
}
