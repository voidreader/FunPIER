using System;
using SA.Android.GMS.Internal;
using SA.Android.Utilities;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// Data interface for retrieving  information.
    /// </summary>
    [Serializable]
    public class AN_Player : AN_LinkedObject
    {

        /// <summary>
        /// Retrieves the ID of this player.
        /// </summary>
        public string Id {
            get {
                return AN_GMS_Lib.Games.Player_GetPlayerId(this);
            }
        }

        /// <summary>
        /// Retrieves the display name for this player.
        /// </summary>
        public string DisplayName {
            get {
                return AN_GMS_Lib.Games.Player_GetDisplayName(this);
            }
        }

        /// <summary>
        /// Retrieves the URI for loading this player's hi-res profile image.
        /// </summary>
        public string HiResImageUri {
            get {
                return AN_GMS_Lib.Games.Player_GetHiResImageUri(this);
            }
        }


        /// <summary>
        /// Retrieves the URI for loading this player's icon-size profile image.
        /// </summary>
        public string IconImageUri {
            get {
                return AN_GMS_Lib.Games.Player_GetIconImageUri(this);
            }
        }


        /// <summary>
        /// Indicates whether this player has a hi-res profile image to display.
        /// </summary>
        public bool HasIconImage {
            get {
                return AN_GMS_Lib.Games.Player_HasIconImage(this);
            }
        }


        /// <summary>
        /// Indicates whether this player has a icon-size profile image to display.
        /// </summary>
        public bool HasHiResImage {
            get {
                return AN_GMS_Lib.Games.Player_HasHiResImage(this);
            }
        }

        public string Title {
            get {
                return AN_GMS_Lib.Games.Player_GetTitle(this);
            }
        }
    }
}