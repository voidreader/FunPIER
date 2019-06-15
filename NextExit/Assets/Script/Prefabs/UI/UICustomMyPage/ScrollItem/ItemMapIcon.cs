using UnityEngine;
using System.Collections;

public class ItemMapIcon : UIScrollPrefab
{
    public class ItemData
    {
        public bool IsNew;
        /// <summary>
        /// Index가 -1인 경우에는 서버에만 데이터가 있는 경우이다.
        /// </summary>
        public int Index;
        /// <summary>
        /// Length == 0 이면 서버에 데이터가 없는 경우이다.
        /// </summary>
        public string ServerIndex;
        public string CreateTime;
        public string MapName;
        public string CreatorID;
        public string Nickname;
        public float DifficultyProgress;
        public float ScoreProgress;
        public int PlayCount;
        public int ClearCount;
        public string BestID;
        public int BestTime;
        public int BestDeath;
        public bool IsFirstPlay;    // 게임을 다운로드 받을때 골드가 소모됨.
        public bool IsSelected;
        /// <summary>
        /// 맵의 데이터.
        /// 서버에서 가져오는 경우에는 서버에서 가져온 후 추가로 저장합니다.
        /// </summary>
        public string MapData = "";

        public ItemData()
        {
            IsNew = true;
        }

        public ItemData(int _Index, string _ServerIndex, string _CreateTime, string _MapName, string _CreatorID, string _Nickname, float _DifficultyProgress, float _ScoreProgress, 
            int _PlayCount, int _ClearCount, string _BestID, int _BestTime, int _BestDeath,
            bool _IsFirstPlay, bool _IsSelected)
        {
            IsNew = false;
            Index = _Index;
            ServerIndex = _ServerIndex;
            CreateTime = _CreateTime;
            MapName = _MapName;
            CreatorID = _CreatorID;
            Nickname = _Nickname;
            DifficultyProgress = _DifficultyProgress;
            ScoreProgress = _ScoreProgress;
            PlayCount = _PlayCount;
            ClearCount = _ClearCount;
            BestID = _BestID;
            BestTime = _BestTime;
            BestDeath = _BestDeath;
            IsFirstPlay = _IsFirstPlay;
            IsSelected = _IsSelected;
        }
    }

    GameObject m_GoMapData;
    GameObject m_GoNew;

    //tk2dUIToggleButton m_ToggleBase;
    GameObject m_GoToggleOn;
    GameObject m_GoToggleOff;

    RPGTextMesh m_TextDate;
    RPGTextMesh m_TextMapName;
    RPGTextMesh m_TextCreatorID;

    tk2dUIProgressBar m_ProgressDifficulty;
    tk2dClippedSprite m_SpriteDifficulty;
    tk2dUIProgressBar m_ProgressScore;

    tk2dUIItem m_ToggleButton;
    public tk2dUIItem ToggleButton
    {
        get
        {
            if (m_ToggleButton == null)
                m_ToggleButton = transform.Find("MapData/BTN/btn_Base").GetComponent<tk2dUIItem>();
            return m_ToggleButton;                
        }
    }

    public override void ObjFind()
    {
        base.ObjFind();

        Transform MapData = transform.Find("MapData");
        m_GoMapData = MapData.gameObject;
        m_GoNew = transform.Find("New").gameObject;

        //m_ToggleBase = MapData.FindChild("BTN/btn_Base").GetComponent<tk2dUIToggleButton>();
        m_GoToggleOn = MapData.Find("BTN/btn_Base/On").gameObject;
        m_GoToggleOff = MapData.Find("BTN/btn_Base/Off").gameObject;
        m_TextDate = MapData.Find("TEXT/text_Date").GetComponent<RPGTextMesh>();
        m_TextMapName = MapData.Find("TEXT/text_MapName").GetComponent<RPGTextMesh>();
        m_TextCreatorID = MapData.Find("TEXT/text_CreatorID").GetComponent<RPGTextMesh>();

        m_ProgressDifficulty = MapData.Find("PROGRESS/Difficulty").GetComponent<tk2dUIProgressBar>();
        m_SpriteDifficulty = MapData.Find("PROGRESS/Difficulty/Bar").GetComponent<tk2dClippedSprite>();
        m_ProgressScore = MapData.Find("PROGRESS/Score").GetComponent<tk2dUIProgressBar>();

        m_GoToggleOn.SetActive(false);
        m_GoToggleOff.SetActive(true);
    }

    public override void Parsing(object data_)
    {
        base.Parsing(data_);

        ItemData data = data_ as ItemData;
        if (data.IsNew)
        {
            m_GoNew.SetActive(true);
            m_GoMapData.SetActive(false);
        }
        else
        {
            m_GoNew.SetActive(false);
            m_GoMapData.SetActive(true);

            System.DateTime date = RPGDefine.convertTimeStampToDateTimeByLanguage(long.Parse(data.CreateTime));            

            m_TextDate.Text = date.ToString("yyyy.MM.dd");
            m_TextMapName.Text = data.MapName;
            m_TextCreatorID.Text = data.Nickname;

            m_ProgressDifficulty.Value = data.DifficultyProgress;
            m_ProgressScore.Value = data.ScoreProgress;
            if (data.DifficultyProgress < 0.5f)
                m_SpriteDifficulty.SetSprite("ui_mypage_gage_green");
            else if (data.DifficultyProgress <= 0.8f)
                m_SpriteDifficulty.SetSprite("ui_mypage_gage_yellow");
            else
                m_SpriteDifficulty.SetSprite("ui_mypage_gage_red");

            m_GoToggleOn.SetActive(data.IsSelected);
            m_GoToggleOff.SetActive(!data.IsSelected);
        }
    }

    /// <summary>
    /// 새로운 커스텀맵을 생성합니다.
    /// </summary>
    void OnBtnNew()
    {
        GameManager.Instance.startCustom(false);
    }


}
