﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using Google2u;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager main = null;

    public Camera mainCamera; // 메인카메라 

    public bool isPlaying = false; // 게임 시작여부 
    public bool isPause = false; // 일시정지 
    public bool isWaiting = false; // 캐릭터 무빙 등으로 조작 및 로직 방지 
    public bool isEnemyDead = false; // 적 죽었는지..?
    public bool isMissed = false; // 빗나감!

    [SerializeField] List<Sprite> listEnemySprite;



    public Stair currentStair = null; // 적이 등장하는 발판 
    public List<Stair> listStairs;

    public float posFirstStairY = -2.5f;
    public int indexStair = 0;
    public float topStairY; // 꼭대기 계단 Y 좌표
    public int indexPlayerStair = 0; // 플레이어 캐릭터가 서있는 발판 index 

    Stair stair;
    Enemy enemy;
    Player player;


    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)) {
            enemy.KillEnemy();
        }



    }


    /// <summary>
    /// 카메라 초기화
    /// </summary>
    public void InitMainCamera() {
        mainCamera.transform.position = new Vector3(0, 0, -10);
    }

    /// <summary>
    /// 게임 시작~
    /// </summary>
    public void InitGame() {

        InitMainCamera();

        indexStair = 0;

        listStairs = new List<Stair>();

        for(int i=0; i<6; i++) {
            InsertNewStair();
        }

        // 가장 하단 계단을 플레이어 위치로
        // 두번째 계단에 적 설정 
        currentStair = listStairs[indexPlayerStair+1];

        // 적 생성 
        enemy = GetNewEnemy();
        currentStair.SetEnemey(enemy);


        // 플레이어 생성
        player = PoolManager.Pools[ConstBox.poolGame].Spawn(ConstBox.prefabPlayer).GetComponent<Player>();
        listStairs[indexPlayerStair].SetPlayer(player);

        

    } // end of InitGame

    public void OnClickPlay() {
        isPlaying = true;
        StartCoroutine(PlayRoutine());

        Debug.Log("OnClickPlay is clicked");
    }

    #region Routine 

    /// <summary>
    /// 한번의 플레이 세션 
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayRoutine() {

        while(isPlaying) {

            // 대기중.. 
            while (isPause) {
                yield return null;
            }

            while(isWaiting) {
                yield return null;
            }

            if(isEnemyDead) {
                // 무빙 연출 
                yield return StartCoroutine(KillEnemyRoutine());
                isEnemyDead = false; // 다시 enemyDead 초기화
                currentStair.enemy = null;
                currentStair = listStairs[indexPlayerStair + 1];



                // 적 생성 
                enemy = GetNewEnemy();
                currentStair.SetEnemey(enemy);

            }

            
            yield return null;
        }
    }

    IEnumerator KillEnemyRoutine() {
        // '현재' 적이 다 죽었는지 확인 
        while (enemy.isKilling) {
            yield return null;
        }

        // 캐릭터의 이동
        // listStairs[indexPlayerStair + 1].SetPlayer(player);
        listStairs[indexPlayerStair].player = null; // null 

        // 다음 칸으로 이동 
        player.MoveNextStair(listStairs[indexPlayerStair + 1].GetPlayerPosition());

        // 카메라 이동 처리 
        MoveMainCamera(GetDistance(listStairs[indexPlayerStair].transform.position.y, currentStair.transform.position.y));
        InsertNewStair(); // 새 계단 생성

        while(player.isMoving) {
            yield return null;
        }

        indexPlayerStair++;
        listStairs[indexPlayerStair].SetPlayer(player);

        


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

        Enemy enemy = PoolManager.Pools[ConstBox.poolGame].Spawn(ConstBox.prefabEnemy).GetComponent<NormalEnemy>();
        enemy.SetEnemy(EnemyType.Normal, GetRandomNormalEnemyID()); // 정보 설정 
        isEnemyDead = false; // 적 생성되면 적 죽지 않았다고 처리

        return enemy;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Sprite GetEnemySprite(string name) {
        for (int i = 0; i < main.listEnemySprite.Count; i++) {
            if (main.listEnemySprite[i].name == name)
                return main.listEnemySprite[i];
        }

        return null;
    }

    /// <summary>
    /// 랜덤 노멀 에너미 아이디 가져오기
    /// </summary>
    /// <returns></returns>
    public string GetRandomNormalEnemyID() {
        return EnemyData.Instance.Rows[Random.Range(0, EnemyData.Instance.Rows.Count)]._identifier;
    }

    #endregion

    void InsertNewStair() {
        stair = GetNewStair();
        topStairY = stair.transform.localPosition.y; // 높이 처리
        indexStair++;
        listStairs.Add(stair);
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

        if(indexStair == 0) {
            // 무조건 오른쪽 
            stair.SetStairPosition(new Vector2(Random.Range(2.9f, 3.5f), posFirstStairY), false);
            return stair;
        }


        // 좌우 체크
        if (indexStair % 2 == 0) { // 오른쪽 
            posX = Random.Range(2.9f, 3.5f);
            posY = topStairY + Random.Range(0.8f, 2.2f);
            stair.SetStairPosition(new Vector2(posX, posY), false);
        }
        else { // 왼쪽
            posX = Random.Range(-3.5f, -2.9f);
            posY = topStairY + Random.Range(0.8f, 2.2f);
            stair.SetStairPosition(new Vector2(posX, posY), true);
        }


        return stair;
    }


}