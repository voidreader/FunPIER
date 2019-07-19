using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;
using SA.Android.Vending.Billing;

namespace SA.Android.GMS.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class AN_GMS_Lib
    {
        //--------------------------------------
        // Auth
        //--------------------------------------

        /// <summary>
        /// Returns a new instance of <see cref="AN_iGMS_AuthAPI"/>
        /// </summary>
        private static AN_iGMS_AuthAPI m_auth = null;
        public static AN_iGMS_AuthAPI Auth {
            get {

                if(!AN_Settings.Instance.GooglePlay) {
                    SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay");
                }

                
                if (m_auth == null) {
                    if(Application.platform == RuntimePlatform.Android) {
                        m_auth = new AN_GMS_Native_AuthAPI();
                    } else {
                        m_auth = new AN_GMS_Editor_AuthAPI();
                    }
                }

                return m_auth;
            }
        }

        //--------------------------------------
        // Players
        //--------------------------------------


        /// <summary>
        /// Returns a new instance of <see cref="AN_iGMS_PlayersAPI"/>
        /// </summary>
        private static AN_iGMS_PlayersAPI m_players = null;
        public static AN_iGMS_PlayersAPI Players {
            get {
                if (!AN_Settings.Instance.GooglePlay || !AN_Settings.Instance.GooglePlayGamesAPI) {
                    SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Players API");
                }

                if (m_players == null) {
                    if (Application.platform == RuntimePlatform.Android) {
                        m_players = new AN_GMS_Native_PlayersAPI();
                    } else {
                        m_players = new AN_GMS_Editor_PlayersAPI();
                    }
                }
                return m_players;
            }
        }


        //--------------------------------------
        // Games
        //--------------------------------------


        private static AN_iGMS_GamesAPI m_games = null;
        public static AN_iGMS_GamesAPI Games {
            get {
                if (!AN_Settings.Instance.GooglePlay  || !AN_Settings.Instance.GooglePlayGamesAPI) {
                    SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Games API");
                }

                if (m_games == null) {
                    if (Application.platform == RuntimePlatform.Android) {
                        m_games = new AN_GMS_Native_GamesAPI();
                    } else {
                        m_games = new AN_GMS_Editor_GamesAPI();
                    }
                }
                return m_games;
            }
        }


        //--------------------------------------
        // Achievements
        //--------------------------------------


        private static AN_iGMS_AchievementsAPI m_achievements = null;
        public static AN_iGMS_AchievementsAPI Achievements {
            get {
                if (!AN_Settings.Instance.GooglePlay || !AN_Settings.Instance.GooglePlayGamesAPI) {
                    SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Games API");
                }

                if (m_achievements == null) {
                    if (Application.platform == RuntimePlatform.Android) {
                        m_achievements = new AN_GMS_Native_AchievementAPI();
                    } else {
                        m_achievements = new AN_GMS_Editor_AchievementAPI();
                    }
                }
                return m_achievements;
            }
        }


        //--------------------------------------
        // Leaderboards
        //--------------------------------------


        private static AN_iGMS_LeaderboardsAPI m_leaderboards = null;
        public static AN_iGMS_LeaderboardsAPI Leaderboards {
            get {
                if (!AN_Settings.Instance.GooglePlay || !AN_Settings.Instance.GooglePlayGamesAPI) {
                    SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Games API");
                }

                if (m_leaderboards == null) {
                    if (Application.platform == RuntimePlatform.Android) {
                        m_leaderboards = new AN_GMS_Native_LeaderboardsAPI();
                    } else {
                        m_leaderboards = new AN_GMS_Editor_LeaderboardsAPI();
                    }
                }
                return m_leaderboards;
            }
        }

        //--------------------------------------
        // Saved Games
        //--------------------------------------


        private static AN_iGMS_SnapshotsAPI m_snapshots = null;
        public static AN_iGMS_SnapshotsAPI Snapshots {
            get {
                if (!AN_Settings.Instance.GooglePlay || !AN_Settings.Instance.GooglePlayGamesAPI) {
                    SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Games API");
                }

                if (m_snapshots == null) {
                    if (Application.platform == RuntimePlatform.Android) {
                        m_snapshots = new AN_GMS_Native_SnapshotsAPI();
                    } else {
                        m_snapshots = new AN_GMS_Editor_SnapshotsAPI();
                    }
                }
                return m_snapshots;
            }
        }


        //--------------------------------------
        // Common
        //--------------------------------------


        private static AN_iGMS_Common m_common = null;
        public static AN_iGMS_Common Common {
            get {
                if (!AN_Settings.Instance.GooglePlay) {
                    SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "GooglePlay Games API");
                }

                if (m_common == null) {
                    if (Application.platform == RuntimePlatform.Android) {
                        m_common = new AN_GMS_Native_CommonAPI();
                    } else {
                        m_common = new AN_GMS_Editor_CommonAPI();
                    }
                }
                return m_common;
            }
        }


    }

}
