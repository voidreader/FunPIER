using System;
using System.Collections.Generic;
using SA.Android.OS;
using SA.Android.Utilities;
using UnityEngine;

namespace SA.Android.GMS.Games.Multiplayer
{
    [Serializable]
    public class AN_RoomConfig : AN_LinkedObject
    {
        /// <summary>
        /// Builder class for <see cref="AN_RoomConfig"/>.
        /// </summary>
        [Serializable]
        public class Builder 
        {
            [Serializable]
            private class RoomUpdate
            {
                [SerializeField] private int m_StatusCode = 0;
                [SerializeField] private int m_RoomHashCode = 0;
                [SerializeField] private string m_RoomId = string.Empty;
                
                [SerializeField] public string m_ParticipantId = string.Empty;
                [SerializeField] public List<string> m_ParticipantIds = null;

                public int StatusCode
                {
                    get { return m_StatusCode; }
                }
                public AN_Room Room
                {
                    get { return m_RoomHashCode != k_NullObjectHash ? new AN_Room(m_RoomHashCode) : null;; }
                }

                public string ParticipantId
                {
                    get { return m_ParticipantId; }
                }

                public List<string> ParticipantIds
                {
                    get { return m_ParticipantIds; }
                }

                public string RoomId
                {
                    get { return m_RoomId; }
                }
            }
            
#pragma warning disable 414
            [SerializeField] private List<string> m_PlayersToInvite = null;
            [SerializeField] private string m_InvitationIdToAccept = string.Empty;
            [SerializeField] private int m_Variant = AN_Room.ROOM_VARIANT_DEFAULT;
            [SerializeField] private int m_BundleHashCode = 0;
#pragma warning restore 414
          

            private AN_iOnRealTimeMessageReceivedListener m_RealTimeMessageReceivedListener;
            private AN_iRoomStatusUpdateCallback m_RoomStatusUpdateCallback;
            private AN_iRoomUpdateCallback m_RoomUpdateCallback;

            internal Builder(AN_iRoomUpdateCallback roomUpdateCallback)
            {
                m_RoomUpdateCallback = roomUpdateCallback;
            }

            /// <summary>
            /// Sets the auto-match criteria for the room.
            /// See <see cref="AN_RoomConfig.CreateAutoMatchCriteria"/>
            /// </summary>
            /// <param name="autoMatchCriteria"></param>
            public void SetAutoMatchCriteria(AN_Bundle autoMatchCriteria)
            {
                m_BundleHashCode = autoMatchCriteria.HashCode;
            }
            
            /// <summary>
            /// Add one or more player IDs to invite to the room.
            /// This should be set only when calling <see cref="AN_RealTimeMultiplayerClient.Create"/>
            /// </summary>
            /// <param name="playerIds">One or more player IDs to invite to the room.</param>
            public void AddPlayersToInvite(params string[] playerIds)
            {
                m_PlayersToInvite = new List<string>(playerIds);
            }

            /// <summary>
            /// Set the ID of the invitation to accept.
            /// This is required and should be set only when <see cref="AN_RealTimeMultiplayerClient.Join"/>.
            /// </summary>
            /// <param name="invitationId">The ID of the invitation to accept.</param>
            public void SetInvitationIdToAccept(string invitationId)
            {
                m_InvitationIdToAccept = invitationId;
            }

            /// <summary>
            /// Set the listener for message received from a connected peer in a room.
            /// If not using socket-based communication,
            /// a non-null listener must be provided here before constructing the <see cref="AN_RoomConfig"/> object.
            /// </summary>
            /// <param name="listener">
            /// The message received listener that is called to notify the client when it receives a message in a room.
            /// The listener is called on the main thread.
            /// </param>
            public void SetOnMessageReceivedListener(AN_iOnRealTimeMessageReceivedListener listener)
            {
                m_RealTimeMessageReceivedListener = listener;
            }

            /// <summary>
            /// Set the callback for room status changes.
            /// </summary>
            /// <param name="callback">
            /// The callback that is called to notify the client when the status of the room has changed.
            /// The callback is called on the main thread.
            /// </param>
            public void SetRoomStatusUpdateCallback(AN_iRoomStatusUpdateCallback callback)
            {
                m_RoomStatusUpdateCallback = callback;
            }

            /// <summary>
            /// Sets the variant for the room when calling create(RoomConfig).
            /// This is an optional, developer-controlled parameter describing the type of game to play, and is used for auto-matching criteria.
            /// Must be either a positive integer or <see cref="AN_Room.ROOM_VARIANT_DEFAULT"/> (the default) if not desired.
            ///
            /// Note that variants must match exactly.
            /// Thus, if you do not specify a variant, only other rooms created with ROOM_VARIANT_DEFAULT will be considered potential auto-matches.
            /// </summary>
            /// <param name="variant">The variant for the match.</param>
            public void SetVariant(int variant)
            {
                m_Variant = variant;
            }
            
