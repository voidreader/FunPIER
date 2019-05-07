using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PathologicalGames;

public class LobbyManager : MonoBehaviour {

    public static LobbyManager main = null;
    public static bool isAnimation = false;

    // 테마 관련 개체들 
    public Theme currentTheme = Theme.Car;
    public UILabel lblThemeName; // 테마 명칭
    public UIProgressBar themeProgressBar; // 진척도 게이지 
    public UILabel lblCurrentProgress; // 현재 진척도 
    public UILabel lblTotalProgress; // 전체 진척도 
    public GameObject btnNoAds; // 광고없애기 버튼 

    public GameObject tutorialButton; // 튜토리얼 버튼 

    public UISprite splash;

    
    public GameObject lobbyPanel;
    public Transform lobbyBottomGroup;
    // public UIButton playButton;
    public UIButton btnThemeLeft; // 테마 좌우
    public UIButton btnThemeRight; // 테마 좌우 
    public int themeIndex = 0;

    public List<MuseumStruct> listStructs; // 로비 뮤지엄 구조물들.
    readonly float structPosY = -6;

    #region 파티클 
    public ParticleSystem _Firework1, _Firework2, _Firework3;
    public ParticleSystem _upgradeStar;
    #endregion

    public GameObject bgLobby, bgInGame;


    bool isTallScreen;


    private void Awake() {
        main = this;

        float screenw = (float)Screen.width;
        float screenh = (float)Screen.height;

        float ratio = screenw / screenh;

        isTallScreen = false;
        if (ratio < 0.56f)
            isTallScreen = true;


        if (isTallScreen)
            lobbyBottomGroup.transform.localPosition = new Vector3(0, -650, 0);
        else
            lobbyBottomGroup.transform.localPosition = new Vector3(0, -550, 0);

    }

    void Update() {

        if(Input.GetKeyDown(KeyCode.U))
            TestUpgrade();
    }

    /// <summary>
    /// 광고 없애기 구매 
    /// </summary>
    public void BuyNoAds() {
        IAPControl.main.Purchase("noads_m2048");
    }

    /// <summary>
    /// 이전에 플레이 하던 게임이 있는지 체크 
    /// </summary>
    bool CheckNeedResumeGame() {
        bool hasHistory = InGame.main.LoadInGameSnapShot();

        if (!hasHistory)
            return false; // 재개할 게임 없음




        return true;
    }


    // Start is called before the first frame update
    IEnumerator Start() {

        
        splash.alpha = 1;
        splash.gameObject.SetActive(true); // 일단 까맣게 만들어놓고 시작.


        // 기본 초기화 로직 
        // 박물관 구조물 초기화 
        InitStructs();

        yield return null;

        // 즉시 포커스 
        FocusThemeInstantly(PierSystem.main.themeIndex);
        themeIndex = PierSystem.main.themeIndex;

        yield return null;

        // 이전에 플레이 하던 게임이 있는지 체크 
        if (CheckNeedResumeGame()) {

            Debug.Log(">> Need Resume Checked in Lobby");

            // 바로 인게임 시작.
            PlayGame(true);
        }
        else {
            ShowLobbyUI(true);
            SplashClean(0.1f); // 스플래시 클린.

            AudioAssistant.main.PlayMusic("LobbyBGM", true);



        }

        yield return StartCoroutine(DelayedInit());
    }


    /// <summary>
    /// 서프라이즈팩 오픈 
    /// </summary>
    void CheckSurprisePack() {

        if (PierSystem.main.NoAds > 0)
            return;

        if (!IAPControl.IsInitialized)
            return;

        int dayofyear = 0;
        if (!ES2.Exists(ConstBox.KeySavedSurprisePack))
            dayofyear = -1;
        else
            dayofyear = ES2.Load<int>(ConstBox.KeySavedSurprisePack);


        // 깜짝패키지 오픈
        // 하루에 한번만 오픈 
        if (!ES2.Exists(ConstBox.KeySavedSurprisePack) || dayofyear != System.DateTime.Now.DayOfYear) {
            PageManager.main.OpenPage(UILayerEnum.SurprisePack);
        }
    }



