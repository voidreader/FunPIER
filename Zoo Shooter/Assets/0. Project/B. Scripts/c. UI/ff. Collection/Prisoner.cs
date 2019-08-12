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

    public void SetPrisoner() {
        this.gameObject.SetActive(true);
        this.transform.localPosition = new Vector3(Random.Range(-320f, 320f), Random.Range(0f, 120f), 0); // 위치 지정
        this.transform.DOKill();
        _originPos = this.transform.localPosition;

        _sprite.sprite = Stocks.GetRandomPrisoner();
        _sprite.SetNativeSize(); // 크기 조정 


        _anim.SetBool("Walk", false);
        _isMoving = false;


        if(Random.Range(0,2) == 0)
            this.transform.localScale = new Vector3(1, 1, 1);
        else
            this.transform.localScale = new Vector3(-1, 1, 1);


        // StartCoroutine(StartMove());
        StartCoroutine(PrisonerRoutine());
    }

    IEnumerator PrisonerRoutine() {
        float destX, destY;

        while (true) {

            while (_isMoving)
                yield return null;

            // 목적지 설정
            if (Random.Range(0, 2) == 0)
                destX = this.transform.localPosition.x + Random.Range(-80f, -20f);
            else
                destX = this.transform.localPosition.x + Random.Range(20f, 80f);

            if (destX < -320f)
                destX = -320f;
            else if (destX > 320f)
                destX = 320f;

            destY = this.transform.localPosition.y + Random.Range(-20f, 20f);
            if (destY < 0)
                destY = 0;
            else if (destY > 120f)
                destY = 120f;

            // 랜덤한 시간 대기 
            yield return new WaitForSeconds(Random.Range(2f, 8f));

            // 걷기 시작
            _anim.SetBool("Walk", true);
            _isMoving = true;

            // 방향 설정 
            if (destX > this.transform.localPosition.x)
                this.transform.localScale = new Vector3(1, 1, 1);
            else
                this.transform.localScale = new Vector3(-1, 1, 1);

            this.transform.DOLocalMove(new Vector3(destX, destY, 0), Random.Range(0.5f, 2.5f)).SetEase(Ease.Linear).OnComplete(OnCompleteMove);

        }
    }


    IEnumerator StartMove() {
        float destX, destY;


        if(Random.Range(0,2) == 0)
            destX = this.transform.localPosition.x + Random.Range(-80f, -20f);
        else
            destX = this.transform.localPosition.x + Random.Range(20f, 80f);




        if (destX < -320f)
            destX = -320f;
        else if (destX > 320f)
            destX = 320f;

        destY = this.transform.localPosition.y + Random.Range(-20f, 20f);
        if (destY < -20f)
            destY = -20f;
        else if (destY > 120f)
            destY = 120f;

        yield return new WaitForSeconds(Random.Range(2f, 8f));

        _anim.SetBool("Walk", true);
        _isMoving = true;

        if(destX > this.transform.localPosition.x)
            this.transform.localScale = new Vector3(1, 1, 1);
        else
            this.transform.localScale = new Vector3(-1, 1, 1);

        this.transform.DOLocalMove(new Vector3(destX, destY, 0), Random.Range(0.5f, 2.5f)).SetEase(Ease.Linear).OnComplete(OnCompleteMove);

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



    public void SetHide() {
        this.gameObject.SetActive(false);
    }
}
