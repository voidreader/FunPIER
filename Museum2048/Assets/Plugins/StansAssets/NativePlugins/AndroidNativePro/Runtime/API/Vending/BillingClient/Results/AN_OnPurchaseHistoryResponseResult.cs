using System;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    internal class AN_OnPurchaseHistoryResponseResult : SA_Result
    {
        [SerializeField] private List<AN_PurchaseHistoryRecord> m_PurchaseHistoryRecordList = null;

        public List<AN_PurchaseHistoryRecord> PurchaseHistoryRecordList
        {
            get { return m_PurchaseHistoryRecordList; }
        }
    }
}