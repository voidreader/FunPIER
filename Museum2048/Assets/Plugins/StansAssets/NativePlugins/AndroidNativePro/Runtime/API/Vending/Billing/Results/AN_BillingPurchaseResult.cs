using System;
using UnityEngine;

using SA.Foundation.Templates;


namespace SA.Android.Vending.Billing
{
    [Obsolete("Use AN_BillingClient API instead")]
    public class AN_BillingPurchaseResult : SA_Result
    {
        [SerializeField] AN_Purchase m_purchase;

        public AN_BillingPurchaseResult(SA_iResult result) : base(result)
        {
        }

        public AN_BillingPurchaseResult(AN_Purchase purchase) {
            m_purchase = purchase;
        }

        public AN_Purchase Purchase {
            get {
                return m_purchase;
            }
        }
    }
}