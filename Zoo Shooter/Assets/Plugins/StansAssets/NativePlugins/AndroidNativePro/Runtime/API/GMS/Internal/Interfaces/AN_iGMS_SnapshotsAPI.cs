using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Android.GMS.Games;
using SA.Android.GMS.Common;

namespace SA.Android.GMS.Internal
{
    internal interface AN_iGMS_SnapshotsAPI
    {
        //--------------------------------------
        // AN_SnapshotsClient
        //--------------------------------------

        void ShowSelectSnapshotIntent(AN_SnapshotsClient client, string title, bool allowAddButton, bool allowDelete, int maxSnapshots, Action<AN_SnapshotUIResult> callback);
        void Load(AN_SnapshotsClient client, bool forceReload, Action<AN_SnapshotsMetadataResult> callback);
        void Open(AN_SnapshotsClient client, string name, bool createIfNotFound, int conflictPolicy, Action<AN_LinkedObjectResult<AN_DataOrConflictResult>> callback);
        void ResolveConflict(AN_SnapshotsClient client, string conflictId,AN_Snapshot snapshot, Action<AN_LinkedObjectResult<AN_DataOrConflictResult>> callback);
        void CommitAndClose(AN_SnapshotsClient client, AN_Snapshot snapshot, AN_SnapshotMetadataChange metadataChange, Action<AN_SnapshotMetadataResult> callback);
        void Delete(AN_SnapshotsClient client, AN_SnapshotMetadata meta, Action<AN_SnapshotsDeleteResult> callback);

        //--------------------------------------
        // AN_Snapshot
        //--------------------------------------

        void Snapshot_WriteBytes(AN_Snapshot snapshot, byte[] data);
        byte[] Snapshot_ReadFully(AN_Snapshot snapshot);
        AN_SnapshotMetadata Snapshot_GetMetadata(AN_Snapshot snapshot);
        
        //--------------------------------------
        // DataOrConflictResult
        //--------------------------------------

        bool DataOrConflictResult_IsConflict(AN_DataOrConflictResult result);
        AN_LinkedObjectResult<AN_Snapshot> DataOrConflictResult_GetSnapshot(AN_DataOrConflictResult result);
        AN_LinkedObjectResult<AN_SnapshotConflict> DataOrConflictResult_GetConflict(AN_DataOrConflictResult result);
       
        //--------------------------------------
        // AN_SnapshotConflict
        //--------------------------------------

        string SnapshotConflict_GetConflictId(AN_SnapshotConflict conflict);
        AN_LinkedObjectResult<AN_Snapshot> SnapshotConflict_GetSnapshot(AN_SnapshotConflict conflict);
        AN_LinkedObjectResult<AN_Snapshot> SnapshotConflict_GetConflictingSnapshot(AN_SnapshotConflict conflict);
    }
}
