using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using SA.Android.Editor;

namespace SA.Android.Editor
{
    class AN_SettingsTab : SA_GUILayoutElement
    {
        const string k_JarResolverDocLink = "https://unionassets.com/android-native-pro/unity-jar-resolver-669";
        const string k_StorageOptionsDocLink = "https://unionassets.com/android-native-pro/preferred-images-storage-828";
        const string k_UnityJarResolverTitle = "Unity Jar Resolver (EDM4U)";

        const int k_ButtonHeight = 18;

        [SerializeField]
        SA_PluginActiveTextLink m_JarResolverLink;

        [SerializeField]
        SA_PluginActiveTextLink m_StorageOptionsLink;

        readonly GUIContent m_Info = new GUIContent("Info[?]:", "Full communication logs between Native plugin part");
        readonly GUIContent m_Warnings = new GUIContent("Warnings[?]:", "Warnings");
        readonly GUIContent m_Errors = new GUIContent("Errors[?]:", "Errors");

        public override void OnAwake()
        {
            m_JarResolverLink = new SA_PluginActiveTextLink("[?] How to use");
            m_StorageOptionsLink = new SA_PluginActiveTextLink("[?] Learn More");
        }

        public override void OnGUI()
        {
            using (new SA_WindowBlockWithSpace(new GUIContent("Log Level")))
            {
                EditorGUILayout.HelpBox("We recommend you to keep full logging level while your project in development mode. " +
                    "Full communication logs between Native plugin part & " +
                    "Unity side will be only available with Info logging  level enabled. \n" +
                    "Disabling the error logs isn't recommended.", MessageType.Info);

                using (new SA_GuiBeginHorizontal())
                {
                    var logLevel = AN_Settings.Instance.LogLevel;
                    logLevel.Info = GUILayout.Toggle(logLevel.Info, m_Info, GUILayout.Width(80));
                    logLevel.Warning = GUILayout.Toggle(logLevel.Warning, m_Warnings, GUILayout.Width(100));
                    logLevel.Error = GUILayout.Toggle(logLevel.Error, m_Errors, GUILayout.Width(100));
                }

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("On some Android devices, Log.d or Log.e methods will not print anything to console," +
                    "so sometimes the only ability to see the logs is to enable the WTF printing. This will make all" +
                    "logs to be printed with Log.wtf method despite message log level.", MessageType.Info);

                AN_Settings.Instance.WTFLogging = GUILayout.Toggle(AN_Settings.Instance.WTFLogging, "WTF Logging", GUILayout.Width(130));
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Environment Management")))
            {
                EditorGUILayout.HelpBox("The Android Native plugin will alter manifest " +
                    "automatically for your convenience. But in case you want to do it manually, " +
                    "you may un-toggle the checkbox below \n" +
                    "The plugin manifest is located under: " + AN_Settings.ANDROID_CORE_LIB_PATH, MessageType.Info);
                AN_Settings.Instance.ManifestManagement = SA_EditorGUILayout.ToggleFiled("Auto Manifest Management", AN_Settings.Instance.ManifestManagement, SA_StyledToggle.ToggleType.EnabledDisabled);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField(k_UnityJarResolverTitle, EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("The External Dependency Manager for Unity (EDM4U) (formerly Play Services Resolver / Jar Resolver) is a requirement.\n" +
                    "It will download and integrate Android library dependencies " +
                    "and handle any conflicts between plugins that share the same dependencies.\n" +
                    "If you do not have it in your project or have never used it before, please download and learn about using some helper links below.",
                    MessageType.Info);

                AN_Settings.Instance.EnforceEdm4UDependency = SA_EditorGUILayout.ToggleFiled("Enforce EDM4U Dependency", AN_Settings.Instance.EnforceEdm4UDependency, SA_StyledToggle.ToggleType.EnabledDisabled);
                EditorGUILayout.Space();

                GUILayout.Space(2);
                using (new SA_GuiBeginHorizontal())
                {
                    GUILayout.FlexibleSpace();
                    var click = m_JarResolverLink.DrawWithCalcSize();
                    if (click) Application.OpenURL(k_JarResolverDocLink);
                }
            }

            using (new SA_WindowBlockWithSpace("Storage"))
            {
                EditorGUILayout.HelpBox("When plugin needs to have a valid URI for an image, " +
                    "it can be saved using the Internal or External storage. " +
                    "In case saving attempt is failed, an alternative option will be used. " +
                    "You can define if Internal or External storage should be a preferred option.",
                    MessageType.Info);
                AN_Settings.Instance.PreferredImagesStorage = (AN_Settings.StorageType)SA_EditorGUILayout.EnumPopup("Preferred Images Storage", AN_Settings.Instance.PreferredImagesStorage);
                using (new SA_GuiBeginHorizontal())
                {
                    GUILayout.FlexibleSpace();
                    var click = m_StorageOptionsLink.DrawWithCalcSize();
                    if (click) Application.OpenURL(k_StorageOptionsDocLink);
                }
            }

            using (new SA_WindowBlockWithSpace("Debug"))
            {
                EditorGUILayout.HelpBox("API Resolver's are normally launched with build pre-process stage", MessageType.Info);
                var pressed = GUILayout.Button("Start API Resolvers");
                if (pressed)
                {
                    AN_Preprocessor.Resolve();
                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.HelpBox("Action will reset all of the plugin settings to default.", MessageType.Info);
                pressed = GUILayout.Button("Reset To Defaults");
                if (pressed)
                {
                    AN_Settings.Delete();
                    AN_Preprocessor.Resolve();
                }
            }

            using (new SA_WindowBlockWithSpace("Export/import settings"))
            {
                EditorGUILayout.HelpBox("Export settings to file.", MessageType.Info);
                var pressed = GUILayout.Button("Export settings");
                if (pressed)
                {
                    var path = EditorUtility.SaveFilePanel("Save settings as JSON",
                        "",
                        "AN_Settings",
                        "an_settings");
                    AN_SettingsManager.Export(path);
                }

                EditorGUILayout.HelpBox("Import settings from file.", MessageType.Info);
                pressed = GUILayout.Button("Import settings");
                if (pressed)
                {
                    var path = EditorUtility.OpenFilePanel("Import settings from json",
                        "",
                        "an_settings");
                    AN_SettingsManager.Import(path);
                }
            }
        }
    }
}
