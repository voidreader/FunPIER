﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

using AudienceNetwork;
using AudienceNetwork.Utility;

using System;

public class AdsManager : MonoBehaviour {

    public static AdsManager main = null;

    public Action OnWatchReward;

    #region 애드몹 
    [Header("- Google Admob -")]
    private BannerView bannerView = null;
    private GoogleMobileAds.Api.InterstitialAd interstitial = null;
    private RewardedAd rewardedAd = null;

    public string admob_android_appID, admob_ios_appID;
    public string admob_android_bannerID, admob_android_interstitialID, admob_android_rewardedID;
    public string admob_ios_bannerID, admob_ios_interstitialID, admob_ios_rewardedID;

    #endregion

    #region Facebook Audience 
    [Header("- Facebook Audience -")]
    public bool isFBLoaded = false;
    public bool didCloseFB = false;
    public string fb_android_rewardedID, fb_ios_rewardedID;
    private RewardedVideoAd rewardedVideoAd; // 보상형 동영상 광고 
    #endregion

    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start() {


#if UNITY_ANDROID
        string appId = admob_android_appID;
#elif UNITY_IPHONE
            string appId = admob_ios_appID;
#else
            string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId); 

        // 애드몹 초기화 
        RequestBanner();
        RequestInterstitial();
        RequestRewardAd();

        if (!AdUtility.IsInitialized()) {
            AdUtility.Initialize();
        }

        // Facebook Audience
        LoadRewardedVideo();
    }


    /// <summary>
    /// 동영상광고 시청 가능 여부 
    /// </summary>
    /// <returns></returns>
    public bool IsAvailableRewardAD() {

        if (!this.rewardedAd.IsLoaded())
            RequestRewardAd();

        if (!this.isFBLoaded)
            LoadRewardedVideo();


        if (this.rewardedAd.IsLoaded() || isFBLoaded)
            return true;
        else return false;
        
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
    public void OpenRewardAd(Action callback) {

        OnWatchReward = callback;

        // 애드몹 최우선 
        if(this.rewardedAd == null || !this.rewardedAd.IsLoaded()) {
            RequestRewardAd();
            return;
        }

        // Facebook Audience
        if(isFBLoaded) {
            this.rewardedVideoAd.Show();
            this.isFBLoaded = false;
            return;
        }
    }


    #region 배너

    private void RequestBanner() {

#if UNITY_ANDROID
        string adUnitId = admob_android_bannerID;
#elif UNITY_IPHONE
            string adUnitId = admob_ios_bannerID;
#else
            string adUnitId = "unexpected_platform";
#endif

        bannerView = new BannerView(adUnitId, GoogleMobileAds.Api.AdSize.Banner, GoogleMobileAds.Api.AdPosition.Bottom);

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
        string adUnitId = admob_android_interstitialID;
#elif UNITY_IPHONE
        string adUnitId = admob_ios_interstitialID;
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new GoogleMobileAds.Api.InterstitialAd(adUnitId);

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

    #region 애드몹 동영상

    public void RequestRewardAd() {

        if (rewardedAd != null && rewardedAd.IsLoaded())
            return;

        string adUnitId;
#if UNITY_ANDROID
        adUnitId = admob_android_rewardedID;
#elif UNITY_IPHONE
            adUnitId = admob_ios_rewardedID;
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

        OnWatchReward(); // callback 호출 
    }

    #endregion

    #region 페이스북 동영상 광고 

    /// <summary>
    /// 페이스북 보상형 광고 불러오기 
    /// </summary>
    public void LoadRewardedVideo() {

        if(Application.isEditor) {
            isFBLoaded = false;
            return;
        }

        string placement_id = string.Empty;
#if UNITY_ANDROID
        placement_id = fb_android_rewardedID;
#elif UNITY_IOS
        placement_id = fb_ios_rewardedID;
#endif

        Debug.Log("Loading.. Facebook Audience RewardedVideo :: " + placement_id);

        // Create the rewarded video unit with a placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        this.rewardedVideoAd = new RewardedVideoAd(placement_id);
        this.rewardedVideoAd.Register(this.gameObject);
        

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.rewardedVideoAd.RewardedVideoAdDidLoad = (delegate () {

            Debug.Log("RewardedVideo ad loaded.");
            isFBLoaded = true;
            didCloseFB = false;
            string isAdValid = rewardedVideoAd.IsValid() ? "valid" : "invalid";
            Debug.Log("Ad loaded and is " + isAdValid + ". Click show to present!");

        });
        this.rewardedVideoAd.RewardedVideoAdDidFailWithError = (delegate (string error) {
            Debug.Log("RewardedVideo ad failed to load with error: " + error);
        });
        this.rewardedVideoAd.RewardedVideoAdWillLogImpression = (delegate () {
            Debug.Log("RewardedVideo ad logged impression.");
        });
        this.rewardedVideoAd.RewardedVideoAdDidClick = (delegate () {
            Debug.Log("RewardedVideo ad clicked.");
        });

        this.rewardedVideoAd.RewardedVideoAdDidClose = (delegate () {
            Debug.Log("Rewarded video ad did close.");
            didCloseFB = true;
            if (this.rewardedVideoAd != null) {
                this.rewardedVideoAd.Dispose();
            }
        });

#if UNITY_ANDROID
        /*
         * Only relevant to Android.
         * This callback will only be triggered if the Rewarded Video activity
         * has been destroyed without being properly closed. This can happen if
         * an app with launchMode:singleTask (such as a Unity game) goes to
         * background and is then relaunched by tapping the icon.
         */
        rewardedVideoAd.RewardedVideoAdActivityDestroyed = delegate () {
            if (!didCloseFB) {
                Debug.Log("Rewarded video activity destroyed without being closed first.");
                Debug.Log("Game should resume. User should not get a reward.");
            }
        };
#endif

        // 완료 콜백 추가 
        this.rewardedVideoAd.RewardedVideoAdComplete = OnCompleteFacebookAD;

        // Initiate the request to load the ad.
        this.rewardedVideoAd.LoadAd();
    }

    void OnCompleteFacebookAD() {
        Debug.Log("Rewarded video ad OnCompleteFacebookAD");
        OnWatchReward();

        LoadRewardedVideo(); // 주모! 여기 다음광고 추가요 
    }

    #endregion
}
