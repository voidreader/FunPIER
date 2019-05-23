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
        void Open(AN_SnapshotsClient client, string name, bool createIfNotFound, int conflictPolicy, Action<AN_LinkedObjectResult<AN_Snapshot>> callback);
        void CommitAndClose(AN_SnapshotsClient client, AN_Snapshot snapshot, AN_SnapshotMetadataChange metadataChange, Action<AN_SnapshotMetadataResult> callback);
        void Delete(AN_SnapshotsClient client, AN_SnapshotMetadata meta, Action<AN_SnapshotsDeleteResult> callback);


        //--------------------------------------
        // AN_Snapshot
        //--------------------------------------


        void Snapshot_WriteBytes(AN_Snapshot snapshot, byte[] data);
        byte[] Snapshot_ReadFully(AN_Snapshot snapshot);
        AN_SnapshotMetadata Snapshot_GetMetadata(AN_Snapshot snapshot);

    }
}
