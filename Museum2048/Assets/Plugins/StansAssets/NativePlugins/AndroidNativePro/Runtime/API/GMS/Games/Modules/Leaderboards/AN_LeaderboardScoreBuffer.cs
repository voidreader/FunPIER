using System;
using System.Collections.Generic;
using UnityEngine;


using SA.Android.Utilities;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Games
{

    /// <summary>
    /// Containing LeaderboardScore data.
    /// </summary>
    [Serializable]
    public class AN_LeaderboardScoreBuffer : AN_LinkedObject
    {


        [Serializable]
        public class ScoresList
        {
#pragma warning disable 414
            [SerializeField] List<AN_LeaderboardScore> m_scores = new List<AN_LeaderboardScore>();
#pragma warning restore 414

            public List<AN_LeaderboardScore> Scores {
                get {
                    return m_scores;
                }
            }
        }


        /// <summary>
        /// List of the loaded scores
        /// </summary>
        /// <value>The scores.</value>
        public List<AN_LeaderboardScore> Scores {
            get {

                ScoresList list = AN_GMS_Lib.Leaderboards.LeaderboardScoreBuffer_GetScores(this);
                return list.Scores;
            }
        }

    }
}