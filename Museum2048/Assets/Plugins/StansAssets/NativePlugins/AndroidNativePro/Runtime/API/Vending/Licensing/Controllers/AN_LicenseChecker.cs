using System;
using SA.Android.Utilities;
using UnityEngine;
using SA.Foundation.Utility;
using StansAssets.Foundation.Async;

namespace SA.Android.Vending.Licensing
{
    public static class AN_LicenseChecker
    {
        const string k_JavaLicenseCheckerClass = "com.stansassets.licensing.AN_LicenseChecker";

        public static void CheckAccess(Action<AN_LicenseResult> callback)
        {
            if (!AN_Settings.Instance.Licensing)
            {
                SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "Licensing");
                return;
            }

            if (Application.isEditor)
            {
                 CoroutineUtility.WaitForSeconds(1, () =>
                {
                    callback.Invoke(new AN_LicenseResult(AN_Policy.LICENSED));
                });
                return;
            }

            AN_Java.Bridge.CallStaticWithCallback(k_JavaLicenseCheckerClass,
                "CheckAccess",
                callback,
                AN_Settings.Instance.RSAPublicKey);
        }
    }
}
