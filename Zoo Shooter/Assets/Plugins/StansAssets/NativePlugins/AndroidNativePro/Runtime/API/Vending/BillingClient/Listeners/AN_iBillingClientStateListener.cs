using SA.Foundation.Templates;

namespace SA.Android.Vending.BillingClient
{
    /// <summary>
    /// Callback for setup process.
    /// This listener's <see cref="OnBillingSetupFinished"/>
    /// method is called when the setup process is complete.
    /// </summary>
    public interface AN_iBillingClientStateListener
    {
        /// <summary>
        /// Called to notify that setup is complete.
        /// </summary>
        /// <param name="billingResult"></param>
        void OnBillingSetupFinished(SA_iResult billingResult);
        
        /// <summary>
        /// Called to notify that connection to billing service was lost.
        ///
        /// Note: This does not remove billing service connection itself - this binding to the service will remain active,
        /// and you will receive a call to <see cref="OnBillingSetupFinished"/>
        /// when billing service is next running and setup is complete.
        /// </summary>
        void OnBillingServiceDisconnected();
    }
}