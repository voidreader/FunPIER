using SA.Foundation.Editor;
using UnityEditor;
using UnityEngine;

namespace SA.Android.Editor
{
    class AN_GooglePlayFeaturesUI : AN_ServiceSettingsUI
    {
        [SerializeField]
        SA_PluginActiveTextLink m_configureYourGameLink;
        [SerializeField]
        SA_PluginActiveTextLink m_setResource;

        readonly GUIContent SingInLabelContent = new GUIContent("Sign-in [?]:", "Before you start using Google Play API with the plugin" +
            "You must first configure your game in the Google Play Developer Console, " +
            "and then define google play resources using the plugin.");

        public static GUIContent GamesLabelContent = new GUIContent("Games API [?]:", "Start integrating popular gaming features " +
            "in your mobile games and web games by using the Google Play games services APIs.");

        public static string GOOGLE_PLAY_ICON_NAME = "android_googleplay.png";

        public override void OnAwake()
        {
            base.OnAwake();

            m_configureYourGameLink = new SA_PluginActiveTextLink("Configure Your Game");
            m_setResource = new SA_PluginActiveTextLink(string.Empty);

            AddFeatureUrl("Getting Started", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Getting-Started");
            AddFeatureUrl("Checking Availability", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Android-Games-Sing-in#checking-availability");
            AddFeatureUrl("Player Sing-in", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Android-Games-Sing-in#implementing-player-sign-in");
            AddFeatureUrl("Player Information", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Android-Games-Sing-in#retrieving-player-information");
            AddFeatureUrl("Game Pop-ups", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Android-Games-Sing-in#displaying-game-pop-ups");
            AddFeatureUrl("Player Sing-out", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Android-Games-Sing-in#signing-the-player-out");
            AddFeatureUrl("Server API Access", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Server-side-API-Access");
            AddFeatureUrl("Achievements", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Achievements");
            AddFeatureUrl("Leaderboards", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Leaderboards");
            AddFeatureUrl("Saved Games", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Saved-Games");
            AddFeatureUrl("Image Manager", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Image-Manager");
            AddFeatureUrl("Settings Intent", "https://github.com/StansAssets/com.stansassets.android-native/wiki/Android-Games-Settings-Intent");
        }

        public override string Title => "Google Play";

        public override string Description =>
            "The Play Games SDK provides Google Play games services " +
            "that lets you easily integrate popular gaming features.";

        protected override Texture2D Icon => AN_Skin.GetIcon(GOOGLE_PLAY_ICON_NAME);

        public override SA_iAPIResolver Resolver => AN_Preprocessor.GetResolver<AN_GooglePlayResolver>();

        bool m_achievmentsShown;
        bool m_leaderboardsShown;

        protected override void OnServiceUI()
        {
            using (new SA_WindowBlockWithSpace(new GUIContent("Configuration")))
            {
                var setResourceName = "Update Game Resource";
                if (AN_GoolgePlayRersources.GamesIds == null)
                {
                    EditorGUILayout.HelpBox("Before you start using Google Play API with the plugin" +
                        "You must first configure your game in the Google Play Developer Console, " +
                        "and then define google play resources using the plugin.",
                        MessageType.Warning);
                    using (new SA_GuiBeginHorizontal())
                    {
                        GUILayout.FlexibleSpace();
                        var click = m_configureYourGameLink.DrawWithCalcSize();
                        if (click) Application.OpenURL("https://unionassets.com/android-native-pro/getting-started-670");
                    }

                    setResourceName = "Set Game Resource";
                }
                else
                {
                    var applicationIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
                    if (!applicationIdentifier.Equals(AN_GoolgePlayRersources.GamesIds.PackageName))
                        EditorGUILayout.HelpBox("Player Settings Package Name does not match with " +
                            "Android Games Package Name \n" +
                            "unity: " + applicationIdentifier + "\n" +
                            "games-ids.xml: " + AN_GoolgePlayRersources.GamesIds.PackageName,
                            MessageType.Warning);
                    using (new SA_GuiBeginVertical(EditorStyles.helpBox))
                    {
                        SA_EditorGUILayout.SelectableLabel("App Id", AN_GoolgePlayRersources.GamesIds.AppId);
                        SA_EditorGUILayout.SelectableLabel("Package Name", AN_GoolgePlayRersources.GamesIds.PackageName);

                        m_achievmentsShown = EditorGUILayout.Foldout(m_achievmentsShown, "Achievments");
                        if (m_achievmentsShown)
                        {
                            if (AN_GoolgePlayRersources.GamesIds.Achievements.Count > 0)
                                AN_GoolgePlayRersources.GamesIds.Achievements.ForEach(pair =>
                                {
                                    SA_EditorGUILayout.SelectableLabel(pair.Key, pair.Value);
                                });
                            else
                                EditorGUILayout.LabelField("There are no achievments in games-ids.xml");
                        }

                        m_leaderboardsShown = EditorGUILayout.Foldout(m_leaderboardsShown, "Leaderboards");
                        if (m_leaderboardsShown)
                        {
                            if (AN_GoolgePlayRersources.GamesIds.Leaderboards.Count > 0)
                                AN_GoolgePlayRersources.GamesIds.Leaderboards.ForEach(pair =>
                                {
                                    SA_EditorGUILayout.SelectableLabel(pair.Key, pair.Value);
                                });
                            else
                                EditorGUILayout.LabelField("There are no leaderboards in games-ids.xml");
                        }
                    }
                }

                using (new SA_GuiBeginHorizontal())
                {
                    GUILayout.FlexibleSpace();
                    m_setResource.Content.text = setResourceName;
                    var click = m_setResource.DrawWithCalcSize();
                    if (click) AN_GoolgePlayRersourcesWindow.ShowAsModal();
                }
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Google Mobile Services APIs")))
            {
                EditorGUILayout.HelpBox("In order to access Google Play games services functionality, " +
                    "your game needs to provide the signed-in player’s account. If the player is not authenticated, " +
                    "your game may encounter errors when making calls to the Google Play games services APIs.",
                    MessageType.Info);

                using (new SA_GuiBeginHorizontal())
                {
                    EditorGUILayout.LabelField(SingInLabelContent);
                    using (new SA_GuiEnable(false))
                    {
                        SA_EditorGUILayout.ToggleFiled(new GUIContent(), true, SA_StyledToggle.ToggleType.EnabledDisabled);
                    }
                }

                AN_Settings.Instance.GooglePlayGamesAPI = SA_EditorGUILayout.ToggleFiled(GamesLabelContent, AN_Settings.Instance.GooglePlayGamesAPI, SA_StyledToggle.ToggleType.EnabledDisabled);
            }
        }
    }
}
