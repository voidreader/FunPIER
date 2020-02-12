using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;



namespace SA.Android.GMS.Games
{
    /// <summary>
    /// An achievements increment result.
    /// </summary>
    [Serializable]
    public class AN_AchievementIncrementResult : SA_Result
    {
        [SerializeField] private bool m_isUnlocked = false;

        //Editor use only
        public AN_AchievementIncrementResult():base () { }

        /// <summary>
        /// Indicates whether the achievement is now unlocked.
        /// </summary>
        public bool IsUnlocked {
            get {
                return m_isUnlocked;
            }
        }
    }
}