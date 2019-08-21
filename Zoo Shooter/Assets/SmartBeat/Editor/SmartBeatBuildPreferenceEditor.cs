using UnityEngine;
using UnityEditor;

#if UNITY_5_4_OR_NEWER || UNITY_5_3_5 || UNITY_5_3_6 || UNITY_5_3_7 || UNITY_5_3_8

[CustomEditor(typeof(SmartBeatBuildPreferences))]
public class SmartBeatBuildPreferenceEditor : Editor
{

    private SmartBeatBuildPreferences pref;
    private static string TOOLTIP_API_KEY = "The API key provided from SmartBeat console.";
    private static string TOOLTIP_API_TOKEN = "The API token provided from SmartBeat console.";
    private static string TOOLTIP_UPLOAD = "Upload Android .SO symbols for application build.";

    public void OnEnable()
    {
        pref = (SmartBeatBuildPreferences)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("SDK Version", SmartBeatPreferenceEditor.SDK_VER_UNITY);
        EditorGUILayout.Space();

        pref.androidSoUpload = EditorGUILayout.Toggle(new GUIContent("Enable Symbol Upload [?]", TOOLTIP_UPLOAD), pref.androidSoUpload);
        EditorGUILayout.Space();
        pref.androidApiKey = EditorGUILayout.TextField(new GUIContent("Android API Key [?]", TOOLTIP_API_KEY), pref.androidApiKey);
        pref.androidApiToken = EditorGUILayout.TextField(new GUIContent("Android API Token [?]", TOOLTIP_API_TOKEN), pref.androidApiToken);

        EditorGUILayout.Space();
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(pref);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
