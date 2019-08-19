using System;
using SA.Android.GMS.Internal;
using SA.Android.Utilities;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// Snapshot Open result
    /// </summary>
    [Serializable]
    public class AN_DataOrConflictResult : AN_LinkedObject
    {
        /// <summary>
        /// Returns true if there is conflict, in which case <see cref="GetConflict"/>
        /// can be used to access the details. If this method returns false,
        /// <see cref="GetSnapshot"/> can be used to access the requested data.
        /// </summary>
        public bool IsConflict
        {
            get
            {
                return AN_GMS_Lib.Snapshots.DataOrConflictResult_IsConflict(this);  
            }
        }
        
        /// <summary>
        /// Snapshot data if the result was successful.
        /// </summary>
        /// <returns>Returns snapshot data if the result was successful.</returns>
        public AN_Snapshot GetSnapshot()
        {
            return AN_GMS_Lib.Snapshots.DataOrConflictResult_GetSnapshot(this).Data;
        }
        
        /// <summary>
        /// Snapshot conflict info.
        /// </summary>
        /// <returns>Returns snapshot conflict info.</returns>
        public AN_SnapshotConflict GetConflict()
        {
            return AN_GMS_Lib.Snapshots.DataOrConflictResult_GetConflict(this).Data;
        }
    }
}