using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using System.Linq;
using DG.Tweening;
using SimpleJSON;

public enum Moving {
    Up,
    Down,
    Right,
    Left
}

public enum Theme {
    Parameter,
    Prototype,
    Car,
    Wine,
    Viking
}

public class InGame : MonoBehaviour {

    public static InGame main = null;

    public static bool isPlaying = false;

    public static bool isChipMoving = false; // 타일 무빙중! 
    public static bool isPostProcessMove = false; // 타일 무빙 후 초기화중!


    public bool isMoved = false; // 입력시 칩이 한번이라도 이동했는지 체크 


    public static Theme currentTheme = Theme.Car;
    public int currentThemeStep = 0; // 현재 테마의 현 스텝 
    public int currentThemeMaxStep = 0;
    public int currentHighScore = 0; 

    public GameObject tileDaddy;
    public List<TileCtrl> ListTiles; // 단순 순서맞추기용 
    public List<TileCtrl> SpawnSpotTiles = new List<TileCtrl>(); // 스폰 스팟선정요
    public TileCtrl[,] tiles = new TileCtrl[4, 4]; // 실제 2차원 배열 (좌측 하단이 0,0 우측 상단이 3,3)

    public AskingEndGame askingEndGame;
    public GameObject topUIs, bottomUIs; // 아래 위 UI 그룹
    public GameOverCtrl gameOverControl; // 게임 오버 컨트롤러 
    public bool isAskedNoMove = false; // 움직일 수 있는 블록 없음 경고!

    public UIButton _btnBack, _btnUpgrade, _btnCleaner; // 아이템 3종
    public UIButton _btnItemCacncel; // 아이템 취소 버튼 
    public UILabel _lblBottomMessage; // 아래 메세지 


    // 상단 UI의 오브젝트
    public UILabel lblThemeName;
    public UIProgressBar themeProgressBar; 
    public UILabel lblCurrentProgress; // 현재 진척도
    public UILabel lblTotalProgress; // 진척도 전체
    public UILabel lblHighScore; // 최고 스코어 
    public UILabel lblCurrentScore; // 현재 스코어

    public JSONNode NodeTileHistory = JSON.Parse("{}"); // 히스토리 
    public int SnapSeq = 0;

    public SpriteRenderer _moonBG, _moon;
    

    Vector3 punchScale = new Vector3(1.05f, 1.05f, 1.05f);
    readonly int WIDTH = 4;
    readonly int HEIGHT = 4;
    public float DragGap = 0.5f;
    public float MovingTime = 0.15f;

    public int currentScore = 0;
    int tempCollectScore = 0;

    public static float _spriteFadeTime = 0.6f;

    #region 조작 후 무빙용 변수들 
    int targetH = -1;
    int targetW = -1;
    bool exitCheck = false;

    // 사운드 생성 용도
    bool moveShotCheck = false;
    public bool mergeShotCheck = false;

    #endregion


    #region Awake Start Update

    private void Awake() {
        main = this;
        ShowInGameUIs(false);

        _moon.gameObject.SetActive(false);
        _moonBG.gameObject.SetActive(false);
    }

    
    void Start()  {

        // Debug.Log(string.Format(PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT48), "Back", "3"));
        

        StarsRoutineStart(); // 별 루틴(배경 용도)
    }

