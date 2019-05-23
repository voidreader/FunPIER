using System;

using SA.Android.Vending.Billing;
using SA.Foundation.Templates;
using SA.Android.Vending.Licensing;
using SA.Foundation.Async;

namespace SA.Android.Vending.Internal
{

    internal class AnIVendingEditorApi : AN_IVendingAPI
    {


        public void Connect(AN_VendingLib.AN_ConnectionRequest request, Action<AN_BillingConnectionResult> callback) {

            SA_Coroutine.WaitForSeconds(1, () => {
                callback.Invoke(new AN_BillingConnectionResult());
            });

        }

        public void Purchase(AN_VendingLib.AN_PurchaseRequest request, Action<AN_BillingPurchaseResult> callback) {

            SA_Coroutine.WaitForSeconds(1, () => {
                AN_Purchase purchase  =new AN_Purchase(request.m_product.ProductId, request.m_product.Type.ToString(), request.m_developerPayload);
                callback.Invoke(new AN_BillingPurchaseResult(purchase));
            });
        }

        public void PurchaseSubscriptionReplace(AN_VendingLib.AN_PurchaseSubscriptionReplaceRequest request,
            Action<AN_BillingPurchaseResult> callback) {

            SA_Coroutine.WaitForSeconds(1, () => {
                AN_Purchase purchase = new AN_Purchase(request.m_newProductId, AN_ProductType.subs.ToString(),
                    request.m_developerPayload);
                callback.Invoke(new AN_BillingPurchaseResult(purchase));
            });
        }

        public void Consume(AN_Purchase purchase, Action<SA_Result> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                callback.Invoke(new SA_Result());
            });
        }

        public void GetPurchases(AN_VendingLib.AN_ConnectionRequest request, Action<AN_InventoryResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                callback.Invoke(new AN_InventoryResult());
            });
        }

        public void CheckAccess(string base64EncodedPublicKey, Action<AN_LicenseResult> callback) {
            SA_Coroutine.WaitForSeconds(1, () => {
                callback.Invoke(new AN_LicenseResult(AN_Policy.LICENSED));
            });
        }
    }
}