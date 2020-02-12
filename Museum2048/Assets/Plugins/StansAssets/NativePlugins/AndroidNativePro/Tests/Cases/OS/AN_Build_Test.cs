using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.OS;
using SA.Android.Utilities;
using SA.Foundation.Tests;

namespace SA.Android.Tests.OS
{
    public class AN_Build_Test : SA_BaseTest
    {

        public override void Test() {
            //The base OS build the product is based on.
            AN_Logger.Log("AN_Build.VERSION.BASE_OS: " + AN_Build.VERSION.BASE_OS);

            //The current development codename, or the string "REL" if this is a release build.
            AN_Logger.Log("AN_Build.VERSION.CODENAME: " + AN_Build.VERSION.CODENAME);

            //The internal value used by the underlying source control to represent this build.
            AN_Logger.Log("AN_Build.VERSION.INCREMENTAL " + AN_Build.VERSION.INCREMENTAL);

            //The developer preview revision of a prerelease SDK.
            AN_Logger.Log("AN_Build.VERSION.PREVIEW_SDK_INT: " + AN_Build.VERSION.PREVIEW_SDK_INT);

            //The user-visible version string.
            AN_Logger.Log("AN_Build.VERSION.RELEASE: " + AN_Build.VERSION.RELEASE);

            //The SDK version of the software currently running on this hardware device.
            AN_Logger.Log("AN_Build.VERSION.SDK_INT: " + AN_Build.VERSION.SDK_INT);

            //The user-visible security patch level.
            AN_Logger.Log("AN_Build.VERSION.SECURITY_PATCH: " + AN_Build.VERSION.SECURITY_PATCH);
        }
     }
}