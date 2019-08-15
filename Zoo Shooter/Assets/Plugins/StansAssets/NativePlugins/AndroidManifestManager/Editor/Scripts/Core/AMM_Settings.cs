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

namespace SA.Android.Manifest {

	public class AMM_Settings: SA_ScriptableSingleton<AMM_Settings>  {

        public const string PLUGIN_NAME = "Android Manifest";
        public const string DOCUMENTATION_URL = "https://unionassets.com/android-manifest-manager";
        

        public const string MANIFEST_MANAGER_FOLDER = SA_Config.STANS_ASSETS_NATIVE_PLUGINS_PATH + "AndroidManifestManager/";

        public const string DEFAULT_MANIFEST_PATH =  MANIFEST_MANAGER_FOLDER + "Editor/Files/default.xml";

        public const string MANIFEST_FOLDER_PATH = SA_Config.UNITY_ANDROID_PLUGINS_PATH;
        public const string MANIFEST_FILE_PATH = MANIFEST_FOLDER_PATH + "AndroidManifest.xml"; 

		private static PluginVersionHandler s_pluginVersion;




        //--------------------------------------
        // SA_ScriptableSettings
        //--------------------------------------


        protected override string BasePath {
            get { return MANIFEST_MANAGER_FOLDER; }
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
                return SA_Config.EDITOR_PRODUCTIVITY_NATIVE_UTILITY_MENU_ROOT + "Android Manifest/Settings";
            }
        }
    }
}