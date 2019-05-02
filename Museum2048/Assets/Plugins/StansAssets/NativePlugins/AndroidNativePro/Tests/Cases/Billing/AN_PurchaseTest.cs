using UnityEngine;
using System.Collections;

using SA.Android.Utilities;
using SA.Android.Vending.Billing;

using SA.Foundation.Tests;

namespace SA.Android.Tests.Billing
{
    public class AN_PurchaseTest : SA_BaseTest {

        public override bool RequireUserInteraction { get { return true; } }

        public override void Test() {
            AN_Product product = new AN_Product("android.test.purchased", AN_ProductType.inapp);
            AN_Billing.Purchase(product, (result) => {
                AN_Logger.Log("Purchase result.IsSucceeded: " + result.IsSucceeded);
                if (result.IsSucceeded) {
                    var purchase = result.Purchase;
                    PrintPurchaseInfo(purchase);
                }


                AN_Logger.Log("Now print it from invent");
                foreach(var purchase in AN_Billing.Inventory.Purchases) {
                    PrintPurchaseInfo(purchase);
                }

                SetAPIResult(result);
            });

        }


        private void PrintPurchaseInfo(AN_Purchase purchase) {
            AN_Logger.Log("purchase.OrderId" + purchase.OrderId);
            AN_Logger.Log("purchase.ProductId" + purchase.ProductId);
            AN_Logger.Log("purchase.PackageName" + purchase.PackageName);
            AN_Logger.Log("purchase.PurchaseState" + purchase.PurchaseState);
            AN_Logger.Log("purchase.PurchaseTime" + purchase.PurchaseTime);
            AN_Logger.Log("purchase.Signature" + purchase.Signature);
            AN_Logger.Log("purchase.Token" + purchase.Token);
            AN_Logger.Log("purchase.Type" + purchase.Type);
            AN_Logger.Log("purchase.DeveloperPayload" + purchase.DeveloperPayload);
            AN_Logger.Log("purchase.AutoRenewing" + purchase.AutoRenewing);
            AN_Logger.Log("purchase.OriginalJson" + purchase.OriginalJson);
            AN_Logger.Log("----------------------------------------------------");
        }
    }
}
