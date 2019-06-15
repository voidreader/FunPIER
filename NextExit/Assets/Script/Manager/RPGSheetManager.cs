using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPG.AntiVariable;

/// <summary>
/// <para>StartDownload를 실행하여 다운로드를 시작합니다.</para>
/// <para>getSheetData를 사용하여 시트데이터의 내용을 가져옵니다.</para>
/// <para>https://docs.google.com/spreadsheets/d/1KkW3rUge1XRlm5l7OJ7pFz8UOwvcKBk5OELsjmcfxl4/edit#gid=1675528912</para>
/// <para></para>
/// <para>텍스트 출력 예제.</para>
/// <para>rpgames.sheet.Text_Sub sub = (rpgames.sheet.Text_Sub)getSheetData("data_text.bin", "130002");</para>
/// <para>Debug.Log(sub.korea);</para>
/// </summary>
public class RPGSheetManager : RPGSingleton<RPGSheetManager>
{
    // 사용안함.
    //[Tooltip("Start에서 StartDownload를 실행할지 여부")]
    //public bool IsDebugStart = false;
    [Tooltip("결과 로그를 출력할지 여부.")]
    public bool isDebugLog = false;
    
    public enum eStatus
    {
        Waiting,
        Sheet_Downloading,
        File_Downloading,
        Done
    }

    const string _LIST_FILENAME = "sheet_list.json";

    [Tooltip("캐싱 할까?")]
    public bool IsCaching = true;
    [Tooltip("캐싱을 삭제하고 다운로드 받을지?")]
    public bool IsRemoveCaching = false;

    const string prefix_sheet_md5 = "sheet_md5_";
    const string prefix_sheet_data = "sheet_data_";

    //public delegate void SheetProgressDelegate(int progressCount, int totalCount);
    /// <summary>
    /// (int progressCount, int totalCount)
    /// </summary>
    public System.Action<int, int> AProgressEvent = null;
    //public delegate void SheetStatusDelegate(eStatus status);
    public System.Action<eStatus> AStatusEvent = null;

    /// <summary>
    /// 다운로드 받을 시트 리스트.
    /// key = binary 파일명, value = 파일 비교용 md5 데이터.
    /// </summary>
    Dictionary<string, object> m_DownloadSheetList;
    /// <summary>
    /// key = binary 파일명, value = json 데이터.
    /// </summary>
    Dictionary<string, object> m_SheetList = new Dictionary<string,object>();

    int m_DownloadCount = 0;
    int m_TotalDownloadCount = 0;
    eStatus m_Status = eStatus.Waiting;
    public eStatus Status { get { return m_Status; } }
    public int TotalDownloadCount { get { return m_TotalDownloadCount; } }
    public int DownloadCount { get { return m_DownloadCount; } }

	//######################
	//kjh::
	public delegate void EndLoadingDelegate();
	private event EndLoadingDelegate m_EndEvent = null;
	public EndLoadingDelegate SetEndEvent { set { m_EndEvent = value; } }
	public EndLoadingDelegate AddEndEvent { set { m_EndEvent += value; } }

    public override void Init()
    {
        base.Init();
        m_Status = eStatus.Waiting;
		startGetInfo();
    }

	/// <summary>
	/// json 시트를 읽어옵니다.
	/// </summary>
	public void startGetInfo( EndLoadingDelegate endEvent = null )
	{
		if ( endEvent != null )
			m_EndEvent = endEvent;
		// 이미 시트를 읽어온 상태라면 읽어오지 않습니다.
		if ( m_SheetList.Count > 0 )
		{
			endLoading();
			return;
		}

		Object[] jsonFileList = Resources.LoadAll( "GameData/json" );
		foreach ( Object data in jsonFileList )
		{
			TextAsset jsonText = data as TextAsset;
			JSONObject json_result = new JSONObject( jsonText.text );
			ArrayList json_array = RPGDefine.JsonToArray( json_result );
		
			m_SheetList.Add( jsonText.name + ".bin", json_array );
//			Debug.Log("read json jsonText.name = "+jsonText.name+ "\ntext = "+jsonText.text);
		}

		endLoading();
	}

