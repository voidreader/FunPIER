

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;

namespace SA.Android.Manifest
{
    public static class AMM_Skin
    {

        public const string ICONS_PATH = AMM_Settings.MANIFEST_MANAGER_FOLDER + "Editor/Art/Icons/";

        public static Texture2D SettingsWindowIcon {
            get {
                if (EditorGUIUtility.isProSkin) {
                    return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + "amm_pro.png");
                } else {
                    Debug.Log(ICONS_PATH);
                    return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + "amm.png");
                }
            }
        }
    }
}