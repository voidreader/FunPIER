using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;

/*
using AudienceNetwork;
using AudienceNetwork.Utility;
*/


public class AdsManager : MonoBehaviour {

    public static AdsManager main = null;

    public Action OnWatchReward;

    [Header("- IronSource -")]
    public string IronSource_Android_ID;
    public string IronSource_iOS_ID;

    #region Unity Ads
    /*
    [Header("- Unity Ads -")]
    public string unityads_android;
    public string unityads_ios, unityads_placement;
    public bool IsUnityAdsAvailable = false;
    */
    #endregion

    #region Facebook Audience 
    /*
    [Header("- Facebook Audience -")]
    
    public bool isFBLoaded = false;
    public bool didCloseFB = false;
    public string fb_android_rewardedID, fb_ios_rewardedID;
    private RewardedVideoAd rewardedVideoAd; // 보상형 동영상 광고 
    */
    #endregion

    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    IEnumerator Start() {

        if (Application.isEditor)
            yield break;


        yield return new WaitForSeconds(0.5f);

        string unityAdsID = string.Empty;
        string ironSourceID = string.Empty;

#if UNITY_ANDROID
        ironSourceID = IronSource_Android_ID;
        // unityAdsID = unityads_android;
#elif UNITY_IOS
        ironSourceID = IronSource_iOS_ID;
        // unityAdsID = unityads_ios;
#else

#endif

        Debug.Log(">>> IronSource Init..!! << "  + ironSourceID);

        InitIronSourceRewarded(); // 예만 예외적으로 가장 먼저. 
        IronSource.Agent.setAdaptersDebug(true);
        IronSource.Agent.init(ironSourceID, IronSourceAdUnits.REWARDED_VIDEO);
        IronSource.Agent.init(ironSourceID, IronSourceAdUnits.INTERSTITIAL);
        IronSource.Agent.init(ironSourceID, IronSourceAdUnits.BANNER);
        IronSource.Agent.validateIntegration();

        InitIronSourceBanner();
        InitIronSourceInterstitial();

        // Debug.Log(">>> Unity Ads init.... !!!! :: " + unityAdsID);

        /*
        Advertisement.AddListener(this);
        Advertisement.Initialize(unityAdsID, false);
        */


        // FAN
        // Init_FAN();
        
    }


    /// <summary>
    /// 중간 광고 넣기 (게임오버, 클리어)
    /// </summary>
    public void OpenMidAdvertisement() {

        Debug.Log("Called OpenMidAdvertisement");

        if (Application.isEditor)
            return;

        // 스페셜리스트 상품 구매자는 광고 띄우지 않음 
        if (PIER.IsSpecialist)
            return;

        int rand = UnityEngine.Random.Range(0, 1000);

        Debug.Log("Called OpenMidAdvertisement rand :: " + rand);


        if (rand < 600 && IsAvailableInterstitial()) {
            OpenInterstitial();
        }
        else {
            OpenRewardAd(delegate { });
        }
    }

    public bool IsAvailableInterstitial() {

        if (Application.isEditor)
            return false;

        Debug.Log(">> IsAvailableInterstitial :: " + IronSource.Agent.isInterstitialReady());
        return IronSource.Agent.isInterstitialReady();
    }

    /// <summary>
    /// 동영상광고 시청 가능 여부 
    /// </summary>
    /// <returns></returns>
    public bool IsAvailableRewardAD() {

        if (Application.isEditor)
            return false;



        //if (this.rewardedAd.IsLoaded() || isFBLoaded)
        //if (this.rewardedAd.IsLoaded() || Advertisement.IsReady(unityads_placement) || isFBLoaded)
        //if (IronSource.Agent.isRewardedVideoAvailable() || Advertisement.IsReady(unityads_placement))
        if (IronSource.Agent.isRewardedVideoAvailable())
            return true;
        else
            return false;
        
    }


    /// <summary>
    /// 전면광고 오픈 
    /// </summary>
    public void OpenInterstitial() {
        Debug.Log(">> OpenInterstitial <<");
        IronSource.Agent.showInterstitial();
        // this.interstitial.Show();   
    }


    /// <summary>
    /// 동영상 광고 시청 
    /// </summary>
    public void OpenRewardAd(Action callback) {

        OnWatchReward = callback;

        if(IronSource.Agent.isRewardedVideoAvailable()) {
            IronSource.Agent.showRewardedVideo();
            return;
        }

        /*
        if (Advertisement.IsReady(unityads_placement)) {
            ShowUnityAds();
            return;
        }
        */

        
    }






