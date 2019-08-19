using System;
using SA.Android.Utilities;
using SA.Android.GMS.Common;
using SA.Android.GMS.Internal;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// A client to interact with Snapshots.
    /// </summary>
    [Serializable]
    public class AN_SnapshotsClient : AN_LinkedObject
    {
        public enum ResolutionPolicy {

            /// <summary>
            /// NOT YET SUPPORTED. PLEASE CONTACT US at support@stansassets.com if you need this.
            /// 
            /// In the case of a conflict, the result will be returned to the app for resolution. 
            /// No automatic resolution will be performed.
            /// 
            /// This policy ensures that no user changes to the state of the save game will ever be lost.
            /// </summary>
            MANUAL = -1,

            /// <summary>
            /// In the case of a conflict, the snapshot with the longest played time will be used. 
            /// In the case of a tie, the last known good snapshot will be chosen instead.
            ///
            /// This policy is a good choice if the length of play time is a reasonable proxy for the "best" save game.
            /// Note that you must use <see cref="AN_SnapshotMetadataChange.Builder.SetPlayedTimeMillis(long)"/> when saving games 
            /// for this policy to be meaningful.
            /// </summary>
            LONGEST_PLAYTIME = 1,

            /// <summary>
            /// In the case of a conflict, the last known good version of this snapshot will be used.
            /// 
            /// This policy is a reasonable choice if your game requires stability from the snapshot data. 
            /// This policy ensures that only writes which are not contested are seen by the player, which guarantees that all clients converge.
            /// </summary>
            LAST_KNOWN_GOOD = 2,


            /// <summary>
            /// In the case of a conflict, the most recently modified version of this snapshot will be used.
            /// 
            /// This policy is a reasonable choice if your game can tolerate players 
            /// on multiple devices clobbering their own changes. 
            /// Because this policy blindly chooses the most recent data, it is possible that a player's changes may get lost.
            /// </summary>
            MOST_RECENTLY_MODIFIED = 3,


            /// <summary>
            /// In the case of a conflict, the snapshot with the highest progress value will be used. 
            /// In the case of a tie, the last known good snapshot will be chosen instead.
            ///
            /// This policy is a good choice if your game uses the progress value of the snapshot to determine the best saved game. 
            /// Note that you must use <see cref="AN_SnapshotMetadataChange.Builder.SetProgressValue(long)"/> when saving games for this policy to be meaningful.
            /// </summary>
            POLICY_HIGHEST_PROGRESS = 4

        }

        /// <summary>
        /// Constant passed to <see cref="ShowSelectSnapshotIntent(string, bool, bool, int, Action{AN_SnapshotUIResult})"/>
        /// indicating that the UI should not cap the number of displayed snapshots.
        /// </summary>
        public const int DISPLAY_LIMIT_NONE = -1;

        /// <summary>
        /// Shows the UI that will let the user select a snapshot.
        /// 
        /// If the user canceled without selecting a snapshot, 
        /// the result state will be <see cref="AN_SnapshotUIResult.UserInteractionState.NOTHING_SELECTED"/>. 
        /// If the user selected a snapshot from the list, 
        /// the result state will be <see cref="AN_SnapshotUIResult.UserInteractionState.EXTRA_SNAPSHOT_METADATA"/>. 
        /// If the user pressed the add button, 
        /// the result state will be <see cref="AN_SnapshotUIResult.UserInteractionState.EXTRA_SNAPSHOT_NEW"/>. 
        /// See the 
        /// </summary>
        /// <param name="title">The title to display in the action bar of the returned Activity.</param>
        /// <param name="callback">User interaction callback</param>
        public void ShowSelectSnapshotIntent(string title, Action<AN_SnapshotUIResult> callback) 
        {
            ShowSelectSnapshotIntent(title, true, true, DISPLAY_LIMIT_NONE, callback);
        }

        /// <summary>
        /// Shows the UI that will let the user select a snapshot.
        /// 
        /// If the user canceled without selecting a snapshot, 
        /// the result state will be <see cref="AN_SnapshotUIResult.UserInteractionState.NOTHING_SELECTED"/>. 
        /// If the user selected a snapshot from the list, 
        /// the result state will be <see cref="AN_SnapshotUIResult.UserInteractionState.EXTRA_SNAPSHOT_METADATA"/>. 
        /// If the user pressed the add button, 
        /// the result state will be <see cref="AN_SnapshotUIResult.UserInteractionState.EXTRA_SNAPSHOT_NEW"/>. 
        /// See the 
        /// </summary>
        /// <param name="title">The title to display in the action bar of the returned Activity.</param>
        /// <param name="allowAddButton">Whether or not to display a "create new snapshot" option in the selection UI.</param>
        /// <param name="allowDelete">Whether or not to provide a delete overflow menu option for each snapshot in the selection UI.</param>
        /// <param name="maxSnapshots">
        /// The maximum number of snapshots to display in the UI. 
        /// Use <see cref="DISPLAY_LIMIT_NONE"/> to display all snapshots.
        /// </param>
        /// <param name="callback">User interaction callback</param>
        public void ShowSelectSnapshotIntent(string title, bool allowAddButton, bool allowDelete, int maxSnapshots, Action<AN_SnapshotUIResult> callback) 
        {
            AN_GMS_Lib.Snapshots.ShowSelectSnapshotIntent(this, title, allowAddButton, allowAddButton, maxSnapshots, callback);
        }

        /// <summary>
        /// Asynchronously loads the maximum data size per snapshot in bytes. 
        /// Guaranteed to be at least 3 MB. May increase in the future.
        /// </summary>
        public void GetMaxDataSize() 
        {

        }

        /// <summary>
        /// Asynchronously loads list of <see cref="AN_SnapshotMetadata"/> 
        /// that represents the snapshot data for the currently signed-in player.
        /// Required Scopes: SCOPE_GAMES_LITE and SCOPE_APPFOLDER.
        /// </summary>
        /// <param name="callback">The task callback</param>
        public void Load(Action<AN_SnapshotsMetadataResult> callback) 
        {
            AN_GMS_Lib.Snapshots.Load(this, false, callback);
        }

        /// <summary>
        /// Asynchronously loads list of <see cref="AN_SnapshotMetadata"/> 
        /// that represents the snapshot data for the currently signed-in player.
        /// Required Scopes: SCOPE_GAMES_LITE and SCOPE_APPFOLDER.
        /// <param name="forceReload">
        /// If <c>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. 
        /// This would commonly be used for something like a user-initiated refresh. 
        /// Normally, this should be set to <c>false</c> to gain advantages of data caching.
        /// </param>
        /// <param name="callback">The task callback</param>
        /// </summary>
        public void Load(bool forceReload, Action<AN_SnapshotsMetadataResult> callback) 
        {
            AN_GMS_Lib.Snapshots.Load(this, forceReload, callback);
        }

        /// <summary>
        /// Starts a task which asynchronously opens a snapshot with the given fileName. 
        /// If createIfNotFound is set to true, the specified snapshot will be created if it does not already exist.
        /// 
        /// Required Scopes: SCOPE_GAMES_LITE and SCOPE_APPFOLDER.
        /// </summary>
        /// <param name="fileName">
        /// The name of the snapshot file to open. 
        /// Must be between 1 and 100 non-URL-reserved characters (a-z, A-Z, 0-9, or the symbols "-", ".", "_", or "~").
        /// </param>
        /// <param name="createIfNotFound">If true, the snapshot will be created if one cannot be found.</param>
        /// <param name="conflictPolicy">The conflict resolution policy to use for this snapshot.</param>
        /// <param name="callback">The task callback.</param>
        public void Open(string fileName, bool createIfNotFound, ResolutionPolicy conflictPolicy, Action<AN_LinkedObjectResult<AN_DataOrConflictResult>> callback) 
        {
            AN_GMS_Lib.Snapshots.Open(this, fileName, createIfNotFound, (int)conflictPolicy, callback);
        }
        
        /// <summary>
        /// Starts a task which asynchronously resolves a conflict using the data from the provided Snapshot.
        /// 
        /// This will replace the data on the server with the specified Snapshot.
        /// Note that it is possible for this operation to result in a conflict itself, in which case resolution should be repeated.
        /// </summary>
        /// <param name="conflictId">he ID of the conflict to resolve. Must come from <see cref="AN_SnapshotConflict"/></param>
        /// <param name="snapshot">The snapshot to use to resolve the conflict.</param>
        /// <param name="callback">The task callback.</param>
        public void ResolveConflict(string conflictId, AN_Snapshot snapshot,  Action<AN_LinkedObjectResult<AN_DataOrConflictResult>> callback) 
        {
            AN_GMS_Lib.Snapshots.ResolveConflict(this, conflictId, snapshot, callback);
        }
        

        /// <summary>
        /// Asynchronously commits any modifications in <see cref="AN_SnapshotMetadataChange"/>
        /// made to the <see cref="AN_Snapshot"/> and loads a <see cref="AN_SnapshotMetadata"/>.
        /// The callback returned by this method is complete once the changes are synced locally 
        /// and the background sync request for this data has been requested.
        /// 
        /// This method fails a task with an exception 
        /// when called with a snapshot that was not opened or has already been committed/discarded.
        /// 
        /// Note that the total size of the contents of snapshot 
        /// may not exceed the size provided <see cref="GetMaxDataSize"/>
        /// </summary>
        /// <param name="snapshot">The snapshot to commit the data for.</param>
        /// <param name="metadataChange">
        /// The set of changes to apply to the metadata for the snapshot. 
        /// Use <see cref="AN_SnapshotMetadataChange.EMPTY_CHANGE"/> to preserve the existing metadata.
        /// </param>
        /// <param name="callback">The operation callback.</param>
        public void CommitAndClose(AN_Snapshot snapshot, AN_SnapshotMetadataChange metadataChange, Action<AN_SnapshotMetadataResult> callback) 
        {
            AN_GMS_Lib.Snapshots.CommitAndClose(this, snapshot, metadataChange, callback);
        }

        /// <summary>
        /// Asynchronously deletes the specified by <see cref="AN_SnapshotMetadata"/> snapshot and loads the deleted snapshot ID. 
        /// This will delete the data of the snapshot locally and on the server.
        /// </summary>
        /// <param name="meta">The metadata of the snapshot to delete.</param>
        /// <param name="callback">The operation callback.</param>
        public void Delete(AN_SnapshotMetadata meta, Action<AN_SnapshotsDeleteResult> callback) 
        {
            AN_GMS_Lib.Snapshots.Delete(this, meta, callback);
        }
    }
}