    void Update() {

        /* 이 부분은 테스트를 위한 로직 */

        if(Input.GetKeyDown(KeyCode.O)) {
            // GameOver();
            AskWhenNoMove();
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            StartZoom(tiles[2, 3].transform);
        }

        if(Input.GetKeyDown(KeyCode.R)) {
            AppearRedMoon();
        }

        if (Input.GetKeyDown(KeyCode.A))
            AlignBlocks();
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public void QuitInGame() {
        PageManager.main.OpenDoubleButtonMessage(Message.InGameQuit, CloseSessionForce, delegate { });
    }

    void CloseSessionForce() {
        isPlaying = false;
        CloseSession();
    }

    /// <summary>
    /// 
    /// </summary>
    public void RestartSession() {

        InGame.main.ShowInGameUIs(true);

        PoolManager.Pools[ConstBox.poolIngame].DespawnAll();
        StartSession(currentTheme);
    }

    void InitCamera() {
        _mainCamera.transform.position = OriginCameraPos;
        _mainCamera.orthographicSize = 13;
    }

    /// <summary>
    /// 게임 세션 스타트
    /// </summary>
    /// <param name="playTheme"></param>
    public void StartSession(Theme playTheme, bool resume = false) {

        InitCamera();

        currentTheme = playTheme;

        // 테마별 진척도 불러오기
        currentThemeStep = PierSystem.main.GetCurrentProgress(currentTheme);
        currentThemeMaxStep = PierSystem.main.GetMaxProgress(currentTheme);

        
        SnapSeq = 0;
        currentScore = 0;
        isChipMoving = false;
        isPostProcessMove = false;

        SetProgressUI(); // 진척도 
        SetHighScore(); // 하이스코어
        InitTiles(); // 타일 초기화
        

        if (resume) {
            SetSnapShotTile();
        }
        else { 

            
            NodeTileHistory = null;
            SetScores(); // 스코어 (타일 초기화 뒤에 나올것)
            SaveInGameSnapShot(); // 스냅샷 저장 

            // 신규 게임일때. 마지막 진척도에 따른 광고 보고 오브젝트 얻기 제안 
            if(currentThemeStep != currentThemeMaxStep && currentThemeStep > 7) {
                RequestStartWatch();
            }

        }

        // 무브먼트 History (Snapshot)
        // isPlaying = true;
        isAskedNoMove = false;

        StartCoroutine(DelayingStartSession());

        AudioAssistant.main.PlayMusic("InBGM", true);


        WaitRedMoon(); // 레드문 대기 

    }

    IEnumerator DelayingStartSession() {
        yield return new WaitForSeconds(0.5f);
        isPlaying = true;
    }

    /// <summary>
    /// 진척도 
    /// </summary>
    void SetProgressUI() {

        lblThemeName.text = PierSystem.GetThemeName(currentTheme);

        lblTotalProgress.text = "/" + currentThemeMaxStep.ToString();
        lblCurrentProgress.text = currentThemeStep.ToString();

        themeProgressBar.value = (float)currentThemeStep / (float)currentThemeMaxStep;
    }

    /// <summary>
    /// 스코어 계산
    /// </summary>
    void SetScores() {
        tempCollectScore = 0;

        // 전체 칩을 통해 수집
        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {
                if (tiles[h, w].chip != null) {
                    tempCollectScore += tiles[h, w].chip.GetScore();
                }
            }
        }

        currentScore = tempCollectScore;
        lblCurrentScore.text = string.Format("{0:n0}", currentScore);
        lblCurrentScore.transform.localScale = Vector3.one;
        lblCurrentScore.transform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => OnCompletePunch(lblCurrentScore.transform));
    }

    void OnCompletePunch(Transform tr) {
        tr.localScale = Vector3.one;
    }

    /// <summary>
    /// 하이 스코어 설정
    /// </summary>
    void SetHighScore() {
        switch(currentTheme) {
            case Theme.Car:
                lblHighScore.text = "★ " + string.Format("{0:n0}", PierSystem.main.carHighScore);
                currentHighScore = PierSystem.main.carHighScore;
                break;
            case Theme.Wine:
                lblHighScore.text = "★ " + string.Format("{0:n0}", PierSystem.main.wineHighScore);
                currentHighScore = PierSystem.main.wineHighScore;

                break;
            case Theme.Viking:
                lblHighScore.text = "★ " + string.Format("{0:n0}", PierSystem.main.vikingHighScore);
                currentHighScore = PierSystem.main.vikingHighScore;
                break;
        }
    }

    /// <summary>
    ///  세션 닫기
    /// </summary>
    public void CloseSession() {

        StartCoroutine(ClosingSession());


    }

    IEnumerator ClosingSession() {
        Debug.Log("CloseSession");

        isPlaying = false;

        PoolManager.Pools[ConstBox.poolIngame].DespawnAll();
        ES2.Delete(ConstBox.KeySavedTileHistory); // 스냅샷 삭제 

        tileDaddy.SetActive(false);
        InGame.main.ShowInGameUIs(false);

        _moon.gameObject.SetActive(false);
        _moonBG.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.05f);

        LobbyManager.main.LobbyFrom2048();
        InitCamera();

        
    }


    /// <summary>
    /// 인게임 UI 제어 
    /// </summary>
    /// <param name="flag"></param>
    public void ShowInGameUIs(bool flag) {
        topUIs.SetActive(flag);
        bottomUIs.SetActive(flag);

        if(flag) {

            ItemCounter.RefreshItems();

            _lblBottomMessage.gameObject.SetActive(false);
            _btnItemCacncel.gameObject.SetActive(false);

            _btnBack.gameObject.SetActive(true);
            _btnUpgrade.gameObject.SetActive(true);
            _btnCleaner.gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    void InitTiles() {

        tileDaddy.SetActive(true);

        // 2차원 배열 설정
        int i = 0;
        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {
                tiles[h, w] = ListTiles[i++];
                tiles[h, w].Init(); // 초기화 
            }
        } // end of 배열 설정


        SpawnChip(2);
        // SpawnTest();

    }

    void SpawnTest() {
        tiles[3, 3].chip = PoolManager.Pools[ConstBox.poolIngame].Spawn(GetSpawnChipPrefabName(16)).GetComponent<Chip>();
        tiles[3, 3].ChipInit();

        tiles[2, 3].chip = PoolManager.Pools[ConstBox.poolIngame].Spawn(GetSpawnChipPrefabName(2)).GetComponent<Chip>();
        tiles[2, 3].ChipInit();

        tiles[1, 3].chip = PoolManager.Pools[ConstBox.poolIngame].Spawn(GetSpawnChipPrefabName(4)).GetComponent<Chip>();
        tiles[1, 3].ChipInit();
        tiles[1, 3].ChipInit();

        tiles[0, 3].chip = PoolManager.Pools[ConstBox.poolIngame].Spawn(GetSpawnChipPrefabName(4)).GetComponent<Chip>();
        tiles[0, 3].ChipInit();
    }


    /// <summary>
    /// 칩 스폰 
    /// </summary>
    /// <param name="count">생성 개수</param>
    void SpawnChip(int count) {

        Transform tr;
        int index;

        for(int i=0; i< count; i++) {

            int r = Random.Range(0, 2);

            if(r % 2 == 0) { // 2짜리 
                tr = PoolManager.Pools[ConstBox.poolIngame].Spawn(GetSpawnChipPrefabName(2));
            }
            else { // 4짜리
                tr = PoolManager.Pools[ConstBox.poolIngame].Spawn(GetSpawnChipPrefabName(2));
            }


            // tr을 랜덤한 타일 위치로 이동 
            SpawnSpotTiles.Clear();
            SpawnSpotTiles = ListTiles.Where(x => x.chip == null).ToList<TileCtrl>();
            
            index = Random.Range(0, SpawnSpotTiles.Count);
            SpawnSpotTiles[index].chip = tr.GetComponent<Chip>(); // 넣어주기 
            SpawnSpotTiles[index].ChipInit(); // 칩 Init 
            // SpawnSpotTiles.RemoveAt(index);
        }
    }


    #region 광고 시청 제안 (정렬, 시작) 

    /// <summary>
    /// 시작할때 광고보고 N단계 블록 얻을래?
    /// </summary>
    public void RequestStartWatch() {
        PageManager.main.OpenDoubleButtonMessage(Message.GameStartWatch, ShowStartWatch, delegate { });
    }

    /// <summary>
    /// 광고 오픈 
    /// </summary>
    void ShowStartWatch () {
        Debug.Log("ShowStartWatch");
        AdsControl.main.ShowWatchAd(GetStartDisplay);
    }

    void GetStartDisplay() {

        Debug.Log("GetStartDisplay");


        Transform tr;
        int index;
        int id = PierSystem.GetIDByStep(currentThemeStep);

        tr = PoolManager.Pools[ConstBox.poolIngame].Spawn(GetSpawnChipPrefabName(id));

        


        // tr을 랜덤한 타일 위치로 이동 
        SpawnSpotTiles.Clear();
        SpawnSpotTiles = ListTiles.Where(x => x.chip == null).ToList<TileCtrl>();

        index = Random.Range(0, SpawnSpotTiles.Count);
        SpawnSpotTiles[index].chip = tr.GetComponent<Chip>(); // 넣어주기 
        SpawnSpotTiles[index].ChipInit(); // 칩 Init 

    }




    #endregion


    #region GetSpawnChipPrefabName

    /// <summary>
    /// 스폰 대상 칩 이름 가져오기
    /// </summary>
    /// <param name="t">테마</param>
    /// <param name="id">숫자</param>
    /// <returns></returns>
    public static string GetSpawnChipPrefabName(int id) {

        switch(currentTheme) {

            case Theme.Car:
                switch (id) {
                    case 2:
                        return "Car_2";
                    case 4:
                        return "Car_4";
                    case 8:
                        return "Car_8";
                    case 16:
                        return "Car_16";
                    case 32:
                        return "Car_32";
                    case 64:
                        return "Car_64";
                    case 128:
                        return "Car_128";
                    case 256:
                        return "Car_256";
                    case 512:
                        return "Car_512";
                }

                break;

            case Theme.Wine:
                switch (id) {
                    case 2:
                        return "Wine_2";
                    case 4:
                        return "Wine_4";
                    case 8:
                        return "Wine_8";
                    case 16:
                        return "Wine_16";
                    case 32:
                        return "Wine_32";
                    case 64:
                        return "Wine_64";
                    case 128:
                        return "Wine_128";
                    case 256:
                        return "Wine_256";
                    case 512:
                        return "Wine_512";
                    case 1024:
                        return "Wine_1024";
                    case 2048:
                        return "Wine_2048";
                }

                break;

            case Theme.Viking:
                switch (id) {
                    case 2:
                        return "Viking_2";
                    case 4:
                        return "Viking_4";
                    case 8:
                        return "Viking_8";
                    case 16:
                        return "Viking_16";
                    case 32:
                        return "Viking_32";
                    case 64:
                        return "Viking_64";
                    case 128:
                        return "Viking_128";
                    case 256:
                        return "Viking_256";
                    case 512:
                        return "Viking_512";
                    case 1024:
                        return "Viking_1024";
                    case 2048:
                        return "Viking_2048";
                    case 4096:
                        return "Viking_4096";
                }

                break;


            case Theme.Prototype:
                switch(id) {
                    case 2:
                        return "Temp_2";
                    case 4:
                        return "Temp_4";
                    case 8:
                        return "Temp_8";
                    case 16:
                        return "Temp_16";
                    case 32:
                        return "Temp_32";
                    case 64:
                        return "Temp_64";
                    case 128:
                        return "Temp_128";
                    case 256:
                        return "Temp_256";
                    case 512:
                        return "Temp_512";
                }

                break;
        }

        return string.Empty;
    }

    #endregion


    #region 진행 (MergeCheck)

    /// <summary>
    /// Merge 후 step 체크 
    /// </summary>
    /// <param name="id"></param>
    public void MergeCheck(int id) {

        int step = GetStepByID(id);

        if(!mergeShotCheck) {
            mergeShotCheck = true;
            AudioAssistant.LowShot("Merge");
        }


        // Debug.Log("New Merge rasied! :: " + id);

        // 현 단계보다 낮거나 같은 경우는 아무것도 하지 않음 
        if(step > currentThemeStep) {

            // step 저장 
            currentThemeStep = step;

            // 저장 호출 
            PierSystem.main.SaveCurrentThemeStep(currentTheme, currentThemeStep);
            PierSystem.main.SaveProfile(); // 저장 

            SetProgressUI(); // 진척도 설정
        }

        // 업적 체크
        if (step == 10)
            PlatformManager.main.UnlockAchievements(MIMAchievement.Make10);
        else if (step == 11)
            PlatformManager.main.UnlockAchievements(MIMAchievement.Make11);
        else if (step == 11)
            PlatformManager.main.UnlockAchievements(MIMAchievement.Make12);

        // 마지막 오브젝트 오픈!
        if (step == currentThemeMaxStep) {
            Debug.Log(">> GameClear !!");
            // 게임 클리어 연출 시작
            StartCoroutine(ClearRoutine());
        }
    }

    IEnumerator ClearRoutine() {
        yield return null;

        AudioAssistant.main.PlayMusic("Final");

        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {

                if(tiles[h,w].chip != null && GetStepByID(tiles[h,w].chip.id) == currentThemeMaxStep) {
                    StartZoom(tiles[h, w].transform);
                    // 회전 처리 
                    // tiles[h, w].RotateChip();
                }

            }
        }

        yield return new WaitForSeconds(3);
        GameClear();
    }


    /// <summary>
    /// id로 몇 단계인지 알아오기
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    int GetStepByID(int id) {
        switch(id) {
            case 2:
                return 1;
            case 4:
                return 2;
            case 8:
                return 3;
            case 16:
                return 4;
            case 32:
                return 5;
            case 64:
                return 6;
            case 128:
                return 7;
            case 256:
                return 8;
            case 512:
                return 9;
            case 1024:
                return 10;
            case 2048:
                return 11;
            case 4096:
                return 12;
            case 8192:
                return 13;
            case 16384:
                return 14;

        }

        return 0;
            
    }

    #endregion

    #region 무빙 

    /// <summary>
    /// 마지막 스냅샷 정보 삭제 
    /// </summary>
    /// <returns></returns>
    public bool RemoveLastSnapShot() {
        if (NodeTileHistory["history"].Count == 0)
            return false;

        NodeTileHistory["history"].Remove(NodeTileHistory["history"][NodeTileHistory["history"].Count - 1]);

        return true;
    }



    /// <summary>
    /// 매 무빙, 액션마다 타일의 전체 상태를 저장합니다. 
    /// </summary>
    public void SaveInGameSnapShot(bool NoCount = false) {
        // 액션 seq 
        // 타일 배열 

        if(NoCount) {
            ES2.Save<string>(NodeTileHistory.ToString(), ConstBox.KeySavedTileHistory);
            ES2.Save<Theme>(currentTheme, ConstBox.KeySavedPlayTheme);
            return;
        }

        // 각 타일의 칩만 저장하면 되겠다. 
        JSONNode node = PierSystem.GetEmptyNode();

        if(NodeTileHistory == null)
            NodeTileHistory = PierSystem.GetEmptyNode();

        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {
                // NodeTileHistory["history"][-1]
                if (tiles[h, w].chip == null)
                    node["tiles"][-1]["chip"].AsInt = -1; // 타일 없으
                else
                    node["tiles"][-1]["chip"].AsInt = tiles[h, w].chip.id; // id 입력 
            }
        }

        // 각 노드마다 식별자 붙여놓는다. 
        node["snapseq"].AsInt = SnapSeq++;

        // 타일정보를 모아모아서 저장
        NodeTileHistory["history"][-1] = node;

        // 10개 넘어가면 맨 앞에꺼 하나씩 제거 
        if(NodeTileHistory["history"].Count > 10) 
            NodeTileHistory["history"].Remove(NodeTileHistory["history"][0]);

        ES2.Save<string>(NodeTileHistory.ToString(), ConstBox.KeySavedTileHistory);
        ES2.Save<Theme>(currentTheme, ConstBox.KeySavedPlayTheme);

        // Debug.Log("Saved tile history :: " + NodeTileHistory.ToString());
        

    }

    /// <summary>
    /// 스냅샷정보 로드 
    /// </summary>
    /// <returns></returns>
    public bool LoadInGameSnapShot() {
        if(!ES2.Exists(ConstBox.KeySavedTileHistory)) {
            return false; // 스냅샷 정보 없음 
        }

        Debug.Log(">> Exists InGame Snapshot <<");
        NodeTileHistory = JSON.Parse(ES2.Load<string>(ConstBox.KeySavedTileHistory)); // 로드 
        int count = NodeTileHistory["history"].Count;
        SnapSeq = NodeTileHistory["history"][count - 1]["snapseq"].AsInt;

        currentTheme = ES2.Load<Theme>(ConstBox.KeySavedPlayTheme);

        return true; // 스냅샷 정보 있음 
    }

    /// <summary>
    /// 스냅샷 정보를 타일로 저장 
    /// </summary>
    void SetSnapShotTile() {

        Debug.Log(">> SetSnapShotTile :: " + NodeTileHistory["history"].Count); 

        // 전체 디스폰 처리 
        PoolManager.Pools[ConstBox.poolIngame].DespawnAll();
        int count = NodeTileHistory["history"].Count;
        JSONNode lastnode = NodeTileHistory["history"][count - 1];

        Debug.Log("Check LastNode :: " + lastnode.ToString());

        
        int nodeIndex = 0;
        int chipid = 0;
        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {
                tiles[h, w].chip = null;
                
                if(lastnode["tiles"][nodeIndex]["chip"].AsInt > 0) {
                    chipid = lastnode["tiles"][nodeIndex]["chip"].AsInt;
                    tiles[h, w].chip = PoolManager.Pools[ConstBox.poolIngame].Spawn(GetSpawnChipPrefabName(chipid)).GetComponent<Chip>();
                    tiles[h, w].ChipInit();
                }

                nodeIndex++;

            } // end of w for
        }

        SetScores();
    }


    /// <summary>
    /// 타일 무빙 
    /// </summary>
    public void MoveTiles(Moving move) {

        if (isChipMoving) {
            Debug.Log(">> Can't move! isChipMoving is true!");
            return;
        }

        if (isPostProcessMove) {
            Debug.Log(">> Can't move! isPostProcessMove is true!");
            return;
        }

        // 사운드 체크용 두개의 boolean.
        moveShotCheck = false;
        mergeShotCheck = false;

        switch(move) {
            case Moving.Up:
                OnMoveUp();
                break;

            case Moving.Down:
                OnMoveDown();
                break;

            case Moving.Left:
                OnMoveLeft();
                break;

            case Moving.Right:
                OnMoveRight();
                break;
        }

        StartCoroutine(MovingWait());
    }

    /// <summary>
    /// 칩 이동 대기 후 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator MovingWait() {

        isPostProcessMove = true;

        while (isChipMoving)
            yield return null;


        if (isMoved) {
            isMoved = false;
            isAskedNoMove = false; // 움직임이 있었으면 false로 초기화
            MergeTiles();
            SpawnChip(1);

            SaveInGameSnapShot(); // 스냅샷 저장 

            PierSystem.main.MoveCount++;

            // 업적 체크 
            if (PierSystem.main.MoveCount == 100)
                PlatformManager.main.UnlockAchievements(MIMAchievement.Move100);
            else if (PierSystem.main.MoveCount == 500)
                PlatformManager.main.UnlockAchievements(MIMAchievement.Move500);
            else if (PierSystem.main.MoveCount == 1000)
                PlatformManager.main.UnlockAchievements(MIMAchievement.Move1000);
        }

        yield return null;

        InitAfterMove();

        // 스코어 처리 추가
        SetScores();

        yield return null;

        if (!CheckPossibleMove()) {

            AskWhenNoMove(); // 

            // GameOver(); // 게임오버 전에 이전단계 필요 
        }

        isPostProcessMove = false;
    }



    /// <summary>
    /// 무빙 후 초기화
    /// </summary>
    void InitAfterMove() {
        for(int h=0; h<HEIGHT;h++) {
            for(int w=0; w<WIDTH; w++) {
                tiles[h, w].movingChip = null;
                tiles[h, w].mergeChip = null;
            }
        }
    }

    /// <summary>
    /// 움직일 수 없는 블록이 없을때. 
    /// </summary>
    public void AskWhenNoMove() {

        if (isAskedNoMove) {
            GameOver();
            return;
        }

        isPlaying = false;
        isAskedNoMove = true;
        // PageManager.main.OpenMessage(Message.NoMoreMove, OnCloseAsking);

        // 게임오버 물어보기
        //AskingEndGame.main.OpenAsking();
        askingEndGame.OpenAsking();
    }

    public void OnCloseAsking() {
        StartCoroutine(DelayedAsked());
        
    }

    

    IEnumerator DelayedAsked() {
        yield return new WaitForSeconds(0.5f);
        isPlaying = true;
    }

    /// <summary>
    /// 게임 오버 처리 
    /// </summary>
    public void GameOver() {
        Debug.Log("Call Game End");
        isPlaying = false;

        gameOverControl.OpenGameOver();

        topUIs.gameObject.SetActive(false);
        bottomUIs.gameObject.SetActive(false);
    }

    /// <summary>
    /// 모든 오브젝트 오픈, 게임 클리어 
    /// </summary>
    public void GameClear() {
        Debug.Log("GameClear");
        isPlaying = false;

        gameOverControl.OpenGameClear();

        topUIs.gameObject.SetActive(false);
        bottomUIs.gameObject.SetActive(false);
    }

    /// <summary>
    /// 타일 머지
    /// </summary>
    public void MergeTiles() {

        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {
                tiles[h, w].MergeChip();
            }
        }

    }

    /// <summary>
    /// 이동 가능 여부 체크 
    /// </summary>
    /// <returns></returns>
    bool CheckPossibleMove() {

        int chipid = 0;
        int checkH = -1;
        int checkW = -1;

        for(int h=0; h<HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {

                // 하나라도 null 이 있으면 이동 가능 
                if (tiles[h, w].chip == null)
                    return true;
                
            }
        }

        // 모든 타일이 꽉 차있는 경우 인접한 타일을 모두 체크 
        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {

                chipid = tiles[h, w].chip.id;
                checkW = -1;
                checkH = -1;

                checkH = h + 1;
                if(checkH < HEIGHT && tiles[checkH, w].chip.id == chipid) {
                    return true;
                }

                checkH = h - 1;
                if (checkH >= 0 && tiles[checkH, w].chip.id == chipid)
                    return true;

                checkW = w + 1;
                if (checkW < WIDTH && tiles[h, checkW].chip.id == chipid)
                    return true;

                checkW = w - 1;
                if (checkW >= 0 && tiles[h, checkW].chip.id == chipid)
                    return true;
            }
        }

        // 이도 저도 아니면 false.

        return false;
    }


    // [h, w]

    /// <summary>
    /// ↖↖↖ 이동
    /// </summary>
    public void OnMoveUp() {

        Debug.Log("OnMoveUp");

        isMoved = false;

        targetH = -1;
        exitCheck = false;

        // 첫라인은 h = 3
        for (int h = 3; h >= 0; h--) {
            for(int w = 0; w < WIDTH; w++) {

                if(h == 3) continue; // 가장 상단 라인은 움직이지 않는다.

                if (tiles[h, w].chip == null)
                    continue;


                exitCheck = false; // 아래 for문 탈출용 변수 
                targetH = -1;

                // 상단라인까지 차례대로 훑는다.
                for (int i= h+1 ; i<HEIGHT; i++) { // 한단계 윗 라인부터 훑어야된다. 

                    if (exitCheck)
                        continue;

                    if (tiles[i, w].chip == null) { // 널인지 체크 
                        targetH = i; // 이동 목적지 설정 
                    }
                    else { // null 이 아닌 경우 (이미 칩이 있다.)

                        if(tiles[i,w].chip.id == tiles[h,w].chip.id && tiles[i, w].mergeChip == null && tiles[i,w].movingChip == null) { // Merge 대상 
                            targetH = i; // 이동 목적지 설정 
                            tiles[i, w].mergeChip = tiles[h, w].chip; // Merge 대상 설정
                            exitCheck = true;
                            // continue; // 체크 종료
                        }
                        else { // Merge 대상 아님 
                            exitCheck = true;
                            // continue; // 체크 종료  (이동 목적지는 한칸 전 위치)
                        }

                    }
                } // end of i for

                // 이동 체크 
                if (targetH >= 0) {

                    MoveCurrentChip(tiles[h, w], targetH, w, true);
                    #region
                    /*
                    isMoved = true;
                    isChipMoving = true;
                    targetChip = tiles[h, w].chip;
                    targetChip.transform.DOMove(tiles[targetH, w].GetChipPosition(), MovingTime); // 실제 이동

                    tiles[h, w].chip = null; // 원래 자리의 Chip null 처리.

                    // 이동 목적지의 타일에 chip이 없고, 머지도 아니면 현재 칩을 설정.
                    if (tiles[targetH, w].chip == null && tiles[targetH, w].mergeChip == null) {
                        tiles[targetH, w].chip = targetChip;
                        // tiles[targetH, w].movingChip = targetChip;
                    }
                    */
                    #endregion
                }

            } // end of h for
        } // end of w for

    }

    /// <summary>
    /// ↘↘↘ Down 이동
    /// </summary>
    public void OnMoveDown() {

        Debug.Log("OnMoveDown");

        isMoved = false;

        targetH = -1;
        exitCheck = false;

        // 첫라인은 h = 0
        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {

                if (h == 0) continue; // 첫 라인은 움직일 수 없음 

                if (tiles[h, w].chip == null)
                    continue;

                exitCheck = false; // 아래 for문 탈출용 변수 
                targetH = -1;

                for (int i = h - 1; i >= 0; i--) {  // 아래로 훑고 내려간다

                    if (exitCheck)
                        continue;

                    if (tiles[i, w].chip == null) { // 널인지 체크 
                        targetH = i; // 이동 목적지 설정 
                        
                    }
                    else { // null 이 아닌 경우 

                        if (tiles[i, w].chip.id == tiles[h, w].chip.id && tiles[i,w].mergeChip == null && tiles[i, w].movingChip == null) { // Merge 대상 
                            targetH = i; // 이동 목적지 설정 
                            tiles[i, w].mergeChip = tiles[h, w].chip; // Merge 대상 설정

                            exitCheck = true;
                        }
                        else { // Merge 대상 아님 
                            exitCheck = true;
                        }

                    }
                } // end of i for

                // 이동 체크 
                if (targetH >= 0) {

                    MoveCurrentChip(tiles[h, w], targetH, w, true);

                    #region
                    /*
                    isMoved = true;
                    isChipMoving = true;
                    targetChip = tiles[h, w].chip;
                    targetChip.transform.DOMove(tiles[targetH, w].GetChipPosition(), MovingTime); // 실제 이동

                    tiles[h, w].chip = null; // 원래 자리의 Chip null 처리.

                    // 이동 목적지의 타일에 chip이 없고, 머지도 아니면 현재 칩을 설정.
                    if (tiles[targetH, w].chip == null && tiles[targetH, w].mergeChip == null) {
                        tiles[targetH, w].chip = targetChip;
                        // tiles[targetH, w].movingChip = targetChip;
                    }
                    */
                    #endregion
                }

            } // end of h for
        } // end of w for

    }

    /// <summary>
    /// 오른쪽 이동!
    /// </summary>
    public void OnMoveRight() {
        Debug.Log("OnMoveRight");

        isMoved = false;
        targetW = -1;
        exitCheck = false;

        for(int h=0; h<HEIGHT; h++) { 
            // 첫라인은 w = 3 (가장 오른쪽부터 순차이동)
            for(int w = 3; w >=0; w--) {

                if (w == 3) continue; // 맨 끝 라인은 움직이지 않음. 

                if (tiles[h, w].chip == null)
                    continue;

                exitCheck = false; // 아래 for문 탈출용 변수 
                targetW = -1;
                
                for (int i = w + 1; i < WIDTH; i++) {

                    if (exitCheck)
                        continue;

                    if(tiles[h,i].chip == null) {
                        targetW = i; // 이동목적지 설정 
                    }
                    else { // 다음 칸에 칩이 있는 경우

                        if(tiles[h,i].chip.id == tiles[h,w].chip.id && tiles[h,i].mergeChip == null) {
                            targetW = i; // 이동 목적지 설정
                            tiles[h, i].mergeChip = tiles[h, w].chip; // Merge 대상
                            exitCheck = true;
                        }
                        else { // Merge 대상 아님
                            exitCheck = true;
                        }
                    }
                } // end of i for

                if(targetW >= 0) {
                    MoveCurrentChip(tiles[h, w], targetW, h, false);
                }

            } // end of w for

        } // end of for. 끗.
         
    }

    /// <summary>
    /// 왼쪽 이동!
    /// </summary>
    public void OnMoveLeft() {
        Debug.Log("OnMoveLeft");

        isMoved = false;
        targetW = -1;
        exitCheck = false;

        for (int h = 0; h < HEIGHT; h++) {
            // 첫라인은 w = 0 (가장 왼쪽부터 )
            for (int w = 0; w < WIDTH; w++) {

                if (w == 0) continue; // 맨 끝 라인은 움직이지 않음. 

                if (tiles[h, w].chip == null)
                    continue;

                exitCheck = false; // 아래 for문 탈출용 변수 
                targetW = -1;

                // 오른쪽으로 차례로 훑는다
                for (int i = w -1; i >= 0; i--) {

                    if (exitCheck)
                        continue;

                    if (tiles[h, i].chip == null) {
                        targetW = i; // 이동목적지 설정 
                    }
                    else { // 다음 칸에 칩이 있는 경우

                        if (tiles[h, i].chip.id == tiles[h, w].chip.id && tiles[h, i].mergeChip == null) {
                            targetW = i; // 이동 목적지 설정
                            tiles[h, i].mergeChip = tiles[h, w].chip; // Merge 대상
                            exitCheck = true;
                        }
                        else { // Merge 대상 아님
                            exitCheck = true;
                        }
                    }
                } // end of i for

                if (targetW >= 0) {
                    MoveCurrentChip(tiles[h, w], targetW, h, false);
                }

            } // end of w for

        } // end of for. 끗.

    }


    /// <summary>
    /// 입력 후 대상 칩 무빙 
    /// </summary>
    /// <param name="currentTile">이동을 시작하는 타일</param>
    /// <param name="destPos">목적지(UpDown시는 row, LeftRight시는 col)</param>
    /// <param name="currentPos">UpDown에서는 col 위치, LeftRight에서는 row</param>
    /// <param name="UpDown">이동 방식, 상하인지 좌우인지</param>
    void MoveCurrentChip(TileCtrl currentTile, int destPos, int currentPos, bool UpDown) {

        isMoved = true;
        isChipMoving = true;
        Chip c = currentTile.chip; // null로 바꿀꺼라서 미리 복사 
        isChipMoving = true;

        StartCoroutine(LockMoveRoutine());

        if (UpDown) { // 상하 이동 
            c.transform.DOMove(tiles[destPos, currentPos].GetChipPosition(c.id), MovingTime); // 이동

            currentTile.chip = null; // 이동했으니까 null.

            // 이동 목적지의 타일에 chip이 없고, 머지도 아니면 현재 칩을 설정.
            if (tiles[destPos, currentPos].chip == null & tiles[destPos, currentPos].mergeChip == null) {
                tiles[destPos, currentPos].chip = c;
            }

        }
        else { // 좌우 이동
            c.transform.DOMove(tiles[currentPos, destPos].GetChipPosition(c.id), MovingTime); // 이동

            currentTile.chip = null; // 이동했으니까 null.

            if(tiles[currentPos, destPos].chip == null && tiles[currentPos, destPos].mergeChip == null) {
                tiles[currentPos, destPos].chip = c;
            }
        }

        
        if(!moveShotCheck) {
            moveShotCheck = true;
            AudioAssistant.LowShot("Move");
        }
        
    }

    IEnumerator LockMoveRoutine() {
        
        yield return new WaitForSeconds(MovingTime);
        yield return null;
        isChipMoving = false;
    }

    #endregion

    #region Item 기능 

    /// <summary>
    /// 블록 정렬기능.
    /// </summary>
    public void AlignBlocks() {
        List<int> ids = new List<int>();
        for(int i=0; i<ListTiles.Count;i++) {
            if (ListTiles[i].chip == null)
                continue;

            ids.Add(ListTiles[i].chip.id); // id 입력 
        }

        ids.Sort(); // 정렬.
        PoolManager.Pools[ConstBox.poolIngame].DespawnAll(); // 칩 모두 해제 
        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {
                tiles[h, w].Init(); // 초기화 
            }
        } 


        int sortIndex = ids.Count-1;
        Transform tr;

        // 정렬한 것들 배열 
        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {

                if (sortIndex < 0)
                    return;

                tr = PoolManager.Pools[ConstBox.poolIngame].Spawn(GetSpawnChipPrefabName(ids[sortIndex]));
                tiles[h, w].chip = tr.GetComponent<Chip>();
                tiles[h, w].ChipInit();

                sortIndex--;
            }
        }
    }


    void CallDelayedRecoverState() {
        StartCoroutine(DelayedInputItemCheckDisable());
    }

    /// <summary>
    /// 클린 아이템 클릭
    /// </summary>
    public void OnClickClean() {

        if (_lblBottomMessage.gameObject.activeSelf)
            return;

        
        if (PierSystem.main.itemCleaner < 1 && PierSystem.main.AdminPlay) {
            PierSystem.main.itemCleaner += 5; // 임시로 5그냥 증가 처리 
            PierSystem.main.SaveProfile();
            ItemCounter.RefreshItems();
            return;
        }

        bool hasLv3Higher = false;
        
        // 최소 3단계 이상의 칩이 있어야한다.
        for(int i=0; i<ListTiles.Count;i++) {
            if (ListTiles[i].chip == null)
                continue;

            if(ListTiles[i].chip.id >= 8) {
                hasLv3Higher = true;
                break;
            }
        }

        // 최소 3단계 이상의 오브젝트가 하나 이상 필요
        if(!hasLv3Higher) {
            PageManager.main.OpenMessage(Message.NeedLv3, delegate { });
            return;
        }

        
        // 메세지창 호출 
        PageManager.main.OpenDoubleButtonMessage(Message.AskCleanItemUse, ProcessClean, CallDelayedRecoverState);
    }

    /// <summary>
    /// 클린처리
    /// </summary>
    void ProcessClean() {

        PierSystem.main.itemCleaner--;
        PierSystem.main.SaveProfile();
        ItemCounter.RefreshItems();

        for (int h = 0; h < HEIGHT; h++) {
            for (int w = 0; w < WIDTH; w++) {

                // 2,4 만 대상 
                if(tiles[h,w].chip != null && (tiles[h,w].chip.id == 2 || tiles[h, w].chip.id == 4)) {
                    PoolManager.Pools[ConstBox.poolIngame].Despawn(tiles[h, w].chip.transform);
                    tiles[h, w].chip = null;
                }
            }
        }

        SetScores(); // 스코어 재계산 
        SaveInGameSnapShot(); // 스냅샷 저장 

        // 입력제한 풀기 
        StartCoroutine(DelayedInputItemCheckDisable());
    }

    /// <summary>
    /// 1회 되돌리기
    /// </summary>
    public void OnClickBack() {

        if (_lblBottomMessage.gameObject.activeSelf)
            return;


        /*
        if (PierSystem.main.itemBack < 1) {
            PierSystem.main.itemBack += 5; // 임시로 5그냥 증가 처리 
            PierSystem.main.SaveProfile();
            ItemCounter.RefreshItems();
            return;
        }
        */

        if (NodeTileHistory == null ||  NodeTileHistory["history"].Count <= 1)
            return;


        
        // 메세지창 호출 
        PageManager.main.OpenDoubleButtonMessage(Message.AskBackItemUse, ProcessBack, CallDelayedRecoverState);
    }

    void ProcessBack() {
        RemoveLastSnapShot();
        SaveInGameSnapShot(true); // 노카운트로 저장 
        SetSnapShotTile();

        PierSystem.main.itemBack--;
        PierSystem.main.SaveProfile();
        ItemCounter.RefreshItems();

        CallDelayedRecoverState();
    }


    /// <summary>
    /// 업그레이드 아이템 클릭 
    /// </summary>
    public void OnClickUpgrade() {
        if(PierSystem.main.itemUpgrade < 1) {
            PierSystem.main.itemUpgrade += 5; // 임시로 5그냥 증가 처리 
            PierSystem.main.SaveProfile();
            ItemCounter.RefreshItems();
            return;
        }


        ShowBottomItemMessage("upgrader");
        InputManager.itemInput = true; // true 처리
    }

    /// <summary>
    /// 하단에 특정 메세지 보여주기 
    /// </summary>
    /// <param name="item"></param>
    void ShowBottomItemMessage(string item) {

        if(item == "upgrader") {
            _lblBottomMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT29);
        }

        _btnBack.gameObject.SetActive(false);
        _btnCleaner.gameObject.SetActive(false);
        _btnUpgrade.gameObject.SetActive(false);


        _lblBottomMessage.transform.localPosition = new Vector3(0, 120, 0);
        _lblBottomMessage.gameObject.SetActive(true);
        _lblBottomMessage.transform.DOLocalMoveY(100, 0.4f);

        _btnItemCacncel.transform.localPosition = new Vector3(0, -20, 0);
        _btnItemCacncel.gameObject.SetActive(true);
        _btnItemCacncel.transform.DOLocalMoveY(0, 0.4f);
    }

    /// <summary>
    /// '업그레이드' 아이템 사용 완료
    /// </summary>
    public void OnCompleteItemUpgrade() {

        InputManager.itemInput = false;

        CancelBottomMessage();

        PierSystem.main.itemUpgrade--;
        PierSystem.main.SaveProfile();
        ItemCounter.RefreshItems();
    }



    /// <summary>
    /// 하단 메세지 취소 
    /// </summary>
    public void CancelBottomMessage() {
        _btnBack.gameObject.SetActive(true);
        _btnCleaner.gameObject.SetActive(true);
        _btnUpgrade.gameObject.SetActive(true);

        _lblBottomMessage.gameObject.SetActive(false);
        _btnItemCacncel.gameObject.SetActive(false);

        StartCoroutine(DelayedInputItemCheckDisable());
        
    }

    // 바로 해제하면 움직임을 인식해버려서..
    IEnumerator DelayedInputItemCheckDisable() {
        yield return new WaitForSeconds(0.5f);
        InputManager.itemInput = false;
        InGame.isPlaying = true;

    }

    #endregion

    #region Camera Moving

    public Transform _zoomTarget;
    public Camera _mainCamera;
    float Zoom = 2.5f;
    float ZoomSize = 0.01f;

    [SerializeField] Vector3 OriginCameraPos = new Vector3(10.9f, 8.87f, 10.25f);
    



    IEnumerator Zooming() {

        
        int frameCount = 0;
        Zoom = 2f;
        ZoomSize = 0.01f;

        while(frameCount < 100) {
            Vector3 dist_position = (_mainCamera.transform.position) - (_zoomTarget.position);

            //정규화

            dist_position = Vector3.Normalize(dist_position);

            //마우스 휠이 입력될 경우 Zoom만큼 카메라의 거리를 증감

            _mainCamera.transform.position -= (dist_position * Time.deltaTime * Zoom);     // 마우스 휠로 화면확대 축소
            _mainCamera.orthographicSize -= ZoomSize;

            Zoom *= 1.011f;
            ZoomSize *= 1.011f;

            frameCount++;
            yield return null;
        }

    }

    public void StartZoom(Transform target) {
        _zoomTarget = target;
        StartCoroutine(Zooming());
    }

    #endregion

    #region Red Moon  & BG Effect

    bool _isMoonGoing = false;

    void StarsRoutineStart() {
        StartCoroutine(ShootingStarRoutine());
        StartCoroutine(TwinkleStarRoutine());
    }


    IEnumerator ShootingStarRoutine() {

        yield return new WaitForSeconds(Random.Range(6f, 11f));

        while (true) {

            PoolManager.Pools[ConstBox.poolSpriteEffect].Spawn(ConstBox.SpriteEffectShootingStar, Vector3.zero, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
            PoolManager.Pools[ConstBox.poolSpriteEffect].Spawn(ConstBox.SpriteEffectShootingStar, Vector3.zero, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(1.5f, 3f));
            PoolManager.Pools[ConstBox.poolSpriteEffect].Spawn(ConstBox.SpriteEffectShootingStar, Vector3.zero, Quaternion.identity);


            yield return new WaitForSeconds(Random.Range(6f, 15f));

        }
    }

    IEnumerator TwinkleStarRoutine() {

        yield return new WaitForSeconds(Random.Range(3f, 7f));

        while (true) {

            PoolManager.Pools[ConstBox.poolSpriteEffect].Spawn(ConstBox.SpriteEffectTwinkleStar, Vector3.zero, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(1.5f, 3f));
            PoolManager.Pools[ConstBox.poolSpriteEffect].Spawn(ConstBox.SpriteEffectTwinkleStar, Vector3.zero, Quaternion.identity);


            yield return new WaitForSeconds(Random.Range(10f, 20f));

        }
    }

    void WaitRedMoon() {
        StopCoroutine(RedMoonWaiting());
        StartCoroutine(RedMoonWaiting());
    }

    IEnumerator RedMoonWaiting() {
        yield return null;

        while(true) {
            // 일정 시간 대기 
            yield return new WaitForSeconds(Random.Range(180f, 240f));

            while (!isPlaying) {
                yield return null;
            }


            if (_moon.gameObject.activeSelf)
                continue;

            AppearRedMoon();
        }
    }


    /// <summary>
    /// 레드문 등장 
    /// </summary>
    public void AppearRedMoon() {
        FadeInUnitySprite(_moonBG, 2);
        FadeInUnitySprite(_moon, 2.5f);

        StartCoroutine(RedMoonRoutine());
    }

    /// <summary>
    /// 붉은달이 떴다가 다시 돌아가는 시간. 
    /// </summary>
    /// <returns></returns>
    IEnumerator RedMoonRoutine() {
        yield return new WaitForSeconds(30);

        DisappearRedMoon();
    }


    /// <summary>
    /// 레드문 터치 
    /// </summary>
    public void OnClickRedMoon() {
        //PageManager.main.OpenMessage()
        int itemRange = Random.Range(0, 100);
        int valueRange = Random.Range(0, 100);

        if (itemRange < 50)
            PierSystem.currentRedMoonItem = "back";
        else if (itemRange >= 50 && itemRange < 75)
            PierSystem.currentRedMoonItem = "upgrader";
        else
            PierSystem.currentRedMoonItem = "cleaner";


        if (valueRange < 50)
            PierSystem.currentRedMoonValue = 1;
        else if (valueRange >= 50 && valueRange < 85)
            PierSystem.currentRedMoonValue = 2;
        else
            PierSystem.currentRedMoonValue = 3;

        // 아이템 종류, 개수 계산 끝

        // 유저에게 물어봅니다. 
        PageManager.main.OpenDoubleButtonMessage(Message.RedMoonOpen, ShowRedMoonWatch, DisappearRedMoon);
        


    }

    /// <summary>
    /// 광고 오픈 
    /// </summary>
    void ShowRedMoonWatch() {
        Debug.Log("ShowRedMoonWatch");
        AdsControl.main.ShowWatchAd(GetRedMoonItem);
    }

    /// <summary>
    /// 실제 아이템 획득
    /// </summary>
    void GetRedMoonItem() {

        Debug.Log(">> GetRedMoonItem :: " + PierSystem.currentRedMoonItem);
        StartCoroutine(OpenItemGetMessageRoutine());

    }

    /// <summary>
    /// 자꾸 튕겨서..
    /// </summary>
    IEnumerator OpenItemGetMessageRoutine() {
        yield return new WaitForSeconds(0.1f);

        if (PierSystem.currentRedMoonItem == "back") {
            PierSystem.main.itemBack += PierSystem.currentRedMoonValue;
            PageManager.main.OpenMessage(Message.ItemGet, delegate { }, PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT6), PierSystem.currentRedMoonValue.ToString());
        }
        else if (PierSystem.currentRedMoonItem == "upgrader") {
            PierSystem.main.itemUpgrade += PierSystem.currentRedMoonValue;
            PageManager.main.OpenMessage(Message.ItemGet, delegate { }, PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT7), PierSystem.currentRedMoonValue.ToString());
        }
        else if (PierSystem.currentRedMoonItem == "cleaner") {
            PierSystem.main.itemCleaner += PierSystem.currentRedMoonValue;
            PageManager.main.OpenMessage(Message.ItemGet, delegate { }, PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT8), PierSystem.currentRedMoonValue.ToString());
        }

        ItemCounter.RefreshItems();
        PierSystem.main.SaveProfile();

        Debug.Log(">> GetRedMoonItem :: " + PierSystem.currentRedMoonValue);
    }

    /// <summary>
    /// 레드문 사라지기 
    /// </summary>
    public void DisappearRedMoon() {

        if (!_moon.gameObject.activeSelf)
            return;

        if (_isMoonGoing)
            return;

        Debug.Log("DisappearRedMoon");
        _isMoonGoing = true;

        FadeOutUnitySprite(_moonBG, 1);
        FadeOutUnitySprite(_moon, 1.5f);
        Invoke("RecoverMoonGoing", 1.5f);
    }

    void RecoverMoonGoing() {
        _isMoonGoing = false;
    }

    #endregion

    #region Unity Sprite Renderer 

    public static void FadeInUnitySprite(SpriteRenderer sp, float time = 0) {
        sp.gameObject.SetActive(true);
        sp.color = ConstBox.colorTransparent;

        if (time == 0)
            sp.DOColor(ConstBox.colorOrigin, _spriteFadeTime);
        else
            sp.DOColor(ConstBox.colorOrigin, time);
    }

    public static void FadeOutUnitySprite(SpriteRenderer sp, float time = 0) {
        sp.gameObject.SetActive(true);
        sp.color = ConstBox.colorOrigin;

        if (time == 0)
            sp.DOColor(ConstBox.colorTransparent, _spriteFadeTime).OnComplete(() => main.OnCompleteWithInactive(sp.gameObject));
        else
            sp.DOColor(ConstBox.colorTransparent, time).OnComplete(() => main.OnCompleteWithInactive(sp.gameObject));
    }


    public void OnCompleteWithInactive(GameObject obj) {
        obj.SetActive(false);
    }
    #endregion

}
