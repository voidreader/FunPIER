namespace SA.Android.GMS.Common
{
    /// <summary>
    /// Common status codes that are often shared across API surfaces.
    /// </summary>
    public enum AN_CommonStatusCodes 
    {
        /// <summary>
        /// The operation was successful, but was used the device's cache. 
        /// If this is a write, the data will be written when the device is online; errors will be written to the logs. 
        /// If this is a read, the data was read from a device cache and may be stale.
        /// </summary>
        SUCCESS_CACHE = -1,

        /// <summary>
        /// The operation was successful.
        /// </summary>
        SUCCESS = 0,


        /// <summary>
        /// The client attempted to connect to the service but the user is not signed in. 
        /// The client may choose to continue without using the API. 
        /// Alternately, if hasResolution() returns true the client may call startResolutionForResult(Activity, int) 
        /// to prompt the user to sign in. After the sign in activity returns with RESULT_OK further attempts should succeed.
        /// </summary>
        SIGN_IN_REQUIRED = 4,

        /// <summary>
        /// The client attempted to connect to the service with an invalid account name specified.
        /// </summary>
        INVALID_ACCOUNT = 5,

        /// <summary>
        /// Completing the operation requires some form of resolution. 
        /// A resolution will be available to be started with startResolutionForResult(Activity, int). 
        /// If the result returned is RESULT_OK, 
        /// then further attempts should either complete or continue on to the next issue that needs to be resolved.
        /// </summary>
        RESOLUTION_REQUIRED = 6,

        /// <summary>
        /// A network error occurred. Retrying should resolve the problem.
        /// </summary>
        NETWORK_ERROR = 7,


        /// <summary>
        /// An internal error occurred. Retrying should resolve the problem.
        /// </summary>
        INTERNAL_ERROR = 8,

        /// <summary>
        /// The application is misconfigured. This error is not recoverable and will be treated as fatal. 
        /// The developer should look at the logs after this to determine more actionable information.
        /// </summary>
        DEVELOPER_ERROR = 10,

        /// <summary>
        /// The operation failed with no more detailed information.
        /// </summary>
        ERROR = 13,


        /// <summary>
        /// A blocking call was interrupted while waiting and did not run to completion.
        /// </summary>
        INTERRUPTED = 14,


        /// <summary>
        /// Timed out while awaiting the result.
        /// </summary>
        TIMEOUT = 15,

        /// <summary>
        /// The result was canceled either due to client disconnect or cancel.
        /// </summary>
        CANCELED = 16,


        /// <summary>
        /// The client attempted to call a method from an API that failed to connect. Possible reasons include:
        /// The API previously failed to connect with a resolvable error, but the user declined the resolution.
        /// The device does not support GmsCore.
        /// The specific API cannot connect on this device.
        /// </summary>
        API_NOT_CONNECTED = 17,
    }
}