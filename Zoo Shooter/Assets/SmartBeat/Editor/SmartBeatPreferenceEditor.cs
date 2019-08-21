using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(SmartBeatPreferences))]
public class SmartBeatPreferenceEditor : Editor
{

    SmartBeatPreferences pref;
    private static string BASE_PATH = "Assets" + Path.DirectorySeparatorChar + "SmartBeat";
    private static string RESOURCE_DIR = "Resources";
    private static string ASSET_NAME = "SmartBeat.asset";
    public static string BUILD_ASSET_NAME = "SmartBeatBuild.asset";
    public static string ASSET_PATH = BASE_PATH + Path.DirectorySeparatorChar + RESOURCE_DIR + Path.DirectorySeparatorChar + ASSET_NAME;

    // Not included in built package
    private static string EDITOR_DIR = "Editor Default Resources";
    public static string EDITOR_ASSET_PATH = "Assets" + Path.DirectorySeparatorChar + EDITOR_DIR + Path.DirectorySeparatorChar + BUILD_ASSET_NAME;

	//Tooltip messages
	private static string TOOLTIP_API_KEY = "The API key provided from SmartBeat console.";
	private static string TOOLTIP_SCREENSHOT = "Set enable to take screenshot when error occurs.";
	private static string TOOLTIP_LOG_IOS = "Set enbale to send SBLog/NSLog with error information.";
	private static string TOOLTIP_LOG_ANDROID = "Set enbale to send Logcat with error information.";
	private static string TOOLTIP_DEBUG_LOG_IOS = "Set enable to out SmartBeat DebugLog to SBLog/NSLog.";
	private static string TOOLTIP_DEBUG_LOG_ANDROID = "Set tag to out SmartBeat DebugLog to Logcat.";
	private static string TOOLTIP_LOG_REDIRECT_IOS = "Set prefix to redirect UnityLog to SBLog/NSLog.";
	private static string TOOLTIP_LOG_REDIRECT_ANDROID = "Set prefix to redirect UnityLog to Logcat.";
	private static string TOOLTIP_SIGSEGV = "If SIGSEGV detection is enabled, the application may close when NullReferenceException is occurred but detect SIGSEGV (not only NullReferenceException cases).";

	public static string SDK_VER_UNITY = "1.18";

    public void OnEnable()
    {
        pref = (SmartBeatPreferences)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("SDK Version", SDK_VER_UNITY);
        EditorGUILayout.Space();

        pref.iosShowSetting = EditorGUILayout.Foldout(pref.iosShowSetting, "iOS");
        if (pref.iosShowSetting)
        {
            EditorGUI.indentLevel++;
            pref.iosApiKey = EditorGUILayout.TextField(new GUIContent("API Key [?]", TOOLTIP_API_KEY), pref.iosApiKey);
            pref.iosScreenshot = EditorGUILayout.Toggle(new GUIContent("Enable Screenshot [?]", TOOLTIP_SCREENSHOT), pref.iosScreenshot);
            pref.iosLog = EditorGUILayout.Toggle(new GUIContent("Enable Log [?]", TOOLTIP_LOG_IOS), pref.iosLog);
            pref.iosDebugLog = EditorGUILayout.Toggle(new GUIContent("SmartBeatDebugLog [?]", TOOLTIP_DEBUG_LOG_IOS), pref.iosDebugLog);
            pref.iosLogRedirect = EditorGUILayout.TextField(new GUIContent("Log Redirect Prefix [?]", TOOLTIP_LOG_REDIRECT_IOS), pref.iosLogRedirect);
            pref.iosSIGSEGVDetection = EditorGUILayout.Toggle(new GUIContent("SIGSEGV detection [?]", TOOLTIP_SIGSEGV), pref.iosSIGSEGVDetection);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();

        pref.androidShowSetting = EditorGUILayout.Foldout(pref.androidShowSetting, "Android");
        if (pref.androidShowSetting)
        {
            EditorGUI.indentLevel++;
            pref.androidApiKey = EditorGUILayout.TextField(new GUIContent("API Key [?]", TOOLTIP_API_KEY), pref.androidApiKey);
            pref.androidScreenshot = EditorGUILayout.Toggle(new GUIContent("Enable Screenshot [?]", TOOLTIP_SCREENSHOT), pref.androidScreenshot);
            pref.androidLog = EditorGUILayout.Toggle(new GUIContent("Enable Log [?]", TOOLTIP_LOG_ANDROID), pref.androidLog);
            pref.androidDebugLog = EditorGUILayout.TextField(new GUIContent("SmartBeatDebugLog [?]", TOOLTIP_DEBUG_LOG_ANDROID), pref.androidDebugLog);
            pref.androidLogRedirect = EditorGUILayout.TextField(new GUIContent("Log Redirect Prefix [?]", TOOLTIP_LOG_REDIRECT_ANDROID), pref.androidLogRedirect);
            pref.androidSIGSEGVDetection = EditorGUILayout.Toggle(new GUIContent("SIGSEGV detection [?]", TOOLTIP_SIGSEGV), pref.androidSIGSEGVDetection);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(pref);
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("SmartBeat/Preferences")]
    static void OpenPreferences()
    {
        //Load from existing asset
        SmartBeatPreferences asset = AssetDatabase.LoadAssetAtPath(ASSET_PATH, typeof(SmartBeatPreferences)) as SmartBeatPreferences;

        //if not exisiting, create new asset
        if (asset == null)
        {
            asset = CreateInstance<SmartBeatPreferences>();

            bool existsFolder = false;
#if UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#else
            //This API is supported from UNITY 5
            existsFolder = AssetDatabase.IsValidFolder(BASE_PATH + Path.DirectorySeparatorChar + RESOURCE_DIR);
#endif
            if (!existsFolder)
            {
                AssetDatabase.CreateFolder(BASE_PATH, RESOURCE_DIR);
            }
            AssetDatabase.CreateAsset(asset, ASSET_PATH);
            AssetDatabase.Refresh();
        }
        //set active to show editor
        Selection.activeObject = asset;
    }

    #if UNITY_5_4_OR_NEWER || UNITY_5_3_5 || UNITY_5_3_6 || UNITY_5_3_7 || UNITY_5_3_8
    [MenuItem("SmartBeat/Build Preferences")]
    static void OpenBuildPreferences()
    {
        //Load from existing asset
        SmartBeatBuildPreferences asset = AssetDatabase.LoadAssetAtPath(EDITOR_ASSET_PATH, typeof(SmartBeatBuildPreferences)) as SmartBeatBuildPreferences;

        //if not exisiting, create new asset
        if (asset == null)
        {
            asset = CreateInstance<SmartBeatBuildPreferences>();

            bool existsFolder = false;
#if UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#else
            //This API is supported from UNITY 5
            existsFolder = AssetDatabase.IsValidFolder("Assets" + Path.DirectorySeparatorChar + EDITOR_DIR);
#endif
            if (!existsFolder)
            {
                AssetDatabase.CreateFolder("Assets", EDITOR_DIR);
            }
            // Not included in built package
            AssetDatabase.CreateAsset(asset, EDITOR_ASSET_PATH);
            AssetDatabase.Refresh();
        }
        //set active to show editor
        Selection.activeObject = asset;
    }
    #endif

	[MenuItem("SmartBeat/Open SmartBeat")]
	static void OpenConsole()
	{
		Application.OpenURL ("https://dash.smrtbeat.com");
	}

	[MenuItem("SmartBeat/User Manual")]
	static void OpenDocuments()
	{
		Application.OpenURL ("http://smrtbeat.com/docs/link/unity-sdk/?system_lang=" + Application.systemLanguage);
	}
}
