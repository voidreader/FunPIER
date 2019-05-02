
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;
using Rotorz.ReorderableList;



namespace SA.Android
{

    public class AN_LocalNotificationsFeaturesUI : AN_ServiceSettingsUI
    {

        private const string REQUIRED_ICON_EXTENTION = ".png";
        private const string REQUIRED_SOUND_EXTENTION = ".wav";

        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/android-native-pro/getting-started-769");
            AddFeatureUrl("Scheduling", "https://unionassets.com/android-native-pro/local-notifications-684");
            AddFeatureUrl("Canceling Notifications", "https://unionassets.com/android-native-pro/local-notifications-684#canceling-notifications");
            AddFeatureUrl("Handling Notifications", "https://unionassets.com/android-native-pro/responding-to-notification-772");
            AddFeatureUrl("Notification Styles", "https://unionassets.com/android-native-pro/notification-styles-717");
            AddFeatureUrl("Notification Channels", "https://unionassets.com/android-native-pro/notification-channels-716");

        }

        public override string Title {
            get {
                return "Local Notifications";
            }
        }

        public override string Description {
            get {
                return "Allows you to display & manage local notifications on the android device.";
            }
        }

        protected override Texture2D Icon {
            get {
                return AN_Skin.GetIcon("android_notifications.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return AN_Preprocessor.GetResolver<AN_LocalNotificationsResolver>();
            }
        }


        protected override void OnServiceUI() {

            using (new SA_WindowBlockWithSpace(new GUIContent("Customization"))) {

                EditorGUILayout.HelpBox("A notification is a message that Android displays outside your app's UI " +
                             "to provide the user with reminders, communication from other people, or other timely information " +
                             "from your app. Users can tap the notification to open your app or take an action directly from the notification.",
                                                 MessageType.Info);


                ReorderableListGUI.Title("Custom Icons (*" + REQUIRED_ICON_EXTENTION + ")");
                ReorderableListGUI.ListField(AN_EditorSettings.Instance.NotificationIcons, DrawIconField, DrawEmptyIcons);
                ValidateAssets(AN_EditorSettings.Instance.NotificationIcons, AN_Settings.ANDROID_DRAWABLE_PATH, REQUIRED_ICON_EXTENTION);
                EditorGUILayout.Space();

                ReorderableListGUI.Title("Custom Sounds (*" + REQUIRED_SOUND_EXTENTION + ")");
                ReorderableListGUI.ListField(AN_EditorSettings.Instance.NotificationAlertSounds, DrawSoundField, DrawEmptySounds);
                ValidateAssets(AN_EditorSettings.Instance.NotificationAlertSounds, AN_Settings.ANDROID_RAW_PATH, REQUIRED_SOUND_EXTENTION);

            }


        }



        private void ValidateAssets<T>(List<T> assets, string requiredLocation, string requiredExtention) where T : Object {


            //Let's make sure we aren't missin assets under requiredLocation
            List<string> assetPaths = SA_AssetDatabase.FindAssetsWithExtentions(requiredLocation, requiredExtention);
            foreach (var assetPath in assetPaths) {
                string assetExtention = SA_PathUtil.GetExtension(assetPath);
                if (assetExtention.Equals(requiredExtention)) {
                    T file = (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                    if (!assets.Contains(file)) {
                        assets.Add(file);
                        return;
                    }
                }
            }


            for (int i = 0; i < assets.Count; i++) {
                var asset = assets[i];
                if (asset == null) {
                    //We do not allow null elemnt's unless this is a last element
                    if(i != assets.Count - 1) {
                        assets.Remove(asset);
                        return;
                    }
                    continue;
                }

                if(!HasValidExtension(asset, requiredExtention)) {
                    EditorGUILayout.HelpBox(asset.name + " need to be in *" + requiredExtention  + " format.", MessageType.Error);
                    continue;
                }

                if (!SA_AssetDatabase.IsAssetInsideFolder(asset, requiredLocation)) {

                    EditorGUILayout.HelpBox(asset.name + " has to be inside: \n" + requiredLocation, MessageType.Error);
                    using (new SA_GuiBeginHorizontal()) {
                        GUILayout.FlexibleSpace();
                        bool move = GUILayout.Button("Move", EditorStyles.miniButton);
                        if (move) {

                            string currentPath = AssetDatabase.GetAssetPath(asset);
                            string assetName = SA_AssetDatabase.GetFileName(currentPath);
                            string newPath = requiredLocation + assetName;
                            SA_AssetDatabase.MoveAsset(currentPath, newPath);
                        }
                    }
                }

            }
        }


        private bool HasValidExtension(Object asset, string requiredExtention) {
            var assetPath = SA_AssetDatabase.GetAssetPath(asset);
            string assetExtention = SA_PathUtil.GetExtension(assetPath);
            if (assetExtention.Equals(requiredExtention)) {
                return true;
            } else {
                return false;
            }
        }
        

    

        private Object DrawIconField(Rect position, Object asset) {

            Color color = GUI.color;
            if(asset != null) {
                if (!HasValidExtension(asset, REQUIRED_ICON_EXTENTION)) {
                    GUI.color = Color.red;
                }

                if (!SA_AssetDatabase.IsAssetInsideFolder(asset, AN_Settings.ANDROID_DRAWABLE_PATH)) {
                    GUI.color = Color.red;
                }
            }

            Object result= DrawObjectField<Object>(position, asset);
            GUI.color = color;

            return result;
        }


        private Object DrawSoundField(Rect position, Object asset) {

            Color color = GUI.color;
            if (asset != null) {
                if (!HasValidExtension(asset, REQUIRED_SOUND_EXTENTION)) {
                    GUI.color = Color.red;
                }

                if (!SA_AssetDatabase.IsAssetInsideFolder(asset, AN_Settings.ANDROID_RAW_PATH)) {
                    GUI.color = Color.red;
                }
            }

            Object result = DrawObjectField<Object>(position, asset);
            GUI.color = color;

            return result;
        }




        private T DrawObjectField<T>(Rect position, T itemValue) where T : Object {
            Rect drawRect = new Rect(position);
            drawRect.y += 2;
            drawRect.height = 15;
            return (T)EditorGUI.ObjectField(drawRect, itemValue, typeof(T), false);
        }

        private void DrawEmptyIcons() {
            EditorGUILayout.LabelField("Add icons you want to use as custom notification icons. The application icon will be used by default", SA_Skin.MiniLabelWordWrap);
        }

        private void DrawEmptySounds() {
            EditorGUILayout.LabelField("Add sound clips you want to use as custom notification alert sound. The phone default alert sound will be used by default", SA_Skin.MiniLabelWordWrap);
        }

    }
}