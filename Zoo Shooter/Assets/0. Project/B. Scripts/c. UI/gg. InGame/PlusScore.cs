using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlusScore : MonoBehaviour
{

    public Text _text;
    public Animator _anim;

    public void GetScore(int s, bool isDouble = false) {

        this.gameObject.SetActive(true);
        this.transform.DOKill();
        this.transform.localPosition = new Vector3(-271, 500, 0);
        

        _text.text = "+" + s.ToString();
        if(isDouble) {
            _text.fontSize = 50;
            _text.color = Stocks.main.ColorHeadshotFont;
        }
        else {
            _text.fontSize = 35;
            _text.color = Color.white;

        }

        _anim.Play("Score");

        this.transform.DOLocalMoveY(550, 0.25f);

    }

    void OnCompleteAnim() {
        this.gameObject.SetActive(false);
    }
}
