using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SA.Android.GMS.Common {

    /// <summary>
    /// Contains all possible error codes for when a client fails to connect to Google Play services.
    /// </summary>
    public class AN_ConnectionResult 
    {

        public const int UNKNOWN = -1;
        public const int SUCCESS = 0;
        public const int SERVICE_MISSING = 1;
        public const int SERVICE_VERSION_UPDATE_REQUIRED = 2;
        public const int SERVICE_DISABLED = 3;
        public const int SIGN_IN_REQUIRED = 4;
        public const int INVALID_ACCOUNT = 5;
        public const int RESOLUTION_REQUIRED = 6;
        public const int NETWORK_ERROR = 7;
        public const int INTERNAL_ERROR = 8;
        public const int SERVICE_INVALID = 9;
        public const int DEVELOPER_ERROR = 10;
        public const int LICENSE_CHECK_FAILED = 11;
        public const int CANCELED = 13;
        public const int TIMEOUT = 14;
        public const int INTERRUPTED = 15;
        public const int API_UNAVAILABLE = 16;
        public const int SIGN_IN_FAILED = 17;
        public const int SERVICE_UPDATING = 18;
        public const int SERVICE_MISSING_PERMISSION = 19;
        public const int RESTRICTED_PROFILE = 20;
        public const int API_VERSION_UPDATE_REQUIRED = 21;
        public const int UNFINISHED = 99;

        public const int DRIVE_EXTERNAL_STORAGE_REQUIRED = 1500;

      
    }
}