    #region IronSource Banner

    void InitIronSourceBanner() {
        IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;

        if(!PIER.IsSpecialist)
            IronSource.Agent.loadBanner(new IronSourceBannerSize(320, 50), IronSourceBannerPosition.BOTTOM);
        
    }

    public void ActivateBannerView() {

        if (Application.isEditor)
            return;

        IronSource.Agent.displayBanner();
    }

    public void HideBannerView() {
        if (Application.isEditor)
            return;

        IronSource.Agent.hideBanner();
    }

    //Invoked once the banner has loaded
    void BannerAdLoadedEvent() {
        Debug.Log("BannerAdLoadedEvent");
    }
    //Invoked when the banner loading process has failed.
    //@param description - string - contains information about the failure.
    void BannerAdLoadFailedEvent(IronSourceError error) {
        Debug.Log("BannerAdLoadFailedEvent : " + error.getDescription());
    }
    // Invoked when end user clicks on the banner ad
    void BannerAdClickedEvent() {
        Debug.Log("BannerAdClickedEvent");
    }
    //Notifies the presentation of a full screen content following user click
    void BannerAdScreenPresentedEvent() {
        Debug.Log("BannerAdScreenPresentedEvent");
    }
    //Notifies the presented screen has been dismissed
    void BannerAdScreenDismissedEvent() {
        Debug.Log("BannerAdScreenDismissedEvent");
    }
    //Invoked when the user leaves the app
    void BannerAdLeftApplicationEvent() {
        Debug.Log("BannerAdLeftApplicationEvent");
    }

    #endregion

    #region IronSource Interstitial
    void InitIronSourceInterstitial() {
        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;

        IronSource.Agent.loadInterstitial();

    }

    //Invoked when the Interstitial Ad Unit has opened
    void InterstitialAdOpenedEvent() {
        Debug.Log("InterstitialAdOpenedEvent");
    }

    //Invoked when the initialization process has failed.
    //@param description - string - contains information about the failure.
    void InterstitialAdLoadFailedEvent(IronSourceError error) {
        Debug.Log("InterstitialAdLoadFailedEvent : " + error.getDescription());
    }
    //Invoked right before the Interstitial screen is about to open.
    void InterstitialAdShowSucceededEvent() {
        Debug.Log("InterstitialAdShowSucceededEvent");
    }
    //Invoked when the ad fails to show.
    //@param description - string - contains information about the failure.
    void InterstitialAdShowFailedEvent(IronSourceError error) {
        Debug.Log("InterstitialAdShowFailedEvent : " + error.getDescription());
    }
    // Invoked when end user clicked on the interstitial ad
    void InterstitialAdClickedEvent() {
        Debug.Log("InterstitialAdClickedEvent");
    }
    //Invoked when the interstitial ad closed and the user goes back to the application screen.
    void InterstitialAdClosedEvent() {
        Debug.Log("InterstitialAdClosedEvent");
        IronSource.Agent.loadInterstitial();
    }
    //Invoked when the Interstitial is Ready to shown after load function is called
    void InterstitialAdReadyEvent() {
        Debug.Log("InterstitialAdReadyEvent");
    }

    #endregion

    #region IronSource Rewarded

    void InitIronSourceRewarded() {
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;

        IronSource.Agent.shouldTrackNetworkState(true);
        

    }

    //Invoked when the RewardedVideo ad view has opened.
    //Your Activity will lose focus. Please avoid performing heavy 
    //tasks till the video ad will be closed.
    void RewardedVideoAdOpenedEvent() {
        Debug.Log("RewardedVideoAdOpenedEvent");
    }
    //Invoked when the RewardedVideo ad view is about to be closed.
    //Your activity will now regain its focus.
    void RewardedVideoAdClosedEvent() {
        Debug.Log("RewardedVideoAdClosedEvent");
    }
    //Invoked when there is a change in the ad availability status.
    //@param - available - value will change to true when rewarded videos are available. 
    //You can then show the video by calling showRewardedVideo().
    //Value will change to false when no videos are available.
    void RewardedVideoAvailabilityChangedEvent(bool available) {
        //Change the in-app 'Traffic Driver' state according to availability.
        bool rewardedVideoAvailability = available;
        Debug.Log("RewardedVideoAvailabilityChangedEvent : " + available);
    }
    //  Note: the events below are not available for all supported rewarded video 
    //   ad networks. Check which events are available per ad network you choose 
    //   to include in your build.
    //   We recommend only using events which register to ALL ad networks you 
    //   include in your build.
    //Invoked when the video ad starts playing.
    void RewardedVideoAdStartedEvent() {
        Debug.Log("RewardedVideoAdStartedEvent");
    }
    //Invoked when the video ad finishes playing.
    void RewardedVideoAdEndedEvent() {
        Debug.Log("RewardedVideoAdEndedEvent");
    }
    //Invoked when the user completed the video and should be rewarded. 
    //If using server-to-server callbacks you may ignore this events and wait for the callback from the  ironSource server.
    //
    //@param - placement - placement object which contains the reward data
    //
    void RewardedVideoAdRewardedEvent(IronSourcePlacement placement) {
        Debug.Log("RewardedVideoAdRewardedEvent");
        OnWatchReward();
    }
    //Invoked when the Rewarded Video failed to show
    //@param description - string - contains information about the failure.
    void RewardedVideoAdShowFailedEvent(IronSourceError error) {

        Debug.Log("RewardedVideoAdShowFailedEvent : " + error.getDescription());
    }


