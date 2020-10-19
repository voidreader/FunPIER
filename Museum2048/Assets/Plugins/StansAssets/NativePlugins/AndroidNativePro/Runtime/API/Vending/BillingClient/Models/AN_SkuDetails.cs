using System;
using StansAssets.Foundation;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    public class AN_SkuDetails
    {
        [SerializeField]
        string m_Type;
        [SerializeField]
        string m_Sku;
        [SerializeField]
        string m_Title = string.Empty;
        [SerializeField]
        string m_Description = string.Empty;
        [SerializeField]
        string m_FreeTrialPeriod = string.Empty;
        [SerializeField]
        string m_IconUrl = string.Empty;
        [SerializeField]
        string m_IntroductoryPrice = string.Empty;
        [SerializeField]
        long m_IntroductoryPriceAmountMicros = 0;
        [SerializeField]
        string m_IntroductoryPriceCycles = string.Empty;
        [SerializeField]
        string m_IntroductoryPricePeriod = string.Empty;
        [SerializeField]
        string m_OriginalPrice = string.Empty;
        [SerializeField]
        long m_OriginalPriceAmountMicros = 0;
        [SerializeField]
        string m_Price = string.Empty;
        [SerializeField]
        long m_PriceAmountMicros = 0;
        [SerializeField]
        string m_PriceCurrencyCode = string.Empty;
        [SerializeField]
        string m_SubscriptionPeriod = string.Empty;
        [SerializeField]
        bool m_IsRewarded = false;
        [SerializeField]
        string m_OriginalJson = string.Empty;
        [SerializeField]
        int m_SkuDetailsHashId = 0;

        //Custom data (does not exists in the native part)
        [SerializeField]
        Texture2D m_SettingsIcon = null;
        [SerializeField]
        bool m_IsConsumable = false;

        internal AN_SkuDetails(string sku, AN_BillingClient.SkuType productType)
        {
            m_Sku = sku;
            m_Type = productType.ToString();
        }

        /// <summary>
        /// Returns SKU type.
        /// </summary>
        public AN_BillingClient.SkuType Type
        {
            get => EnumUtility.ParseEnum<AN_BillingClient.SkuType>(m_Type);
            set => m_Type = value.ToString();
        }

        /// <summary>
        /// Returns the product Id.
        /// </summary>
        public string Sku
        {
            get => m_Sku;
            set => m_Sku = value;
        }

        /// <summary>
        /// Returns the title of the product.
        /// </summary>
        public string Title
        {
            get => m_Title;
            set => m_Title = value;
        }

        /// <summary>
        /// Returns the description of the product.
        /// </summary>
        public string Description
        {
            get => m_Description;
            set => m_Description = value;
        }

        /// <summary>
        /// Trial period configured in Google Play Console, specified in ISO 8601 format.
        /// </summary>
        public string FreeTrialPeriod => m_FreeTrialPeriod;

        /// <summary>
        /// Returns the icon of the product if present.
        /// </summary>
        public string IconUrl => m_IconUrl;

        /// <summary>
        /// Formatted introductory price of a subscription, including its currency sign, such as €3.99.
        /// </summary>
        public string IntroductoryPrice => m_IntroductoryPrice;

        /// <summary>
        /// Introductory price in micro-units.
        /// </summary>
        public long IntroductoryPriceAmountMicros => m_IntroductoryPriceAmountMicros;

        /// <summary>
        /// The number of subscription billing periods for which the user will be given the introductory price, such as 3.
        /// </summary>
        public string IntroductoryPriceCycles => m_IntroductoryPriceCycles;

        /// <summary>
        /// The billing period of the introductory price, specified in ISO 8601 format.
        /// </summary>
        public string IntroductoryPricePeriod => m_IntroductoryPricePeriod;

        /// <summary>
        /// Returns formatted original price of the item, including its currency sign.
        /// </summary>
        public string OriginalPrice => m_OriginalPrice;

        /// <summary>
        /// Returns the original price in micro-units, where 1,000,000 micro-units equal one unit of the currency.
        /// </summary>
        public long OriginalPriceAmountMicros => m_OriginalPriceAmountMicros;

        /// <summary>
        /// Returns formatted price of the item, including its currency sign.
        /// </summary>
        public string Price
        {
            get => m_Price;
            set => m_Price = value;
        }

        /// <summary>
        /// Returns price in micro-units, where 1,000,000 micro-units equal one unit of the currency.
        /// </summary>
        public long PriceAmountMicros => m_PriceAmountMicros;

        /// <summary>
        /// Returns ISO 4217 currency code for price and original price.
        /// </summary>
        public string PriceCurrencyCode => m_PriceCurrencyCode;

        /// <summary>
        /// Subscription period, specified in ISO 8601 format.
        /// </summary>
        public string SubscriptionPeriod => m_SubscriptionPeriod;

        /// <summary>
        /// Returns true if sku is rewarded instead of paid.
        /// </summary>
        public bool IsRewarded => m_IsRewarded;

        /// <summary>
        /// Returns a String in JSON format that contains Sku details.
        /// </summary>
        public string OriginalJson => m_OriginalJson;

        internal int NativeHashId => m_SkuDetailsHashId;

        public bool IsConsumable
        {
            get => m_IsConsumable;
            set => m_IsConsumable = value;
        }

        public Texture2D Icon
        {
            get => m_SettingsIcon;
            set => m_SettingsIcon = value;
        }
    }
}
