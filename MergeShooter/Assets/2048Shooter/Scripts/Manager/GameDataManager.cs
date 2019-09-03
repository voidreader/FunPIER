using System;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;

public class GameDataManager : MonoSingleton<GameDataManager>
{
    public static readonly int MaxLevel = 50;

    public const float SpeedChangeScale = 1.2f;

    public const float SpeedChangeScale1 = 1.5f;

    public const float SpeedChangeScale2 = 1.8f;

    public const float DefNormalDownSpeed = 2.4f;

    public const float DefHardDownSpeed = 2.7f;

    public const float DefNormalHitValue = 12f;

    public const float DefHardHitValue = 10f;

    private const string C_GAMEMODE = "GAMEMODE";

    private const string C_GAMELEVEL = "GAMELEVEL";

    private const string C_HARDUNLOCK = "C_HARDUNLOCK";

    private const string C_LEVELUNLOCK = "C_HARDUNLOCK";

    private const string C_MAXSCORE = "MAXSCORE";

    private const string C_SCORE = "SCORE";

    private const string C_DOWNVALUE = "DOWNVALUE";

    private const string C_HITVALUE = "HITVALUE";

    private const string C_DOWNSPEED = "DOWNSPEED";

    private const string C_COMBINEVALUE = "COMBINEVALUE";

    private const string C_COURSE = "COURSE";

    private const string C_DAY = "DAY";

    private const string C_NORMALMAXGAMETIMES = "NORMALMAXGAMETIMES";

    private const string C_HARDMAXGAMETIMES = "HARDMAXGAMETIMES";

    private const string C_HARDSTARTTIMES = "HARDSTARTTIMES";

    private const string C_BRICKCOUNT = "BRICKCOUNT";

    private const string C_EVERYGAMETIME = "EVERYGAMETIME";

    private const string C_MONEY = "MONEY";

    private const string C_GETMONEY = "GETMONEY";

    private const string C_USESKIN = "USESKIN";

    private const string C_UNLOCKSKIN = "UNLOCKSKIN";

    private const string C_GRADE = "GRADE";

    private const string C_GRADETIMER = "GRADETIMERNEW";

    private const string C_PHONESHAKE = "PHONESHAKE";

    private const string C_AUDIO = "AUDIO";

    private const string C_AD = "AD";

    private const string C_GAMEDAY = "GAMEDAY";

    private const string C_DAYTWOSEND = "DAYTWOSEND";

    private const string C_GALOGINSTATE = "GALOGINSTATE";

    private const string C_BOOLCONTINUEGAME = "BOOLCONTINUEGAME";

    private const string C_CONTINUEVIDEO = "ONTINUEVIDEO";

    private const string C_BIGENDVIDEO = "BIGENDVIDEO";

    private const string C_RESUMEVIDEO = "RESUMEVIDEO";

    private const string C_PAUSEVIDEO = "PAUSEVIDEO";

    private const string C_OPENVIDEO = "OPENVIDEO";

    private const string C_PROCESSOPEN = "PROCESSOPEN";

    private const string C_PROCESSFIRSTSHOOT = "C_PROCESSFIRSTSHOOT";

    private const string C_ISOPENACHIEVE = "C_ISOPENACHIEVE";

    private const string C_PROCESS_2048 = "C_PROCESS_2048";

    private const string C_PROCESS_4096 = "C_PROCESS_4096";

    private const string C_PROCESS_8192 = "C_PROCESS_8192";

    private const string C_FIRSTTOUCH = "FIRSTTOUCH";

    private const string C_PROCESS_COVER = "PROCESS_COVER";

    private const string C_BRICKNUMDIC = "BRICKNUMDIC";

    [SerializeField]
    public float m_DownSpeedDef = 3.4f;

    private float m_CombineValueDef = 6f;

    private int m_Day;

    private int m_VideoTime;

    private int m_GameDay;

    [HideInInspector]
    public int m_process2048;

    [HideInInspector]
    public int m_process4096;

    [HideInInspector]
    public int m_process8192;

    [HideInInspector]
    public int m_isFirstTouch;

    [HideInInspector]
    public bool m_process_cover;

    private int m_PlayMode;

    private int m_GameMode;

    private int m_GameLevel = 1;

    private int m_HardUnlock;

    private int m_LevelUnlock;

    private int m_MaxScore;

    private int m_Score;

    private float m_DownValue;

    private float m_HitValue;

