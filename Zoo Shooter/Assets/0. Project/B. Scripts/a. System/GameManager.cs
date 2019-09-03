using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using Google2u;
using DG.Tweening;
using Doozy.Engine;


public class GameManager : MonoBehaviour {

    public static GameManager main = null;

    public static bool isCameraMoving = false; // 메인카메라 무빙 여부 
    public static bool isGameStarted = true; // 초기화 하고 게임 한번이라도 시작 했는지 
    public static bool isPlaying = false; // 게임 플레이 중

    public static bool isEnemyHit = false; // Enemy가 플레이어 총알에 맞았는지 체크 
    public static bool isMissed = false; // 플레이어 총알 빗나갔는지? 
    public static bool isWait = false;
    readonly float minStairHeightDifference = 1f;

    public StageDataRow CurrentLevelData = null; // 현재 스테이지 정보
    public Camera mainCamera; // 메인카메라
    public Camera bgCamera;
    public ClearMobility helicopter;

    public bool AutoInit;
    public int SpawnEnemyCount = 0;
    public bool isRevived = false; // 광고보고 부활 여부. (Continue)
    public bool isContinueAvailable = false; // Continue 창 오픈 가능 

    public List<BossDamageText> ListBossDamageTexts;
    int BossDamageIndex = 0;
    public List<GetCoin> ListGetCoins;
    int GetCoinIndex = 0;

    #region 인피니트 모드 변수들
    public int InfiniteIndex = 0; // 보스 인덱스 Loop 
    public int InfiniteKillCount = 0; // 킬 카운트 
    public int InfiniteHPx = 1; // 순환 HP 배수 

    #endregion


    /// <summary>
    /// 플레이어가 다음으로 이동'할' 발판
    /// </summary>
    public Stair currentStair = null; // 적이 등장하는 발판 
    public List<Stair> listStairs;
    public Weapon currentWeapon; // 현재 플레이어가 장착한 무기.

    public DancingPlayer Dancer;
    public FakeEquipGun FakeEquipGun;

    public float posFirstStairY = -1.5f;
    public int indexLastStair = 0;
    public float topStairY; // 꼭대기 계단 Y 좌표
    public int indexPlayerStair = 0; // 플레이어 캐릭터가 서있는 발판 index 

    Stair stair;
    public Enemy enemy;
    public Player player;

    #region InGame Environment Vars

    public bool isEntering = false;
    [SerializeField] SpriteRenderer _whiteBox, _nightBox, _botGround; // 배경 스플래쉬 및 낮밤 바꾸기 용도 
    [SerializeField] Transform _leftTreeGroup, _rightTreeGroup, _moon;
    [SerializeField] List<SpriteRenderer> _listStars;
    [SerializeField] GameObject _nightTile;


    #endregion

    #region 파티클

    public ParticleSystem particleSlam;
    public ParticleSystem particleFlashBomb;
    public Transform rotMinus90;

    #endregion

    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start() {

        mainCamera.aspect = 9f / 16f;
        bgCamera.aspect = 9f / 16f;

    }


    void SetCameraAspect() {

    }


    // Update is called once per frame
    void Update() {

        if (!AutoInit)
            return;

        // Kill 테스트용
        if (Input.GetKeyDown(KeyCode.K)) {
            // PlayerBullet.isHitEnemy = true;
            enemy.HitEnemy(100, false);
            // GameViewManager.main.AddScore(GameManager.main.CurrentLevelData._level + 1);
        }



        if (Player.isMoving)
            return;

        if (!isPlaying)
            return;

        if (AimController.Wait)
            return;

        if (Input.GetMouseButtonDown(0))
            player.Shoot();

    }

