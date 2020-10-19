using SA.Foundation.Templates;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Listener to a result of acknowledge purchase request
    /// </summary>
    public interface AN_iAcknowledgePurchaseResponseListener
    {
        /// <summary>
        /// Called to notify that an acknowledge purchase operation has finished.
        /// </summary>
        /// <param name="billingResult">BillingResult of the update.</param>
        void onAcknowledgePurchaseResponse(SA_iResult billingResult);
    }
}
