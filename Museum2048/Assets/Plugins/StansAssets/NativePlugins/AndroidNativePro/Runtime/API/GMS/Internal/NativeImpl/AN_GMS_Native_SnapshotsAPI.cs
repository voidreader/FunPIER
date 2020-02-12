using System;
using UnityEngine;
using SA.Android.GMS.Games;
using SA.Android.GMS.Common;
using SA.Android.Utilities;

namespace SA.Android.GMS.Internal
{
    internal class AN_GMS_Native_SnapshotsAPI : AN_iGMS_SnapshotsAPI 
    {
        const string JAVA_PACKAGE = "com.stansassets.gms.games.saves.";

        //--------------------------------------
        // AN_SnapshotsClient
        //--------------------------------------

        const string AN_SnapshotsClient = JAVA_PACKAGE + "AN_SnapshotsClient";


        public void ShowSelectSnapshotIntent(AN_SnapshotsClient client, string title, bool allowAddButton, bool allowDelete, int maxSnapshots, Action<AN_SnapshotUIResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_SnapshotsClient, "ShowSelectSnapshotIntent", callback, client.HashCode, title, allowAddButton, allowDelete, maxSnapshots);
        }

        public void Load(AN_SnapshotsClient client, bool forceReload, Action<AN_SnapshotsMetadataResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_SnapshotsClient, "Load", callback, client.HashCode, forceReload);

        }

        public void Open(AN_SnapshotsClient client, string name, bool createIfNotFound, int conflictPolicy, Action<AN_LinkedObjectResult<AN_DataOrConflictResult>> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_SnapshotsClient, "Open", callback, client.HashCode, name, createIfNotFound, conflictPolicy);
        }
        
        public void ResolveConflict(AN_SnapshotsClient client, string conflictId, AN_Snapshot snapshot,
            Action<AN_LinkedObjectResult<AN_DataOrConflictResult>> callback)
        {
            AN_Java.Bridge.CallStaticWithCallback(AN_SnapshotsClient, "ResolveConflict", callback, client.HashCode, conflictId, snapshot.HashCode);
        }


        public void CommitAndClose(AN_SnapshotsClient client, AN_Snapshot snapshot, AN_SnapshotMetadataChange metadataChange, Action<AN_SnapshotMetadataResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_SnapshotsClient, "CommitAndClose", callback, client.HashCode, snapshot.HashCode, metadataChange);
        }

        public void Delete(AN_SnapshotsClient client, AN_SnapshotMetadata meta,  Action<AN_SnapshotsDeleteResult> callback) {
            AN_Java.Bridge.CallStaticWithCallback(AN_SnapshotsClient, "Delete", callback, client.HashCode, meta.HashCode);
        }


        //--------------------------------------
        // AN_Snapshot
        //--------------------------------------

        const string AN_Snapshot = JAVA_PACKAGE + "AN_Snapshot";

        public void Snapshot_WriteBytes(AN_Snapshot snapshot, byte[] data) 
        {
            var base64EncodedString = Convert.ToBase64String(data);
            AN_Java.Bridge.CallStatic(AN_Snapshot, "WriteBytes", snapshot.HashCode, base64EncodedString);
        }

        public byte[] Snapshot_ReadFully(AN_Snapshot snapshot)
        {
            var base64EncodedString = AN_Java.Bridge.CallStatic<string>(AN_Snapshot, "ReadFully", snapshot.HashCode);
            if (string.IsNullOrEmpty(base64EncodedString))
            {
                return new byte[] { };
            }

            return base64EncodedString.BytesFromBase64String();
        }

        public AN_SnapshotMetadata Snapshot_GetMetadata(AN_Snapshot snapshot) 
        {
            var json = AN_Java.Bridge.CallStatic<string>(AN_Snapshot, "GetMetadata", snapshot.HashCode);
            if (string.IsNullOrEmpty(json)) {
                return null;
            } 
            return JsonUtility.FromJson<AN_SnapshotMetadata>(json);
        }
        
        //--------------------------------------
        // AN_DataOrConflictResult
        //--------------------------------------
        
        const string AN_DataOrConflictResult = JAVA_PACKAGE + "AN_DataOrConflictResult";
        
        public bool DataOrConflictResult_IsConflict(AN_DataOrConflictResult result) 
        {
            return AN_Java.Bridge.CallStatic<bool>(AN_DataOrConflictResult, "IsConflict", result.HashCode);
        }

        public AN_LinkedObjectResult<AN_Snapshot> DataOrConflictResult_GetSnapshot(AN_DataOrConflictResult result) 
        {
            var json = AN_Java.Bridge.CallStatic<string>(AN_DataOrConflictResult, "GetSnapshot", result.HashCode);
            return JsonUtility.FromJson<AN_LinkedObjectResult<AN_Snapshot>>(json);
        }

        public AN_LinkedObjectResult<AN_SnapshotConflict> DataOrConflictResult_GetConflict(AN_DataOrConflictResult result) 
        {
            var json = AN_Java.Bridge.CallStatic<string>(AN_DataOrConflictResult, "GetConflict", result.HashCode);
            return JsonUtility.FromJson<AN_LinkedObjectResult<AN_SnapshotConflict>>(json);
        }
       
        //--------------------------------------
        // AN_SnapshotConflict
        //--------------------------------------
        
        const string AN_SnapshotConflict = JAVA_PACKAGE + "AN_SnapshotConflict";
        
        public string SnapshotConflict_GetConflictId(AN_SnapshotConflict conflict) 
        {
            return AN_Java.Bridge.CallStatic<string>(AN_SnapshotConflict, "GetConflictId", conflict.HashCode);
        }

        public AN_LinkedObjectResult<AN_Snapshot> SnapshotConflict_GetSnapshot(AN_SnapshotConflict conflict) 
        {
            var json = AN_Java.Bridge.CallStatic<string>(AN_SnapshotConflict, "GetSnapshot", conflict.HashCode);
            return JsonUtility.FromJson<AN_LinkedObjectResult<AN_Snapshot>>(json);
        }

        public AN_LinkedObjectResult<AN_Snapshot> SnapshotConflict_GetConflictingSnapshot(AN_SnapshotConflict conflict) 
        {
            var json = AN_Java.Bridge.CallStatic<string>(AN_SnapshotConflict, "GetConflictingSnapshot", conflict.HashCode);
            return JsonUtility.FromJson<AN_LinkedObjectResult<AN_Snapshot>>(json);
        }
    }
}