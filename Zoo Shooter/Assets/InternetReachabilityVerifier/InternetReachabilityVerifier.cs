// InternetReachabilityVerifier
//
// Helper class for verifying actual connectivity to Internet.
// Implements various "captive portal detection" methods using Unity.
//
// Copyright 2014-2019 Jetro Lauha (Strobotnik Ltd)
// http://strobotnik.com
// http://jet.ro
//
// $Revision: 1340 $
//
// File version history:
// 2014-06-18, 1.0.0 - Initial version
// 2014-07-22, 1.0.1 - Added Apple2 method (now default for Apple platforms).
// 2014-09-15, 1.0.2 - Refined customMethodExpectedData check to support
//                     expected empty result. Added option to append use a
//                     "cache buster" query string when using custom method.
//                     Added getTimeWithoutInternetConnection().
// 2015-03-26, 1.0.3 - Support for Unity 5. Made DontDestroyOnLoad optional.
//                     Also start/stop netActivity in OnEnable/OnDisable.
// 2016-08-27, 1.1.0 - Changed internal coroutine to wait using realtime
//                     instead of pauseable normal time. Also the 
//                     getTimeWithoutInternetConnection is now realtime-based.
//                     Added forceReverification method which sets status
//                     to pending verification, which is also now internally
//                     noticed immediately. Exposed the time value settings to
//                     inspector. Added AppleHTTPS method (due to ATS) and
//                     waitForNetVerifiedStatus convenience helper coroutine.
//                     Fixed regression bug of running verification twice.
// 2017-04-06, 1.1.1 - Added tooltips.
// 2017-04-12, 1.1.2 - Hotfix for Apple methods.
// 2018-03-06, 1.1.3 - Fixed deprecation warning with latest Unity versions.
//                     Made responseHeaders related code bit more robust.
// 2019-01-29, 1.2.0 - Use UnityWebRequest with Unity 2018.3+. Added new
//                     detection methods. Verified WebGL&Facebook support.

#define DEBUG_ERRORS
#if UNITY_EDITOR
//#define DEBUG_LOGS
#define DEBUG_WARNINGS
#define DEBUG_ERRORS
#endif

// If you want to use UnityWebRequest with earlier Unity versions, you can comment-out Unity version >2018.3 check
#if UNITY_2018_3_OR_NEWER
#define IRV_USE_WEBREQUEST
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if IRV_USE_WEBREQUEST
using IRVWebRequest = UnityEngine.Networking.UnityWebRequest;
#else
using IRVWebRequest = UnityEngine.WWW;
#endif

public class InternetReachabilityVerifier : MonoBehaviour
{
    const int version = 1200;

    //! Method to detect if network only reaches a "captive portal".
    /*! DefaultByPlatform picks the "platform provider" method,
     * e.g. Android->Google204, iOS->AppleHTTPS, Windows->MicrosoftNCSI.
     */
    public enum CaptivePortalDetectionMethod
    {
        DefaultByPlatform = 0, //!< Use "native" method for current platform.
        Google204, //!< Google's "HTTP result 204" method used by e.g. Android.
        GoogleBlank, //!< Google's fallback method, a blank page from google.com.
        MicrosoftNCSI, //!< Microsoft Network Connectivity Status Indicator check.
        Apple, //!< Fetch html page from apple.com with word "Success".
        Ubuntu, //!< Ubuntu connectivity check, returns Lorem ipsum.
        Custom, //!< Test using your self-hosted server. Both HTTP or HTTPS are fine.
        Apple2, //!< Like Apple method but use captive.apple.com and random path.
        AppleHTTPS, //!< Same as Apple but fetch using HTTPS.
        Google204HTTPS, //!< Same as Google204 but fetch using HTTPS.
        UbuntuHTTPS, //!< Same as Ubuntu but fetch using HTTPS.
        MicrosoftConnectTest, //!< Microsoft's alternative connect tester. Note: this one seems to set Access-Control-Allow-Origin.
        MicrosoftNCSI_IPV6, //!< Same as MicrosoftNCSI but works only in IPV6 networks.
        MicrosoftConnectTest_IPV6, //!< Same as MicrosoftConnectTest but works only in IPV6 networks.
    };

