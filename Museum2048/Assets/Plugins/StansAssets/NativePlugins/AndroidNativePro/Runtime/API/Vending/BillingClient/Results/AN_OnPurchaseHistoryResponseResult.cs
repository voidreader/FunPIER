using System;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    class AN_OnPurchaseHistoryResponseResult : SA_Result
    {
        [SerializeField]
        List<AN_PurchaseHistoryRecord> m_PurchaseHistoryRecordList = null;

        public List<AN_PurchaseHistoryRecord> PurchaseHistoryRecordList => m_PurchaseHistoryRecordList;
    }
}
