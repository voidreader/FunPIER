using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPG.AntiVariable;

public class DataSaveManager : RPGSingleton<DataSaveManager> {

	public class cPlayerPrefsKey
	{
		public const string QuestDataInfo = "QuestDataInfo";
		public const string CharacterData = "CharacterData";
		public const string AnalyticsList = "AnalyticsList";
		public const string GameRetension = "gameRetension";
	}

    const string _PREFIX_DATA = "IvnData_";
    const string _KEY_LIST = "IvnList";

	const string _STAGE_CLEAR = "StageClearList";

	const string _Best_Score = "_Best_Score";
    /// <summary>
    /// 제작 시간.
    /// </summary>
    public const string _KEY_MAKE_TIME = "KeyIvnMakeTime";
    /// <summary>
    /// 맵 이름.
    /// </summary>
    public const string _KEY_MAP_NAME = "KeyIvnMapName";
    /// <summary>
    /// 제작자 아이디.
    /// </summary>
    public const string _KEY_CREATOR_ID = "KeyIvnCreatorID";
    /// <summary>
    /// 서버에 저장 되었다면 서버의 인덱스. 없으면 공백.
    /// </summary>
    public const string _KEY_SERVER_INDEX = "KeyIvnServerIndex";
    /// <summary>
    /// 클리어 여부.
    /// </summary>
    public const string _KEY_IS_CLEAR = "KeyIvnIsClear";

    /// <summary>
    /// 게임 데이터.
    /// </summary>
    public const string _KEY_GAME_DATA = "KeyIvnGameData";

    List<int> m_InventoryList = new List<int>();
    Dictionary<int, object> m_InventoryData = new Dictionary<int, object>();

    /// <summary>
    /// 인벤토리 리스트.
    /// </summary>
    public List<int> InventoryList { get { return m_InventoryList; } }
    /// <summary>
    /// 인벤토리 데이터.
    /// </summary>
    public Dictionary<int, object> InventoryData { get { return m_InventoryData; } }

	private Dictionary<int, object> m_stageClearList = new Dictionary<int, object>();

	public Dictionary<int, object> StageClearList{ get{ return m_stageClearList; } }

	public int m_bestScore = 0;

    /// <summary>
    /// 수정중인 데이터의 인덱스.
    /// 0이면 새로운 데이터 추가.
    /// </summary>
    public int ModifyIndex { get; set; }

	public class cQuestData
	{
		/*
		 * GAME_PLAY : 게임 클리어 최종 스테이지
		 * DEATH_COUNT : 데스 카운드
		 * NO_DEATH_PLAY : 노데스 스테이지 클리어
		/**/
		private HInt32 m_death_count;
		public int DEATH_COUNT
		{
			get
			{
				return m_death_count;
			}
			set
			{
				m_death_count = value;
				SaveData();
			}
		}

		private ArrayList m_QuestInfoList = null;
		public cQuestData( bool _is_Data )
		{
			initData( _is_Data );
			LoadData();
		}

