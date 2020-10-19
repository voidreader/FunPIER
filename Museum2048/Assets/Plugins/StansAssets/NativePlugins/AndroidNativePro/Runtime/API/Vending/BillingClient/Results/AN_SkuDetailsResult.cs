using System;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    class AN_SkuDetailsResult : SA_Result
    {
        [SerializeField]
        List<AN_SkuDetails> m_SkuDetailsList = null;

        public List<AN_SkuDetails> SkuDetailsList => m_SkuDetailsList;
    }
}
