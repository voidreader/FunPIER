using UnityEngine;
using System;

using SA.Foundation.Templates;

namespace SA.Android.GMS.Games
{

    /// <summary>
    /// The result of Snapshot loading
    /// </summary>
    [Serializable]
    public class AN_SnapshotMetadataResult : SA_Result
    {
        [SerializeField] AN_SnapshotMetadata m_metadata;


        //Editor use only
        public AN_SnapshotMetadataResult(AN_SnapshotMetadata metadata) : base() {
            m_metadata = metadata;
        }

        /// <summary>
        /// Loaded Snapshot Metadata
        /// </summary>
        public AN_SnapshotMetadata Metadata {
            get {
                return m_metadata;
            }
        }
    }
}