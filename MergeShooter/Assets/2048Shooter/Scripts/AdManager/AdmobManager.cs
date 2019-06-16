using System;
using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;
using UnityEngine;

public class AdmobManager
{
    private ApiInfor _apiInfor;

    // BANNER
    private BannerView _bannerView;
    private bool _isBannerRequesting;
    private bool _isBannerLoaded;
    private int _countTryLoadBanner;

    // INTERSTITIALAD
    private InterstitialAd _interstitialAd;
    private bool _isInterstitialAdRequesting;
    private Action _onInterstitialClosed;
    private int _countTryLoadInterstitial;
    private float _lastTimeShowInterstitial;

    // REWARD
    private RewardBasedVideoAd rewardBasedVideo;
    private bool _isRewardRequesting;
    private bool _isRewardShowing;
    private bool _isInited;
    private Action<bool> _onRewardClosed;

    private int _countTryLoadReward;
    private bool _isRewarded;

    public void InitAdmob(ApiInfor infor)
    {
        _apiInfor = infor;

        if (infor.AppId != string.Empty)
        {
            infor.AppId.CorrectString();
            MobileAds.Initialize(infor.AppId);
        }

        if (_apiInfor.IsUseBanner && !AdManager.IsRemoveAds)
        {
            _apiInfor.BannerId.CorrectString();
            RequestBanner();
        }

        if (_apiInfor.IsUseInterstitial && !AdManager.IsRemoveAds)
        {
            _apiInfor.InterstitialId.CorrectString();
            RequestInterstitial();
            _lastTimeShowInterstitial = Time.time - infor.InterstitialInterval;
        }

        if (_apiInfor.IsUseReward)
        {
            _apiInfor.RewardId.CorrectString();
            RequestReward();
        }
    }

    #region BANNER

    private void RequestBanner()
    {
        if (_isBannerRequesting)
            return;

        LogBanner("request");

        if (AdManager.Instance.IsEnableDebug)
            Debug.Log("BannerId:" + _apiInfor.BannerId);

        _isBannerRequesting = true;
        _isBannerLoaded = false;
        if (_bannerView != null)
            _bannerView.Destroy();
        _bannerView = new BannerView(_apiInfor.BannerId, AdSize.Banner, AdPosition.Bottom);

        _bannerView.OnAdLoaded += (sender, args) =>
        {
            _isBannerRequesting = false;
            _isBannerLoaded = true;
            LogBanner("loaded");
            _countTryLoadBanner = 0;

            if (AdManager.IsRemoveAds || !AdManager.IsBannerShowing)
                HideBanner();
        };
        // Called when an ad request failed to load.
        _bannerView.OnAdFailedToLoad += (sender, args) =>
        {
            _isBannerLoaded = false;
            _isBannerRequesting = false;
            LogBanner("failed");

            if (AdManager.Instance.IsEnableDebug)
                Debug.Log("banner failed: " + args.Message);

            if (_countTryLoadBanner < _apiInfor.NumTryLoad)
            {
                _countTryLoadBanner++;
                LogBanner("try_load" + _countTryLoadBanner);
                if (AdManager.Instance.IsEnableDebug)
                    AdManager.Instance.ApiDebug("Banner try load " + _countTryLoadBanner + "/" + _apiInfor.NumTryLoad);
                RequestBanner();
            }
            else
            {
                _countTryLoadBanner = 0;
            }
        };
        // Called when an ad is clicked.
        _bannerView.OnAdOpening += (sender, args) => { LogBanner("opening"); };
        // Called when the user returned from the app after an ad click.
        _bannerView.OnAdClosed += (sender, args) => { LogBanner("closed"); };
        // Called when the ad click caused the user to leave the application.
        _bannerView.OnAdLeavingApplication += (sender, args) => { LogBanner("leaving"); };

        // Load the banner with the request.
        _bannerView.LoadAd(GetAdRequest());
    }

    public bool IsBannerLoaded()
    {
        return _isBannerLoaded;
    }

