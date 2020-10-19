using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// Data interface for a specific variant of a leaderboard; 
    /// a variant is defined by the combination of the leaderboard's collection (public or social) 
    /// and time span (daily, weekly, or all-time).
    /// </summary>
    [Serializable]
    public class AN_LeaderboardVariant
    {
        [SerializeField]
        int m_collection = 0;
        [SerializeField]
        string m_displayPlayerRank = null;
        [SerializeField]
        string m_displayPlayerScore = null;
        [SerializeField]
        long m_numScores = 0;
        [SerializeField]
        long m_playerRank = 0;
        [SerializeField]
        string m_playerScoreTag = null;
        [SerializeField]
        long m_rawPlayerScore = 0;
        [SerializeField]
        int m_timeSpan = 0;
        [SerializeField]
        bool m_hasPlayerInfo = false;

        /// <summary>
        /// Retrieves the collection of scores contained by this variant.
        /// </summary>
        public AN_Leaderboard.Collection Collection => (AN_Leaderboard.Collection)m_collection;

        /// <summary>
        /// Retrieves the viewing player's formatted rank for this variant, if any.
        /// </summary>
        public string DisplayPlayerRank => m_displayPlayerRank;

        /// <summary>
        /// Retrieves the viewing player's score for this variant, if any.
        /// </summary>
        public string DisplayPlayerScore => m_displayPlayerScore;

        /// <summary>
        /// Retrieves the total number of scores for this variant.
        /// </summary>
        public long NumScores => m_numScores;

        /// <summary>
        /// Retrieves the viewing player's rank for this variant, if any.
        /// </summary>
        public long PlayerRank => m_playerRank;

        /// <summary>
        /// Retrieves the viewing player's score tag for this variant, if any.
        /// </summary>
        public string PlayerScoreTag => m_playerScoreTag;

        /// <summary>
        /// Retrieves the viewing player's score for this variant, if any.
        /// </summary>
        public long RawPlayerScore => m_rawPlayerScore;

        /// <summary>
        /// Retrieves the time span that the scores for this variant are drawn from.
        /// </summary>
        public AN_Leaderboard.TimeSpan TimeSpan => (AN_Leaderboard.TimeSpan)m_timeSpan;

        /// <summary>
        /// Get whether or not this variant contains score information for the viewing player or not.
        /// </summary>
        public bool HasPlayerInfo => m_hasPlayerInfo;
    }
}
