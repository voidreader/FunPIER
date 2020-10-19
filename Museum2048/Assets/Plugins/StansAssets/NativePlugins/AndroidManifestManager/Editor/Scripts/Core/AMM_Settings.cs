////////////////////////////////////////////////////////////////////////////////
//  
// @module Manifest Manager
// @author Alex Yaremenko (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using SA.Foundation.Config;
using SA.Foundation.Patterns;

namespace SA.Android.Manifest
{
    public class AMM_Settings : SA_ScriptableSingleton<AMM_Settings>
    {
        public const string PLUGIN_NAME = "Android Manifest";
        public const string DOCUMENTATION_URL = "https://unionassets.com/android-manifest-manager";

        public const string MANIFEST_MANAGER_FOLDER = SA_Config.StansAssetsNativePluginsPath + "AndroidManifestManager/";

        public const string DEFAULT_MANIFEST_PATH = MANIFEST_MANAGER_FOLDER + "Editor/Files/default.xml";

        public const string MANIFEST_FOLDER_PATH = SA_Config.UnityAndroidPluginsPath;
        public const string MANIFEST_FILE_PATH = MANIFEST_FOLDER_PATH + "AndroidManifest.xml";

        static PluginVersionHandler s_pluginVersion;

        //--------------------------------------
        // SA_ScriptableSettings
        //--------------------------------------

        protected override string BasePath => MANIFEST_MANAGER_FOLDER;

        public override string PluginName => PLUGIN_NAME;

        public override string DocumentationURL => DOCUMENTATION_URL;

        public override string SettingsUIMenuItem => SA_Config.EditorProductivityNativeUtilityMenuRoot + "Android Manifest/Settings";
    }
}
