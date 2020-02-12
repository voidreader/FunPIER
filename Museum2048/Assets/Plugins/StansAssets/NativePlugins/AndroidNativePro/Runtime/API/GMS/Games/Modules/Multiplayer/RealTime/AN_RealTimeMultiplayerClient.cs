using System;
using  UnityEngine;
using SA.Android.Utilities;

namespace SA.Android.GMS.Games.Multiplayer
{
    /// <summary>
    /// A client to interact with real time multiplayer functionality.
    /// </summary>
    public class AN_RealTimeMultiplayerClient : AN_JavaObject
    {
        /// <summary>
        /// Called to notify the client that a reliable message was sent for a room. Possible status codes include:
        /// <see cref="AN_GamesCallbackStatusCodes.OK"/> if the message was successfully sent.
        /// <see cref="AN_GamesCallbackStatusCodes.REAL_TIME_MESSAGE_SEND_FAILED"/> if the attempt to send message failed due to network error.
        /// <see cref="AN_GamesCallbackStatusCodes.REAL_TIME_ROOM_NOT_JOINED"/>  if the attempt to send message failed because the user has not joined the room.
        /// </summary>
        [Serializable]
        public class ReliableMessageSentCallback
        {
        
            [SerializeField] private int m_StatusCode = 0;
            [SerializeField] private int m_TokenId = 0;
            [SerializeField] private string m_RecipientParticipantId = string.Empty;

            /// <summary>
            /// A status code indicating the result of the operation.
            /// </summary>
            public int StatusCode
            {
                get { return m_StatusCode; }
            }
            
            /// <summary>
            /// The ID of the message which was sent.
            /// </summary>
            public int TokenId
            {
                get { return m_TokenId; }
            }
            
            /// <summary>
            /// The participant ID of the peer to whom the message was sent.
            /// </summary>
            public string RecipientParticipantId
            {
                get { return m_RecipientParticipantId; }
            }
        }

        public void Join(AN_RoomConfig config)
        {
            CallStatic("Join", config.HashCode);
        }
        
        public void Create(AN_RoomConfig config)
        {
            CallStatic("Create", config.HashCode);
        }
        
        public void Leave(AN_RoomConfig config, string roomId)
        {
            CallStatic("Leave", config.HashCode, roomId);
        }
        
        public void SendUnreliableMessage (byte[] messageData, string roomId, string recipientParticipantId)
        {
            CallStatic("SendUnreliableMessage", 
                Convert.ToBase64String(messageData), 
                roomId,
                recipientParticipantId);
        }

        public void SendReliableMessage(byte[] messageData, string roomId, string recipientParticipantId, Action<ReliableMessageSentCallback> callback)
        {
            var javaRequestBuilder = new AN_JavaRequestBuilder(JavaClassName, "SendReliableMessage");
            javaRequestBuilder.AddArgument(HashCode);
            javaRequestBuilder.AddArgument(Convert.ToBase64String(messageData));
            javaRequestBuilder.AddArgument(roomId);
            javaRequestBuilder.AddArgument(recipientParticipantId);
            javaRequestBuilder.AddCallback(callback);
            
            javaRequestBuilder.Invoke();
        }

        internal AN_RealTimeMultiplayerClient(int hasCode) : base(hasCode)
        {
            
        }

        protected override string JavaClassName
        {
            get { return "com.stansassets.gms.games.multiplayer.realtime.AN_RealTimeMultiplayerClient"; }
        }
        
    }
}