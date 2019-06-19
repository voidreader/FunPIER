using UnityEngine;
using System;
using System.Collections;
using SA.Android.Vending.Billing;
using SA.Android.Utilities;

using SA.Foundation.Tests;

namespace SA.Android.Tests.Billing
{
    public class AN_ConnectBillingTest : SA_BaseTest {

       

        public override void Test() {

            AN_Billing.Connect((result) => {
                AN_Logger.Log("Connect result.IsSucceeded: " + result.IsSucceeded);
                AN_Logger.Log("Connect result.IsInAppsAPIAvalible: " + result.IsInAppsAPIAvalible);
                AN_Logger.Log("Connect result.IsSubsAPIAvalible: " + result.IsSubsAPIAvalible);

                if (result.IsSucceeded) {
                    AN_Logger.Log("AN_Billing.Inventory.Purchases.Count: " + AN_Billing.Inventory.Purchases.Count);
                    AN_Logger.Log("AN_Billing.Inventory.Products.Count: " + AN_Billing.Inventory.Products.Count);

                    //Let's print all purchases info
                    foreach (AN_Purchase purchase in AN_Billing.Inventory.Purchases) {
                        AN_Logger.Log("purchase.OrderId " + purchase.OrderId);
                        AN_Logger.Log("purchase.ProductId " + purchase.ProductId);
                        AN_Logger.Log("purchase.PackageName " + purchase.PackageName);
                        AN_Logger.Log("purchase.PurchaseState " + purchase.PurchaseState);
                        AN_Logger.Log("purchase.PurchaseTime " + purchase.PurchaseTime);
                        AN_Logger.Log("purchase.Signature " + purchase.Signature);
                        AN_Logger.Log("purchase.Token " + purchase.Token);
                        AN_Logger.Log("purchase.Type " + purchase.Type);
                        AN_Logger.Log("purchase.DeveloperPayload " + purchase.DeveloperPayload);
                        AN_Logger.Log("purchase.AutoRenewing " + purchase.AutoRenewing);
                        AN_Logger.Log("purchase.OriginalJson " + purchase.OriginalJson);
                        AN_Logger.Log("----------------------------------------------------");
                    }

                    //And products info as well
                    foreach (AN_Product product in AN_Billing.Inventory.Products) {
                        AN_Logger.Log("product.ProductId " + product.ProductId);
                        AN_Logger.Log("product.Type " + product.Type);
                        AN_Logger.Log("product.Price " + product.Price);
                        AN_Logger.Log("product.Title " + product.Title);
                        AN_Logger.Log("product.Description " + product.Description);
                        AN_Logger.Log("product.PriceAmountMicros " + product.PriceAmountMicros);
                        AN_Logger.Log("product.PriceCurrencyCode " + product.PriceCurrencyCode);
                        AN_Logger.Log("product.SubscriptionPeriod " + product.SubscriptionPeriod);
                        AN_Logger.Log("product.FreeTrialPeriod " + product.FreeTrialPeriod);
                        AN_Logger.Log("product.SubscriptionPeriod " + product.SubscriptionPeriod);
                        AN_Logger.Log("product.FreeTrialPeriod " + product.FreeTrialPeriod);
                        AN_Logger.Log("product.IntroductoryPrice " + product.IntroductoryPrice);
                        AN_Logger.Log("product.IntroductoryPriceAmountMicros " + product.IntroductoryPriceAmountMicros);
                        AN_Logger.Log("product.IntroductoryPricePeriod " + product.IntroductoryPricePeriod);
                        AN_Logger.Log("product.IntroductoryPriceCycles " + product.IntroductoryPriceCycles);
                        AN_Logger.Log("product.OriginalJson " + product.OriginalJson);
                        AN_Logger.Log("----------------------------------------------------");

                    }
                }

                SetAPIResult(result);
            });

        }

    }
}
