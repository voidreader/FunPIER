using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;

namespace SA.Android.GMS.Games
{

    /// <summary>
    /// An achievements load result.
    /// </summary>
    [Serializable]
    public class AN_AchievementsLoadResult : SA_Result
    {
        [SerializeField] List<AN_Achievement> m_achievements = new List<AN_Achievement>();

        //Editor use only
        public AN_AchievementsLoadResult() : base() { }

        /// <summary>
        /// Gets loaded achievements list.
        /// </summary>
        public List<AN_Achievement> Achievements {
            get {
                return m_achievements;
            }
        }
    }
}