		private void initData( bool _is_Data )
		{
			m_death_count = 0;

			string cacheData = null;

			if ( _is_Data )
				cacheData = !HPlayerPrefs.HasKey( cPlayerPrefsKey.QuestDataInfo ) ? "" : HPlayerPrefs.GetString( cPlayerPrefsKey.QuestDataInfo, "" );
			else
				HPlayerPrefs.DeleteKey( cPlayerPrefsKey.QuestDataInfo );

			Debug.Log( "cQuestDataeData : " + cacheData );
			if ( cacheData == null || cacheData.Length == 0 )
			{
				Debug.Log( "NE_QUEST" );
				ArrayList sheetInfo = RPGSheetManager.Instance.getInfoArray( "NE_QUEST.bin" );
				m_QuestInfoList = new ArrayList();

				foreach ( Dictionary<string, object> dic in sheetInfo )
				{
					//Debug.Log( dic["GOOGLE_QUEST_ID"].ToString() );
					string questId = dic["QUEST_ID"].ToString();
					Debug.Log( "questId : " + questId );
					string complete = "N";
					Dictionary<string, object> questInfo = new Dictionary<string, object>();
					questInfo.Add( "QUEST_ID", questId );
					questInfo.Add( "COMPLETE", complete );
					m_QuestInfoList.Add( questInfo );
				}
				SaveData();
			}
		}
		public void LoadData()
		{
			if ( m_QuestInfoList != null )
				m_QuestInfoList.Clear();
			string result = HPlayerPrefs.GetString( cPlayerPrefsKey.QuestDataInfo );
			Debug.Log( "result : " + result );
			JSONObject json_result = new JSONObject( result );
			Dictionary<string, object> info = RPGDefine.JsonToDictionary( json_result );
			Dictionary<string, object> questInfo = RPGDefine.JsonToDictionary( new JSONObject( info["info"].ToString() ) );
			m_death_count = int.Parse( questInfo["m_death_count"].ToString() );

			m_QuestInfoList = RPGDefine.JsonToArray( new JSONObject( info["questInfo"].ToString() ) );

		}

		public void SaveData()
		{
			JSONObject data = new JSONObject();
			JSONObject infoData = new JSONObject();
			infoData.AddField( "m_death_count", m_death_count.ToString() );

			data.AddField( "info", infoData.print() );
			//*
			JSONObject questInfoList = new JSONObject();
			foreach ( Dictionary<string, object> questDic in m_QuestInfoList )
			{
				JSONObject questInfo = new JSONObject();
				string questId = questDic["QUEST_ID"].ToString();
				string complete = questDic["COMPLETE"].ToString();
				questInfo.AddField( "QUEST_ID", questId );
				questInfo.AddField( "COMPLETE", complete );
				questInfoList.Add( questInfo );
			}
			data.AddField( "questInfo", questInfoList.print() );

			/**/
			HPlayerPrefs.SetString( cPlayerPrefsKey.QuestDataInfo, data.print() );
			HPlayerPrefs.Save();
		}

		/// <summary>
		/// 퀘스트 완료 체크. 
		/// </summary>
		/// <param name="_questId"></param>
		/// <returns></returns>
		public bool CheckQuestComplete( string _questId )
		{
			if ( _questId.Equals( "NONE" ) )
				return true;

			foreach ( Dictionary<string, object> dic in m_QuestInfoList )
			{
				string questId = dic["QUEST_ID"].ToString();
				string complete = dic["COMPLETE"].ToString();
				if ( questId.Equals( _questId ) )
					return complete.Equals( "Y" ) ? true : false;
			}
			return false;
		}

		/// <summary>
		/// 퀘스트 완료 입력.
		/// </summary>
		/// <param name="_questId"></param>
		public void QuestComplete( string _questId )
		{
			foreach ( Dictionary<string, object> dic in m_QuestInfoList )
			{
				string questId = dic["QUEST_ID"].ToString();
				if ( questId.Equals( _questId ) )
					dic["COMPLETE"] = "Y";
			}
		}
	}

	public cQuestData m_questData;

	public long gameRetension;
	private string m_charId;
	public string CharId{ get { return m_charId; }
	
		set
		{
			m_charId = value;
			HPlayerPrefs.SetString( cPlayerPrefsKey.CharacterData, m_charId );
		}
	}

	private ArrayList analyticsList;