    [SerializeField]
    private float m_DownSpeed;

    private float m_CombineValue;

    private bool m_Course;

    private int m_NormalMaxGameTimes;

    private int m_HardMaxGameTimes;

    private int m_HardStartTimes;

    private bool m_PhoneShake = false;

    private bool m_Audio = true;

    private bool m_AD = true;

    private int m_Money;

    private int m_getMoney;

    private bool m_Grade;

    private int m_GradeTimer;

    private int m_BrickCount;

    private SkinType m_UseSkinType = SkinType.DAY;

    private int m_ContinueVideo;

    private int m_BigEndVideo;

    private int m_ResumVideo;

    private int m_PauseVideo;

    private int m_OpenVideo;

    private int m_MiddleVideo;

    private int m_BoolContineGame;

    private int m_ProcessOpen;

    private int m_ProcessFirstShoot;

    private int m_isOpenAchieve;

    private Dictionary<int, List<int>> m_BricksInfo = new Dictionary<int, List<int>>();

    private Dictionary<int, int> m_BrickNumDic = new Dictionary<int, int>();

    private Dictionary<int, List<int>> m_BrickItemsInfo = new Dictionary<int, List<int>>();

    private Dictionary<int, int> m_BrickCombineCount = new Dictionary<int, int>();

    [SerializeField]
    private List<ValuePer> m_ValueList = new List<ValuePer>();

    private Dictionary<SkinType, bool> m_SkinListState = new Dictionary<SkinType, bool>();

    private int m_GALoginState;
    
    // FIXME 0. Classic Mode, 1. Challenge Mode 추가
    public int PlayMode
    {
        get
        {
            return this.m_PlayMode;
        }
    }

    public int GameMode
    {
        get
        {
            return this.m_GameMode;
        }
    }

    public int GameLevel
    {
        get
        {
            return this.m_GameLevel;
        }
    }

    public int HardUnlock
    {
        get
        {
            return this.m_HardUnlock;
        }
        set
        {
            this.m_HardUnlock = value;
        }
    }

    public int LevelUnlock
    {
        get
        {
            return this.m_LevelUnlock;
        }
        set
        {
            this.m_LevelUnlock = value;
        }
    }

    public int MaxScore
    {
        get
        {
            return this.m_MaxScore;
        }
    }

    public int Score
    {
        get
        {
            return this.m_Score;
        }
    }

    public float DownValue
    {
        get
        {
            return this.m_DownValue;
        }
    }

    public float HitValue
    {
        get
        {
            return this.m_HitValue;
        }
    }

    public float DownSpeed
    {
        get
        {
            return this.m_DownSpeed;
        }
    }

    public float CombineValue
    {
        get
        {
            return this.m_CombineValue;
        }
    }

    public bool Course
    {
        get
        {
            return this.m_Course;
        }
    }

    public int NormalMaxGameTimes
    {
        get
        {
            return this.m_NormalMaxGameTimes;
        }
    }

    public int HardMaxGameTimes
    {
        get
        {
            return this.m_HardMaxGameTimes;
        }
    }

    public int HardStartGameTimes
    {
        get
        {
            return this.m_HardStartTimes;
        }
        set
        {
            this.m_HardStartTimes = value;
        }
    }

    public bool PhoneShake
    {
        get
        {
            return this.m_PhoneShake;
        }
    }

    public bool Audio
    {
        get
        {
            return this.m_Audio;
        }
    }

    public bool AD
    {
        get
        {
            return this.m_AD;
        }
    }

    public int Money
    {
        get
        {
            return this.m_Money;
        }
    }

    public int GetMoney
    {
        get
        {
            return this.m_getMoney;
        }
    }

    public bool Grade
    {
        get
        {
            return this.m_Grade;
        }
    }

    public int GradeTimer
    {
        get
        {
            return this.m_GradeTimer;
        }
    }

    public int BrickCount
    {
        get
        {
            return this.m_BrickCount;
        }
    }

    public SkinType UseSkinType
    {
        get
        {
            return this.m_UseSkinType;
        }
    }

    public int ContineuVideo
    {
        get
        {
            this.m_ContinueVideo++;
            if (!this.CommonDay())
            {
                this.m_ContinueVideo = 1;
            }
            return this.m_ContinueVideo;
        }
    }

    public int BigEndVideo
    {
        get
        {
            this.m_BigEndVideo++;
            if (!this.CommonDay())
            {
                this.m_BigEndVideo = 1;
            }
            return this.m_BigEndVideo;
        }
    }

