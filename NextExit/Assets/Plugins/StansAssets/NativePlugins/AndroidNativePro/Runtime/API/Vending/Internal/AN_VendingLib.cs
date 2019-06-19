using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;
using SA.Android.Vending.Billing;

namespace SA.Android.Vending.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class AN_VendingLib
    {
        private static AN_IVendingAPI m_api;
        public static AN_IVendingAPI API {
            get {

                if(!AN_Settings.Instance.Vending) 
                {
                    SA_Plugins.OnDisabledAPIUseAttempt(AN_Settings.PLUGIN_NAME, "Vending");
                }


                if (m_api == null) 
                {
                    switch (Application.platform) 
                    {
                        case RuntimePlatform.Android:
                            m_api = new AnIVendingNativeApi();
                            break;
                        default:
                            m_api = new AnIVendingEditorApi();
                            break;
                    }
                }

                return m_api;
            }
        }


        [Serializable]
        internal class AN_ConnectionRequest
        {
            public string m_base64EncodedPublicKey = string.Empty;
            public List<string> m_inapp = new List<string>();
            public List<string> m_subs = new List<string>();
        }

        [Serializable]
        internal class AN_PurchaseRequest
        {
            public string m_developerPayload = string.Empty;
            public AN_Product m_product;
        }
        
        [Serializable]
        internal class AN_PurchaseSubscriptionReplaceRequest
        {
            public string m_developerPayload = string.Empty;
            public string m_newProductId;
            public List<string> m_oldProductsId;
        }

    }

}
