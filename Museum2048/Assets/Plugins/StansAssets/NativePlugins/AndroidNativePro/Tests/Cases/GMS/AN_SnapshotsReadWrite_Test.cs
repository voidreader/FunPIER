using System;
using SA.Android.Utilities;
using SA.Foundation.Utility;
using SA.Foundation.Tests;
using SA.Android.GMS.Games;

namespace SA.Android.Tests.GMS.Snapshots
{
    public class AN_SnapshotsReadWrite_Test : AN_Snapshots_Test
    {

        private long m_progress;
        private long m_playedTime;
        private string m_snpashotData;
        private readonly bool m_createIfNotFound = true;
        private readonly AN_SnapshotsClient.ResolutionPolicy m_conflictPolicy = AN_SnapshotsClient.ResolutionPolicy.LAST_KNOWN_GOOD;

        public override void Test() {
            string name = "XTestGameSave_" + SA_IdFactory.NextId;
            ReadAndWrite(name);
        }

        private void ReadAndWrite(string name) {
           
          
            var client = AN_Games.GetSnapshotsClient();
            client.Open(name, m_createIfNotFound, m_conflictPolicy, (result) => {
                if (result.IsSucceeded) {
                    AN_Logger.Log("We have snapshot, reading data...");
                    AN_Snapshot snapshot = result.Data.GetSnapshot();

                    byte[] data = snapshot.ReadFully();

                    var meta = snapshot.GetMetadata();
                    m_progress = meta.ProgressValue + 10;
                    m_playedTime = meta.PlayedTime + 100;


                    string base64Text = Convert.ToBase64String(data);
                    AN_Logger.Log("Snapshot data: " + base64Text);
                    PrintSnapshotMetadata(snapshot.GetMetadata());


                    AN_Logger.Log("Writing data...");

                    m_snpashotData = "My game data " + name;
                    data = m_snpashotData.ToBytes();

                    snapshot.WriteBytes(data);


                    SA_ScreenUtil.TakeScreenshot(512, (screenshot) => {
                        var changeBuilder = new AN_SnapshotMetadataChange.Builder();
                        changeBuilder.SetDescription("Hello Description");
                        changeBuilder.SetPlayedTimeMillis(m_playedTime);
                        changeBuilder.SetProgressValue(m_progress);
                        changeBuilder.SetCoverImage(screenshot);

                        AN_SnapshotMetadataChange changes = changeBuilder.Build();
                        client.CommitAndClose(snapshot, changes, (commitResult) => {
                            if (commitResult.IsSucceeded) {
                                PrintSnapshotMetadata(commitResult.Metadata);
                                VerifySnapshotsSave(name);
                            } else {
                                SetAPIResult(result);
                            }

                            
                        });
                    });


                } else {
                    SetAPIResult(result);
                }
            });
        }


        private void VerifySnapshotsSave(string name) {
            var client = AN_Games.GetSnapshotsClient();
            client.Open(name, m_createIfNotFound, m_conflictPolicy, (result) => {
                if (result.IsSucceeded) {
                    AN_Snapshot snapshot = result.Data.GetSnapshot();
                    byte[] data = snapshot.ReadFully();
                    var meta = snapshot.GetMetadata();

                    if (meta.ProgressValue != m_progress) {
                        SetResult(SA_TestResult.WithError("ProgressValue verification failed"));
                        return;
                    }

                    if (meta.PlayedTime != m_playedTime) {
                        SetResult(SA_TestResult.WithError("PlayedTime verification failed"));
                        return;
                    }

                    string snapshotData = string.Empty;
                    snapshotData = snapshotData.FromBytes(data);
                    if (!snapshotData.Equals(m_snpashotData)) {
                        SetResult(SA_TestResult.WithError("Snapshot Data verification failed"));
                        return;
                    }
                }

                SetAPIResult(result);
            });
        }

    }
}