    public int ResumVideo
    {
        get
        {
            this.m_ResumVideo++;
            if (!this.CommonDay())
            {
                this.m_ResumVideo = 1;
            }
            return this.m_ResumVideo;
        }
    }

    public int PauseVideo
    {
        get
        {
            this.m_PauseVideo++;
            if (!this.CommonDay())
            {
                this.m_PauseVideo = 1;
            }
            return this.m_PauseVideo;
        }
    }

    public int OpenVideo
    {
        get
        {
            this.m_OpenVideo++;
            if (!this.CommonDay())
            {
                this.m_OpenVideo = 1;
            }
            return this.m_OpenVideo;
        }
    }

    public int MiddleVideo
    {
        get
        {
            this.m_MiddleVideo++;
            if (!this.CommonDay())
            {
                this.m_MiddleVideo = 1;
            }
            return this.m_MiddleVideo;
        }
    }

    public bool BoolContiueGame
    {
        get
        {
            return this.m_BoolContineGame == 1;
        }
    }

    public bool ProcessOpen
    {
        get
        {
            return this.m_ProcessOpen == 0;
        }
    }

    public bool ProcessFirstShoot
    {
        get
        {
            return this.m_ProcessFirstShoot == 0;
        }
    }

    public int IsOpenAchieve
    {
        get
        {
            return this.m_isOpenAchieve;
        }
        set
        {
            this.m_isOpenAchieve = value;
        }
    }

    public Dictionary<int, List<int>> BricksInfo
    {
        get
        {
            return new Dictionary<int, List<int>>(this.m_BricksInfo);
        }
    }

    public Dictionary<int, int> BrickNumDic
    {
        get
        {
            return this.m_BrickNumDic;
        }
    }

    public Dictionary<int, List<int>> BrickItemsInfo
    {
        get
        {
            return new Dictionary<int, List<int>>(this.m_BrickItemsInfo);
        }
    }

    public Dictionary<int, int> BrickCombineCount
    {
        get
        {
            return this.m_BrickCombineCount;
        }
    }

    public List<ValuePer> ValueList
    {
        get
        {
            return new List<ValuePer>(this.m_ValueList);
        }
    }

    public override void Init()
    {
        foreach (SkinInfo current in MonoSingleton<ConfigeManager>.Instance.Config)
        {
            this.m_SkinListState.Add(current.Type, current.Price == 0);
        }
        this.Load();
    }

