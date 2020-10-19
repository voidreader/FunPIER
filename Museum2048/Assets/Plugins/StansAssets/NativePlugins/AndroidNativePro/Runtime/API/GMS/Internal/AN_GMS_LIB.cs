using UnityEngine;
using SA.Foundation.Utility;

namespace SA.Android.GMS.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    static class AN_GMS_Lib
    {
        //--------------------------------------
        // Auth
        //--------------------------------------

        /// <summary>
        /// Returns a new instance of <see cref="AN_iGMS_AuthAPI"/>
        /// </summary>
        static AN_iGMS_AuthAPI s_Auth;

        public static AN_iGMS_AuthAPI Auth
        {
            get
            {
                if (!AN_Settings.Instance.GooglePlay) SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay");

                if (s_Auth == null)
                {
                    if (Application.platform == RuntimePlatform.Android)
                        s_Auth = new AN_GMS_Native_AuthAPI();
                    else
                        s_Auth = new AN_GMS_Editor_AuthAPI();
                }

                return s_Auth;
            }
        }

        //--------------------------------------
        // Players
        //--------------------------------------

        /// <summary>
        /// Returns a new instance of <see cref="AN_iGMS_PlayersAPI"/>
        /// </summary>
        static AN_iGMS_PlayersAPI s_Players;

        public static AN_iGMS_PlayersAPI Players
        {
            get
            {
                if (!AN_Settings.Instance.GooglePlay || !AN_Settings.Instance.GooglePlayGamesAPI) SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Players API");

                if (s_Players == null)
                {
                    if (Application.platform == RuntimePlatform.Android)
                        s_Players = new AN_GMS_Native_PlayersAPI();
                    else
                        s_Players = new AN_GMS_Editor_PlayersAPI();
                }

                return s_Players;
            }
        }

        //--------------------------------------
        // Games
        //--------------------------------------

        static AN_iGMS_GamesAPI s_Games;

        public static AN_iGMS_GamesAPI Games
        {
            get
            {
                if (!AN_Settings.Instance.GooglePlay || !AN_Settings.Instance.GooglePlayGamesAPI) SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Games API");

                if (s_Games == null)
                {
                    if (Application.platform == RuntimePlatform.Android)
                        s_Games = new AN_GMS_Native_GamesAPI();
                    else
                        s_Games = new AN_GMS_Editor_GamesAPI();
                }

                return s_Games;
            }
        }

        //--------------------------------------
        // Achievements
        //--------------------------------------

        static AN_iGMS_AchievementsAPI s_Achievements;

        public static AN_iGMS_AchievementsAPI Achievements
        {
            get
            {
                if (!AN_Settings.Instance.GooglePlay || !AN_Settings.Instance.GooglePlayGamesAPI) SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Games API");

                if (s_Achievements == null)
                {
                    if (Application.platform == RuntimePlatform.Android)
                        s_Achievements = new AN_GMS_Native_AchievementAPI();
                    else
                        s_Achievements = new AN_GMS_Editor_AchievementAPI();
                }

                return s_Achievements;
            }
        }

        //--------------------------------------
        // Leaderboards
        //--------------------------------------

        static AN_iGMS_LeaderboardsAPI s_Leaderboards;

        public static AN_iGMS_LeaderboardsAPI Leaderboards
        {
            get
            {
                if (!AN_Settings.Instance.GooglePlay || !AN_Settings.Instance.GooglePlayGamesAPI) SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Games API");

                if (s_Leaderboards == null)
                {
                    if (Application.platform == RuntimePlatform.Android)
                        s_Leaderboards = new AN_GMS_Native_LeaderboardsAPI();
                    else
                        s_Leaderboards = new AN_GMS_Editor_LeaderboardsAPI();
                }

                return s_Leaderboards;
            }
        }

        //--------------------------------------
        // Saved Games
        //--------------------------------------

        static AN_iGMS_SnapshotsAPI s_Snapshots;

        public static AN_iGMS_SnapshotsAPI Snapshots
        {
            get
            {
                if (!AN_Settings.Instance.GooglePlay || !AN_Settings.Instance.GooglePlayGamesAPI) SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Games API");

                if (s_Snapshots == null)
                {
                    if (Application.platform == RuntimePlatform.Android)
                        s_Snapshots = new AN_GMS_Native_SnapshotsAPI();
                    else
                        s_Snapshots = new AN_GMS_Editor_SnapshotsAPI();
                }

                return s_Snapshots;
            }
        }

        //--------------------------------------
        // Common
        //--------------------------------------

        static AN_iGMS_Common m_common = null;

        public static AN_iGMS_Common Common
        {
            get
            {
                if (!AN_Settings.Instance.GooglePlay) SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Games API");

                if (m_common == null)
                {
                    if (Application.platform == RuntimePlatform.Android)
                        m_common = new AN_GMS_Native_CommonAPI();
                    else
                        m_common = new AN_GMS_Editor_CommonAPI();
                }

                return m_common;
            }
        }
    }
}