    public CaptivePortalDetectionMethod captivePortalDetectionMethod = CaptivePortalDetectionMethod.DefaultByPlatform;
    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Self-hosted URL for using CaptivePortalDetectionMethod.Custom. For example: https://example.com/IRV.txt")]
    #endif
    public string customMethodURL = "";
    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Data expected from the custom self-hosted URL. By default the data returned by the custom url is expected to start with contents of this string. Alternatively you can set the customMethodVerifierDelegate (see example code), in which case this string will be passed to the delegate.")]
    #endif
    public string customMethodExpectedData = "OK";
    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Makes the IRV object not be destroyed automatically when loading a new scene.")]
    #endif
    public bool dontDestroyOnLoad = true;
    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("When enabled, custom method URL is appended with a query string containing a random number.\nExample of what such a query string may look like: ?z=13371337")]
    #endif
    public bool customMethodWithCacheBuster = true;
    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Default time in seconds to wait until trying to verify network connectivity again.\nSuggested minimum: 1 second.")]
    #endif
    public float defaultCheckPeriod = 4.0f;
    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Time in seconds to wait before retrying, after last verification attempt resulted in an error.\nSuggested minimum: 3 seconds.")]
    #endif
    public float errorRetryDelay = 15.0f;
    #if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
    [Tooltip("Time in seconds to wait after detecting a captive portal (WiFi login screen).\nSuggested minimum: 2 seconds.")]
    #endif
    public float mismatchRetryDelay = 7.0f;

    [HideInInspector]
    public bool alwaysUseCacheBuster = false;

    public enum Status
    {
        Offline,             // no network connectivity / no internet access
        PendingVerification, // have network connectivity, internet access is being verified
        Error,               // error trying to verify internet access, will retry shortly
        Mismatch,            // captive portal detected (e.g. wifi login screen), will retry shortly
        NetVerified          // internet access is verified and functional
    }

    public delegate void StatusChangedDelegate(Status newStatus);
    public event StatusChangedDelegate statusChangedDelegate = null;

#if IRV_USE_WEBREQUEST
    public delegate bool CustomMethodVerifierDelegate(UnityEngine.Networking.UnityWebRequest wr, string customMethodExpectedData);
#else
    public delegate bool CustomMethodVerifierDelegate(UnityEngine.WWW www, string customMethodExpectedData);
#endif
    public CustomMethodVerifierDelegate customMethodVerifierDelegate = null;

    float noInternetStartTime = 0;

    Status _status = Status.Offline;
    public Status status
    {
        get
        {
            return _status;
        }
        set
        {
            Status prevStatus = _status;
            _status = value;
            if (prevStatus == Status.NetVerified && _status != Status.NetVerified)
                noInternetStartTime = Time.realtimeSinceStartup;
            if (statusChangedDelegate != null)
                statusChangedDelegate(value);
        }
    }

    string _lastError = "";
    public string lastError
    {
        get
        {
            return _lastError;
        }
        set
        {
            _lastError = value;
        }
    }

    private static InternetReachabilityVerifier _instance = null;
    public static InternetReachabilityVerifier Instance { get { return _instance; } }


    static RuntimePlatform[] methodGoogle204Supported = new RuntimePlatform[]
    {
        RuntimePlatform.WindowsPlayer,
        RuntimePlatform.WindowsEditor,
        RuntimePlatform.Android,
        RuntimePlatform.LinuxPlayer,
        RuntimePlatform.OSXPlayer,
        RuntimePlatform.OSXEditor,
    };

    const CaptivePortalDetectionMethod fallbackMethodIfNoDefaultByPlatform = CaptivePortalDetectionMethod.MicrosoftNCSI;

    bool netActivityRunning = false;

    string apple2MethodURL = "";


    //! Returns how long app has been without internet connection (time in seconds).
    /*! Returns 0 when online (internet connection is supposedly available).
     */
    public float getTimeWithoutInternetConnection()
    {
        if (status == Status.NetVerified)
            return 0; // we're online
        else
            return Time.realtimeSinceStartup - noInternetStartTime; // time without internet in seconds
    }

