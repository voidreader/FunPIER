using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Advertisements;
//using UnityEngine.Analytics;
using UnityEngine.UI;

public class ADs : MonoBehaviour {
    public float adTimer = 0; //Displaying interstitial ads

    public Text adMoneyText; //For the Watch ad to get money Booster

    public static ADs _instance; //instance for easier usage
    public decimal moneyToAdd; //For double money (in upgrade / offline reward)

    public GameObject glowExplosion;

    public float timeToshowInterstitial = 300;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {/*
#if UNITY_IOS
        Advertisement.Initialize("2673566");
#elif UNITY_ANDROID
        Advertisement.Initialize("2673568");
#endif

        StartCoroutine(UpdateTexts());
        */
    }

    // Update is called once per frame
    void Update() {
        adTimer += Time.deltaTime;
        if (adTimer > timeToshowInterstitial) {
            ShowInterstitial();
            adTimer = 0;
        }
    }

    /// <summary>
    /// Updating BOOSTER text (watch ad to get money) with the amount of money you get
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateTexts() {
        while (true) {
            adMoneyText.text = Statistics._instance.GetSuffix(Statistics._instance.DMax(3500,
                decimal.Round(8 * Statistics._instance.ballDamage * (int)Statistics._instance.fireRate *
                              LevelBar._instance.level * LevelBar._instance.level)));
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// Showing interstitial AD and sending unity custom event
    /// </summary>
    public void ShowInterstitial() {
        if (adTimer < 120) return;
        adTimer = 0;
        //        Analytics.CustomEvent("interstitial_request ");

        //       if (PlayerPrefs.GetInt("ADS") != 1 && Advertisement.IsReady()) //Checking if Remove Ads purchased
        //            Advertisement.Show();

    }

    /// <summary>
    /// Booster OnClick method
    /// </summary>
    public void BuyMoneyByAd() {
        moneyToAdd = Statistics._instance.DMax(3500,
            decimal.Round(8 * Statistics._instance.ballDamage * (int)Statistics._instance.fireRate *
                          LevelBar._instance.level * LevelBar._instance.level));
        ShowRewardedGetMoney();
    }

    public void ShowRewardedGetMoney() {
        /*        Analytics.CustomEvent("video_request_money");

                ShowOptions options = new ShowOptions();
                options.resultCallback = HandleShowResultAddMoney;
                Advertisement.Show("rewardedVideo", options);    
                */
    }


    /// <summary>
    /// Callback after viewing an AD
    /// </summary>
    /// <param name="result"></param>
    /* private void HandleShowResultAddMoney(ShowResult result) {
         if (result == ShowResult.Finished) {
             Debug.Log("Video completed - Offer a reward to the player");

             Statistics._instance.AddMoney(moneyToAdd);
             Toast.instance.ShowMessage(Statistics._instance.GetSuffix(moneyToAdd) + " collected");

             foreach (Level l in Statistics._instance.levels) {
                 if (l.type == Level.Type.useBoosters)
                     l.progression++;
             }
         }
     }*/

    public void ShowRewardedBoost() {
/*
        Analytics.CustomEvent("video_request_boost");

        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResultBoost;
        Advertisement.Show("rewardedVideo", options);
        */
    }

    /*
    private void HandleShowResultBoost(ShowResult result) {
        if (result == ShowResult.Finished) {
            Debug.Log("Video completed - Offer a reward to the player");
            GameManager._instance.boostTime = 120;
            Toast.instance.ShowMessage("Boost activated");

            foreach (Level l in Statistics._instance.levels) {
                if (l.type == Level.Type.useBoosters)
                    l.progression++;
            }
        }
    }*/

    /// <summary>
    /// OnClick listener for Tap Boost
    /// </summary>
    public void ShowRewardedTap() {
/*
        Analytics.CustomEvent("video_request_tap");

        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResultTap;
        Advertisement.Show("rewardedVideo", options);
        */
    }

    /// <summary>
    /// Callback for Tap rewrded video
    /// </summary>
    /// <param name="result"></param>
/*
    private void HandleShowResultTap(ShowResult result) {
        if (result == ShowResult.Finished) {
            Debug.Log("Video completed - Offer a reward to the player");
            GameManager._instance.tapBoostTime = 60;
            Toast.instance.ShowMessage("Tap boost activated");

            foreach (Level l in Statistics._instance.levels) {
                if (l.type == Level.Type.useBoosters)
                    l.progression++;
            }
        }
    }
    */

}