    public void DataInit()
    {
        base.Init();
		m_questData = new cQuestData( true );
		m_bestScore = HPlayerPrefs.GetInt( _Best_Score, 0 );
		m_charId = HPlayerPrefs.GetString( cPlayerPrefsKey.CharacterData, "Character_000" );

		string cacheData = HPlayerPrefs.GetString( cPlayerPrefsKey.AnalyticsList, "" );
		Debug.Log( "cacheData : " + cacheData );
		if ( cacheData == null || cacheData.Length == 0 )
		{
			analyticsList = new ArrayList();
		}
		else
		{
			JSONObject jsonList = new JSONObject(cacheData);
			analyticsList = jsonList.ToArray();
		}
		string retension = HPlayerPrefs.GetString( cPlayerPrefsKey.GameRetension, "" );
		if ( retension == null || retension.Length == 0 )
			gameRetension = 0L;
		else
			gameRetension = long.Parse( retension );

		ModifyIndex = 0;
        Load();
    }

	public void SetGameRetension( long _time )
	{
		gameRetension = _time;
		HPlayerPrefs.SetString( cPlayerPrefsKey.GameRetension, gameRetension.ToString() );
	}

	public bool CheckAnalytics( string _stageId )
	{
		for( int index = 0; index < analyticsList.Count; ++index )
		{
			if ( analyticsList[index].ToString().Equals( _stageId ) )
				return true;
		}
		return false;
	}

	public void SetAnalytics( string _stageId )
	{
		analyticsList.Add( _stageId );
		JSONObject jsonList = new JSONObject( JSONObject.Type.ARRAY );
		for( int index = 0; index < analyticsList.Count; ++index )
			jsonList.Add( analyticsList[index].ToString() );
		HPlayerPrefs.SetString( cPlayerPrefsKey.AnalyticsList, jsonList.print() );
	}

	private bool scoreLeaderboard = false;

	private List<int> m_LeaderboardScoreList = new List<int>();

	public void SetBestScore( int _score )
	{
		m_bestScore = _score;
		HPlayerPrefs.SetInt( _Best_Score, m_bestScore );
		
//#if UNITY_EDITOR
//#elif UNITY_ANDROID
		m_LeaderboardScoreList.Add( m_bestScore );
//#endif
	}
	void Update()
	{
        if (!scoreLeaderboard && m_LeaderboardScoreList.Count > 0)
        {
            scoreLeaderboard = true;

            //AndroidManager.GetInstance.SetScore(m_LeaderboardScoreList[0], null, OnSetScoreCompletion);

            GooglePlayManager.ActionScoreSubmited += OnSetScoreCompletion;

#if UNITY_EDITOR
#elif UNITY_ANDROID
			AndroidManager.GetInstance.SetScore( m_LeaderboardScoreList[0], null);
#elif UNITY_IOS
            Social.ReportScore(m_LeaderboardScoreList[0], "Score_Leaderboard", OnSetScoreCompletion);
#endif
        }
	}
	private void OnSetScoreCompletion(GP_LeaderboardResult result)	{
        GooglePlayManager.ActionScoreSubmited -= OnSetScoreCompletion;

        m_LeaderboardScoreList.Remove( m_LeaderboardScoreList[0] );
		scoreLeaderboard = false;
	}



    /// <summary>
    /// PlayerPrefs에 저장된 데이터를 가져옵니다.
    /// </summary>
    void Load()
    {
        m_InventoryList.Clear();
        m_InventoryData.Clear();
		m_stageClearList.Clear();

        string listStr = HPlayerPrefs.GetString(_KEY_LIST, "");
        if (listStr.Length > 0)
        {
            JSONObject jsonList = new JSONObject(listStr);
            ArrayList list = jsonList.ToArray();
            for (int i=0; i<list.Count; i++)
            {
                int index = int.Parse(list[i].ToString());
                m_InventoryList.Add(index);
                JSONObject jsonData = new JSONObject(HPlayerPrefs.GetString(_PREFIX_DATA + index.ToString()));
				Debug.Log( "jsonData : " + jsonData.print() );
                m_InventoryData[index] = jsonData.ToDictionary();
            }
        }
		string listStageClear = HPlayerPrefs.GetString( _STAGE_CLEAR, "" );
		if ( listStageClear.Length > 0 && listStageClear != null && !listStageClear.Equals( "null" ) )
		{
			JSONObject jsonList = new JSONObject( listStageClear );
			Dictionary<string, object> list = jsonList.ToDictionary();
			foreach( KeyValuePair<string, object> pair in list )
			{
				Dictionary<string, object> dic = ( Dictionary<string, object> )pair.Value;
				m_stageClearList.Add( int.Parse( pair.Key ), pair.Value );
			}
		}
    }

