using UnityEngine;
using SA.Foundation.Editor;
using UnityEditor;
using System.Collections.Generic;

namespace SA.Android.Editor
{
    class AN_FirebaseFeaturesUI : AN_ServiceSettingsUI
    {
        public override void OnAwake()
        {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/android-native-pro/getting-started-758");
            AddFeatureUrl("Cloud Messaging", "https://unionassets.com/android-native-pro/cloud-messaging-751");
            AddFeatureUrl("Analytics", "https://unionassets.com/android-native-pro/analytics-752");
        }

        protected override IEnumerable<string> SupportedPlatforms => new List<string>() { "Android", "Android TV", "Android Wear", "iOS" };

        public override string Title => "Firebase";

        public override string Description => "Firebase gives you the tools to develop high-quality apps, grow your user base, and earn more money.";

        protected override Texture2D Icon => AN_Skin.GetIcon("android_firebase.png");

        public override SA_iAPIResolver Resolver => AN_Preprocessor.GetResolver<AN_FirebaseResolver>();

        protected override bool CanBeDisabled => false;

        protected override void OnServiceUI()
        {
            using (new SA_WindowBlockWithSpace(new GUIContent("Cloud Messaging(FCM)")))
            {
                if (AN_Packages.IsMessagingSdkInstalled)
                {
                    EditorGUILayout.HelpBox("Cloud Messaging Active.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("FCM Messaging Plugin not found.", MessageType.Warning);
                    PackageInstallButton(AN_Packages.FirebaseMessagingPackage);
                }
            }
            
            using (new SA_WindowBlockWithSpace(new GUIContent("Analytics")))
            {
                if (AN_Packages.IsAnalyticsSdkInstalled)
                {
                    EditorGUILayout.HelpBox("Analytics Active.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Firebase Analytics not found.", MessageType.Warning);
                    PackageInstallButton(AN_Packages.FirebaseAnalyticsPackage);
                }
            }
        }
        
        static void PackageInstallButton(string packageName)
        {
            using (new SA_GuiBeginHorizontal())
            {
                GUILayout.FlexibleSpace();
                var click = GUILayout.Button("Install", EditorStyles.miniButton, GUILayout.Width(80));
                if (click)
                {
                    AN_Packages.InstallFirebasePackage(packageName);
                }
            }
        }
    }
}
