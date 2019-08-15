namespace SA.Android.GMS.Games
{
    /// <summary>
    /// Status codes for Games callbacks.
    /// </summary>
    public static class AN_GamesCallbackStatusCodes
    {
        /// <summary>
        /// An unspecified error occurred; no more specific information is available.
        /// The device logs may provide additional data.
        /// </summary>
        public static int INTERNAL_ERROR = 1;

        /// <summary>
        /// This game does not support multiplayer.
        /// This could occur if the linked app is not configured appropriately in the developer console.
        /// </summary>
        public static int MULTIPLAYER_DISABLED = 6003;

        /// <summary>
        /// The operation was successful.
        /// </summary>
        /// <returns></returns>
        public static int OK = 0;

        /// <summary>
        /// Failed to initialize the network connection for a real-time room.
        /// </summary>
        public static int REAL_TIME_CONNECTION_FAILED = 7000;

        /// <summary>
        /// Failed to send message to the peer participant for a real-time room.
        /// </summary>
        public static int REAL_TIME_MESSAGE_SEND_FAILED = 7001;

        /// <summary>
        /// Failed to send message to the peer participant for a real-time room,
        /// because the user has not joined the room.
        /// </summary>
        public static int REAL_TIME_ROOM_NOT_JOINED = 7004;

        /// <summary>
        /// Get the string associated with the status code.
        /// This can be used for clearer logging messages to avoid having to look up error codes.
        /// </summary>
        /// <param name="statusCode">The status code to get the message string for.</param>
        /// <returns>String associated with the status code.</returns>
        public static string getStatusCodeString(int statusCode)
        {
            return "Not yet implemented. Contact plugin developer";
        }
    }
}