    private void Load()
    {
        this.m_BricksInfo.Clear();
        this.m_BrickItemsInfo.Clear();
        this.m_BrickCombineCount.Clear();

        string @string = PlayerPrefs.GetString("UNLOCKSKIN");
        string[] array = @string.Split(new char[]
        {
            ':'
        });
        string[] array2 = array;
        for (int i = 0; i < array2.Length; i++)
        {
            string value = array2[i];
            if (!string.IsNullOrEmpty(value))
            {
                SkinType key = (SkinType)Enum.Parse(typeof(SkinType), value);
                if (this.m_SkinListState.ContainsKey(key))
                {
                    this.m_SkinListState[key] = true;
                }
            }
        }
        string string2 = PlayerPrefs.GetString("USESKIN", SkinType.NOTEBOOK.ToString());
        if (Enum.IsDefined(typeof(SkinType), string2))
        {
            this.m_UseSkinType = (SkinType)Enum.Parse(typeof(SkinType), string2);
        }
        else
        {
            this.m_UseSkinType = SkinType.NOTEBOOK;
        }
        //change style;
        this.m_UseSkinType = SkinType.NOTEBOOK;

        this.m_PlayMode = PlayerPrefs.GetInt("PLAYMODE", 0);
        this.m_GameMode = PlayerPrefs.GetInt("GAMEMODE", 0);
        this.m_GameLevel = PlayerPrefs.GetInt("GAMELEVEL", 1);
        this.m_isOpenAchieve = PlayerPrefs.GetInt("C_ISOPENACHIEVE", 0);
        this.m_MaxScore = PlayerPrefs.GetInt("MAXSCORE", 0);
        int @int = PlayerPrefs.GetInt("MaxScore", 0);
        this.m_MaxScore = Mathf.Max(this.m_MaxScore, @int);
        this.m_Score = PlayerPrefs.GetInt("SCORE", this.m_Score);
        //this.m_Course = (PlayerPrefs.GetInt("COURSE", 0) == 1);
        this.m_Course = true;
        this.m_Day = PlayerPrefs.GetInt("DAY", 0);
        this.m_NormalMaxGameTimes = PlayerPrefs.GetInt("NORMALMAXGAMETIMES", 0);
        this.m_HardMaxGameTimes = PlayerPrefs.GetInt("HARDMAXGAMETIMES", 0);
        this.m_HardStartTimes = PlayerPrefs.GetInt("HARDSTARTTIMES", 0);
        this.m_process2048 = PlayerPrefs.GetInt("C_PROCESS_2048", 0);
        this.m_process4096 = PlayerPrefs.GetInt("C_PROCESS_4096", 0);
        this.m_process8192 = PlayerPrefs.GetInt("C_PROCESS_8192", 0);
        this.m_process_cover = (PlayerPrefs.GetInt("PROCESS_COVER", 0) == 1);
        this.m_VideoTime = PlayerPrefs.GetInt("DAYTWOSEND", 0);
        this.m_GALoginState = PlayerPrefs.GetInt("GALOGINSTATE", 0);
        this.m_GameDay = PlayerPrefs.GetInt("GAMEDAY", 0);
        this.m_BoolContineGame = PlayerPrefs.GetInt("BOOLCONTINUEGAME", 0);
        this.m_ContinueVideo = PlayerPrefs.GetInt("ONTINUEVIDEO", 0);
        this.m_BigEndVideo = PlayerPrefs.GetInt("BIGENDVIDEO", 0);
        this.m_ResumVideo = PlayerPrefs.GetInt("RESUMEVIDEO", 0);
        this.m_PauseVideo = PlayerPrefs.GetInt("PAUSEVIDEO", 0);
        this.m_OpenVideo = PlayerPrefs.GetInt("OPENVIDEO", 0);
        this.m_ProcessOpen = PlayerPrefs.GetInt("PROCESSOPEN", 0);
        this.m_ProcessFirstShoot = PlayerPrefs.GetInt("C_PROCESSFIRSTSHOOT", 0);
        this.m_isFirstTouch = PlayerPrefs.GetInt("FIRSTTOUCH", 0);
        this.refreshDataEveryday();
        if (this.m_GALoginState == 0)
        {
            //// // MonoSingleton<GAEvent>.Instance.EventLogin(this.GameDay());
            this.m_GALoginState = 1;
        }
        this.m_Money = PlayerPrefs.GetInt("MONEY", 0);
        this.m_getMoney = PlayerPrefs.GetInt("GETMONEY", 0);
        this.m_Audio = (PlayerPrefs.GetInt("AUDIO", 1) == 1);

       

        this.m_PhoneShake = (PlayerPrefs.GetInt("PHONESHAKE", 0) == 1);
        this.m_Grade = (PlayerPrefs.GetInt("GRADE", 0) == 1);
        this.m_GradeTimer = PlayerPrefs.GetInt("GRADETIMERNEW", 0);
        //this.m_AD = (PlayerPrefs.GetInt("AD", 1) == 0);
        if(PlayerPrefs.GetInt("AD") == 1) this.m_AD = false;
        this.m_DownSpeed = PlayerPrefs.GetFloat("DOWNSPEED", (this.m_GameMode != 0) ? 3.7f : 3.4f);
        this.m_CombineValue = this.m_CombineValueDef;
        this.m_HitValue = ((this.m_GameMode != 0) ? 10f : 12f);
        this.m_DownValue = PlayerPrefs.GetFloat("DOWNVALUE", 0f);
        this.m_BrickCount = PlayerPrefs.GetInt("BRICKCOUNT", 0);
        if (this.m_BrickCount > 0)
        {
            for (int j = 0; j < 5; j++)
            {
                string string3 = PlayerPrefs.GetString(j.ToString());
                if (!string.IsNullOrEmpty(string3))
                {
                    string[] array3 = string3.Split(new char[]
                    {
                        ':'
                    });
                    for (int k = 0; k < array3.Length; k++)
                    {
                        int item = int.Parse(array3[k]);
                        if (this.m_BricksInfo.ContainsKey(j))
                        {
                            this.m_BricksInfo[j].Add(item);
                        }
                        else
                        {
                            this.m_BricksInfo.Add(j, new List<int>());
                            this.m_BricksInfo[j].Add(item);
                        }
                    }
                }
            }
            string string4 = PlayerPrefs.GetString("BRICKNUMDIC");
            if (!string.IsNullOrEmpty(string4))
            {
                string[] array4 = string4.Split(new char[]
                {
                    ':'
                });
                int l = 0;
                int num = 2;
                while (l < array4.Length)
                {
                    int value2 = int.Parse(array4[l]);
                    this.m_BrickNumDic.Add(num, value2);
                    l++;
                    num *= 2;
                }
            }
            else
            {
                for (int m = 2; m <= 8192; m *= 2)
                {
                    this.m_BrickNumDic.Add(m, 0);
                }
            }
        }

        // 아이템 셋팅
        for(int j = 0; j < 2; j++)
        {
            for(int i = 0; i < 3; i++)
            {
                int item = PlayerPrefs.GetInt("ITEMMODE" + j + "_" + i, 0);
                if (this.m_BrickItemsInfo.ContainsKey(j))
                {
                    this.m_BrickItemsInfo[j].Add(item);
                }
                else
                {
                    this.m_BrickItemsInfo.Add(j, new List<int>());
                    this.m_BrickItemsInfo[j].Add(item);
                }
            }
        }

        string string5 = PlayerPrefs.GetString("COMBINECNT");
        if (!string.IsNullOrEmpty(string5))
        {
            string[] array5 = string5.Split(new char[]
            {
                ':'
            });
            int l = 0;
            int num = 2;
            
            while (l < array5.Length)
            {
                int value2 = int.Parse(array5[l]);
                this.m_BrickCombineCount.Add(num, 0);
                l++;
                num *= 2;
            }
        }
        else
        {
            for (int m = 2; m <= 8192; m *= 2)
            {
                this.m_BrickCombineCount.Add(m, 0);
            }
        }
    }

