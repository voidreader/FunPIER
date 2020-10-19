using System.Collections.Generic;
using SA.Foundation.Templates;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Listener to a result of purchases history query.
    /// </summary>
    public interface AN_iPurchaseHistoryResponseListener
    {
        /// <summary>
        /// Called to notify that purchase history fetch operation has finished.
        /// </summary>
        /// <param name="billingResult">BillingResult of the query.</param>
        /// <param name="purchaseHistoryRecordList">
        /// List of purchase records (even if that purchase is expired, canceled, or consumed - up to 1 per each SKU)
        /// or null with corresponding <see cref="SA_iResult"/> responseCode if purchase history was not queried successfully.
        /// </param>
        void OnConsumeResponse(SA_iResult billingResult, List<AN_PurchaseHistoryRecord> purchaseHistoryRecordList);
    }
}
