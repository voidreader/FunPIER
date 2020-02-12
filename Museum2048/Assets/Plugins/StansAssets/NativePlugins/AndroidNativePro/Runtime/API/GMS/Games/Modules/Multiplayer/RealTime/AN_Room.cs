using System;
using System.Collections.Generic;
using SA.Android.OS;
using SA.Android.Utilities;
using UnityEngine;

namespace SA.Android.GMS.Games.Multiplayer
{
    /// <summary>
    /// Data interface for room functionality.
    /// </summary>
    public class AN_Room : AN_JavaObject
    {
        /// <summary>
        /// Constant returned by <see cref="GetStatus"/>
        /// indicating that the room is active and connections are established.
        /// </summary>
        public static int ROOM_STATUS_ACTIVE = 3;

        /// <summary>
        /// Constant returned by <see cref="GetStatus"/>
        /// indicating that one or more slots are waiting to be filled by auto-matching.
        /// </summary>
        public static int ROOM_STATUS_AUTO_MATCHING = 1;

        /// <summary>
        /// Constant returned by <see cref="GetStatus"/>
        /// indicating that this room is waiting for clients to connect to each other.
        /// </summary>
        public static int ROOM_STATUS_CONNECTING = 2;

        /// <summary>
        /// Constant returned by <see cref="GetStatus"/>
        /// indicating that the room has one or more players that have been invited and have not responded yet.
        /// </summary>
        public static int ROOM_STATUS_INVITING = 0;

        /// <summary>
        /// Constant used to indicate that the variant for a room is unspecified.
        /// </summary>
        public static int ROOM_VARIANT_DEFAULT = -1;

        internal AN_Room(int hasCode):base(hasCode) {}
        
        /// <summary>
        /// Retrieves the automatch criteria used to create or join this room, if any.
        /// May be null if the room has no automatch properties.
        /// </summary>
        public AN_Bundle AutoMatchCriteria
        {
            get
            {
                var json = CallStatic<string>("GetAutoMatchCriteria");
                return JsonUtility.FromJson<AN_Bundle>(json);
            }
        }

        /// <summary>
        /// Retrieves the estimated wait time for automatching to finish for players
        /// who are not automatched immediately,
        /// as measured from the time that the room entered the automatching pool.
        /// </summary>
        public int AutoMatchWaitEstimateSeconds 
        {
            get { return CallStatic<int>("GetAutoMatchWaitEstimateSeconds"); }
        }

        /// <summary>
        /// The server timestamp at which the room was created.
        /// </summary>
        public long CreationTimestamp
        {
            get { return CallStatic<long>("GetCreationTimestamp"); }
        }
        
        /// <summary>
        /// he ID of the participant who created this Room.
        /// Note that not all participants will see the same value for the creator.
        /// In the case of an automatch, this value may differ for each participant.
        /// </summary>
        public string CreatorId
        {
            get { return CallStatic<string>("GetCreatorId"); }
        }

        /// <summary>
        /// Description of this room.
        /// </summary>
        public string Description
        {
            get { return CallStatic<string>("GetDescription"); }
        }

        /// <summary>
        /// Get a participant in a room by its ID.
        /// Note that the participant ID must correspond to a participant in this match,
        /// or this method will throw an exception.
        /// </summary>
        /// <param name="participantId">Match-local ID of the participant to retrieve status for.</param>
        /// <returns>The participant corresponding to the given ID.</returns>
        public AN_Participant GetParticipant(string participantId)
        {
             return CallStatic<AN_Participant>("GetParticipant", participantId);
        }

        /// <summary>
        /// Get the participant ID for a given player.
        /// This will only return a non-null ID if the player is actually a participant
        /// in the room and that player's identity is visible to the current player.
        /// Note that this will always return non-null for the current player.
        /// </summary>
        /// <param name="playerId">Player ID to find participant ID for.</param>
        /// <returns>The participant ID corresponding to given player, or null if none found.</returns>
        public string GetParticipantId(string playerId)
        {
            return CallStatic<string>("GetParticipantId", playerId);
        }

        /// <summary>
        /// The IDs of the participants in this room.
        /// These are returned in the participant order of the room.
        /// Note that these are not stable across rooms.
        /// </summary>
        public List<string> ParticipantIds
        {
            
            get { return CallStatic<AN_ParticipantIds>("GetParticipantIds").Ids; }
        }

        /// <summary>
        /// Get the status of a participant in a room.
        /// Note that the participant ID must correspond to a participant in this room,
        /// or this method will throw an exception.
        /// </summary>
        /// <param name="participantId">Room-local ID of the participant to retrieve status for.</param>
        /// <returns>
        /// The current status of the participant in this room.
        /// One of <see cref="AN_Participant.STATUS_INVITED"/>, <see cref="AN_Participant.STATUS_JOINED"/>,
        /// <see cref="AN_Participant.STATUS_DECLINED"/>, or <see cref="AN_Participant.STATUS_LEFT"/>.
        /// </returns>
        public int GetParticipantStatus(string participantId)
        {
            return CallStatic<int>("GetParticipantIds", participantId);
        }
        
        /// <summary>
        /// The ID of this Room.
        /// </summary>
        public string RoomId
        {
            get { return CallStatic<string>("GetRoomId"); }
        }
        
        /// <summary>
        /// The current status of the room.
        /// One of <see cref="ROOM_STATUS_INVITING"/>, <see cref="ROOM_STATUS_ACTIVE"/>,
        /// <see cref="ROOM_STATUS_AUTO_MATCHING"/>, <see cref="ROOM_STATUS_CONNECTING"/>.
        /// </summary>
        public int Status
        {
            get { return CallStatic<int>("GetStatus"); }
        }

        /// <summary>
        /// Variant specified for this room, if any.
        /// A variant is an optional developer-controlled parameter describing the type of game to play.
        /// If specified, this value will be a positive integer.
        /// If this room had no variant specified, returns <see cref="ROOM_VARIANT_DEFAULT"/>.
        /// </summary>
        public int Variant
        {
            get { return CallStatic<int>("Variant"); }
        }

        protected override string JavaClassName
        {
            get { return "com.stansassets.gms.games.multiplayer.realtime.AN_Room"; }
        }
        
        [Serializable]
        internal class AN_ParticipantIds  {
           [SerializeField] public List<string> Ids = new List<string>();
        }
    }
}