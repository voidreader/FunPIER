namespace SA.Android.GMS.Games.Multiplayer
{
    /// <summary>
    /// Listener for message received callback, which is called when the client receives a message from a peer.
    /// </summary>
    public interface AN_iOnRealTimeMessageReceivedListener
    {
        /// <summary>
        /// Called to notify the client that a reliable or unreliable message was received for a room.
        /// </summary>
        /// <param name="message">The message that was received.</param>
        void OnRealTimeMessageReceived(AN_RealTimeMessage message);
    }
}