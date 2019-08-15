using System;
using UnityEngine;
using SA.Foundation.Utility;
using SA.Foundation.Time;

namespace SA.Android.Vending.Billing
{
    /// <summary>
    /// Response data from an In-app Billing purchase request.
    /// </summary>
    [Serializable]
    [Obsolete("Use AN_BillingClient API instead")]
    public class AN_Purchase
    { 
        [SerializeField] string m_type;  
        [SerializeField] string m_orderId;
        [SerializeField] string m_packageName;
        [SerializeField] string m_productId;
        [SerializeField] long m_purchaseTime;
        [SerializeField] int m_purchaseState;
        [SerializeField] string m_developerPayload;
        [SerializeField] string m_token;
        [SerializeField] string m_originalJson;
        [SerializeField] string m_signature;
        [SerializeField] bool m_autoRenewing;


        //For editor only
        public AN_Purchase(string productId, string type, string developerPayload) {
            m_type = type;
            m_productId = productId;
            m_developerPayload = developerPayload;

            m_packageName = Application.identifier;
            m_orderId = SA_IdFactory.RandomString;
            m_purchaseTime = SA_Unix_Time.ToUnixTime(DateTime.Now);
            m_purchaseState = 0;
            m_token = SA_IdFactory.RandomString;
            m_signature = SA_IdFactory.RandomString;

            m_autoRenewing = false;

            m_originalJson = JsonUtility.ToJson(this);
        }

        public AN_Purchase(SA.Android.Vending.BillingClient.AN_Purchase purchase)
        {
            m_type = purchase.Type.ToString();
            m_orderId = purchase.OrderId;
            m_packageName = purchase.PackageName;
            m_productId = purchase.Sku;
            m_purchaseTime = purchase.PurchaseTime;
            m_purchaseState = (int) purchase.PurchaseState;
            m_developerPayload = purchase.DeveloperPayload;
            m_token = purchase.PurchaseToken;
            m_originalJson = purchase.OriginalJson;
            m_signature = purchase.Signature;
            m_autoRenewing = purchase.IsAutoRenewing;
        }


        /// <summary>
        /// Type of the purchased product
        /// </summary>
        public AN_ProductType Type {
            get {
                return SA_EnumUtil.ParseEnum<AN_ProductType>(m_type);
            }
        }


        /// <summary>
        /// A unique order identifier for the transaction. This identifier corresponds to the Google payments order ID
        /// </summary>
        public string OrderId {
            get {
                return m_orderId;
            }
        }

        /// <summary>
        /// The application package from which the purchase originated.
        /// </summary>
        public string PackageName {
            get {
                return m_packageName;
            }
        }

        /// <summary>
        /// The item's product identifier. 
        /// Every item has a product ID, which you must specify in the application's product list on the Google Play Console.
        /// </summary>
        public string ProductId {
            get {
                return m_productId;
            }
        }

        /// <summary>
        /// The time the product was purchased, in milliseconds since the epoch (Jan 1, 1970).
        /// </summary>
        public long PurchaseTime {
            get {
                return m_purchaseTime;
            }

        }

        /// <summary>
        /// The purchase state of the order. It always returns 0 (purchased).
        /// </summary>
        public int PurchaseState {
            get {
                return m_purchaseState;
            }
        }

        /// <summary>
        /// A developer-specified string that contains supplemental information about an order. 
        /// You can specify a value for this field when you make <see cref=" AN_Billing.Purchase"/> request.
        /// </summary>
        public string DeveloperPayload {
            get {
               return m_developerPayload;
            }

        }

        /// <summary>
        /// A token that uniquely identifies a purchase for a given item and user pair.
        /// </summary>
        public string Token {
            get {
                return m_token;
            }
        }


        /// <summary>
        /// Original non modified google billing service response.
        /// </summary>
        public string OriginalJson {
            get {
                return m_originalJson;
            }
        }

        /// <summary>
        /// String containing the signature of the purchase data that was signed with the private key of the developer.
        /// The data signature uses the RSASSA-PKCS1-v1_5 scheme.
        /// </summary>
        public string Signature {
            get {
                return m_signature;
            }
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
        public bool AutoRenewing {
            get {
                return m_autoRenewing;
            }
        }

        public override string ToString()
        {
            return OriginalJson;
        }
    }
}