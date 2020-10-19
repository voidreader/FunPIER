using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

namespace SA.Android.Editor
{
    class AN_AppFeaturesUI : AN_ServiceSettingsUI
    {
        public override void OnAwake()
        {
            base.OnAwake();

            //Plugin related
            AddFeatureUrl("Settings", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Settings");
            AddFeatureUrl("Compatibility", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Compatibility");

            //App Service related
            AddFeatureUrl("Build Info", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Build-Info");
            AddFeatureUrl("Package Info", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Package-Info");

            AddFeatureUrl("Popups & Preloaders", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Popups-&-Preloaders");
            AddFeatureUrl("Runtime Permissions", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Runtime-Permissions");
            AddFeatureUrl("Run External App", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Run-External-App");
            AddFeatureUrl("Immersive Mode", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Immersive-Mode");

            AddFeatureUrl("Package Manager", "https://github.com/StansAssets/com.stansassets.android-native/wiki/PackageManager");
            AddFeatureUrl("Check If App Installed", "https://github.com/StansAssets/com.stansassets.android-native/wiki/PackageManager#check-if-app-installed");
            AddFeatureUrl("Open External App", "https://github.com/StansAssets/com.stansassets.android-native/wiki/PackageManager#open-external-app");
            AddFeatureUrl("Query Intent Activities", "https://github.com/StansAssets/com.stansassets.android-native/wiki/PackageManager#query-intent-activities");

            AddFeatureUrl("Native Pop-ups", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Popups-&-Preloaders#dialogs");
            AddFeatureUrl("Native Preloader", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Popups-&-Preloaders#preloader");
            AddFeatureUrl("Rate Us Dialog", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Rate-Us-Dialog");
            AddFeatureUrl("Date Picker Dialog", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Date-Picker-Dialog");
            AddFeatureUrl("Wheel Picker Dialog", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Wheel-Picker-Dialog");

            AddFeatureUrl("Activity", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Activity");
            AddFeatureUrl("Move to Background", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Activity#movetasktoback");
            AddFeatureUrl("Intent", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Intent");
            AddFeatureUrl("Locale", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Locale");
            AddFeatureUrl("Settings Page", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Settings-Page");

            AddFeatureUrl("Show Remote Video", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Media-Player#play-remove-video");
        }

        public override string Title => "App";

        protected override bool CanBeDisabled => false;

        public override string Description => "Contains high-level classes encapsulating the overall Android application model.";

        protected override Texture2D Icon => AN_Skin.GetIcon("android_app.png");

        public override SA_iAPIResolver Resolver => AN_Preprocessor.GetResolver<AN_CoreResolver>();

        protected override void OnServiceUI()
        {
            using (new SA_WindowBlockWithSpace(new GUIContent("Runtime Permissions")))
            {
                EditorGUILayout.HelpBox("Every Android app runs in a limited-access sandbox." +
                    "If an app needs to use resources or information outside of its own sandbox, " +
                    "the app has to request the appropriate permission.",
                    MessageType.Info);

                AN_Settings.Instance.SkipPermissionsDialog = SA_EditorGUILayout.ToggleFiled("Startup Permissions Dialog", AN_Settings.Instance.SkipPermissionsDialog, SA_StyledToggle.ToggleType.YesNo);
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Media Player")))
            {
                EditorGUILayout.HelpBox("Media Player can be used to control playback of audio/video files and streams.",
                    MessageType.Info);

                AN_Settings.Instance.MediaPlayer = SA_EditorGUILayout.ToggleFiled("API State", AN_Settings.Instance.MediaPlayer, SA_StyledToggle.ToggleType.EnabledDisabled);
            }
        }
    }
}