    public void SaveDataForGameOver(int score, int state, bool continueGame)
    {
        this.m_BoolContineGame = ((!continueGame) ? 0 : 1);
        if (state == 1 || state == 2)
        {
            this.m_BoolContineGame = 0;
        }
        if (state == 1 || (state == 2 && !continueGame))
        {
            if (this.m_GameMode == 0)
            {
                this.m_NormalMaxGameTimes++;
            }
            else
            {
                this.m_HardMaxGameTimes++;
            }
            this.m_GradeTimer++;
        }
        PlayerPrefs.SetInt("BOOLCONTINUEGAME", this.m_BoolContineGame);
        if (state == 0)
        {
            this.m_Score = score;
            PlayerPrefs.SetInt("SCORE", this.m_Score);
        }
        else
        {
            this.m_Score = 0;
            PlayerPrefs.SetInt("SCORE", this.m_Score);
            this.m_MaxScore = Mathf.Max(score, this.MaxScore);
        }
        PlayerPrefs.SetInt("SCORE", (state != 0) ? 0 : score);
        PlayerPrefs.SetInt("MAXSCORE", this.m_MaxScore);
        this.refreshDataEveryday();
        PlayerPrefs.SetInt("ONTINUEVIDEO", this.m_ContinueVideo);
        PlayerPrefs.SetInt("BIGENDVIDEO", this.m_BigEndVideo);
        PlayerPrefs.SetInt("RESUMEVIDEO", this.m_ResumVideo);
        PlayerPrefs.SetInt("PAUSEVIDEO", this.m_PauseVideo);
        PlayerPrefs.SetInt("OPENVIDEO", this.m_OpenVideo);
        PlayerPrefs.SetInt("GALOGINSTATE", this.m_GALoginState);
        PlayerPrefs.SetInt("NORMALMAXGAMETIMES", this.m_NormalMaxGameTimes);
        PlayerPrefs.SetInt("HARDMAXGAMETIMES", this.m_HardMaxGameTimes);
        PlayerPrefs.SetInt("HARDSTARTTIMES", this.m_HardStartTimes);
        PlayerPrefs.SetInt("DAY", this.m_Day);
        PlayerPrefs.SetInt("MONEY", this.m_Money);
        PlayerPrefs.SetInt("GRADE", (!this.m_Grade) ? 0 : 1);
        PlayerPrefs.SetInt("GRADETIMERNEW", this.m_GradeTimer);
        // if(this.m_AD == false)
        //     PlayerPrefs.SetInt("AD", 0);
        PlayerPrefs.SetInt("AUDIO", (!this.m_Audio) ? 0 : 1);
        PlayerPrefs.SetInt("PHONESHAKE", (!this.m_PhoneShake) ? 0 : 1);
        PlayerPrefs.SetString("USESKIN", this.m_UseSkinType.ToString());
        PlayerPrefs.SetInt("PROCESSOPEN", this.m_ProcessOpen);
        PlayerPrefs.SetInt("C_PROCESSFIRSTSHOOT", this.m_ProcessFirstShoot);
        PlayerPrefs.SetInt("C_PROCESS_2048", this.m_process2048);
        PlayerPrefs.SetInt("C_PROCESS_4096", this.m_process4096);
        PlayerPrefs.SetInt("C_PROCESS_8192", this.m_process8192);
        PlayerPrefs.SetInt("PROCESS_COVER", (!this.m_process_cover) ? 0 : 1);
        PlayerPrefs.SetInt("GAMEDAY", this.m_GameDay);
        PlayerPrefs.SetInt("DAYTWOSEND", this.m_VideoTime);
        string text = string.Empty;
        foreach (KeyValuePair<SkinType, bool> current in this.m_SkinListState)
        {
            if (current.Value)
            {
                text = string.Format("{0}:{1}", text, (int)current.Key);
            }
        }
        PlayerPrefs.SetString("UNLOCKSKIN", text);
        this.m_BrickCount = 0;
        if (state == 0) // GamePause
        {
            this.saveGamingMap();
            this.saveGamingMergeNum();
            this.saveGamingCombineCount();
            PlayerPrefs.SetInt("BRICKCOUNT", this.m_BrickCount);
            PlayerPrefs.SetFloat("DOWNVALUE", MonoSingleton<GamePlayManager>.Instance.DownValue);
            PlayerPrefs.SetFloat("DOWNSPEED", this.m_DownSpeed);
            PlayerPrefs.SetInt("C_ISOPENACHIEVE", this.m_isOpenAchieve);
            PlayerPrefs.SetInt("GETMONEY", this.m_getMoney);
        }
        else // GameRestart
        {
            PlayerPrefs.SetInt("BRICKCOUNT", 0);
            PlayerPrefs.SetFloat("DOWNVALUE", 0f);
            this.m_DownSpeed = this.m_DownSpeedDef;
            PlayerPrefs.SetFloat("DOWNSPEED", this.m_DownSpeed);
            this.m_isOpenAchieve = 0;
            PlayerPrefs.SetInt("C_ISOPENACHIEVE", this.m_isOpenAchieve);
            this.m_getMoney = 0;
            PlayerPrefs.SetInt("GETMONEY", this.m_getMoney);
        }

        this.saveGamingItems();
    }

