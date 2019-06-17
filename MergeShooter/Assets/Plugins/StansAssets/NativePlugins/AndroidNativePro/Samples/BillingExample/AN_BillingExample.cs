using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using SA.Foundation.Templates;


using SA.Android;
using SA.Android.Utilities;
using SA.Android.Vending.Billing;
using SA.Android.Vending.Licensing;



public class AN_BillingExample : MonoBehaviour 
{
#pragma warning disable 649
    [SerializeField] Button m_checkBillingAvalibility = null;

    [SerializeField] Button m_connectButton = null;
    [SerializeField] Button m_consumeButton = null;
    [SerializeField] Button m_purchaseButton = null;

    [SerializeField] Button m_checkLicenseButton;
#pragma warning restore 649

    void Start () {
        m_checkBillingAvalibility.onClick.AddListener(() => {

          
        });

        m_consumeButton.onClick.AddListener(() => {
            AN_Billing.Consume(AN_Billing.Inventory.Purchases[0], (SA_Result result) =>  {
                AN_Logger.Log("Consume result.IsSucceeded: " + result.IsSucceeded);
            });
        });

        m_purchaseButton.onClick.AddListener(() => {
            AN_Product product = new AN_Product("android.test.purchased", AN_ProductType.inapp);
            AN_Billing.Purchase(product, (result) => {
                AN_Logger.Log("Unity Purchase result.IsSucceeded: " + result.IsSucceeded);
                if(result.IsSucceeded) {
                    var purchase = result.Purchase;
                    AN_Logger.Log("purchase.OrderId: " + purchase.OrderId);
                    AN_Logger.Log("purchase.ProductId: " + purchase.ProductId);
                    AN_Logger.Log("purchase.PackageName: " + purchase.PackageName);
                    AN_Logger.Log("purchase.PurchaseState: " + purchase.PurchaseState);
                    AN_Logger.Log("purchase.PurchaseTime: " + purchase.PurchaseTime);
                    AN_Logger.Log("purchase.Signature: " + purchase.Signature);
                    AN_Logger.Log("purchase.Token: " + purchase.Token);
                    AN_Logger.Log("purchase.Type: " + purchase.Type);
                    AN_Logger.Log("purchase.DeveloperPayload: " + purchase.DeveloperPayload);
                    AN_Logger.Log("purchase.AutoRenewing: " + purchase.AutoRenewing);
                    AN_Logger.Log("purchase.OriginalJson: " + purchase.OriginalJson);
                    AN_Logger.Log("----------------------------------------------------");
                } else {
                    AN_Logger.Log("Unity  Purchase failed: " + result.Error.FullMessage);
                }
            });
        });

        m_connectButton.onClick.AddListener(() => {

            AN_Billing.Connect((result) => {
                AN_Logger.Log("Connect result.IsSucceeded: " + result.IsSucceeded);
                AN_Logger.Log("Connect result.IsInAppsAPIAvalible: " + result.IsInAppsAPIAvalible);
                AN_Logger.Log("Connect result.IsSubsAPIAvalible: " + result.IsSubsAPIAvalible);

                if (result.IsSucceeded) {
                    AN_Logger.Log("AN_Billing.Inventory.Purchases.Count: " + AN_Billing.Inventory.Purchases.Count);
                    AN_Logger.Log("AN_Billing.Inventory.Products.Count: " + AN_Billing.Inventory.Products.Count);

                    //Let's print all purchases info
                    foreach(AN_Purchase purchase in AN_Billing.Inventory.Purchases) {
                        AN_Logger.Log("purchase.OrderId: " + purchase.OrderId);
                        AN_Logger.Log("purchase.ProductId: " + purchase.ProductId);
                        AN_Logger.Log("purchase.PackageName: " + purchase.PackageName);
                        AN_Logger.Log("purchase.PurchaseState: " + purchase.PurchaseState);
                        AN_Logger.Log("purchase.PurchaseTime: " + purchase.PurchaseTime);
                        AN_Logger.Log("purchase.Signature: " + purchase.Signature);
                        AN_Logger.Log("purchase.Token: " + purchase.Token);
                        AN_Logger.Log("purchase.Type: " + purchase.Type);
                        AN_Logger.Log("purchase.DeveloperPayload: " + purchase.DeveloperPayload);
                        AN_Logger.Log("purchase.AutoRenewing: " + purchase.AutoRenewing);
                        AN_Logger.Log("purchase.OriginalJson: " + purchase.OriginalJson);
                        AN_Logger.Log("----------------------------------------------------");
                    }

                    //And products info as well
                    foreach (AN_Product product in AN_Billing.Inventory.Products) {
                        AN_Logger.Log("product.ProductId: " + product.ProductId);
                        AN_Logger.Log("product.Type: " + product.Type);
                        AN_Logger.Log("product.Price: " + product.Price);
                        AN_Logger.Log("product.Title: " + product.Title);
                        AN_Logger.Log("product.Description: " + product.Description);
                        AN_Logger.Log("product.PriceAmountMicros: " + product.PriceAmountMicros);
                        AN_Logger.Log("product.PriceCurrencyCode: " + product.PriceCurrencyCode);
                        AN_Logger.Log("product.SubscriptionPeriod: " + product.SubscriptionPeriod);
                        AN_Logger.Log("product.FreeTrialPeriod: " + product.FreeTrialPeriod);
                        AN_Logger.Log("product.SubscriptionPeriod: " + product.SubscriptionPeriod);
                        AN_Logger.Log("product.FreeTrialPeriod: " + product.FreeTrialPeriod);
                        AN_Logger.Log("product.IntroductoryPrice: " + product.IntroductoryPrice);
                        AN_Logger.Log("product.IntroductoryPriceAmountMicros: " + product.IntroductoryPriceAmountMicros);
                        AN_Logger.Log("product.IntroductoryPricePeriod: " + product.IntroductoryPricePeriod);
                        AN_Logger.Log("product.IntroductoryPriceCycles: " + product.IntroductoryPriceCycles);
                        AN_Logger.Log("product.IsValid: " + product.IsValid);
                        AN_Logger.Log("product.OriginalJson: " + product.OriginalJson);
                        AN_Logger.Log("----------------------------------------------------");

                    }
                } else {
                    AN_Logger.Log("Billing service connection failed: " + result.Error.FullMessage);
                }
            });
        });

        m_checkLicenseButton.onClick.AddListener(() => {
            AN_LicenseChecker.CheckAccess((result) => {
                switch(result.PolicyCode) {
                    case AN_Policy.LICENSED:
                        AN_Logger.Log("AN_LicenseChecker: LICENSED");
                        break;
                    case AN_Policy.NOT_LICENSED:
                        AN_Logger.Log("AN_LicenseChecker: NOT_LICENSED");
                        break;
                    case AN_Policy.RETRY:
                        AN_Logger.Log("AN_LicenseChecker: RETRY");
                        break;

                }

                if(result.IsFailed) {
                    Debug.Log("AN_LicenseChecker error: " + result.ErrorCode.ToString());
                }
            });
        });


    }


}
