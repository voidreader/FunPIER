using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Config;
using SA.Foundation.Patterns;
using UnityEditor;


namespace SA.Android
{

    public class AN_EditorSettings : SA_ScriptableSingletonEditor<AN_EditorSettings>
    {
        public List<Object> NotificationIcons = new List<Object>();
        public List<Object> NotificationAlertSounds = new List<Object>();


        //--------------------------------------
        // SA_ScriptableSettings
        //--------------------------------------

        protected override string BasePath {
            get { return AN_Settings.ANDROID_NATIVE_FOLDER; }
        }


        public override string PluginName {
            get {
                return AN_Settings.Instance.PluginName + " Editor";
            }
        }

        public override string DocumentationURL {
            get {
                return AN_Settings.Instance.DocumentationURL;
            }
        }


        public override string SettingsUIMenuItem {
            get {
                return AN_Settings.Instance.SettingsUIMenuItem;
            }
        }
    }
}