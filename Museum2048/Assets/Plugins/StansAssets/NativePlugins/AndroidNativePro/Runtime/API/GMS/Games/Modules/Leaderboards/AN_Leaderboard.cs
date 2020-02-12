using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace SA.Android.GMS.Games
{

    /// <summary>
    /// Data interface for leaderboard metadata.
    /// </summary>
    [Serializable]
    public class AN_Leaderboard 
    {
        /// <summary>
        /// Score order constant for leaderboards
        /// </summary>
        public enum ScoreOrder
        {

            /// <summary>
            /// Score order constant for leaderboards where scores are sorted in descending order.
            /// </summary>
            SMALLER_IS_BETTER = 0,

            /// <summary>
            /// Score order constant for leaderboards where scores are sorted in ascending order.
            /// </summary>
            LARGER_IS_BETTER = 1
        }


        public enum Collection
        {
            /// <summary>
            /// Collection constant for public leaderboards. 
            /// Public leaderboards contain the scores of players who are sharing their gameplay activity publicly.
            /// </summary>
            Public = 0,


            /// <summary>
            /// This constant was deprecated.
            /// Google+ no longer integrated so social APIs will not work as expected. 
            /// See Play Games authentication adopting Google Sign-In API for details
            /// <see cref="https://android-developers.googleblog.com/2016/12/games-authentication-adopting-google.html"/>
            /// </summary>
            Social = 1
        }

        public enum TimeSpan
        {
            /// <summary>
            /// Scores are reset every day. The reset occurs at 11:59PM PST.
            /// </summary>
            Daily = 0,

            /// <summary>
            /// Scores are reset once per week. The reset occurs at 11:59PM PST on Sunday.
            /// </summary>
            Weekly = 1,

            /// <summary>
            /// Scores are never reset.
            /// </summary>
            AllTime = 2
        }

        [SerializeField] string m_displayName = string.Empty;
        [SerializeField] string m_iconImageUri= string.Empty;
        [SerializeField] string m_leaderboardId= string.Empty;
        [SerializeField] private int m_scoreOrder = 0;

        [SerializeField] List<AN_LeaderboardVariant> m_variants = new List<AN_LeaderboardVariant>();



        /// <summary>
        /// Retrieves the ID of this leaderboard.
        /// </summary>
        public string LeaderboardId {
            get {
                return m_leaderboardId;
            }
        }



        /// <summary>
        /// Retrieves the display name of this leaderboard.
        /// </summary>
        public string DisplayName {
            get {
                return m_displayName;
            }
        }

        /// <summary>
        /// Retrieves an image URI that can be used to load this leaderboard's icon, or null if there was a problem retrieving the icon.
        /// </summary>
        public string IconImageUri {
            get {
                return m_iconImageUri;
            }
        }


        /// <summary>
        /// Retrieves the sort order of scores for this leaderboard.
        /// </summary>
        public ScoreOrder LeaderboardScoreOrder {
            get {
                return (ScoreOrder) m_scoreOrder;
            }
        }


        /// <summary>
        /// Retrieves the <see cref="AN_LeaderboardVariant"/>'s for this leaderboard.
        ///  These will be returned sorted by time span first, then by variant type.
        /// Note that these variants are volatile, and are tied to the lifetime of the original buffer.
        /// </summary>
        public List<AN_LeaderboardVariant> Variants {
            get {
                return m_variants;
            }
        }
    }
}