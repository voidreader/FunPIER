﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using Google2u;
using DG.Tweening;
using Doozy.Engine;

public class GameManager : MonoBehaviour {
    public static GameManager main = null;
    public static bool isPlaying = false; // 게임 시작여부 

    public static bool isEnemyHit = false; // Enemy가 플레이어 총알에 맞았는지 체크 
    public static bool isMissed = false; // 플레이어 총알 빗나갔는지? 
    public static bool isWait = false;

    public StageDataRow CurrentLevelData = null; // 현재 스테이지 정보
    public Camera mainCamera; // 메인카메라 

    public bool AutoInit;
    public int SpawnEnemyCount = 0;
    public bool isRevived = false; // 광고보고 부활 여부. (Continue)





    /// <summary>
    /// 플레이어가 다음으로 이동'할' 발판
    /// </summary>
    public Stair currentStair = null; // 적이 등장하는 발판 
    public List<Stair> listStairs;
    public Weapon currentWeapon; // 현재 플레이어가 장착한 무기.

    public float posFirstStairY = -2.5f;
    public int indexLastStair = 0;
    public float topStairY; // 꼭대기 계단 Y 좌표
    public int indexPlayerStair = 0; // 플레이어 캐릭터가 서있는 발판 index 

    Stair stair;
    Enemy enemy;
    public Player player;

    #region InGame Environment Vars

    public bool isEntering = false;
    [SerializeField] SpriteRenderer _whiteBox, _nightBox, _botGround; // 배경 스플래쉬 및 낮밤 바꾸기 용도 
    [SerializeField] Transform _leftTreeGroup, _rightTreeGroup, _moon;
    [SerializeField] List<SpriteRenderer> _listStars;
    [SerializeField] GameObject _nightTile;


    #endregion


    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start() {





    }

    // Update is called once per frame
    void Update() {

        if (!AutoInit)
            return;


        if (Input.GetKeyDown(KeyCode.K)) {
            enemy.KillEnemy();
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            GameViewManager.main.AppearBoss();
        }
    }


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

        PoolManager.Pools[ConstBox.poolGame].DespawnAll();
        Debug.Log("Init InGame Starts.... :: " + CurrentLevelData);
        InitEnvironments();



        CurrentLevelData = StageData.Instance.Rows[PIER.CurrentLevel];

        isRevived = false;
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


