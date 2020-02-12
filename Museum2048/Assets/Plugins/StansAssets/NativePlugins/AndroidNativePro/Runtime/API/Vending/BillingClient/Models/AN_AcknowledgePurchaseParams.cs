using System;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Parameters to acknowledge a purchase.
    /// <see cref="AN_BillingClient.AcknowledgePurchase(AN_AcknowledgePurchaseParams, AN_iAcknowledgePurchaseResponseListener)"/>
    /// </summary>
    [Serializable]
    public class AN_AcknowledgePurchaseParams
    {
        /// <summary>
        /// Helps construct <see cref="AN_AcknowledgePurchaseParams"/> that are used to acknowledge a purchase.
        /// </summary>
        [Serializable]
        public class Builder
        {
#pragma warning disable 414
            [SerializeField] internal string m_DeveloperPayload = string.Empty;
            [SerializeField] internal string m_PurchaseToken = string.Empty;
#pragma warning restore 414

            internal Builder()
            {
            }

            /// <summary>
            /// Returns <see cref="AN_AcknowledgePurchaseParams"/> reference to initiate a purchase flow.
            /// </summary>
            /// <returns><see cref="AN_AcknowledgePurchaseParams"/> reference to initiate a purchase flow.</returns>
            public AN_AcknowledgePurchaseParams Build()
            {
                return new AN_AcknowledgePurchaseParams(this);
            }

            /// <summary>
            /// Specify developer payload be sent back with the purchase information.
            /// </summary>
            /// <param name="developerPayload">Developer Payload string.</param>
            /// <returns><see cref="AN_AcknowledgePurchaseParams"/> reference.</returns>
            public Builder SetDeveloperPayload(string developerPayload)
            {
                m_DeveloperPayload = developerPayload;
                return this;
            }
            
            /// <summary>
            /// Specify the token that identifies the purchase to be acknowledged.
            /// </summary>
            /// <param name="purchaseToken">Purchase Token string.</param>
            /// <returns><see cref="AN_AcknowledgePurchaseParams"/> reference.</returns>
            public Builder SetPurchaseToken(string purchaseToken)
            {
                m_PurchaseToken = purchaseToken;
                return this;
            }
        }
        
#pragma warning disable 414
        [SerializeField] private Builder m_Builder;
#pragma warning restore 414

        private AN_AcknowledgePurchaseParams(Builder builder)
        {
            m_Builder = builder;
        }

        /// <summary>
        /// Constructs a new <see cref="Builder"/> instance.
        /// </summary>
        /// <returns>a new <see cref="Builder"/> instance.</returns>
        public static Builder NewBuilder()
        {
            return new Builder();
        }

        /// <summary>
        /// Returns developer data associated with the purchase to be acknowledged.
        /// </summary>
        public string DeveloperPayload
        {
            get { return m_Builder.m_DeveloperPayload; }
        }
        
        /// <summary>
        /// Returns token that identifies the purchase to be acknowledged
        /// </summary>
        public string PurchaseToken
        {
            get { return m_Builder.m_PurchaseToken; }
        }
    }
}