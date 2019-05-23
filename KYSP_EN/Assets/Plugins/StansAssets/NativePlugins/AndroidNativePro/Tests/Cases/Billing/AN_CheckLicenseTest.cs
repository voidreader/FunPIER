using UnityEngine;
using System.Collections;
using SA.Android.Utilities;
using SA.Android.Vending.Licensing;
using SA.Foundation.Tests;

namespace SA.Android.Tests.Billing
{
    public class AN_CheckLicenseTest : SA_BaseTest {


        public override void Test() {
            AN_LicenseChecker.CheckAccess((result) => {
                //We may fail with licence check, it's okay as long as PolicyCode is prsed corretly.
                AN_Logger.Log("result.PolicyCode: " + result.PolicyCode);
                SetResult(SA_TestResult.OK);
            });
        }
    }
}