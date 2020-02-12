using System;
using System.Collections.Generic;
using SA.Android.Vending.Billing;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    public class AN_PurchasesUpdatedResult : SA_Result
    {
        [SerializeField] private List<AN_Purchase> m_Purchases = null;

        /// <summary>
        /// Returns the list of <see cref="AN_Purchase"/>.
        /// </summary>
        public List<AN_Purchase> Purchases
        {
            get { return m_Purchases; }
        }
    }
}