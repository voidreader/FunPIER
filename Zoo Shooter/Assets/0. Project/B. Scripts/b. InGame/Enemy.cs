using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;


public enum EnemyType {
    Normal,
    Boss
}

public class Enemy : MonoBehaviour {

    public EnemyType type = EnemyType.Normal; // Enemy 타입
    public string spriteName = string.Empty;
    public string id = string.Empty;
    public BoxCollider2D headCol;
    public BoxCollider2D bodyCol;
    public int HP = 1;

    public SpriteRenderer sp;

    EnemyDataRow data;
    public bool isKilling = false;

    void InitEnemy() {
        isKilling = false;
    }

    public virtual void SetEnemy(EnemyType t, string pID) {
        type = t;
        id = pID;

        data = EnemyData.Instance.GetRow(pID); // 기준정보 불러오기
       
        

        spriteName = data._sprite; // 스프라이트 이름
        

        // Collider 
        bodyCol.offset = new Vector2(data._offsetX, data._offsetY);
        bodyCol.size = new Vector2(data._sizeX, data._sizeY);

        headCol.offset = new Vector2(data._hoffsetX, data._hoffsetY);
        headCol.size = new Vector2(data._hsizeX, data._hsizeY);
        HP = data._hp; // HP

        sp.sprite = GameManager.GetEnemySprite(id);
    }

    public virtual void KillEnemy() {
        GameManager.main.isEnemyDead = true; // 게임매니저에게 죽었다고 전달.
    }

    /// <summary>
    /// 스프라이트 방향 설정 
    /// </summary>
    /// <param name="isLeft"></param>
    public void SetSpriteDirection(bool isLeft) {

        sp.flipX = !isLeft;

    }





}
