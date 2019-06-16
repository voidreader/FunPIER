using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using Random = System.Random;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;
    public string Prefix;
    public static bool IsShowingAdsReturn;
    public static bool IsBannerShowing = true;
    public static Action OnFirebaseInited;
    public static Action OnFetchCompleted;
    public static Action<Dictionary<string, object>> OnSetDefault;

    public static bool IsRemoveAds
    {
        get { return PlayerPrefs.GetInt("IsRemoveAds", 0) == 1; }
        set { PlayerPrefs.SetInt("IsRemoveAds", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public bool IsEnableDebug;

    [Header("Android Info")]
    public ApiInfor AndroidApiInfor;
    [Header("IOS Info")]
    public ApiInfor IosApiInfor;

    public AdmobManager Admob;
    public UnityAdsManager Unity;

    [HideInInspector] public ApiInfor ApiInfor;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (IsAndroid())
        {
            ApiInfor = AndroidApiInfor;
        }
        if (IsIOS())
        {
            ApiInfor = IosApiInfor;
        }
        InitAds();
    }


    private void InitAds()
    {

        Debug.Log("============= InitAds");

        Admob = new AdmobManager();
        Admob.InitAdmob(ApiInfor);

        if (ApiInfor.UnityId != string.Empty)
        {
            Unity = new UnityAdsManager();
            Unity.Init(ApiInfor.UnityId);
        }
    }

    public void ShowBanner()
    {
        IsBannerShowing = true;
        if (Admob != null)
            Admob.ShowBanner();
    }

    public void HideBanner()
    {
        IsBannerShowing = false;
        if (Admob != null)
            Admob.HideBanner();
    }

    public void ShowFull(Action onClosed)
    {
        if (IsRemoveAds)
        {
            if (onClosed != null)
                onClosed();

            return;
        }

        if (IsEditor())
        {
            if (onClosed != null)
                onClosed();
        }
        else
        {
            if (Admob != null)
                Admob.ShowInterstitial(onClosed);
            else if (onClosed != null)
                onClosed();
        }
    }

    public bool IsRewardLoaded()
    {
        if (IsEditor())
        {
            if (Unity != null)
                return Unity.IsVideoAvailable();
        }
        else
        {
            if (Admob != null)
            {
                if (Admob.IsRewardLoaded())
                    return true;
                if (Unity != null)
                    return Unity.IsVideoAvailable();
            }
        }

        return false;
    }

    public void ShowReward(Action<bool> onClosed)
    {
        if (IsEditor())
        {
            if (Unity != null)
                if (Unity.IsVideoAvailable())
                    Unity.ShowRewardedAd(onClosed);
        }
        else
        {
            if (Admob != null)
            {
                if (Admob.IsRewardLoaded())
                    Admob.ShowReward(onClosed);
                else if (Unity != null)
                {
                    if (Unity.IsVideoAvailable())
                        Unity.ShowRewardedAd(onClosed);
                }
            }
        }
    }


    public void ApiDebug(string content)
    {
        if (IsEnableDebug)
            Debug.Log(content);
    }

    public static bool IsEditor()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
            return true;
        return false;
    }

    public static bool IsAndroid()
    {
#if UNITY_ANDROID
        return true;
#endif
        return false;
    }

    public static bool IsIOS()
    {
#if UNITY_IOS
        return true;
#endif
        return false;
    }


}

public static class APIExtention
{
    public static string CorrectString(this string input)
    {
        return input.Replace("\r", string.Empty);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}