	public Dictionary<string, object> GetStageClearInfo( int _stageNum )
	{
		if ( m_stageClearList.ContainsKey( _stageNum ) )
			return ( Dictionary<string, object> )m_stageClearList[_stageNum];
		return null;
	}

	public void SetStageClearInfo( int _stageNum, int _playTime, int _death )
	{
		if( m_stageClearList.ContainsKey( _stageNum ) )
		{
			Dictionary<string, object> stageInfo = ( Dictionary<string, object> )m_stageClearList[_stageNum];
			int playTime = int.Parse( stageInfo["playTime"].ToString() );
			if( playTime > _playTime )
			{
				stageInfo["stageNum"] = _stageNum;
				//stageInfo["stageName"] = _stageName;
				stageInfo["playTime"] = _playTime;
				stageInfo["death"] = _death;
				SaveStageClearInfo();
			}
			else if( playTime == _playTime )
			{
				int death = int.Parse( stageInfo["death"].ToString() );
				if ( death > _death )
				{
					stageInfo["stageNum"] = _stageNum;
					//stageInfo["stageName"] = _stageName;
					stageInfo["playTime"] = _playTime;
					stageInfo["death"] = _death;
					SaveStageClearInfo();
				}
			}
		}
		else
		{
			Dictionary<string, object> stageInfo = new Dictionary<string, object>();
			stageInfo.Add( "stageNum", _stageNum );
			//stageInfo.Add( "stageName : ", _stageName );
			stageInfo.Add( "playTime", _playTime );
			stageInfo.Add( "death", _death );
			m_stageClearList.Add( _stageNum, stageInfo );
			SaveStageClearInfo();
		}
	}

	void SaveStageClearInfo()
	{
		JSONObject jsonList = new JSONObject();

		foreach ( KeyValuePair<int, object> pair in m_stageClearList )
		{
			JSONObject info = new JSONObject();
			Dictionary<string, object> dic = ( Dictionary<string, object> )pair.Value;
			info.AddField( "stageNum", dic["stageNum"].ToString() );
			//info.AddField( "stageName", dic["stageName"].ToString() );
			info.AddField( "playTime", dic["playTime"].ToString() );
			info.AddField( "death", dic["death"].ToString() );

			jsonList.AddField( pair.Key.ToString(), info );
		}
		HPlayerPrefs.SetString( _STAGE_CLEAR, jsonList.print() );
	}

    /// <summary>
    /// 인벤토리 리스트를 m_InventoryList와 동기화 하여 저장합니다.
    /// </summary>
    void SaveList()
    {
        JSONObject jsonList = new JSONObject(JSONObject.Type.ARRAY);
        for (int i = 0; i < m_InventoryList.Count; i++)
            jsonList.AddField(m_InventoryList[i]);
        HPlayerPrefs.SetString(_KEY_LIST, jsonList.print());
    }

    /// <summary>
    /// PlayerPrefs에 데이터를 저장합니다.
    /// </summary>
    void Save(JSONObject json)
    {
        if (ModifyIndex > 0)
        {
            // 기존의 데이터를 수정합니다.
            m_InventoryData[ModifyIndex] = json.ToDictionary();
            HPlayerPrefs.SetString(_PREFIX_DATA + ModifyIndex.ToString(), json.print());
        }
        else
        {
            // 새로운 데이터를 추가합니다.
            int lastIndex = 1;
            if (InventoryList.Count > 0)
                lastIndex = InventoryList[InventoryList.Count - 1] + 1;

            m_InventoryList.Add(lastIndex);
            SaveList();

            m_InventoryData[lastIndex] = json.ToDictionary();
            HPlayerPrefs.SetString(_PREFIX_DATA + lastIndex.ToString(), json.print());
        }
        HPlayerPrefs.Save();
    }