    /// <summary>
    /// 게임 시작 클릭!
    /// </summary>
    public void OnClickPlay() {

        GameViewManager.isContinueWatchAD = false;
        InfiniteIndex = 0;
        InfiniteKillCount = 0;

        SingularSDK.Event("Game Play");


        isPlaying = true;
        isGameStarted = true;
        StartCoroutine(EnteringMission());

        // 무제한모드와 일반모드 분기.
        if (PIER.main.InfiniteMode) {

            // 인피니트 모드 시작 UI
            GameViewManager.main.ShowInfiniteStart();
            StartCoroutine(InfiniteRoutine());
        }
        else
            StartCoroutine(PlayRoutine());


        Debug.Log("OnClickPlay is clicked");
        Dancer.gameObject.SetActive(false);
        FakeEquipGun.SetHide();
        player.SetHide(false);


    }



    #region 인게임 시스템 초기화 

    /// <summary>
    /// 카메라 초기화
    /// </summary>
    public void InitMainCamera() {
        mainCamera.transform.position = new Vector3(0, 0, -10);
    }

    /// <summary>
    /// 인게임 초기화
    /// </summary>
    public void InitGame() {

        if (!isGameStarted)
            return;

        isGameStarted = false; // 한번 초기화 했으면 플레이할 때까지 또 초기화 시키지 않음. 

        // 무제한 모드 초기화.
        if(PIER.main.InfiniteMode) {
            InitInfiniteMode();
            return;
        }

        // 일반게임 초기화 
        CurrentLevelData = StageData.Instance.Rows[PIER.CurrentLevel];
        Debug.Log("Init Game.. Level ::" + CurrentLevelData._level);

        InitInGameSystem();


    } // end of InitGame

    /// <summary>
    /// 인피니트 모드 초기화 
    /// </summary>
    void InitInfiniteMode() {
        InitInGameSystem();

        // 마지막 LevelData를 넣어준다
        CurrentLevelData = StageData.Instance.Rows[StageData.Instance.Rows.Count - 1];


    }


    /// <summary>
    /// 인게임 시스템 초기화 공통
    /// </summary>
    void InitInGameSystem() {
        PoolManager.Pools[ConstBox.poolGame].DespawnAll();
        Debug.Log("Init InGame Starts.... :: " + CurrentLevelData);

        // 환경 처리 
        InitEnvironments();

        isRevived = false;
        isContinueAvailable = false;
        isMissed = false;
        isEnemyHit = false;
        isWait = false;
        SpawnEnemyCount = 0;

        // 카메라 초기화 및 계단 리스트 초기화 
        InitMainCamera();
        indexPlayerStair = 0; // 플레이어 계단 위치 
        indexLastStair = 0;
        listStairs = new List<Stair>();

        // 먼저 2개의 계단만 생성한다. 
        for (int i = 0; i < 2; i++) {
            InsertNewStair();
        }

        // 게임이 시작되면 플레이어가 한칸 뛰어올라와 우측을 겨냥한다. 
        // 즉, 세번째 계단에서 적이 등장 
        // currentStair는 항상 적이 등장하는 계단. 

        // 플레이어 다음 계단을 무조건 CurrentStair로 설정
        currentStair = listStairs[indexPlayerStair + 1];


        // 주인공 설정 
        GetNewPlayer();
        player.SetHide(true);

        StartCoroutine(PositioningDancer());
    }


    IEnumerator PositioningDancer() {
        while (!listStairs[indexPlayerStair].isInPosition)
            yield return null;

        yield return null;
        Dancer.SetPosition(player.transform.position);
        FakeEquipGun.SetFakeEquipWeapon(player.transform.position, PIER.main.CurrentWeapon);
    }

    public void RefreshFakeEquipGun() {

        if (player == null)
            return;

        FakeEquipGun.SetFakeEquipWeapon(player.transform.position, PIER.main.CurrentWeapon);
    }

    #endregion

    #region GetNewPlayer 신규 플레이어 캐릭터 생성 


