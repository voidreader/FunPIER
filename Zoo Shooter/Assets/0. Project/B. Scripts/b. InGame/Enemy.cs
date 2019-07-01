using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;
using DG.Tweening;

/// <summary>
/// 적 등장 스타일 
/// </summary>
public enum NormalEnmeyMove {
    Jump,
    Walk
}

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
    public GameObject head;
    public bool isLeft = false;
    public EnemyWeapon weapon; // 들고있는 무기 
    

    public Rigidbody2D rigid; // rigidbody2D
    public int HP = 1;

    public SpriteRenderer sp;

    public float jumpPower;
    public float walkSpeed;

    public Animator anim;


    EnemyDataRow data;
    public bool isKilling = false;
    public bool isOnGroud = false;


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

        // sp.sprite = GameManager.GetEnemySprite(id);
        sp.sprite = Stocks.GetEnemySprite(id);
        rigid.bodyType = RigidbodyType2D.Dynamic;

        EquipWeapon(); // 무기 장착
    }

    /// <summary>
    /// 무기 장착
    /// </summary>
    void EquipWeapon() {
        weapon = GameObject.Instantiate(Stocks.main.prefabEnemyWeapon, Vector3.zero, Quaternion.identity).GetComponent<EnemyWeapon>();
        
        weapon.transform.SetParent(this.transform);
        weapon.transform.localPosition = new Vector2(data._gunposX, data._gunposY);
        weapon.transform.localScale = Vector3.one;
        weapon.SetEnemyWeapon(data._gun);
        weapon.transform.localEulerAngles = new Vector3(0, 180, 0);


    }


    public virtual void KillEnemy() {

        this.transform.DOKill();


        GameManager.main.isEnemyDead = true; // 게임매니저에게 죽었다고 전달.
        anim.SetBool("isKill", true);

        head.layer = 15;
        this.gameObject.layer = 15; // 레이어 수정해서 충돌 처리 되지 않도록 수정 

        //Invoke("WeaponDrop", 0.25f);
        WeaponDrop();

    }

    /// <summary>
    /// 무기 떨어뜨리기 
    /// </summary>
    void WeaponDrop() {
        weapon.transform.SetParent(null);
        weapon.SetDrop(isLeft);
    }

    public virtual void KillingEffect() {
        
    }

    /// <summary>
    /// 스프라이트 방향 설정 
    /// </summary>
    /// <param name="isLeft"></param>
    public void SetSpriteDirection(bool p) {
        isLeft = p;
        // 좌측 등장이 아닌 경우 flip 처리 
        if (!isLeft)
            this.transform.localEulerAngles = new Vector3(0, 180, 0);

    }


    /// <summary>
    /// 점프 
    /// </summary>
    public void Jump() {
        anim.SetBool("isJump", true);
    }

    public void Walk() {
        anim.SetBool("isWalk", true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnGround() {
        anim.SetBool("isWalk", false);
        anim.SetBool("isJump", false);

        rigid.bodyType = RigidbodyType2D.Dynamic;
    }

    public void KillPlayer(Transform t) {
        weapon.SetTarget(t);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "Stair") {
            isOnGroud = true;
        }

    }


}
