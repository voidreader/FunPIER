using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;
using SimpleJSON;
using System;
using Doozy.Engine;

public class PIER : MonoBehaviour {

    public static PIER main = null;
    public static int CurrentList = 0;
    public static int CurrentLevel = 0;

    public static Action CoinRefresh;
    public static bool IsSpecialist = false;

    [SerializeField] int debugCurrentList, debugCurrentLevel;
    public bool InfiniteMode = false;
    public int Coin = 0; // 보유 코인 
    public int NoAds = 0; // false. 광고 
    public int BestScore = 0;
    public int CurrentScore = 0; // 현 스코어 - 실패의 경우 0으로 초기화
    public int AdsCounter = 0; //광고 보여주는 타이밍체크 (4의 배수에서 보여준다. )

    // 일일 보상 
    public int DailyRewardDay = 0;
    public int DayOfYear = 0;

    public List<BossDataRow> ListBossData;
    public List<Sprite> ListBossPortrait;

    public JSONNode GunListNode;
    public string DebugGunList = string.Empty;
    string ColumnGun = "MyGun";

    public Weapon CurrentWeapon = null;


    void Awake() {
        main = this;
    }

    void Start() {
        ListBossData = BossData.Instance.Rows;
        LoadData();
    }

    #region 스페셜 리스트 

    /// <summary>
    /// 스페셜리스트 세팅 
    /// </summary>
    /// <param name="flag"></param>
    public void SetSpecialist(bool flag) {
        IsSpecialist = flag;

        if(IsSpecialist) { // 스페셜리스트 !
            AddGun(Stocks.main.WeaponSpecialist);
            ChangeEquipWeapon(Stocks.main.WeaponSpecialist);

            // 광고 모듈 처리 
            AdsManager.main.HideBannerView();
        }
        else { // 일반 
            RemoveGun(Stocks.main.WeaponSpecialist); // 특별 무기 제거
            AdsManager.main.ActivateBannerView(); // 배너뷰 살리기 
        }
    }

    #endregion

    #region Level, List 처리


    /// <summary>
    /// 스테이지 모두 클리어 후 무한 모드 진입여부 체크 
    /// </summary>
    void CheckInfiniteMode() {

        // 스테이지 더이상 없으면 인피니트 모드 
        if( ListBossData.Count <= CurrentLevel) {
            Debug.Log(">> Into Infinite Mode <<");
            InfiniteMode = true;
        }
        else {
            InfiniteMode = false;
        }
    }

    public static int GetListTargetCollectionPanel() {

        return PIER.CurrentList / 5;
    }


    /// <summary>
    /// 현재 스코어 처리 (게임실패시 초기화)
    /// </summary>
    /// <param name="score"></param>
    public void SaveCurrentScore(int score) {
        PlayerPrefs.SetInt(ConstBox.keyCurrentScore, score);
        PlayerPrefs.Save();
        CurrentScore = score;
    }

    /// <summary>
    /// 베스트 스코어 처리 
    /// </summary>
    /// <param name="score"></param>
    public void SaveBestScore(int score) {

        Debug.Log("Save Best Score :: " + score + "/" + BestScore);

        if(score <= BestScore) 
            return;
      

        PlayerPrefs.SetInt(ConstBox.keyBestScore, score);
        PlayerPrefs.Save();
        BestScore = score;

        // 게임 플랫폼 연동 
        PlatformManager.main.ReportScore(BestScore);
    }


    /// <summary>
    /// 아직 못받은 보상이 있는지 체크 
    /// </summary>
    /// <returns></returns>
    public bool HasWantedReward() {
        int maxListLevel = GetMaxLevelFromList(CurrentList);

        if (maxListLevel < CurrentLevel)
            return true;

        return false;

    }

    /// <summary>
    /// 레벨 클리어. 
    /// </summary>
    public void ClearLevel() {

        int maxListLevel = 0;
        CurrentLevel++; //  레벨 증가 

        // 현재 리스트에 제일 높은 레벨을 찾는다. 
        maxListLevel = GetMaxLevelFromList(CurrentList);
        if(maxListLevel < CurrentLevel) { // 현 원티드 리스트의 최고 레벨보다 현재 레벨이 더 높아졌다면..! 
            // CurrentList++; // 리스트도 증가. 
            // 리스트 증가는 나중에!(보상받을때)

            // 새로운 리스트가 있다! -- UI 호출해야함. 
            Debug.Log(">> New List!");
            GameEventMessage.SendEvent("GameClearEvent");
        }
        else {
            Debug.Log(">> There is no new list!");
            GameEventMessage.SendEvent("CallMain"); // 바로 메인으로 진입한다. 
        }

        CheckInfiniteMode(); // 모두 클리어 체크 .

        SaveData();
    }

