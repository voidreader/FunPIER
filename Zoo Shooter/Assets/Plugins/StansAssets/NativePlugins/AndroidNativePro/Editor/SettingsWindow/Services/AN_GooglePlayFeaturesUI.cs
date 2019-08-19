using SA.Foundation.Editor;
using UnityEditor;
using UnityEngine;

namespace SA.Android {

    public class AN_GooglePlayFeaturesUI : AN_ServiceSettingsUI {
        [SerializeField] SA_PluginActiveTextLink m_configureYourGameLink;
        [SerializeField] SA_PluginActiveTextLink m_setResource;

        GUIContent SingInLabelContent = new GUIContent("Sign-in [?]:", "Before you start using Google Play API with the plugin" +
                   "You must first configure your game in the Google Play Developer Console, " +
                   "and then define google play resources using the plugin.");

        public static GUIContent GamesLabelContent = new GUIContent("Games API [?]:", "Start integrating popular gaming features " +
            "in your mobile games and web games by using the Google Play games services APIs.");


        public static string GOOGLE_PLAY_ICON_NAME = "android_googleplay.png";



        public override void OnAwake() {
            base.OnAwake();

            m_configureYourGameLink = new SA_PluginActiveTextLink("Configure Your Game");
            m_setResource = new SA_PluginActiveTextLink(string.Empty);

            AddFeatureUrl("Getting Started", "https://unionassets.com/android-native-pro/getting-started-670");
            AddFeatureUrl("Checking Availability", "https://unionassets.com/android-native-pro/android-games-sing-in-679#checking-availability");
            AddFeatureUrl("Player Sing-in", "https://unionassets.com/android-native-pro/android-games-sing-in-679#implementing_player_sign-in");
            AddFeatureUrl("Player Information", "https://unionassets.com/android-native-pro/android-games-sing-in-679#retrieving_player_information");
            AddFeatureUrl("Game Pop-ups", "https://unionassets.com/android-native-pro/android-games-sing-in-679#displaying_game_pop-ups");
            AddFeatureUrl("Player Sing-out", "https://unionassets.com/android-native-pro/android-games-sing-in-679#signing-the-player-out");
            AddFeatureUrl("Server API Access", "https://unionassets.com/android-native-pro/server-side-api-access-714");
            AddFeatureUrl("Achievements", "https://unionassets.com/android-native-pro/achievements-680");
            AddFeatureUrl("Leaderboards", "https://unionassets.com/android-native-pro/leaderboards-681");
            AddFeatureUrl("Saved Games", "https://unionassets.com/android-native-pro/saved-games-682");
            AddFeatureUrl("Image Manager", "https://unionassets.com/android-native-pro/image-manager-818");

        }

        public override string Title {
            get {
                return "Google Play";
            }
        }


        public override string Description {
            get {
                return "The Play Games SDK provides Google Play games services " +
                    "that lets you easily integrate popular gaming features.";
            }
        }

        protected override Texture2D Icon {
            get {
                return AN_Skin.GetIcon(GOOGLE_PLAY_ICON_NAME);
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return AN_Preprocessor.GetResolver<AN_GooglePlayResolver>();
            }
        }

        private bool m_achievmentsShown;
        private bool m_leaderboardsShown;
        protected override void OnServiceUI() {

            using (new SA_WindowBlockWithSpace(new GUIContent("Configuration"))) {


                string setResourceName = "Update Game Resource";
                if (AN_GoolgePlayRersources.GamesIds == null) {
                    EditorGUILayout.HelpBox("Before you start using Google Play API with the plugin" +
                   "You must first configure your game in the Google Play Developer Console, " +
                   "and then define google play resources using the plugin.",
                                       MessageType.Warning);
                    using (new SA_GuiBeginHorizontal()) {
                        GUILayout.FlexibleSpace();
                        bool click = m_configureYourGameLink.DrawWithCalcSize();
                        if (click) {
                            Application.OpenURL("https://unionassets.com/android-native-pro/getting-started-670");
                        }
                    }

                    setResourceName = "Set Game Resource";
                }
                else {
                    string applicationIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
                    if (!applicationIdentifier.Equals(AN_GoolgePlayRersources.GamesIds.PackageName)) {
                        EditorGUILayout.HelpBox("Player Settings Package Name does not match with " +
                            "Android Games Package Name \n" +
                            "unity: " + applicationIdentifier + "\n" +
                            "games-ids.xml: " + AN_GoolgePlayRersources.GamesIds.PackageName,
                                       MessageType.Warning);
                    }
                    using (new SA_GuiBeginVertical(EditorStyles.helpBox)) {
                        SA_EditorGUILayout.SelectableLabel("App Id", AN_GoolgePlayRersources.GamesIds.AppId);
                        SA_EditorGUILayout.SelectableLabel("Package Name", AN_GoolgePlayRersources.GamesIds.PackageName);

                        m_achievmentsShown = EditorGUILayout.Foldout(m_achievmentsShown, "Achievments");
                        if (m_achievmentsShown) {
                            if (AN_GoolgePlayRersources.GamesIds.Achievements.Count > 0) {
                                AN_GoolgePlayRersources.GamesIds.Achievements.ForEach(pair => {
                                    SA_EditorGUILayout.SelectableLabel(pair.Key, pair.Value);
                                });
                            }
                            else {
                                EditorGUILayout.LabelField("There are no achievments in games-ids.xml");
                            }
                        }

                        m_leaderboardsShown = EditorGUILayout.Foldout(m_leaderboardsShown, "Leaderboards");
                        if (m_leaderboardsShown) {
                            if (AN_GoolgePlayRersources.GamesIds.Leaderboards.Count > 0) {
                                AN_GoolgePlayRersources.GamesIds.Leaderboards.ForEach(pair => {
                                    SA_EditorGUILayout.SelectableLabel(pair.Key, pair.Value);
                                });
                            }
                            else {
                                EditorGUILayout.LabelField("There are no leaderboards in games-ids.xml");
                            }
                        }
                    }

                }

                using (new SA_GuiBeginHorizontal()) {
                    GUILayout.FlexibleSpace();
                    m_setResource.Content.text = setResourceName;
                    bool click = m_setResource.DrawWithCalcSize();
                    if (click) {
                        AN_GoolgePlayRersourcesWindow.ShowModal();
                    }
                }


            }



            using (new SA_WindowBlockWithSpace(new GUIContent("Google Mobile Services APIs"))) {
                EditorGUILayout.HelpBox("In order to access Google Play games services functionality, " +
                    "your game needs to provide the signed-in player’s account. If the player is not authenticated, " +
                    "your game may encounter errors when making calls to the Google Play games services APIs.",
                                        MessageType.Info);


                using (new SA_GuiBeginHorizontal()) {
                    EditorGUILayout.LabelField(SingInLabelContent);
                    using (new SA_GuiEnable(false)) {
                        SA_EditorGUILayout.ToggleFiled(new GUIContent(), true, SA_StyledToggle.ToggleType.EnabledDisabled);
                    }
                }
                    
                AN_Settings.Instance.GooglePlayGamesAPI = SA_EditorGUILayout.ToggleFiled(GamesLabelContent, AN_Settings.Instance.GooglePlayGamesAPI, SA_StyledToggle.ToggleType.EnabledDisabled);
            }
        }
    }
}