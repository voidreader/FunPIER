using System.Collections.Generic;

namespace SA.Android.GMS.Games.Multiplayer
{
    /// <summary>
    /// The callback invoked when the status of a room,
    /// status of its participants or connection status of the participants has changed.
    /// </summary>
    public interface AN_iRoomStatusUpdateCallback
    {
        /// <summary>
        /// Called when the client is connected to the connected set in a room.
        /// </summary>
        /// <param name="room">
        /// The room data with the status of a room and its participants.
        /// The room can be null if it could not be loaded successfully.
        /// </param>
        void OnConnectedToRoom(AN_Room room);

        /// <summary>
        /// Called when the client is disconnected from the connected set in a room.
        /// </summary>
        /// <param name="room">
        /// 	The room data with the status of a room and its participants.
        /// The room can be null if it could not be loaded successfully.
        /// </param>
        void OnDisconnectedFromRoom(AN_Room room);

        /// <summary>
        /// Called when the client is successfully connected to a peer participant.
        /// </summary>
        /// <param name="participantId">
        /// ID of the peer participant who was successfully connected.
        /// </param>
        void OnP2PConnected(string participantId);

        /// <summary>
        /// Called when client gets disconnected from a peer participant.
        /// </summary>
        /// <param name="participantId">ID of the peer participant who was disconnected.</param>
        void OnP2PDisconnected(string participantId);

        /// <summary>
        /// Called when one or more peers decline the invitation to a room.
        /// </summary>
        /// <param name="room">
        /// The room data with the status of a room and its participants.
        /// The room can be null if it could not be loaded successfully.
        /// </param>
        /// <param name="participantIds">IDs of the peers invited to a room.</param>
        void OnPeerDeclined(AN_Room room, List<string> participantIds);

        /// <summary>
        /// Called when one or more peers are invited to a room.
        /// </summary>
        /// <param name="room">
        /// The room data with the status of a room and its participants.
        /// The room can be null if it could not be loaded successfully.
        /// </param>
        /// <param name="participantIds">IDs of the peers invited to a room.</param>
        void OnPeerInvitedToRoom(AN_Room room, List<string> participantIds);

        /// <summary>
        /// Called when one or more peer participants join a room.
        /// </summary>
        /// <param name="room">
        /// The room data with the status of a room and its participants.
        /// The room can be null if it could not be loaded successfully.
        /// </param>
        /// <param name="participantIds">IDs of peer participants who joined a room.</param>
        void OnPeerJoined(AN_Room room, List<string> participantIds);

        /// <summary>
        /// Called when one or more peer participant leave a room.
        /// </summary>
        /// <param name="room">
        /// The room data with the status of a room and its participants.
        /// The room can be null if it could not be loaded successfully.
        /// </param>
        /// <param name="participantIds">IDs of peer participants who left the room.</param>
        void OnPeerLeft(AN_Room room, List<string> participantIds);

        /// <summary>
        /// Called when one or more peer participants are connected to a room.
        /// </summary>
        /// <param name="room">
        /// The room data with the status of a room and its participants.
        /// The room can be null if it could not be loaded successfully.
        /// </param>
        /// <param name="participantIds">IDs of peer participants who were connected.</param>
        void OnPeersConnected(AN_Room room, List<string> participantIds);

        /// <summary>
        /// Called when one or more peer participants are disconnected from a room.
        /// </summary>
        /// <param name="room">
        /// The room data with the status of a room and its participants.
        /// The room can be null if it could not be loaded successfully.
        /// </param>
        /// <param name="participantIds">IDs of peer participants who were disconnected.</param>
        void OnPeersDisconnected(AN_Room room, List<string> participantIds);

        /// <summary>
        /// Called when the server has started the process of auto-matching.
        /// Any invited participants must have joined and fully connected to each other before this will occur.
        /// </summary>
        /// <param name="room">he room data with the status of a room and its participants.
        /// The room can be null if it could not be loaded successfully.
        /// </param>
        void OnRoomAutoMatching(AN_Room room);

        /// <summary>
        /// Called when one or more participants have joined the room
        /// and have started the process of establishing peer connections.
        /// </summary>
        /// <param name="room">
        /// The room data with the status of a room and its participants. The room can be null if it could not be loaded successfully.
        /// </param>
        void OnRoomConnecting(AN_Room room);
    }
}