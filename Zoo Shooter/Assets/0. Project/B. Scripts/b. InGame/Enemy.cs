using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;
using DG.Tweening;
using PathologicalGames;

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
    public GameObject head;
    public BoxCollider2D headCol;
    public BoxCollider2D bodyCol;
    
    public bool isLeft = false;
    public EnemyWeapon weapon; // 들고있는 무기 
    

    public Rigidbody2D rigid; // rigidbody2D
    public int HP = 1;

    public SpriteRenderer sp;

    public float jumpPower;
    public float walkSpeed;

    public Animator anim;


    EnemyDataRow data;
    public bool isKilled = false;
    
    public bool isOnGroud = false;
    public bool isHeadShotKill = false;

    Vector3 jumpPos;
    public bool isMoving = false;
    public System.Action JumpCallback;

    void InitEnemy() {
        
    }


    /// <summary>
    /// 에너지 정보를 설정한다.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="pID"></param>
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
        if (type == EnemyType.Normal)
            sp.sprite = Stocks.GetEnemySprite(id);
        else {
            sp.sprite = Stocks.GetBossSprite(data._sprite);
            Debug.Log("Enemy.cs Boss HP :: " + HP);
        }

        rigid.bodyType = RigidbodyType2D.Dynamic;

        EquipWeapon(); // 무기 장착

        // 크기 조정
        this.transform.localScale = new Vector3(data._scale, data._scale, 1);
        this.gameObject.SetActive(true);
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


    /// <summary>
    /// Hit 처리 
    /// </summary>
    /// <param name="d"></param>
    public virtual void HitEnemy(int d, bool isHeadShot) {

        if (isKilled)
            return;


        // Debug.Log("HitEnemy : " + d);

        WeaponManager.isHit = true;
        GameManager.isEnemyHit = true; // 명중했음!
        // 비슷한 변수인데..?

        if (isHeadShot)
            HP -= d*2;
        else
            HP -= d;



        // Debug.Log("HP after hit :: " + HP);
        // 헤드샷 연출
        if (isHeadShot) {
            GameManager.main.Splash(); // 스플래시 효과
            PoolManager.Pools[ConstBox.poolGame].Spawn(ConstBox.prefabHeadshot, new Vector3(0, 5f, 0), Quaternion.identity);
            GameManager.main.ShowGetCoin(); // 코인 획득 

            // 헤드샷에 소리 추가
            AudioAssistant.Shot("Headshot");
        
        }


        // 스코어 및 데미지 처리 
        // 데미지는 보스일때만 표시 
        if (type == EnemyType.Boss) {
            GameViewManager.main.AddScore((GameManager.main.CurrentLevelData._level + 1) * 2, isHeadShot);
            GameManager.main.ShowDamage(GameManager.main.currentWeapon.Damage, isHeadShot);
        }
        else {
            GameViewManager.main.AddScore(GameManager.main.CurrentLevelData._level + 1);
        }


        // 보스의 경우 HP 게이지와 연동되어야 한다. 
        if (HP > 0)
            return;

        KillEnemy(isHeadShot);
    }
    

    public void DisableColliders() {

        headCol.enabled = false;
        bodyCol.enabled = false;

    }

    #region Kill 포즈 여러가지 

    /// <summary>
    /// 헤드샷 킬 with 회전 
    /// </summary>
    void PoseHeadKillRotate() {
        if (isLeft) {
            this.rigid.AddForce(new Vector2(-250, 800));
            this.transform.DORotate(new Vector3(0, 0, 360), 0.4f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart); 
        }
        else {
            this.rigid.AddForce(new Vector2(250, 800));
            this.transform.DORotate(new Vector3(0, 0, -360), 0.4f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        PostKillProcess();

        if (Random.Range(0, 3) < 2)
            WeaponDrop(WeaponDropType.BigHit);
        else
            WeaponDrop(WeaponDropType.HighJump);

    }

    /// <summary>
    /// 일반 킬 with 회전 
    /// </summary>
    void PoseGeneralKillRotate() {
        if (isLeft) {
            this.rigid.AddForce(new Vector2(Random.Range(-150f, -80f), Random.Range(300f, 500f)));
            this.transform.DORotate(new Vector3(0, 0, 360), Random.Range(0.8f, 1.2f), RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }
        else {
            this.rigid.AddForce(new Vector2(Random.Range(80f, 150f), Random.Range(300f, 500f)));
            this.transform.DORotate(new Vector3(0, 0, -360), Random.Range(0.8f, 1.2f), RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        PostKillProcess();
        WeaponDrop(WeaponDropType.Normal);
    }

    void PoseKillDrop() {
        //this.rigid.isKinematic = true; // 일단 물리를 끄고.

        this.transform.DOPunchPosition(new Vector3(0.2f, 0, 0), 0.25f, 15, 1);


        if (isLeft)
            this.transform.DORotate(new Vector3(0, 0, -230), 1f, RotateMode.WorldAxisAdd).SetEase(Ease.InQuad).SetDelay(0.2f);
        else
            this.transform.DORotate(new Vector3(0, 0, 230), 1f, RotateMode.WorldAxisAdd).SetEase(Ease.InQuad).SetDelay(0.2f);


        Invoke("PostKillProcess", 0.2f);

        WeaponDrop(WeaponDropType.NoDrop);

        // StartCoroutine(InvokedWeaponDrop());

    }

    IEnumerator InvokedWeaponDrop() {
        yield return new WaitForSeconds(0.1f);
        WeaponDrop(WeaponDropType.NoDrop);
    }


    #endregion


    /// <summary>
    /// Kill 처리 
    /// </summary>
    public virtual void KillEnemy(bool pHeadShotKill = false) {


        if (isKilled)
            return;

        isHeadShotKill = pHeadShotKill;
        isKilled = true; // 얘는 죽었음!
        this.transform.DOKill();

        int killRand = Random.Range(0, 3);

        // 연출 
        PoolManager.Pools[ConstBox.poolGame].Spawn(Stocks.main.prefabKillEffect, this.transform.position, Quaternion.identity)
            .GetComponent<KillEffect>().SetKillEffect(this.transform.position);


        
        // 헤드샷때는 더 강렬하게 kill
        if(isHeadShotKill) {

            PoseHeadKillRotate();
            
        } // 헤드샷 킬 종료
        else { // 일반 킬 

            if (killRand < 2) {
                PoseGeneralKillRotate();
            }
            else if(killRand == 2) { // 탕 맞고 억하고 쓰러지기 
                PoseKillDrop();
            }
            



        } // 일반 킬 종료 

    }

    /// <summary>
    /// Kill 후 처리. 
    /// </summary>
    void PostKillProcess() {
        head.gameObject.layer = 15;
        this.gameObject.layer = 15; // 레이어 수정해서 충돌 처리 되지 않도록 수정 
        
        StartCoroutine(Destroying());
    }


    IEnumerator Destroying() {
        

        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 발사!
    /// </summary>
    public virtual void Shoot() {
        weapon.Shoot();
    }

    public void InitWeaponRotation() {
        weapon.InitRotation(isLeft);
    }


    /// <summary>
    /// 점프 무브 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="callback"></param>
    public virtual void Move(Vector3 target, System.Action callback) {
        isMoving = false;
        JumpCallback = callback;

        jumpPos = target;
        // this.transform.DOJump(jumpPos, 1.5f, 1, 0.5f).OnComplete(OnCompleteJump);
        this.transform.DOJump(jumpPos, 2f, 1, 0.8f).OnComplete(OnCompleteJump).SetEase(Ease.InQuad);

        if (isLeft)
            this.transform.DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.WorldAxisAdd).SetDelay(0.4f);
        else
            this.transform.DORotate(new Vector3(0, 0, 360), 0.3f, RotateMode.WorldAxisAdd).SetDelay(0.4f);

        AudioAssistant.Shot("BossJump");



        /*
        if (isLeft)
            this.transform.DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.WorldAxisAdd).SetDelay(0.5f);
        else
            this.transform.DORotate(new Vector3(0, 0, 360), 0.3f, RotateMode.WorldAxisAdd).SetDelay(0.5f);

        this.transform.DORotate(new Vector3(0, 0, 360), 0.4f, RotateMode.WorldAxisAdd);
        */
    }

    void OnCompleteJump() {
        isMoving = false;
        JumpCallback();
    }



    /// <summary>
    /// 무기 떨어뜨리기 
    /// </summary>
    void WeaponDrop(WeaponDropType t) {
        weapon.transform.SetParent(null);
        weapon.SetDrop(isLeft, t);
    }

    void ResetWeaponRotation() {
        weapon.transform.rotation = Quaternion.identity;
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
        else
            this.transform.localEulerAngles = Vector3.zero;

    }


    #region Animation 제어 

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
    #endregion




    private void OnCollisionEnter2D(Collision2D collision) {

        switch(collision.collider.tag) {
            case "Stair":
                isOnGroud = true;
                break;

                /*
            case "Bullet":
                HitEnemy(GameManager.main.currentWeapon.Damage, false);
                break;
                */

        }
    }


}
