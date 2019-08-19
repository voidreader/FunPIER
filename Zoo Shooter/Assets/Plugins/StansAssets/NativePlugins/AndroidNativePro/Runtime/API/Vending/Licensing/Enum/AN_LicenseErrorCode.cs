using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.Vending.Licensing
{
    public enum AN_LicenseErrorCode
    {
        ERROR_UNDEFINED = 0,
        ERROR_INVALID_PACKAGE_NAME = 1,
        ERROR_NON_MATCHING_UID = 2,
        ERROR_NOT_MARKET_MANAGED = 3,
        ERROR_CHECK_IN_PROGRESS = 4,
        ERROR_INVALID_PUBLIC_KEY = 5,
        ERROR_MISSING_PERMISSION = 6,
    }
}