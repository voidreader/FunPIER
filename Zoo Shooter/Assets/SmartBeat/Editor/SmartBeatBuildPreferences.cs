using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartBeatBuildPreferences : ScriptableObject
{
    [SerializeField]
    public bool androidSoUpload = false;
    [SerializeField]
    public string androidApiKey = "API key for Android";
    [SerializeField]
    public string androidApiToken = "API token for Android";
}
