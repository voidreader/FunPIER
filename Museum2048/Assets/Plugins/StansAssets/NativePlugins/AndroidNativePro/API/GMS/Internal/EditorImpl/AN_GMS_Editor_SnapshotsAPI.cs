using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

using SA.Android.GMS.Games;
using SA.Android.GMS.Common;
using SA.Android.Utilities;

using SA.Foundation.Async;



namespace SA.Android.GMS.Internal
{

    internal class AN_GMS_Editor_SnapshotsAPI : AN_iGMS_SnapshotsAPI {


        


        //--------------------------------------
        // AN_SnapshotsClient
        //--------------------------------------


        public void ShowSelectSnapshotIntent(AN_SnapshotsClient client, string title, bool allowAddButton, bool allowDelete, int maxSnapshots, Action<AN_SnapshotUIResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_SnapshotUIResult((int) AN_SnapshotUIResult.UserInteractionState.NOTHING_SELECTED);
                callback.Invoke(result);
            });
        }

        public void Load(AN_SnapshotsClient client, bool forceReload, Action<AN_SnapshotsMetadataResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                callback.Invoke(new AN_SnapshotsMetadataResult());
            });

        }

        public void Open(AN_SnapshotsClient client, string name, bool createIfNotFound, int conflictPolicy, Action<AN_LinkedObjectResult<AN_Snapshot>> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_LinkedObjectResult<AN_Snapshot>(new AN_Snapshot());
                callback.Invoke(result);
            });
        }

        public void CommitAndClose(AN_SnapshotsClient client, AN_Snapshot snapshot, AN_SnapshotMetadataChange metadataChange, Action<AN_SnapshotMetadataResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_SnapshotMetadataResult(new AN_SnapshotMetadata());
                callback.Invoke(result);
            });
        }

        public void Delete(AN_SnapshotsClient client, AN_SnapshotMetadata meta, Action<AN_SnapshotsDeleteResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                var result = new AN_SnapshotsDeleteResult();
                callback.Invoke(result);
            });
        }



        //--------------------------------------
        // AN_Snapshot
        //--------------------------------------



        public void Snapshot_WriteBytes(AN_Snapshot snapshot, byte[] data) {
          
        }

        public byte[] Snapshot_ReadFully(AN_Snapshot snapshot) {
            return new byte[] { };
        }


        public AN_SnapshotMetadata Snapshot_GetMetadata(AN_Snapshot snapshot) {
            return new AN_SnapshotMetadata();
        }

    }

}