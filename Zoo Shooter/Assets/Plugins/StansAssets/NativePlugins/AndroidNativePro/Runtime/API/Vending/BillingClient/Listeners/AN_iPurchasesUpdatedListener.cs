using System.Collections.Generic;
using SA.Android.Vending.Billing;
using SA.Foundation.Templates;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Listener interface for purchase updates which happen when,
    /// for example, the user buys something within the app or by initiating a purchase from Google Play Store.
    /// </summary>
    public interface AN_iPurchasesUpdatedListener
    {
        /// <summary>
        /// Implement this method to get notifications for purchases updates.
        /// Both purchases initiated by your app and the ones initiated outside of your app will be reported here.
        ///
        /// Warning! All purchases reported here must either be consumed or acknowledged.
        /// Failure to either consume or acknowledge a purchase will result in that purchase being refunded. Please refer to
        /// https://developer.android.com/google/play/billing/billing_library_overview#acknowledge for more details.
        /// </summary>
        /// <param name="billingResult">BillingResult of the update.</param>
        /// <param name="purchases">List of updated purchases if present.</param>
        void onPurchasesUpdated(SA_iResult billingResult, List<AN_Purchase> purchases);
    }
}