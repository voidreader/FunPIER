using System;
using SA.Android.Vending.BillingClient;
using UnityEngine;

using SA.Foundation.Utility;


namespace SA.Android.Vending.Billing
{
    /// <summary>
    /// An object to define Google Play in-app product.
    /// </summary>
    [Serializable]
    [Obsolete("Use AN_BillingClient API instead")]
    public class AN_Product
    {

        //We just using this predefined data, so it would look nice in the editor,
        //when user adds a new produt.
        [SerializeField] string m_price = "$0.99";
        [SerializeField] string m_title = "New Product";
        [SerializeField] string m_type = AN_ProductType.inapp.ToString();

        [SerializeField] string m_productId = string.Empty;

        [SerializeField] string m_description = string.Empty;
        [SerializeField] string m_priceAmountMicros = string.Empty;
        [SerializeField] string m_priceCurrencyCode = string.Empty;
        [SerializeField] string m_originalJson = string.Empty;


        [SerializeField] string m_subscriptionPeriod = string.Empty;
        [SerializeField] string m_freeTrialPeriod = string.Empty;
        [SerializeField] string m_introductoryPrice = string.Empty;
        [SerializeField] string m_introductoryPriceAmountMicros = string.Empty;
        [SerializeField] string m_introductoryPricePeriod = string.Empty;
        [SerializeField] string m_introductoryPriceCycles = string.Empty;

        [SerializeField] Texture2D m_icon;
        [SerializeField] bool m_isConsumable = true;

        private bool m_isValid = false;


        public AN_Product(AN_SkuDetails skuDetails)
        {
            m_productId = skuDetails.Sku;
            m_price = skuDetails.Price;
            m_type = skuDetails.Type.ToString();
            m_description = skuDetails.Description;
            m_priceAmountMicros = skuDetails.PriceAmountMicros.ToString();
            m_priceCurrencyCode = skuDetails.PriceCurrencyCode;
            m_originalJson = skuDetails.OriginalJson;
            m_subscriptionPeriod = skuDetails.SubscriptionPeriod;
            m_freeTrialPeriod = skuDetails.FreeTrialPeriod;
            m_introductoryPrice = skuDetails.IntroductoryPrice;
            m_introductoryPriceAmountMicros = skuDetails.IntroductoryPriceAmountMicros.ToString();
            m_introductoryPricePeriod = skuDetails.IntroductoryPricePeriod;
            m_introductoryPriceCycles = skuDetails.IntroductoryPriceCycles;
            m_isConsumable = skuDetails.IsConsumable;

        }
        
        public AN_Product(string productId, AN_ProductType type) {
            m_productId = productId;
            m_type = type.ToString();
        }


        /// <summary>
        /// Internal use only.
        /// </summary>
        public void MarkAsValid() {
            m_isValid = true;
        }


        /// <summary>
        /// The string that identifies the product to the Google Play Store.
        /// </summary>
        public string ProductId {
            get {
                return m_productId;
            }

            set {
                m_productId = value;
            }

        }

        /// <summary>
        /// Type of the current product
        /// </summary>
        public AN_ProductType Type {
            get {
                return SA_EnumUtil.ParseEnum<AN_ProductType>(m_type);
            }

            set {
                m_type = value.ToString();
            }

        }


        /// <summary>
        /// Formatted price of the item, including its currency sign. The price does not include tax.
        /// </summary>
        public string Price {
            get {
                if(string.IsNullOrEmpty(m_price)) {
                    return "$0.00";
                }
                return m_price;
            }

            set {
                m_price = value;
            }
        }


        /// <summary>
        /// Title of the product.
        /// </summary>
        public string Title {
            get {
                return m_title;
            }

            set {
                m_title = value;
            }
        }


        /// <summary>
        /// Description of the product.
        /// </summary>
        public string Description {
            get {
                return m_description;
            }

            set {
                m_description = value; 
            }
        }

        /// <summary>
        /// Gets the price in micros.
        /// </summary>
        public long PriceAmountMicros {
            get {
                if (string.IsNullOrEmpty(m_priceAmountMicros)) {
                    return 0;
                }
                return Convert.ToInt64(m_priceAmountMicros);
            }
        }


        /// <summary>
        /// ISO 4217 currency code for price. 
        /// For example, if price is specified in British pounds sterling, price_currency_code is "GBP".
        /// </summary>
        public string PriceCurrencyCode {
            get {
                return m_priceCurrencyCode;
            }

        }

        /// <summary>
        /// Subscription period, specified in ISO 8601 format. 
        /// For example, P1W equates to one week, 
        /// P1M equates to one month, P3M equates to three months, 
        /// P6M equates to six months, and P1Y equates to one year.
        /// 
        /// Note: Returned only for subscriptions.
        /// </summary>
        public string SubscriptionPeriod {
            get {
                return m_subscriptionPeriod;
            }

        }

        /// <summary>
        /// Trial period configured in Google Play Console, specified in ISO 8601 format. 
        /// For example, P7D equates to seven days. To learn more about free trial eligibility.
        /// 
        /// Note: Returned only for subscriptions which have a trial period configured.
        /// </summary>
        public string FreeTrialPeriod {
            get {
                return m_freeTrialPeriod;
            }

        }

        /// <summary>
        /// Formatted introductory price of a subscription, including its currency sign, such as €3.99. 
        /// The price doesn't include tax.
        /// 
        /// Note: Returned only for subscriptions which have an introductory period configured.
        /// </summary>
        public string IntroductoryPrice {
            get {
                return m_introductoryPrice;
            }
        }

        /// <summary>
        /// Introductory price in micro-units. The currency is the same as <see cref="PriceCurrencyCode"/>.
        /// 
        /// Note: Returned only for subscriptions which have an introductory period configured.
        /// </summary>
        public string IntroductoryPriceAmountMicros {
            get {
                return m_introductoryPriceAmountMicros;
            }
        }

        /// <summary>
        /// The billing period of the introductory price, specified in ISO 8601 format.
        /// 
        /// Note: Returned only for subscriptions which have an introductory period configured.
        /// </summary>
        public string IntroductoryPricePeriod {
            get {
                return m_introductoryPricePeriod;
            }
        }


        /// <summary>
        /// The number of subscription billing periods for which the user will be given the introductory price, such as 3.
        /// 
        /// Note: Returned only for subscriptions which have an introductory period configured.
        /// </summary>
        public int IntroductoryPriceCycles {
            get {
                if(string.IsNullOrEmpty(m_introductoryPriceCycles)) {
                    return 0;
                }
                return Convert.ToInt32(m_introductoryPriceCycles);
            }

        }


        /// <summary>
        /// The origonal JSON responce sent by a google play server
        /// </summary>
        public string OriginalJson {
            get {
                return m_originalJson;
            }
        }

        public Texture2D Icon {
            get {
                return m_icon;
            }

            set {
                m_icon = value;
            }
        }


        /// <summary>
        /// Inidcates if product is valid for the purchase
        /// Can only be true for the product that was recived after connection
        /// to the Google Billing API
        /// </summary>
        public bool IsValid {
            get {
                return m_isValid;
            }
        }


        /// <summary>
        /// Inidcated if product can be consumed
        /// </summary>
        public bool IsConsumable {
            get {
                return m_isConsumable;
            }

            set {
                m_isConsumable = value;
            }
        }
    }
}
