using UnityEngine;
using System;
using SA.Android.Utilities;
using SA.Foundation.Tests;

using SA.Android.GMS.Common;

namespace SA.Android.Tests.GMS
{
    public class AN_GoogleApiAvailability_Test : SA_BaseTest
    {

        public override void Test() {
            int responce =  AN_GoogleApiAvailability.IsGooglePlayServicesAvailable();
            if(responce == AN_ConnectionResult.SUCCESS) {
                SetResult(SA_TestResult.OK);
            } else {
                AN_Logger.Log("Google Api not avaliable on current device, trying to resolve");
                AN_GoogleApiAvailability.MakeGooglePlayServicesAvailable((result) => {
                    if(result.IsSucceeded) {
                        SetResult(SA_TestResult.OK);
                    } else {
                        SetResult(SA_TestResult.WithError("Google Api not avaliable on current device"));
                    }
                });
            }
        }
    }
}