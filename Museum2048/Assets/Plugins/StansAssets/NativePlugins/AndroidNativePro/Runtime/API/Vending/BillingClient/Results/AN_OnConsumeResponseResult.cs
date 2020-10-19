using System;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    class AN_OnConsumeResponseResult : SA_Result
    {
        [SerializeField]
        string m_PurchaseToken = string.Empty;

        public string PurchaseToken => m_PurchaseToken;
    }
}
