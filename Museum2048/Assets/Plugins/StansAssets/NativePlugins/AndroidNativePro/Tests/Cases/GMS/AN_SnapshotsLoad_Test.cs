using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Snapshots
{
    public class AN_SnapshotsLoad_Test : AN_Snapshots_Test
    {

        public override void Test() {
            var client = AN_Games.GetSnapshotsClient();
            client.Load((result) => {
                if (result.IsSucceeded) {
                    foreach (var meta in result.Snapshots) {
                        PrintSnapshotMetadata(meta);
                    }
                }

                SetAPIResult(result);
            });
        }

    }
}