    /// <summary>
    /// 신규 플레이어 생성 
    /// </summary>
    /// <param name="isStart"></param>
    void GetNewPlayer(bool isStart = true) {
        // 플레이어 생성
        player = GameObject.Instantiate(Stocks.main.prefabPlayer, new Vector3(20, 0, 0), Quaternion.identity).GetComponent<Player>();

        // 플레이어를 계단에 위치시킨다. 
        if (isStart) // 시작시점의 생성 
            listStairs[indexPlayerStair].SetPlayer(player);
        else { // 부활한 경우 다르게 생성 
            listStairs[indexPlayerStair].SetRevivedPlayer(player);
            enemy.InitWeaponRotation();
        }
    }

    #endregion

    #region 부활 처리 Revive
    public void Revive() {

        if (isRevived)
            return;

        isRevived = true;
        

        // 플레이어 생성
        GetNewPlayer(false);

        StartCoroutine(WaitingRevive());

    }

    IEnumerator WaitingRevive() {
        yield return null;

        while(Player.isMoving) {
            yield return null;
        }

        isPlaying = true;
        isMissed = false;
        player.Aim(); // 조준 다시 시작.

    }

    #endregion


    #region 인게임 시작 연출 Enter Mission

    /// <summary>
    /// 
    /// </summary>
    void EnterMission() {
        Debug.Log("EnterMission Start");
    }

    public void InitEnvironments() {
        _leftTreeGroup.gameObject.SetActive(false);
        _rightTreeGroup.gameObject.SetActive(false);
        _nightBox.gameObject.SetActive(false);
        _botGround.gameObject.SetActive(false);
        _moon.gameObject.SetActive(false);

        _nightTile.SetActive(false);

        for (int i = 0; i < _listStars.Count; i++) {
            _listStars[i].color = new Color(0, 0, 0, 0);
            _listStars[i].DOKill();
            _listStars[i].gameObject.SetActive(false);
        }

    }

