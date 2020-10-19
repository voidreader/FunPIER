using System;
using UnityEngine;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// Data interface for retrieving player information.
    /// </summary>
    [Serializable]
    public class AN_Player
    {
        /// <summary>
        /// Constant indicating that the current XP total for a player is not known.
        /// </summary>
        public static long CURRENT_XP_UNKNOWN = -1;

        /// <summary>
        /// Constant indicating that a timestamp for a player is not known.
        /// </summary>
        public static long TIMESTAMP_UNKNOWN = -1;

        [SerializeField]
        string m_BannerImageLandscapeUri = string.Empty;
        [SerializeField]
        string m_BannerImagePortraitUri = string.Empty;
        [SerializeField]
        string m_DisplayName = string.Empty;
        [SerializeField]
        string m_HiResImageUri = string.Empty;
        [SerializeField]
        string m_IconImageUri = string.Empty;
        [SerializeField]
        long m_LastPlayedWithTimestamp = 0;
        [SerializeField]
        AN_PlayerLevelInfo m_LevelInfo = null;
        [SerializeField]
        string m_PlayerId = string.Empty;
        [SerializeField]
        long m_RetrievedTimestamp = 0;
        [SerializeField]
        string m_Title = string.Empty;
        [SerializeField]
        bool m_HasHiResImage = false;
        [SerializeField]
        bool m_HasIconImage = false;

        /// <summary>
        ///  Retrieves the URI for loading this player's landscape banner image. Returns null if the player has no landscape banner image.
        ///
        /// To retrieve the Image from the Uri, use <see cref="AN_ImageManager"/>.
        /// </summary>
        public string BannerImageLandscapeUri => m_BannerImageLandscapeUri;

        /// <summary>
        /// Retrieves the URI for loading this player's portrait banner image. Returns null if the player has no portrait banner image.
        ///
        /// To retrieve the Image from the Uri, use <see cref="AN_ImageManager"/>.
        /// </summary>
        public string BannerImagePortraitUri => m_BannerImagePortraitUri;

        /// <summary>
        /// Retrieves the display name for this player.
        /// </summary>
        public string DisplayName => m_DisplayName;

        /// <summary>
        ///  Retrieves the URI for loading this player's hi-res profile image.
        /// Returns null if the player has no profile image.
        ///
        /// To retrieve the Image from the Uri, use <see cref="AN_ImageManager"/>.
        /// </summary>
        public string HiResImageUri => m_HiResImageUri;

        /// <summary>
        /// Retrieves the URI for loading this player's icon-size profile image. Returns null if the player has no profile image.
        ///
        /// To retrieve the Image from the Uri, use <see cref="AN_ImageManager"/>.
        /// </summary>
        public string IconImageUri => m_IconImageUri;

        /// <summary>
        /// Retrieves the timestamp at which this player last played a multiplayer game
        /// with the currently signed in user.
        /// If the timestamp is not found, this method returns <see cref="TIMESTAMP_UNKNOWN"/>.
        /// </summary>
        public long LastPlayedWithTimestamp => m_LastPlayedWithTimestamp;

        /// <summary>
        /// Retrieves the player level associated information if any exists.
        /// If no level information exists for this player, this method will return null.
        /// </summary>
        public AN_PlayerLevelInfo LevelInfo => m_LevelInfo;

        /// <summary>
        /// Retrieves the ID of this player.
        /// </summary>
        public string PlayerId => m_PlayerId;

        /// <summary>
        ///  Retrieves the timestamp at which this player record was last updated locally.
        /// </summary>
        public long RetrievedTimestamp => m_RetrievedTimestamp;

        /// <summary>
        /// Retrieves the title of the player.
        /// This is based on the player's gameplay activity in apps using Google Play Games services.
        /// Note that not all players have titles, and that a player's title may change over time.
        /// </summary>
        public string Title => m_Title;

        /// <summary>
        /// Indicates whether this player has a hi-res profile image to display.
        /// </summary>
        public bool HasHiResImage => m_HasHiResImage;

        /// <summary>
        /// Indicates whether this player has an icon-size profile image to display.
        /// </summary>
        public bool HasIconImage => m_HasIconImage;
    }
}
