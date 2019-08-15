using SA.Android.GMS.Internal;
using SA.Android.Utilities;

namespace SA.Android.GMS.Games
{
    public class AN_SnapshotConflict : AN_LinkedObject
    {
        /// <summary>
        /// Returns the ID of the conflict to resolve,
        /// if any. Pass this to<see cref="AN_SnapshotsClient.ResolveConflict"/> when resolving the conflict.
        /// </summary>
        public string ConflictId
        {
            get { return AN_GMS_Lib.Snapshots.SnapshotConflict_GetConflictId(this); }
        }

        /// <summary>
        /// The server's version of the Snapshot that was opened.
        /// </summary>
        /// <returns>Returns the server's version of the Snapshot that was opened.</returns>
        public AN_Snapshot GetSnapshot()
        {
            return AN_GMS_Lib.Snapshots.SnapshotConflict_GetSnapshot(this).Data;
        }
        
        /// <summary>
        /// The modified version of the Snapshot in the case of a conflict.
        /// </summary>
        /// <returns>Returns the modified version of the Snapshot in the case of a conflict.</returns>
        public AN_Snapshot GetConflictingSnapshot()
        {
            return AN_GMS_Lib.Snapshots.SnapshotConflict_GetConflictingSnapshot(this).Data;
        }
    }
}