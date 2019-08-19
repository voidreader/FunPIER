using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using SA.Android.GMS.Games;

using SA.Android.Utilities;

using SA.Foundation.Utility;

public class AN_GMS_Snapshot_Example : MonoBehaviour {


   
    [SerializeField] Button m_nativeUI = null;
    [SerializeField] Button m_load = null;

    [SerializeField] Button m_createNew = null;

    private void Start() {


        m_createNew.onClick.AddListener(() => {
            Debug.Log("User choosed to create the Snapshot");
            CreateSnapshot();
        });

        m_nativeUI.onClick.AddListener(() => {
            var client = AN_Games.GetSnapshotsClient();
            client.ShowSelectSnapshotIntent("Hello World!", (result) => {

                if(result.IsSucceeded) {
                    switch (result.State) {
                        case AN_SnapshotUIResult.UserInteractionState.EXTRA_SNAPSHOT_METADATA:
                            Debug.Log("User choosed to load the Snapshot");
                            LoadSnapshot(result.Metadata);
                            break;
                        case AN_SnapshotUIResult.UserInteractionState.EXTRA_SNAPSHOT_NEW:
                            Debug.Log("User choosed to create the Snapshot");
                            CreateSnapshot();
                            break;
                    }
                } else {
                    Debug.Log("Snapshots UI Failed: " + result.Error.FullMessage);
                }
            });

        });


        m_load.onClick.AddListener(() => {
            var client = AN_Games.GetSnapshotsClient();
            client.Load((result) => {
                if(result.IsSucceeded) {
                    AN_Logger.Log("Load Snapshots Succeeded, count: " + result.Snapshots.Count);
                    foreach(var meta in result.Snapshots) {
                        PrintMeta(meta);
                    }
                } else {
                    Debug.Log("Load Snapshots Failed: " + result.Error.FullMessage);
                }
            });
        });
    }


    private void CreateSnapshot() {
        string name = "NewGameSave_" + SA_IdFactory.NextId;
        OpenSnapshot(name);
    }

    private void LoadSnapshot(AN_SnapshotMetadata meta) {

        PrintMeta(meta);
        OpenSnapshot(meta.Title);
    }


    private void PrintMeta(AN_SnapshotMetadata meta) {
        AN_Logger.Log("------------------------------------------------");
        AN_Logger.Log("meta.CoverImageUri: " + meta.CoverImageUri);
        AN_Logger.Log("meta.Title: " + meta.Title);
        AN_Logger.Log("meta.Description: " + meta.Description);
        AN_Logger.Log("meta.DeviceName: " + meta.DeviceName);
        AN_Logger.Log("meta.PlayedTime: " + meta.PlayedTime);
        AN_Logger.Log("meta.ProgressValue: " + meta.ProgressValue);
    }


    private void OpenSnapshot(string name) {
        var client = AN_Games.GetSnapshotsClient();

        bool createIfNotFound = true;
        var conflictPolicy = AN_SnapshotsClient.ResolutionPolicy.LAST_KNOWN_GOOD;

        client.Open(name, createIfNotFound, conflictPolicy, (result) => {
            if(result.IsSucceeded) {
                Debug.Log("We have snapshot, reading data...");
                AN_Snapshot snapshot = result.Data.GetSnapshot();

                byte[] data = snapshot.ReadFully();

                long progress = snapshot.GetMetadata().ProgressValue;
                string base64Text = Convert.ToBase64String(data);
                Debug.Log("Snapshot data: " + base64Text);
                Debug.Log("Snapshot progress: " + snapshot.GetMetadata().ProgressValue);
                Debug.Log("Snapshot played time: " + snapshot.GetMetadata().PlayedTime);


                Debug.Log("Writing data...");

                string mydata = "My game data";
                data = mydata.ToBytes();

                snapshot.WriteBytes(data);

                SA_ScreenUtil.TakeScreenshot(512, (screenshot) => {
                    var changeBuilder = new AN_SnapshotMetadataChange.Builder();
                    changeBuilder.SetDescription("Hello Description");
                    changeBuilder.SetPlayedTimeMillis(10000);
                    changeBuilder.SetProgressValue(progress + 1);
                    changeBuilder.SetCoverImage(screenshot);

                    AN_SnapshotMetadataChange changes = changeBuilder.Build();
                    client.CommitAndClose(snapshot, changes, (commitResult) => {
                        if (commitResult.IsSucceeded) {
                            PrintMeta(commitResult.Metadata);
                        } else {
                            Debug.Log("CommitAndClose Snapshots Failed: " + result.Error.FullMessage);
                        }
                    });
                });

               

            } else {
                Debug.Log("Open Snapshots Failed: " + result.Error.FullMessage);
            }
        });
    }
}
