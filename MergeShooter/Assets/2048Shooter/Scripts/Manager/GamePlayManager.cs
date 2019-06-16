using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayManager : MonoSingleton<GamePlayManager>
{
    public enum GameState
    {
        None,
        CourseHandle,
        StartWait,
        Handle,
        Check,
        Tween,
        Pause,
        ContinueWait,
        GameOver,
        UseItem
    }

    private sealed class _openAchievement_c__AnonStorey0
    {
        internal int num;

        internal void __m__0()
        {
            // MonoSingleton<GAEvent>.Instance.Middle_All(this.num);
        }
    }

    private int _GetMoney_k__BackingField;

    private int m_Score;

    private int m_MaxScore;

    private float m_HitValue;

    private bool m_Hit;

    [SerializeField]
    private float m_DownValue; // 블럭 내려오는 값

    private float m_CombineValue;

    [SerializeField]
    public int m_MaxValue;

    [SerializeField]
    private int m_MaxValueScore;

    [SerializeField]
    private GameObject m_Content;

    [SerializeField]
    private Text m_ScoreText;

    [SerializeField]
    private Text m_MaxScoreText;

    [SerializeField]
    private Next m_Next;

    [SerializeField]
    private GameObject m_ContentRoot;

    [SerializeField]
    private GameObject m_Canvas;

    [SerializeField]
    private TextTweener m_TextTweener;

    [SerializeField]
    private TextTweener m_LevelUpTweenr;

    [SerializeField]
    private Text m_LevelUpText;

    [SerializeField]
    private Image m_levelBg;

    [SerializeField]
    private Text[] m_CurrentItemCount;

    private bool m_isOpenLevelUpTip;

    private GamePlayManager.GameState m_State;

    private int m_GiveMoneyForCombineTimes = 15;

    private int m_CombineTimes;

    private int m_CombineOneTimes;

    private int m_LasetSelect = -1;

    private bool playLiHua;

    private List<Brick> m_ChangeList = new List<Brick>();

    private Dictionary<int, List<Brick>> m_BricksDic = new Dictionary<int, List<Brick>>();

    private Dictionary<int, int> m_BrickNumDic = new Dictionary<int, int>();

    private Dictionary<int, List<BrickItem>> m_BrickItemDic = new Dictionary<int, List<BrickItem>>();

    private Dictionary<int, int> m_BrickCombineCount = new Dictionary<int, int>();

    private bool m_ContinueGame;

    private bool m_Press;

    private RectTransform m_Rect;

    private int m_ClassicModeLine = 3;

    public int GetMoney
    {
        get;
        set;
    }

    public int Score
    {
        get
        {
            return this.m_Score;
        }
    }

    public int MaxSocre
    {
        get
        {
            return this.m_MaxScore;
        }
    }

    public float DownValue
    {
        get
        {
            return this.m_DownValue;
        }
    }

    public GamePlayManager.GameState State
    {
        get
        {
            return this.m_State;
        }
    }

    public Dictionary<int, List<Brick>> BricksDic
    {
        get
        {
            return this.m_BricksDic;
        }
    }

    public Dictionary<int, int> BrickNumDic
    {
        get
        {
            return this.m_BrickNumDic;
        }
    }

    public Dictionary<int, int> BrickCombineCount
    {
        get
        {
            return this.m_BrickCombineCount;
        }
    }

    // ANCHOR Add Use GameItems
    public Dictionary<int, List<BrickItem>> BrickItemsDic
    {
        get
        {
            return this.m_BrickItemDic;
        }
    }

    public bool BoolContinueGame
    {
        get
        {
            return this.m_ContinueGame;
        }
    }

    public override void Init()
    {
        base.Init();
        this.RefreshInitInfo();
        this.Hide();
    }

    private void LoadData()
    {
        this.m_MaxScore = MonoSingleton<GameDataManager>.Instance.MaxScore;
        this.m_Score = MonoSingleton<GameDataManager>.Instance.Score;
        this.m_HitValue = MonoSingleton<GameDataManager>.Instance.HitValue;
        this.m_DownValue = ((MonoSingleton<GameDataManager>.Instance.BrickCount != 0) ? MonoSingleton<GameDataManager>.Instance.DownValue : 0f);
        this.m_CombineValue = MonoSingleton<GameDataManager>.Instance.CombineValue;
        this.m_ContinueGame = MonoSingleton<GameDataManager>.Instance.BoolContiueGame;
        this.GetMoney = MonoSingleton<GameDataManager>.Instance.GetMoney;
        foreach (KeyValuePair<int, List<Brick>> current in this.m_BricksDic)
        {
            foreach (Brick current2 in current.Value)
            {
                UnityEngine.Object.Destroy(current2.gameObject);
            }
        }

        this.m_BricksDic.Clear();
        for (int i = 0; i < 5; i++)
        {
            this.m_BricksDic.Add(i, new List<Brick>());
        }

        // 저장되어 있는 진행중인 블럭 갯수

        if (MonoSingleton<GameDataManager>.Instance.BrickCount > 0)
        {
            Dictionary<int, List<int>> bricksInfo = MonoSingleton<GameDataManager>.Instance.BricksInfo;
            foreach (KeyValuePair<int, List<int>> current3 in bricksInfo)
            {
                int key = current3.Key; // 가로 인덱스
                List<int> value = current3.Value;
                for (int j = 0; j < value.Count; j++) // 세로 인덱스
                {
                    Brick brick = MonoSingleton<BricksFactoryManager>.Instance.CreateBrick(key, j, value[j], this.m_MaxValue);
                    brick.transform.localPosition = new Vector3(-237f, -57f, 0f) + Vector3.right * 118f * (float)key + Vector3.down * (float)this.m_BricksDic[key].Count * 118f;
                    this.m_BricksDic[key].Add(brick);
                }
            }
        }
        else
        {
            int i, j;
            // ANCHOR Classic Mode Start
            if(GameDataManager.Instance.PlayMode == 0)
            {
                int[, ] dicValues = new int[5, m_ClassicModeLine];

                for(i = 0; i < 5; i++)
                {
                    for (j = 0; j < m_ClassicModeLine; j++) 
                        dicValues[i, j] = NextValue2();
                }

                for(i = 0; i < 5; i++)
                {
                    for (j = 0; j < m_ClassicModeLine; j++) 
                    {
                        int left = i - 1;
                        int right = i + 1;
                        int up = j - 1;
                        int down = j + 1;
 
                        List<int> findValues = new List<int>();
                        if(left >= 0)
                        {
                            findValues.Add(dicValues[left, j]);
                        }
                        if(right > 0 && right < 5)
                        {
                            findValues.Add(dicValues[right, j]);
                        }
                        if(up >= 0)
                        {
                            findValues.Add(dicValues[i, up]);
                        }
                        if(down > 0 && down < m_ClassicModeLine)
                        {
                            findValues.Add(dicValues[i, down]);
                        }
                        
                        List<ValuePer> nextValues = GetValueList();
                        for(int k = 0; k < findValues.Count; k++)
                        {
                            for(int l = 0; l < nextValues.Count; l++)
                            {
                                if(findValues[k] == nextValues[l].Value)
                                {
                                    nextValues.Remove(nextValues[l]);
                                    break;
                                }
                            }
                        }

                        if(findValues.Count > 0)
                        {
                            dicValues[i, j] = nextValues[UnityEngine.Random.Range(0, nextValues.Count)].Value;
                        }
                    } // j
                }

                for(i = 0; i < 5; i++)
                {
                    for (j = 0; j < m_ClassicModeLine; j++) 
                    {
                        Brick brick = MonoSingleton<BricksFactoryManager>.Instance.CreateBrick(i, j, dicValues[i, j], this.m_MaxValue);
                        brick.transform.localPosition = new Vector3(-237f, -57f, 0f) + Vector3.right * 118f * (float)i + Vector3.down * (float)this.m_BricksDic[i].Count * 118f;
                        this.m_BricksDic[i].Add(brick);
                    }
                }
            }
        }

        // 게임 시작시 블록 구입 (클래식 모드)
        // int key = 2;
        // Brick brick = MonoSingleton<BricksFactoryManager>.Instance.CreateBrick(key, 0, 512, this.m_MaxValue);
        // brick.transform.localPosition = new Vector3(-237f, -57f, 0f) + Vector3.right * 118f * (float)key + Vector3.down * (float)this.m_BricksDic[key].Count * 118f;
        // this.m_BricksDic[key].Add(brick);

        this.m_BrickNumDic.Clear();
        Dictionary<int, int> brickNumDic = MonoSingleton<GameDataManager>.Instance.BrickNumDic;
        for (int k = 2; k <= 8192; k *= 2)
        {
            if (brickNumDic.Count == 0)
            {
                this.m_BrickNumDic.Add(k, 0);
            }
            else
            {
                this.m_BrickNumDic.Add(k, brickNumDic[k]);
            }
        }

        // 각 블럭의 합쳐진 갯수
        this.m_BrickCombineCount.Clear();
        Dictionary<int, int> brickCombine = MonoSingleton<GameDataManager>.Instance.BrickCombineCount;
        for(int i = 0; i < GameDataManager.Instance.ValueList.Count; i++)
        {
            int key = (int)Mathf.Pow(2f, (float)i + 1);
            if (brickCombine.Count == 0)
                this.m_BrickCombineCount.Add(key, 0);
            else
                this.m_BrickCombineCount.Add(key, brickCombine[key]);
        }
    }

    private List<ValuePer> GetValueList()
    {
        List<ValuePer> nextValues = new List<ValuePer>();
        // 2 ~ 512 까지 추출
        for(int i = 0; i < 9; i++)
        {
            nextValues.Add(GameDataManager.Instance.ValueList[i]);
        }

        return nextValues;
    }

    private int NextValue2()
    {
        List<ValuePer> nextValues = GetValueList();

        int num = UnityEngine.Random.Range(0, nextValues.Count);
        
        return nextValues[num].Value;
    }

    [SerializeField]
    private GameObject itemPanel;
    [SerializeField]
    private GameObject useItemPanel;
    [SerializeField]
    private GameObject useItemClose;
    [SerializeField]
    private Text useItemInfo;

    private void LoadItem()
    {
        int i;

        for(i = 0; i < itemPanel.transform.childCount; i++)
        {
            Destroy(itemPanel.transform.GetChild(i).gameObject);
        }
        this.m_BrickItemDic.Clear();
        for (i = 0; i < 2; i++)
        {
            this.m_BrickItemDic.Add(i, new List<BrickItem>());
        }

        ItemType[,] itemTypes = {
            {ItemType.underNumberDeleteBrick, ItemType.selectUpgradeBrick, ItemType.selectDeleteBrick},
            {ItemType.lineDelete, ItemType.selectBrickChangeBoomb, ItemType.stopTime}
        };
        Dictionary<int, List<int>> brickItemsInfo = MonoSingleton<GameDataManager>.Instance.BrickItemsInfo;
        foreach (KeyValuePair<int, List<int>> current in brickItemsInfo)
        {            
            int key = current.Key;
            if(key != GameDataManager.Instance.PlayMode) continue; // 게임 모드 별로 구분

            List<int> value = current.Value;
            for (int j = 0; j < value.Count; j++)
            {
                //UnityEngine.Debug.Log("itemmode : " + key + "j " + j + "> " + value[j]);
                
                ItemType type = itemTypes[key, j];
                BrickItem item = ItemManager.Instance.SetBrickItem(type, j, value[j]);
                this.m_BrickItemDic[key].Add(item);
            }
        }
        ItemManager.Instance.gamingStopTime = false;
    }

    public void RefreshInitInfo()
    {
        this.ClearData();
        this.LoadData();
        this.m_ScoreText.text = this.m_Score.ToString();
        this.m_MaxScoreText.text = this.m_MaxScore.ToString();
        if(GameDataManager.Instance.PlayMode == 0)
            this.m_ContentRoot.transform.localPosition = Vector3.zero;
        else // 도전 모드
            this.m_ContentRoot.transform.localPosition = this.m_DownValue * Vector3.down;
        this.m_Next.Init(2, this.NextValue());
        this.m_State = GamePlayManager.GameState.None;
        this.playLiHua = false;
        MonoSingleton<MoneyManager>.Instance.Hide();
        this.m_CombineTimes = 0;

        this.LoadItem();
    }

    public void Hide()
    {
        this.m_Content.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.m_Content.gameObject.SetActive(true);
    }

    // 다음 차례에 나올 블럭 설정
    private int NextValue()
    {
       // return 64;
        if (!MonoSingleton<GameDataManager>.Instance.Course)
        {
            return 2;
        }
        int num = 0;
        foreach (ValuePer current in MonoSingleton<GameDataManager>.Instance.ValueList)
        {
            //UnityEngine.Debug.Log("ValuePer : " + current.Per);

            if (current.Value <= this.m_MaxValue)
            {
                num += current.Per;
            }
        }

        int num2 = UnityEngine.Random.Range(0, num);
        foreach (ValuePer current2 in MonoSingleton<GameDataManager>.Instance.ValueList)
        {
            if (num2 < current2.Per)
            {
                return current2.Value;
            }
            num2 -= current2.Per;
        }
        return 0;
    }

    private void ChangeState(GamePlayManager.GameState state)
    {
        this.m_State = state;
    }

    public void GameStartForWait()
    {
        if (MonoSingleton<GameDataManager>.Instance.Course)
        {
            this.m_TextTweener.Show(-1f, 0f);
            this.ChangeState(GamePlayManager.GameState.StartWait);
        }
        else
        {
            this.m_TextTweener.Hide();
            this.ChangeState(GamePlayManager.GameState.CourseHandle);
            MonoSingleton<GameUIManager>.Instance.ShowCourse();
        }
    }

    public void GamePause()
    {
        this.ChangeState(GamePlayManager.GameState.Pause);
    }

    public void GameHome()
    {
        this.ChangeState(GamePlayManager.GameState.None);
    }

    public void GamePlay()
    {
        if (this.m_TextTweener.gameObject.activeSelf)
        {
            this.ChangeState(GamePlayManager.GameState.StartWait);
        }
        else
        {
            this.ChangeState(GamePlayManager.GameState.Tween);
        }
    }

    public void PauseByUseItem()
    {
        if(ItemManager.Instance.type != ItemType.stopTime)
            this.ChangeState(GamePlayManager.GameState.UseItem);
        else 
            ItemManager.Instance.gamingStopTime = true;
        this.useItemPanel.SetActive(true);

        string blockSelect = Localization.Instance.GetLable("BrickSelect");
        if(ItemManager.Instance.type == ItemType.selectDeleteBrick)
        {
            blockSelect += Localization.Instance.GetLable("ClassicItemInfo3");
        }
        else if(ItemManager.Instance.type == ItemType.selectUpgradeBrick)
        {
            blockSelect += Localization.Instance.GetLable("ClassicItemInfo2");
        }
        else if(ItemManager.Instance.type == ItemType.underNumberDeleteBrick)
        {
            blockSelect += Localization.Instance.GetLable("ClassicItemInfo1");
        }
        else if(ItemManager.Instance.type == ItemType.lineDelete)
        {
            blockSelect += Localization.Instance.GetLable("ChallengeItemInfo1"); 
        }
        else if(ItemManager.Instance.type == ItemType.selectBrickChangeBoomb)
        {
            blockSelect += Localization.Instance.GetLable("ChallengeItemInfo2");
        }
        else if(ItemManager.Instance.type == ItemType.stopTime)
        {
            blockSelect = Localization.Instance.GetLable("ChallengeItemInfo1") + "\n10";
        }
        useItemInfo.text = blockSelect;
        if(ItemManager.Instance.type == ItemType.stopTime)
        {  
            GameDataManager.Instance.BrickItemsInfo[GameDataManager.Instance.PlayMode][2]--;
            GameDataManager.Instance.saveGamingItems();

            useItemClose.SetActive(false);
            StartCoroutine(CheckDownCoroutine());
        }
        else useItemClose.SetActive(true);
    }

    IEnumerator CheckDownCoroutine()
    {
        int m_Count = 0;
        string szTime = Localization.Instance.GetLable("ChallengeItemInfo1") + "\n";

        while(m_Count < 10)
        {
            yield return new WaitForSeconds(1f);
            m_Count++;
            useItemInfo.text = szTime + (10 - m_Count).ToString();
        }
        ItemManager.Instance.gamingStopTime = false;
        UpdateEndUseItems();
    }

    public void WaitGameContinue()
    {
        this.ChangeState(GamePlayManager.GameState.ContinueWait);
    }

    private void LateUpdate()
    {
        switch (this.m_State)
        {
            // case GamePlayManager.GameState.CourseHandle:
            //     //==this.CourseHandle();
            //     break;
            case GamePlayManager.GameState.StartWait:
                this.StartWait();
                break;
            case GamePlayManager.GameState.Handle:
                this.checkGameWin();
                if (!this.CheckGameOver())
                {
                    //==this.Handle();
                    this.Movement();
                }
                break;
            case GamePlayManager.GameState.Check:
                this.Check();
                this.Movement();
                break;
            case GamePlayManager.GameState.Tween:
                this.Tween();
                this.Movement();
                break;
        }

        //UnityEngine.Debug.Log(GameDataManager.Instance.GameMode);
    }

    private void StartWait() // 게임시작을 기다린다.
    {
        this.m_CombineOneTimes = 0;
        // if (this.m_Rect == null)
        // {
        //     this.m_Rect = this.m_Canvas.GetComponent<RectTransform>();
        // }
        // Vector2 zero = Vector2.zero;
        // if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_Rect, Input.mousePosition, Camera.main, out zero))
        // {
        // }
        if (Input.GetMouseButtonUp(0))
        {
            this.m_TextTweener.Stop();
            this.m_TextTweener.Hide();

            this.ChangeState(GamePlayManager.GameState.Handle);

            m_AccelValue = 0f;
        }
        /*if (Input.GetMouseButtonDown(0))
        {
            // MonoSingleton<GAEvent>.Instance.StopFetchAd();
            Vector3 vector = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            int num = (int)(zero.x + 252.5f);
            int num2 = num / 101;
            num2 = ((num2 >= 0) ? num2 : 0);
            num2 = ((num2 <= 4) ? num2 : 4);
            // 게임 화면 안이면
            if (vector.y <= 0.8f)
            {
                this.m_TextTweener.Stop();
                this.m_TextTweener.Hide();

                this.ChangeState(GamePlayManager.GameState.Handle);

                m_AccelValue = 0f;

                //this.m_Press = true;
                //this.m_Next.Init(num2, this.m_Next.Value);
            }
        }*/
        /*if (Input.GetMouseButton(0))
        {
            int num3 = (int)(zero.x + 252.5f);
            int num4 = num3 / 101;
            num4 = ((num4 >= 0) ? num4 : 0);
            num4 = ((num4 <= 4) ? num4 : 4);
            if (this.m_Press)
            {
                this.m_Next.Init(num4, this.m_Next.Value);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            int num5 = (int)(zero.x + 252.5f);
            int num6 = num5 / 101;
            num6 = ((num6 >= 0) ? num6 : 0);
            num6 = ((num6 <= 4) ? num6 : 4);
            if (this.m_Press)
            {
                this.m_TextTweener.Stop();
                this.m_TextTweener.Hide();
                this.LevelUpTween();
                this.Selected(num6, this.m_Next.Value);
                this.m_Press = false;
            }
        }*/
    }
    
    float m_AccelValue;
    private void Movement()
    {
        if(GameDataManager.Instance.PlayMode == 1 && !ItemManager.Instance.gamingStopTime) // 도전 모드
        { 
            this.m_DownValue += MonoSingleton<GameDataManager>.Instance.DownSpeed * Time.deltaTime * 6f;
            
            this.m_AccelValue += 0.0001f;
            this.m_DownValue += this.m_AccelValue;

            Vector3 localPosition = this.m_ContentRoot.transform.localPosition;
            Vector3 b = (float)((int)this.m_DownValue) * Vector3.down;
            this.m_ContentRoot.transform.localPosition = Vector3.Lerp(localPosition, b, 0.3f);
        }
    }

    public void GameOver()
    {
        this.ChangeState(GamePlayManager.GameState.GameOver);
    }

    private bool CheckGameOver()
    {
        int num = 0;
        foreach (List<Brick> current in this.m_BricksDic.Values)
        {
            num = Mathf.Max(num, current.Count);
        }
        float num2 = this.m_ContentRoot.transform.localPosition.y;
        num2 -= (float)(num * 118f);
        if (num2 <= -804f)
        {
            if (!this.m_ContinueGame)
            {
                // UnityEngine.Debug.Log("1 >>>>> " + GameDataManager.Instance.AD);
                // UnityEngine.Debug.Log("2 >>>>> " + AdManager.Instance.IsRewardLoaded());

                if (!MonoSingleton<GameDataManager>.Instance.AD)
                {
                    MonoSingleton<GameManager>.Instance.OpenContinuePanel();
                }
                else if (AdManager.Instance.IsRewardLoaded())
                {
                    MonoSingleton<GameManager>.Instance.OpenContinuePanel();
                }
                else
                {
                    MonoSingleton<GameManager>.Instance.GameOver();
                }
            }
            else
            {
                MonoSingleton<GameManager>.Instance.GameOver();
            }
            return true;
        }
        return false;
    }

    // FIXME Touch Zone : 터치 칸 인덱스 0 ~ 4
    public void TouchHandle(int arrayNum)
    {
        /*if(this.m_State == GamePlayManager.GameState.CourseHandle)
        {
            this.m_CombineOneTimes = 0;

            this.m_Next.Init(arrayNum, this.m_Next.Value);
            MonoSingleton<GameManager>.Instance.StopCourse();

            this.Selected(this.m_Next.Value, this.m_Next.Value);
            MonoSingleton<GameManager>.Instance.CompleteCourse();
            this.ChangeState(GamePlayManager.GameState.Handle);
        }
        else*/ if(this.m_State == GamePlayManager.GameState.Handle)
        {
            if (!this.CheckGameOver())
            {
                this.m_CombineOneTimes = 0;

                this.m_Next.Init(arrayNum, this.m_Next.Value);
                this.Selected(arrayNum, this.m_Next.Value); 
            }
        }
    }

    private void CourseHandle()
    {
        UnityEngine.Debug.Log("CourseHandle");

        /*this.m_CombineOneTimes = 0;
        if (this.m_Rect == null)
        {
            this.m_Rect = this.m_Canvas.GetComponent<RectTransform>();
        }
        Vector2 zero = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.m_Rect, Input.mousePosition, Camera.main, out zero))
        {
        }
        if (Input.GetMouseButtonDown(0))
        {
            // MonoSingleton<GAEvent>.Instance.StopFetchAd();
            Vector3 vector = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            int num = (int)(zero.x + 252.5f);
            int num2 = num / 101;
            num2 = ((num2 >= 0) ? num2 : 0);
            num2 = ((num2 <= 4) ? num2 : 4);
            if (vector.y <= 0.8f)
            {
                this.m_Press = true;
                this.m_Next.Init(num2, this.m_Next.Value);
                MonoSingleton<GameManager>.Instance.StopCourse();
            }
        }
        if (Input.GetMouseButtonUp(0) && this.m_Press)
        {
            this.Selected(this.m_Next.Value, this.m_Next.Value);
            MonoSingleton<GameManager>.Instance.CompleteCourse();
            this.m_Press = false;
            this.ChangeState(GamePlayManager.GameState.Handle);
        }*/
    }

    public void Selected(int array, int value)
    {
        int value2 = this.NextValue();
        this.m_Next.Init(2, value2);
        this.m_Hit = true;
        if (!this.m_BricksDic.ContainsKey(array))
        {
            this.m_BricksDic.Add(array, new List<Brick>());
        }
        Brick brick = MonoSingleton<BricksFactoryManager>.Instance.CreateBrick(array, this.m_BricksDic[array].Count, value, this.m_MaxValue);
        this.m_BricksDic[array].Add(brick);
        brick.MoveTarget();
        this.m_LasetSelect = array;
        this.ChangeState(GamePlayManager.GameState.Tween);
    }

    private void Tween()
    {
        foreach (List<Brick> current in this.m_BricksDic.Values)
        {
            foreach (Brick current2 in current)
            {
                if (current2.m_Tween)
                {
                    return;
                }
            }
        }
        this.ChangeState(GamePlayManager.GameState.Check);
    }

    private void Check()
    {
        if (this.m_Hit)
        {
            if(GameDataManager.Instance.PlayMode == 1 && !ItemManager.Instance.gamingStopTime) // 도전 모드
            {
                this.m_DownValue -= this.m_HitValue;
                this.m_DownValue = ((this.m_DownValue >= 0f) ? this.m_DownValue : 0f);
            }
            this.m_Hit = false;
            MonoSingleton<AudioManager>.Instance.PlayHitAudio();
            if (this.m_LasetSelect == MonoSingleton<MoneyManager>.Instance.Array && MonoSingleton<MoneyManager>.Instance.State)
            {
                MonoSingleton<MoneyManager>.Instance.ShowTweener();
                MonoSingleton<AudioManager>.Instance.PlayMoneyAudio();
                MonoSingleton<GameDataManager>.Instance.CollectMoney();
                this.GetMoney++;
                this.m_CombineTimes = 0;
            }
        }
    
        bool flag = false;
        foreach (List<Brick> current in this.m_BricksDic.Values)
        {
            bool flag2 = false;
            for (int i = current.Count - 1; i >= 0; i--)
            {
                if (current[i].m_Destory)
                {
                    flag = true;
                    flag2 = true;

                    Vector3 pos = current[i].gameObject.transform.localPosition;
                    int number = current[i].m_Number;
                    bool boombed = current[i].m_Boomb;

                    UnityEngine.Object.Destroy(current[i].gameObject);
                    current.RemoveAt(i);

                    if(boombed)
                    {
                        EffectManager.Instance.PlayBoombEffect(pos, number);
                    }
                }
                else
                {
                    int number = current[i].m_Number;
                    current[i].RefreshNumber(this.m_MaxValue);
                    if (current[i].m_Number > number)
                    {
                        this.gaProcessBigNumCount(current[i].m_Number);
                        if (current[i].m_Number > this.m_MaxValue)
                        {
                            this.m_Score += this.m_MaxValueScore;
                            this.ShowScoreTweener(this.m_MaxValueScore, current[i].transform.localPosition + this.m_ContentRoot.transform.localPosition);
                        }
                        else
                        {
                            this.m_Score += current[i].m_Number;
                            this.ShowScoreTweener(current[i].m_Number, current[i].transform.localPosition + this.m_ContentRoot.transform.localPosition);
                        }
                        if(GameDataManager.Instance.PlayMode == 1 && !ItemManager.Instance.gamingStopTime) // 도전 모드
                        {
                            this.m_DownValue -= Mathf.Log((float)current[i].m_Number, 2f) * this.m_CombineValue;
                            this.m_DownValue = ((this.m_DownValue >= 0f) ? this.m_DownValue : 0f);
                        }
                    }
                }
            }
            if (flag2) // 합쳐져서 없어질때 
            {
                this.RefreshScore();
                for (int j = 0; j < current.Count; j++)
                {
                    current[j].Init(current[j].m_Array, j, current[j].m_Number, this.m_MaxValue);
                    current[j].MoveTarget();
                }
            }
        }
        if (flag) // 합쳐져서 없어질때 
        {
            if (MonoSingleton<MoneyManager>.Instance.State && !MonoSingleton<MoneyManager>.Instance.Tween)
            {
                int array = MonoSingleton<MoneyManager>.Instance.Array;
                if (this.m_BricksDic.ContainsKey(array))
                {
                    List<Brick> list = this.m_BricksDic[array];
                    MonoSingleton<MoneyManager>.Instance.MoveTarget(array, (list != null) ? list.Count : 0);
                }
                else
                {
                    MonoSingleton<MoneyManager>.Instance.MoveTarget(array, 0);
                }
            }
            this.RefreshScore();
            this.ChangeState(GamePlayManager.GameState.Tween);
            return;
        }
        int num = 0;
        bool flag3 = false;
        foreach (List<Brick> current2 in this.m_BricksDic.Values)
        {
            foreach (Brick current3 in current2)
            {
                if (current3.m_Number > num)
                {
                    num = current3.m_Number;
                }
                if (current3.m_Number > this.m_MaxValue)
                {
                    current3.RefreshNumber(-1, this.m_MaxValue);
                    flag3 = true;
                    break;
                }
            }
        }
        MonoSingleton<GameDataManager>.Instance.SetDownSpeed(num);
        if (flag3)
        {
            this.ToBouns();
            return;
        }
        this.m_ChangeList.Clear();
        foreach (int current4 in this.m_BricksDic.Keys) // 가로 인덱스
        {
            //UnityEngine.Debug.Log("BricksDic Key : " + current4);

            List<Brick> list2 = this.m_BricksDic[current4];
            foreach (Brick current5 in list2)
            {
                int number2 = current5.m_Number;
                int index = current5.m_Index;
                int array2 = current5.m_Array;
                int num2 = 0;
                if (list2.Count > index + 1) // key 값에서 세로줄 검색
                {
                    Brick brick = list2[index + 1];
                    if (brick.m_Number == number2 && number2 > 0)
                    {
                        num2++;
                    }
                }
                if (index > 0)
                {
                    Brick brick2 = list2[index - 1];
                    if (brick2.m_Number == number2 && number2 > 0)
                    {
                        num2++;
                    }
                }
                if (this.m_BricksDic.ContainsKey(array2 - 1) && this.m_BricksDic[array2 - 1].Count > index && this.m_BricksDic[array2 - 1][index].m_Number == number2 && number2 > 0)
                {
                    num2++;
                }
                if (this.m_BricksDic.ContainsKey(array2 + 1) && this.m_BricksDic[array2 + 1].Count > index && this.m_BricksDic[array2 + 1][index].m_Number == number2 && number2 > 0)
                {
                    num2++;
                }
                // 같은 숫자의 블럭이 있음
                if (num2 > 0)
                {
                    this.m_ChangeList.Add(current5);
                    current5.SetCount(num2);
                }
            }
        }

        this.checkGameWin();
        if (this.m_ChangeList.Count == 0)
        {
            if (this.CheckGameOver())
            {
                return;
            }
            this.GiveMoney();
            this.ChangeState(GamePlayManager.GameState.Handle);
        }
        else
        {
            foreach (int current6 in this.m_BricksDic.Keys)
            {
                List<Brick> list3 = this.m_BricksDic[current6];
                foreach (Brick current7 in list3)
                {
                    int number3 = current7.m_Number;
                    int index2 = current7.m_Index;
                    int array3 = current7.m_Array;
                    int num3 = 0;
                    int num4 = 0;
                    if (this.m_BricksDic.ContainsKey(array3 - 1) && this.m_BricksDic[array3 - 1].Count > index2 && this.m_BricksDic[array3 - 1][index2].m_Number == number3 && number3 > 0)
                    {
                        num4 = ((this.m_BricksDic[array3 - 1][index2].m_Count <= num4) ? num4 : this.m_BricksDic[array3 - 1][index2].m_Count);
                        num3 = 1;
                    }
                    if (this.m_BricksDic.ContainsKey(array3 + 1) && this.m_BricksDic[array3 + 1].Count > index2 && this.m_BricksDic[array3 + 1][index2].m_Number == number3 && number3 > 0)
                    {
                        num4 = ((this.m_BricksDic[array3 + 1][index2].m_Count <= num4) ? num4 : this.m_BricksDic[array3 + 1][index2].m_Count);
                        if (this.m_BricksDic[array3 + 1][index2].m_Count >= num4)
                        {
                            num3 = 2;
                        }
                    }
                    if (list3.Count > index2 + 1)
                    {
                        Brick brick3 = list3[index2 + 1];
                        if (brick3.m_Number == number3 && number3 > 0)
                        {
                            num4 = ((brick3.m_Count <= num4) ? num4 : brick3.m_Count);
                            if (brick3.m_Count >= num4)
                            {
                                num3 = 4;
                            }
                        }
                    }
                    if (index2 > 0)
                    {
                        Brick brick4 = list3[index2 - 1];
                        if (brick4.m_Number == number3 && number3 > 0)
                        {
                            num4 = ((brick4.m_Count <= num4) ? num4 : brick4.m_Count);
                            if (brick4.m_Count >= num4)
                            {
                                num3 = 3;
                            }
                        }
                    }
                    if (current7.m_Count > num4)
                    {
                        if (num4 >= 2)
                        {
                            this.recursionBrick(current7, current7, null);
                        }
                        current7.ShowCombine();
                        this.BrickCombine();
                        this.PlayEffect(current7.transform.localPosition, current7.m_Number);

                        //UnityEngine.Debug.Log("BrickCombine ::::: " + current7.m_Number);
                    }
                    else if (current7.m_Count == num4)
                    {
                        if (current7.m_Count != 0)
                        {
                            if (num3 == 4 || (num3 == 1 && array3 <= this.m_LasetSelect) || (num3 == 2 && array3 >= this.m_LasetSelect))
                            {
                                if (num4 >= 2)
                                {
                                    this.recursionBrick(current7, current7, null);
                                }
                                current7.ShowCombine();
                                this.BrickCombine();
                                this.PlayEffect(current7.transform.localPosition, current7.m_Number);

                                //UnityEngine.Debug.Log("BrickCombine ::::: " + current7.m_Number);
                            }
                            else
                            {
                                current7.MoveToCombine(num3);
                            }
                        }
                    }
                    else
                    {
                        current7.MoveToCombine(num3);
                    }
                }
            }
            MonoSingleton<ShakeManager>.Instance.DoShake();
            this.ChangeState(GamePlayManager.GameState.Tween);
        }
        //this.openAchievement(num);
    }

    private void BrickCombine()
    {
        this.PlayCombineAudio(this.m_CombineOneTimes++);
        this.m_CombineTimes++;
    }

    private void GiveMoney()
    {
        if (this.m_CombineTimes >= this.m_GiveMoneyForCombineTimes && !MonoSingleton<MoneyManager>.Instance.State)
        {
            int num = this.RandomMoneyArray();
            if (this.m_BricksDic.ContainsKey(num))
            {
                List<Brick> list = this.m_BricksDic[num];
                MonoSingleton<MoneyManager>.Instance.Show(num, (list != null) ? list.Count : 0);
            }
            else
            {
                MonoSingleton<MoneyManager>.Instance.Show(num, 0);
            }
        }
    }

    private void PlayCombineAudio(int times = 0)
    {
        MonoSingleton<AudioManager>.Instance.PlayCombineAudio(times);
    }

    private int RandomMoneyArray()
    {
        int num = UnityEngine.Random.Range(0, 5);
        if (!this.m_BricksDic.ContainsKey(num))
        {
            return num;
        }
        List<Brick> list = this.m_BricksDic[num];
        if (list == null || list.Count < 7)
        {
            return num;
        }
        return this.RandomMoneyArray();
    }

    private void ShowScoreTweener(int score, Vector3 pos)
    {
        MonoSingleton<ScoreHitManager>.Instance.ShowScore(score, pos);
    }

    private void RefreshScore()
    {
        this.m_ScoreText.text = this.m_Score.ToString();
        if (!this.playLiHua && this.m_Score > this.m_MaxScore)
        {
            this.playLiHua = true;
        }
    }

    private void PlayEffect(Vector3 pos, int num)
    {
        MonoSingleton<EffectManager>.Instance.PlayEffect(pos, num);
    }

    private void ToBouns()
    {
        this.ChangeState(GamePlayManager.GameState.Pause);
        this.m_DownValue = 0f;
        this.m_ContentRoot.transform.DOLocalMove(Vector3.zero, 0.2f, false).OnComplete(delegate
        {
            float num = 0.5f;
            foreach (List<Brick> current in this.m_BricksDic.Values)
            {
                for (int i = current.Count - 1; i >= 0; i--)
                {
                    if (current[i].m_Number > 0)
                    {
                        num = Mathf.Max(num, 0.5f + (float)current[i].m_Index * 0.1f);
                        current[i].MoveToDeep((float)current[i].m_Index * 0.1f);
                        current.RemoveAt(i);
                    }
                }
            }
            base.transform.DOLocalMoveX(0f, num, false).OnComplete(delegate
            {
                foreach (List<Brick> current2 in this.m_BricksDic.Values)
                {
                    for (int j = 0; j < current2.Count; j++)
                    {
                        current2[j].Init(current2[j].m_Array, j, current2[j].m_Number, this.m_MaxValue);
                        current2[j].MoveTarget();
                    }
                }
                base.transform.DOLocalMoveX(0f, 0.2f, false).OnComplete(delegate
                {
                    this.ChangeState(GamePlayManager.GameState.Tween);
                });
            });
        });
    }

    private void ClearData()
    {
        foreach (List<Brick> current in this.m_BricksDic.Values)
        {
            foreach (Brick current2 in current)
            {
                UnityEngine.Object.Destroy(current2.gameObject);
            }
        }
        this.m_BricksDic.Clear();
        this.m_BrickCombineCount.Clear();
        this.m_Score = 0;
        this.m_DownValue = 0f;
        this.m_ContinueGame = false;
        this.m_isOpenLevelUpTip = false;
        this.GetMoney = 0;
    }

    public void ContinueGame()
    {
        this.m_ContinueGame = true;
        this.m_DownValue = 0f;
        base.transform.DOLocalMove(base.transform.localPosition, 0.2f, false).OnComplete(delegate
        {
            foreach (List<Brick> current in this.m_BricksDic.Values)
            {
                for (int i = current.Count - 1; i > 2; i--)
                {
                    current[i].MoveToDeep(0f);
                    current.RemoveAt(i);
                }
            }
            this.m_ContentRoot.transform.DOLocalMove(Vector3.zero, 0.5f, false).OnComplete(delegate
            {
                this.ChangeState(GamePlayManager.GameState.Tween);
            });
        });
    }

    public void UseSkin(SkinType type)
    {
    }

    [ContextMenu("Test")]
    public void LevelUpTween()
    {
        if (MonoSingleton<GameDataManager>.Instance.GameMode == 0 || MonoSingleton<GameDataManager>.Instance.GameMode == 2)
        {
            return;
        }
        if (this.m_isOpenLevelUpTip)
        {
            this.m_isOpenLevelUpTip = false;
            this.m_levelBg.gameObject.transform.localScale = Vector3.one;
            this.m_levelBg.gameObject.SetActive(true);
            this.m_LevelUpTweenr.Show(2f, 0f);
            this.m_levelBg.DOFade(0f, 2f).OnComplete(delegate
            {
                Color color = this.m_levelBg.color;
                color.a = 1f;
                this.m_levelBg.color = color;
                this.m_levelBg.gameObject.SetActive(false);
            }).SetDelay(2f).OnStart(delegate
            {
                this.m_LevelUpTweenr.Stop();
            });
        }
    }

    public void SetLevelUpContent(bool Leapfrogto)
    {
        this.m_isOpenLevelUpTip = true;
        string key = string.Empty;
        if (Leapfrogto)
        {
            key = "SPEEDLEVELUPTIP2";
        }
        else
        {
            key = "SPEEDLEVELUPTIP";
        }
        this.m_LevelUpText.text = Singleton<Localization>.instance.GetLable(key, 0);
        if (MonoSingleton<GameDataManager>.Instance.IsOpenAchieve == 2)
        {
            this.LevelUpTween();
        }
    }

    private bool checkGameWin()
    {
        if (MonoSingleton<GameDataManager>.Instance.GameMode == 2)
        {
            switch (1)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    SingleInstance<DebugManager>.Instance.LogError("GamePlayManager.checkGameWin error winType");
                    break;
            }
        }
        return false;
    }

    private void openAchievement(int num)
    {

        if (MonoSingleton<GameDataManager>.Instance.GameMode == 2)
        {
            return;
        }
        string achievementText = string.Empty;
        if (MonoSingleton<GameDataManager>.Instance.IsOpenAchieve == 0 && num == 2048)
        {
            achievementText = Singleton<Localization>.instance.GetLable("SPEEDLEVELUP", 0);
            MonoSingleton<GameDataManager>.Instance.IsOpenAchieve = 1;
        }
        else if (MonoSingleton<GameDataManager>.Instance.IsOpenAchieve <= 1 && num == 4096)
        {
            achievementText = Singleton<Localization>.instance.GetLable("SPEEDLEVELUP2", 0);
            MonoSingleton<GameDataManager>.Instance.IsOpenAchieve = 2;
        }
        else
        {
            if (MonoSingleton<GameDataManager>.Instance.IsOpenAchieve != 0 || num < 8192)
            {
                return;
            }
            achievementText = Singleton<Localization>.instance.GetLable("SPEEDLEVELUP3", 0);
            MonoSingleton<GameDataManager>.Instance.IsOpenAchieve = 2;
        }
        MonoSingleton<GameUIManager>.Instance.SetAchievementText(achievementText);
        this.GamePause();
        MonoSingleton<GameUIManager>.Instance.OpenAchievementPanel(delegate
        {
            // MonoSingleton<GAEvent>.Instance.Middle_All(num);
        }, 0f);
    }

    private void gaProcessBigNumCount(int num)
    {
        if (num < 2048 || num > 8192)
        {
            return;
        }
        if (num != 2048)
        {
            if (num != 4096)
            {
                if (num != 8192)
                {
                    UnityEngine.Debug.LogError("GamePlayManager.gaProcessBigNumCount error");
                }
                else
                {
                    MonoSingleton<GameDataManager>.Instance.m_process8192++;
                    // MonoSingleton<GAEvent>.Instance.Process8192(MonoSingleton<GameDataManager>.Instance.m_process8192);
                }
            }
            else
            {
                MonoSingleton<GameDataManager>.Instance.m_process4096++;
                // MonoSingleton<GAEvent>.Instance.Process4096(MonoSingleton<GameDataManager>.Instance.m_process4096);
            }
        }
        else
        {
            MonoSingleton<GameDataManager>.Instance.m_process2048++;
            // MonoSingleton<GAEvent>.Instance.Process2048(MonoSingleton<GameDataManager>.Instance.m_process2048);
        }
    }

    private void AddAndSortBricks(Brick newBrick)
    {
        this.m_ChangeList.Add(newBrick);
        int i;
        for (i = this.m_ChangeList.Count - 2; i >= 0; i--)
        {
            if (newBrick.m_Index < this.m_ChangeList[i].m_Index || (newBrick.m_Index == this.m_ChangeList[i].m_Index && newBrick.m_Array < this.m_ChangeList[i].m_Array))
            {
                break;
            }
            this.m_ChangeList[i + 1] = this.m_ChangeList[i];
        }
        this.m_ChangeList[i + 1] = newBrick;
    }

    private void recursionBrick(Brick combineBrick, Brick selfBrick, Brick ignoreBrick = null)
    {
        if (combineBrick == null || selfBrick == null)
        {
            SingleInstance<DebugManager>.Instance.LogError("GamePlayManager.recursionBrick param is null");
            return;
        }
        int index = selfBrick.m_Index;
        int array = selfBrick.m_Array;
        foreach (Brick current in this.m_ChangeList)
        {
            if (!(current == selfBrick))
            {
                if (!(ignoreBrick != null) || !(ignoreBrick == current))
                {
                    if (current.m_Count > 1)
                    {
                        if (current.m_Number == selfBrick.m_Number && selfBrick.m_Number > 0 && ((current.m_Index == index + 1 && current.m_Array == array) || (current.m_Index == index - 1 && current.m_Array == array) || (current.m_Index == index && current.m_Array == array - 1) || (current.m_Index == index && current.m_Array == array + 1)))
                        {
                            combineBrick.m_Count++;
                            this.recursionBrick(combineBrick, current, selfBrick);
                        }
                    }
                }
            }
        }
    }

    /**
     ************************************ 아이템 사용 추가 ************************************ 
     */
    public void UpdateEndUseItems()
    {
        ItemManager.Instance.type = ItemType.None;
        foreach (List<Brick> current in GamePlayManager.Instance.BricksDic.Values)
        {
            for (int i = current.Count - 1; i >= 0; i--)
            {
                current[i].m_Image.raycastTarget = false;
            }
        }

        useItemPanel.SetActive(false);
        this.ChangeState(GameState.Handle);
    }

    public void deleteOneBrick(Brick current)
    {
        GameDataManager.Instance.BrickItemsInfo[GameDataManager.Instance.PlayMode][2]--;
        GameDataManager.Instance.saveGamingItems();

        current.m_Destory = true;
        UpdateEndUseItems();

        this.Check();
        this.Movement();
    }

    // ANCHOR Delete 특정 라인 제거 (세로)
    public void deleteBricksVerticalLine(int arrayNum)
    {
        GameDataManager.Instance.BrickItemsInfo[GameDataManager.Instance.PlayMode][0]--;
        GameDataManager.Instance.saveGamingItems();

        foreach (Brick current in this.m_BricksDic[arrayNum])
        {
            current.m_Destory = true;
        }
        UpdateEndUseItems();

        this.Check();
        this.Movement();
    }

    public void deleteBricksUnderNumber(int number)
    {
        GameDataManager.Instance.BrickItemsInfo[GameDataManager.Instance.PlayMode][0]--;
        GameDataManager.Instance.saveGamingItems();

        foreach (List<Brick> current in this.m_BricksDic.Values)
        {
            foreach (Brick current2 in current)
            {
                if (current2.m_Number <= number)
                {
                    current2.m_Destory = true;
                }
            }
        }
        UpdateEndUseItems();

        this.Check();
        this.Movement();
    }

    public void UpdateBrickNumber(Brick current)
    {
        GameDataManager.Instance.BrickItemsInfo[GameDataManager.Instance.PlayMode][1]--;
        GameDataManager.Instance.saveGamingItems();

        current.m_Number *= 2;
        UpdateEndUseItems();

        this.Check();
        this.Movement();
    }

    public void ProcessBrickBoomb(int m_Array, int m_Index)
    {
        int i;
        List<Brick> bricks = null;

        // 가로 제거
        if(m_Array - 1 >= 0)
        {
            bricks = this.m_BricksDic[m_Array - 1];
            for(i = 0; i < bricks.Count; i++)
            {
                if(i == m_Index)
                {
                    bricks[i].m_Destory = true; bricks[i].m_Boomb = true;
                    break;
                }
            }
        }
        if(m_Array + 1 < 5)
        {
            bricks = this.m_BricksDic[m_Array + 1];
            for(i = 0; i < bricks.Count; i++)
            {
                if(i == m_Index)
                {
                    bricks[i].m_Destory = true; bricks[i].m_Boomb = true;
                    break;
                }
            }
        }
        // 세로 제거
        bricks = this.m_BricksDic[m_Array];
        for(i = 0; i < bricks.Count; i++)
        {
            if(i == m_Index - 1 || i == m_Index + 1)
            {
                bricks[i].m_Destory = true; bricks[i].m_Boomb = true;
            }
        }

        GameDataManager.Instance.BrickItemsInfo[GameDataManager.Instance.PlayMode][1]--;
        GameDataManager.Instance.saveGamingItems();

        UpdateEndUseItems();

        this.Check();
        this.Movement();
    }
}