#if UNITY_EDITOR
    [ContextMenu("Debug.Log Internet Reachability Verifier info")]
    void debugLogInfo()
    {
        Debug.Log("IRV status: " + status + 
                  ", time without internet connection: " +
                  getTimeWithoutInternetConnection() + " seconds");
    }
#endif


    //! Helper coroutine for waiting until status becomes NetVerified if it isn't already.
    /*! If status isn't already PendingVerification, will also force reverification first.
     */
    public IEnumerator waitForNetVerifiedStatus()
    {
        if (status != Status.NetVerified)
            forceReverification();
        while (status != Status.NetVerified)
            yield return null;
    }

    //! Sets net activity time wait periods.
    /*! There are reasonable defaults, so there is no need to call this
     *  at all unless you want to change the times.
     */
    public void setNetActivityTimes(float defaultCheckPeriodSeconds,
                                    float errorRetryDelaySeconds,
                                    float mismatchRetryDelaySeconds)
    {
#       if DEBUG_WARNINGS
        if (defaultCheckPeriodSeconds < 1.0f)
            Debug.LogWarning("IRV - custom defaultCheckPeriodSeconds is set to a very low value: " + defaultCheckPeriodSeconds, this);
        if (errorRetryDelaySeconds < 3.0f)
            Debug.LogWarning("IRV - custom errorRetryDelaySeconds is set to a very low value: " + errorRetryDelaySeconds, this);
        if (mismatchRetryDelaySeconds < 2.0f)
            Debug.LogWarning("IRV - custom mismatchRetryDelaySeconds is set to a very low value: " + mismatchRetryDelaySeconds, this);
#       endif
        defaultCheckPeriod = defaultCheckPeriodSeconds;
        errorRetryDelay = errorRetryDelaySeconds;
        mismatchRetryDelay = mismatchRetryDelaySeconds;
    }

    //! Requests that the internet access is verified again.
    /*! You should call this after your own networking calls start to return
     *  errors which indicate effective loss of network connectivity.
     * \note equivalent to forcing the status to PendingVerification.
     */
    public void forceReverification()
    {
        status = Status.PendingVerification;
    }


    string getCaptivePortalDetectionURL(CaptivePortalDetectionMethod cpdm)
    {
        string url = "";
        bool appendCacheBuster = false;
        if (cpdm == CaptivePortalDetectionMethod.Custom)
        {
            url = customMethodURL;
            if (customMethodWithCacheBuster)
                appendCacheBuster = true;
        }
        else if (cpdm == CaptivePortalDetectionMethod.Google204)
            url = "http://clients3.google.com/generate_204";
        else if (cpdm == CaptivePortalDetectionMethod.MicrosoftNCSI)
            url = "http://www.msftncsi.com/ncsi.txt";
        else if (cpdm == CaptivePortalDetectionMethod.GoogleBlank)
            url = "http://www.google.com/blank.html";
        else if (cpdm == CaptivePortalDetectionMethod.Apple)
            url = "http://www.apple.com/library/test/success.html";
        else if (cpdm == CaptivePortalDetectionMethod.Ubuntu)
            url = "http://start.ubuntu.com/connectivity-check";
        else if (cpdm == CaptivePortalDetectionMethod.Apple2)
        {
            if (apple2MethodURL.Length == 0)
            {
                apple2MethodURL = "http://captive.apple.com/";
                char[] path = new char[17];
                for (int a = 0; a < 17; ++a)
                    path[a] = (char)((int)'a' + Random.Range(0, 'z' - 'a' + 1));
                path[8] = '/';
                apple2MethodURL += new string(path);
#               if DEBUG_LOGS
                Debug.Log("IRV using apple2MethodURL: " + apple2MethodURL);
#               endif
            }
            url = apple2MethodURL;
        }
        else if (cpdm == CaptivePortalDetectionMethod.AppleHTTPS)
            url = "https://www.apple.com/library/test/success.html";
        else if (cpdm == CaptivePortalDetectionMethod.Google204HTTPS)
            url = "https://clients3.google.com/generate_204";
        else if (cpdm == CaptivePortalDetectionMethod.UbuntuHTTPS)
            url = "https://start.ubuntu.com/connectivity-check";
        else if (cpdm == CaptivePortalDetectionMethod.MicrosoftConnectTest)
            url = "http://www.msftconnecttest.com/connecttest.txt";
        else if (cpdm == CaptivePortalDetectionMethod.MicrosoftNCSI_IPV6)
            url = "http://ipv6.msftncsi.com/ncsi.txt";
        else if (cpdm == CaptivePortalDetectionMethod.MicrosoftConnectTest_IPV6)
            url = "http://ipv6.msftconnecttest.com/connecttest.txt";
        if (appendCacheBuster || alwaysUseCacheBuster)
            url += "?z=" + (Random.Range(0, 0x7fffffff) ^ 0x13377AA7);
        return url;
    }

    #region Internal IRVWebRequest helper wrapper methods (iwrGet)

    private long iwrGet_bytesDownloaded(IRVWebRequest iwr)
    {
#if IRV_USE_WEBREQUEST
        return (long)iwr.downloadedBytes;
#else
#if UNITY_3_5
        return iwr.size;
#else
        return iwr.bytesDownloaded;
#endif
#endif
    }

    private string iwrGet_text(IRVWebRequest iwr)
    {
#if IRV_USE_WEBREQUEST
        if (iwr == null || iwr.isNetworkError || iwr.downloadHandler == null)
            return "";
        return iwr.downloadHandler.text;
#else
        return iwr.text;
#endif
    }

    private byte[] iwrGet_bytes(IRVWebRequest iwr)
    {
#if IRV_USE_WEBREQUEST
        if (iwr.isNetworkError || iwr.downloadHandler == null)
            return new byte[0];
        return iwr.downloadHandler.data;
#else
        return iwr.bytes;
#endif
    }

    private Dictionary<string, string> iwrGet_responseHeaders(IRVWebRequest iwr)
    {
#if IRV_USE_WEBREQUEST
        return iwr.GetResponseHeaders();
#else
        return iwr.responseHeaders;
#endif
    }

    private string iwrGet_responseHeader(IRVWebRequest iwr, string key)
    {
#if IRV_USE_WEBREQUEST
        return iwr.GetResponseHeader(key);
#else
        Dictionary<string, string> responseHeaders = iwrGet_responseHeaders(iwr);
        if (responseHeaders.ContainsKey(key))
            return iwr.responseHeaders[key];
        return null;
#endif
    }

    private bool iwrGet_isError(IRVWebRequest iwr)
    {
#if IRV_USE_WEBREQUEST
        return iwr.isNetworkError || iwr.responseCode >= 400;
#else
        return iwr.error != null && iwr.error.Length > 0;
#endif
    }

    private string iwrGet_errorString(IRVWebRequest iwr)
    {
#if IRV_USE_WEBREQUEST
        if (iwr.isNetworkError)
            return iwr.error;
        else if (iwr.responseCode >= 400)
            return iwr.responseCode.ToString();
        else
            return null;
#else
        return iwr.error;
#endif
    }

    #endregion

    bool checkCaptivePortalDetectionResult(CaptivePortalDetectionMethod cpdm, IRVWebRequest iwr)
    {
        if (iwr == null)
        {
#           if DEBUG_WARNINGS
            Debug.LogWarning("IRV checkCaptivePortalDetectionResult - iwr is null!", this);
#           endif
            return false; // error
        }
#       if DEBUG_LOGS
        { // debug scope
            long bytesDownloaded = iwrGet_bytesDownloaded(iwr);
            string text = iwrGet_text(iwr);
            Debug.Log("IRV checkCaptivePortalDetectionResult cpdm:" + cpdm + ", size:" + bytesDownloaded + ", data:" + text, this);
            Dictionary<string, string> responseHeaders = iwrGet_responseHeaders(iwr);
            if (responseHeaders != null && responseHeaders.Keys != null && responseHeaders.Keys.Count > 0)
            {
                string hdrnfo = "IRV - " + responseHeaders.Keys.Count + " response headers:\n";
                foreach (string key in responseHeaders.Keys)
                    hdrnfo += key + ": " + responseHeaders[key] + "\n";
                Debug.Log(hdrnfo, this);
            }
        }
#       endif

        if (iwr.error != null && iwr.error.Length > 0)
            return false; // www ended up in error, can't be success

        switch (cpdm)
        {
            case CaptivePortalDetectionMethod.Custom:
#               if DEBUG_WARNINGS
                string cacheControl = iwrGet_responseHeader(iwr, "CACHE-CONTROL");
                if (cacheControl != null && cacheControl.Length > 0)
                {
                    Debug.LogWarning("IRV - Cache-Control header contents: " + cacheControl, this);
                    Debug.LogWarning("IRV - Warning, custom www response contains Cache-Control header - you should verify its contents. Recommendation is to have no caching or very short max-age.", this);
                }
#               endif
                if (customMethodVerifierDelegate != null)
                    return customMethodVerifierDelegate(iwr, customMethodExpectedData);
                else
                {
                    string text = iwrGet_text(iwr);
                    byte[] bytes = iwrGet_bytes(iwr);
                    if ((customMethodExpectedData.Length > 0 &&
                          text != null &&
                          text.StartsWith(customMethodExpectedData)) ||
                         (customMethodExpectedData.Length == 0 &&
                          (bytes == null || bytes.Length == 0)))
                        return true;
                }
                break;

            case CaptivePortalDetectionMethod.Google204:
            case CaptivePortalDetectionMethod.Google204HTTPS:
#               if IRV_USE_WEBREQUEST
                if (iwr.responseCode == 204)
                    return true;
#               else // legacy hacks for older unity versions:
                Dictionary<string,string> responseHeaders = iwrGet_responseHeaders(iwr);
                if (responseHeaders != null && responseHeaders.Keys != null && responseHeaders.Keys.Count > 0)
                {
                    string httpStatus = "";
                    if (responseHeaders.ContainsKey("STATUS")) // some platforms
                        httpStatus = responseHeaders["STATUS"];
                    else if (responseHeaders.ContainsKey("NULL")) // on android
                        httpStatus = responseHeaders["NULL"];
                    if (httpStatus.Length > 0)
                    {
                        if (httpStatus.IndexOf("204 No Content") >= 0)
                            return true;
                    }
                }
                else
                {
                    if (iwrGet_bytesDownloaded(iwr) == 0)
                    {
                        // Some versions of Unity WWW class implementation don't always give
                        // response headers. In that case (or if forcibly using Google204
                        // method in other platforms), we have to fall back to check the
                        // data size, same way how the GoogleBlank check works.
                        return true;
                    }
                }
#               endif // legacy
                break;

            case CaptivePortalDetectionMethod.GoogleBlank:
                if (iwrGet_bytesDownloaded(iwr) == 0)
                {
                    return true;
                }
                break;

            case CaptivePortalDetectionMethod.MicrosoftNCSI:
            case CaptivePortalDetectionMethod.MicrosoftNCSI_IPV6:
                if (iwrGet_text(iwr).StartsWith("Microsoft NCSI"))
                    return true;
                break;

            case CaptivePortalDetectionMethod.Apple:
            case CaptivePortalDetectionMethod.Apple2:
            case CaptivePortalDetectionMethod.AppleHTTPS:
                // returns a short html doc, do a semi-soft check for it
                string lowerText = iwrGet_text(iwr).ToLower();
                int bodySuccessPos = lowerText.IndexOf("<body>success</body>");
                int titleSuccessPos = lowerText.IndexOf("<title>success</title>");
                if ((bodySuccessPos >= 0 && bodySuccessPos < 500) ||
                    (titleSuccessPos >= 0 && titleSuccessPos < 500))
                    return true;
                break;

            case CaptivePortalDetectionMethod.Ubuntu:
            case CaptivePortalDetectionMethod.UbuntuHTTPS:
                // returns a whole html doc with lorem ipsum text,
                // let's use a smaller check for it (start of body)
                if (iwrGet_text(iwr).IndexOf("Lorem ipsum dolor sit amet") == 109)
                    return true;
                break;

            case CaptivePortalDetectionMethod.MicrosoftConnectTest:
            case CaptivePortalDetectionMethod.MicrosoftConnectTest_IPV6:
                if (iwrGet_text(iwr).StartsWith("Microsoft Connect Test"))
                    return true;
                break;

        }

        return false;
    }


    private float _yieldWaitStart = 0;

    private bool internal_yieldWait(float seconds)
    {
        if (_yieldWaitStart == 0)
            _yieldWaitStart = Time.realtimeSinceStartup;
        bool yieldWait = (Time.realtimeSinceStartup - _yieldWaitStart) < seconds;
        if (!yieldWait)
            _yieldWaitStart = 0;
        return yieldWait;
    }


    IEnumerator netActivity()
    {
        netActivityRunning = true;

        NetworkReachability prevUnityReachability = Application.internetReachability;

        if (Application.internetReachability != NetworkReachability.NotReachable)
            status = Status.PendingVerification;
        else
            status = Status.Offline;
        noInternetStartTime = Time.realtimeSinceStartup;

        while (netActivityRunning)
        {
#           if DEBUG_LOGS
            Debug.Log("IRV netActivity cycle, status: " + status, this);
#           endif

            if (status == Status.Error)
            {
                while (internal_yieldWait(errorRetryDelay) && status != Status.PendingVerification) yield return null;
                status = Status.PendingVerification;
            }
            else if (status == Status.Mismatch)
            {
                while (internal_yieldWait(mismatchRetryDelay) && status != Status.PendingVerification) yield return null;
                status = Status.PendingVerification;
            }

            NetworkReachability unityReachability = Application.internetReachability;
            if (prevUnityReachability != unityReachability)
            {
#               if DEBUG_LOGS
                Debug.Log("IRV unity reachability changed: " + unityReachability, this);
#               endif
                if (unityReachability != NetworkReachability.NotReachable)
                {
                    status = Status.PendingVerification;
                }
                else if (unityReachability == NetworkReachability.NotReachable)
                {
                    status = Status.Offline;
                }
                prevUnityReachability = Application.internetReachability;
            }

            if (status == Status.PendingVerification)
            {
                verifyCaptivePortalDetectionMethod();
                CaptivePortalDetectionMethod cpdm = this.captivePortalDetectionMethod;
                string url = getCaptivePortalDetectionURL(cpdm);
#               if DEBUG_LOGS
                Debug.Log("IRV - trying to verify internet access with method " + cpdm + " and url:" + url, this);
#               endif

#               if IRV_USE_WEBREQUEST
                IRVWebRequest iwr = IRVWebRequest.Get(url);
                yield return iwr.SendWebRequest();
#               else
                IRVWebRequest iwr = new IRVWebRequest(url);
                yield return iwr;
#               endif

                if (iwrGet_isError(iwr))
                {
                    lastError = iwrGet_errorString(iwr);

#                   if DEBUG_LOGS
                    Debug.Log("IRV www error: " + lastError, this);
#                   endif
#                   if DEBUG_WARNINGS
                    if (lastError.Contains("no crossdomain.xml"))
                    {
                        Debug.LogWarning("IRV www error: " + lastError, this);
                        Debug.LogWarning("See http://docs.unity3d.com/462/Documentation/Manual/SecuritySandbox.html", this);
                        Debug.LogWarning("You should also check WWW Security Emulation Host URL of Unity Editor in Edit->Project Settings->Editor", this);
                    }
#                   endif

                    status = Status.Error;
                    continue;
                }
                else // or no www error, verify result:
                {
                    bool success = checkCaptivePortalDetectionResult(cpdm, iwr);
                    if (success)
                    {
#                       if DEBUG_LOGS
                        Debug.Log("IRV net access verification success", this);
#                       endif
                        status = Status.NetVerified; // success
                    }
                    else
                    {
#                       if DEBUG_LOGS
                        Debug.Log("IRV net verification mismatch (network login screen?)", this);
#                       endif
                        status = Status.Mismatch;
                        continue;
                    }
                } // no www error, verify result
            } // netAccessStatus == NetAccessStatus.PendingVerification

            while (internal_yieldWait(defaultCheckPeriod) && status != Status.PendingVerification) yield return null;
        } // while true

        netActivityRunning = false;
        status = Status.PendingVerification;

    } // netActivity


    void Awake()
    {
        // prevent additional objects being created on level reloads
        if (_instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        _instance = this;
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }

    void verifyCaptivePortalDetectionMethod()
    {
        // if we're using DefaultByPlatform, figure out what's platform's
        // "native" method (platform-vendor-provided way), and switch to it
        // (or use fallback if there is no such thing)

        if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.DefaultByPlatform)
        {
#           if UNITY_STANDALONE_OSX || UNITY_DASHBOARD_WIDGET || UNITY_IPHONE || UNITY_IOS || UNITY_TVOS
            captivePortalDetectionMethod = CaptivePortalDetectionMethod.AppleHTTPS;
#           elif UNITY_STANDALONE_WIN
            captivePortalDetectionMethod = CaptivePortalDetectionMethod.MicrosoftNCSI;
#           elif UNITY_WEBPLAYER || UNITY_FLASH || UNITY_NACL || UNITY_FACEBOOK
            captivePortalDetectionMethod = CaptivePortalDetectionMethod.Custom;
#           elif UNITY_FACEBOOK && UNITY_WEBGL
            // Facebook platform with WebGL requires both HTTPS and Access-Control-Allow-Origin;
            // that combination is only available with custom server configured in the right way.
            captivePortalDetectionMethod = CaptivePortalDetectionMethod.Custom;
#           elif UNITY_WEBGL
            // WebGL platform needs Access-Control-Allow-Origin in the server
            // which seems to be set by MicrosoftConnectTest, so we can use it as default
            captivePortalDetectionMethod = CaptivePortalDetectionMethod.MicrosoftConnectTest;
#           elif UNITY_ANDROID
            captivePortalDetectionMethod = CaptivePortalDetectionMethod.Google204;
#           elif UNITY_STANDALONE_LINUX
            captivePortalDetectionMethod = CaptivePortalDetectionMethod.Ubuntu;
#           elif UNITY_METRO || UNITY_WP8 || UNITY_WP_8_1 || UNITY_WSA
            captivePortalDetectionMethod = CaptivePortalDetectionMethod.MicrosoftNCSI;
#           elif UNITY_BLACKBERRY
#           if DEBUG_WARNINGS
            Debug.LogWarning("IRV - " + Application.platform + " platform isn't supported/tested (however, it probably works - trying to use default fallback)", this);
#           endif
#           elif UNITY_WII || UNITY_PS3 || UNITY_PS4 || UNITY_XBOX360 || UNITY_XBOXONE || UNITY_TIZEN || UNITY_SAMSUNGTV
#           if DEBUG_WARNINGS
            Debug.LogWarning("IRV - " + Application.platform + " platform isn't supported/tested", this);
#           endif
#           endif

            if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.DefaultByPlatform)
            {
                // there's no "native" one, use fallback
                captivePortalDetectionMethod = fallbackMethodIfNoDefaultByPlatform;
            }
#           if DEBUG_LOGS
            Debug.Log("IRV - default by platform selected, using " + captivePortalDetectionMethod + " method", this);
#           endif
        }

