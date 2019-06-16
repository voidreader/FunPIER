using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager
{
    private Action<bool> _onFinish;

    public void Init(string id)
    {
        Advertisement.Initialize(id.CorrectString(), false);
    }

    public static bool IsRewardedAdReady()
    {
        return Advertisement.IsReady("rewardedVideo");
    }

    public void ShowRewardedAd(Action<bool> onFinish = null)
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            AdManager.IsShowingAdsReturn = true;
//            _onFailed = null;
            _onFinish = null;
//            _onSkipped = null;

//            _onFailed = onFailed;
            _onFinish = onFinish;
//            _onSkipped = onSkipped;
            var options = new ShowOptions {resultCallback = HandleShowResult};
            Advertisement.Show("rewardedVideo", options);
        }
        else
            Debug.Log("Video is unavaiable!");
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");

                if (_onFinish != null)
                    _onFinish(true);
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");

                if (_onFinish != null)
                    _onFinish(false);
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");

                break;
        }
    }

    public bool IsVideoAvailable()
    {
        return Advertisement.IsReady();
    }
}