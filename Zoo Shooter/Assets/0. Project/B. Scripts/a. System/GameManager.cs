using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using Google2u;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager main = null;

    public StageDataRow CurrentLevelData = null;
    public Camera mainCamera; // 메인카메라 

    public bool AutoInit;
    public bool isPlaying = false; // 게임 시작여부 
    public bool isPause = false; // 일시정지 
    public bool isWaiting = false; // 캐릭터 무빙 등으로 조작 및 로직 방지 
    public bool isEnemyDead = false; // 적 죽었는지..?
    public bool isMissed = false; // 빗나감!

    


    /// <summary>
    /// 플레이어가 다음으로 이동'할' 발판
    /// </summary>
    public Stair currentStair = null; // 적이 등장하는 발판 
    public List<Stair> listStairs;

    public float posFirstStairY = -2.5f;
    public int indexLastStair = 0;
    public float topStairY; // 꼭대기 계단 Y 좌표
    public int indexPlayerStair = 0; // 플레이어 캐릭터가 서있는 발판 index 

    Stair stair;
    Enemy enemy;
    Player player;

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
    void Start()
    {

        if(AutoInit)
            InitGame();
        else {
            // test 환경 
        }

        

    }

    // Update is called once per frame
    void Update()
    {

        if (!AutoInit)
            return;

        
        if(Input.GetKeyDown(KeyCode.K)) {
            enemy.KillEnemy();
        }

        if(Input.GetKeyDown(KeyCode.B)) {
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

        Debug.Log("Init InGame Starts.... :: " + CurrentLevelData);
        CurrentLevelData = StageData.Instance.Rows[PIER.CurrentLevel];



        // 카메라 초기화 및 계단 리스트 초기화 
        InitMainCamera();
        indexPlayerStair = 0; // 플레이어 계단 위치 
        indexLastStair = 0;
        listStairs = new List<Stair>();

        // 먼저 2개의 계단만 생성한다. 
        for(int i=0; i<2; i++) {
            InsertNewStair();
        }

        // 게임이 시작되면 플레이어가 한칸 뛰어올라와 우측을 겨냥한다. 
        // 즉, 세번째 계단에서 적이 등장 
        // currentStair는 항상 적이 등장하는 계단. 

        // 플레이어 다음 계단을 무조건 CurrentStair로 설정
        currentStair = listStairs[indexPlayerStair+1];

        // 플레이어 생성
        player = PoolManager.Pools[ConstBox.poolGame].Spawn(ConstBox.prefabPlayer, new Vector3(20, 0 ,0), Quaternion.identity).GetComponent<Player>();
        

        // 플레이어를 첫번째 계단에 위치시킨다. 
        listStairs[indexPlayerStair].SetPlayer(player);

        

    } // end of InitGame



    /// <summary>
    /// 게임 시작 클릭!
    /// </summary>
    public void OnClickPlay() {
        isPlaying = true;

        StartCoroutine(EnteringMission());
        StartCoroutine(PlayRoutine());

        Debug.Log("OnClickPlay is clicked");
    }

    #region Routine 

    /// <summary>
    /// 
    /// </summary>
    void EnterMission() {
        Debug.Log("EnterMission Start");
    }

    IEnumerator EnteringMission() {

        isEntering = true;

        // 초기화 ㄱ
        _leftTreeGroup.localPosition = new Vector3(-3.5f, 0, 0);
        _rightTreeGroup.localPosition = new Vector3(3.5f, -1.32f, 0);

        // moon -1.13f, 4.15f
        _moon.localPosition = new Vector3(-4.34f, 0.95f, 0);

        for(int i =0; i<_listStars.Count; i++) {
            _listStars[i].color = new Color(0, 0, 0, 0);
            _listStars[i].DOKill();
            _listStars[i].gameObject.SetActive(false);

        }


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

        _leftTreeGroup.DOLocalMoveX(-0.4f, 0.5f);
        _rightTreeGroup.DOLocalMoveX(0.4f, 0.5f);

        yield return new WaitForSeconds(0.2f);
        _moon.DOLocalMove(new Vector3(2.56f, 3.79f, 0), 1f);

        // 별. 
        for(int i=0; i<_listStars.Count;i++) {
            _listStars[i].gameObject.SetActive(true);
            _listStars[i].DOColor(new Color(1, 1, 1, 1), Random.Range(1f, 3.5f)).SetLoops(-1, LoopType.Yoyo);
        }

        _nightTile.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        isEntering = false; // 진입 연출 종료 


    }

    /// <summary>
    /// 한번의 플레이 세션 
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayRoutine() {


        // 게임 시작 연출 끝날때까지 기다린다. 
        while(isEntering) {
            yield return null;
        }

        // 연출 끝나면 플레이어가 한칸을 뛰어 올라간다. 
        MovePlayer();
        while (Player.isMoving)
            yield return null;

        // 무빙 끝나고 적 캐릭터 등장. 
        currentStair = listStairs[indexLastStair-1];
        currentStair.SetReadyEnemy();
        enemy = currentStair.enemy;

        while (isPlaying) {

            // 대기중.. 
            while (isPause) {
                yield return null;
            }

            while(isWaiting) {
                yield return null;
            }

            // 적 죽었을때.. 
            if(isEnemyDead) {

                // Kill 연출 종료 체크 
                while (enemy.isKilling) {
                    yield return null;
                }

                // 다음 칸으로 이동 
                MovePlayer();

                yield return null;

                isEnemyDead = false; // 다시 enemyDead 초기화
                currentStair = listStairs[indexLastStair - 1];
                currentStair.SetReadyEnemy(); // 적 등장 처리 
                enemy = currentStair.enemy;
            }

            
            yield return null;
        }
    }

    /// <summary>
    /// 플레이어를 다음 계단으로 이동, 카메라 이동 및 신규 계단 생성까지 포함 
    /// </summary>
    void MovePlayer() {
        listStairs[indexPlayerStair].player = null; // 현 계단의 player null 처리 
        player.MoveNextStair(listStairs[indexPlayerStair + 1].GetPlayerPosition());
        

        // 카메라 이동 처리 
        MoveMainCamera(GetDistance(listStairs[indexPlayerStair].transform.position.y, currentStair.transform.position.y));



        InsertNewStair(); // 단일 새 계단 생성
        indexPlayerStair++; // 플레이어 계단 위치 인덱스 ++ 

        StartCoroutine(WaitingPlayerMoving());
    }

    IEnumerator WaitingPlayerMoving() {

        // 캐릭터 움직일때는. 정지 
        while (Player.isMoving)
            yield return null;

        listStairs[indexPlayerStair].SetPlayer(player);
        player.Aim();
    }



    float GetDistance(float a, float b) {
        return Mathf.Abs(b - a);
    }

    void MoveMainCamera(float dis) {
        mainCamera.transform.DOMoveY(mainCamera.transform.position.y + dis, 1);
    }

    #endregion


    #region Enemy 처리 

    /// <summary>
    /// 새로운 적 생성
    /// </summary>
    /// <returns></returns>
    Enemy GetNewEnemy() {

        /*
        Enemy enemy = PoolManager.Pools[ConstBox.poolGame].Spawn(ConstBox.prefabEnemy).GetComponent<NormalEnemy>();
        enemy.SetEnemy(EnemyType.Normal, GetRandomNormalEnemyID()); // 정보 설정 
        isEnemyDead = false; // 적 생성되면 적 죽지 않았다고 처리
        */

        Enemy e = GameObject.Instantiate(Stocks.main.prefabNormalEnemy, new Vector3(20, 0, 0), Quaternion.identity).GetComponent<Enemy>();
        e.SetEnemy(EnemyType.Normal, Stocks.GetRandomNormalEnemyID()); // 정보 설정 

        return e;
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
            stair.SetStairPosition(new Vector2(Random.Range(2.9f, 3.5f), posFirstStairY), false);
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


}
