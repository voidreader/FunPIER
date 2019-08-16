using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardRow : MonoBehaviour
{
    public Image RowImage;
    public Image GunImage;

    public Text CoinText;
    public int GetCoin = 0;

    public void SetRow(int order) {

        switch(order) {
            case 0:
                GetCoin = 50;
                break;
            case 1:
                GetCoin = 75;
                break;
            case 2:
                GetCoin = 100;
                break;
            case 3:
                GetCoin = 200;
                break;

        }

        CoinText.text = "+ " + GetCoin.ToString();
        GunImage.gameObject.SetActive(false);


        // 바탕 처리 
    }

    public void SetLastRow(Weapon w) {

    }
}
