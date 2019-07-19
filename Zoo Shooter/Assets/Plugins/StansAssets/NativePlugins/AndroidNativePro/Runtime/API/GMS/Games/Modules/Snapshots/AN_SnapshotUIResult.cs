using UnityEngine;
using System;

using SA.Foundation.Templates;


namespace SA.Android.GMS.Games
{


    /// <summary>
    /// Object that desribed user interaction with the Snapshots UI.
    /// </summary>
    [Serializable]
    public class AN_SnapshotUIResult : SA_Result
    {

        public enum UserInteractionState {
            NOTHING_SELECTED = 0,

            /// <summary>
            ///  Indicate that user had selected snapshot.
            /// </summary>
            EXTRA_SNAPSHOT_METADATA = 1,

            /// <summary>
            /// Indicate that user wants to create a new snapshot.
            /// </summary>
            EXTRA_SNAPSHOT_NEW = 2
        }

        [SerializeField] private int m_state = 0;
        [SerializeField] private AN_SnapshotMetadata m_metadata = null;


        //Edior use only
        public AN_SnapshotUIResult(int state) :base() {
            m_state = state;
        }


        /// <summary>
        /// The state us user interaction with UI
        /// </summary>
        public UserInteractionState State {
            get {
                return (UserInteractionState )m_state;
            }
        }


        /// <summary>
        /// Selected Snapshot Metadata.
        /// Only valide for <see cref="State"/> equals <see cref="UserInteractionState.EXTRA_SNAPSHOT_METADATA"/>
        /// </summary>
        public AN_SnapshotMetadata Metadata {
            get {
                return m_metadata;
            }
        }
    }
}