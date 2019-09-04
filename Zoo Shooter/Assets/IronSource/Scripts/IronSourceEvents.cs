using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public class IronSourceEvents : MonoBehaviour
{
    public static event Action<IronSourceError> onRewardedVideoAdShowFailedEvent;
    public static event Action onRewardedVideoAdOpenedEvent;
    public static event Action onRewardedVideoAdClosedEvent;
    public static event Action onRewardedVideoAdStartedEvent;
    public static event Action onRewardedVideoAdEndedEvent;
    public static event Action<IronSourcePlacement> onRewardedVideoAdRewardedEvent;
    public static event Action<IronSourcePlacement> onRewardedVideoAdClickedEvent;
    public static event Action<bool> onRewardedVideoAvailabilityChangedEvent;

    public static event Action<String> onRewardedVideoAdOpenedDemandOnlyEvent;
    public static event Action<String> onRewardedVideoAdClosedDemandOnlyEvent;
    public static event Action<String> onRewardedVideoAdLoadedDemandOnlyEvent;
    public static event Action<String> onRewardedVideoAdRewardedDemandOnlyEvent;
    public static event Action<String, IronSourceError> onRewardedVideoAdShowFailedDemandOnlyEvent;
    public static event Action<String> onRewardedVideoAdClickedDemandOnlyEvent;
    public static event Action<String, IronSourceError> onRewardedVideoAdLoadFailedDemandOnlyEvent;

    public static event Action onInterstitialAdReadyEvent;
    public static event Action<IronSourceError> onInterstitialAdLoadFailedEvent;
    public static event Action onInterstitialAdOpenedEvent;
    public static event Action onInterstitialAdClosedEvent;
    public static event Action onInterstitialAdShowSucceededEvent;
    public static event Action<IronSourceError> onInterstitialAdShowFailedEvent;
    public static event Action onInterstitialAdClickedEvent;

    public static event Action onInterstitialAdRewardedEvent;

    public static event Action<string> onInterstitialAdReadyDemandOnlyEvent;
    public static event Action<string> onInterstitialAdOpenedDemandOnlyEvent;
    public static event Action<string> onInterstitialAdClosedDemandOnlyEvent;
    public static event Action<string, IronSourceError> onInterstitialAdLoadFailedDemandOnlyEvent;
    public static event Action<string> onInterstitialAdClickedDemandOnlyEvent;
    public static event Action<string, IronSourceError> onInterstitialAdShowFailedDemandOnlyEvent;

    public static event Action<bool> onOfferwallAvailableEvent;
    public static event Action onOfferwallOpenedEvent;
    public static event Action<Dictionary<string, object>> onOfferwallAdCreditedEvent;
    public static event Action<IronSourceError> onGetOfferwallCreditsFailedEvent;
    public static event Action onOfferwallClosedEvent;
    public static event Action<IronSourceError> onOfferwallShowFailedEvent;

    public static event Action onBannerAdLoadedEvent;
    public static event Action onBannerAdLeftApplicationEvent;
    public static event Action onBannerAdScreenDismissedEvent;
    public static event Action onBannerAdScreenPresentedEvent;
    public static event Action onBannerAdClickedEvent;
    public static event Action<IronSourceError> onBannerAdLoadFailedEvent;

    public static event Action<string> onSegmentReceivedEvent;

    #if UNITY_ANDROID
        private IUnityRewardedVideo rewardedVideoAndroid;
        private IUnityInterstitial interstitialAndroid;
        private IUnityOfferwall offerwallAndroid;
        private IUnityBanner bannerAndroid;
        private IUnitySegment segmentAndroid;

    #endif


    void Awake()
    {
    #if UNITY_ANDROID
        this.rewardedVideoAndroid = (IUnityRewardedVideo)new IronSourceRewardedVideoAndroid();//sets this.rewardedVideoAndroid as listener for RV(Mediation& Demand Only) events in the bridge
        this.rewardedVideoAndroid.CreateRewardedVideo();//implement RV(Mediation & Demand Only) callbacks in the bridge
        registerRewardedVideoEvents();//subscribe to RV events from this.rewardedVideoAndroid
        registerRewardedVideoDemandOnlyEvents();//subscribe to RV Demand Only events from this.rewardedVideoAndroid
        this.interstitialAndroid = (IUnityInterstitial)new IronSourceInterstitialAndroid();//sets this.interstitialAndroid as listener for IS(Mediation& Demand Only& rewarded interstitial)  events in the bridge
        this.interstitialAndroid.CreateInterstitial();//implement IS(Mediation & Demand Only& rewarded Interstitial) callbacks in the bridge
        registerInterstitialEvents();//subscribe to interstitial events from this.interstitialAndroid
        registerInterstitialDemandOnlyEvents();//subscribe to interstitial Demand Only events from this.interstitialAndroid
        this.offerwallAndroid = (IUnityOfferwall)new IronSourceOfferwallAndroid();//sets this.offerwallAndroid as listener for Offerwall events in the bridge
        this.offerwallAndroid.CreateOfferwall();//implement OW callbacks in the bridge
        registerOfferwallEvents();//subscribe to OW events from this.offerwallAndroid
        this.bannerAndroid = (IUnityBanner)new IronSourceBannerAndroid();//sets banners  this.rewardedVideoAndroid as listener for RV(Mediation& Demand Only) in the bridge and implement Banner callbacks
        registerBannerEvents();//subscribe to banners events from this.bannerAndroid
        this.segmentAndroid = (IUnitySegment)new IronSourceSegmentAndroid();//sets this.segmentAndroid as listener for Segment events in the bridge
        this.segmentAndroid.CreateSegment();//implement onSegmentReceived callback in the bridge
        registerSegmentEvents();//subscribe to onSegmentRecieved event from this.bannerAndroid
#endif
        Debug.Log("IronSourceEvents Awake called");
        DontDestroyOnLoad(gameObject);                  //Makes the object not be destroyed automatically when loading a new scene.
    }


#if UNITY_IOS
    // *******************************iOS Rewarded Video Events *******************************

    public void onRewardedVideoAdShowFailed(string error)
    {
        if (onRewardedVideoAdShowFailedEvent != null)
        {
            IronSourceError ironSourceError = IronSourceUtils.getErrorFromErrorObject(error);
            onRewardedVideoAdShowFailedEvent(ironSourceError);
        }
    }

    public void onRewardedVideoAdClosed(string empty)
    {
        if (onRewardedVideoAdClosedEvent != null)
        {
            onRewardedVideoAdClosedEvent();
        }

    }

    public void onRewardedVideoAdOpened(string empty)
    {
        if (onRewardedVideoAdOpenedEvent != null)
        {
            onRewardedVideoAdOpenedEvent();
        }

    }

    public void onRewardedVideoAdStarted(string empty)
    {
        if (onRewardedVideoAdStartedEvent != null)
        {
            onRewardedVideoAdStartedEvent();
        }

    }

    public void onRewardedVideoAdEnded(string empty)
    {
        if (onRewardedVideoAdEndedEvent != null)
        {
            onRewardedVideoAdEndedEvent();
        }

    }

    public void onRewardedVideoAdRewarded(string description)
    {
        if (onRewardedVideoAdRewardedEvent != null)
        {
            IronSourcePlacement ssp = IronSourceUtils.getPlacementFromObject(description);
            onRewardedVideoAdRewardedEvent(ssp);
        }
    }

    public void onRewardedVideoAdClicked(string description)
    {
        if (onRewardedVideoAdClickedEvent != null)
        {
            IronSourcePlacement ssp = IronSourceUtils.getPlacementFromObject(description);
            onRewardedVideoAdClickedEvent(ssp);
        }

    }

    public void onRewardedVideoAvailabilityChanged(string stringAvailable)
    {
        bool isAvailable = (stringAvailable == "true") ? true : false;

        if (onRewardedVideoAvailabilityChangedEvent != null)
        {
            onRewardedVideoAvailabilityChangedEvent(isAvailable);
        }

    }

    // *******************************iOS RewardedVideo DemandOnly Events *******************************
    public void onRewardedVideoAdLoadedDemandOnly(string instanceId)
    {
        if (onRewardedVideoAdLoadedDemandOnlyEvent != null)
        {
            onRewardedVideoAdLoadedDemandOnlyEvent(instanceId);
        }
    }

    public void onRewardedVideoAdLoadFailedDemandOnly(string args)
    {
        if (onRewardedVideoAdLoadFailedDemandOnlyEvent != null && !String.IsNullOrEmpty(args))
        {
            List<object> argList = IronSourceJSON.Json.Deserialize(args) as List<object>;
            IronSourceError err = IronSourceUtils.getErrorFromErrorObject(argList[1]);
            string instanceId = argList[0].ToString();
            onRewardedVideoAdLoadFailedDemandOnlyEvent(instanceId, err);
        }
    }

    public void onRewardedVideoAdOpenedDemandOnly(string instanceId)
    {
        if (onRewardedVideoAdOpenedDemandOnlyEvent != null)
        {
            onRewardedVideoAdOpenedDemandOnlyEvent(instanceId);
        }
    }

    public void onRewardedVideoAdClosedDemandOnly(string instanceId)
    {
        if (onRewardedVideoAdClosedDemandOnlyEvent != null)
        {
            onRewardedVideoAdClosedDemandOnlyEvent(instanceId);
        }
    }

    public void onRewardedVideoAdRewardedDemandOnly(string instanceId)
    {
        if (onRewardedVideoAdRewardedDemandOnlyEvent != null)
        {
            onRewardedVideoAdRewardedDemandOnlyEvent(instanceId);
        }
    }

    public void onRewardedVideoAdShowFailedDemandOnly(string args)
    {
        if (onRewardedVideoAdShowFailedDemandOnlyEvent != null && !String.IsNullOrEmpty(args))
        {
            List<object> argList = IronSourceJSON.Json.Deserialize(args) as List<object>;
            IronSourceError err = IronSourceUtils.getErrorFromErrorObject(argList[1]);
            string instanceId = argList[0].ToString();
            onRewardedVideoAdShowFailedDemandOnlyEvent(instanceId, err);
        }
    }

    public void onRewardedVideoAdClickedDemandOnly(string instanceId)
    {
        if (onRewardedVideoAdClickedDemandOnlyEvent != null)
        {
            onRewardedVideoAdClickedDemandOnlyEvent(instanceId);
        }
    }

    // *******************************iOS Interstitial Events *******************************

    public void onInterstitialAdReady ()
    {
        if (onInterstitialAdReadyEvent != null)
            onInterstitialAdReadyEvent ();
    }

    
    public void onInterstitialAdLoadFailed (string description)
    {
        if (onInterstitialAdLoadFailedEvent != null) {
            IronSourceError sse = IronSourceUtils.getErrorFromErrorObject(description);
            onInterstitialAdLoadFailedEvent (sse);
        }
    }

    public void onInterstitialAdOpened (string empty)
    {
        if (onInterstitialAdOpenedEvent != null) {
            onInterstitialAdOpenedEvent ();
        }
    }
        
    public void onInterstitialAdClosed (string empty)
    {
        if (onInterstitialAdClosedEvent != null) {
            onInterstitialAdClosedEvent ();
        }
    }

    public void onInterstitialAdShowSucceeded (string empty)
    {
        if (onInterstitialAdShowSucceededEvent != null) {
            onInterstitialAdShowSucceededEvent ();
        }
    }
    
    public void onInterstitialAdShowFailed (string description)
    {
        if (onInterstitialAdShowFailedEvent != null) {
            IronSourceError sse = IronSourceUtils.getErrorFromErrorObject(description);
            onInterstitialAdShowFailedEvent (sse);
        }   
    }

    public void onInterstitialAdClicked (string empty)
    {
        if (onInterstitialAdClickedEvent != null) {
            onInterstitialAdClickedEvent ();
        }
    }

    // *******************************iOS Interstitial DemanOnly Events *******************************

        public void onInterstitialAdReadyDemandOnly (string instanceId)
    {
        if (onInterstitialAdReadyDemandOnlyEvent != null)
            onInterstitialAdReadyDemandOnlyEvent (instanceId);
    }

    
    public void onInterstitialAdLoadFailedDemandOnly (string args)
    {
        if (onInterstitialAdLoadFailedDemandOnlyEvent  != null && !String.IsNullOrEmpty(args)) {
            List<object> argList = IronSourceJSON.Json.Deserialize(args) as List<object>;
            IronSourceError err = IronSourceUtils.getErrorFromErrorObject(argList[1]);
            string instanceId = argList[0].ToString(); 
            onInterstitialAdLoadFailedDemandOnlyEvent  (instanceId, err);
        }
    }

    public void onInterstitialAdOpenedDemandOnly (string instanceId)
    {
        if (onInterstitialAdOpenedDemandOnlyEvent != null) {
            onInterstitialAdOpenedDemandOnlyEvent (instanceId);
        }
    }
        
    public void onInterstitialAdClosedDemandOnly (string instanceId)
    {
        if (onInterstitialAdClosedDemandOnlyEvent != null) {
            onInterstitialAdClosedDemandOnlyEvent(instanceId);
        }
    }

    
    public void onInterstitialAdShowFailedDemandOnly (string args)
    {
        if (onInterstitialAdShowFailedDemandOnlyEvent != null && !String.IsNullOrEmpty(args))
        {
            List<object> argList = IronSourceJSON.Json.Deserialize(args) as List<object>;
            IronSourceError err = IronSourceUtils.getErrorFromErrorObject(argList[1]);
            string instanceId = argList[0].ToString();
            onInterstitialAdShowFailedDemandOnlyEvent (instanceId, err);
        }   
    }

    public void onInterstitialAdClickedDemandOnly (string instanceId)
    {
        if (onInterstitialAdClickedDemandOnlyEvent != null) {
            onInterstitialAdClickedDemandOnlyEvent (instanceId);
        }
    }

    // *******************************iOS Rewarded Interstitial Events *******************************

    public void onInterstitialAdRewarded(string empty)
    {
        if (onInterstitialAdRewardedEvent != null)
        {
            onInterstitialAdRewardedEvent();
        }
    }

    // *******************************iOS Offerwall Events ******************************* 

    public void onOfferwallOpened(string empty)
    {
        if (onOfferwallOpenedEvent != null)
        {
            onOfferwallOpenedEvent();
        }
    }


    public void onOfferwallShowFailed(string description)
    {
        if (onOfferwallShowFailedEvent != null)
        {
            IronSourceError sse = IronSourceUtils.getErrorFromErrorObject(description);
            onOfferwallShowFailedEvent(sse);
        }
    }


    public void onOfferwallClosed(string empty)
    {
        if (onOfferwallClosedEvent != null)
        {
            onOfferwallClosedEvent();
        }
    }


    public void onGetOfferwallCreditsFailed(string description)
    {
        if (onGetOfferwallCreditsFailedEvent != null)
        {
            IronSourceError sse = IronSourceUtils.getErrorFromErrorObject(description);
            onGetOfferwallCreditsFailedEvent(sse);

        }
    }


    public void onOfferwallAdCredited(string json)
    {
        if (onOfferwallAdCreditedEvent != null)
            onOfferwallAdCreditedEvent(IronSourceJSON.Json.Deserialize(json) as Dictionary<string, object>);
    }


    public void onOfferwallAvailable(string stringAvailable)
    {
        bool isAvailable = (stringAvailable == "true") ? true : false;
        if (onOfferwallAvailableEvent != null)
            onOfferwallAvailableEvent(isAvailable);
    }

    // ******************************* iOS Banner Events *******************************    

    public void onBannerAdLoaded ()
    {
        if (onBannerAdLoadedEvent != null)
            onBannerAdLoadedEvent ();
    }

    public void onBannerAdLoadFailed (string description)
    {
        if (onBannerAdLoadFailedEvent != null) {
            IronSourceError sse = IronSourceUtils.getErrorFromErrorObject (description);
            onBannerAdLoadFailedEvent (sse);
        }
        
    }
    
    public void onBannerAdClicked ()
    {
        if (onBannerAdClickedEvent != null)
            onBannerAdClickedEvent ();
    }

    public void onBannerAdScreenPresented ()
    {
        if (onBannerAdScreenPresentedEvent != null)
            onBannerAdScreenPresentedEvent ();
    }

    public void onBannerAdScreenDismissed ()
    {
        if (onBannerAdScreenDismissedEvent != null)
            onBannerAdScreenDismissedEvent ();
    }

    public void onBannerAdLeftApplication ()
    {
        if (onBannerAdLeftApplicationEvent != null)
            onBannerAdLeftApplicationEvent ();
    }

    // ******************************* iOS Segment Events *******************************    

    public void onSegmentReceived (string segmentName)
    {
        if (onSegmentReceivedEvent != null)
            onSegmentReceivedEvent (segmentName);
    }

#endif
#if UNITY_ANDROID

    //subscribe to IronSourceInterstitialAndroid IS Mediation & rewarded Interstitial events and notify to subscribed events inside the app
    private void registerInterstitialEvents()
    {
        this.interstitialAndroid.OnInterstitialAdClicked += () =>
        {
            if (onInterstitialAdClickedEvent != null)
            {
                onInterstitialAdClickedEvent();
            }
        };

        this.interstitialAndroid.OnInterstitialAdReady += () =>
        {
            if (onInterstitialAdReadyEvent != null)
            {
                onInterstitialAdReadyEvent();
            }
        };

        this.interstitialAndroid.OnInterstitialAdClosed += () =>
        {
            if (onInterstitialAdClosedEvent != null)
            {
                onInterstitialAdClosedEvent();
            }
        };

        this.interstitialAndroid.OnInterstitialAdOpened += () =>
        {
            if (onInterstitialAdOpenedEvent != null)
            {
                onInterstitialAdOpenedEvent();
            }
        };

        this.interstitialAndroid.OnInterstitialAdLoadFailed += (ironsourceError) =>
        {
            if (onInterstitialAdLoadFailedEvent != null)
            {
                onInterstitialAdLoadFailedEvent(ironsourceError);
            }
        };

        this.interstitialAndroid.OnInterstitialAdShowFailed += (ironSourceError) =>
        {
            if (onInterstitialAdShowFailedEvent != null)
            {
                onInterstitialAdShowFailedEvent(ironSourceError);
            }
        };

        this.interstitialAndroid.OnInterstitialAdShowSucceeded += () =>
        {
            if (onInterstitialAdShowSucceededEvent != null)
            {
                onInterstitialAdShowSucceededEvent();
            }
        };
        this.interstitialAndroid.OnInterstitialAdRewarded += () =>
        {
            if (onInterstitialAdRewardedEvent != null)
            {
                onInterstitialAdRewardedEvent();
            }
        };

    }

    //subscribe to IronSourceInterstitialAndroid IS Demand Only events and notify to subscribed events inside the app
    private void registerInterstitialDemandOnlyEvents()
    {
        this.interstitialAndroid.OnInterstitialAdReadyDemandOnly += (instanceId) =>
        {
            if (onInterstitialAdReadyDemandOnlyEvent != null)
            {
                onInterstitialAdReadyDemandOnlyEvent(instanceId);
            }
        };

        this.interstitialAndroid.OnInterstitialAdClosedDemandOnly += (instanceId) =>
        {
            if (onInterstitialAdClosedDemandOnlyEvent != null)
            {
                onInterstitialAdClosedDemandOnlyEvent(instanceId);
            }
        };

        this.interstitialAndroid.OnInterstitialAdOpenedDemandOnly += (instanceId) =>
        {
            if (onInterstitialAdOpenedDemandOnlyEvent != null)
            {
                onInterstitialAdOpenedDemandOnlyEvent(instanceId);
            }
        };

        this.interstitialAndroid.OnInterstitialAdClickedDemandOnly += (instanceId) =>
        {
            if (onInterstitialAdClickedDemandOnlyEvent != null)
            {
                onInterstitialAdClickedDemandOnlyEvent(instanceId);
            }
        };

        this.interstitialAndroid.OnInterstitialAdLoadFailedDemandOnly += (instanceId, ironSourceError) =>
        {
            if (onInterstitialAdLoadFailedDemandOnlyEvent != null)
            {
                onInterstitialAdLoadFailedDemandOnlyEvent(instanceId, ironSourceError);
            }
        };

        this.interstitialAndroid.OnInterstitialAdShowFailedDemandOnly += (instanceId, ironSourceError) =>
        {
            if (onInterstitialAdShowFailedDemandOnlyEvent != null)
            {
                onInterstitialAdShowFailedDemandOnlyEvent(instanceId, ironSourceError);
            }
        };

    }
    //subscribe to IronSourceRewardedVideoAndroid RV Mediation events and notify to subscribed events inside the app
    private void registerRewardedVideoEvents()
    {
        this.rewardedVideoAndroid.OnRewardedVideoAdClicked += (IronSourcePlacement) =>
        {
            if (onRewardedVideoAdClickedEvent != null)
            {
                onRewardedVideoAdClickedEvent(IronSourcePlacement);
            }
        };
        this.rewardedVideoAndroid.OnRewardedVideoAdShowFailed += (IronSourceError) =>
        {
            if (onRewardedVideoAdShowFailedEvent != null)
            {
                onRewardedVideoAdShowFailedEvent(IronSourceError);
            }
        };
        this.rewardedVideoAndroid.OnRewardedVideoAdOpened += () =>
        {
            if (onRewardedVideoAdOpenedEvent != null)
            {
                onRewardedVideoAdOpenedEvent();
            }
        };
        this.rewardedVideoAndroid.OnRewardedVideoAdClosed += () =>
        {
            if (onRewardedVideoAdClosedEvent != null)
            {
                onRewardedVideoAdClosedEvent();
            }
        };
        this.rewardedVideoAndroid.OnRewardedVideoAdStarted += () =>
        {
            if (onRewardedVideoAdStartedEvent != null)
            {
                onRewardedVideoAdStartedEvent();
            }
        };
        this.rewardedVideoAndroid.OnRewardedVideoAdEnded += () =>
        {
            if (onRewardedVideoAdEndedEvent != null)
            {
                onRewardedVideoAdEndedEvent();
            }
        };
        this.rewardedVideoAndroid.OnRewardedVideoAdRewarded += (IronSourcePlacement) =>
        {
            if (onRewardedVideoAdRewardedEvent != null)
            {
                onRewardedVideoAdRewardedEvent(IronSourcePlacement);
            }
        };

        this.rewardedVideoAndroid.OnRewardedVideoAvailabilityChanged += (isAvailable) =>
        {
            if (onRewardedVideoAvailabilityChangedEvent != null)
            {
                onRewardedVideoAvailabilityChangedEvent(isAvailable);
            }
        };
    }

    //subscribe to IronSourceRewardedVideoAndroid RV Demand Only events and notify to subscribed events inside the app
    public void registerRewardedVideoDemandOnlyEvents()
    {

        this.rewardedVideoAndroid.OnRewardedVideoAdClosedDemandOnlyEvent += (instanceId) =>
        {
            if (onRewardedVideoAdClosedDemandOnlyEvent != null)
            {
                onRewardedVideoAdClosedDemandOnlyEvent(instanceId);
            }
        };

        this.rewardedVideoAndroid.OnRewardedVideoAdLoadedDemandOnlyEvent += (instanceId) =>
        {
            if (onRewardedVideoAdLoadedDemandOnlyEvent != null)
            {
                onRewardedVideoAdLoadedDemandOnlyEvent(instanceId);
            }
        };

        this.rewardedVideoAndroid.OnRewardedVideoAdOpenedDemandOnlyEvent += (instanceId) =>
        {
            if (onRewardedVideoAdOpenedDemandOnlyEvent != null)
            {
                onRewardedVideoAdOpenedDemandOnlyEvent(instanceId);
            }
        };

        this.rewardedVideoAndroid.OnRewardedVideoAdClickedDemandOnlyEvent += (instanceId) =>
        {
            if (onRewardedVideoAdClickedDemandOnlyEvent != null)
            {
                onRewardedVideoAdClickedDemandOnlyEvent(instanceId);
            }
        };

        this.rewardedVideoAndroid.OnRewardedVideoAdRewardedDemandOnlyEvent += (instanceId) =>
        {
            if (onRewardedVideoAdRewardedDemandOnlyEvent != null)
            {
                onRewardedVideoAdRewardedDemandOnlyEvent(instanceId);
            }
        };

        this.rewardedVideoAndroid.OnRewardedVideoAdLoadFailedDemandOnlyEvent += (instanceId, error) =>
        {
            if (onRewardedVideoAdLoadFailedDemandOnlyEvent != null)
            {
                onRewardedVideoAdLoadFailedDemandOnlyEvent(instanceId, error);
            }
        };

        this.rewardedVideoAndroid.OnRewardedVideoAdShowFailedDemandOnlyEvent += (instanceId, error) =>
        {
            if (onRewardedVideoAdShowFailedDemandOnlyEvent != null)
            {
                onRewardedVideoAdShowFailedDemandOnlyEvent(instanceId, error);
            }
        };
    }
    //subscribe to IronSourceOfferwallAndroid OW events and notify to subscribed events inside the app

    void registerOfferwallEvents()
    {

        this.offerwallAndroid.OnOfferwallOpened += () =>
        {
            if (onOfferwallOpenedEvent != null)
            {
                onOfferwallOpenedEvent();
            }
        };

        this.offerwallAndroid.OnOfferwallShowFailed += (error) =>
        {
            if(onOfferwallShowFailedEvent != null)
            {
                onOfferwallShowFailedEvent(error);
            }

        };

        this.offerwallAndroid.OnOfferwallClosed += () =>
        {
            if (onOfferwallClosedEvent != null)
            {
                onOfferwallClosedEvent();
            }
        };

        this.offerwallAndroid.OnOfferwallAvailable += (isAvailable) =>
        {
            if(onOfferwallAvailableEvent != null)
            {
                onOfferwallAvailableEvent(isAvailable);
            }
        };

        this.offerwallAndroid.OnOfferwallAdCredited += (dic) =>
        {
            if(onOfferwallAdCreditedEvent != null)
            {
                onOfferwallAdCreditedEvent(dic);
            }
        };

        this.offerwallAndroid.OnGetOfferwallCreditsFailed += (error) =>
        {
            if(onGetOfferwallCreditsFailedEvent != null)
            {
                onGetOfferwallCreditsFailedEvent(error);
            }
        };

    }
    //subscribe to IronSourceBannerAndroid banner events and notify to subscribed events inside the app

    private void registerBannerEvents()
    {
        this.bannerAndroid.OnBannerAdLoaded += () =>
        {
            if(onBannerAdLoadedEvent != null){
                onBannerAdLoadedEvent();
            }

        };

        this.bannerAndroid.OnBannerAdClicked += () =>
        {
            if(onBannerAdClickedEvent != null){
                onBannerAdClickedEvent();
            }
        };

        this.bannerAndroid.OnBannerAdLoadFailed += (ironSourceError) =>
        {
            if(onBannerAdLoadFailedEvent != null){

                onBannerAdLoadFailedEvent(ironSourceError);
            }
        };

        this.bannerAndroid.OnBannerAdLeftApplication += () =>
        {
            if (onBannerAdLeftApplicationEvent != null)
            {
                onBannerAdLeftApplicationEvent();
            }
        };

        this.bannerAndroid.OnBannerAdScreenDismissed += () =>
        {
            if (onBannerAdScreenDismissedEvent != null)
            {
                onBannerAdScreenDismissedEvent();
            }
        };

        this.bannerAndroid.OnBannerAdScreenPresented += () =>
        {
            if (onBannerAdScreenPresentedEvent != null)
            {
                onBannerAdScreenPresentedEvent();
            }
        };
    }
    //subscribe to IronSourceSegmentAndroid onSegmentRecieved event and notify to subscribed event inside the app

    private void registerSegmentEvents()
    {
        this.segmentAndroid.OnSegmentRecieved += (segmentName) =>
        {
            if (onSegmentReceivedEvent != null)
            {
                onSegmentReceivedEvent(segmentName);
            }
        };
    }
#endif

}
