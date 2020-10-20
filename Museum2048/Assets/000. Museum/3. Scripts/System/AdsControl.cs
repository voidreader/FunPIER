using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsControl : MonoBehaviour
{

    public static AdsControl main = null;

    public string appID = string.Empty;
    public string bannerUnitID = string.Empty;
    public string interstitialID = string.Empty;
    public string rewardUnitID = string.Empty;

    private BannerView bannerView = null; // 배너
    InterstitialAd interstitial = null; // 전면광고 
    RewardedAd rewardedAd = null; // 보상형광고 

    bool _isCoolingPauseAds = false;

    public Action OnWatchedAd = delegate { };

    public bool IsCoolingPauseAds { get => _isCoolingPauseAds; set => _isCoolingPauseAds = value; }

    private void Awake() {
        main = this;



    }

    // Start is called before the first frame update
    void Start() {
#if UNITY_ANDROID
        appID = "ca-app-pub-4267813218499125~2522519468";
#elif UNITY_IOS
        appID = "ca-app-pub-4267813218499125~1373089321";
#endif

        Debug.Log("Google admob appID :: " + appID);

        // Initialize the Google Mobile Ads SDK.
        // MobileAds.Initialize(appID);
        MobileAds.Initialize(AdmobInitCallback);


    }

    void AdmobInitCallback(InitializationStatus __stat)
    {
        Debug.Log("AdmobInitCallback : " + __stat.ToString());
    }


#region Banner

    public void RequestBanner() {


        if (PierSystem.main.NoAds > 0)
            return;

        

#if UNITY_ANDROID
        bannerUnitID = "ca-app-pub-4267813218499125/4829952751";
#elif UNITY_IOS
        bannerUnitID = "ca-app-pub-4267813218499125/7631661327";
#endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(bannerUnitID, AdSize.Banner, AdPosition.Bottom);


        // Called when an ad request has successfully loaded.
        bannerView.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        bannerView.OnAdOpening += HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        bannerView.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args) {
        Debug.Log("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        Debug.Log("HandleFailedToReceiveAd event received with message: "
                            + args.Message);

        Debug.Log("HandleFailedToReceiveAd event received with message: "
                        + args.ToString());

    }

    public void HandleOnAdOpened(object sender, EventArgs args) {
        Debug.Log("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args) {
        Debug.Log("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args) {
        Debug.Log("HandleAdLeavingApplication event received");
    }

#endregion

#region Interstitial

    public void RequestInterstitial() {
#if UNITY_ANDROID
        interstitialID = "ca-app-pub-4267813218499125/2762478023";
#else
        interstitialID = "ca-app-pub-4267813218499125/2451201421";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(interstitialID);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoadedInterstitial;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoadInterstitial;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpenedInterstitial;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosedInterstitial;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplicationInterstitial;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);

    }


    void HandleOnAdLoadedInterstitial(object sender, EventArgs args) {
        Debug.Log("HandleAdLoaded event received");
    }

    void HandleOnAdFailedToLoadInterstitial(object sender, AdFailedToLoadEventArgs args) {
        Debug.Log("HandleFailedToReceiveAd event received with message: "
                            + args.Message);

        Debug.Log("HandleFailedToReceiveAd event received with message: "
                        + args.ToString());

    }

    void HandleOnAdOpenedInterstitial(object sender, EventArgs args) {
        Debug.Log("HandleAdOpened event received");
    }

    void HandleOnAdClosedInterstitial(object sender, EventArgs args) {
        Debug.Log("HandleAdClosed event received new ad loading");

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);

    }

    void HandleOnAdLeavingApplicationInterstitial(object sender, EventArgs args) {
        Debug.Log("HandleAdLeavingApplication event received");
    }



    /// <summary>
    /// 전면광고 오픈 
    /// </summary>
    public void ShowInterstitial() {

        if (PierSystem.main.NoAds > 0)
            return;

        if (this.interstitial == null)
            return;

        if(this.interstitial.IsLoaded()) {
            IsCoolingPauseAds = true;
            this.interstitial.Show();
            
        }
    }

#endregion

#region 동영상 광고 

    public void RequestRewardAd() {
#if UNITY_ANDROID
        rewardUnitID = "ca-app-pub-4267813218499125/6497708908";
#else
        rewardUnitID = "ca-app-pub-4267813218499125/6433844317";
#endif

        this.rewardedAd = new RewardedAd(rewardUnitID);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);



    }

    void HandleRewardedAdLoaded(object sender, EventArgs args) {
        Debug.Log("HandleRewardedAdLoaded event received");
    }

    void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args) {
        Debug.Log(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    void HandleRewardedAdOpening(object sender, EventArgs args) {
        Debug.Log("HandleRewardedAdOpening event received");
    }

    void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args) {
        Debug.Log(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    void HandleRewardedAdClosed(object sender, EventArgs args) {
        Debug.Log("HandleRewardedAdClosed event received");
        CreateAndLoadRewardedAd();
        
    }

    /// <summary>
    /// 동영상 광고 플레이 
    /// </summary>
    public void ShowWatchAd(Action callback) {


        OnWatchedAd = callback;
        

        if (Application.isEditor) {
            OnWatchedAd();
            OnWatchedAd = delegate { };
            return;
        }

        if (!this.rewardedAd.IsLoaded()) {
            Debug.Log("admob rewarded ad is not ready");
            OnWatchedAd();
            OnWatchedAd = delegate { };
            CreateAndLoadRewardedAd();
            return;
        }

        Debug.Log(">> ShowWatchAd << ");

        IsCoolingPauseAds = true;
        this.rewardedAd.Show();
        IsCoolingPauseAds = true;



    }

    /// <summary>
    /// 보상 획득 처리 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void HandleUserEarnedReward(object sender, Reward args) {
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);


        // 

        OnWatchedAd();
        OnWatchedAd = delegate { };
    }

    public void CreateAndLoadRewardedAd() {
#if UNITY_ANDROID
        rewardUnitID = "ca-app-pub-4267813218499125/6497708908";
#else
        rewardUnitID = "ca-app-pub-4267813218499125/6433844317";
#endif

        this.rewardedAd = new RewardedAd(rewardUnitID);

        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

#endregion


    /// <summary>
    /// 광고 없애기 체크 
    /// </summary>
    public void NoAdsCheck() {
        if(PierSystem.main.NoAds > 0) {
            bannerView.Hide();
        }

        LobbyManager.main.RefreshNoAdsButton();
        
    }



    /// <summary>
    /// 화면 활성 비활성 체크 
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool pause) {

        if (pause) // 활성화될때에만 로직 실행
            return;

        if (IsCoolingPauseAds) {
            StartCoroutine(CoolingPauseADs());
            return;
        }


        // 전면 배너 오픈 
        ShowInterstitial();

    }

    IEnumerator CoolingPauseADs() {
        yield return new WaitForSeconds(3);
        IsCoolingPauseAds = false;
    }
}
