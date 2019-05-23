using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;
using SA.Android.Vending.Internal;


namespace SA.Android.Vending.Licensing
{
    public static class AN_LicenseChecker
    {


        public static void CheckAccess(Action<AN_LicenseResult> callback) {

            if (!AN_Settings.Instance.Licensing) {
                SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "Licensing");
                return;
            }

            AN_VendingLib.API.CheckAccess(AN_Settings.Instance.RSAPublicKey, callback);
        }

    }
}