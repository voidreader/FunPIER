using System;
using UnityEngine;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// Data object representing the current level information of a player in the metagame.
    /// A <see cref="AN_PlayerLevelInfo"/> has four components:
    /// the player's current XP,
    /// the timestamp of the player's last level-up,
    /// the player's current level,
    /// and the player's next level.
    /// </summary>
    [Serializable]
    public class AN_PlayerLevelInfo
    {
        [SerializeField]
        AN_PlayerLevel m_CurrentLevel = null;
        [SerializeField]
        long m_CurrentXpTotal = 0;
        [SerializeField]
        long m_LastLevelUpTimestamp = 0;
        [SerializeField]
        AN_PlayerLevel m_NextLevel = null;
        [SerializeField]
        bool m_IsMaxLevel = false;

        /// <summary>
        /// Getter for the player's current level object.
        /// This object will be the same as the one returned from <see cref="NextLevel"/>
        /// if the player reached the maximum level.
        /// See <see cref="IsMaxLevel"/>
        /// </summary>
        public AN_PlayerLevel CurrentLevel => m_CurrentLevel;

        /// <summary>
        /// The player's current XP value.
        /// </summary>
        public long CurrentXpTotal => m_CurrentXpTotal;

        /// <summary>
        /// The timestamp of the player's last level-up.
        /// </summary>
        public long LastLevelUpTimestamp => m_LastLevelUpTimestamp;

        /// <summary>
        ///   Getter for the player's next level object.
        /// This object will be the same as the one returned from <see cref="CurrentLevel"/>
        /// if the player reached the maximum level.
        ///  See isMaxLevel()
        /// </summary>
        public AN_PlayerLevel NextLevel => m_NextLevel;

        /// <summary>
        /// Returns true if the player reached the maximum level
        /// (<see cref="CurrentLevel"/> is the same as <see cref="NextLevel"/>
        /// </summary>
        public bool IsMaxLevel => m_IsMaxLevel;
    }
}
