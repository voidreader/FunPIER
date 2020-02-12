using System;
using SA.Foundation.Utility;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    public class AN_SkuDetails
    {
        [SerializeField] private string m_Type;
        [SerializeField] private string m_Sku;
        [SerializeField] private string m_Title = string.Empty;
        [SerializeField] private string m_Description = string.Empty;
        [SerializeField] private string m_FreeTrialPeriod = string.Empty;
        [SerializeField] private string m_IconUrl = string.Empty;
        [SerializeField] private string m_IntroductoryPrice = string.Empty;
        [SerializeField] private long m_IntroductoryPriceAmountMicros = 0;
        [SerializeField] private string m_IntroductoryPriceCycles = string.Empty;
        [SerializeField] private string m_IntroductoryPricePeriod = string.Empty;
        [SerializeField] private string m_OriginalPrice = string.Empty;
        [SerializeField] private long m_OriginalPriceAmountMicros = 0;
        [SerializeField] private string m_Price = string.Empty;
        [SerializeField] private long m_PriceAmountMicros = 0;
        [SerializeField] private string m_PriceCurrencyCode = string.Empty;
        [SerializeField] private string m_SubscriptionPeriod = string.Empty;
        [SerializeField] private bool m_IsRewarded = false;
        [SerializeField] private string m_OriginalJson = string.Empty;
        [SerializeField] private int m_SkuDetailsHashId = 0;

         //Custom data (does not exists in the native part)
        [SerializeField] private Texture2D m_SettingsIcon = null;
        [SerializeField] private bool m_IsConsumable = false;
       
        
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
            get { return SA_EnumUtil.ParseEnum<AN_BillingClient.SkuType>(m_Type); }
            set { m_Type = value.ToString(); }
        }

        /// <summary>
        /// Returns the product Id.
        /// </summary>
        public string Sku
        {
            get { return m_Sku; }
            set { m_Sku = value; }
        }

        /// <summary>
        /// Returns the title of the product.
        /// </summary>
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        /// <summary>
        /// Returns the description of the product.
        /// </summary>
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        /// <summary>
        /// Trial period configured in Google Play Console, specified in ISO 8601 format.
        /// </summary>
        public string FreeTrialPeriod
        {
            get { return m_FreeTrialPeriod; }
        }

        /// <summary>
        /// Returns the icon of the product if present.
        /// </summary>
        public string IconUrl
        {
            get { return m_IconUrl; }
        }

        /// <summary>
        /// Formatted introductory price of a subscription, including its currency sign, such as €3.99.
        /// </summary>
        public string IntroductoryPrice
        {
            get { return m_IntroductoryPrice; }
        }

        /// <summary>
        /// Introductory price in micro-units.
        /// </summary>
        public long IntroductoryPriceAmountMicros
        {
            get { return m_IntroductoryPriceAmountMicros; }
        }

        /// <summary>
        /// The number of subscription billing periods for which the user will be given the introductory price, such as 3.
        /// </summary>
        public string IntroductoryPriceCycles
        {
            get { return m_IntroductoryPriceCycles; }
        }

        /// <summary>
        /// The billing period of the introductory price, specified in ISO 8601 format.
        /// </summary>
        public string IntroductoryPricePeriod
        {
            get { return m_IntroductoryPricePeriod; }
        }

        /// <summary>
        /// Returns formatted original price of the item, including its currency sign.
        /// </summary>
        public string OriginalPrice
        {
            get { return m_OriginalPrice; }
        }

        /// <summary>
        /// Returns the original price in micro-units, where 1,000,000 micro-units equal one unit of the currency.
        /// </summary>
        public long OriginalPriceAmountMicros
        {
            get { return m_OriginalPriceAmountMicros; }
        }

        /// <summary>
        /// Returns formatted price of the item, including its currency sign.
        /// </summary>
        public string Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        /// <summary>
        /// Returns price in micro-units, where 1,000,000 micro-units equal one unit of the currency.
        /// </summary>
        public long PriceAmountMicros
        {
            get { return m_PriceAmountMicros; }
        }

        /// <summary>
        /// Returns ISO 4217 currency code for price and original price.
        /// </summary>
        public string PriceCurrencyCode
        {
            get { return m_PriceCurrencyCode; }
        }

        /// <summary>
        /// Subscription period, specified in ISO 8601 format.
        /// </summary>
        public string SubscriptionPeriod
        {
            get { return m_SubscriptionPeriod; }
        }

        /// <summary>
        /// Returns true if sku is rewarded instead of paid.
        /// </summary>
        public bool IsRewarded
        {
            get { return m_IsRewarded; }
        }

        /// <summary>
        /// Returns a String in JSON format that contains Sku details.
        /// </summary>
        public string OriginalJson
        {
            get { return m_OriginalJson; }
        }

        internal int NativeHashId
        {
            get { return m_SkuDetailsHashId; }
        }

        public bool IsConsumable
        {
            get { return m_IsConsumable; }
            set { m_IsConsumable = value; }
        }

        public Texture2D Icon
        {
            get { return m_SettingsIcon; }
            set { m_SettingsIcon = value; }
        }
    }
}