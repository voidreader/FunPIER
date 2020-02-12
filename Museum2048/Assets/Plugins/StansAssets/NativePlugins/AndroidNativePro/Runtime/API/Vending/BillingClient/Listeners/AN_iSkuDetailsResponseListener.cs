using System.Collections.Generic;
using SA.Android.Vending.Billing;
using SA.Foundation.Templates;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Listener to a result of SKU details query
    /// </summary>
    public interface AN_iSkuDetailsResponseListener
    {
        /// <summary>
        /// Called to notify that a fetch SKU details operation has finished.
        /// </summary>
        /// <param name="billingResult">BillingResult of the update.</param>
        /// <param name="skuDetailsList">List of SKU details.</param>
        void OnSkuDetailsResponse(SA_Result billingResult, List<AN_SkuDetails> skuDetailsList);
    }
}