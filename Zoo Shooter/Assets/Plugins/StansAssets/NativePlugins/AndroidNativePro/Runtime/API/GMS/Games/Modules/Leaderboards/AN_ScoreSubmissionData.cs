using UnityEngine;
using System;
using System.Collections;

using SA.Android.Utilities;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Games
{

    /// <summary>
    /// Data object representing the result of submitting a score to a leaderboard.
    /// </summary>
    [Serializable]
    public class AN_ScoreSubmissionData : AN_LinkedObject {


        /// <summary>
        /// Retrieves the ID of the leaderboard the score was submitted to.
        /// </summary>
        public string LeaderboardId {
            get {
                return AN_GMS_Lib.Leaderboards.ScoreSubmissionData_GetLeaderboardId(this);
            }
        }

        /// <summary>
        /// Retrieves the ID of the player the score was submitted for.
        /// </summary>
        public string PlayerId {
            get {
                return AN_GMS_Lib.Leaderboards.ScoreSubmissionData_GetPlayerId(this);
            }
        }


        /// <summary>
        /// Retrieves the <see cref="Result"/> object for the given time span, if any.
        /// </summary>
        public Result GetScoreResult(AN_Leaderboard.TimeSpan timeSpan) {
            return AN_GMS_Lib.Leaderboards.ScoreSubmissionData_GetScoreResult(this, (int) timeSpan);
        }




        //--------------------------------------
        // AN_ScoreSubmissionDataResult
        //--------------------------------------


        /// <summary>
        /// Simple data class containing the result data for a particular time span.
        /// </summary>
        [Serializable]
        public class Result : AN_LinkedObject
        {

            /// <summary>
            /// Containing the score data in a display-appropriate format.
            /// </summary>
            public string FormattedScore {
                get {
                    return AN_GMS_Lib.Leaderboards.ScoreSubmissionDataResult_GetFormattedScore(this);
                }
            }

            /// <summary>
            /// Indicating whether or not this score was the player's new best score for this time span.
            /// </summary>
            public bool NewBest {
                get {
                    return AN_GMS_Lib.Leaderboards.ScoreSubmissionDataResult_GetNewBest(this);
                }
            }

            /// <summary>
            /// The raw score value of this score result.
            /// </summary>
            public long RawScore {
                get {
                    return AN_GMS_Lib.Leaderboards.ScoreSubmissionDataResult_GetRawScore(this);
                }
            }

            /// <summary>
            /// The score tag associated with this result, if any.
            /// </summary>
            public string ScoreTag {
                get {
                    return AN_GMS_Lib.Leaderboards.ScoreSubmissionDataResult_GetScoreTag(this);
                }
            }

        }

    }
}