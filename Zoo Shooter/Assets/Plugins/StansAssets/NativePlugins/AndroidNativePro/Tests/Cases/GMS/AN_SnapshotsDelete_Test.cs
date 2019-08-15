using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Snapshots
{
    public class AN_SnapshotsDelete_Test : AN_Snapshots_Test
    {
        public override void Test() {
            var client = AN_Games.GetSnapshotsClient();
            client.Load((result) => {
                if (result.IsSucceeded) {

                    if(result.Snapshots.Count == 0) {
                        SetResult(SA_TestResult.WithError("There are no spanpshot's. Can't test delete action"));
                        return;
                    }

                    AN_SnapshotMetadata meta = result.Snapshots[0];
                    client.Delete(meta, (deleteResult) => {
                        if(deleteResult.IsSucceeded) {
                            AN_Logger.Log("deleteResult.SnapshotId: " + deleteResult.SnapshotId);
                        } 

                        SetAPIResult(deleteResult);
                    });

                } else {
                    SetAPIResult(result);
                }

                
            });
        }

    }
}