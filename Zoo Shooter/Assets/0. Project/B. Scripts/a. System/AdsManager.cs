using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour {

    private BannerView bannerView = null;
    private InterstitialAd interstitial = null;
    private RewardedAd rewardedAd = null;

    // Start is called before the first frame update
    void Start() {
#if UNITY_ANDROID
        string appId = "ca-app-pub-8118299571958162~9535004317";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-8118299571958162~7461652542";
#else
            string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);


        RequestBanner();
        RequestInterstitial();
        RequestRewardAd();
    }


    /// <summary>
    /// 동영상광고 시청 가능 여부 
    /// </summary>
    /// <returns></returns>
    public bool IsAvailableRewardAD() {

        if (!this.rewardedAd.IsLoaded())
            RequestRewardAd();    

        return this.rewardedAd.IsLoaded();
    }


    /// <summary>
    /// 전면광고 오픈 
    /// </summary>
    public void OpenInterstitial() {
        if(this.interstitial == null || !this.interstitial.IsLoaded()) {
            RequestInterstitial();
            return;
        }
    }


    /// <summary>
    /// 동영상 광고 시청 
    /// </summary>
    public void OpenRewardAd() {
        if(this.rewardedAd == null || !this.rewardedAd.IsLoaded()) {
            RequestRewardAd();
            return;
        }
    }


    #region 배너

    private void RequestBanner() {

#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

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

    #region 전면
    private void RequestInterstitial() {


        if (this.interstitial != null && this.interstitial.IsLoaded())
            return;


#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += InterHandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += InterHandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += InterHandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += InterHandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += InterHandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);



        

    }

    public void InterHandleOnAdLoaded(object sender, EventArgs args) {
        Debug.Log("HandleAdLoaded event received");
    }

    public void InterHandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        Debug.Log("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void InterHandleOnAdOpened(object sender, EventArgs args) {
        Debug.Log("HandleAdOpened event received");
    }

    public void InterHandleOnAdClosed(object sender, EventArgs args) {
        Debug.Log("HandleAdClosed event received");
        RequestInterstitial();
    }

    public void InterHandleOnAdLeavingApplication(object sender, EventArgs args) {
        Debug.Log("HandleAdLeavingApplication event received");
    }
    #endregion

    #region 동영상

    public void RequestRewardAd() {

        if (rewardedAd != null && rewardedAd.IsLoaded())
            return;

        string adUnitId;
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);

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

    public void HandleRewardedAdLoaded(object sender, EventArgs args) {
        Debug.Log("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args) {
        Debug.Log(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args) {
        Debug.Log("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args) {
        Debug.Log(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args) {
        Debug.Log("HandleRewardedAdClosed event received");

        RequestRewardAd();
    }

    public void HandleUserEarnedReward(object sender, Reward args) {
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);


        RequestRewardAd();
    }

    #endregion
}
