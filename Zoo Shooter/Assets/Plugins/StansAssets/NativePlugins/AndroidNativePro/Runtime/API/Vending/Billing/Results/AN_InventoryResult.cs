using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;

namespace SA.Android.Vending.Billing
{
    [Obsolete("Use AN_BillingClient API instead")]
    [Serializable]
    public class AN_InventoryResult : SA_Result
    {

        [SerializeField] AN_Inventory m_inventory;

        public AN_InventoryResult() : base() {
            m_inventory = new AN_Inventory();
        }

        public AN_Inventory Inventory {
            get {
                return m_inventory;
            }
        }
    }
}
