using System;
using System.Collections.Generic;
using SA.Android.Vending.BillingClient;
using SA.Foundation.Utility;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Parameters to initiate a query for SKU details. 
    /// </summary>
    [Serializable]
    public class AN_SkuDetailsParams
    {
        /// <summary>
        /// Helps to construct <see cref="AN_SkuDetailsParams"/> that are used to query for SKU details.
        /// </summary>
        [Serializable]
        public class Builder
        {
            [SerializeField] internal string m_Type;
            [SerializeField] internal List<string> m_SkusList;

            internal Builder() { } 

            /// <summary>
            /// Specify the type of SKUs we are querying for.
            /// Mandatory To query for SKU details
            /// </summary>
            /// <param name="type">SKUs type we are querying for.</param>
            public void SetType(AN_BillingClient.SkuType type)
            {
                m_Type = type.ToString();
            }

            /// <summary>
            /// Specify the SKUs that are queried for as published in the Google Developer console.
            /// Mandatory To query for SKU details
            /// </summary>
            /// <param name="skusList">SKUs list to we are querying.</param>
            public void SetSkusList(List<string> skusList)
            {
                m_SkusList = skusList;
            }

            /// <summary>
            /// Returns <see cref="AN_SkuDetailsParams"/> reference to initiate a purchase flow.
            /// </summary>
            /// <returns><see cref="AN_SkuDetailsParams"/> reference to initiate a purchase flow.</returns>
            public AN_SkuDetailsParams Build()
            {
                return new AN_SkuDetailsParams(this);
            }
        }

        [SerializeField] private Builder m_Builder;

        private AN_SkuDetailsParams(Builder builder)
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
        /// SKUs list to we are querying.
        /// </summary>
        public List<string> SkusList
        {
            get { return m_Builder.m_SkusList; }
        }

        /// <summary>
        /// SKUs type we are querying for.
        /// </summary>
        public AN_BillingClient.SkuType SkuType
        {
            get { return SA_EnumUtil.ParseEnum<AN_BillingClient.SkuType>(m_Builder.m_Type); }
        }
    }
}