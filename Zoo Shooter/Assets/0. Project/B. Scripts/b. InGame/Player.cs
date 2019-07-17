using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Player : MonoBehaviour
{
    

    public SpriteRenderer sp;
    public static bool isMoving = false;
    Vector3 targetPos;
    public bool isLeft = false;

    

    public WeaponManager weapon; // 무기 
    public BoxCollider2D collider;
    public Rigidbody2D rigid;


    // Start is called before the first frame update
    void Start()
    {
        SetRegularCollider();
    }

    // Update is called once per frame
    void Update() { // Player 입력

        // 이동 중일때 쏘지 못함 
        if (isMoving)
            return;

        // 플레이 중이 아닐 경우 체크 
        if (!GameManager.isPlaying)
            return;


        // Aim 중이지 않을때 대기 
        if (AimController.Wait)
            return;

        if (Input.GetMouseButtonDown(0))
            StartCoroutine(ShootRoutine());

    }

    public void KillPlayer() {
        rigid.bodyType = RigidbodyType2D.Dynamic;
        rigid.AddForce(new Vector2(Random.Range(-100f, 0f), 450));

        this.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        // rigid.AddTorque(360, ForceMode2D.Impulse);
        DropWeapon();

        Destroy(this, 5f);
    }

    void DropWeapon() {
        weapon.transform.SetParent(null);
        weapon.SetDrop(isLeft);
    }

    /// <summary>
    /// 탕탕!
    /// </summary>
    /// <returns></returns>
    IEnumerator ShootRoutine() {
        Debug.Log("Monkey Shoot..!!");

        weapon.Shoot();
        yield return new WaitForSeconds(1f); // 1초 대기

        // 적 사살 체크


    }


    /// <summary>
    /// 다음 계단으로 이동 연출 
    /// </summary>
    /// <param name="target"></param>
    public void MoveNextStair(Vector3 target) {
        targetPos = target;
        isMoving = true;
        StartCoroutine(Moving());
    }

    IEnumerator Moving() {
        // anim.SetBool("isJump", true);
        //this.transform.DOJump(targetPos, 1.8f, 1, 0.8f).OnComplete(OnCompleteMove);
        //this.transform.DORotate(new Vector3(0, 0, 360), 0.6f, RotateMode.WorldAxisAdd);

        AudioAssistant.Shot("Jump");

        this.transform.DOJump(targetPos, 3f, 1, 0.8f).OnComplete(OnCompleteMove).SetEase(Ease.InQuint);

        if(isLeft)
            this.transform.DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.WorldAxisAdd).SetDelay(0.5f);
        else
            this.transform.DORotate(new Vector3(0, 0, 360), 0.3f, RotateMode.WorldAxisAdd).SetDelay(0.5f);

        yield return null;
        //yield return new WaitForSeconds(0.4f);
        // anim.SetBool("isJump", false);
    }

    void OnCompleteMove() {

        isMoving = false;
        AudioAssistant.Shot("Land");
        CameraShake.main.ShakeByPlayerJump();
    }



    public void SetSpriteDirection(bool p) {

        Debug.Log("Player SetSpriteDirection " + p);
        isLeft = p;

        // sp.flipX = isLeft;
        if (isLeft)
            this.transform.localEulerAngles = new Vector3(0, 180, 0);
        else
            this.transform.localEulerAngles = Vector3.zero;


        weapon.Init();
    }

    public void InitWeaponOnly() {
        weapon.Init(true);
    }

    /// <summary>
    /// 총 겨누기 시작 
    /// </summary>
    public void Aim() {
        // weapon.StartAim(isLeft);

        weapon.CurrentAim.ResetAim();
        AimController.Wait = false;
        
    }

    public void Shoot() {
        // weapon.Shoot();
    }

    public void SetLargeCollider() {
        collider.offset = new Vector2(-0.1229745f, 0.06598422f);
        collider.size = new Vector2(0.5971596f, 1.648212f);
        
    }

    public void SetRegularCollider() {
        collider.offset = new Vector2(-0.1229745f, -0.1582869f);
        collider.size = new Vector2(0.5971596f, 1f);
        
    }
    

    void OnSpawned() {
        SetRegularCollider();
        isMoving = false;
    }
}
