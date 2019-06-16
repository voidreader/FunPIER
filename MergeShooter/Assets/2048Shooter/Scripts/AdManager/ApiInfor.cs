using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ApiInfor
{
    [Space(10)] public bool IsUseBanner;
    public bool IsUseInterstitial;
    public bool IsUseReward;

    public string AppId;

    [Space(10)] public string BannerId;
    public string InterstitialId;
    public string RewardId;

    [Space(10)] public string UnityId;
    [Space(10)] public int InterstitialInterval = 120;
    public int NumTryLoad = 0;

    [Space(10)] public List<string> TestDevices = new List<string>();
    public List<string> KeyWords = new List<string>();
    [Space(10)] public MaxAdContentRating MaxAdContentRating;
}

[Serializable]
public class AdsInfor
{
}

public enum MaxAdContentRating
{
    None,
    G,
    PG,
    T,
    MA,
}