    IEnumerator DelayedInit() {
        yield return new WaitForSeconds(3);
        AdsControl.main.RequestBanner();
        AdsControl.main.RequestInterstitial();
        AdsControl.main.RequestRewardAd();
    }


    /// <summary>
    /// 로비 UI 제어
    /// </summary>
    /// <param name="flag"></param>
    void ShowLobbyUI(bool flag) {

        lobbyPanel.SetActive(flag);

        bgLobby.SetActive(flag);
        bgInGame.SetActive(!flag);

        if (flag) {
            CheckLeftRightButton();


            CheckSurprisePack();
        }

    }

    #region 박물관 구조체 제어
    /// <summary>
    /// 박물관 구조체 초기화 
    /// </summary>
    public void InitStructs() {
        for (int i = 0; i < listStructs.Count; i++) {
            listStructs[i].gameObject.SetActive(false);
        }


        listStructs[0].InitMuseumStep(PierSystem.main.carMuseumStep, PierSystem.main.MaxCarMuseumStep);
        listStructs[1].InitMuseumStep(PierSystem.main.wineMuseumStep, PierSystem.main.MaxWineMuseumStep);
        listStructs[2].InitMuseumStep(PierSystem.main.vikingMuseumStep, PierSystem.main.MaxVikingMuseumStep);
    }

