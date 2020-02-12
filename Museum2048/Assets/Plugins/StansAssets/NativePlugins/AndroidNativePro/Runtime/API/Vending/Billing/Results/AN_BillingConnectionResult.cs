using System;
using UnityEngine;
using SA.Foundation.Templates;

namespace SA.Android.Vending.Billing
{
    [Obsolete("Use AN_BillingClient API instead")]
    public class AN_BillingConnectionResult : SA_Result
    {

        [SerializeField] int m_inappState;
        [SerializeField] int m_subsState;

        public AN_BillingConnectionResult():base() {
            //When initialized in editor
            m_inappState = AN_Billing.RESULT_OK;
            m_subsState = AN_Billing.RESULT_OK;
        }

        public AN_BillingConnectionResult(SA_iResult result):base(result) {
            if (result.IsSucceeded)
            {
                m_inappState = AN_Billing.RESULT_OK;
                m_subsState = AN_Billing.RESULT_OK;
            }
        }

        public bool IsInAppsAPIAvalible {
            get {
                return m_inappState == AN_Billing.RESULT_OK;
            }
        }

        public bool IsSubsAPIAvalible {
            get {
                return m_subsState == AN_Billing.RESULT_OK;
            }
        }
    }
}