            /// <summary>
            /// Builds a new <see cref="AN_RoomConfig"/> object.
            /// </summary>
            /// <returns>a new <see cref="AN_RoomConfig"/> object.</returns>
            public AN_RoomConfig Build()
            {
                //MakeBuilder
                var javaRequestBuilder = new AN_JavaRequestBuilder(k_RoomConfigJavaClass, "MakeBuilder");
                javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                {
                    m_RoomUpdateCallback.OnRoomCreated(result.StatusCode, result.Room);
                });
       
                javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                {
                    m_RoomUpdateCallback.OnJoinedRoom(result.StatusCode, result.Room);
                });
                
                javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                {
                    m_RoomUpdateCallback.OnLeftRoom(result.StatusCode, result.RoomId);
                });
                
                javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                {
                    m_RoomUpdateCallback.OnRoomConnected(result.StatusCode, result.Room);
                });
       
                var builderHashCode = javaRequestBuilder.Invoke<int>();

                //SetRealTimeMessageReceivedListener
                if (m_RealTimeMessageReceivedListener != null)
                {
                    javaRequestBuilder = new AN_JavaRequestBuilder(k_RoomConfigJavaClass, "SetRealTimeMessageReceivedListener");
                    javaRequestBuilder.AddArgument(builderHashCode);
                    
                    javaRequestBuilder.AddCallback<AN_RealTimeMessage>(message =>
                    {
                        m_RealTimeMessageReceivedListener.OnRealTimeMessageReceived(message);
                    });
                    
                    javaRequestBuilder.Invoke();
                }

                //RoomStatusUpdateCallback
                if (m_RoomStatusUpdateCallback != null)
                {
                    javaRequestBuilder = new AN_JavaRequestBuilder(k_RoomConfigJavaClass, "RoomStatusUpdateCallback");
                    javaRequestBuilder.AddArgument(builderHashCode);
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnRoomConnecting(result.Room);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnRoomAutoMatching(result.Room);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnPeerInvitedToRoom(result.Room, result.ParticipantIds);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnPeerDeclined(result.Room, result.ParticipantIds);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnPeerJoined(result.Room, result.ParticipantIds);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnPeerLeft(result.Room, result.ParticipantIds);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnConnectedToRoom(result.Room);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnDisconnectedFromRoom(result.Room);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnPeersConnected(result.Room, result.ParticipantIds);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnPeersDisconnected(result.Room, result.ParticipantIds);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnP2PConnected(result.ParticipantId);
                    });
                    
                    javaRequestBuilder.AddCallback<RoomUpdate>(result =>
                    {
                        m_RoomStatusUpdateCallback.OnP2PDisconnected(result.ParticipantId);
                    });
                    
                    javaRequestBuilder.Invoke();
                }
                
                //Builder Data
                javaRequestBuilder = new AN_JavaRequestBuilder(k_RoomConfigJavaClass, "BuildRoomConfig");
                javaRequestBuilder.AddArgument(builderHashCode);
                javaRequestBuilder.AddArgument(JsonUtility.ToJson(this));
               
                var configHashCode = javaRequestBuilder.Invoke<int>();
                
                return new AN_RoomConfig(configHashCode);
            }
        }

        private const string k_RoomConfigJavaClass = "com.stansassets.gms.games.multiplayer.realtime.AN_RoomConfig";
       
        /// <summary>
        /// Creates a builder for assembling a <see cref="AN_RoomConfig"/>.
        /// The provided listener is required, and must not be null.
        /// It will be invoked on the main thread when appropriate.
        /// </summary>
        /// <param name="callback">The callback to be invoked when the primary state of the room changes.</param>
        /// <returns>An instance of a builder.</returns>
        public static Builder NewBuilder(AN_iRoomUpdateCallback callback)
        {
            return new Builder(callback);
        }

        /// <summary>
        /// Creates an auto-match criteria <see cref="AN_Bundle"/> for a new invitation.
        /// Can be passed to <see cref="Builder.SetAutoMatchCriteria"/>.
        /// </summary>
        /// <param name="minAutoMatchPlayers">Minimum number of auto-matched players.</param>
        /// <param name="maxAutoMatchPlayers">Maximum number of auto-matched players.</param>
        /// <param name="exclusiveBitMask">
        /// Exclusive bitmasks for the automatching request.
        /// The logical AND of each pairing of automatching requests must equal zero for auto-match.
        /// If there are no exclusivity requirements for the game, this value should just be set to 0.
        /// </param>
        /// <returns>A bundle of auto-match criteria data.</returns>
        public static AN_Bundle CreateAutoMatchCriteria(int minAutoMatchPlayers, int maxAutoMatchPlayers,
            long exclusiveBitMask)
        {
            var json = AN_Java.Bridge.CallStatic<string>(k_RoomConfigJavaClass, "CreateAutoMatchCriteria",
                minAutoMatchPlayers,
                maxAutoMatchPlayers,
                exclusiveBitMask);

            return JsonUtility.FromJson<AN_Bundle>(json);
        }
        
        private AN_RoomConfig(int hasCode):base(hasCode) {}
    }
}