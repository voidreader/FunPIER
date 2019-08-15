using System;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Parameters to consume a purchase. (See consumeAsync(ConsumeParams, ConsumeResponseListener)
    /// </summary>
    [Serializable]
    public class AN_ConsumeParams
    {
#pragma warning disable 414
        [SerializeField] private Builder m_Builder;
#pragma warning restore 414
        
        /// <summary>
        /// Helps construct <see cref="AN_ConsumeParams"/> that are used to consume a purchase.
        /// </summary>
        [Serializable]
        public class Builder
        {
#pragma warning disable 414
            [SerializeField] private string m_DeveloperPayload;
            [SerializeField] private string m_PurchaseToken;
#pragma warning restore 414

            internal Builder() { }
            
            /// <summary>
            /// Specify developer payload be sent back with the purchase information.
            /// </summary>
            /// <param name="developerPayload">Developer Payload</param>
            public void SetDeveloperPayload(string developerPayload)
            {
                m_DeveloperPayload = developerPayload;
            }

            /// <summary>
            /// Specify the token that identifies the purchase to be consumed.
            /// </summary>
            /// <param name="purchaseToken">Purchase Token</param>
            public void SetPurchaseToken(string purchaseToken)
            {
                m_PurchaseToken = purchaseToken;
            }

            /// <summary>
            /// Returns <see cref="AN_ConsumeParams"/> reference to initiate consume action.
            /// </summary>
            /// <returns></returns>
            public AN_ConsumeParams Build()
            {
                return new AN_ConsumeParams(this);
            }
        }

        private AN_ConsumeParams(Builder builder)
        {
            m_Builder = builder;
        }

        /// <summary>
        /// Constructs a new <see cref="Builder"/> instance.
        /// </summary>
        /// <returns>a new <see cref="Builder"/> instance.</returns>
        public static Builder NewBuilder()
        {
            return  new Builder();
        }
    }
}