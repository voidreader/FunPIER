using UnityEngine;
using SA.Foundation.Editor;
using UnityEditor;

using System.Collections.Generic;


namespace SA.Android
{

    public class AN_FirebaseFeaturesUI : AN_ServiceSettingsUI
    {

        public const string SDK_DOWNLOAD_URL = "https://firebase.google.com/docs/unity/setup";



        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/android-native-pro/getting-started-758");
            AddFeatureUrl("Cloud Messaging", "https://unionassets.com/android-native-pro/cloud-messaging-751");
            AddFeatureUrl("Analytics", "https://unionassets.com/android-native-pro/analytics-752");

        }

        protected override IEnumerable<string> SupportedPlatforms {
            get {
                return new List<string>() { "Android", "Android TV", "Android Wear", "iOS" };
            }
        }


        public override string Title {
            get {
                return "Firebase";
            }
        }

        public override string Description {
            get {
                return "Firebase gives you the tools to develop high-quality apps, grow your user base, and earn more money.";
            }
        }

        protected override Texture2D Icon {
            get {
                return AN_Skin.GetIcon("android_firebase.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return AN_Preprocessor.GetResolver<AN_FirebaseResolver>();
            }
        }

        protected override bool CanBeDisabled {
            get {
                return false;
            }
        }


        protected override void OnServiceUI() {


            using (new SA_WindowBlockWithSpace(new GUIContent("Cloud Messaging(FCM)"))) {
                if (AN_FirebaseDefinesResolver.IsMessagingSDKInstalled) {
                    EditorGUILayout.HelpBox("Cloud Messaging Active.", MessageType.Info);
                } else {
                    EditorGUILayout.HelpBox("FCM Messaging Plugin is required.", MessageType.Warning);
                    PluginDonwloadButton();
                }
            }


           
            using (new SA_WindowBlockWithSpace(new GUIContent("Analytics"))) {
                if (AN_FirebaseDefinesResolver.IsAnalyticsSDKInstalled) {
                    EditorGUILayout.HelpBox("Analytics Active.", MessageType.Info);
                } else {
                    DrawAnalyticsInstalRequired();
                }
            }

        }

        public static void DrawAnalyticsInstalRequired() {
            EditorGUILayout.HelpBox("Firebase Analytics Plugin is required.", MessageType.Warning);
            PluginDonwloadButton();
        }

        private static void PluginDonwloadButton() {
            using (new SA_GuiBeginHorizontal()) {
                GUILayout.FlexibleSpace();
                var click = GUILayout.Button("Download", EditorStyles.miniButton, GUILayout.Width(80));
                if (click) {
                    Application.OpenURL(SDK_DOWNLOAD_URL);
                }

                var refreshClick = GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(80));
                if (refreshClick) {
                    AN_FirebaseDefinesResolver.ProcessAssets();
                }
            }
        }

      
    }
}