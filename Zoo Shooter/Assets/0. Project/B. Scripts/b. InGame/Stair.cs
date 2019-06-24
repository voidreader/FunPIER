using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using DG.Tweening;


public class Stair : MonoBehaviour
{
    //최초 생성 x,y좌표 
    static float initY = 1.7f; 
    static float initX = 2.6f; 

    public SpriteRenderer spriteGround;
    public bool isLeftStair = true; // 좌측 발판인지 체크 
    public Enemy enemy = null;
    public Player player = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 적 세팅!
    /// </summary>
    /// <param name="e"></param>
    public void SetEnemey(Enemy e) {
        enemy = e;

        // 등장시켜놓고 대기시킨다. 
        enemy.transform.position = GetFirstPosition();
        enemy.SetSpriteDirection(isLeftStair);
    }


    /// <summary>
    /// 적, 움직임과 함께 등장
    /// </summary>
    /// <param name="e"></param>
    public void SetReadyEnemy() {

        int r = Random.Range(0, 2); // 랜덤 변수
        NormalEnmeyMove move = NormalEnmeyMove.Jump; // 무빙 타입 
        

        if (r % 2 == 0)
            move = NormalEnmeyMove.Jump;
        else
            move = NormalEnmeyMove.Walk;


        if(move == NormalEnmeyMove.Jump) {
            enemy.transform.DOJump(GetEnemyPosition(), enemy.jumpPower, 1, 0.5f).OnComplete(OnCompleteEnemyAppear);
            enemy.Jump(); 
            // enemy.transform.DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.FastBeyond360).SetDelay(0.1f);
        }
        else { // 걷기 
            enemy.Walk(); //(애니메이션)
            enemy.transform.DOMove(GetEnemyPosition(), Random.Range(0.5f, 2f)).SetEase(Ease.Linear).OnComplete(OnCompleteEnemyAppear);

        }

        
    }

    void OnCompleteEnemyAppear() {
        enemy.OnGround();
    }

    


    /// <summary>
    /// 플레이어 위치 세팅!
    /// </summary>
    /// <param name="p"></param>
    public void SetPlayer(Player p) {
        player = p;
        player.transform.position = GetPlayerPosition();
        player.SetSpriteDirection(isLeftStair);
    }

    


    /// <summary>
    /// 발판 위치 잡기 (초기화 로직)
    /// </summary>
    /// <param name="p"></param>
    /// <param name="left"></param>
    public void SetStairPosition(Vector3 p, bool left) {
        this.transform.localPosition = p;
        isLeftStair = left;

        if (!isLeftStair) {
            spriteGround.flipX = true;
            this.GetComponent<BoxCollider2D>().offset = new Vector2(0.9822102f, 0.3920624f);
            this.GetComponent<BoxCollider2D>().size = new Vector2(6.428232f, 0.6529487f);
        }
        else {
            spriteGround.flipX = false;
            this.GetComponent<BoxCollider2D>().offset = new Vector2(-1.660136f, 0.3920624f);
            this.GetComponent<BoxCollider2D>().size = new Vector2(7.800061f, 0.6529487f);
        }
    }


    void OnSpawned() {

        // 초기화
        enemy = null;
        isLeftStair = true;
        spriteGround.flipX = false;
    }

    void OnDespawned() {

    }

    /// <summary>
    /// 적 준비 포지션 가져오기 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetEnemyPosition() {

        Vector3 pos = this.spriteGround.transform.position;
        

        if (isLeftStair)
            pos = new Vector3(pos.x + Random.Range(1.4f, 1.9f), enemy.transform.position.y, 0);
        else
            pos = new Vector3(pos.x + Random.Range(-1.9f, -1.4f), enemy.transform.position.y, 0);

        return pos;
    }

    /// <summary>
    /// 플레이어 기준 위치 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerPosition() {
        Vector3 pos = this.spriteGround.transform.position;

        if(isLeftStair)
            pos = new Vector3(pos.x + 1.8f, pos.y + 0.7f, 0);
        else
            pos = new Vector3(pos.x - 1.8f, pos.y + 0.7f, 0);

        return pos;
    }

    /// <summary>
    /// 적 첫 등장 위치.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFirstPosition() {
        Vector3 pos = this.spriteGround.transform.position;


        if (isLeftStair)
            pos = new Vector3(pos.x - initX, pos.y + initY, 0);
        else
            pos = new Vector3(pos.x + initX, pos.y + initY, 0);

        return pos;
    }
}