    IEnumerator EnteringMission() {

        isEntering = true;

        // 초기화 ㄱ
        _leftTreeGroup.localPosition = new Vector3(-3.5f, 0, 0);
        _rightTreeGroup.localPosition = new Vector3(3.5f, -1.32f, 0);

        // moon -1.13f, 4.15f
        _moon.localPosition = new Vector3(-4.34f, 0.95f, 0);



        _nightBox.color = new Color(1, 1, 1, 0);
        _nightBox.transform.localPosition = new Vector3(0, 20, 0);
        _nightBox.gameObject.SetActive(true);
        _nightBox.DOColor(new Color(1, 1, 1, 1), 0.5f);
        _nightBox.transform.DOLocalMoveY(0, 0.5f);

        yield return new WaitForSeconds(0.5f);

        _botGround.color = new Color(1, 1, 1, 0);
        _botGround.transform.localPosition = new Vector3(0, -12, 0);
        _botGround.gameObject.SetActive(true);
        _botGround.DOColor(new Color(1, 1, 1, 1), 0.5f);
        _botGround.transform.DOLocalMoveY(-4f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        _leftTreeGroup.gameObject.SetActive(true);
        _rightTreeGroup.gameObject.SetActive(true);

        _leftTreeGroup.DOLocalMoveX(-0.4f, 0.5f);
        _rightTreeGroup.DOLocalMoveX(0.4f, 0.5f);

        yield return new WaitForSeconds(0.2f);
        _moon.gameObject.SetActive(true);
        _moon.DOLocalMove(new Vector3(2.56f, 3.79f, 0), 1f);

        // 별. 
        for (int i = 0; i < _listStars.Count; i++) {
            _listStars[i].gameObject.SetActive(true);
            _listStars[i].DOColor(new Color(1, 1, 1, 1), Random.Range(1f, 3.5f)).SetLoops(-1, LoopType.Yoyo);
        }

        _nightTile.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        isEntering = false; // 진입 연출 종료 


    }

    #endregion


    #region 플레이 루틴 


    /// <summary>
    /// 무한모드 플레이 루틴 
    /// </summary>
    /// <returns></returns>
    IEnumerator InfiniteRoutine() {

        Debug.Log("### Infinite Routine Go...!! ###");





        bool isEnemyKillCheck = false; // 미리 변수 갖기 용도 


        // 게임 시작 연출 끝날때까지 기다린다. 
        while (isEntering) {
            yield return null;
        }

        while (isWait)
            yield return null;
            
        // 연출 끝나면 플레이어가 한칸을 뛰어 올라간다. 
        MovePlayer();
        while (Player.isMoving)
            yield return null;

        // 무빙 끝나고 적 캐릭터 등장. 
        currentStair = listStairs[indexLastStair - 1];
        currentStair.SetReadyEnemy();
        enemy = currentStair.enemy;

        // 본격 게임 투틴 시작 지점 
        while(isPlaying) {
           


            while (WeaponManager.isShooting) {
                // 보스가 죽은 경우는 바로 kill처리
                if (enemy.isKilled)
                    WeaponManager.isShooting = false;
                    

                yield return null;
            }

            #region Miss 처리 
            if (isMissed) {

                Debug.Log("!! Missed in Infinite Mode");
                

                while (!enemy.isOnGroud)
                    yield return null;

                enemy.Shoot(); // 몹 발사!

                // 여벌 목숨과 부활 여부 체크 후 처리 
                yield return StartCoroutine(RoutinePlayerLifeAfterMiss());

                isMissed = false;
            }
            #endregion

            // 명중 처리 
            if(isEnemyHit) {

                isEnemyKillCheck = enemy.isKilled;
                InsertNewStair(isEnemyKillCheck); // 새 발판 추가

                while (!stair.isInPosition)
                    yield return null;

                yield return null;

                if (isEnemyKillCheck) { // Kill 된 경우. 
                    MovePlayer(false); // 다음칸으로 선이동
                    isEnemyHit = false; // 다시 enemyDead 초기화
                    currentStair = listStairs[indexLastStair - 1]; // 다음 계단정보 가져오고. 
                    enemy = currentStair.enemy;

                    currentStair.SetReadyEnemy();

                }
                else { // 아직 죽지않고 맞기만 한 경우
                    enemy.Move(stair.GetBossJumpPosition(), SetBossJumpDirection);
                    yield return new WaitForSeconds(0.1f);
                    MovePlayer(false); // 캐릭터 이동 처리 
                    isEnemyHit = false; // 다시 enemyDead 초기화
                    currentStair = listStairs[indexLastStair - 1];
                }


            } // end of isEnemyHit

            yield return null;

        }

    }


    /// <summary>
    /// 다음 인피니트 모드 보스 처리 
    /// </summary>
    public void SetNextInfiniteModeIndex() {

        
            

        InfiniteIndex++;
        InfiniteKillCount++;

        Debug.Log("SetNextInfiniteModeIndex :: " + InfiniteIndex + "/" + BossData.Instance.Rows.Count);

        // 다 돌았으면 다시 0으로..
        if (BossData.Instance.Rows.Count <= InfiniteIndex) {
            Debug.Log("Infinite Cycle Init.... !");
            InfiniteIndex = 0;
            InfiniteHPx *= 2;
        }


        GameViewManager.main.SetInfiniteBossInfo(InfiniteIndex);
        GameViewManager.main.SetInfiniteKillCount(InfiniteKillCount);
    }


    /// <summary>
    /// 한번의 플레이 세션 
    /// </summary>
    /// <returns></returns> 
    IEnumerator PlayRoutine() {

        Debug.Log("### Play Routine Go...!! ###");

        // 게임 시작 연출 끝날때까지 기다린다. 
        while (isEntering) {
            yield return null;
        }

        // 연출 끝나면 플레이어가 한칸을 뛰어 올라간다. 
        MovePlayer();
        while (Player.isMoving)
            yield return null;

        // 무빙 끝나고 적 캐릭터 등장. 
        currentStair = listStairs[indexLastStair - 1];
        currentStair.SetReadyEnemy();
        enemy = currentStair.enemy;

        while (isPlaying) {

            // 쏘고 있을때(특히 머신건) 대기 
            // 보스 일때만 대기하도록 처리 해야 한다. 
            // 일반 몹일때는 휙휙 지나가게.. 

            while (WeaponManager.isShooting) {

                if (enemy.isKilled)
                    WeaponManager.isShooting = false;

                yield return null;
            }


            #region 빗나갔을때 Gameover, Enemy Shoot 처리 

            if (isMissed) { // 빗나갔을때 Gameover, Enemy Shoot 처리 

                player.SetBlue();

                // Enemy가 아직 인포지션 안된 상태라면 대기
                while (!enemy.isOnGroud)
                    yield return null;

                Debug.Log("Missed. Enemy is aiming...");

                enemy.Shoot();

                if (player.ExtraLife) { // 여벌목숨이 있는 경우 

                    yield return new WaitForSeconds(1.5f); // 잠깐 기다렸다가 
                    player.Aim();

                }
                else { // 게임 오버 처리 

                    yield return new WaitForSeconds(1.8f);
                    if (isRevived) {
                        GameOver();
                    }
                    else {
                        // 한번도 부활하지 않은 경우는 Continue 화면을 호출. 
                        // GameEventMessage.SendEvent("ContinueEvent");
                        ContinueEvent();
                    }
                    
                }

                isMissed = false;
            }
            #endregion


            #region 몹을 맞췄을 경우 
            // 적 죽었을때.. 
            if (isEnemyHit) {

                #region 보스 처리 
                if (enemy.type == EnemyType.Boss) { // 보스 Hit. 

                    // 샷건의 경우에는 동시타격이 진행되기 때문에, 다 처리될때까지 대기
                    while(PIER.main.CurrentWeapon.CurrentType == WeaponType.Shotgun && WeaponManager.ListShootingBullets.Count > 0) {
                        yield return null;
                    }


                    // 죽은 경우
                    if (enemy.isKilled) {

                        Debug.Log("PlayRoutine Boss is just killed!!! ");

                        // 점프 뛰지 않는다. 
                        // 헬리콥터 등장해야 한다. 
                        helicopter.CallMobility();

                        yield return new WaitForSeconds(2);
                        // 게임 클리어 처리 

                        GameClear();

                    }
                    else { // 보스 죽지 않은 경우.

                        Debug.Log("PlayRoutine Boss is about to move!!! ");

                        // 보스 한칸 이동 
                        InsertNewStair();

                        while (!stair.isInPosition)
                            yield return null;

                        yield return null;

                        enemy.Move(stair.GetBossJumpPosition(), SetBossJumpDirection);

                        yield return new WaitForSeconds(0.1f);

                        MovePlayer(false); // 캐릭터 이동 처리 

                        isEnemyHit = false; // 다시 enemyDead 초기화
                        currentStair = listStairs[indexLastStair - 1];
                    }
                }
                #endregion
                #region 일반몹 처리 
                else {

                    // 다음 칸으로 이동 
                    MovePlayer();
                    isEnemyHit = false; // 다시 enemyDead 초기화
                    currentStair = listStairs[indexLastStair - 1]; // 다음 계단정보 가져오고. 
                                                                   // 다음 적이 boss인지 체크한다.
                    enemy = currentStair.enemy;

                    if (enemy.type == EnemyType.Normal) {
                        currentStair.SetReadyEnemy(); // 적 등장 처리 
                        SetLevelProgressor();
                    }
                    else { //  다음 몹이 보스일때는 연출을 기다린다.
                        isContinueAvailable = true; // 컨티뉴 사용가능 
                        SetLevelProgressor();
                        GameViewManager.main.AppearBoss();
                        AimController.Wait = true;

                        while (isWait)
                            yield return null;

                        

                        // 보스 UI 연출 종료
                        currentStair.SetReadyEnemy(); // 적 등장 처리 
                    }
                }
                #endregion
            } // end of isEnemyHit
            #endregion

            yield return null;
        }
    }

    /// <summary>
    /// 빗나간 후 플레이어 처리 
    /// </summary>
    IEnumerator RoutinePlayerLifeAfterMiss() {
        if (player.ExtraLife) { // 여벌목숨이 있는 경우 

            yield return new WaitForSeconds(1.5f); // 잠깐 기다렸다가 
            player.Aim();

        }
        else { // 게임 오버 처리 

            yield return new WaitForSeconds(1.8f);
            if (isRevived) {
                GameOver();
            }
            else {
                // 한번도 부활하지 않은 경우는 Continue 화면을 호출. 
                // GameEventMessage.SendEvent("ContinueEvent");
                ContinueEvent();
            }

        }
    }


    void SetBossJumpDirection() {
        stair.SetJumpingBoss(enemy);
    }

    /// <summary>
    /// 플레이어를 다음 계단으로 이동, 카메라 이동 및 신규 계단 생성까지 포함 
    /// </summary>
    void MovePlayer(bool makeStair = true) {
        listStairs[indexPlayerStair].player = null; // 현 계단의 player null 처리 
        player.MoveNextStair(listStairs[indexPlayerStair + 1].GetPlayerPosition());


        // 카메라 이동 처리 
        MoveMainCamera(GetDistance(listStairs[indexPlayerStair].transform.position.y, currentStair.transform.position.y));


        if (makeStair)
            InsertNewStair(); // 단일 새 계단 생성


        indexPlayerStair++; // 플레이어 계단 위치 인덱스 ++ 

        StartCoroutine(WaitingPlayerMoving());
    }

    IEnumerator WaitingPlayerMoving() {

        // 캐릭터 움직일때는. 정지 
        while (Player.isMoving)
            yield return null;


        // Debug.Log("SetPlayer with Direction");

        listStairs[indexPlayerStair].SetPlayer(player);
        player.Aim();
    }



    float GetDistance(float a, float b) {
        return Mathf.Abs(b - a);
    }

    void MoveMainCamera(float dis) {
        StartCoroutine(MovingMainCamera(dis));
    }

    IEnumerator MovingMainCamera(float dis) {

        yield return new WaitForSeconds(0.1f);

        // 카메라 흔들림 효과 중에 이동 대기 
        while(CameraShake.isShaking) {
            yield return null;
        }

        isCameraMoving = true;

        mainCamera.transform.DOMoveY(mainCamera.transform.position.y + dis, 0.3f).OnComplete(OnCompleteCameraMove);
    }

    void OnCompleteCameraMove() {
        isCameraMoving = false;
    }



    /// <summary>
    /// continue 가능 여부 체크해서 GameOver 혹은 Continue 처리 
    /// </summary>
    void ContinueEvent() {
        if (AdsManager.main.IsAvailableRewardAD() && isContinueAvailable)
            GameEventMessage.SendEvent("ContinueEvent");
        else
            GameOver();
    }

    /// <summary>
    /// 게임 오버 처리 
    /// </summary>
    public void GameOver() {

        Debug.Log("!! GameOver !!");
        PIER.main.AddAdCounter(); // 광고 


        // 이벤트 발생시키고 모든 적 제거한다. 
        GameEventMessage.SendEvent("GameOverEvent");
        CleanGameObjects();
    }

    public void GameClear() {
        Debug.Log("!! GameClear !!");
        PIER.main.AddAdCounter(); // 광고 

        // 스코어 이어하기를 위해 저장 
        PIER.main.SaveCurrentScore(PIER.main.CurrentScore);
        PIER.main.SaveBestScore(PIER.main.CurrentScore);
        CleanGameObjects(); // 클리어 오브젝트 

        PIER.main.ClearLevel(); // 클리어 레벨 처리 
        // 다음 UI는 PIER.main.ClearLevel()에서 처리한다 .

    }

    public void CleanGameObjects() {
        isPlaying = false;
        GameObject[] es = GameObject.FindGameObjectsWithTag("Body");
        for (int i = 0; i < es.Length; i++) {
            Destroy(es[i]);
        }

        GameObject[] ps = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < ps.Length; i++) {
            Destroy(ps[i]);
        }


        // 헬리콥터 
        helicopter.OffMobility();

    }

