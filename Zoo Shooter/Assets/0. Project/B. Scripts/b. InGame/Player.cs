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


    public void InitPlayer() {
        isMoving = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() { // Player 입력

        // 이동 중일때 쏘지 못함 
        if (isMoving)
            return;

        // 플레이 중이 아닐 경우 체크 
        if (!GameManager.main.isPlaying)
            return;


        // Aim 중이지 않을때 대기 
        if (AimController.Wait)
            return;

        if (Input.GetMouseButtonDown(0))
            StartCoroutine(ShootRoutine());

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
        this.transform.DOMove(targetPos, 0.5f).OnComplete(OnCompleteMove);
        yield return null;
    }

    void OnCompleteMove() {
        isMoving = false;
    }



    public void SetSpriteDirection(bool isLeft) {
        sp.flipX = isLeft;
    }

    /// <summary>
    /// 총 겨누기 시작 
    /// </summary>
    public void Aim() {
        // weapon.StartAim(isLeft);
        weapon.Init();
        
    }

    public void Shoot() {
        // weapon.Shoot();
    }
}
