using System;
using System.Collections.Generic;
using SA.Android.Vending.Billing;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    internal class AN_SkuDetailsResult : SA_Result
    {
        [SerializeField] private List<AN_SkuDetails> m_SkuDetailsList = null;
        
        public List<AN_SkuDetails> SkuDetailsList
        {
            get { return m_SkuDetailsList; }
        }
    }
}