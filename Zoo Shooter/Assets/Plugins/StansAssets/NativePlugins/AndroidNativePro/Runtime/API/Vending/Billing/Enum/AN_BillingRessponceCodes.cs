using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.Vending.Billing
{
    [Obsolete("Use AN_BillingClient API instead")]
    public enum AN_BillingRessponceCodes 
    {
        /// <summary>
        /// Success
        /// </summary>
        BILLING_RESPONSE_RESULT_OK  =  0,   

        /// <summary>
        /// User pressed back or canceled a dialog
        /// </summary>
        BILLING_RESPONSE_RESULT_USER_CANCELED  = 1,   

        /// <summary>
        /// Network connection is down
        /// </summary>
        BILLING_RESPONSE_RESULT_SERVICE_UNAVAILABLE = 2,  

        /// <summary>
        /// Billing API version is not supported for the type requested
        /// </summary>
        BILLING_RESPONSE_RESULT_BILLING_UNAVAILABLE = 3,   

        /// <summary>
        ///  Requested product is not available for purchase
        /// </summary>
        BILLING_RESPONSE_RESULT_ITEM_UNAVAILABLE = 4,  

        /// <summary>
        /// Invalid arguments provided to the API. 
        /// This error can also indicate that the application was not correctly signed or properly set up 
        /// for In-app Billing in Google Play, or does not have the necessary permissions in its manifest
        /// </summary>
        BILLING_RESPONSE_RESULT_DEVELOPER_ERROR = 5,   

        /// <summary>
        ///  Fatal error during the API action
        /// </summary>
        BILLING_RESPONSE_RESULT_ERROR  = 6,

        /// <summary>
        /// Failure to purchase since item is already owned
        /// </summary>
        BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED = 7,   

        /// <summary>
        /// Failure to consume since item is not owned
        /// </summary>
        BILLING_RESPONSE_RESULT_ITEM_NOT_OWNED = 8   
       
    }
}
