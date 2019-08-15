using SA.Foundation.Templates;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Listener to a result of load reward request
    /// </summary>
    public interface AN_iRewardResponseListener
    {
        /// <summary>
        /// Called to notify that a load reward operation has finished.
        /// </summary>
        /// <param name="billingResult"></param>
        void OnSkuDetailsResponse(SA_Result billingResult);
    }
}