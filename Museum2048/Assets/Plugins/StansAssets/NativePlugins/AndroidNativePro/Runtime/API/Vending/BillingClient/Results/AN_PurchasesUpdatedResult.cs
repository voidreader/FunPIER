using System;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    public class AN_PurchasesUpdatedResult : SA_Result
    {
        [SerializeField]
        List<AN_Purchase> m_Purchases = null;

        /// <summary>
        /// Returns the list of <see cref="AN_Purchase"/>.
        /// </summary>
        public List<AN_Purchase> Purchases => m_Purchases;
    }
}
