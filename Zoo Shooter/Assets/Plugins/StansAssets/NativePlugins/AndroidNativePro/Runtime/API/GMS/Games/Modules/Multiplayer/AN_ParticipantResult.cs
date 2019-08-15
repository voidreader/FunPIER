using System;
using UnityEngine;

namespace SA.Android.GMS.Games.Multiplayer
{
    /// <summary>
    /// Data class used to report a participant's result in a match.
    /// </summary>
    [Serializable]
    public class AN_ParticipantResult
    {
        /// <summary>
        ///  Constant indicating that this participant had different results reported by different clients.
        /// </summary>
        public const int MATCH_RESULT_DISAGREED = 5;
       
        /// <summary>
        /// Constant indicating that this participant disconnected or left during the match.
        /// </summary>
        private const int MATCH_RESULT_DISCONNECT = 4;

        /// <summary>
        /// Constant indicating that this participant lost the match.
        /// </summary>
        public const int MATCH_RESULT_LOSS = 1;


        /// <summary>
        /// Constant indicating that this participant had no result for the match.
        /// </summary>
        public const int MATCH_RESULT_NONE = 3;
        
        /// <summary>
        ///  Constant indicating that this participant tied the match.
        /// </summary>
        public const int MATCH_RESULT_TIE = 2;

        /// <summary>
        /// Constant indicating that this participant has not reported a result at all yet.
        /// This will commonly be seen when the match is currently in progress.
        /// Note that this is distinct from MATCH_RESULT_NONE,
        /// </summary>
        public const int MATCH_RESULT_UNINITIALIZED = -1;

        /// <summary>
        /// Constant indicating that this participant won the match.
        /// </summary>
        public const int MATCH_RESULT_WIN = 0;
        
        /// <summary>
        /// Constant returned by <see cref="Placing"/>
        /// if the participant has not reported a placing in the match yet.
        /// Usually seen when a match is still in progress.
        /// </summary>
        public const int PLACING_UNINITIALIZED = -1;

        [SerializeField] private string m_ParticipantId = string.Empty;
        [SerializeField] private int m_Placing = 0;
        [SerializeField] private int m_Result = 0;

        /// <summary>
        /// The ID of the participant this result is for.
        /// </summary>
        public string ParticipantId
        {
            get { return m_ParticipantId; }
        }
        
        /// <summary>
        /// The placing of this participant in the match.
        /// <see cref="PLACING_UNINITIALIZED"/> means that this result has no placing value to report.
        /// </summary>
        public int Placing
        {
            get { return m_Placing; }
        }
        
        /// <summary>
        /// The result type for this participant in the match.
        /// One of <see cref="MATCH_RESULT_WIN"/>, <see cref="MATCH_RESULT_LOSS"/>,
        /// <see cref="MATCH_RESULT_TIE"/>, <see cref="MATCH_RESULT_NONE"/>,
        /// <see cref="MATCH_RESULT_DISCONNECT"/>, or <see cref="MATCH_RESULT_DISAGREED"/>.
        /// </summary>
        public int Result
        {
            get { return m_Result; }
        }
    }
}