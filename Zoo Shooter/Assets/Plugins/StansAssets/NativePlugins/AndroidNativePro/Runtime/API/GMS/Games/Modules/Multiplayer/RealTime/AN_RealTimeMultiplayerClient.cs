using System;
using SA.Android.Utilities;

namespace SA.Android.GMS.Games.Multiplayer
{
    /// <summary>
    /// A client to interact with real time multiplayer functionality.
    /// </summary>
    public class AN_RealTimeMultiplayerClient : AN_JavaObject
    {
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

        internal AN_RealTimeMultiplayerClient(int hasCode) : base(hasCode)
        {
            
        }

        internal override string JavaClassName
        {
            get { return "com.stansassets.gms.games.multiplayer.realtime.AN_RealTimeMultiplayerClient"; }
        }
        
    }
}