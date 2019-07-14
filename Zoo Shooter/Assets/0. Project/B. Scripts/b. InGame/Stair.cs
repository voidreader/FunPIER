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

    [SerializeField] float targetPosX = 0;
    [SerializeField] public bool isInPosition = false;

    public SpriteRenderer spriteGround;
    public bool isLeftStair = true; // 좌측 발판인지 체크 
    public Enemy enemy = null;
    public Player player = null;

    [SerializeField] Vector3 bossJumpPos;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }


    /// <summary>
    /// 보스 점프 뛰고, 방향 설정 
    /// </summary>
    /// <param name="e"></param>
    public void SetJumpingBoss(Enemy e) {
        enemy = e;
        enemy.SetSpriteDirection(isLeftStair);
    }

    /// <summary>
    /// 적 세팅! (위치잡기 )
    /// </summary>
    /// <param name="e"></param>
    public void SetEnemey(Enemy e) {
        enemy = e;

        if (enemy == null)
            return;

        StartCoroutine(SettingEnemy());
    }

    IEnumerator SettingEnemy() {
        // 자리 잡기 이전이면 대기한다. 
        while(!isInPosition) {
            yield return null;
        }

        yield return null;

        // 등장시켜놓고 대기시킨다. 
        enemy.transform.position = GetFirstPosition();
        enemy.SetSpriteDirection(isLeftStair);
    }




    /// <summary>
    /// 적, 움직임과 함께 등장
    /// </summary>
    /// <param name="e"></param>
    public void SetReadyEnemy() {

        if (enemy == null)
            return;

        StartCoroutine(EnemyPositionRoutine());
    }

    IEnumerator EnemyPositionRoutine() {
        while (!isInPosition)
            yield return null;

        while (!enemy.isOnGroud)
            yield return null;


        int r = Random.Range(0, 2); // 랜덤 변수
        NormalEnmeyMove move = NormalEnmeyMove.Jump; // 무빙 타입 


        if (r % 2 == 0)
            move = NormalEnmeyMove.Jump;
        else
            move = NormalEnmeyMove.Walk;

        // 보스면 무조건 점프.
        if (enemy.type == EnemyType.Boss)
            move = NormalEnmeyMove.Jump;


        if (move == NormalEnmeyMove.Jump) {
            enemy.transform.DOJump(GetEnemyPosition(), enemy.jumpPower, 1, 0.4f).OnComplete(OnCompleteEnemyAppear);
            enemy.Jump();
            
        }
        else { // 걷기 
            enemy.Walk(); //(애니메이션)
            enemy.transform.DOMove(GetEnemyPosition(), Random.Range(0.5f, 1.2f)).SetEase(Ease.Linear).OnComplete(OnCompleteEnemyAppear);

        }
    }

    void OnCompleteEnemyAppear() {
        enemy.OnGround();

        if (enemy.type == EnemyType.Boss) { // 보스 첫 등장에서는 슬램 파티클 
            Vector3 p = new Vector3(enemy.transform.position.x, this.transform.position.y + 0.65f, 0);
            GameManager.main.ShowParticleSlam(p); // 파티클 처리

            CameraShake.main.ShakeOnce(0.25f, 0.2f); // 카메라 흔들림
        }
    }

    


    /// <summary>
    /// 플레이어 위치 세팅!
    /// </summary>
    /// <param name="p"></param>
    public void SetPlayer(Player p) {
        player = p;

        StartCoroutine(SettingPlayer());

    }

    IEnumerator SettingPlayer() {
        // 자리 잡기 이전이면 대기한다. 
        while (!isInPosition) {
            yield return null;
        }

        player.transform.position = GetPlayerPosition();
        player.SetSpriteDirection(isLeftStair);
    }



    /// <summary>
    /// 발판 위치 잡기 (초기화 로직)
    /// </summary>
    /// <param name="p"></param>
    /// <param name="left"></param>
    public void SetStairPosition(Vector3 p, bool left) {
        // this.transform.localPosition = p;
        isInPosition = false;
        targetPosX = p.x; // 
        isLeftStair = left;

        isInPosition = false;


        if (!isLeftStair) {
            spriteGround.flipX = true;
            this.GetComponent<BoxCollider2D>().offset = new Vector2(0.9822102f, 0.3704366f);
            this.GetComponent<BoxCollider2D>().size = new Vector2(6.428232f, 0.6096972f);
        }
        else {
            spriteGround.flipX = false;
            this.GetComponent<BoxCollider2D>().offset = new Vector2(-1.660136f, 0.3704366f);
            this.GetComponent<BoxCollider2D>().size = new Vector2(7.800061f, 0.6096972f);
        }

        if (isLeftStair)
            this.transform.localPosition = new Vector3(-6, p.y, 0);
        else
            this.transform.localPosition = new Vector3(6, p.y, 0);

        this.transform.DOLocalMoveX(targetPosX, 0.1f).OnComplete(OnCompletePos);
    }

    void OnCompletePos() {
        isInPosition = true;
    }


    void OnSpawned() {

        // 초기화
        enemy = null;
        isLeftStair = true;
        spriteGround.flipX = false;
        isInPosition = false;


        spriteGround.sprite = Stocks.GetRandomStairSprite();
    }

    void OnDespawned() {
        isInPosition = false;
    }

    /// <summary>
    /// 적 준비 포지션 가져오기 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetEnemyPosition() {

        Vector3 pos = this.spriteGround.transform.position;
        

        if (isLeftStair)
            pos = new Vector3(pos.x + Random.Range(1.8f, 1.9f), enemy.transform.position.y, 0);
        else
            pos = new Vector3(pos.x + Random.Range(-1.9f, -1.8f), enemy.transform.position.y, 0);

        return pos;
    }

    /// <summary>
    /// 플레이어 기준 위치 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerPosition() {
        Vector3 pos = this.spriteGround.transform.position;

        if(isLeftStair)
            pos = new Vector3(pos.x + 0.9f, pos.y + 1.2f, 0);
        else
            pos = new Vector3(pos.x - 0.9f, pos.y + 1.2f, 0);

        return pos;
    }

    /// <summary>
    /// 보스가 플레이어 총에 맞고 다음 발판으로 점프
    /// </summary>
    /// <returns></returns>
    public Vector3 GetBossJumpPosition() {
        Vector3 pos = this.spriteGround.transform.position;
        bossJumpPos = pos;

        if (isLeftStair)
            pos = new Vector3(pos.x + Random.Range(1.8f, 1.9f), pos.y + 1.6f, 0);
        else
            pos = new Vector3(pos.x + Random.Range(-1.9f, -1.8f), pos.y + 1.6f, 0);

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
