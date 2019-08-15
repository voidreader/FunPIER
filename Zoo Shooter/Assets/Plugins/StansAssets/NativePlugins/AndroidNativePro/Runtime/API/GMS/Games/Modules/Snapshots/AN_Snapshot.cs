using System;
using SA.Android.Utilities;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// Data interface for a representation of a saved game. 
    /// This includes both the metadata and the actual game content.
    /// </summary>
    [Serializable]
    public class AN_Snapshot : AN_LinkedObject
    {
        /// <summary>
        /// Retrieves the metadata for this snapshot.
        /// </summary>
        public AN_SnapshotMetadata GetMetadata() 
        {
            return AN_GMS_Lib.Snapshots.Snapshot_GetMetadata(this);
        }

        /// <summary>
        /// Read the contents of a snapshot.
        /// 
        /// If this snapshot was not opened via 
        /// <see cref="AN_SnapshotsClient.Open(string, bool, AN_SnapshotsClient.ResolutionPolicy, Action{Common.AN_LinkedObjectResult{AN_Snapshot}})"/>,
        /// or if the contents have already been committed via 
        /// <see cref="AN_SnapshotsClient.CommitAndClose(AN_Snapshot, AN_SnapshotMetadataChange, Action{AN_SnapshotMetadataResult})"/>
        /// this method will throw an exception.
        /// </summary>
        /// <returns></returns>
        public byte[] ReadFully() 
        {
            return AN_GMS_Lib.Snapshots.Snapshot_ReadFully(this);
        }

        /// <summary>
        /// Write the specified data into the snapshot. 
        /// The contents of the snapshot will be replaced with the data provided in content. 
        /// The data will be persisted on disk, but is not uploaded to the server until the snapshot is committed via 
        /// <see cref="AN_SnapshotsClient.CommitAndClose(AN_Snapshot, AN_SnapshotMetadataChange, Action{AN_SnapshotMetadataResult})"/>
        /// 
        /// If this snapshot was not opened via 
        /// <see cref="AN_SnapshotsClient.Open(string, bool, AN_SnapshotsClient.ResolutionPolicy, Action{Common.AN_LinkedObjectResult{AN_Snapshot}})"/>,
        /// or if the contents have already been committed via 
        /// <see cref="AN_SnapshotsClient.CommitAndClose(AN_Snapshot, AN_SnapshotMetadataChange, Action{AN_SnapshotMetadataResult})"/>
        /// this method will throw an exception.
        /// </summary>
        /// <param name="content">The data to write.</param>
        public void WriteBytes(byte[] content) 
        {
            AN_GMS_Lib.Snapshots.Snapshot_WriteBytes(this, content);
        }
    }
}