	void endLoading()
	{
		if ( m_EndEvent != null )
			m_EndEvent();
		m_EndEvent = null;
	}


    /// <summary>
    /// 다운로드를 시작합니다.
    /// </summary>
    public bool StartDownload()
    {
        // 이미 완료 되었으면 더이상 받지 않습니다.
        if (m_Status == eStatus.Done)
            return false;

        m_Status = eStatus.Sheet_Downloading;
        if (AStatusEvent != null)
            AStatusEvent(m_Status);
        // 다운로드할 리스트를 다운로드 합니다.
        WebDownloader.Instance.setServerUrl(GameConfig.getServerInfo().SheetURL);
        //WebDownloader.Instance.setServerUrl("http://rpgames.co.kr/xcross_data/gz/");
        WebDownloader.Instance.request(_LIST_FILENAME, responseList);

        return true;
    }

	/// <summary>
	/// 시트이름과 키이름 해당키의 내용을 입력하여 정보를 가져옵니다.
	/// </summary>
	/// <returns>The info.</returns>
	/// <param name="sheetName">Sheet name.</param>
	/// <param name="keyName">Key name.</param>
	/// <param name="id">Identifier.</param>
	public Dictionary<string, object> getInfo( string sheetName, string keyName, string id )
	{
		startGetInfo();

		ArrayList sheetList = ( ArrayList )m_SheetList[sheetName];
		foreach ( Dictionary<string, object> dic in sheetList )
		{
			if ( dic[keyName].Equals( id ) )
				return dic;
		}
		return null;
	}

	public Dictionary<string, object> getInfo( string sheetName, string keyName_1, string id_1, string keyName_2, string id_2 )
	{
		startGetInfo();

		ArrayList sheetList = ( ArrayList )m_SheetList[sheetName];
		foreach ( Dictionary<string, object> dic in sheetList )
		{
			if ( dic[keyName_1].Equals( id_1 ) && dic[keyName_2].Equals( id_2 ) )
				return dic;
		}
		return null;
	}

	/// <summary>
	/// 해당 시트의 리스트를 전부 가져옵니다.
	/// </summary>
	/// <returns>The info array.</returns>
	/// <param name="sheetName">Sheet name.</param>
	public ArrayList getInfoArray( string sheetName )
	{
		startGetInfo();

		if ( m_SheetList.ContainsKey( sheetName ) )
			return ( ArrayList )m_SheetList[sheetName];
		return null;
	}


    /// <summary>
    /// 전체 시트의 데이터를 가져가는데 사용합니다.
    /// dictionary 구조로 가져갑니다.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public Dictionary<string, object> getSheetData(string filename)
    {
        return (Dictionary<string, object>)m_SheetList[filename];
    }

    /// <summary>
    /// 전체 시트의 데이터에서 해당 키의 내용만 가져옵니다.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="key"></param>
    /// <returns></returns>
	public object getSheetData( string filename, string key )
    {        
        Dictionary<string, object> dic = getSheetData(filename);
        if (!dic.ContainsKey(key))
            Debug.LogError("getSheetData "+filename+" : "+key+" not found");
        return dic[key];
    }

    /// <summary>
    /// sheet_list.json 다운로드 결과를 진행합니다.
    /// </summary>
    /// <param name="result"></param>
    void responseList(WebDownloader.WebDownloaderObject result)
    {
        // 서버로부터 받은 gz 데이터를 압축해제 합니다.
        string uncompress = Zipper.UnGzipString(result.getBytes());
        JSONObject json = new JSONObject(uncompress);
        Debug.Log(json.print());
        m_DownloadSheetList = json.ToDictionary();

        foreach (KeyValuePair<string, object> pair in m_DownloadSheetList)
        {
            //시트 디버그용.
            Debug.Log(pair.Key + " : " + pair.Value);
            //캐싱된 데이터를 지워야 하는 경우에 여기서 삭제한다.
            if (IsRemoveCaching)
                HPlayerPrefs.DeleteKey(prefix_sheet_md5 + pair.Key);
        }
        if (IsRemoveCaching)
        {
            HPlayerPrefs.Save();
            IsRemoveCaching = false;
        }
        m_Status = eStatus.File_Downloading;
        if (AStatusEvent != null)
            AStatusEvent(m_Status);
        BinaryDownload();
    }

