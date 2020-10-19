using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Represents an in-app billing purchase history record.
    /// This class includes a subset of fields in <see cref="AN_Purchase"/>.
    /// </summary>
    [System.Serializable]
    public class AN_PurchaseHistoryRecord
    {
        [SerializeField]
        string m_DeveloperPayload = string.Empty;
        [SerializeField]
        string m_OriginalJson = string.Empty;
        [SerializeField]
        long m_PurchaseTime = 0;
        [SerializeField]
        string m_PurchaseToken = string.Empty;
        [SerializeField]
        string m_Signature = string.Empty;
        [SerializeField]
        string m_Sku = string.Empty;

        /// <summary>
        /// Returns the payload specified when the purchase was acknowledged or consumed.
        /// </summary>
        public string DeveloperPayload => m_DeveloperPayload;

        /// <summary>
        /// The time the product was purchased, in milliseconds since the epoch (Jan 1, 1970).
        /// </summary>
        public long PurchaseTime => m_PurchaseTime;

        /// <summary>
        /// A token that uniquely identifies a purchase for a given item and user pair.
        /// </summary>
        public string PurchaseToken => m_PurchaseToken;

        /// <summary>
        /// String containing the signature of the purchase data that was signed with the private key of the developer.
        /// </summary>
        public string Signature => m_Signature;

        /// <summary>
        /// Returns the product Id.
        /// </summary>
        public string Sku => m_Sku;

        /// <summary>
        /// Original non modified google billing service response.
        /// </summary>
        public string OriginalJson => m_OriginalJson;

        public override string ToString()
        {
            return OriginalJson;
        }
    }
}
