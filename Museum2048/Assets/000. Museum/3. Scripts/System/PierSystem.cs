using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Google2u;
using CodeStage.AntiCheat.ObscuredTypes;

public class PierSystem : MonoBehaviour {

    public static PierSystem main = null;

    // 레드문 관련 
    public static string currentRedMoonItem = string.Empty;
    public static int currentRedMoonValue = 0;
        

    readonly string SaveData = "SaveData";
    public bool AdminPlay = false;
    public SystemLanguage currentLang = SystemLanguage.Korean;

    // 아이템 개수 
    public ObscuredInt itemCleaner = 0;
    public ObscuredInt itemUpgrade = 0;
    public ObscuredInt itemBack = 0;

    public ObscuredInt carHighScore = 0;
    public ObscuredInt wineHighScore = 0;
    public ObscuredInt vikingHighScore = 0;
    public ObscuredInt iceHighScore = 0;
    public ObscuredInt currentScore = 0;

    // 각 박물관 진척도 
    public ObscuredInt carMuseumStep = 0;
    public ObscuredInt wineMuseumStep = 0;
    public ObscuredInt vikingMuseumStep = 0;
    public ObscuredInt iceMuseumStep = 0;

    // 연출을 위한 백업용 step 값. 2048 플레이 전의 스텝 값
    public int carMuseumPreviousStep = 0;
    public int wineMuseumPreviousStep = 0;
    public int vikingMuseumPreviousStep = 0;
    public int iceMuseumPreviousStep = 0;


    public ObscuredInt NoAds = 0; // 노 광고..  0(광고있음), 1(광고없음)

    public int MaxCarMuseumStep = 0;
    public int MaxWineMuseumStep = 0;
    public int MaxVikingMuseumStep = 0;
    public int MaxIceMuseumStep = 0;

    public ObscuredLong MoveCount = 0; // 오브젝트 이동 카운트 

    public int themeIndex = 0;



    public List<NGUIAtlas> _themeAtlas; // 진행상황용 Atlas 


    
    private void Awake() {
        main = this;

        // 프레임레이트 고정
        Application.targetFrameRate = 60;

        // listMuseums[0].transform.position = new Vector3(20, 8, 20);
        InitGameBaseData(); // 게임 데이터 초기화
        LoadProfile(); // 데이터 로드

    }

    // Start is called before the first frame update
    void Start() {

        // 플랫폼 
        PlatformManager.main.InitPlatformService();

        // listMuseums[0].transform.position = posCarMuseum;
    }



    /// <summary>
    /// 게임 Data 초기화 및 언어처리
    /// </summary>
    public void InitGameBaseData() {
        MaxCarMuseumStep = MData.Instance.GetRow(MData.rowIds.CAR_MUSEUM)._Step;
        MaxWineMuseumStep = MData.Instance.GetRow(MData.rowIds.WINE_MUSEUM)._Step;
        MaxVikingMuseumStep = MData.Instance.GetRow(MData.rowIds.VIKING_MUSEUM)._Step;
        MaxIceMuseumStep = MData.Instance.GetRow(MData.rowIds.ICE_MUSEUM)._Step;


        // 언어 처리 ! 중요
        if (ES2.Exists(ConstBox.KeySavedLang)) {
            currentLang = LoadLanguage();
        } 
        else {

            if (Application.systemLanguage == SystemLanguage.Korean)
                currentLang = SystemLanguage.Korean;
            else
                currentLang = SystemLanguage.English;
        }

        SaveLanguage();

    }

    public void SaveLanguage() {
        ES2.Save<string>(currentLang.ToString(), ConstBox.KeySavedLang);
    }

