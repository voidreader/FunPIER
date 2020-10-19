using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SA.Android.GMS.Games;
using SA.Android.Utilities;
using SA.Foundation.Utility;
using StansAssets.Foundation;

public class AN_GMS_Snapshot_Example : MonoBehaviour
{
    [SerializeField]
    Button m_nativeUI = null;
    [SerializeField]
    Button m_load = null;

    [SerializeField]
    Button m_createNew = null;

    void Start()
    {
        m_createNew.onClick.AddListener(() =>
        {
            Debug.Log("User choosed to create the Snapshot");
            CreateSnapshot();
        });

        m_nativeUI.onClick.AddListener(() =>
        {
            var client = AN_Games.GetSnapshotsClient();
            client.ShowSelectSnapshotIntent("Hello World!", (result) =>
            {
                if (result.IsSucceeded)
                    switch (result.State)
                    {
                        case AN_SnapshotUIResult.UserInteractionState.EXTRA_SNAPSHOT_METADATA:
                            Debug.Log("User choosed to load the Snapshot");
                            LoadSnapshot(result.Metadata);
                            break;
                        case AN_SnapshotUIResult.UserInteractionState.EXTRA_SNAPSHOT_NEW:
                            Debug.Log("User choosed to create the Snapshot");
                            CreateSnapshot();
                            break;
                    }
                else
                    Debug.Log("Snapshots UI Failed: " + result.Error.FullMessage);
            });
        });

        m_load.onClick.AddListener(() =>
        {
            var client = AN_Games.GetSnapshotsClient();
            client.Load((result) =>
            {
                if (result.IsSucceeded)
                {
                    Debug.Log("Load Snapshots Succeeded, count: " + result.Snapshots.Count);
                    foreach (var meta in result.Snapshots) PrintMeta(meta);
                }
                else
                {
                    Debug.Log("Load Snapshots Failed: " + result.Error.FullMessage);
                }
            });
        });
    }

    void CreateSnapshot()
    {
        var name = "NewGameSave_" + IdFactory.NextId;
        OpenSnapshot(name);
    }

    void LoadSnapshot(AN_SnapshotMetadata meta)
    {
        PrintMeta(meta);
        OpenSnapshot(meta.Title);
    }

    void PrintMeta(AN_SnapshotMetadata meta)
    {
        Debug.Log("------------------------------------------------");
        Debug.Log("meta.CoverImageUri: " + meta.CoverImageUri);
        Debug.Log("meta.Title: " + meta.Title);
        Debug.Log("meta.Description: " + meta.Description);
        Debug.Log("meta.DeviceName: " + meta.DeviceName);
        Debug.Log("meta.PlayedTime: " + meta.PlayedTime);
        Debug.Log("meta.ProgressValue: " + meta.ProgressValue);
    }

    void OpenSnapshot(string name)
    {
        var client = AN_Games.GetSnapshotsClient();

        var createIfNotFound = true;
        var conflictPolicy = AN_SnapshotsClient.ResolutionPolicy.LAST_KNOWN_GOOD;

        client.Open(name, createIfNotFound, conflictPolicy, (result) =>
        {
            if (result.IsSucceeded)
            {
                Debug.Log("We have snapshot, reading data...");
                var snapshot = result.Data.GetSnapshot();

                var data = snapshot.ReadFully();

                var progress = snapshot.GetMetadata().ProgressValue;
                var base64Text = Convert.ToBase64String(data);
                Debug.Log("Snapshot data: " + base64Text);
                Debug.Log("Snapshot progress: " + snapshot.GetMetadata().ProgressValue);
                Debug.Log("Snapshot played time: " + snapshot.GetMetadata().PlayedTime);

                Debug.Log("Writing data...");

                var mydata = "My game data";
                data = System.Text.Encoding.UTF8.GetBytes(mydata);

                snapshot.WriteBytes(data);

                SA_ScreenUtil.TakeScreenshot(512, (screenshot) =>
                {
                    var changeBuilder = new AN_SnapshotMetadataChange.Builder();
                    changeBuilder.SetDescription("Hello Description");
                    changeBuilder.SetPlayedTimeMillis(10000);
                    changeBuilder.SetProgressValue(progress + 1);
                    changeBuilder.SetCoverImage(screenshot);

                    var changes = changeBuilder.Build();
                    client.CommitAndClose(snapshot, changes, (commitResult) =>
                    {
                        if (commitResult.IsSucceeded)
                            PrintMeta(commitResult.Metadata);
                        else
                            Debug.Log("CommitAndClose Snapshots Failed: " + result.Error.FullMessage);
                    });
                });
            }
            else
            {
                Debug.Log("Open Snapshots Failed: " + result.Error.FullMessage);
            }
        });
    }
}