    #endregion

    #region Enemy 처리 

    /// <summary>
    /// 인피니트 모드 몹 생성 
    /// </summary>
    /// <returns></returns>
    Enemy GetInfiniteEnemy() {

        Debug.Log("GetInfiniteEnemy :: " + InfiniteIndex);

        Enemy e = null;
        e = GameObject.Instantiate(Stocks.main.prefabBossEnemy, new Vector3(20, 0, 0), Quaternion.identity).GetComponent<Enemy>();
        e.SetEnemy(EnemyType.Boss, Stocks.GetBossDataRow(InfiniteIndex)._identifier);


        SpawnEnemyCount++;

        if (SpawnEnemyCount > 6)
            isContinueAvailable = true;

        return e;

    }

    /// <summary>
    /// 새로운 적 생성
    /// 일반 모드 
    /// </summary>
    /// <returns></returns>
    Enemy GetNewEnemy() {
        Enemy e = null;

        //  숫자가 같아지면 보스를 생성한다. 
        if (SpawnEnemyCount == CurrentLevelData._enemycount) {
            Debug.Log(">> Spawn Boss!!! <<");
            e = GameObject.Instantiate(Stocks.main.prefabBossEnemy, new Vector3(20, 0, 0), Quaternion.identity).GetComponent<Enemy>();
            e.SetEnemy(EnemyType.Boss, Stocks.GetBossDataRow(CurrentLevelData._level)._identifier);
        }
        else if(SpawnEnemyCount < CurrentLevelData._enemycount) { // Normal Enemy 생성 
            e = GameObject.Instantiate(Stocks.main.prefabNormalEnemy, new Vector3(20, 0, 0), Quaternion.identity).GetComponent<Enemy>();
            e.SetEnemy(EnemyType.Normal, Stocks.GetRandomNormalEnemyID()); // 정보 설정 
        }
        else {
            // spawn 수가 많아지면 더이상 적을 만들지 않음 
            return null;
        }

        SpawnEnemyCount++;

        if (SpawnEnemyCount > 8)
            isContinueAvailable = true;

        return e;
    }