    public void ShowBanner()
    {
        if (_bannerView != null)
        {
            if (_isBannerLoaded)
                _bannerView.Show();
            else
                RequestBanner();
        }
    }

    public void HideBanner()
    {
        if (_bannerView != null)
            _bannerView.Hide();
    }

    #endregion

    #region INTERSTITIAL

    private void RequestInterstitial()
    {
//        Debug.Log("requesting " + _isInterstitialAdRequesting);
        if (_isInterstitialAdRequesting)
            return;

        LogInterstitial("request");

        if (AdManager.Instance.IsEnableDebug)
            Debug.Log("FullId:" + _apiInfor.InterstitialId);

        _isInterstitialAdRequesting = true;

        if (_interstitialAd != null)
            _interstitialAd.Destroy();

        _interstitialAd = new InterstitialAd(_apiInfor.InterstitialId);

        // Called when an ad request has successfully loaded.
        _interstitialAd.OnAdLoaded += (sender, args) =>
        {
            _isInterstitialAdRequesting = false;
            LogInterstitial("loaded");
            _countTryLoadInterstitial = 0;
        };
        // Called when an ad request failed to load.
        _interstitialAd.OnAdFailedToLoad += (sender, args) =>
        {
            _isInterstitialAdRequesting = false;
            LogInterstitial("failed");
            Debug.Log("interstitial failed: " + args.Message);

            if (_countTryLoadInterstitial < _apiInfor.NumTryLoad)
            {
                _countTryLoadInterstitial++;
                LogInterstitial("try_load" + _countTryLoadInterstitial);
                if (AdManager.Instance.IsEnableDebug)
                    AdManager.Instance.ApiDebug("Interstitial try load " + _countTryLoadInterstitial + "/" +
                                          _apiInfor.NumTryLoad);
                RequestInterstitial();
            }
            else
            {
                _countTryLoadInterstitial = 0;
            }
        };
        // Called when an ad is clicked.
        _interstitialAd.OnAdOpening += (sender, args) => { LogInterstitial("opening"); };
        // Called when the user returned from the app after an ad click.
        _interstitialAd.OnAdClosed += (sender, args) =>
        {
            LogInterstitial("closed");
            RequestInterstitial();

            AdManager.Instance.StartCoroutine(CompleteMethodInterstitial());
        };
        // Called when the ad click caused the user to leave the application.
        _interstitialAd.OnAdLeavingApplication += (sender, args) => { LogInterstitial("leaving"); };

        // Load the banner with the request.
        _interstitialAd.LoadAd(GetAdRequest());
    }

    IEnumerator CompleteMethodInterstitial()
    {
        yield return null;

        if (_onInterstitialClosed != null)
            _onInterstitialClosed();

        GameUIManager.Instance.PlayBGM();
    }

    public bool IsInterstitialAds()
    {
        if (_interstitialAd == null)
            return false;
        return _interstitialAd.IsLoaded();
    }

    public void ShowInterstitial(Action onClosed)
    {
        if (_interstitialAd == null)
        {
            if (onClosed != null)
                onClosed();
            return;
        }

        GameUIManager.Instance.StopBGM();

        if (_interstitialAd.IsLoaded())
        {
            if (CanShowFull())
            {
                _onInterstitialClosed = onClosed;
                _lastTimeShowInterstitial = Time.time;
                _interstitialAd.Show();
            }
            else
            {
                if (onClosed != null)
                    onClosed();
            }
        }
        else
        {
            if (onClosed != null)
                onClosed();
            RequestInterstitial();
        }
    }

    public bool CanShowFull()
    {
        return Time.time - _lastTimeShowInterstitial >= _apiInfor.InterstitialInterval;
    }

    #endregion

    #region REWARD

