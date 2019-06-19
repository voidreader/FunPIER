using UnityEngine;
using System;
using System.Collections;

using SA.Android.Utilities;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Games
{
    [Serializable]
    public class AN_LeaderboardScore : AN_LinkedObject
    {
        
        /// <summary>
        /// Retrieves a formatted string to display for this rank.
        /// </summary>
        public string DisplayRank {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScore_GetDisplayRank(this);
            }
        }


        /// <summary>
        /// Retrieves a formatted string to display for this score.
        /// </summary>
        public string DisplayScore {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScore_GetDisplayScore(this);
            }
        }


        /// <summary>
        /// Retrieves the rank returned from the server for this score.
        /// </summary>
        public long Rank {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScore_GetRank(this);
            }
        }


        /// <summary>
        /// Retrieves the raw score value.
        /// </summary>
        public long RawScore {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScore_GetRawScore(this);
            }
        }

        /// <summary>
        /// Retrieves the player that scored this particular score.
        /// </summary>
        public AN_Player ScoreHolder {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScore_GetScoreHolder(this);
            }
        }


        /// <summary>
        /// Retrieves the name to display for the player who scored this score.
        /// </summary>
        public string ScoreHolderDisplayName {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScore_GetScoreHolderDisplayName(this);
            }
        }


        /// <summary>
        /// Retrieves the URI of the icon image to display for the player who scored this score.
        /// </summary>
        public string ScoreHolderIconImageUri {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScore_GetScoreHolderIconImageUri(this);
            }
        }

        /// <summary>
        /// Retrieves the URI of the hi-res image to display for the player who scored this score.
        /// </summary>
        public string ScoreHolderHiResImageUri {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScore_GetScoreHolderHiResImageUri(this);
            }
        }

        /// <summary>
        /// Retrieve the optional score tag associated with this score, if any.
        /// </summary>
        public string ScoreTag {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScore_GetScoreTag(this);
            }
        }

        /// <summary>
        /// Retrieves the timestamp (in milliseconds from epoch) at which this score was achieved.
        /// </summary>
        public long TimestampMillis {
            get {
                return AN_GMS_Lib.Leaderboards.LeaderboardScore_GetTimestampMillis(this);
            }
        }

    }
}