using System;
using UnityEngine;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// Data object representing a level a player can obtain in the metagame.
    ///
    /// A PlayerLevel has three components: a numeric value, and a range of XP totals it represents.
    /// A player is considered a given level if they have at least <see cref="MinXp"/> and less than <see cref="MaxXp"/>.
    /// </summary>
    [Serializable]
    public class AN_PlayerLevel
    {
        [SerializeField]
        int m_LevelNumber = 0;
        [SerializeField]
        long m_MaxXp = 0;
        [SerializeField]
        long m_MinXp = 0;

        /// <summary>
        ///  Returns the number for this level, e.g. "level 10".
        ///  This is the level that this object represents.
        /// For a player to be considered as being of this level,
        /// the value given by <see cref="AN_PlayerLevelInfo.CurrentXpTotal"/> must fall in the range [<see cref="MinXp"/>, <see cref="MaxXp"/>).
        /// </summary>
        public int LevelNumber => m_LevelNumber;

        /// <summary>
        /// The maximum XP value represented by this level, exclusive.
        /// </summary>
        public long MaxXp => m_MaxXp;

        /// <summary>
        /// The minimum XP value needed to attain this level, inclusive.
        /// </summary>
        public long MinXp => m_MinXp;
    }
}