#if UNITY_WEBPLAYER || UNITY_FLASH || UNITY_NACL
        if (captivePortalDetectionMethod != CaptivePortalDetectionMethod.Custom)
        {
#           if UNITY_FLASH || UNITY_NACL
            if (Application.platform == RuntimePlatform.NaCl ||
                Application.platform == RuntimePlatform.FlashPlayer)
            {
#               if DEBUG_WARNINGS
                Debug.LogWarning("IRV - " + Application.platform + " platform isn't supported/tested (However, using the Custom method may work)", this);
#               endif
            }
#           endif // flash|nacl

#           if DEBUG_WARNINGS
            Debug.LogWarning("IRV - Web-based platform selected - forcing custom method! (" + Application.platform + ")", this);
#           endif
            captivePortalDetectionMethod = CaptivePortalDetectionMethod.Custom;
        }
#endif // webplayer|flash|nacl

#if UNITY_FACEBOOK && UNITY_WEBGL
        if (captivePortalDetectionMethod != CaptivePortalDetectionMethod.Custom)
        {
#           if DEBUG_WARNINGS
            Debug.LogWarning("IRV - Forcing custom method for Facebook&WebGL combination", this);
#           endif
            captivePortalDetectionMethod = CaptivePortalDetectionMethod.Custom;
        }
