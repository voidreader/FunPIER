using System;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    internal class AN_OnConsumeResponseResult : SA_Result
    {
        [SerializeField] private string m_PurchaseToken = string.Empty;

        public string PurchaseToken
        {
            get { return m_PurchaseToken; }
        }
    }
}