    /// <summary>
    /// 리스트에 있는 시트 데이터를 다운로드 합니다.
    /// </summary>
    void BinaryDownload()
    {
        m_TotalDownloadCount = m_DownloadSheetList.Count;
        m_DownloadCount = 0;
        if (AProgressEvent != null)
            AProgressEvent(m_DownloadCount, m_TotalDownloadCount);
        foreach (string filename in m_DownloadSheetList.Keys)
        {
            string remote_md5 = m_DownloadSheetList[filename].ToString();
            string local_md5 = HPlayerPrefs.GetString(prefix_sheet_md5 + filename, "");
            bool isDownload = true;
            // md5 비교.
            if (remote_md5.Equals(local_md5) && IsCaching)
                isDownload = !parsing(filename);
            if (isDownload)
                WebDownloader.Instance.request(filename, responseBinary);
            else
                updateProgress();
        }
    }

    /// <summary>
    /// 시트 데이터 다운로드 결과를 리턴합니다.
    /// </summary>
    /// <param name="result"></param>
    void responseBinary(WebDownloader.WebDownloaderObject result)
    {
        if (parsing(result.Filename, result.getBytes()))
            save(result.Filename, result.getBytes());

        updateProgress();
    }

    /// <summary>
    /// 저장되어있는 시트 데이터를 로드 합니다.
    /// </summary>
    /// <param name="filename"></param>
    bool parsing(string filename)
    {
        string compress = HPlayerPrefs.GetString(prefix_sheet_data + filename, "");
        if (string.IsNullOrEmpty(compress))
            return false;
        parsing(filename, System.Convert.FromBase64String(compress));
        //parsing(filename, System.Text.Encoding.UTF8.GetBytes(compress));
        return true;
    }

    bool parsing(string filename, byte[] sheet_data)
    {
        string uncompress = Zipper.UnGzipString(sheet_data);
        //Debug.Log("parsing " + filename + " : " + uncompress);
        JSONObject json = new JSONObject(uncompress);
        if (isDebugLog)
            Debug.Log(json.print());

        Dictionary<string, object> dic = json.ToDictionary();
        //m_SheetList.Add(filename, dic);
        m_SheetList[filename] = dic;
        return true;
    }

    void save(string filename, byte[] sheet_data)
    {
        //Debug.Log("save uncompress = " + System.Text.Encoding.UTF8.GetString(data).Length);
        //string compress = Zipper.ZipString(sheet_data);
        string compress = System.Convert.ToBase64String(sheet_data);
        //string compress = System.Text.Encoding.UTF8.GetString(sheet_data);
        if (isDebugLog)
        {
            Debug.Log("sheet_data length = " + sheet_data.Length);
            Debug.Log("save compress length = " + compress.Length);
        }

        // sheet data 를 캐싱합니다.
        HPlayerPrefs.SetString(prefix_sheet_data + filename, compress);
        // md5 를 캐싱합니다.
        HPlayerPrefs.SetString(prefix_sheet_md5 + filename, m_DownloadSheetList[filename].ToString());
    }

    void updateProgress()
    {
        m_DownloadCount++;
        if (AProgressEvent != null)
            AProgressEvent(m_DownloadCount, m_TotalDownloadCount);

        if (m_DownloadCount == m_TotalDownloadCount)
        {
            HPlayerPrefs.Save();
            m_Status = eStatus.Done;
            if (AStatusEvent != null)
                AStatusEvent(m_Status);

            AProgressEvent = null;
            AStatusEvent = null;
        }
    }




}
