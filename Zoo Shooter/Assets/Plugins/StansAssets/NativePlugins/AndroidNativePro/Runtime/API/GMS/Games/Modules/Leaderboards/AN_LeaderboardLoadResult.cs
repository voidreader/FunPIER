using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;

namespace SA.Android.GMS.Games
{

    /// <summary>
    /// Leaderboards load result object.
    /// </summary>
    [Serializable]
    public class AN_LeaderboardLoadResult : SA_Result
    {

#pragma warning disable 414
        [SerializeField] AN_Leaderboard m_leaderboard = null;
#pragma warning restore 414

       


        /// <summary>
        /// Loaded leaderboards info
        /// </summary>
        public AN_Leaderboard Leaderboard {
            get {
                return m_leaderboard;
            }
        }
    }
}