    public void HideStructs() {
        for (int i = 0; i < listStructs.Count; i++) {
            listStructs[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 즉시 포커스! 
    /// </summary>
    /// <param name="index"></param>
    public void FocusThemeInstantly(int index) {
        for (int i = 0; i < listStructs.Count; i++) {
            listStructs[i].gameObject.SetActive(false);
        }

        listStructs[index].transform.position = new Vector3(0, structPosY, 0); // 
        listStructs[index].gameObject.SetActive(true);

        currentTheme = listStructs[index].theme;
        SetCurrentThemeInfo();
    }

    /// <summary>
    /// 현재 테마정보 세팅 
    /// </summary>
    public void SetCurrentThemeInfo() {
        lblThemeName.text = PierSystem.GetThemeName(currentTheme);  // 테마 이름
        lblTotalProgress.text = "/" + PierSystem.main.GetMaxProgress(currentTheme).ToString();
        lblCurrentProgress.text = PierSystem.main.GetCurrentProgress(currentTheme).ToString();

        themeProgressBar.value = (float)PierSystem.main.GetCurrentProgress(currentTheme) / (float)PierSystem.main.GetMaxProgress(currentTheme);

        // 튜토리얼 버튼은 자동차에서만 등장 
        if(currentTheme == Theme.Car) {
            tutorialButton.transform.localScale = Vector3.zero;
            tutorialButton.SetActive(true);
            tutorialButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
        }
        else {
            tutorialButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack).OnComplete(()=>OnScaleZero(tutorialButton));
        }

    }

    void OnScaleZero(GameObject obj) {
        obj.SetActive(false);
    }


    /// <summary>
    /// 왼쪽으로 이동하기 
    /// </summary>
    public void OnClickThemeLeft() {
        if (themeIndex <= 0)
            return;

        if (LobbyManager.isAnimation)
            return;

        LobbyManager.isAnimation = true;

        listStructs[themeIndex].transform.DOMoveX(-20, 0.65f);
        themeIndex--;

        listStructs[themeIndex].transform.position = new Vector3(20, structPosY, 0);
        listStructs[themeIndex].gameObject.SetActive(true);
        listStructs[themeIndex].transform.DOMoveX(0, 0.65f).OnComplete(OnCompleteThemeMove); ;

        PierSystem.main.themeIndex = themeIndex;
        PierSystem.main.SaveProfile(); // 저장
        currentTheme = listStructs[themeIndex].theme; // 현재 theme 
        SetCurrentThemeInfo();
        CheckLeftRightButton();

        // themeIndex - 1이 -20으로 이동. 


    }

    public void OnClickThemeRight() {
        if (themeIndex+1 >= listStructs.Count)
            return;

        if (LobbyManager.isAnimation)
            return;

        LobbyManager.isAnimation = true;

        listStructs[themeIndex].transform.DOMoveX(20, 0.65f);
        themeIndex++;

        listStructs[themeIndex].transform.position = new Vector3(-20, structPosY, 0);
        listStructs[themeIndex].gameObject.SetActive(true);
        listStructs[themeIndex].transform.DOMoveX(0, 0.65f).OnComplete(OnCompleteThemeMove);

        PierSystem.main.themeIndex = themeIndex;
        PierSystem.main.SaveProfile(); // 저장

        currentTheme = listStructs[themeIndex].theme; // 현재 theme 
        SetCurrentThemeInfo();
        CheckLeftRightButton();

    }


    void OnCompleteThemeMove() {
        LobbyManager.isAnimation = false;
    }


    /// <summary>
    /// 테마 이동 버튼 제어
    /// </summary>
    void CheckLeftRightButton() {

        btnThemeLeft.gameObject.SetActive(false);
        btnThemeRight.gameObject.SetActive(false);

        if(themeIndex == 0)
            btnThemeRight.gameObject.SetActive(true);
        else if (themeIndex == listStructs.Count-1)
            btnThemeLeft.gameObject.SetActive(true);
        else {
            btnThemeLeft.gameObject.SetActive(true);
            btnThemeRight.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 불꽃놀이 개장
    /// </summary>
    public void ShotFireworks() {
        StartCoroutine(FireworkRoutine());
    }

    IEnumerator FireworkRoutine() {
        
        PoolManager.Pools[ConstBox.poolIngame].Spawn(_Firework1, new Vector3(-1, -5, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));
        yield return new WaitForSeconds(0.3f);

        PoolManager.Pools[ConstBox.poolIngame].Spawn(_Firework2, new Vector3(1, -5, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));
        yield return new WaitForSeconds(0.25f);

        PoolManager.Pools[ConstBox.poolIngame].Spawn(_Firework1, new Vector3(-1, -5, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));
        yield return new WaitForSeconds(0.3f);

        PoolManager.Pools[ConstBox.poolIngame].Spawn(_Firework3, new Vector3(1, -5, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));
        yield return new WaitForSeconds(0.35f);


        PoolManager.Pools[ConstBox.poolIngame].Spawn(_Firework2, new Vector3(2, -5, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));
        yield return new WaitForSeconds(0.4f);

        PoolManager.Pools[ConstBox.poolIngame].Spawn(_Firework3, new Vector3(-2, -5, 0), Quaternion.Euler(new Vector3(-90, 0, 0)));
        

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    public void ShowUpgradeStar(Vector3 pos) {
        PoolManager.Pools[ConstBox.poolIngame].Spawn(_upgradeStar, pos, Quaternion.identity);
    }

    #endregion



    /// <summary>
    /// 게임 플레이 
    /// </summary>
    public void PlayGame(bool resume = false) {
        // InGame.main.StartSession(currentTheme);
        StartCoroutine(OnPlayGameRoutine(resume));
    }

    IEnumerator OnPlayGameRoutine(bool resume) {
        isAnimation = true;

        // 로비 초기화때, 화면은 까만 상태. 
        if(resume) { // 재시작시에는 과정이 조금 다르다. 
            ShowLobbyUI(false);
            InGame.main.ShowInGameUIs(true);
            HideStructs();
            InGame.main.StartSession(currentTheme, resume);
            yield return StartCoroutine(SplashingClear());
        }
        else { // 신규 게임 시작 
            yield return StartCoroutine(SplasingBlack());

            ShowLobbyUI(false);

            InGame.main.ShowInGameUIs(true);
            HideStructs();

            yield return new WaitForSeconds(0.5f);
            InGame.main.StartSession(currentTheme, resume);
            yield return StartCoroutine(SplashingClear());

            // previousStep은 오직 이부분에서만 수정해야한다.
            switch (currentTheme) {
                case Theme.Car:
                    PierSystem.main.carMuseumPreviousStep = PierSystem.main.GetCurrentProgress(currentTheme);
                    break;

                case Theme.Wine:
                    PierSystem.main.wineMuseumPreviousStep = PierSystem.main.GetCurrentProgress(currentTheme);
                    break;

                case Theme.Viking:
                    PierSystem.main.vikingMuseumPreviousStep = PierSystem.main.GetCurrentProgress(currentTheme);
                    break;
            }
            PierSystem.main.SaveProfile(); // 이전단계 정보 저장

        }

        isAnimation = false;
    }

    /// <summary>
    /// 2048 하고 로비로 돌아왔을 때.
    /// </summary>
    public void LobbyFrom2048() {

        Debug.Log(" >> Called Lobby From 2048 <<");

        FocusThemeInstantly(themeIndex); // 

        // 업그레이드 
        MuseumStruct target = GetCurrentMuseumStruct();

        if(target == null) {
            Debug.Log("Error in GetCurrentMuseumStruct");
            return;
        }

               

        target.gameObject.SetActive(true);

        Debug.Log(currentTheme.ToString() + "Upgrade Check");

        target.UpgradeMuseum(InGame.main.currentThemeStep); // 업그레이드 처리 
            

        // 로비 UI
        ShowLobbyUI(true);



    }

    /// <summary>
    /// 테스트 업그레이드 로직 
    /// </summary>
    public void TestUpgrade() {
        MuseumStruct target = GetCurrentMuseumStruct();
        target.UpgradeMuseum(PierSystem.main.GetMaxProgress(target.theme));
    }


    /// <summary>
    /// 현재 테마의 구조물 가져오기 
    /// </summary>
    /// <returns></returns>
    MuseumStruct GetCurrentMuseumStruct() {
        switch (currentTheme) {
            case Theme.Car:
                return listStructs[0];
            case Theme.Wine:
                return listStructs[1];
            case Theme.Viking:
                return listStructs[2];

        }

        return null;
    }




    #region Splash 

    public void Splash() {
        StartCoroutine(Splashing());
    }

    public void SplashClean(float waitTime = 0.08f) {
        StartCoroutine(SplashingClean(waitTime));
    }

    IEnumerator SplashingClean(float waitTime = 0.08f) {
        splash.alpha = 1;
        splash.gameObject.SetActive(true);

        for (int i = 0; i < 20; i++) {
            yield return new WaitForSeconds(0.08f);
            splash.alpha -= 0.05f;
        }

        splash.alpha = 0;
    }

    IEnumerator SplasingBlack() {
        splash.alpha = 0;
        splash.gameObject.SetActive(true);

        for (int i = 0; i < 20; i++) {
            yield return new WaitForSeconds(0.08f);
            splash.alpha += 0.05f;
        }

        splash.alpha = 1;
    }

    IEnumerator SplashingClear() {
        for (int i = 0; i < 20; i++) {
            yield return new WaitForSeconds(0.08f);
            splash.alpha -= 0.05f;
        }

        splash.alpha = 0;
        splash.gameObject.SetActive(false);
    }

    IEnumerator Splashing() {
        splash.alpha = 0;
        splash.gameObject.SetActive(true);

        for(int i=0; i<20; i++) {
            yield return new WaitForSeconds(0.08f);
            splash.alpha += 0.05f;
        }

        splash.alpha = 1;

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 20; i++) {
            yield return new WaitForSeconds(0.08f);
            splash.alpha -= 0.05f;
        }

        splash.alpha = 0;
        splash.gameObject.SetActive(false);
    }

    #endregion
}
