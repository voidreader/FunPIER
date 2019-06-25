using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceManager : MonoBehaviour {

   /* public static EnhanceManager _instance;
    public int boostTime;

    public Text getMoneyText;


    float adTimer;

    public decimal moneyToAdd;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {
        Enhance.ServiceTermsOptIn();
    }

    // Update is called once per frame
    void Update() {
        getMoneyText.text = Statistics._instance.GetSuffix(Statistics._instance.DMax(3500, decimal.Round(15 * Statistics._instance.ballDamage * (int)Statistics._instance.fireRate * LevelBar._instance.level * Statistics._instance.offlinePercent)));

        adTimer += Time.deltaTime;
        if (adTimer > 240) {
            //ShowRandom();
            adTimer = 0;
        }
    }

    public void OnShowRewardedAdBoost() {
        // Check whether a rewarded ad is ready 

        if (!Enhance.IsRewardedAdReady()) {
            return;
        }

        // The ad is ready, show it

        Enhance.ShowRewardedAd(FGLEnhance.REWARDED_PLACEMENT_NEUTRAL, OnRewardGrantedBoost, OnRewardDeclined, OnRewardUnavailable);
    }

    private void OnRewardGrantedBoost(Enhance.RewardType rewardType, int rewardValue) {
        // Reward is granted (user watched video for example)
        // You can check reward type:
        RewardSuccessBoost();
        FGLEnhance.LogEvent("boost_video");
    }

    private void OnRewardDeclined() {
        // Reward is declined (user closed the ad for example)
    }

    private void OnRewardUnavailable() {
        // Reward is unavailable (network error for example)
    }

    public void OnShowRewardedAdGetMoney() {
        // Check whether a rewarded ad is ready 

        if (!Enhance.IsRewardedAdReady()) {
            return;
        }

        // The ad is ready, show it

        Enhance.ShowRewardedAd(OnRewardGrantedGetMoney, OnRewardDeclined, OnRewardUnavailable);
    }

    private void OnRewardGrantedGetMoney(Enhance.RewardType rewardType, int rewardValue) {
        // Reward is granted (user watched video for example)
        // You can check reward type:

        //rewardRevive = true;
        RewardedSuccess();
        Enhance.LogEvent("OnRewardGrantedGetMoney");
    }


    public void ShowRandom() {
        if (PlayerPrefs.GetInt("ADS", 0) == 0)
            Enhance.ShowInterstitialAd();
        //Advertisement.Show();
    }

    private void RewardedSuccess() {
        Statistics._instance.AddMoney(moneyToAdd);
    }

    private void RewardSuccessBoost() {
        GameManager._instance.boostTime = boostTime;
    }

    public void RemoveAds() {
        PlayerPrefs.SetInt("ADS", 1);
    }

    public void BuyMoneyByAd() {
        moneyToAdd = Statistics._instance.DMax(3500, decimal.Round(15 * Statistics._instance.ballDamage * (int)Statistics._instance.fireRate * LevelBar._instance.level * Statistics._instance.offlinePercent));

        OnShowRewardedAdGetMoney();
    }*/
}
