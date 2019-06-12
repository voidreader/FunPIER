using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using DG.Tweening;


public class Stair : MonoBehaviour
{
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

        // 등장
        enemy.transform.position = GetEnemyPosition();
        enemy.SetSpriteDirection(isLeftStair);
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

        if (!isLeftStair)
            spriteGround.flipX = true;
        else
            spriteGround.flipX = false;
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
    /// 적 등장 포지션 가져오기 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetEnemyPosition() {

        Vector3 pos = this.spriteGround.transform.position;
        

        if (isLeftStair)
            pos = new Vector3(pos.x + Random.Range(1.4f, 1.9f), pos.y + 0.5f, 0);
        else
            pos = new Vector3(pos.x + Random.Range(-1.9f, -1.4f), pos.y + 0.5f, 0);

        // x:0.7~1
        // y:0.5

        // return new Vector3(Random.Range(0.7f, 1f), 0.5f, 0);
        return pos;
    }

    public Vector3 GetPlayerPosition() {
        Vector3 pos = this.spriteGround.transform.position;

        if(isLeftStair)
            pos = new Vector3(pos.x + 1.8f, pos.y + 0.5f, 0);
        else
            pos = new Vector3(pos.x - 1.8f, pos.y + 0.5f, 0);

        return pos;
    }
}
