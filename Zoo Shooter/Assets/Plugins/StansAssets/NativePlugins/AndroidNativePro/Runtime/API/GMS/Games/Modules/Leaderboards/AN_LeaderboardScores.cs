using UnityEngine;
using System;
using System.Collections;

using SA.Android.Utilities;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Games
{

    /// <summary>
    /// Result delivered when leaderboard scores have been loaded.
    /// </summary>
    [Serializable]
    public class AN_LeaderboardScores : AN_LinkedObject
    {

        /// <summary>
        /// The leaderboard that the requested scores belong to. 
        /// This may be null if the leaderboard metadata could not be found.
        /// </summary>
        public AN_Leaderboard Leaderboard {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScores_GetLeaderboard(this);
            }
        }

        /// <summary>
        /// The leaderboard scores that were requested. Could be empty. 
        /// The listener must close this object when finished.
        /// </summary>
        public AN_LeaderboardScoreBuffer Scores {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScores_GetScores(this);
            }
        }
    }
}