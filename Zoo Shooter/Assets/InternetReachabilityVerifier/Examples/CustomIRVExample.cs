// Custom method example for InternetReachabilityVerifier.
//
// Copyright 2014-2019 Jetro Lauha (Strobotnik Ltd)
// http://strobotnik.com
// http://jet.ro
//
// $Revision: 1336 $
//
// File version history:
// 2014-06-18, 1.0.0 - Initial version
// 2016-08-27, 1.1.0 - Minor changes.
// 2019-01-29, 1.2.0 - Use UnityWebRequest with Unity 2018.3+. Scale UI by dpi.

#if UNITY_2018_3_OR_NEWER
#define CUSTOMIRVEXAMPLE_USE_WEBREQUEST // Need to enable this when IRV_USE_WEBREQUEST inside InternetReachabilityVerifier.cs is enabled
using UnityEngine.Networking;
#endif

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InternetReachabilityVerifier))]
public class CustomIRVExample : MonoBehaviour
{
    InternetReachabilityVerifier irv;
    string log = "";

    void appendLog(string s, bool error = false)
    {
        log += s + "\n";
        if (error)
            Debug.LogError(s, this);
        else
            Debug.Log(s, this);
    }

#if CUSTOMIRVEXAMPLE_USE_WEBREQUEST
    bool verifyNetCheckData(UnityWebRequest wr, string customMethodExpectedData)
#else
    bool verifyNetCheckData(WWW www, string customMethodExpectedData)
#endif
    {
        // Example validation - require that given custom string is not empty
        // and that it appears at some place in the returned document.
        if (customMethodExpectedData == null ||
            customMethodExpectedData.Length == 0)
        {
            appendLog("Custom verifyNetCheckData - Null or empty customMethodExpectedData!");
            return false;
        }
#if CUSTOMIRVEXAMPLE_USE_WEBREQUEST
        string text = wr.downloadHandler.text;
#else
        string text = www.text;
#endif
        bool result = text.Contains(customMethodExpectedData);
        appendLog("Custom verifyNetCheckData - result:" + result + ", customMethodExpectedData:" + customMethodExpectedData + ", text:" + text);
        return result;
    }

    void netStatusChanged(InternetReachabilityVerifier.Status newStatus)
    {
        appendLog("Net status changed: " + newStatus);
        if (newStatus == InternetReachabilityVerifier.Status.Error)
        {
            string error = irv.lastError;
            appendLog("Error: " + error);
            if (error.Contains("no crossdomain.xml"))
            {
                appendLog("See http://docs.unity3d.com/462/Documentation/Manual/SecuritySandbox.html - You should also check WWW Security Emulation Host URL of Unity Editor in Edit->Project Settings->Editor");
            }
        }
    }

    void Start()
    {
        irv = GetComponent<InternetReachabilityVerifier>();
        irv.customMethodVerifierDelegate = verifyNetCheckData;
        irv.statusChangedDelegate += netStatusChanged;

        appendLog("CustomIRVExample log:\n");
        appendLog("Selected method: " + irv.captivePortalDetectionMethod);
        appendLog("Custom Method URL: " + irv.customMethodURL);
        appendLog("Custom Method Expected Data: " + irv.customMethodExpectedData);
        if (irv.customMethodVerifierDelegate != null)
            appendLog("Using custom method verifier delegate.");
        if (irv.customMethodURL.Contains("strobotnik.com"))
            appendLog("*** IMPORTANT WARNING: ***\nYou're using the default TEST value for Custom Method URL specified in example scene.\nTHAT SERVER HAS NO GUARANTEE OF BEING UP AND RUNNING.\nPlease use your own custom server and URL!\n*****\n", true);
    }

    Vector2 scrollPos;
    bool onGUIscaled = false;

    void OnGUI()
    {
        if (!onGUIscaled)
        {
            int scaler;
            if (Screen.width > Screen.height)
                scaler = Mathf.Max(1, (int)(Screen.height / 480.0f));
            else
                scaler = Mathf.Max(1, (int)(Screen.height / 800.0f));
            GUI.skin.label.fontSize = 14 * scaler;
            onGUIscaled = true;
        }

        GUI.color = new Color(0.9f, 0.95f, 1.0f);
        GUILayout.Label("Strobotnik InternetReachabilityVerifier for Unity");
        GUILayout.BeginHorizontal();
        GUI.color = new Color(0.7f, 0.8f, 0.9f);
        GUILayout.Label("Status: ");
        GUI.color = Color.white;
        GUILayout.Label("" + irv.status);
        GUILayout.EndHorizontal();

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.Label(log);
        GUILayout.EndScrollView();
    }
}
