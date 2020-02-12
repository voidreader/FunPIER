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
    public class AN_LeaderboardsLoadResult : SA_Result
    {
#pragma warning disable 414
        [SerializeField] List<AN_Leaderboard> m_leaderboards = new List<AN_Leaderboard>();
#pragma warning restore 414


        /// <summary>
        /// Loaded leaderboards info
        /// </summary>
        public List<AN_Leaderboard> Leaderboards {
            get {
                return m_leaderboards;
            }
        }
    }
}