    #endregion

    #region 유니티 애즈 동영상

    /*
    public void ShowUnityAds() {
        Advertisement.Show(unityads_placement);
    }


    void IUnityAdsListener.OnUnityAdsReady(string placementId) {
        IsUnityAdsAvailable = true;
        Debug.Log(">> OnUnityAdsReady :: " + placementId);
    }

    void IUnityAdsListener.OnUnityAdsDidError(string message) {
        Debug.Log(">> OnUnityAdsDidError :: " + message);

    }

    void IUnityAdsListener.OnUnityAdsDidStart(string placementId) {
        Debug.Log(">> OnUnityAdsDidStart :: " + placementId);
    }

    void IUnityAdsListener.OnUnityAdsDidFinish(string placementId, ShowResult showResult) {
        Debug.Log(">> OnUnityAdsDidFinish :: " + showResult.ToString());

        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished) {
            OnWatchReward();
        }
        else if (showResult == ShowResult.Skipped) {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed) {
            
        }
    }
    */

    #endregion

    #region FAN

    /*
    void Init_FAN() {

        if (Application.isEditor)
            return;

        AdUtility.Initialize();


        // Facebook Audience
        FAN_LoadRewardedVideo();
    }

    public void FAN_LoadRewardedVideo() {

        string pid = string.Empty;

#if UNITY_ANDROID

        pid = fb_android_rewardedID;

#elif UNITY_IOS

        pid = fb_ios_rewardedID;

#endif

        Debug.Log("FAN :: LoadRewardedVideo : " + pid);
        // Create the rewarded video unit with a placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        this.rewardedVideoAd = new RewardedVideoAd(pid);

        this.rewardedVideoAd.Register(this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.rewardedVideoAd.RewardedVideoAdDidLoad = (delegate () {
            Debug.Log("FAN :: RewardedVideo ad loaded.");
            this.isFBLoaded = true;
        });
        this.rewardedVideoAd.RewardedVideoAdDidFailWithError = (delegate (string error) {
            Debug.Log("FAN :: RewardedVideo ad failed to load with error: " + error);
        });
        this.rewardedVideoAd.RewardedVideoAdWillLogImpression = (delegate () {
            Debug.Log("FAN :: RewardedVideo ad logged impression.");
        });
        this.rewardedVideoAd.RewardedVideoAdDidClick = (delegate () {
            Debug.Log("FAN :: RewardedVideo ad clicked.");
        });

        this.rewardedVideoAd.RewardedVideoAdDidClose = (delegate () {
            Debug.Log("FAN :: Rewarded video ad did close.");
            if (this.rewardedVideoAd != null) {
                this.rewardedVideoAd.Dispose();
            }

            FAN_LoadRewardedVideo();
        });

        this.rewardedVideoAd.rewardedVideoAdComplete = OnCompleteFAN;
            

        // Initiate the request to load the ad.
        this.rewardedVideoAd.LoadAd();
    }

    void OnCompleteFAN() {

        Debug.Log("Rewarded video ad OnCompleteFAN");
        OnWatchReward();
    }

    public void FAN_ShowRewardedVideo() {
        if (this.isFBLoaded) {
            Debug.Log("FAN_ShowRewardedVideo");

            this.rewardedVideoAd.Show();
            this.isFBLoaded = false;
        }
        else {
            Debug.Log("Ad not loaded. Click load to request an ad.");
        }
    }
    */
    #endregion

    private void OnApplicationPause(bool pause) {
        if (Application.isEditor)
            return;

        IronSource.Agent.onApplicationPause(pause);
    }
}
