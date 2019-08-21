using UnityEngine;

public class SmartBeatPreferences : ScriptableObject {

	[SerializeField]
	public bool iosShowSetting = true;
	[SerializeField]
	public string iosApiKey = "API key for iOS";
	[SerializeField]
	public bool iosScreenshot = false;
	[SerializeField]
	public bool iosLog = false;
	[SerializeField]
	public bool iosDebugLog = false;
	[SerializeField]
	public string iosLogRedirect = "";
	[SerializeField]
	public bool iosSIGSEGVDetection = false;

	[SerializeField]
	public bool androidShowSetting = true;
	[SerializeField]
	public string androidApiKey = "API key for Android";
	[SerializeField]
	public bool androidScreenshot = false;
	[SerializeField]
	public bool androidLog = false;
	[SerializeField]
	public string androidDebugLog = "";
	[SerializeField]
	public string androidLogRedirect = "";
	[SerializeField]
	public bool androidSIGSEGVDetection = false;
}