    /// <summary>
    /// 게임을 인벤토리에 저장합니다.
    /// </summary>
    /// <param name="_data"></param>
    /// <param name="_mapName"></param>
    public void Save(string _data, string _mapName, string _serverIndex="")
    {
        //Time.time
        JSONObject json = new JSONObject();
        json.AddField(_KEY_MAKE_TIME, RPGDefine.getNowTimeStamp().ToString());
        json.AddField(_KEY_MAP_NAME, _mapName);
        json.AddField(_KEY_CREATOR_ID, NENetworkManager.Instance.UserID);
        json.AddField(_KEY_GAME_DATA, _data);
        json.AddField(_KEY_SERVER_INDEX, _serverIndex);
        json.AddField(_KEY_IS_CLEAR, "0");

        Save(json);
    }

    public void Modify(string _data)
    {
        Dictionary<string, object> dic = m_InventoryData[ModifyIndex] as Dictionary<string, object>;
        dic[_KEY_GAME_DATA] = _data;
        dic[_KEY_IS_CLEAR] = "0";

        JSONObject json = new JSONObject();
        foreach (KeyValuePair<string, object> pair in dic)
            json.AddField(pair.Key, pair.Value.ToString());

        Save(json);
    }

    /// <summary>
    /// 인벤토리에 저장된 맵을 삭제합니다.
    /// </summary>
    /// <param name="index"></param>
    public void Delete(int index)
    {
        m_InventoryList.Remove(index);
        SaveList();

        m_InventoryData.Remove(index);
        HPlayerPrefs.DeleteKey(_PREFIX_DATA + index.ToString());
        HPlayerPrefs.Save();
    }

    /// <summary>
    /// ModifyIndex 데이터를 클리어 시킵니다.
    /// </summary>
    public void Clear()
    {
        // 만든 맵을 플레이중이 아니라면 서버로 클리어 처리 합니다.
        //if (ModifyIndex == 0)
        //{
        //    ServerDataManager.Instance.Clear();
        //    return;
        //}
        Dictionary<string, object> dic = m_InventoryData[ModifyIndex] as Dictionary<string, object>;
        dic[_KEY_IS_CLEAR] = "1";

        JSONObject json = new JSONObject();
        foreach (KeyValuePair<string, object> pair in dic)
            json.AddField(pair.Key, pair.Value.ToString());

        Save(json);

        //GameManager.Instance.exitInGame();
        //UICustomMyPage.show(0, 2);
    }

