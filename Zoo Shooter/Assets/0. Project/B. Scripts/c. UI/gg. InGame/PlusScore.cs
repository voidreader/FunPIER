using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlusScore : MonoBehaviour
{

    public Text _text;
    public Animator _anim;

    public void GetScore(int s, int index = 0, bool isDouble = false) {

        this.gameObject.SetActive(true);
        this.transform.DOKill();
        this.transform.localPosition = new Vector3(-271, 500 - (index * 50) , 0); 


        GameViewManager.main.ListActiveScores.Add(this); // 활성화된건 액티브 리스트에 추가 


        _text.text = "+" + s.ToString();
        if(isDouble) {
            _text.fontSize = 50;
            _text.color = Stocks.main.ColorHeadshotFont;
        }
        else {
            _text.fontSize = 40;
            _text.color = Color.white;

        }

        _anim.Play("Score");

        this.transform.DOLocalMoveY(this.transform.localPosition.y + 50, 0.25f);

    }

    void OnCompleteAnim() {
        GameViewManager.main.ListActiveScores.Remove(this); // 연출끝났으면 액티브 리스트에서 제거 
        this.gameObject.SetActive(false);
    }
}
