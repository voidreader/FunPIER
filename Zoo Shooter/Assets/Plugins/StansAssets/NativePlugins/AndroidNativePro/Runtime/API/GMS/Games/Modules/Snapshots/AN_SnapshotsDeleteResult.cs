using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// An achievements increment result.
    /// </summary>
    [Serializable]
    public class AN_SnapshotsDeleteResult : SA_Result
    {
        [SerializeField] string m_snapshotId = null;

        //Editor use only
        public AN_SnapshotsDeleteResult():base () { }


        /// <summary>
        /// The deleted snapshot ID.
        /// </summary>
        public string SnapshotId {
            get {
                return m_snapshotId;
            }
        }
    }
}