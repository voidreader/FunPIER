using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

namespace SA.Android
{
    public class AN_AppFeaturesUI : AN_ServiceSettingsUI
    {

        public override void OnAwake() 
        {
            base.OnAwake();

            //Plugin related
            AddFeatureUrl("Settings", "https://unionassets.com/android-native-pro/settings-817");
            AddFeatureUrl("Compatibility", "https://unionassets.com/android-native-pro/compatibility-668");

            //App Service related 
            AddFeatureUrl("Build Info", "https://unionassets.com/android-native-pro/build-info-715");
            AddFeatureUrl("Package Info", "https://unionassets.com/android-native-pro/build-info-683");

            AddFeatureUrl("Popups & Preloaders", "https://unionassets.com/android-native-pro/popups-preloaders-685");
            AddFeatureUrl("Runtime Permissions", "https://unionassets.com/android-native-pro/runtime-permissions-687");
            AddFeatureUrl("Run External App", "https://unionassets.com/android-native-pro/run-external-app-688");
            AddFeatureUrl("Immersive Mode", "https://unionassets.com/android-native-pro/immersive-mode-689");

            AddFeatureUrl("Package Manager", "https://unionassets.com/android-native-pro/packagemanager-707");
            AddFeatureUrl("Check If App Installed", "https://unionassets.com/android-native-pro/packagemanager-707#check-if-app-installed");
            AddFeatureUrl("Open External App", "https://unionassets.com/android-native-pro/packagemanager-707#check-if-app-installed");
            AddFeatureUrl("Query Intent Activities", "https://unionassets.com/android-native-pro/packagemanager-707#query-intent-activities");

            AddFeatureUrl("Native Pop-ups", "https://unionassets.com/android-native-pro/popups-preloaders-685#dialogs");
            AddFeatureUrl("Native Preloader", "https://unionassets.com/android-native-pro/popups-preloaders-685#preloader");
            AddFeatureUrl("Rate Us Dialog", "https://unionassets.com/android-native-pro/rate-us-dialog-710");
            AddFeatureUrl("Date Picker Dialog", "https://unionassets.com/android-native-pro/date-picker-776");
            AddFeatureUrl("Wheel Picker Dialog", "https://unionassets.com/android-native-pro/wheel-picker-dialog-840");

            AddFeatureUrl("Activity", "https://unionassets.com/android-native-pro/activity-708");
            AddFeatureUrl("Move to Background", "https://unionassets.com/android-native-pro/activity-708#movetasktoback");
            AddFeatureUrl("Intent", "https://unionassets.com/android-native-pro/intent-709");
            AddFeatureUrl("Locale", "https://unionassets.com/android-native-pro/locale-824");
            AddFeatureUrl("Settings Page", "https://unionassets.com/android-native-pro/settings-page-720");
           
            AddFeatureUrl("Show Remote Video", "https://unionassets.com/android-native-pro/media-player-775#play-remove-video");
        }

        public override string Title 
        {
            get 
            {
                return "App";
            }
        }

        protected override bool CanBeDisabled 
        {
            get 
            {
                return false;
            }
        }

        public override string Description 
        {
            get 
            {
                return "Contains high-level classes encapsulating the overall Android application model.";
            }
        }

        protected override Texture2D Icon 
        {
            get 
            {
                return AN_Skin.GetIcon("android_app.png");
            }
        }

        public override SA_iAPIResolver Resolver 
        {
            get 
            {
                return AN_Preprocessor.GetResolver<AN_CoreResolver>();
            }
        }


        protected override void OnServiceUI()
        {

            using (new SA_WindowBlockWithSpace(new GUIContent("Runtime Permissions"))) 
            {
                EditorGUILayout.HelpBox("Every Android app runs in a limited-access sandbox." +
                    "If an app needs to use resources or information outside of its own sandbox, " +
                    "the app has to request the appropriate permission.",
                                       MessageType.Info);
               
               AN_Settings.Instance.SkipPermissionsDialog =  SA_EditorGUILayout.ToggleFiled("Startup Permissions Dialog", AN_Settings.Instance.SkipPermissionsDialog, SA_StyledToggle.ToggleType.YesNo);
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