    public int GetMaxLevelFromList(int l) {
        int max = 0;
        for(int i=0; i<ListBossData.Count;i++) {
            if(ListBossData[i]._list == l) {
                if (max < ListBossData[i]._level)
                    max = ListBossData[i]._level;
            }
        }

        return max;
    }

    /// <summary>
    /// Wanted 진행도 
    /// </summary>
    /// <returns></returns>
    public float GetWantedListProgressValue() {
        int listNo = CurrentList;
        int total = 0;
        int current = 0;

        for(int i=0; i< ListBossData.Count;i++) {
            if(ListBossData[i]._list == listNo) {
                total++;

                if (ListBossData[i]._level < CurrentLevel)
                    current++; // 이미 클리어한 보스 카운팅 

            }
        } // 리스트에 들어가는 보스(스테이지) 카운팅 


        return (float)current / (float)total;

    }

    #endregion

    #region 저장된 데이터 로드 , 재화처리
    /// <summary>
    /// 데이터 로드 
    /// </summary>
    void LoadData() {

        CurrentList = 0;
        CurrentLevel = 0;
        Coin = 0;

        DailyRewardDay = 0;
        DayOfYear = -1;

        BestScore = 0;
        CurrentScore = 0;
        AdsCounter = 0;
        GunListNode = JSON.Parse("{}");

        if (PlayerPrefs.HasKey(ConstBox.keyADCount))
            AdsCounter = PlayerPrefs.GetInt(ConstBox.keyADCount);

        // 현상수배범 리스트 번호 
        if (PlayerPrefs.HasKey(ConstBox.keyCurrentList))
            CurrentList = PlayerPrefs.GetInt(ConstBox.keyCurrentList);

        // 현재 스테이지 
        if (PlayerPrefs.HasKey(ConstBox.keyCurrentLevel))
            CurrentLevel = PlayerPrefs.GetInt(ConstBox.keyCurrentLevel);

        // 보유 코인 
        if (PlayerPrefs.HasKey(ConstBox.keyCurrentCoin))
            Coin = PlayerPrefs.GetInt(ConstBox.keyCurrentCoin); // 코인 

        if (PlayerPrefs.HasKey(ConstBox.keyBestScore))
            BestScore = PlayerPrefs.GetInt(ConstBox.keyBestScore); // 베스트 스코어 

        if (PlayerPrefs.HasKey(ConstBox.keyCurrentScore))
            CurrentScore = PlayerPrefs.GetInt(ConstBox.keyCurrentScore); // 현재 스코어

        // 출첵일수
        if (PlayerPrefs.HasKey(ConstBox.keyDailyRewardDay))
            DailyRewardDay = PlayerPrefs.GetInt(ConstBox.keyDailyRewardDay);

        // day of year 
        if (PlayerPrefs.HasKey(ConstBox.keyDayOfYear))
            DayOfYear = PlayerPrefs.GetInt(ConstBox.keyDayOfYear);

        // 보유 무기 리스트 
        if (PlayerPrefs.HasKey(ConstBox.keyGunList))
            GunListNode = JSON.Parse(PlayerPrefs.GetString(ConstBox.keyGunList)); // 건리스트 
        else {


            GunListNode[ColumnGun][-1]["name"] = "pistol";
            SaveGun();
            // GunListNode[ColumnGun][-1]["name"] = "autorifle";
            // GunListNode[ColumnGun][-1]["name"] = "kitchenknife";
        }

        DebugGunList = GunListNode.ToString();

        // 장착한 무기 
        LoadEquipWeapon();

        // 테스트 용도 
         CurrentList = 10;
         CurrentLevel = 43;
        // CurrentList = 20;
        // CurrentLevel = 86;

        debugCurrentLevel = CurrentLevel;
        debugCurrentList = CurrentList;


        CheckInfiniteMode();

    }

    /// <summary>
    /// 데이터 저장 
    /// </summary>
    public void SaveData() {
        PlayerPrefs.SetInt(ConstBox.keyCurrentList, CurrentList); // 리스트 
        PlayerPrefs.SetInt(ConstBox.keyCurrentLevel, CurrentLevel); // 스테이지 
        PlayerPrefs.SetInt(ConstBox.keyCurrentCoin, Coin); // 코인
        PlayerPrefs.SetInt(ConstBox.keyBestScore, BestScore);// 베스트 스코어 
        PlayerPrefs.SetString(ConstBox.keyEquipGun, CurrentWeapon.name); // 장착한 무기 
        PlayerPrefs.Save();

        SaveGun();

        debugCurrentLevel = CurrentLevel;
        debugCurrentList = CurrentList;
    }

