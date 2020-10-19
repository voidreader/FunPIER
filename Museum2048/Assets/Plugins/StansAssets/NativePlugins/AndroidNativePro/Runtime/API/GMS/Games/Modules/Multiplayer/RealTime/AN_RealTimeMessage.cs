using System;
using UnityEngine;

namespace SA.Android.GMS.Games.Multiplayer
{
    /// <summary>
    /// Message received from participants in a real-time room, which is passed to the client.
    /// </summary>
    [Serializable]
    public class AN_RealTimeMessage
    {
        [SerializeField]
        string m_SenderParticipantId = string.Empty;
        [SerializeField]
        bool m_IsReliable = false;
        [SerializeField]
        string m_MessageData = string.Empty;

        public const int RELIABLE = 1;
        public const int UNRELIABLE = 0;

        /// <summary>
        /// The participant ID of the message sender.
        /// </summary>
        public string SenderParticipantId => m_SenderParticipantId;

        /// <summary>
        /// Whether this message was sent over a reliable channel.
        /// </summary>
        public bool IsReliable => m_IsReliable;

        /// <summary>
        /// The message data.
        /// </summary>
        public byte[] MessageData => Convert.FromBase64String(m_MessageData);
    }
}
