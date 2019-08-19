using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;

namespace SA.Android
{
    public static class AN_Skin
    {

		private const string ICONS_PATH = AN_Settings.ANDROID_NATIVE_FOLDER + "Editor/Art/Icons/";

        public static Texture2D SettingsWindowIcon {
            get {
                if (EditorGUIUtility.isProSkin) {
                    return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + "android_pro.png");
                } else {
                    return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + "android.png");
                }
            }
        }

        public static Texture2D GetIcon(string iconName) {
            return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + iconName); ;
        }
    }
}