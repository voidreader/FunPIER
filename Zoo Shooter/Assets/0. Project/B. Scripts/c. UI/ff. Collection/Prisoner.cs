using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
    

public class Prisoner : MonoBehaviour {
    public Image _sprite;
    public Animator _anim;
    public Vector3 _originPos;

    bool _isMoving = false;
    int routineIndex = 0;
    int tier = 0;

    public Transform currentTransform = null;

    public void SetPrisoner() {

        currentTransform = this.transform;
        routineIndex = 0;

        this.gameObject.SetActive(true);
        this.transform.localPosition = new Vector3(Random.Range(-320f, 320f), CollectionManager.GetPrisonerMoveYcoord(), 0); // 위치 지정
        this.transform.DOKill();

        _originPos = this.transform.localPosition;

        _sprite.sprite = Stocks.GetRandomPrisoner(); // 랜덤 보스  몹으로. 
        _sprite.SetNativeSize(); // 크기 조정 


        _anim.SetBool("Walk", false);
        _isMoving = false;


        if(Random.Range(0,2) == 0)
            this.transform.localScale = new Vector3(0.6f, 0.6f, 1);
        else
            this.transform.localScale = new Vector3(-0.6f, 0.6f, 1);

        
        StartCoroutine(PrisonerRoutine());
    }

    IEnumerator PrisonerRoutine() {
        float destX, destY;

        while (true) {

            while (_isMoving)
                yield return null;

            // 목적지 설정
            if (Random.Range(0, 2) == 0)
                destX = this.transform.localPosition.x + Random.Range(-100f, -30f);
            else
                destX = this.transform.localPosition.x + Random.Range(30f, 100f);

            if (destX < -320f)
                destX = -320f;
            else if (destX > 320f)
                destX = 320f;

            destY = this.transform.localPosition.y + Random.Range(-20f, 20f);
            if (destY < 20)
                destY = 20;
            else if (destY > 135f)
                destY = 135f;

            // 랜덤한 시간 대기 
            if (routineIndex >  0)
                yield return new WaitForSeconds(Random.Range(2f, 6f));

            // 걷기 시작
            _anim.SetBool("Walk", true);
            _isMoving = true;

            // 방향 설정 
            if (destX > this.transform.localPosition.x)
                this.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            else
                this.transform.localScale = new Vector3(-0.6f, 0.6f, 1);

            this.transform.DOLocalMove(new Vector3(destX, destY, 0), Random.Range(0.5f, 2.5f)).SetEase(Ease.Linear).OnComplete(OnCompleteMove);
            routineIndex++;

        }
    }



    void OnCompleteMove() {

        _anim.SetBool("Walk", false);
        _isMoving = false;

        /*
        _anim.SetBool("Walk", false);

        if(CollectionManager.main.gameObject.activeSelf)
            StartCoroutine(StartMove());
        */
    }


    public float GetLocalPositionY() {
        return currentTransform.localPosition.y;
    }

    public void SetHide() {
        this.gameObject.SetActive(false);
    }
}
