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
        [SerializeField] private string m_DeveloperPayload = string.Empty;
        [SerializeField] private string m_OriginalJson = string.Empty;
        [SerializeField] private long m_PurchaseTime = 0;
        [SerializeField] private string m_PurchaseToken = string.Empty;
        [SerializeField] private string m_Signature = string.Empty;
        [SerializeField] private string m_Sku = string.Empty;
        
        
        /// <summary>
        /// Returns the payload specified when the purchase was acknowledged or consumed.
        /// </summary>
        public string DeveloperPayload 
        {
            get { return m_DeveloperPayload; }
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