    /// <summary>
    /// 레벨 프로그레서 처리
    /// </summary>
    void SetLevelProgressor() {
        float t = (float)(SpawnEnemyCount-1) / (float)CurrentLevelData._enemycount;

        if (t > 1)
            t = 1;

        GameViewManager.main.SetLevelProgressor(t);
    }

    #endregion

    #region 발판 처리 
    /// <summary>
    /// 가장 상단에 새로운 계단 생성하고, 'Enemy' 생성 및 포지션 위치 
    /// indexLastStair 증가 처리
    /// listStairs에 추가 
    /// 가장 첫번째와 두번째 계단은 적을 생성하지 않는다. 
    /// </summary>
    void InsertNewStair(bool makeEnemy = true) {
        stair = GetNewStair();
        topStairY = stair.transform.localPosition.y; // 높이 처리

        // Enemy 설정 
        // 가장 첫번째와 두번째 계단은 적을 생성하지 않는다. 
        if (indexLastStair > 1) {

            if(makeEnemy) {
                // 몹 생성 처리 
                if (PIER.main.InfiniteMode)
                    stair.SetEnemey(GetInfiniteEnemy());
                else
                    stair.SetEnemey(GetNewEnemy());
            }
        }

        // 인덱스 증가 처리 
        
        listStairs.Add(stair);
        indexLastStair++;
    }

