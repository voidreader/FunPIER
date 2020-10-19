using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;
using Rotorz.ReorderableList;

namespace SA.Android.Editor
{
    class AN_LocalNotificationsFeaturesUI : AN_ServiceSettingsUI
    {
        const string k_RequiredIconExtension = ".png";
        const string k_RequiredSoundExtension = ".wav";

        public override void OnAwake()
        {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Getting-Started-(Local-Notifications)");
            AddFeatureUrl("Scheduling", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Scheduling-Notifications");
            AddFeatureUrl("Canceling Notifications", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Scheduling-Notifications#canceling-notifications");
            AddFeatureUrl("Handling Notifications", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Responding-to-Notification");
            AddFeatureUrl("Notification Styles", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Notification-Styles");
            AddFeatureUrl("Notification Channels", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Notification-Channels");
            AddFeatureUrl("Application Badges", "https://github.com/StansAssets/com.stansassets.android-native/wiki/ApplicationIcon-Badge-Number");
        }

        public override string Title => "Local Notifications";

        public override string Description => "Allows you to display & manage local notifications on the android device.";

        protected override Texture2D Icon => AN_Skin.GetIcon("android_notifications.png");

        public override SA_iAPIResolver Resolver => AN_Preprocessor.GetResolver<AN_LocalNotificationsResolver>();

        protected override void OnServiceUI()
        {
            using (new SA_WindowBlockWithSpace(new GUIContent("Customization")))
            {
                EditorGUILayout.HelpBox("A notification is a message that Android displays outside your app's UI " +
                    "to provide the user with reminders, communication from other people, or other timely information " +
                    "from your app. Users can tap the notification to open your app or take an action directly from the notification.",
                    MessageType.Info);

                ReorderableListGUI.Title("Custom Icons (*" + k_RequiredIconExtension + ")");
                ReorderableListGUI.ListField(AN_EditorSettings.Instance.NotificationIcons, DrawIconField, DrawEmptyIcons);
                ValidateAssets(AN_EditorSettings.Instance.NotificationIcons, AN_Settings.ANDROID_DRAWABLE_PATH, k_RequiredIconExtension);
                EditorGUILayout.Space();

                ReorderableListGUI.Title("Custom Sounds (*" + k_RequiredSoundExtension + ")");
                ReorderableListGUI.ListField(AN_EditorSettings.Instance.NotificationAlertSounds, DrawSoundField, DrawEmptySounds);
                ValidateAssets(AN_EditorSettings.Instance.NotificationAlertSounds, AN_Settings.ANDROID_RAW_PATH, k_RequiredSoundExtension);
            }
        }

        void ValidateAssets<T>(List<T> assets, string requiredLocation, string requiredExtension) where T : Object
        {
            //Let's make sure we aren't missing assets under requiredLocation
            var assetPaths = SA_AssetDatabase.FindAssetsWithExtentions(requiredLocation, requiredExtension);
            foreach (var assetPath in assetPaths)
            {
                var assetExtension = SA_PathUtil.GetExtension(assetPath);
                if (assetExtension.Equals(requiredExtension))
                {
                    var file = (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                    if (!assets.Contains(file))
                    {
                        assets.Add(file);
                        return;
                    }
                }
            }

            for (var i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                if (asset == null)
                {
                    //We do not allow null element's unless this is a last element
                    if (i != assets.Count - 1)
                    {
                        assets.RemoveAt(i);
                        return;
                    }

                    continue;
                }

                if (!HasValidExtension(asset, requiredExtension))
                {
                    EditorGUILayout.HelpBox(asset.name + " need to be in *" + requiredExtension + " format.", MessageType.Error);
                    continue;
                }

                if (!SA_AssetDatabase.IsAssetInsideFolder(asset, requiredLocation))
                {
                    EditorGUILayout.HelpBox(asset.name + " has to be inside: \n" + requiredLocation, MessageType.Error);
                    using (new SA_GuiBeginHorizontal())
                    {
                        GUILayout.FlexibleSpace();
                        var move = GUILayout.Button("Move", EditorStyles.miniButton);
                        if (move)
                        {
                            var currentPath = AssetDatabase.GetAssetPath(asset);
                            var assetName = SA_AssetDatabase.GetFileName(currentPath);
                            var newPath = requiredLocation + assetName;
                            SA_AssetDatabase.MoveAsset(currentPath, newPath);
                        }
                    }
                }
            }
        }

        bool HasValidExtension(Object asset, string requiredExtension)
        {
            var assetPath = SA_AssetDatabase.GetAssetPath(asset);
            var assetExtension = SA_PathUtil.GetExtension(assetPath);
            if (assetExtension.Equals(requiredExtension))
                return true;
            else
                return false;
        }

        Object DrawIconField(Rect position, Object asset)
        {
            var color = GUI.color;
            if (asset != null)
            {
                if (!HasValidExtension(asset, k_RequiredIconExtension))
                    GUI.color = Color.red;

                if (!SA_AssetDatabase.IsAssetInsideFolder(asset, AN_Settings.ANDROID_DRAWABLE_PATH))
                    GUI.color = Color.red;
            }

            var result = DrawObjectField(position, asset);
            GUI.color = color;

            return result;
        }

        Object DrawSoundField(Rect position, Object asset)
        {
            var color = GUI.color;
            if (asset != null)
            {
                if (!HasValidExtension(asset, k_RequiredSoundExtension))
                    GUI.color = Color.red;

                if (!SA_AssetDatabase.IsAssetInsideFolder(asset, AN_Settings.ANDROID_RAW_PATH))
                    GUI.color = Color.red;
            }

            var result = DrawObjectField(position, asset);
            GUI.color = color;
            return result;
        }

        T DrawObjectField<T>(Rect position, T itemValue) where T : Object
        {
            var drawRect = new Rect(position);
            drawRect.y += 2;
            drawRect.height = 15;
            return (T)EditorGUI.ObjectField(drawRect, itemValue, typeof(T), false);
        }

        void DrawEmptyIcons()
        {
            EditorGUILayout.LabelField("Add icons you want to use as custom notification icons. The application icon will be used by default", SA_Skin.MiniLabelWordWrap);
        }

        void DrawEmptySounds()
        {
            EditorGUILayout.LabelField("Add sound clips you want to use as custom notification alert sound. The phone default alert sound will be used by default", SA_Skin.MiniLabelWordWrap);
        }
    }
}
