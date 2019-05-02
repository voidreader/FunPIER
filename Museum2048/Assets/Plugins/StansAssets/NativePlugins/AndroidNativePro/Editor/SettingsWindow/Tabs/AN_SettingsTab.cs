using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;


namespace SA.Android {
    public class AN_SettingsTab : SA_GUILayoutElement {
        const string JAR_RESOLVER_DOC_LINK = "https://unionassets.com/android-native-pro/unity-jar-resolver-669";
        const string STORAGE_OPTIONS_DOC_LINK = "https://unionassets.com/android-native-pro/preferred-images-storage-828";

        [SerializeField] SA_PluginActiveTextLink m_JarResolverLink;
        [SerializeField] SA_PluginActiveTextLink m_StorageOptionsLink;

        private GUIContent Info = new GUIContent("Info[?]:", "Full communication logs between Native plugin part");
        private GUIContent Warnings = new GUIContent("Warnings[?]:", "Warnings");
        private GUIContent Errors = new GUIContent("Errors[?]:", "Errors");

        public override void OnAwake() {
            m_JarResolverLink = new SA_PluginActiveTextLink("[?] How to use");
            m_StorageOptionsLink = new SA_PluginActiveTextLink("[?] Learn More");
        }


        public override void OnGUI() {
            using (new SA_WindowBlockWithSpace(new GUIContent("Log Level"))) {
                EditorGUILayout.HelpBox("We recommend you to keep full logging level while your project in development mode. " +
                                        "Full communication logs between Native plugin part & " +
                                        "Unity side will be only available with Info logging  level enabled. \n" +
                                        "Disabling the error logs isn't recommended.", MessageType.Info);


                using (new SA_GuiBeginHorizontal()) {
                    var logLevel = AN_Settings.Instance.LogLevel;
                    logLevel.Info = GUILayout.Toggle(logLevel.Info, Info, GUILayout.Width(80));
                    logLevel.Warning = GUILayout.Toggle(logLevel.Warning, Warnings, GUILayout.Width(100));
                    logLevel.Error = GUILayout.Toggle(logLevel.Error, Errors, GUILayout.Width(100));
                }

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("On some Android devices, Log.d or Log.e methods will not print anything to console," +
                    "so sometimes the only ability to see the logs is to enable the WTF printing. This will make all" +
                    "logs to be printed with Log.wtf method despite message log level.", MessageType.Info);

                AN_Settings.Instance.WTFLogging = GUILayout.Toggle(AN_Settings.Instance.WTFLogging, "WTF Logging",  GUILayout.Width(130));
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Environment Management"))) {
                EditorGUILayout.HelpBox("The Android Native plugin will alter manifest " +
                                        "automatically for your convenience. But in case you want to do it manually, " +
                                        "you may un-toggle the checkbox below \n" +
                                        "The plugin manifest is located under: " + AN_Settings.ANDROID_CORE_LIB_PATH, MessageType.Info);
                AN_Settings.Instance.ManifestManagement = SA_EditorGUILayout.ToggleFiled("Auto Manifest Management", AN_Settings.Instance.ManifestManagement, SA_StyledToggle.ToggleType.EnabledDisabled);

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The Android Resolver plugin will download and integrate Android library dependencies " +
                                        "and handle any conflicts between plugins that share the same dependencies. \n" +
                                        "The Resolver is an additional third-party plugin. You need to download, install and configure it" +
                                        "Before Android Native will able to rely on this plugin and disable internal libraries.",
                    MessageType.Info);

                AN_Settings.Instance.UseUnityJarResolver = SA_EditorGUILayout.ToggleFiled("Unity Jar Resolver", AN_Settings.Instance.UseUnityJarResolver, SA_StyledToggle.ToggleType.EnabledDisabled);
                using (new SA_GuiBeginHorizontal()) 
                {
                    GUILayout.FlexibleSpace();
                    var click = m_JarResolverLink.DrawWithCalcSize();
                    if (click) 
                    {
                        Application.OpenURL(JAR_RESOLVER_DOC_LINK);
                    }
                }
            }

            using (new SA_WindowBlockWithSpace("Storage"))
            {
                EditorGUILayout.HelpBox("When plugin needs to have a valid URI for an image, " +
                                        "it can be saved using the Internal or External storage. " +
                                        "In case saving attempt is failed, an alternative option will be used. " +
                                        "You can define if Internal or External storage should be a preferred option.",
                    MessageType.Info);
                AN_Settings.Instance.PreferredImagesStorage = (AN_Settings.StorageType) SA_EditorGUILayout.EnumPopup("Preferred Images Storage", AN_Settings.Instance.PreferredImagesStorage);
                using (new SA_GuiBeginHorizontal()) 
                {
                    GUILayout.FlexibleSpace();
                    var click = m_StorageOptionsLink.DrawWithCalcSize();
                    if (click) 
                    {
                        Application.OpenURL(STORAGE_OPTIONS_DOC_LINK);
                    }
                }
            }


            using (new SA_WindowBlockWithSpace("Debug")) {
                EditorGUILayout.HelpBox("API Resolver's are normally launched with build pre-process stage", MessageType.Info);
                var pressed = GUILayout.Button("Start API Resolvers");
                if (pressed) {
                    SA_PluginsEditor.DisableLibstAtPath(AN_Settings.ANDROID_FOLDER_DISABLED);
                    AN_Preprocessor.Resolve();
                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.HelpBox("Action will reset all of the plugin settings to default.", MessageType.Info);
                pressed = GUILayout.Button("Reset To Defaults");
                if (pressed) {
                    AN_Settings.Delete();
                    AN_Preprocessor.Resolve();
                }
            }

        }
    }
}