    /// <summary>
    /// 일일 보상 관련 저장 
    /// </summary>
    /// <param name="d"></param>
    public void SaveDailyRewardDay(int d) {
        int day = d;
        if (day > 4)
            day = 0; // 5일자 다 받았으면 다시 0으로 돌아간다

        PlayerPrefs.SetInt(ConstBox.keyDailyRewardDay, day);
        PlayerPrefs.SetInt(ConstBox.keyDayOfYear, DateTime.Now.DayOfYear);
        PlayerPrefs.Save();

        // 변수 갱신
        DayOfYear = DateTime.Now.DayOfYear;
    }


    /// <summary>
    /// 게임 오버, 클리어마다 체크해서 광고 오픈 
    /// </summary>
    public void AddAdCounter() {
        AdsCounter++;

        if(AdsCounter % 4 == 0) {
            AdsManager.main.OpenMidAdvertisement();
        }

        PlayerPrefs.SetInt(ConstBox.keyADCount, AdsCounter);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 코인 획득 
    /// </summary>
    /// <param name="c"></param>
    public void AddCoin(int c) {
        Coin += c;
        CoinRefresh();


        PlayerPrefs.SetInt(ConstBox.keyCurrentCoin, Coin); // 코인
        PlayerPrefs.Save();
    }


    /// <summary>
    /// 보유중인 총 체크 
    /// </summary>
    /// <param name="w"></param>
    /// <returns></returns>
    public bool HasGun(Weapon w) {

        for (int i = 0; i < GunListNode[ColumnGun].Count; i++) {

            if(GunListNode[ColumnGun][i]["name"] == w.name) {
                return true;
            }

        }

        return false;
    }

    /// <summary>
    /// 구매한 총 추가
    /// </summary>
    /// <param name="w"></param>
    public void AddGun(Weapon w) {

        Debug.Log(w.WeaponID + "Get!! ");

        if (HasGun(w))
            return;

        GunListNode[ColumnGun][-1]["name"] = w.name;
        SaveGun();
    }

    public void RemoveGun(Weapon w) {
        for(int i=0; i< GunListNode[ColumnGun].Count;i++) {

            if (GunListNode[ColumnGun][i]["name"] == w.name)
                GunListNode[ColumnGun].Remove(i);
        }

        if(CurrentWeapon == w) {
            // ChangeEquipWeapon(Stocks.GetWeaponByID(GunListNode )
            ChangeEquipWeapon(Stocks.main.WeaponBasic);
        }

        

        SaveGun();
    }

    /// <summary>
    /// 저장 
    /// </summary>
    void SaveGun() {
        PlayerPrefs.SetString(ConstBox.keyGunList, GunListNode.ToString()); // 건리스트 
        DebugGunList = GunListNode.ToString();
        PlayerPrefs.Save();
    }


    /// <summary>
    /// 무기 장착하기 
    /// </summary>
    /// <param name="w"></param>
    public void ChangeEquipWeapon(Weapon w) {
        CurrentWeapon = w;
        PlayerPrefs.SetString(ConstBox.keyEquipGun, CurrentWeapon.name);
        PlayerPrefs.Save();
    }


    void LoadEquipWeapon() {
        string name = string.Empty;

        if (PlayerPrefs.HasKey(ConstBox.keyEquipGun))
            name = PlayerPrefs.GetString(ConstBox.keyEquipGun);
        else // 없으면 기본값 피스톨 
            name = "pistol";

        // name으로 무기 찾기 

        for(int i = 0; i < Stocks.main.ListWeapons.Count; i++) {
            if (Stocks.main.ListWeapons[i].name == name) {
                CurrentWeapon = Stocks.main.ListWeapons[i];
                return;
            }
        }
        
    }

    #endregion

    #region 보스 데이터 가져오기

    /// <summary>
    /// 현 보스 블랙리스트 가져오기 
    /// </summary>
    /// <returns></returns>
    public List<BossDataRow> GetCurrentBlacklist() {
        List<BossDataRow> r = new List<BossDataRow>();

        for(int i =0;i<ListBossData.Count;i++) {

            if(ListBossData[i]._list == CurrentList) //current list 맞는것만 가져온다.
                r.Add(ListBossData[i]);
        }

        return r;
    }

    /// <summary>
    /// 보스 초상화 스프라이트 가져오기 
    /// </summary>
    /// <param name="portrait"></param>
    /// <returns></returns>
    public Sprite GetBossPortraitSprite(string p) {
        for(int i =0; i<ListBossPortrait.Count;i++) {
            if (ListBossPortrait[i].name == p)
                return ListBossPortrait[i];
        }

        return null;
    }

    

    #endregion

}
