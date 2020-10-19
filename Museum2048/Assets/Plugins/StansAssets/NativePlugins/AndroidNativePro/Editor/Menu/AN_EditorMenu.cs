////////////////////////////////////////////////////////////////////////////////
//
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using SA.Foundation.Config;

namespace SA.Android.Editor
{
    class AN_EditorMenu
    {
        [MenuItem(SA_Config.EditorMenuRoot + "Android/Services", false, 300)]
        public static void Services()
        {
            var window = AN_SettingsWindow.ShowTowardsInspector(WindowTitle);
            window.SetSelectedTabIndex(0);
        }

        [MenuItem(SA_Config.EditorMenuRoot + "Android/Manifest", false, 300)]
        public static void Manifest()
        {
            var window = AN_SettingsWindow.ShowTowardsInspector(WindowTitle);
            window.SetSelectedTabIndex(1);
        }

        [MenuItem(SA_Config.EditorMenuRoot + "Android/Settings", false, 300)]
        public static void Settings()
        {
            var window = AN_SettingsWindow.ShowTowardsInspector(WindowTitle);
            window.SetSelectedTabIndex(2);
        }

        [MenuItem(SA_Config.EditorMenuRoot + "Android/Documentation", false, 300)]
        public static void ISDSetupPluginSetUp()
        {
            Application.OpenURL(AN_Settings.DOCUMENTATION_URL);
        }

        static GUIContent WindowTitle => new GUIContent("Android", AN_Skin.SettingsWindowIcon);
    }
}
