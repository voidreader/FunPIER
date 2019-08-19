using System;
using UnityEngine;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// Data interface for retrieving achievement information.
    /// </summary>
    [Serializable]
    public class AN_Achievement
    {

        public enum AchievementState 
        {
            UNLOCKED = 0,
            REVEALED = 1,
            HIDDEN = 2,
        }

        public enum AchievementType
        {
            STANDARD = 0,
            INCREMENTAL = 1,
        }

        [SerializeField] string m_achievementId = null;
        [SerializeField] string m_description = null;
        [SerializeField] string m_name = null;
        [SerializeField] string m_unlockedImageUri = null;

        [SerializeField] int m_currentSteps = 0;
        [SerializeField] int m_totalSteps = 0;
        [SerializeField] int m_type = 0;
        [SerializeField] int m_state = 0;
        [SerializeField] long m_xpValue = 0;
        

        /// <summary>
        /// The achievement ID.
        /// </summary>
        public string AchievementId {
            get {
                return m_achievementId;
            }
        }

        /// <summary>
        /// Retrieves the description for this achievement.
        /// </summary>
        public string Description {
            get {
                return m_description;
            }
        }

        /// <summary>
        /// Retrieves the name of this achievement.
        /// </summary>
        public string Name {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// Retrieves a URI that can be used to load the achievement's revealed image icon. 
        /// Returns null if the achievement has no revealed image.
        /// </summary>
        public string UnlockedImageUri {
            get {
                if(string.IsNullOrEmpty(m_unlockedImageUri)) {
                    return null;
                } else {
                    return m_unlockedImageUri;
                }
            }
        }

        /// <summary>
        /// Retrieves the number of steps this user has gone toward unlocking this achievement; 
        /// only applicable for <see cref="AchievementType.INCREMENTAL"/> achievement types.
        /// </summary>
        public int CurrentSteps {
            get {
                return m_currentSteps;
            }
        }

        /// <summary>
        /// Retrieves the total number of steps necessary to unlock this achievement; 
        /// only applicable for <see cref="AchievementType.INCREMENTAL"/> achievement types.
        /// </summary>
        public int TotalSteps {
            get {
                return m_totalSteps;
            }
        }

        /// <summary>
        /// Returns the Type of this achievement.
        /// </summary>
        public AchievementType Type {
            get {
                return (AchievementType) m_type;
            }
        }

        /// <summary>
        /// Returns the State of the achievement.
        /// </summary>
        public AchievementState State {
            get {
                return (AchievementState) m_state;
            }
        }

        /// <summary>
        /// Retrieves the XP value of this achievement.
        /// The XP value will be given to players for unlocking this achievement.
        /// </summary>
        public long XpValue {
            get {
                return m_xpValue;
            }
        }
    }
}