using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Snapshots
{
    public class AN_SnapshotsUI_Test : AN_Snapshots_Test
    {

        public override bool RequireUserInteraction { get { return true; } }

        public override void Test() {
            var client = AN_Games.GetSnapshotsClient();
            client.ShowSelectSnapshotIntent("Hello World!", (result) => {

                if (result.IsSucceeded) {
                    switch (result.State) {
                        case AN_SnapshotUIResult.UserInteractionState.EXTRA_SNAPSHOT_METADATA:
                            Debug.Log("User choosed to load the Snapshot");
                            PrintSnapshotMetadata(result.Metadata);
                            break;
                        case AN_SnapshotUIResult.UserInteractionState.EXTRA_SNAPSHOT_NEW:
                            AN_Logger.Log("User choosed to create the Snapshot");
                            break;
                    }
                }

                SetAPIResult(result);
            });
        }
    }
}