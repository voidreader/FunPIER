using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Utilities;
using SA.Foundation.Tests;
using SA.Android.App;
using SA.Android.GMS.Games;


namespace SA.Android.Tests.GMS.Snapshots
{
    public abstract class AN_Snapshots_Test : SA_BaseTest
    {


        protected void PrintSnapshotMetadata(AN_SnapshotMetadata meta) {
            AN_Logger.Log("------------------------------------------------");
            AN_Logger.Log("meta.CoverImageUri: " + meta.CoverImageUri);
            AN_Logger.Log("meta.Title: " + meta.Title);
            AN_Logger.Log("meta.Description: " + meta.Description);
            AN_Logger.Log("meta.DeviceName: " + meta.DeviceName);
            AN_Logger.Log("meta.PlayedTime: " + meta.PlayedTime);
            AN_Logger.Log("meta.ProgressValue: " + meta.ProgressValue);
        }

    }
}