using UnityEngine;
using System;
using System.Collections.Generic;
using SA.Foundation.Templates;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// The result of Snapshots loading
    /// </summary>
    [Serializable]
    public class AN_SnapshotsMetadataResult : SA_Result
    {
        [SerializeField] List<AN_SnapshotMetadata> m_metadataList = new List<AN_SnapshotMetadata>();

        //Editor use only
        public AN_SnapshotsMetadataResult() : base() { }

        /// <summary>
        /// The Metadata Snapshots List
        /// </summary>
        public List<AN_SnapshotMetadata> Snapshots 
        {
            get { return m_metadataList; }
        }
    }
}