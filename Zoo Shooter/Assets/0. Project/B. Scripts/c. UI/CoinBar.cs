using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinBar : MonoBehaviour
{
    public Text lblCoin;
    public int coinValue = 0;

    private void Awake() {
        PIER.CoinRefresh += UpdateCoin;
    }

    IEnumerator Start() {
        yield return new WaitForSeconds(0.1f);

        // 첫 세팅 
        coinValue = PIER.main.Coin;
        lblCoin.text = coinValue.ToString();
            
    }

    void UpdateCoin() {

        InitScale();

        if(coinValue != PIER.main.Coin) { // 코인값이 다르면 연출 
            this.transform.DOScale(1.1f, 0.2f).SetLoops(4, LoopType.Yoyo).OnComplete(InitScale);
        }


        coinValue = PIER.main.Coin;
        lblCoin.text = coinValue.ToString();
    }

    void InitScale() {
        this.transform.DOKill();
        this.transform.localScale = Vector3.one;
    }
}


