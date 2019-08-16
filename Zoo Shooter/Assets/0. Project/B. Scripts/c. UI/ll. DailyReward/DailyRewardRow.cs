using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class DailyRewardRow : MonoBehaviour
{
    public int ID = 0;
    public Image RowImage; // 행 이미지 
    public Image GunImage; // 마지막 일자 무기 보상
    public GameObject CheckSign;

    
    public GameObject CoinIcon;
    public Text CoinText;
    public int GetCoin = 0;
    public Weapon GetWeapon;

    void InitRow() {
        this.transform.DOKill();
        this.transform.localScale = Vector3.one;
        CheckSign.SetActive(false);
    }
        

    public void SetRow(int order) {

        ID = order;
        InitRow();

        switch (order) {
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
        

        CoinIcon.SetActive(true);
        CoinText.gameObject.SetActive(true);

        // 바탕 처리 
        RowImage.sprite = DailyRewardView.main.ListRowSprite[0];

    }


    /// <summary>
    /// 5일차 보상 설정
    /// </summary>
    /// <param name="w"></param>
    public void SetLastRow(Weapon w) {
        ID = 4;
        GetWeapon = w;
        InitRow();

        GunImage.gameObject.SetActive(true);
        GunImage.sprite = Stocks.GetWeaponInGameSprite(w);
        GunImage.SetNativeSize();

        CoinIcon.SetActive(false);
        CoinText.gameObject.SetActive(false);


        RowImage.sprite = DailyRewardView.main.ListRowSprite[1];
    }


    /// <summary>
    /// 이미 받은 보상 표현 
    /// </summary>
    public void SetTakenRow() {
        RowImage.sprite = DailyRewardView.main.ListRowSprite[2];
    }


    /// <summary>
    /// 금일 보상 받기 처리 
    /// </summary>
    public void TakeReward() {
        RowImage.sprite = DailyRewardView.main.ListRowSprite[2];
        CheckSign.SetActive(true);

        this.transform.DOScale(1.1f, 0.4f).SetLoops(-1, LoopType.Yoyo); // 효과

        if (ID < 4)
            PIER.main.AddCoin(GetCoin);
        else
            PIER.main.AddGun(GetWeapon);


        DailyRewardView.main.OnCompleteTake();
    }
}
