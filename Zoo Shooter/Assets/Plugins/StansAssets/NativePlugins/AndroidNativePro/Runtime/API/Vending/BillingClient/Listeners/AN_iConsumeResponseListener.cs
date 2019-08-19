using SA.Foundation.Templates;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Callback that notifies when a consumption operation finishes.
    /// </summary>
    public interface AN_iConsumeResponseListener
    {
        /// <summary>
        /// Called to notify that a consume operation has finished.
        /// </summary>
        /// <param name="billingResult">BillingResult, the result of the consume operation.</param>
        /// <param name="purchaseToken">The purchase token that was (or was to be) consumed.</param>
        void OnConsumeResponse(SA_iResult billingResult, string purchaseToken);
    }
}