    public void Upload(int index, System.Action<WebObject> selector)
    {
        Dictionary<string, object> dic = m_InventoryData[index] as Dictionary<string, object>;
        string data = ParsingDictionary.ToString(dic, _KEY_GAME_DATA);
        string mapName = ParsingDictionary.ToString(dic, _KEY_MAP_NAME);

        rpgames.game.RequestMapSave request = new rpgames.game.RequestMapSave();
        {
            request.request = WebConnector.Instance.CommonRequest;
            //request.owner = NENetworkManager.Instance.NickName;
            //request.owner = NENetworkManager.Instance.UserID;
            request.map_name = mapName;
            request.data = data;
        }
        WebObject wo = new WebObject((w) =>
        {
            rpgames.game.ResponseMapSave response = w.getResult<rpgames.game.ResponseMapSave>();
            if (response.response.success)
            {
                NENetworkManager.Instance.Gold = response.gold;
                dic[_KEY_SERVER_INDEX] = response.map.idx;
                JSONObject json = new JSONObject();
                foreach (KeyValuePair<string, object> pair in dic)
                    json.AddField(pair.Key, pair.Value.ToString());

                ModifyIndex = index;
                Save(json);
                ModifyIndex = 0;

                if (selector != null)
                    selector(w);
            }
            else
            {
                Debug.LogError("map_save Error = " + response.response.error_code.ToString());
            }

        });
        wo.setData<rpgames.game.RequestMapSave>(request);
        wo.setCommand("map_save");
        WebConnector.Instance.request(wo);
    }
	/// <summary>
	/// 퀘스트 완료 체크. 
	/// </summary>
	public void CheckQuestListComplete()
	{
		ArrayList sheetInfo = RPGSheetManager.Instance.getInfoArray( "NE_QUEST.bin" );
		List<string> googleQuestIdList = new List<string>();

		foreach ( Dictionary<string, object> dic in sheetInfo )
		{
			string questId = dic["QUEST_ID"].ToString();
			if ( !m_questData.CheckQuestComplete( questId ) )
			{
				string needQuestId = dic["NEED_QUEST_ID"].ToString();
				if ( m_questData.CheckQuestComplete( needQuestId ) )
				{
					string questType = dic["QUEST_TYPE"].ToString();
					int value = int.Parse( dic["TYPE_VALUE"].ToString() );
					float per = 0f;
					if ( CheckQuestComplete( questType, value, out per ) )
					{
						Debug.Log( "questId : " + questId );
						m_questData.QuestComplete( questId );
#if UNITY_ANDROID
						googleQuestIdList.Add( dic["GOOGLE_QUEST_ID"].ToString() );
#elif UNITY_IOS
                        googleQuestIdList.Add(dic["APPLE_QUEST_ID"].ToString());
#endif
					}
				}
			}
		}
#if UNITY_EDITOR
#elif UNITY_ANDROID
		for ( int index = 0; index < googleQuestIdList.Count; ++index )
			AndroidManager.GetInstance.AchievementStepUp( googleQuestIdList[index]);
#elif UNITY_IOS
        for (int index = 0; index < googleQuestIdList.Count; ++index)
            Social.ReportProgress(googleQuestIdList[index], 100f, QuestCallBack);
#endif

        m_questData.SaveData();
	}

	/// <summary>
	/// 퀘스트 진행도 및 완료를 체크 한다. 
	/// </summary>
	/// <param name="_questType"></param>
	/// <param name="_value"></param>
	/// <param name="_per"></param>
	/// <returns></returns>
	public bool CheckQuestComplete( string _questType, int _value, out float _per )
	{
		_per = 0f;
		int playerValue = 0;
		switch ( _questType )
		{
		case "GAME_PLAY":
		playerValue = StageClearInfo();
		break;
		case "DEATH_COUNT":
		playerValue = m_questData.DEATH_COUNT;
		break;
		case "NO_DEATH_PLAY":
		playerValue = StageClearInfo();
		break;
		}
//		Debug.Log( _questType + " : " + playerValue );

		_per = ( ( float )playerValue / ( float )_value > 1f ? 1f : ( float )playerValue / ( float )_value ) * 100f;
		return playerValue >= _value;
	}

	public int StageClearInfo()
	{
		int stageInfoIndex = 0;
		for ( int index = 0; index < GameManager.Instance.STAGE_LIST.Count; ++index )
		{
			if ( DataSaveManager.Instance.GetStageClearInfo( ( index ) ) != null )
				stageInfoIndex = index;
		}

		return stageInfoIndex;
	}

	public int stageClearDeathCount()
	{
		int count = 0;
		for ( int index = 0; index < GameManager.Instance.STAGE_LIST.Count; ++index )
		{
			Dictionary<string, object> dic = DataSaveManager.Instance.GetStageClearInfo( ( index + 1 ) );
			if ( dic == null )
				return count;
			if ( int.Parse( dic["death"].ToString() ) == 0 )
				count++;
		}
		return count;
	}
}