    public void SaveDataForAppPause(int score, bool continuegame)
    {
        this.SaveDataForGameOver(score, 0, continuegame);
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        PlayerPrefs.DeleteAll();
    }

    public void CompleteCourse()
    {
        this.m_Course = true;
        PlayerPrefs.SetInt("COURSE", 1);
    }

    public void AudioState(bool state)
    {
        this.m_Audio = state;
    }

    public void PhoneShakeState(bool state)
    {
        this.m_PhoneShake = state;
    }

    public void NoAD()
    {
        this.m_AD = false;
    }

    public void AddMoney(int value = 1)
    {
        this.m_Money += value;
    }

    [ContextMenu("add 100 money")]
    private void addmoney()
    {
        this.AddMoney(6000);
    }

    public bool UnlockSkin(SkinType type, int price)
    {
        if (this.m_SkinListState.ContainsKey(type) && !this.m_SkinListState[type] && this.m_Money >= price)
        {
            this.m_SkinListState[type] = true;
            this.m_Money -= price;
            return true;
        }
        return false;
    }

    public bool UseSkin(SkinType type)
    {
        if (this.m_SkinListState.ContainsKey(type) && this.m_SkinListState[type])
        {
            this.m_UseSkinType = type;
            return true;
        }
        return false;
    }

    public bool SkinState(SkinType type)
    {
        return this.m_SkinListState.ContainsKey(type) && this.m_SkinListState[type];
    }

    public void CollectMoney()
    {
        this.m_getMoney++;
        this.m_Money++;
    }

    [ContextMenu("double")]
    public void DoubleMoney()
    {
        int getMoney = MonoSingleton<GamePlayManager>.Instance.GetMoney;
        this.m_Money += getMoney;
        MonoSingleton<GamePlayManager>.Instance.GetMoney += getMoney;
        MonoSingleton<GameUIManager>.Instance.ShowGameOverMoneyAnim(true, getMoney);
    }