#endif

#if UNITY_WEBGL
        if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.MicrosoftConnectTest)
        {
            // We've defaulted to MicrosoftConnectTest w/WebGL, it's best to
            // use cache buster, otherwise browser may serve cached results.
            alwaysUseCacheBuster = true;
        }
#endif

        if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.Google204)
        {
            if (System.Array.IndexOf(methodGoogle204Supported, Application.platform) < 0)
            {
                // WWW impl. of current runtime can't check http status code
#               if DEBUG_WARNINGS
                Debug.LogWarning("IRV - Can't use Google204 method on " + Application.platform + ", using GoogleBlank as fallback", this);
#               endif
                captivePortalDetectionMethod = CaptivePortalDetectionMethod.GoogleBlank;
            }
        }

        if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.Custom &&
            customMethodURL.Length == 0)
        {
#           if DEBUG_ERRORS
            Debug.LogError("IRV - Custom method is selected but URL is empty, cannot start! (disabling component)", this);
#           endif
            this.enabled = false;
            if (netActivityRunning)
                Stop();
            return; // bail out - no URL to use!
        }
    }

    void Start()
    {
        verifyCaptivePortalDetectionMethod();

#       if DEBUG_WARNINGS
#       if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
        // Unity 4.3+
        bool notSupportedWarning = false;
#       if !UNITY_2017_3_OR_NEWER
        if (Application.platform == RuntimePlatform.TizenPlayer)
            notSupportedWarning = true;
#       endif // <2017.3
#       if !UNITY_4_3
        // Unity 4.5+:
        if (Application.platform == RuntimePlatform.PS4 ||
#           if !UNITY_2018_3_OR_NEWER
            Application.platform == RuntimePlatform.PSP2 ||
#           endif // <2018.3
#           if !UNITY_2017_3_OR_NEWER
            Application.platform == RuntimePlatform.SamsungTVPlayer ||
#           endif // <2017.3
            Application.platform == RuntimePlatform.XboxOne)
            notSupportedWarning = true;
#       endif // 4.5+

#       if UNITY_4_5 || UNITY_4_6
        // Strictly Unity 4.5 or 4.6:
        if (Application.platform == RuntimePlatform.PSMPlayer)
            notSupportedWarning = true;
#       endif // 4.5 or 4.6

#       if UNITY_5_0
        // Strictly Unity 5.0:
        if (Application.platform == RuntimePlatform.PSM)
            notSupportedWarning = true;
#       endif // 5.0

#       if (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER) && !UNITY_2018_1_OR_NEWER
        if (Application.platform == RuntimePlatform.WiiU)
            notSupportedWarning = true;
#       endif

#       if UNITY_5_5_OR_NEWER
        if (Application.platform == RuntimePlatform.Switch)
            notSupportedWarning = true;
#       endif

        if (notSupportedWarning)
            Debug.LogWarning("IRV - " + Application.platform + " platform isn't supported/tested", this);
#       endif // unity 4.3+
#       endif // debug_warnings

        if (!netActivityRunning)
            StartCoroutine("netActivity");
    }

    void OnDisable()
    {
#       if DEBUG_LOGS
        Debug.Log("IRV - OnDisable", this);
#       endif
        Stop();
    }

    void OnEnable()
    {
#       if DEBUG_LOGS
        Debug.Log("IRV - OnEnable", this);
#       endif
        Start();
    }

    public void Stop()
    {
        StopCoroutine("netActivity");
        netActivityRunning = false;
    }

} // InternetReachabilityVerifier
