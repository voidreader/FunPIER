using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class RPGDefine {


	/// <summary>
	/// Jsons to dictionary.
	/// </summary>
	/// <returns>The to dictionary.</returns>
	/// <param name="obj">Object.</param>
	public static Dictionary<string, object> JsonToDictionary( JSONObject obj )
	{
		if ( obj.type == JSONObject.Type.OBJECT )
		{
			Dictionary<string, object> result = new Dictionary<string, object>();
			for ( int i = 0; i < obj.list.Count; i++ )
			{
				JSONObject val = ( JSONObject )obj.list[i];
				switch ( val.type )
				{
				case JSONObject.Type.OBJECT:
				result.Add( ( string )obj.keys[i], JsonToDictionary( val ) );
				break;
				case JSONObject.Type.ARRAY:
				result.Add( ( string )obj.keys[i], JsonToArray( val ) );
				break;
				case JSONObject.Type.STRING:
				result.Add( ( string )obj.keys[i], val.str );
				break;
				case JSONObject.Type.NUMBER:
				result.Add( ( string )obj.keys[i], val.n + "" );
				break;
				case JSONObject.Type.BOOL:
				result.Add( ( string )obj.keys[i], val.b + "" );
				break;
				default:
				Debug.LogWarning( "Omitting object: " + ( string )obj.keys[i] + " in dictionary conversion" );
				break;
				}
			}
			return result;
		}
		else
			Debug.LogWarning( "Tried to turn non-Object JSONObject into a dictionary" );
		return null;
	}

	/// <summary>
	// JSONObject to ArrayList 
	/// </summary>
	/// <returns>The to array.</returns>
	/// <param name="obj">Object.</param>
	public static ArrayList JsonToArray( JSONObject obj )
	{
		if ( obj.type == JSONObject.Type.ARRAY )
		{
			ArrayList result = new ArrayList();
			foreach ( JSONObject val in obj.list )
			{
				switch ( val.type )
				{
				case JSONObject.Type.OBJECT:
				result.Add( JsonToDictionary( val ) );
				break;
				case JSONObject.Type.ARRAY:
				result.Add( JsonToArray( val ) );
				break;
				case JSONObject.Type.STRING:
				result.Add( val.str );
				break;
				case JSONObject.Type.NUMBER:
				result.Add( val.n + "" );
				break;
				case JSONObject.Type.BOOL:
				result.Add( val.b + "" );
				break;
				default:
				Debug.LogWarning( "Omitting object: in array conversion" );
				break;
				}
			}
			return result;
		}
		else
			Debug.LogWarning( "Tried to turn non-Object JSONObject into a array" );
		return null;
	}

    /// <summary>
    /// List 중복제거.
    /// </summary>
    public static List<T> GetDistinctValues<T>(List<T> array)
    {
        List<T> tmp = new List<T>();
        for (int i = 0; i < array.Count; i++)
        {
            if (tmp.Contains(array[i]))
                continue;
            tmp.Add(array[i]);
        }
        return tmp;
    }

    /// <summary>
    /// List 썩어주기.
    /// RPGDefine.Shuffle<T>(list);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(List<T> list)
    {
        int count = list.Count;
        for (var i = 0; i < list.Count; i++)
            Swap(list, i, UnityEngine.Random.Range(0, count));
    }

    /// <summary>
    /// 리스트 오브젝트 스왑.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public static void Swap<T>(List<T> list, int i, int j)
    {
        if (i == j) return;
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    /// <summary>
    /// 각 좌표간의 각도를 구합니다.
    /// </summary>
    /// <param name="me"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static float getAngle(Vector2 me, Vector2 target)
    {
        return Mathf.Atan2(me.x - target.x, me.y - target.y) * 180.0f / Mathf.PI;
    }

    /// <summary>
    /// 각도와 거리로 좌표를 구합니다.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static Vector2 getAngleDistancePosition(float angle, float distance)
    {
        float radian = angle * Mathf.Deg2Rad;
        float tx = Mathf.Sin(radian);
        float ty = Mathf.Cos(radian);
        return new Vector2(tx, ty);
    }

    /// <summary>
    /// object를 base64로 컨버팅 합니다.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string convertObjectToBase64(object data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, data);
        string ret = Convert.ToBase64String(ms.GetBuffer());
        return ret;
    }

    /// <summary>
    /// Base64를 object로 컨버팅 합니다.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static object convertBase64ToObject(string data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(Convert.FromBase64String(data));
        object ret = bf.Deserialize(ms);
        return ret;
    }

    /// <summary>
    /// string을 base64로 컨버팅합니다.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string convertStringToBase64(string data)
    {
        byte[] toEncodeAsBytes
            = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
        string returnValue
            = System.Convert.ToBase64String(toEncodeAsBytes);
        return returnValue;
    }

    /// <summary>
    /// base64를 string으로 컨버팅합니다.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string convertBase64ToString(string data)
    {
        byte[] encodedDataAsBytes
            = System.Convert.FromBase64String(data);
        string returnValue =
            System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
        return returnValue;
    }

    public static string SecToString(int time)
    {
        TimeSpan span = TimeSpan.FromSeconds(time);
        /*
        int hour = 0;
        int min = 0;
        int sec = time % 60;
        if (time >= 60)
            min = time / 60;
        if (time >= 60 * 60)
            hour = time / 60 / 60;
        */
        return string.Format("{0}:{1}:{2}", ((int)span.TotalHours).ToString("D2"), span.Minutes.ToString("D2"), span.Seconds.ToString("D2"));
    }

    private static long m_Server_SyncTime = 0;
    /// <summary>
    /// 서버와의 시간차이를 저장합니다. 클라이언트 시간 - 서버 시간.
    /// </summary>
    /// <value>The get_ server_ sync time.</value>
    //public static long get_Server_SyncTime{ get{ return m_Server_SyncTime; } set { m_Server_SyncTime = value; } }

    /// <summary>
    /// 서버의 타임스템프를 등록해서 클라이언트와 서버의 시간차를 저장해둡니다.
    /// </summary>
    /// <param name="server_timestamp"></param>
    public static void setServerTimeStamp(long timestamp_server)
    {
        // gmt가 계산되지 않은 UTC 시간으로 계산합니다.
        long timestamp_client = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;
        m_Server_SyncTime = timestamp_client - timestamp_server;
        //Debug.Log("m_Server_SyncTime = " + m_Server_SyncTime);
    }

    /// <summary>
    /// 현재 시간을 구합니다.
    /// </summary>
    /// <returns>The now date.</returns>
    /// <param name="gmt">Gmt.</param>
    private static DateTime getNowDate(int gmt = 0)
    {
        return DateTime.UtcNow.AddHours(gmt).AddSeconds(-m_Server_SyncTime);
    }

    /// <summary>
    /// UTC 현재시간 timestamp
    /// </summary>
    /// <returns>The now time stamp.</returns>
    public static long getNowTimeStamp()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds - m_Server_SyncTime;
    }

    /// <summary>
    /// TimeStamp To DateTime
    /// </summary>
    /// <returns>The time stamp to date time.</returns>
    /// <param name="timeStamp">Time stamp.</param>
    /// <param name="gmt">Gmt.</param>
    private static DateTime convertTimeStampToDateTime(long timeStamp, int gmt = 0)
    {
        //DateTime utcTime = Convert.ToDateTime(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, gmt, 0, 0)).ToString());
        //DateTime nowTime = utcTime.AddSeconds(timeStamp);
        DateTime nowTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddHours(gmt).AddSeconds(timeStamp);

        /*
        Debug.Log("UtcNow = "+DateTime.UtcNow.ToString());
        Debug.Log("Now = "+DateTime.Now.ToString());

        Debug.Log("now = " + nowTime.ToString());
        Debug.Log("ToLocalTime = "+nowTime.ToLocalTime().ToString());
        Debug.Log("ToUniversalTime = " + nowTime.ToUniversalTime().ToString());
        */
        return nowTime;
    }
	/// <summary>
	/// KJH:: 더블형 받는 구조.
	/// </summary>
	/// <param name="timeStamp"></param>
	/// <param name="gmt"></param>
	/// <returns></returns>
	private static DateTime convertTimeStampToDateTime( double timeStamp, int gmt = 0 )
	{
		DateTime nowTime = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Local ).AddHours( gmt ).AddSeconds( timeStamp );
		return nowTime;
	}

    /// <summary>
    /// 나라별 GMT를 구합니다.
    /// </summary>
    /// <returns>The GM.</returns>
    public static int getGMT()
    {
        
        switch (GameConfig.Language)
        {
            case 0: // 한국어
            case 2: // 일본어.
                return 9;
            case 3: // 중국어.
                return 8;
        }
        //디폴트.
        return 9;
    }

    /// <summary>
    /// 언어별 GMT 현재시간을 구한다.
    /// </summary>
    /// <returns>The now date by language.</returns>
    public static DateTime getNowDateByLanguage()
    {
        return getNowDate(getGMT());
    }

    /// <summary>
    /// 리셋 시간을 계산합니다.
    /// </summary>
    /// <returns>The calculate reset timer.</returns>
    /// <param name="hour">Hour.</param>
    public static DateTime getCalcResetTimer(int hour)
    {
        DateTime nowTimer = getNowDateByLanguage();
        if (nowTimer.Hour >= hour)
        {
            // 리셋 시간이 현재보다 크거나 같으면 리셋 타임을 다음날로 변경한다.
            nowTimer = nowTimer.AddDays(1);
        }
        return new DateTime(nowTimer.Year, nowTimer.Month, nowTimer.Day,
                            hour, 0, 0);
    }
    /*
    /// <summary>
    /// 언어별로 GMT TimeStamp를 뽑아온다.
    /// </summary>
    /// <returns>The time stamp by language.</returns>
    public static long getNowTimeStampByLanguage()
    {
        return getNowTimeStamp();
    }
    */

    /// <summary>
    /// 언어별로 UTC TimeStamp를 DateTime로 변환한다.
    /// </summary>
    /// <returns>The time stamp to date time by language.</returns>
    /// <param name="timeStamp">Time stamp.</param>
    public static DateTime convertTimeStampToDateTimeByLanguage(long timeStamp)
    {
        return convertTimeStampToDateTime(timeStamp, getGMT());
    }

	/// <summary>
	/// KJH:: 더블형 받는 구조.
	/// </summary>
	/// <param name="timeStamp"></param>
	/// <returns></returns>
	public static DateTime convertTimeStampToDateTimeByLanguage( double timeStamp )
	{
		return convertTimeStampToDateTime( timeStamp, getGMT() );
	}

    private static string UniqueID_key = "NUUID";

    /// <summary>
    /// 새로운 유니크 유저아이디를 가져옵니다.
    /// </summary>
    /// <returns>The unique I.</returns>
    public static string NewUniqueID()
    {
        string uniqueID = "";
        var random = new System.Random();                     
        DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        //double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
        double timestamp = (System.DateTime.UtcNow - epochStart).Milliseconds;
        /*
        uniqueID = Application.systemLanguage                                       //Language
                +"-"+Application.platform.ToString()                                //Device    
                +"-"+String.Format("{0:X}", Convert.ToInt32(timestamp))             //Time
                +"-"+String.Format("{0:X}", Convert.ToInt32(Time.time*1000000))     //Time in game
                +"-"+String.Format("{0:X}", random.Next(1000000000));               //random number
                */
        uniqueID = String.Format("{0}", Convert.ToInt64(timestamp*100000))             //Time
            + String.Format("{0:D10}", random.Next(1000000000));               //random number
        
        Debug.Log("Generated Unique ID: "+uniqueID);
        
        PlayerPrefs.SetString(UniqueID_key, uniqueID);
        PlayerPrefs.Save();    
        return uniqueID;
    }

    /// <summary>
    /// 유니크 유저아이디를 가져옵니다.
    /// </summary>
    /// <returns>The unique I.</returns>
    public static string GetUniqueID()
    {
        /*
        if (GameConfig.TEST_USERID.Length > 1)
            return GameConfig.TEST_USERID;
            */
        //string key = "NUUID";
        string uniqueID = "";
        if (PlayerPrefs.HasKey(UniqueID_key))
        {
            uniqueID = PlayerPrefs.GetString(UniqueID_key);
            Debug.Log("(PlayerPrefs) Generated Unique ID: "+uniqueID);
            return uniqueID;
        }
        return NewUniqueID();
    }

    /// <summary>
    /// 유니크 유저아이디를 저장합니다.
    /// </summary>
    /// <returns>The unique I.</returns>
    /// <param name="id">Identifier.</param>
    public static void SetUniqueID(string id)
    {
        Debug.Log("SetUniqueID : " + id);
        PlayerPrefs.SetString(UniqueID_key, id);
        PlayerPrefs.Save();
    }

    private static string SubID_key = "SUBID";
    /// <summary>
    /// 새로운 서브아이디를 생성합니다.
    /// </summary>
    /// <returns>The sub I.</returns>
    private static string NewSubID()
    {
        string uniqueID = "";
        var random = new System.Random();                     
//        DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
//        double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
        
        uniqueID = String.Format("{0:D10}", random.Next(1000000000));               //random number
        
        Debug.Log("Generated Unique ID: "+uniqueID);
        
        PlayerPrefs.SetString(SubID_key, uniqueID);
        PlayerPrefs.Save();    
        return uniqueID;
    }

    /// <summary>
    /// 서브 아이디를 리턴합니다.
    /// </summary>
    /// <returns>The sub I.</returns>
    public static string GetSubID()
    {
        /*
        if (GameConfig.TEST_SUBID.Length > 1)
            return GameConfig.TEST_SUBID;
            */
        //string key = "SUBID";
        string uniqueID = "";
        if (PlayerPrefs.HasKey(SubID_key))
        {
            uniqueID = PlayerPrefs.GetString(SubID_key);
            Debug.Log("(PlayerPrefs) Generated Unique Sub ID: "+uniqueID);
            return uniqueID;
        }
        return NewSubID();
    }

    /// <summary>
    /// 서브아이디를 등록합니다.
    /// </summary>
    /// <returns>The sub I.</returns>
    /// <param name="id">Identifier.</param>
    public static void SetSubID(string id)
    {
        Debug.Log("SetSubID : " + id);
        PlayerPrefs.SetString(SubID_key, id);
        PlayerPrefs.Save();
    }

    public static float ScreenWidth { get { return 1136.0f; } }
    public static float ScreenHeight { get { return 640.0f; } }

    public static float resolution_width { get { return (ScreenWidth / (float)Screen.width); } }
    public static float resolution_height { get { return (ScreenHeight / (float)Screen.height); } }

    public static float orthographicSize
    {
        get
        {
            float original_size = ScreenWidth / ScreenHeight;
            float now_size = (float)Screen.width / (float)Screen.height;
            Debug.Log("Screen.width = "+Screen.width+" Screen.height = "+Screen.height);
            float size = original_size / now_size;
            Debug.Log("original_size = "+size);
            return size;
            /*
            if (original_size > now_size)
            {
                float size = original_size / now_size;
                Debug.Log("original_size = "+size);
                return size;
            }
            return 1.0f;
            */
        }
    }

    /// <summary>
    /// 줄바꿈 변환.
    /// </summary>
    /// <returns>The string new line.</returns>
    /// <param name="text">Text.</param>
    public static string convertStringNewLine(string text)
    {
        text = text.Replace("\\\\n", System.Environment.NewLine);
        text = text.Replace("\\n", System.Environment.NewLine);
        text = text.Replace("\\/", "/");
        return text;
    }

    /// <summary>
    /// URL 변환.
    /// </summary>
    /// <returns>The string UR.</returns>
    /// <param name="url">URL.</param>
    public static string convertStringURL(string url)
    {
        return url.Replace("\\/", "/");
    }

    /// <summary>
    /// 모든 하위 경로의 파일을 찾습니다.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="filename">Filename.</param>
    public static string findFile(string path, string filename)
    {
        string fullpath = path + filename;
        //Debug.Log("Path.GetFullPath(fullpath) =" + Path.GetFullPath(fullpath));
//        if (File.Exists(Path.GetFullPath(fullpath)))
        if (File.Exists(fullpath))
            return fullpath;

        string[] directoryList = Directory.GetDirectories(path);
        if (directoryList.Length == 0)
            return "";
        foreach (string directory in directoryList)
        {
            //Debug.Log("directory = "+directory);
            //fullpath = path + directory + filename;
            string ret = findFile(directory+"/", filename);
            if (ret.Length > 0)
                return ret;
        }
        return "";
    }

    /// <summary>
    /// 파일 쓰기.
    /// </summary>
    /// <param name="str">String.</param>
    /// <param name="filename">Filename.</param>
    public static void writeStringToFile( string str, string filename )
    {
        writeStringToFile(str, filename, pathForDocuments());
    }

    public static void writeStringToFile( string str, string filename, string path )
    {
        // 웹플레이어에서는 작동되지 않도록 한다.
        if (Application.isWebPlayer)
            return;

        path = Path.Combine(path, filename);
        FileStream file = new FileStream (path, FileMode.Create, FileAccess.Write);
        
        StreamWriter sw = new StreamWriter( file );
        sw.WriteLine( str );
        
        sw.Close();
        file.Close();
    }


    public static void writeByteToFile(byte[] data, string filename)
    {
        // 웹플레이어에서는 작동되지 않도록 한다.
        if (Application.isWebPlayer)
            return;
#if !UNITY_WEBPLAYER        
        string path = pathForDocumentsFile( filename );
        File.WriteAllBytes(path, data);
#endif
    }

    /// <summary>
    /// 파일 읽기.
    /// </summary>
    /// <returns>The string from file.</returns>
    /// <param name="filename">Filename.</param>
    public static string readStringFromFile(string filename)//, int lineIndex )
    {
        // 웹플레이어에서는 작동되지 않도록 한다.
        if (Application.isWebPlayer)
            return null;

        string path = pathForDocumentsFile( filename );

        return readStringFromFullPath(path);
        /*
        if (File.Exists(path))
        {
            FileStream file = new FileStream (path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader( file );
            
            string str = null;
            str = sr.ReadLine ();
            
            sr.Close();
            file.Close();
            
            return str;
        }
        else
        {
            return null;
        }
        */
    }

    /// <summary>
    /// 파일이 존재하는지 확인합니다.
    /// </summary>
    /// <returns><c>true</c>, if file was found, <c>false</c> otherwise.</returns>
    /// <param name="filename">Filename.</param>
    public static bool findFile(string filename)
    {
        string path = pathForDocumentsFile( filename );

        if (File.Exists(path))
            return true;
        return false;
    }

    public static string getUrlToFilePath(string filename)
    {
        string path = Path.GetFileName(filename);
        string fullpath = pathForDocumentsFile( path );

        return fullpath;
    }

    /// <summary>
    /// Reads the texture from file.
    /// </summary>
    /// <returns>The texture from file.</returns>
    /// <param name="fullpath">Fullpath.</param>
    public static Texture2D readTextureFromFile(string filename, int width, int height)
    {
//        string path = pathForDocumentsFile( filename );

        string data = readStringFromFile(filename);
        if (data == null)
            return null;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);

        Texture2D tex = new Texture2D(width, height);
        tex.LoadImage(bytes);
        return tex;
    }

    /// <summary>
    /// 전체경로를 읽어서 파일을 읽는다.
    /// </summary>
    /// <returns>The string from full path.</returns>
    /// <param name="fullpath">Fullpath.</param>
    public static string readStringFromFullPath(string fullpath)
    {
        // 웹플레이어에서는 작동되지 않도록 한다.
        if (Application.isWebPlayer)
            return fullpath;

        if (File.Exists(fullpath))
        {
            FileStream file = new FileStream (fullpath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader( file );
            
            string str = null;
            str = sr.ReadLine ();
            
            sr.Close();
            file.Close();
            
            return str;
        }
        else
        {
            //return fullpath;
            return "";
        }
    }

    /// <summary>
    /// 파일 제거.
    /// </summary>
    /// <param name="filename">Filename.</param>
    public static void deleteFile(string filename)
    {
        // 웹플레이어에서는 작동되지 않도록 한다.
        if (Application.isWebPlayer)
            return;

        string path = pathForDocumentsFile( filename );
        if (File.Exists(path))
            File.Delete(path);
    }

    /// <summary>
    /// 내부 저장소의 모든 파일을 제거합니다.
    /// </summary>
    public static void removeInternalPathAllFile()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            removeAllFile("/data/data/com.glambox.StardustStory/");
        }
    }
    
    /// <summary>
    /// Removes all file.
    /// </summary>
    /// <param name="path">Path.</param>
    public static void removeAllFile(string path)
    {
#if !UNITY_WEBPLAYER
        try {
            Array.ForEach(Directory.GetFiles(path), File.Delete);
            Array.ForEach(Directory.GetDirectories(path), removeAllFile);
            Array.ForEach(Directory.GetDirectories(path), Directory.Delete);
        } catch (Exception ex) {
            Debug.LogWarning(ex.ToString());
        }
#endif
    }

    private static string pathForDocumentsFile( string filename ) 
    {
        return Path.Combine(pathForDocuments(), filename);
        /*
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.persistentDataPath.Substring( 0, Application.persistentDataPath.Length - 5 );
            path = path.Substring( 0, path.LastIndexOf( '/' ) );
            return Path.Combine( Path.Combine( path, "Documents" ), filename );
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath; 
            path = path.Substring(0, path.LastIndexOf( '/' ) ); 
            return Path.Combine (path, filename);
        }
        else 
        {
            string path = Application.dataPath; 
            path = path.Substring(0, path.LastIndexOf( '/' ) );
            return Path.Combine (path, filename);
        }
        */
    }

    private static string pathForDocuments() 
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //            string path = Application.dataPath.Substring( 0, Application.dataPath.Length - 5 );
            string path = Application.persistentDataPath.Substring( 0, Application.persistentDataPath.Length - 5 );
            path = path.Substring( 0, path.LastIndexOf( '/' ) );
            return Path.Combine( path, "Documents" );
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath; 
            return path.Substring(0, path.LastIndexOf( '/' ) ); 
        }
        else 
        {
            string path = Application.dataPath; 
            return path.Substring(0, path.LastIndexOf( '/' ) );
        }
    }

    /// <summary>
    /// 안드로이드 내장 메모리의 경로.
    /// </summary>
    /// <returns>The for android internal path.</returns>
    /// <param name="filename">Filename.</param>
    private static string pathForAndroidInternalPath( string filename )
    {
        string path = "/data/data/com.glambox.StardustStory/files/"; 
        path = path.Substring(0, path.LastIndexOf( '/' ) ); 
        return Path.Combine (path, filename);
    }

    


}
