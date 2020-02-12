using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using SA.Android.Editor;


namespace SA.Android {
    public class AN_SettingsTab : SA_GUILayoutElement 
    {
        private const string k_JarResolverDocLink = "https://unionassets.com/android-native-pro/unity-jar-resolver-669";
        private const string k_StorageOptionsDocLink = "https://unionassets.com/android-native-pro/preferred-images-storage-828";
        private const string k_UnityJarResolverTitle = "Unity Jar Resolver";
        private const string k_UnityJarResolverDownloadUrl = "https://dl.dropboxusercontent.com/s/13wduoat230tgvt/play-services-resolver-1.2.122.0.unitypackage";

        private const int k_ButtonHeight = 18;
        
        [SerializeField] SA_PluginActiveTextLink m_JarResolverLink;
        [SerializeField] SA_PluginActiveTextLink m_StorageOptionsLink;

        private GUIContent Info = new GUIContent("Info[?]:", "Full communication logs between Native plugin part");
        private GUIContent Warnings = new GUIContent("Warnings[?]:", "Warnings");
        private GUIContent Errors = new GUIContent("Errors[?]:", "Errors");
       
        private GUIContent UnityJarResolverState;

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

            using (new SA_WindowBlockWithSpace(new GUIContent("Environment Management"))) 
            {
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


                using (new SA_GuiBeginHorizontal())
                {
                    var resolverLabel = k_UnityJarResolverTitle + " - ";
                    if (AN_Settings.Instance.UseUnityJarResolver)
                    {
                        if(!string.IsNullOrEmpty(AN_Settings.Instance.UnityJarResolverVersion))
                        {
                            resolverLabel += AN_Settings.Instance.UnityJarResolverVersion;
                        }
                        else
                        {
                            resolverLabel += "Enabled";
                        }
                        
                        GUILayout.Label(resolverLabel);
                        GUILayout.FlexibleSpace();
                        var restartResolveContent = new GUIContent("  Restart Resolver", SA_Skin.GetGenericIcon("refresh.png"));
                        var pressed = GUILayout.Button(restartResolveContent, GUILayout.Width(140), GUILayout.Height(15));
                        if (pressed) 
                        {
                            AN_ResolveManager.ProcessAssets();
                            GUIUtility.ExitGUI();
                        }
                    }
                    else
                    {
                        GUILayout.Label(k_UnityJarResolverTitle);
                        GUILayout.FlexibleSpace();
                        
                        var downloadButtonContent = new GUIContent("  Download", SA_Skin.GetGenericIcon("download.png"));
                        var pressed = GUILayout.Button(downloadButtonContent, GUILayout.Width(100), GUILayout.Height(k_ButtonHeight));
                        if (pressed) 
                        {
                            SA_PackageManager.DownloadAndImport("Unity Jar Resolver", k_UnityJarResolverDownloadUrl, interactive:false);
                        }
                        
                        var refreshButtonContent = new GUIContent("  Refresh", SA_Skin.GetGenericIcon("refresh.png"));
                        pressed = GUILayout.Button(refreshButtonContent, GUILayout.Width(100), GUILayout.Height(k_ButtonHeight));
                        if (pressed)
                        {
                            AN_ResolveManager.ProcessAssets();
                            GUIUtility.ExitGUI();
                        }
                    }
                }
                
                if (AN_Settings.Instance.UseUnityJarResolver)
                {
                   
                    using (new SA_GuiBeginHorizontal())
                    {
                        GUILayout.Label("Dependencies: ");
                        GUILayout.FlexibleSpace();
                        var regenerate = GUILayout.Button("Regenerate", GUILayout.Width(105),
                            GUILayout.Height(k_ButtonHeight));
                        if(regenerate)
                            AN_Preprocessor.Resolve();
                            
                        var open = GUILayout.Button("Open", GUILayout.Width(105),
                            GUILayout.Height(k_ButtonHeight));
                        if (open)
                        {
                            var dependenciesFile = AssetDatabase.LoadAssetAtPath<TextAsset>(AN_Settings.DEPENDENCIES_FILE_PATH);
                            AssetDatabase.OpenAsset(dependenciesFile);
                        }
                    }
                }
                
                GUILayout.Space(2);
                using (new SA_GuiBeginHorizontal())
                {
                    GUILayout.FlexibleSpace();
                    var click = m_JarResolverLink.DrawWithCalcSize();
                    if (click) 
                    {
                        Application.OpenURL(k_JarResolverDocLink);
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
                        Application.OpenURL(k_StorageOptionsDocLink);
                    }
                }
            }


            using (new SA_WindowBlockWithSpace("Debug")) 
            {
                EditorGUILayout.HelpBox("API Resolver's are normally launched with build pre-process stage", MessageType.Info);
                var pressed = GUILayout.Button("Start API Resolvers");
                if (pressed) 
                {
                    SA_PluginsEditor.DisableLibstAtPath(AN_Settings.ANDROID_FOLDER_DISABLED);
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
                    string path = EditorUtility.SaveFilePanel("Save settings as JSON",
                    "",
                    "AN_Settings",
                    "an_settings");
                    AN_SettingsManager.Export(path);
                }

                EditorGUILayout.HelpBox("Import settings from file.", MessageType.Info);
                pressed = GUILayout.Button("Import settings");
                if (pressed)
                {
                    string path = EditorUtility.OpenFilePanel("Import settings from json",
                        "",
                        "an_settings");
                    AN_SettingsManager.Import(path);
                }
            }
        }
    }
}