        GetNewPlayer();


    } // end of InitGame

    void GetNewPlayer() {
        // 플레이어 생성
        player = GameObject.Instantiate(Stocks.main.prefabPlayer, new Vector3(20, 0, 0), Quaternion.identity).GetComponent<Player>();

        // 플레이어를 첫번째 계단에 위치시킨다. 
        listStairs[indexPlayerStair].SetPlayer(player);
    }

    /// <summary>
    /// 게임 시작 클릭!
    /// </summary>
    public void OnClickPlay() {

        GameViewManager.isContinueWatchAD = false;


        isPlaying = true;
        StartCoroutine(EnteringMission());
        StartCoroutine(PlayRoutine());

        Debug.Log("OnClickPlay is clicked");
    }

    #region 부활 처리
    public void Revive() {

        if (isRevived)
            return;

        isRevived = true;
        // 플레이어 생성
        GetNewPlayer();

        isPlaying = true;
        isMissed = false;
        player.Aim(); // 조준 다시 시작.
    }
    #endregion


    #region Routine 

    /// <summary>
    /// 
    /// </summary>
    void EnterMission() {
        Debug.Log("EnterMission Start");
    }

    void InitEnvironments() {
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

    /// <summary>
    /// 한번의 플레이 세션 
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayRoutine() {


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

            #region 빗나갔을때 Gameover, Enemy Shoot 처리 

            if (isMissed) { // 빗나갔을때 Gameover, Enemy Shoot 처리 
                enemy.Shoot();

                yield return new WaitForSeconds(4.5f);

                

                if (isRevived) {
                    GameOver();
                }
                else {
                    // 한번도 부활하지 않은 경우는 Continue 화면을 호출. 
                    GameEventMessage.SendEvent("ContinueEvent");
                }
                isMissed = false;
            }
            #endregion 


            // 적 죽었을때.. 
            if (isEnemyHit) {

                if (enemy.type == EnemyType.Boss) { // 보스 Hit. 

                    // 죽은 경우
                    if (enemy.isKilled) {
                        // 점프 뛰지 않는다. 
                        // 헬리콥터 등장해야 한다. 

                    }
                    else { // 안죽은 경우
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
                else {
                    // Kill 연출 종료 체크 
                    while (enemy.isKilling) {
                        yield return null;
                    }

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
                    else { // 보스일때는 연출을 기다린다.
                        GameViewManager.main.AppearBoss();

                        while (isWait)
                            yield return null;

                        currentStair.SetReadyEnemy(); // 적 등장 처리 
                    }
                }
            }


            yield return null;
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


        Debug.Log("SetPlayer with Direction");

        listStairs[indexPlayerStair].SetPlayer(player);
        player.Aim();
    }



    float GetDistance(float a, float b) {
        return Mathf.Abs(b - a);
    }

    void MoveMainCamera(float dis) {
        mainCamera.transform.DOMoveY(mainCamera.transform.position.y + dis, 1);
    }


    /// <summary>
    /// 게임 오버 처리 
    /// </summary>
    public void GameOver() {
        // 이벤트 발생시키고 모든 적 제거한다. 
        GameEventMessage.SendEvent("GameOverEvent");
        isPlaying = false;
        GameObject[] es = GameObject.FindGameObjectsWithTag("Body");
        for (int i = 0; i < es.Length; i++) {
            Destroy(es[i]);
        }
    }

    #endregion

    #region Enemy 처리 

    /// <summary>
    /// 새로운 적 생성
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


    /// <summary>
    /// 가장 상단에 새로운 계단 생성하고, Enemy 설정 
    /// indexLastStair 증가 처리
    /// listStairs에 추가 
    /// 가장 첫번째와 두번째 계단은 적을 생성하지 않는다. 
    /// </summary>
    void InsertNewStair() {
        stair = GetNewStair();
        topStairY = stair.transform.localPosition.y; // 높이 처리

        // Enemy 설정 
        if (indexLastStair > 1) // 가장 첫번째와 두번째 계단은 적을 생성하지 않는다. 
            stair.SetEnemey(GetNewEnemy());

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
            posX = Random.Range(2.9f, 4.6f);
            stair.SetStairPosition(new Vector2(posX, posFirstStairY), false);

            return stair;
        }


        // 좌우 체크
        if (indexLastStair % 2 == 0) { // 오른쪽 
            posX = Random.Range(2.9f, 4.6f);
            posY = topStairY + Random.Range(0.4f, 2.2f);
            stair.SetStairPosition(new Vector2(posX, posY), false);
        }
        else { // 왼쪽
            posX = Random.Range(-4.6f, -2.9f);
            posY = topStairY + Random.Range(0.8f, 2.2f);
            stair.SetStairPosition(new Vector2(posX, posY), true);
        }


        return stair;
    }


    #region White Splash

    public void Splash() {
        _whiteBox.gameObject.SetActive(true);
        _whiteBox.color = new Color(0, 0, 0, 0);
        _whiteBox.gameObject.SetActive(true);
        _whiteBox.DOColor(new Color(1, 1, 1, 1), 0.1f).OnComplete(SplashOff);
    }

    void SplashOff() {
        _whiteBox.DOColor(new Color(0, 0, 0, 0), 0.1f).OnComplete(OnCompleteSplash);
    }
    void OnCompleteSplash() {
        _whiteBox.gameObject.SetActive(false);
    }


    #endregion
}
