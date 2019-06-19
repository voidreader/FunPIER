using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.App;
using SA.Foundation.Templates;


namespace SA.Android.Vending.Billing
{
    public class AN_BillingConnectionResult : SA_Result
    {

        [SerializeField] int m_inappState;
        [SerializeField] int m_subsState;

        public AN_BillingConnectionResult():base() {
            //Wehn initializaed in editor
            m_inappState = AN_Billing.RESULT_OK;
            m_subsState = AN_Billing.RESULT_OK;
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