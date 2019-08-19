namespace SA.Android.GMS.Games.Multiplayer
{
    /// <summary>
    /// The callback invoked when the state of the room has changed.
    /// </summary>
    public interface AN_iRoomUpdateCallback
    {
        /// <summary>
        /// Called when the client attempts to join a real-time room.
        /// Called when the client attempts to join a real-time room.
        /// The real-time room can be joined by calling the <see cref="AN_RealTimeMultiplayerClient.Join"/> operation.
        ///
        /// Possible status codes include:
        /// <see cref="AN_GamesCallbackStatusCodes.OK"/> if data was successfully loaded and is up-to-date.
        /// <see cref="AN_GamesCallbackStatusCodes.REAL_TIME_CONNECTION_FAILED"/> if the client failed to connect to the network
        /// <see cref="AN_GamesCallbackStatusCodes.MULTIPLAYER_DISABLED"/> if the game does not support multiplayer.
        /// <see cref="AN_GamesCallbackStatusCodes.INTERNAL_ERROR"/> if an unexpected error occurred in the service.
        /// </summary>
        /// <param name="statusCode">A status code indicating the result of the operation.</param>
        /// <param name="room">
        /// The data of the room that was joined.
        /// The room can be null if the <see cref="AN_RealTimeMultiplayerClient.Join"/> operation failed.
        /// </param>
        void OnJoinedRoom(int statusCode, AN_Room room);

        /// <summary>
        /// Called when the client attempts to leaves the real-time room.
        ///
        /// Possible status codes include:
        /// <see cref="AN_GamesCallbackStatusCodes.OK"/> if operation was successfully completed.
        /// <see cref="AN_GamesCallbackStatusCodes.INTERNAL_ERROR"/> if an unexpected error occurred in the service.
        /// </summary>
        /// <param name="statusCode">A status code indicating the result of the operation.</param>
        /// <param name="roomId">ID of the real-time room which was left.</param>
        void OnLeftRoom(int statusCode, string roomId);

        /// <summary>
        /// Called when all the participants in a real-time room are fully connected.
        /// This gets called once all invitations are accepted and any necessary automatching has been completed.
        ///
        /// Possible status codes include:
        /// <see cref="AN_GamesCallbackStatusCodes.OK"/> if data was successfully loaded and is up-to-date.
        /// <see cref="AN_GamesCallbackStatusCodes.INTERNAL_ERROR"/> if an unexpected error occurred in the service.
        /// </summary>
        /// <param name="statusCode">A status code indicating the result of the operation.</param>
        /// <param name="room">
        /// The fully connected room object.
        /// The room can be null if it could not be loaded successfully.
        /// </param>
        void OnRoomConnected(int statusCode, AN_Room room);

        /// <summary>
        /// Called when the client attempts to create a real-time room.
        /// The real-time room can be created by calling the <see cref="AN_RealTimeMultiplayerClient.Create"/> operation.
        ///
        /// Possible status codes include:
        /// <see cref="AN_GamesCallbackStatusCodes.OK"/> if data was successfully loaded and is up-to-date.
        /// <see cref="AN_GamesCallbackStatusCodes.REAL_TIME_CONNECTION_FAILED"/> if the client failed to connect to the network
        /// <see cref="AN_GamesCallbackStatusCodes.MULTIPLAYER_DISABLED"/> if the game does not support multiplayer.
        /// <see cref="AN_GamesCallbackStatusCodes.INTERNAL_ERROR"/> if an unexpected error occurred in the service.
        /// </summary>
        /// <param name="statusCode">A status code indicating the result of the operation.</param>
        /// <param name="room">
        /// The room data that was created if successful.
        /// The room can be null if the <see cref="AN_RealTimeMultiplayerClient.Create"/> operation failed.
        /// </param>
        void OnRoomCreated(int statusCode, AN_Room room);
    }
}