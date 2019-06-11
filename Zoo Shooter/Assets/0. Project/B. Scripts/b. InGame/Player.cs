using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Player : MonoBehaviour
{

    public SpriteRenderer sp;
    public bool isMoving = false;
    Vector3 targetPos;


    public void InitPlayer() {
        isMoving = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
