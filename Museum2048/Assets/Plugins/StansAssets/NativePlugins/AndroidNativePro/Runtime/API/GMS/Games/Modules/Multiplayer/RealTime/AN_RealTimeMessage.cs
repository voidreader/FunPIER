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
        [SerializeField] private string m_SenderParticipantId = string.Empty;
        [SerializeField] private bool m_IsReliable = false;
        [SerializeField] private string m_MessageData = string.Empty;

        public const int RELIABLE = 1;
        public const int UNRELIABLE = 0;
        
        /// <summary>
        /// The participant ID of the message sender.
        /// </summary>
        public string SenderParticipantId
        {
            get { return m_SenderParticipantId; }
        }

        /// <summary>
        /// Whether this message was sent over a reliable channel.
        /// </summary>
        public bool IsReliable
        {
            get { return m_IsReliable; }
        }

        /// <summary>
        /// The message data.
        /// </summary>
        public byte[] MessageData
        {
            get { return Convert.FromBase64String(m_MessageData);; }
        }
    }
}