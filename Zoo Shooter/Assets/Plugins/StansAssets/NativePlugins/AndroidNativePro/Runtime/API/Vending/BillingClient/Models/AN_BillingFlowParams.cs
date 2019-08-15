using System;
using SA.Android.Vending.Billing;
using UnityEngine;

namespace SA.Android.Vending.BillingClient
{
    [Serializable]
    public class AN_BillingFlowParams
    {
        public enum ProrationMode
        {
            /// <summary>
            /// Unknown subscription upgrade / downgrade policy.
            /// </summary>
            Unknown = 0,
            
            /// <summary>
            /// Replacement takes effect immediately,
            /// and the remaining time will be prorated and credited to the user. This is the current default behavior.
            /// </summary>
            ImmediateAndChangeProratedPrice = 1,
            
            /// <summary>
            /// Replacement takes effect immediately, and the billing cycle remains the same.
            /// The price for the remaining period will be charged. This option is only available for subscription upgrade.
            /// </summary>
            ImmediateWithTimeProration = 2,
            
            /// <summary>
            /// Replacement takes effect immediately, and the new price will be charged on next recurrence time. The billing cycle stays the same.
            /// </summary>
            ImmediateWithoutProration = 3,
            
            /// <summary>
            /// Replacement takes effect when the old plan expires,
            /// and the new price will be charged at the same time.
            /// </summary>
            Deferred = 4
        }
        
        /// <summary>
        /// Helps to construct <see cref="AN_BillingFlowParams"/> that are used to initiate a purchase flow.
        /// </summary>
        [Serializable]
        public class Builder
        {
#pragma warning disable 414
            [SerializeField] private string m_OldSku = string.Empty;
            [SerializeField] private string m_AccountId = string.Empty;
            [SerializeField] private string m_DeveloperId = string.Empty;
            [SerializeField] private int m_ReplaceSkusProrationMode = 0;
            [SerializeField] private bool m_IsVrPurchaseFlow = false;
            [SerializeField] private int m_SkuNativeHashId = 0;
#pragma warning restore 414
            
            internal Builder() { } 
            
            /// <summary>
            /// Returns <see cref="AN_BillingFlowParams"/> reference to initiate a purchase flow.
            /// </summary>
            /// <returns><see cref="AN_BillingFlowParams"/> reference to initiate a purchase flow.</returns>
            public AN_BillingFlowParams Build()
            {
               return  new AN_BillingFlowParams(this);
            }

            /// <summary>
            /// Specify an optional obfuscated string that is uniquely associated with the user's account in your app.
            ///
            /// If you pass this value, Google Play can use it to detect irregular activity,
            /// such as many devices making purchases on the same account in a short period of time.
            /// Do not use the developer ID or the user's Google ID for this field.
            /// In addition, this field should not contain the user's ID in cleartext.
            /// We recommend that you use a one-way hash to generate a string from the user's ID
            /// and store the hashed string in this field.
            /// 
            /// Optional:
            ///  * To buy in-app item
            ///  * To create a new subscription
            ///  * To replace an old subscription
            /// </summary>
            /// <param name="accountId">Account id</param>
            public void SetAccountId(string accountId)
            {
                m_AccountId = accountId;
            }

            /// <summary>
            /// Specify an optional obfuscated string of developer profile name.
            ///
            /// If you pass this value, Google Play can use it for payment risk evaluation.
            /// Do not use the account ID or the user's Google ID for this field.
            ///
            /// Optional:
            ///  * To buy in-app item
            ///  * To create a new subscription
            ///  * To replace an old subscription
            /// </summary>
            /// <param name="developerId"></param>
            public void SetDeveloperId(string developerId)
            {
                m_DeveloperId = developerId;
            }

            /// <summary>
            /// Specify the SKU that the user is upgrading or downgrading from.
            ///
            /// Mandatory:
            ///  * To replace an old subscription
            /// </summary>
            /// <param name="oldSku"></param>
            public void SetOldSku(string oldSku)
            {
                m_OldSku = oldSku;
            }

            /// <summary>
            /// Specifies the mode of proration during subscription upgrade/downgrade.
            /// This value will only be effective if oldSkus is set.
            ///
            /// If you set this to NO_PRORATION,
            /// the user does not receive credit or charge, and the recurrence date does not change.
            ///
            /// If you set this to PRORATE_BY_TIME,
            /// Google Play swaps out the old SKUs and credits the user with the unused value of their subscription time
            /// on a pro-rated basis. Google Play applies this credit to the new subscription,
            /// and does not begin billing the user for the new subscription until after the credit is used up.
            ///
            /// If you set this to PRORATE_BY_PRICE, Google Play swaps out the old SKUs and keeps the recurrence date not changed.
            /// User will be charged for the price differences to cover the time till next recurrence date.
            ///
            /// Optional:
            ///  * To buy in-app item
            ///  * To create a new subscription
            ///  * To replace an old subscription
            /// </summary>
            /// <param name="replaceSkusProrationMode">Proration Mode.</param>
            public void SetReplaceSkusProrationMode(ProrationMode replaceSkusProrationMode)
            {
                m_ReplaceSkusProrationMode = (int) replaceSkusProrationMode;
            }

            /// <summary>
            /// Specify the <see cref="AN_SkuDetails"/> SkuDetails of the item being purchase.
            /// 
            /// Mandatory:
            ///  * To buy in-app item
            ///  * To create a new subscription
            ///  * To replace an old subscription
            /// </summary>
            /// <param name="skuDetails">Sku Details.</param>
            public void SetSkuDetails(AN_SkuDetails skuDetails)
            {
                m_SkuNativeHashId = skuDetails.NativeHashId;
            }

            /// <summary>
            /// Specify an optional flag indicating whether you wish to launch a VR purchase flow.
            ///
            /// Optional:
            ///  * To buy in-app item
            ///  * To create a new subscription
            ///  * To replace an old subscription
            /// </summary>
            /// <param name="isVrPurchaseFlow">isVrPurchaseFlow.</param>
            public void SetVrPurchaseFlow(bool isVrPurchaseFlow)
            {
                m_IsVrPurchaseFlow = isVrPurchaseFlow;
            }
             
        }
        
#pragma warning disable 414
        [SerializeField] private Builder m_Builder;
#pragma warning restore 414

        private AN_BillingFlowParams(Builder builder)
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

    }
}