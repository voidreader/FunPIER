using System.Collections.Generic;
using SA.Foundation.Config;
using SA.Foundation.Patterns;


using SA.Android.Vending.Billing;
using SA.Android.Utilities;

namespace SA.Android
{
    public class AN_Settings : SA_ScriptableSingleton<AN_Settings>
    {
        public enum StorageType
        {
            Internal,
            External
        }
        
        public const string PLUGIN_NAME = "Android Native";
        public const string DOCUMENTATION_URL = "https://unionassets.com/android-native-pro/manual";

        public const string ANDROID_NATIVE_FOLDER = SA_Config.STANS_ASSETS_NATIVE_PLUGINS_PATH + "AndroidNativePro/";

        public const string EDITOR_FOLDER = ANDROID_NATIVE_FOLDER + "Editor/";
        public const string DEPENDENCIES_FOLDER = EDITOR_FOLDER + "Dependencies/";
        public const string DEPENDENCIES_FILE_PATH = DEPENDENCIES_FOLDER + "AN_Dependencies.xml";


        public const string ANDROID_FOLDER = ANDROID_NATIVE_FOLDER + "Android/";
        public const string ANDROID_FOLDER_DISABLED = ANDROID_NATIVE_FOLDER + "AndroidDisabled/";

        public const string ANDROID_INTERNAL_FOLDER = ANDROID_FOLDER + "Internal/";
        public const string ANDROID_INTERNAL_FOLDER_DISABLED = ANDROID_FOLDER_DISABLED + "Internal/";


        public const string ANDROID_MAVEN_FOLDER = ANDROID_FOLDER + "Maven/";
        public const string ANDROID_MAVEN_FOLDER_DISABLED = ANDROID_FOLDER_DISABLED + "Maven/";


        public const string ANDROID_CORE_LIB_PATH = ANDROID_NATIVE_FOLDER + "Android/Core/an_library.bundle/";

        public const string ANDROID_RES_PATH = ANDROID_CORE_LIB_PATH + "res/";
        public const string ANDROID_VALUES_PATH = ANDROID_RES_PATH + "values/";
        public const string ANDROID_DRAWABLE_PATH = ANDROID_RES_PATH + "drawable/";
        public const string ANDROID_RAW_PATH = ANDROID_RES_PATH + "raw/";


        public const string ANDROID_MANIFEST_FILE_PATH = ANDROID_CORE_LIB_PATH + "AndroidManifest.xml";
        public const string ANDROID_GAMES_IDS_FILE_PATH = ANDROID_VALUES_PATH + "games-ids.xml";


        public const string ANDROID_TEST_SCENE_PATH = ANDROID_NATIVE_FOLDER + "Tests/Scene/AN_TestScene.unity"; 


        //--------------------------------------
        // Editor Settings
        //--------------------------------------
        
        public bool UseUnityJarResolver;
        public bool ManifestManagement = true;
        
        
        //--------------------------------------
        // Runtime Settings
        //--------------------------------------

        public AN_LogLevel LogLevel = new AN_LogLevel();
        public bool WTFLogging;
        public StorageType PreferredImagesStorage = StorageType.External;
        

        //--------------------------------------
        // API Settings
        //--------------------------------------

        public bool Vending;
        public bool Contacts;
        public bool Social;
        public bool GooglePlay;
        public bool CameraAndGallery;



        //--------------------------------------
        // App
        //--------------------------------------

        public bool MediaPlayer;
        public bool LocalNotifications;
        public bool SkipPermissionsDialog;


        //--------------------------------------
        // Support v4
        //--------------------------------------



        //--------------------------------------
        // Billing
        //--------------------------------------

        public string RSAPublicKey = "Base64-encoded RSA public key to include in your binary. Please remove any spaces.";
        public List<AN_Product> InAppProducts = new List<AN_Product>();
        public bool Licensing;

        //--------------------------------------
        // Google Play
        //--------------------------------------

        public bool GooglePlayGamesAPI = true;



        //--------------------------------------
        // SA_ScriptableSettings
        //--------------------------------------


        protected override string BasePath {
            get { return ANDROID_NATIVE_FOLDER; }
        }


        
        public override string PluginName {
            get {
                return PLUGIN_NAME;
            }
        }

        public override string DocumentationURL {
            get {
                return DOCUMENTATION_URL;
            }
        }


        public override string SettingsUIMenuItem {
            get {
                return SA_Config.EDITOR_MENU_ROOT + "Android/Services";
            }
        }


    }
}