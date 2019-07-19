using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.GMS.Internal;
using SA.Foundation.Templates;

namespace SA.Android.GMS.Common
{

    public class AN_GoogleApiAvailability
    {

        /// <summary>
        /// Verifies that Google Play services is installed and enabled on this device, 
        /// and that the version installed on this device is no older than the one required by this client.
        /// </summary>
        /// <returns>
        /// status code indicating whether there was an error. 
        /// Can be one of following in <see cref="AN_ConnectionResult"/> 
        /// <see cref="AN_ConnectionResult.SUCCESS"/>, <see cref="AN_ConnectionResult.SERVICE_MISSING"/>, 
        /// <see cref="AN_ConnectionResult.SERVICE_UPDATING"/>, <see cref="AN_ConnectionResult.SERVICE_VERSION_UPDATE_REQUIRED"/>
        ///  <see cref="AN_ConnectionResult.SERVICE_DISABLED"/>, <see cref="AN_ConnectionResult.SERVICE_INVALID"/>.
        /// </returns>
        public static int IsGooglePlayServicesAvailable() {
            return AN_GMS_Lib.Auth.IsGooglePlayServicesAvailable();
        }


        /// <summary>
        /// Attempts to make Google Play services available on this device. 
        /// If Play Services is already available, the returned Task may complete immediately.
        /// 
        /// f it is necessary to display UI in order to complete this request 
        /// (e.g. sending the user to the Google Play store) the <see cref="App.AN_MainActivity"/> will be used to display this UI.
        /// 
        /// This method must be called from the main thread.
        /// </summary>
        /// <param name="callback">
        /// If callback completes with Success result, 
        /// Play Services is available on this device.
        /// </param>
        public static void MakeGooglePlayServicesAvailable(Action<SA_Result> callback) {
            AN_GMS_Lib.Auth.MakeGooglePlayServicesAvailable(callback);
        }
    }
}