    /// <summary>
    /// 새로운 발판 생성
    /// </summary>
    /// <returns></returns>
    Stair GetNewStair() {
        Stair stair;
        stair = PoolManager.Pools[ConstBox.poolGame].Spawn(ConstBox.prefabStair).GetComponent<Stair>();

        float posX, posY;

        // 계단 위치 지정 
        // 첫번째 계단만 예외.

        if(indexLastStair == 0) {
            // 무조건 오른쪽 
            // stair.SetStairPosition(new Vector2(Random.Range(2.9f, 3.5f), posFirstStairY), false);
            posX = Random.Range(3.4f, 4.5f);
            stair.SetStairPosition(new Vector2(posX, posFirstStairY), false);

            return stair;
        }

        // 좌우 체크
        if (indexLastStair % 2 == 0) { // 오른쪽 
            // posX = Random.Range(3.2f, 4.6f);
            posX = Random.Range(3.5f, 5f);
            posY = topStairY + Random.Range(minStairHeightDifference, 3.2f);
            stair.SetStairPosition(new Vector2(posX, posY), false);
        }
        else { // 왼쪽
            // posX = Random.Range(-4.6f, -3.3f);
            posX = Random.Range(-5f, -3.5f);
            posY = topStairY + Random.Range(minStairHeightDifference, 3.2f);
            stair.SetStairPosition(new Vector2(posX, posY), true);
        }


        return stair;
    }