    private void RequestReward()
    {
        if (_isRewardRequesting || _isRewardShowing)
            return;
        _isRewarded = false;
        LogReward("request");

        if (AdManager.Instance.IsEnableDebug)
            Debug.Log("RewardID:" + _apiInfor.RewardId);

        _isRewardRequesting = true;

        if (!_isInited)
        {
            _isInited = true;

            this.rewardBasedVideo = RewardBasedVideoAd.Instance;

            // Called when an ad request has successfully loaded.
            rewardBasedVideo.OnAdLoaded += (sender, args) =>
            {
                _isRewardRequesting = false;
                LogReward("loaded");
                _countTryLoadReward = 0;

                if (!AdManager.IsBannerShowing)
                    HideBanner();
            };
            // Called when an ad request failed to load.
            rewardBasedVideo.OnAdFailedToLoad += (sender, args) =>
            {
                _isRewardRequesting = false;
                LogReward("failed");
                Debug.Log("reward failed: " + args.Message);

                if (_countTryLoadReward < _apiInfor.NumTryLoad)
                {
                    _countTryLoadReward++;
                    LogReward("try_load" + _countTryLoadReward);
                    if (AdManager.Instance.IsEnableDebug)
                        AdManager.Instance.ApiDebug("reward try load " + _countTryLoadReward + "/" + _apiInfor.NumTryLoad);
                    RequestReward();
                }
                else
                {
                    _countTryLoadReward = 0;
                }
            };
            // Called when an ad is clicked.
            rewardBasedVideo.OnAdOpening += (sender, args) => { LogReward("opening"); };
            // Called when the user returned from the app after an ad click.
            rewardBasedVideo.OnAdClosed += (sender, args) =>
            {
                AdManager.Instance.StartCoroutine(CompleteMethodRewardedVideo(_isRewarded));
                _isRewardShowing = false;
                LogReward("closed");
                RequestReward();
            };
            // Called when the ad click caused the user to leave the application.
            rewardBasedVideo.OnAdLeavingApplication += (sender, args) => { LogReward("leaving"); };
            rewardBasedVideo.OnAdRewarded += (sender, args) =>
            {
                LogReward("finish");
                _isRewarded = true;
            };
        }

        rewardBasedVideo.LoadAd(GetAdRequest(), _apiInfor.RewardId);
    }

    IEnumerator CompleteMethodRewardedVideo(bool isRewarded)
    {
        yield return null;
        if (_onRewardClosed != null)
        {
            GameUIManager.Instance.PlayBGM();

            _onRewardClosed(isRewarded);
            LogReward("rewarded");
        }
    }


    public bool IsRewardLoaded()
    {
        if (rewardBasedVideo.IsLoaded())
            return true;

        RequestReward();
        return false;
    }

    public void ShowReward(Action<bool> onClose)
    {
        if (rewardBasedVideo == null)
        {
            Debug.Log("rewardBasedVideo null");
            return;
        }

        GameUIManager.Instance.StopBGM();

        if (rewardBasedVideo.IsLoaded())
        {
            _isRewardShowing = true;
            AdManager.IsShowingAdsReturn = true;
            _onRewardClosed = onClose;
            rewardBasedVideo.Show();
        }
        else
            RequestReward();
    }

    #endregion

    private AdRequest GetAdRequest()
    {
        var request = new AdRequest.Builder();

        // Add test device
        foreach (var test in _apiInfor.TestDevices)
            if (test != string.Empty)
                request.AddTestDevice(test);

        // Add keyword
        foreach (var keyWord in _apiInfor.KeyWords)
            if (keyWord != string.Empty)
                request.AddKeyword(keyWord);

        if (_apiInfor.MaxAdContentRating != MaxAdContentRating.None)
            request.AddExtra("max_ad_content_rating", _apiInfor.MaxAdContentRating.ToString());

        return request.Build();
    }

    private void LogBanner(string paramaterValue)
    {
        LogApi("banner", paramaterValue);
    }

    private void LogInterstitial(string paramaterValue)
    {
        LogApi("interstitial", paramaterValue);
    }

    private void LogReward(string paramaterValue)
    {
        LogApi("reward", paramaterValue);
    }


    private void LogApi(string parameterName, string parameterValue)
    {
      //  FirebaseAnalytics.LogEvent("Ads", parameterName, parameterValue);
    }
}