    public void GradeIt()
    {
        this.m_Grade = true;
    }

    public bool MeetBuyCondition()
    {
        foreach (SkinInfo current in MonoSingleton<ConfigeManager>.Instance.Config)
        {
            if (!this.m_SkinListState[current.Type] && this.m_Money >= current.Price)
            {
                return true;
            }
        }
        return false;
    }

    public bool SkinMeetBuyCondition(SkinType type)
    {
        if (!this.m_SkinListState[type])
        {
            foreach (SkinInfo current in MonoSingleton<ConfigeManager>.Instance.Config)
            {
                if (type == current.Type && this.m_Money >= current.Price)
                {
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    public bool MeetDayTwoCondition()
    {
        this.m_VideoTime++;
        return this.GameDay() == 2 && this.m_VideoTime == 3;
    }

    public int GameDay()
    {
        int result = 1;
        // DateTime uTC =// // MonoSingleton<GAEvent>.Instance.GetUTC();
        // if (uTC == DateTime.MinValue)
        // {
        //     if (this.m_GameDay != 0 && this.m_Day != 0)
        //     {
        //         result = this.m_Day - this.m_GameDay + 1;
        //     }
        // }
        // else
        // {
        //     if (this.m_GameDay == 0)
        //     {
        //         this.m_GameDay = uTC.Year * 10000 + uTC.Month * 100 + uTC.Day;
        //     }
        //     this.m_Day = uTC.Year * 10000 + uTC.Month * 100 + uTC.Day;
        //     result = this.m_Day - this.m_GameDay + 1;
        // }
        return result;
    }

    public bool CommonDay()
    {
        return this.m_GameDay == this.m_Day || this.m_GameDay == 0 || this.m_Day == 0;
    }

    public void GAFirstShoot()
    {
        this.m_ProcessFirstShoot = 1;
        PlayerPrefs.SetInt("C_PROCESSFIRSTSHOOT", this.m_ProcessFirstShoot);
    }

    public void GAFirstTouch()
    {
        this.m_isFirstTouch = 1;
        PlayerPrefs.SetInt("FIRSTTOUCH", this.m_isFirstTouch);
    }

    public void GAOpen()
    {
        this.m_ProcessOpen = 1;
        PlayerPrefs.SetInt("PROCESSOPEN", this.m_ProcessOpen);
    }

    public void SetDownSpeed(int num)
    {
        this.m_DownSpeed = this.m_DownSpeedDef;

        // if (this.GameMode == 2 || this.GameMode == 0)
        // {
        //     return;
        // }
        // float num2;
        // if (num >= 8192)
        // {
        //     num2 = 1.8f;
        // }
        // else if (num >= 4096)
        // {
        //     num2 = 1.5f;
        // }
        // else if (num >= 2048)
        // {
        //     num2 = 1.2f;
        // }
        // else
        // {
        //     num2 = 1f;
        // }
        // if (this.m_DownSpeed < this.m_DownSpeedDef * num2 && !Mathf.Approximately(this.m_DownSpeed, this.m_DownSpeedDef * num2))
        // {
        //     if ((num2 == 1.5f && Mathf.Approximately(this.m_DownSpeed, this.m_DownSpeedDef)) || (num2 == 1.8f && this.m_DownSpeed <= this.m_DownSpeedDef * 1.2f))
        //     {
        //         MonoSingleton<GamePlayManager>.Instance.SetLevelUpContent(true);
        //     }
        //     else
        //     {
        //         MonoSingleton<GamePlayManager>.Instance.SetLevelUpContent(false);
        //     }
        //     this.m_DownSpeed = this.m_DownSpeedDef * num2;
        // }
    }

    public void PlayModeInit(int mode)
    {
        if (this.m_PlayMode != mode)
        {
            this.m_PlayMode = mode;
            PlayerPrefs.SetInt("PLAYMODE", this.m_PlayMode);
        }
    }

    public void GameModeInit(int mode)
    {
        if (this.m_GameMode != mode)
        {
            this.m_GameMode = mode;
            PlayerPrefs.SetInt("GAMEMODE", this.m_GameMode);
            switch (this.m_GameMode)
            {
                case 0:
                    this.m_DownSpeedDef = 3.4f;
                    this.m_DownSpeed = this.m_DownSpeedDef;
                    this.m_HitValue = 12f;
                    break;
                case 1:
                    this.m_DownSpeedDef = 3.7f;
                    this.m_DownSpeed = this.m_DownSpeedDef;
                    this.m_HitValue = 10f;
                    break;
                case 2:
                    break;
                default:
                    SingleInstance<DebugManager>.Instance.LogError("GameModeInit");
                    break;
            }
        }
    }

    public void ComingNextLevel()
    {
        if (this.m_GameLevel < GameDataManager.MaxLevel)
        {
            this.m_GameLevel++;
            PlayerPrefs.SetInt("GAMELEVEL", this.m_GameLevel);
        }
    }

    public void SelectLevel(int num)
    {
        if (num > GameDataManager.MaxLevel)
        {
            SingleInstance<DebugManager>.Instance.LogError("GameDataManager.SelectLevel error param num ");
        }
        this.m_GameLevel = num;
        PlayerPrefs.SetInt("GAMELEVEL", this.m_GameLevel);
    }

    private void refreshDataEveryday()
    {
        // DateTime uTC =// // MonoSingleton<GAEvent>.Instance.GetUTC();
        // if (uTC != DateTime.MinValue)
        // {
        // 	if (this.m_GameDay == 0)
        // 	{
        // 		this.m_GameDay = uTC.Year * 10000 + uTC.Month * 100 + uTC.Day;
        // 		PlayerPrefs.SetInt("GAMEDAY", this.m_GameDay);
        // 	}
        // 	if (this.m_Day != uTC.Year * 10000 + uTC.Month * 100 + uTC.Day)
        // 	{
        // 		this.m_NormalMaxGameTimes = 0;
        // 		this.m_HardMaxGameTimes = 0;
        // 		this.m_HardStartTimes = 0;
        // 		this.m_GALoginState = 0;
        // 		this.m_VideoTime = 0;
        // 		this.m_Day = uTC.Year * 10000 + uTC.Month * 100 + uTC.Day;
        // 		this.m_ContinueVideo = 0;
        // 		this.m_BigEndVideo = 0;
        // 		this.m_ResumVideo = 0;
        // 		this.m_PauseVideo = 0;
        // 		this.m_OpenVideo = 0;
        // 		this.m_process2048 = 0;
        // 		this.m_process4096 = 0;
        // 		this.m_process8192 = 0;
        // 		this.m_process_cover = false;
        // 	}
        // }
    }

    private void saveGamingMap()
    {
        for (int i = 0; i < 5; i++)
        {
            if (MonoSingleton<GamePlayManager>.Instance.BricksDic.ContainsKey(i))
            {
                int num = i;
                List<Brick> list = MonoSingleton<GamePlayManager>.Instance.BricksDic[i];
                string text = string.Empty;
                if (list.Count > 0)
                {
                    text = list[0].m_Number.ToString();
                }
                for (int j = 1; j < list.Count; j++)
                {
                    text = string.Format("{0}:{1}", text, list[j].m_Number);
                }
                this.m_BrickCount += list.Count;
                PlayerPrefs.SetString(num.ToString(), text);
            }
            else
            {
                PlayerPrefs.DeleteKey(i.ToString());
            }
        }
    }

    private void saveGamingMergeNum()
    {
        string text = MonoSingleton<GamePlayManager>.Instance.BrickNumDic[2].ToString();
        for (int i = 4; i <= 8192; i *= 2)
        {
            text = string.Format("{0}:{1}", text, MonoSingleton<GamePlayManager>.Instance.BrickNumDic[i].ToString());
        }
        PlayerPrefs.SetString("BRICKNUMDIC", text);
    }

    public void saveGamingItems()
    {
        for(int i = 0; i < 2; i++)
        {
            List<int> list = GameDataManager.Instance.BrickItemsInfo[i];
            //List<BrickItem> list = MonoSingleton<GamePlayManager>.Instance.BrickItemsDic[i];

            for(int j = 0; j < list.Count; j++)
                PlayerPrefs.SetInt("ITEMMODE"+ i + "_" + j, list[j]);
        }
    }

    private void saveGamingCombineCount()
    {
        string text = MonoSingleton<GamePlayManager>.Instance.BrickCombineCount[2].ToString();
        for(int i = 4; i <= 8192; i *= 2)
        {
            int count = MonoSingleton<GamePlayManager>.Instance.BrickCombineCount[i];
            text = string.Format("{0}:{1}", text, count.ToString());
        }
        PlayerPrefs.SetString("COMBINECNT", text);
    }
}