    #endregion

    #region 연출 처리 

    

    /// <summary>
    ///  슬램(바닥) 파티클 
    /// </summary>
    /// <param name="pos"></param>
    public void ShowParticleSlam(Vector3 pos) {
        PoolManager.Pools[ConstBox.poolGame].Spawn(particleSlam, pos, rotMinus90.rotation);
    }


    /// <summary>
    /// 보스 총알 맞을때 데미지 표시
    /// </summary>
    public void ShowDamage(int damage, bool isDouble = false) {
        // ListBossDamageTexts
        ListBossDamageTexts[BossDamageIndex++].SetDamage(enemy.transform, damage, isDouble);


        if (BossDamageIndex >= ListBossDamageTexts.Count)
            BossDamageIndex = 0;
    }

    /// <summary>
    /// 헤드샷 발생시, 코인획득 
    /// </summary>
    public void ShowGetCoin() {
        // 보스는 헤드샷 맞아도 코인 안줌.
        if (enemy.type == EnemyType.Boss)
            return;

        ListGetCoins[GetCoinIndex++].SetCoin(enemy.transform);
        if (GetCoinIndex >= ListGetCoins.Count)
            GetCoinIndex = 0;
    }

    /// <summary>
    /// 보스 죽였을때 3개정도 떨구게.
    /// </summary>
    public void ShowGetCoinTriple() {
        StartCoroutine(TripleGetCoinRoutine());
    }
    IEnumerator TripleGetCoinRoutine() {

        Transform tr = enemy.transform;

        for (int i = 0; i < 3; i++) {
            ListGetCoins[GetCoinIndex++].SetCoin(tr);
            if (GetCoinIndex >= ListGetCoins.Count)
                GetCoinIndex = 0;

            yield return new WaitForSeconds(0.1f);
        }
    }

    #endregion

    /// <summary>
    /// 건스토어에서 무기를 바뀌었을때 인게임도 바로 반영하기 위함
    /// InitGame에서 이미 Player를 생성시켜 놓은 상태. 
    /// </summary>
    public void RefreshPlayerWeapon() {
        if (!player)
            return;

        Debug.Log("RefreshPlayerWeapon");
        player.InitWeaponOnly();
        FakeEquipGun.SetWeapon(PIER.main.CurrentWeapon);
    }

    #region White Splash

    public void Splash() {
        _whiteBox.gameObject.SetActive(true);
        _whiteBox.color = new Color(0, 0, 0, 0);
        _whiteBox.gameObject.SetActive(true);
        _whiteBox.DOColor(new Color(1, 1, 1, 1), 0.06f).OnComplete(SplashOff);
    }

    void SplashOff() {
        _whiteBox.DOColor(new Color(0, 0, 0, 0), 0.06f).OnComplete(OnCompleteSplash);

    }
    void OnCompleteSplash() {
        _whiteBox.gameObject.SetActive(false);
    }


    #endregion
}
