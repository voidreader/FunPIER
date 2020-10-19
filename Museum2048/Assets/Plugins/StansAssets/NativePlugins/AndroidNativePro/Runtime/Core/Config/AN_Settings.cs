using System.Collections.Generic;
using SA.Foundation.Config;
using SA.Foundation.Patterns;
using SA.Android.Utilities;
using SA.Android.Vending.BillingClient;
using UnityEngine;

namespace SA.Android
{
    class AN_Settings : SA_ScriptableSingleton<AN_Settings>
    {
        public enum StorageType
        {
            Internal,
            External,
            ForceInternal
        }

        public const string PLUGIN_NAME = "Android Native";
        public const string DOCUMENTATION_URL = "https://unionassets.com/android-native-pro/manual";

        public const string ANDROID_NATIVE_FOLDER = SA_Config.StansAssetsNativePluginsPath + "AndroidNativePro/";

        public const string EDITOR_FOLDER = ANDROID_NATIVE_FOLDER + "Editor/";
        public const string DEPENDENCIES_FOLDER = EDITOR_FOLDER + "Dependencies/";
        public const string DEPENDENCIES_FILE_PATH = DEPENDENCIES_FOLDER + "AN_Dependencies.xml";

        public const string ANDROID_FOLDER = ANDROID_NATIVE_FOLDER + "Android/";
        public const string ANDROID_FOLDER_DISABLED = ANDROID_NATIVE_FOLDER + "AndroidDisabled/";

        public const string ANDROID_INTERNAL_FOLDER = ANDROID_FOLDER + "Internal/";
        public const string ANDROID_INTERNAL_FOLDER_DISABLED = ANDROID_FOLDER_DISABLED + "Internal/";

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

        public bool ManifestManagement = true;
        public bool EnforceEdm4UDependency = true;


        //--------------------------------------
        // Runtime Settings
        //--------------------------------------

        [SerializeField]
        internal AN_LogLevel LogLevel = new AN_LogLevel();

        public bool WTFLogging = false;
        public StorageType PreferredImagesStorage = StorageType.External;

        //--------------------------------------
        // API Settings
        //--------------------------------------

        public bool Vending = false;
        public bool Contacts = false;
        public bool Social = false;
        public bool GooglePlay = false;
        public bool CameraAndGallery = false;

        //--------------------------------------
        // App
        //--------------------------------------

        public bool MediaPlayer = false;
        public bool LocalNotifications = false;
        public bool SkipPermissionsDialog = false;

        //--------------------------------------
        // Billing
        //--------------------------------------

        public string RSAPublicKey = "Base64-encoded RSA public key to include in your binary. Please remove any spaces.";
        public List<AN_SkuDetails> InAppProducts = new List<AN_SkuDetails>();
        public bool Licensing = false;
        public bool GooglePlayBilling = false;

        public void AddInAppProduct(string sku, AN_BillingClient.SkuType productType, bool isConsumable = false)
        {
            var product = new AN_SkuDetails(sku, productType);
            product.Title = sku;
            product.IsConsumable = isConsumable;
            InAppProducts.Add(product);
        }

        //--------------------------------------
        // Google Play
        //--------------------------------------

        public bool GooglePlayGamesAPI = true;

        //--------------------------------------
        // SA_Scriptable Settings
        //--------------------------------------

        protected override string BasePath => ANDROID_NATIVE_FOLDER;

        public override string PluginName => PLUGIN_NAME;

        public override string DocumentationURL => DOCUMENTATION_URL;

        public override string SettingsUIMenuItem => SA_Config.EditorMenuRoot + "Android/Services";
    }
}
