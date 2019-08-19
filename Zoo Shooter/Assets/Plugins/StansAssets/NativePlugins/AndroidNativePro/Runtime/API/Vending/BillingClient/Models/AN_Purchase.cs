using System;
using SA.Foundation.Templates;
using UnityEngine;
using SA.Foundation.Utility;
using SA.Foundation.Time;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Represents an in-app billing purchase.
    /// </summary>
    [Serializable]
    public class AN_Purchase
    { 
        /// <summary>
        /// Result list and code for queryPurchases method
        /// </summary>
        [Serializable]
        public class PurchasesResult : AN_PurchasesUpdatedResult
        {
            public int m_ResponseCode;

            /// <summary>
            /// Returns the response code of In-app Billing API calls.
            /// </summary>
            public AN_BillingClient.BillingResponseCode ResponseCode
            {
                get { return (AN_BillingClient.BillingResponseCode) m_ResponseCode; }
            }

            /// <summary>
            /// Returns the BillingResult of the operation.
            /// </summary>
            public SA_Result BillingResult
            {
                get { return this; }
            }
            
        }
        
        /// <summary>
        /// Possible purchase states.
        /// </summary>
        public enum State
        {
            Unspecified = 0,
            Purchased = 1,
            Pending = 2
        }

        //internal
        [SerializeField] string m_Type;
        
        [SerializeField] string m_OrderId;
        [SerializeField] string m_PackageName;
        [SerializeField] long m_PurchaseTime;
        [SerializeField] int m_PurchaseState;
        [SerializeField] string m_DeveloperPayload;
        [SerializeField] string m_PurchaseToken;
        [SerializeField] bool m_IsAutoRenewing;
        [SerializeField] bool m_IsAcknowledged;
        [SerializeField] string m_OriginalJson;
        [SerializeField] string m_Signature;
        [SerializeField] string m_Sku;


        //For editor only
        public AN_Purchase(string productId, string type, string developerPayload) {
            m_Type = type;
            m_Sku = productId;
            m_DeveloperPayload = developerPayload;

            m_PackageName = Application.identifier;
            m_OrderId = SA_IdFactory.RandomString;
            m_PurchaseTime = SA_Unix_Time.ToUnixTime(DateTime.Now);
            m_PurchaseState = 0;
            m_PurchaseToken = SA_IdFactory.RandomString;
            m_Signature = SA_IdFactory.RandomString;

            m_IsAutoRenewing = false;
            m_IsAcknowledged = false;
            m_OriginalJson = JsonUtility.ToJson(this);
        }


        /// <summary>
        /// Type of the purchased product
        /// </summary>
        public AN_BillingClient.SkuType Type 
        {
            get { return SA_EnumUtil.ParseEnum<AN_BillingClient.SkuType>(m_Type); }
        }

        /// <summary>
        /// Returns the payload specified when the purchase was acknowledged or consumed.
        /// </summary>
        public string DeveloperPayload 
        {
            get { return m_DeveloperPayload; }
        }

        /// <summary>
        /// A unique order identifier for the transaction. This identifier corresponds to the Google payments order ID
        /// </summary>
        public string OrderId 
        {
            get { return m_OrderId; }
        }

        /// <summary>
        /// The application package from which the purchase originated.
        /// </summary>
        public string PackageName 
        {
            get { return m_PackageName; }
        }

        /// <summary>
        /// The purchase state of the order.
        /// </summary>
        public State PurchaseState
        {
            get { return (State) m_PurchaseState; }
        }
        
        /// <summary>
        /// The time the product was purchased, in milliseconds since the epoch (Jan 1, 1970).
        /// </summary>
        public long PurchaseTime 
        {
            get { return m_PurchaseTime; }
        }
        
        /// <summary>
        /// A token that uniquely identifies a purchase for a given item and user pair.
        /// </summary>
        public string PurchaseToken 
        {
            get { return m_PurchaseToken; }
        }
        
        /// <summary>
        /// String containing the signature of the purchase data that was signed with the private key of the developer.
        /// </summary>
        public string Signature 
        {
            get { return m_Signature; }
        }

        /// <summary>
        /// Returns the product Id.
        /// </summary>
        public string Sku
        {
            get { return m_Sku; }
        }
        
        /// <summary>
        /// Indicates whether the purchase has been acknowledged.
        /// </summary>
        public bool IsAcknowledged
        {
            get { return m_IsAcknowledged; }
        }

        /// <summary>
        /// Indicates whether the subscription renews automatically. 
        /// If true, the subscription is active, and will automatically renew on the next billing date. 
        /// If false, indicates that the user has canceled the subscription. 
        /// The user has access to subscription content until the next billing date and will lose access 
        /// at that time unless they re-enable automatic renewal 
        /// (or manually renew, as described in Manual Renewal). If you offer a grace period, 
        /// this value remains set to true for all subscriptions, as long as the grace period has not lapsed. 
        /// The next billing date is extended dynamically every day until the end of the grace period 
        /// or until the user fixes their payment method.
        /// </summary>
        /// <value><c>true</c> if auto renewing; otherwise, <c>false</c>.</value>
        public bool IsAutoRenewing 
        {
            get { return m_IsAutoRenewing; }
        }
        
        /// <summary>
        /// Original non modified google billing service response.
        /// </summary>
        public string OriginalJson 
        {
            get { return m_OriginalJson; }
        }
        
        public override string ToString()
        {
            return OriginalJson;
        }
    }
}