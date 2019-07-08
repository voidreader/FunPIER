using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WantedRewardCol : MonoBehaviour
{

    public Image coinImage, gunImage;
    public GameObject specialBG, selector;
    public Text coinValue;

    public Weapon weapon;
    public int coinReward;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="w"></param>
    public void SetSpecialReward(Weapon w) {

        specialBG.SetActive(true);
        weapon = w;

        if(w != null) { // 얻을 수 있는 무기 있을때 
            SetGunReward();
        }
        else { // 없을때는 코인으로 대체 
            SetCoinReward(100);
        }

       

    }


    void SetGunReward() {
        coinImage.gameObject.SetActive(false);
        coinValue.gameObject.SetActive(false);
        gunImage.gameObject.SetActive(true);
        gunImage.sprite = null; // 
    }

    /// <summary>
    /// 코인 보상으로 설정 
    /// </summary>
    /// <param name="v"></param>
    void SetCoinReward(int v) {
        coinImage.gameObject.SetActive(true);
        coinValue.gameObject.SetActive(true);
        gunImage.gameObject.SetActive(false);

        coinValue.text = "+" + v.ToString();
        coinReward = v;
    }
    

    
}
