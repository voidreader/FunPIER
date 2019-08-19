using System;
using SA.Android.Vending.Billing;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Parameters to load a rewarded SKU.
    /// See <see cref="AN_BillingClient.LoadRewardedSku"/>
    /// </summary>
    [Serializable]
    public class AN_RewardLoadParams
    {
        /// <summary>
        /// Helps construct <see cref="AN_RewardLoadParams"/> that are used to load rewarded SKUs.
        /// </summary>
        public class Builder
        {
            internal AN_SkuDetails m_SkuDetails;
            
            internal Builder() { } 

            /// <summary>
            /// Specify the SKU to load
            /// </summary>
            /// <param name="skuDetails">
            /// Required, the sku details object from
            /// <see cref="AN_BillingClient.QuerySkuDetailsAsync"/>.
            /// </param>
            public void SetSkuDetails(AN_SkuDetails skuDetails)
            {
                m_SkuDetails = skuDetails;
            }

            /// <summary>
            /// Returns <see cref="AN_RewardLoadParams"/> reference to initiate load.
            /// </summary>
            /// <returns><see cref="AN_RewardLoadParams"/> reference to initiate load.</returns>
            public AN_RewardLoadParams Build()
            {
                return new AN_RewardLoadParams(this);
            }

        }

#pragma warning disable 414
        private Builder m_Builder;
        [SerializeField] private int m_NativeHashId;
#pragma warning restore 414

        
        private AN_RewardLoadParams(Builder builder)
        {
            m_Builder = builder;
            m_NativeHashId = builder.m_SkuDetails.NativeHashId;
        }

        /// <summary>
        /// Request sku details. 
        /// </summary>
        public AN_SkuDetails SkuDetails
        {
            get { return m_Builder.m_SkuDetails; }
        }
    }
}