    public static SystemLanguage LoadLanguage() {
        return (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), ES2.Load<string>(ConstBox.KeySavedLang));
    }



    #region 데이터 저장 로드 

    public void SaveCurrentThemeStep(Theme theme, int step) {
        switch (theme) {
            case Theme.Car:
                carMuseumStep = step;
                break;

            case Theme.Wine:
                wineMuseumStep = step;
                break;

            case Theme.Viking:
                vikingMuseumStep = step;
                break;

            case Theme.Ice:
                iceMuseumStep = step;
                break;

        }
    }

    /// <summary>
    /// 데이터 저장
    /// </summary>
    public void SaveProfile() {
        ES2.Save<string>(GetSaveJSONString(), SaveData);
    }

    /// <summary>
    /// 데이터 불러오기
    /// </summary>
    public void LoadProfile() {
        if (!ES2.Exists(SaveData)) {
            itemCleaner = 0;
            itemUpgrade = 0;
            itemBack = 0;

            carMuseumStep = 0;
            wineMuseumStep = 0;
            vikingMuseumStep = 0;
            iceMuseumStep = 0;

            NoAds = 0;
            themeIndex = 0;

            carMuseumPreviousStep = 0;
            wineMuseumPreviousStep = 0;
            vikingMuseumPreviousStep = 0;
            iceMuseumPreviousStep = 0;

            carHighScore = 0;
            wineHighScore = 0;
            vikingHighScore = 0;
            iceHighScore = 0;

            MoveCount = 0;

        }
        else {
            JSONNode node = JSON.Parse(ES2.Load<string>(SaveData));
            itemCleaner = node["cleaner"].AsInt;
            itemUpgrade = node["upgrader"].AsInt;
            itemBack = node["back"].AsInt;

            carMuseumStep = node["carstep"].AsInt;
            wineMuseumStep = node["winestep"].AsInt;
            vikingMuseumStep = node["vikingstep"].AsInt;
            iceMuseumStep = node["icestep"].AsInt;

            carMuseumPreviousStep = node["carpreviousstep"].AsInt;
            wineMuseumPreviousStep = node["winepreviousstep"].AsInt;
            vikingMuseumPreviousStep = node["vikingpreviousstep"].AsInt;
            iceMuseumPreviousStep = node["icepreviousstep"].AsInt;

            carHighScore = node[Theme.Car.ToString() + "highscore"].AsInt;
            wineHighScore = node[Theme.Wine.ToString() + "highscore"].AsInt;
            vikingHighScore = node[Theme.Viking.ToString() + "highscore"].AsInt;
            iceHighScore = node[Theme.Ice.ToString() + "highscore"].AsInt;

            NoAds = node["noads"].AsInt;
            themeIndex = node["themeindex"].AsInt;

            MoveCount = node["movecount"].AsLong;

        }

        NoAds = 1; // 주석 풀면 광고없음 
    }


    /// <summary>
    /// 저장용 스트링 생성
    /// </summary>
    /// <returns></returns>
    string GetSaveJSONString() {
        JSONNode node = JSON.Parse("{}");

        // 아이템 정보 
        node["cleaner"].AsInt = itemCleaner;
        node["upgrader"].AsInt = itemUpgrade;
        node["back"].AsInt = itemBack;

        // 단계 정보
        node["carstep"].AsInt = carMuseumStep;
        node["winestep"].AsInt = wineMuseumStep;
        node["vikingstep"].AsInt = vikingMuseumStep;
        node["icestep"].AsInt = iceMuseumStep;

        node["noads"].AsInt = NoAds;
        node["themeindex"].AsInt = themeIndex;

        node["carpreviousstep"].AsInt = carMuseumPreviousStep;
        node["winepreviousstep"].AsInt = wineMuseumPreviousStep;
        node["vikingpreviousstep"].AsInt = vikingMuseumPreviousStep;
        node["icepreviousstep"].AsInt = iceMuseumPreviousStep;

        node[Theme.Car.ToString() + "highscore"].AsInt = carHighScore;
        node[Theme.Wine.ToString() + "highscore"].AsInt = wineHighScore;
        node[Theme.Viking.ToString() + "highscore"].AsInt = vikingHighScore;
        node[Theme.Ice.ToString() + "highscore"].AsInt = iceHighScore;

        node["movecount"].AsLong = MoveCount;

        return node.ToString();

    }

    #endregion


    #region 공통 메소드 

    public static JSONNode GetEmptyNode() {
        return JSON.Parse("{}");
    }

    /// <summary>
    /// 로컬 텍스트 가져오기 
    /// </summary>
    /// <param name="rowid"></param>
    /// <returns></returns>
    public static string GetLocalizedText(MLocal.rowIds rowid) {

        if (main.currentLang == SystemLanguage.Korean)
            return MLocal.Instance.GetRow(rowid)._Korean;
        else
            return MLocal.Instance.GetRow(rowid)._English;


    }

    public static string GetThemeName(Theme t) {
        switch (t) {
            case Theme.Car:
                return GetLocalizedText(MLocal.rowIds.TEXT1);
            case Theme.Wine:
                return GetLocalizedText(MLocal.rowIds.TEXT2);
            case Theme.Viking:
                return GetLocalizedText(MLocal.rowIds.TEXT3);
            case Theme.Ice:
                return GetLocalizedText(MLocal.rowIds.TEXT63);
        }

        return string.Empty;
    }

    public static Vector3 GetBGSpriteEffectRandomPosition() {

        int updown = Random.Range(0, 2);
        if (updown == 0) { // 상
            return new Vector3(Random.Range(-3f, 0.7f), Random.Range(1.7f, 5f), 0);
        }
        else {
            return new Vector3(Random.Range(-2.6f, 2.6f), Random.Range(-5.2f, -4.2f), 0);
        }

        // 상하를 나눈다. 
        // 상 y : 1.7~5, x 0.7 ~ -3
        // 하 y: -4.2~ -5.2 , x: -2.7, 2.7

        
    }

    #endregion

    #region 테마별 정보 가져오기

    /// <summary>
    /// 대상 테마의 최대 진척도 정보 조회
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public int GetMaxProgress(Theme t) {
        switch(t) {
            case Theme.Car:
                return MaxCarMuseumStep;
            case Theme.Wine:
                return MaxWineMuseumStep;
            case Theme.Viking:
                return MaxVikingMuseumStep;
            case Theme.Ice:
                return MaxIceMuseumStep;

        }

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public int GetHighScore(Theme t) {
        switch (t) {
            case Theme.Car:
                return carHighScore;
            case Theme.Wine:
                return wineHighScore;
            case Theme.Viking:
                return vikingHighScore;
            case Theme.Ice:
                return iceHighScore;
        }

        return 0;
    }

    public void SetHighScore(Theme t, int score) {
        switch (t) {
            case Theme.Car:
                carHighScore = score;
                break; 

            case Theme.Wine:
                wineHighScore = score;
                break;

            case Theme.Viking:
                vikingHighScore = score;
                break;

            case Theme.Ice:
                iceHighScore = score;
                break;
        }

        PierSystem.main.SaveProfile();
    }


    /// <summary>
    /// 대상 테마의 현재 진척도 정보 조회 
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public int GetCurrentProgress(Theme t) {
        switch (t) {
            case Theme.Car:
                return carMuseumStep;
            case Theme.Wine:
                return wineMuseumStep;
            case Theme.Viking:
                return vikingMuseumStep;
            case Theme.Ice:
                return iceMuseumStep;
        }

        return 0;
    }

    public void SetPreviousProgress(Theme t) {
        switch(t) {
            case Theme.Car:
                carMuseumPreviousStep = GetCurrentProgress(t);
                break;
            case Theme.Wine:
                wineMuseumPreviousStep = GetCurrentProgress(t);
                break;
            case Theme.Viking:
                vikingMuseumPreviousStep = GetCurrentProgress(t);
                break;

            case Theme.Ice:
                iceMuseumPreviousStep = GetCurrentProgress(t);
                break;
        }
    }

    /// <summary>
    /// 게임 플레이 전 단계 정보 조회
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public int GetPreviousProgress(Theme t) {
        switch (t) {
            case Theme.Car:
                return carMuseumPreviousStep;
            case Theme.Wine:
                return wineMuseumPreviousStep;
            case Theme.Viking:
                return vikingMuseumPreviousStep;
            case Theme.Ice:
                return iceMuseumPreviousStep;
        }

        return 0;
    }

    /// <summary>
    /// step으로 칩의 ID구하기 
    /// </summary>
    /// <param name="step"></param>
    /// <returns></returns>
    public static int GetIDByStep(int step) {
        switch (step) {
            case 1:
                return 2;
            case 2:
                return 4;
            case 3:
                return 8;
            case 4:
                return 16;
            case 5:
                return 32;
            case 6:
                return 64;
            case 7:
                return 128;
            case 8:
                return 256;
            case 9:
                return 512;
            case 10:
                return 1024;
            case 11:
                return 2048;
            case 12:
                return 4096;
            case 13:
                return 8192;
            case 14:
                return 16384;

        }

        return 0;

    }

    #endregion
}
