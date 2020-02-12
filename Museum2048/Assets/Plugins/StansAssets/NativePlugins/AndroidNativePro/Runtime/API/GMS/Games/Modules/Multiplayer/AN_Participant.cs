using System;
using SA.Android.GMS.Common.Images;
using UnityEngine;

namespace SA.Android.GMS.Games.Multiplayer
{
    /// <summary>
    /// Data interface for multiplayer participants.
    /// </summary>
    [Serializable]
    public class AN_Participant
    {
        /// <summary>
        /// Constant indicating that this participant has declined the invitation.
        /// </summary>
        /// <returns></returns>
        public static int STATUS_DECLINED = 3;

        /// <summary>
        ///  Constant indicating that this participant is finished with this match. Only applies to turn-based match participants.
        /// </summary>
        public static int STATUS_FINISHED = 5;
        
        /// <summary>
        /// Constant indicating that this participant has been sent an invitation.
        /// </summary>
        public static int STATUS_INVITED = 1;
        
        /// <summary>
        /// Constant indicating that this participant has accepted the invitation and is joined.
        /// </summary>
        public static int STATUS_JOINED = 2;
        
        /// <summary>
        /// Constant indicating that this participant joined a multiplayer game and subsequently left.
        /// </summary>
        public static int STATUS_LEFT = 4;
       
        /// <summary>
        ///  Constant indicating that this participant has not yet been sent an invitation. Only applies to turn-based match participants.
        /// </summary>
        public static int STATUS_NOT_INVITED_YET = 0;
       
        /// <summary>
        /// Constant indicating that this participant did not respond to the match in the alloted time.
        /// Only applies to turn-based match participants.
        /// </summary>
        public static int STATUS_UNRESPONSIVE = 6;

        [SerializeField] private string m_DisplayName = string.Empty;
        [SerializeField] private string m_HiResImageUri = string.Empty;
        [SerializeField] private string m_IconImageUri = string.Empty;
        [SerializeField] private string m_ParticipantId = string.Empty;
        [SerializeField] private AN_Player m_Player = null;
        [SerializeField] private AN_ParticipantResult m_Result = null;
        [SerializeField] private int m_Status = 0;
        [SerializeField] private bool m_IsConnectedToRoom = false;

        /// <summary>
        ///  Return the name to display for this participant.
        /// If the identity of the player is unknown, this will be a generic handle to describe the player.
        /// </summary>
        public string DisplayName 
        {
            get { return m_DisplayName; }
        }

        /// <summary>
        /// Returns the URI of the hi-res image to display for this participant.
        /// If the identity of the player is unknown, this will be null. It may also be null if the player simply has no image.
        ///
        /// To retrieve the Image from the Uri, use <see cref="AN_ImageManager"/>.
        /// </summary>
        public string HiResImageUri
        {
            get { return m_HiResImageUri; }
        }

        /// <summary>
        /// Returns the URI of the icon-sized image to display for this participant.
        /// If the identity of the player is unknown, this will be the automatch avatar icon image for the player. It may also be null if the player simply has no image.
        ///
        /// To retrieve the Image from the Uri, use <see cref="AN_ImageManager"/>.
        /// </summary>
        public string IconImageUri
        {
            get { return m_IconImageUri; }
        }

        /// <summary>
        ///  Returns the ID of this participant.
        /// Note that this is only valid for use in the current multiplayer room or match:
        /// a participant will not have the same ID across multiple rooms or matches.
        /// </summary>
        public string ParticipantId
        {
            get { return m_ParticipantId; }
        }

        /// <summary>
        ///  Returns the <see cref="AN_Player"/> that this participant represents.
        /// Note that this may be null if the identity of the player is unknown.
        /// This occurs in automatching scenarios where some players are not permitted
        /// to see the real identity of others.
        /// </summary>
        public AN_Player Player
        {
            get { return m_Player; }
        }
        
        /// <summary>
        /// Returns the <see cref="AN_ParticipantResult"/> associated with this participant, if any.
        /// Only applies to turn-based match participants.
        /// Null if not applicable.
        /// </summary>
        public AN_ParticipantResult Result
        {
            get { return m_Result; }
        }
       
        /// <summary>
        /// Possible status values for room participants are
        /// <see cref="STATUS_INVITED"/>, <see cref="STATUS_JOINED"/>,
        /// <see cref="STATUS_DECLINED"/>, and <see cref="STATUS_LEFT"/>.
        ///
        ///  Possible status values for turn-based match participants are all of the above,
        /// <see cref="STATUS_NOT_INVITED_YET"/>, <see cref="STATUS_FINISHED"/>, and <see cref="STATUS_UNRESPONSIVE"/>.
        /// </summary>
        public int Status
        {
            get { return m_Status; }
        }

        /// <summary>
        /// Retrieves the connected status of the participant.
        /// If true indicates that participant is in the connected set of the room.
        /// Only applies to room participants.
        /// </summary>
        public bool IsConnectedToRoom
        {
            get { return m_IsConnectedToRoom; }
        }
    }
}