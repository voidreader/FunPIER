using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Player : MonoBehaviour
{
    

    public SpriteRenderer sp;
    public Animator anim;

    public bool isRevived = false;
    public Sprite SpriteSpecialist; // 스페셜리스트 
    public Sprite SpriteNormal; // 노멀 

    public GameObject Helmet; // 헬멧(광고용)
    public bool ExtraLife = false;
    

    public static bool isMoving = false;
    Vector3 targetPos;
    public bool isLeft = false;

    

    public WeaponManager weapon; // 무기 
    public BoxCollider2D collider;
    public Rigidbody2D rigid;

    public BreakingArmor _breakingArmor;

    // Start is called before the first frame update
    void Start()
    {
        SetRegularCollider();
        Helmet.SetActive(false);
        SetSpecialist();
    }

    /// <summary>
    ///  감추기 처리 
    /// </summary>
    /// <param name="flag"></param>
    public void SetHide(bool flag) {
        this.gameObject.SetActive(!flag);

        SetSpecialist();
    }


    /// <summary>
    /// 부활했을때는 일반상태로 살아난다. 
    /// </summary>
    void SetNormalAgentForce() {

        Debug.Log(">> SetNormalAgentForce <<");

        // sp.sprite = SpriteNormal;
        anim.Play("GeneralAgent");
        anim.SetBool("isSpecial", false);
        ExtraLife = false;
    }

    /// <summary>
    /// 스페셜 리스트 처리 
    /// </summary>
    public void SetSpecialist() {
        Debug.Log(">> Player Check Specialist <<");
        // 외향 설정
        if (PIER.IsSpecialist) {
            // sp.sprite = SpriteSpecialist;
            anim.SetBool("isSpecial", true);
            ExtraLife = true;
        }
        else {
            // sp.sprite = SpriteNormal;
            // ExtraLife = false;
            SetNormalAgentForce();
        }

        if (isRevived)
            SetNormalAgentForce();

    }

    public void SetBlue() {
        if (ExtraLife)
            return;

        anim.SetBool("isKilled", true);
    }

    public void KillPlayer() {

        // 엑스트라 라이프.
        if(ExtraLife && PIER.IsSpecialist) {
            _breakingArmor.PlayBreakingArmor();
            //sp.sprite = SpriteNormal;
            anim.SetBool("isSpecial", false);
            ExtraLife = false;
            return;
        }


        rigid.bodyType = RigidbodyType2D.Dynamic;

        if (isLeft) {
            rigid.AddForce(new Vector2(Random.Range(-250f, -150), 800));
            this.transform.DORotate(new Vector3(0, 0, 360), 0.4f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }
        else {
            rigid.AddForce(new Vector2(Random.Range(150f, 250f), 800));
            this.transform.DORotate(new Vector3(0, 0, -360), 0.4f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        // this.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        // rigid.AddTorque(360, ForceMode2D.Impulse);
        DropWeapon();

        Destroy(this, 4f);
    }

    void DropWeapon() {
        weapon.transform.SetParent(null);
        weapon.SetDrop(isLeft);
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

        if(!GameManager.isWait)
            AimController.Wait = false;
        
    }

    public void Shoot() {
        weapon.Shoot();
    }

    public void SetLargeCollider() {
        collider.offset = new Vector2(-0.1229745f, 0.06598422f);
        collider.size = new Vector2(1, 2);
        
    }

    public void SetRegularCollider() {
        collider.offset = new Vector2(-0.1229745f, -0.1582869f);
        collider.size = new Vector2(1, 1f);
        
    }
    

    void OnSpawned() {
        SetRegularCollider();
        isMoving = false;
    }
}
