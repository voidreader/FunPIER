using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;
using SA.Android.Vending.Internal;

namespace SA.Android.Vending.Billing
{

    /// <summary>
    /// Communicates with the Billing service
    /// and presents a user interface so that the user can authorize payment. 
    /// </summary>
    public static class AN_Billing 
    {

        public const int RESULT_OK = 0;

        private static AN_Inventory s_Inventory;
        private static AN_BillingConnectionResult m_SuccessInitResultCache;
        private static bool s_IsConnectionInProgress;

        private static event Action<AN_BillingConnectionResult> s_OnConnect = delegate{};
        private static Action<SA_Result> s_GetPurchasesRequestCallback;
            


        //--------------------------------------
        //  Public Methods
        //--------------------------------------


        /// <summary>
        /// Connecting to Google Play Billing service. 
        /// Once the connection is successfully established, 
        /// <see cref="GetPurchases(Action{SA_Result})"/> method will be called automatically.
        /// Which means the <see cref="Inventory"/> will be filled and available after method callback
        /// </summary>
        /// <param name="callback">The Connection result callback</param>
        public static void Connect(Action<AN_BillingConnectionResult> callback) {


            if (m_SuccessInitResultCache != null) {
                callback.Invoke(m_SuccessInitResultCache);
                return;
            }

            s_OnConnect += callback;
            if (s_IsConnectionInProgress) { return; }


            s_IsConnectionInProgress = true;
            var request = GetConnectionRequest();


            AN_VendingLib.API.Connect(request, connectionResult => {
                s_IsConnectionInProgress = false;
                if (connectionResult.IsSucceeded) {
                    GetPurchases((invResult) => {
                        if(invResult.IsSucceeded) {
                            SaveConnectionResult(connectionResult);
                        } else {
                            connectionResult.SetError(invResult.Error);
                        }
                        s_OnConnect.Invoke(connectionResult);
                        s_OnConnect = delegate { };
                    });
                   
                } else {
                    s_OnConnect.Invoke(connectionResult);
                    s_OnConnect = delegate { };
                } 
            });
        }


        /// <summary>
        /// This method will fill up the current un-consumed products owned by the user, 
        /// including both purchased items and items acquired by redeeming a promo code. '
        /// The products details will also be update.
        /// Information about products and purchases is available via <see cref="Inventory"/>
        /// 
        /// Note: during service connection flow method is getting called automatically.
        /// </summary>
        public static void GetPurchases(Action<SA_Result> callback) {

            if(s_GetPurchasesRequestCallback == null) {
                s_GetPurchasesRequestCallback = callback;
            } else {
                //we already waiting for response
                s_GetPurchasesRequestCallback += callback;
                return;
            }

            var request = GetConnectionRequest();
            AN_VendingLib.API.GetPurchases(request, (result) => {
                if(result.IsSucceeded) {
                    UpdateInventory(result.Inventory);
                }
                s_GetPurchasesRequestCallback.Invoke(result);
                s_GetPurchasesRequestCallback = null;
            });
        }

        /// <summary>
        /// This method begins a purchase request.
        /// </summary>
        /// <param name="product">product which you must specify in the application's product list on the Google Play Console.</param>
        /// <param name="callback">Purchase request callback</param>
        public static void Purchase(AN_Product product, Action<AN_BillingPurchaseResult> callback) {
            Purchase(product, string.Empty, callback);
        }

        /// <summary>
        /// This method begins a purchase request.
        /// </summary>
        /// <param name="product">product which you must specify in the application's product list on the Google Play Console.</param>
        /// <param name="developerPayload">
        /// A developer-specified string that contains supplemental information about an order. 
        /// You can specify a value for this field when you make a Purchase request.
        /// </param>
        /// <param name="callback">Purchase request callback</param>
        public static void Purchase(AN_Product product, string developerPayload, Action<AN_BillingPurchaseResult> callback) {

            var request = new AN_VendingLib.AN_PurchaseRequest();
            request.m_product = product;
            request.m_developerPayload = developerPayload;

            AN_VendingLib.API.Purchase(request, (result) => {

                if(result.IsSucceeded) {
                    Inventory.Purchases.Add(result.Purchase);
                } else {
                    //me might try to resolve some know response codes
                    switch (result.Error.Code) {
                        case (int)AN_BillingRessponceCodes.BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED:
                            AN_Purchase purchase = Inventory.GetPurchaseByProductId(product.ProductId);
                            if(purchase != null) {
                                result = new AN_BillingPurchaseResult(purchase);
                                callback.Invoke(result);
                            } else {
                                GetPurchases((purchasesLoadResult) => {
                                    purchase = Inventory.GetPurchaseByProductId(product.ProductId);
                                    if(purchase != null) {
                                        result = new AN_BillingPurchaseResult(purchase);
                                    }
                                    callback.Invoke(result);
                                });

                            }
                            return;
                    }
                   
                }

                callback.Invoke(result);
            });
        }
        
        /// <summary>
        /// This method is used to upgrade or downgrade a subscription purchase.
        /// The method is similar to <see cref="Purchase(SA.Android.Vending.Billing.AN_Product,System.Action{SA.Android.Vending.Billing.AN_BillingPurchaseResult})"/>,
        /// except that it takes a list with exactly one already-purchased SKU to be replaced with the SKU being purchased.
        /// When the user completes the purchase,
        /// Google Play swaps out the old SKU and credits the user with the unused value of their subscription time on a pro-rated basis.
        /// Google Play applies this credit to the new subscription,
        /// and does not begin billing the user for the new subscription until after the credit is used up.
        /// </summary>
        /// <param name="oldProductsId">Already-purchased SKU to be replaced.</param>
        /// <param name="newProductId">SKU being purchased</param>
        /// <param name="developerPayload">
        /// A developer-specified string that contains supplemental information about an order. 
        /// You can specify a value for this field when you make a Purchase request.
        /// </param>
        /// <param name="callback">Purchase request callback</param>
        public static void PurchaseSubscriptionReplace(List<string> oldProductsId, string newProductId, string developerPayload, Action<AN_BillingPurchaseResult> callback) {

            var request = new AN_VendingLib.AN_PurchaseSubscriptionReplaceRequest();
            request.m_oldProductsId = oldProductsId;
            request.m_newProductId = newProductId;
            request.m_developerPayload = developerPayload;

            AN_VendingLib.API.PurchaseSubscriptionReplace(request, (result) => {
                if(result.IsSucceeded) {
                    Inventory.Purchases.Add(result.Purchase);
                }
                callback.Invoke(result);
            });
        }

        /// <summary>
        /// Consume purchase corresponding to the purchase token. 
        /// This will result in this item being removed from all 
        /// subsequent responses to <see cref="GetPurchases(Action{SA_Result})"/> and allow repurchase of items of the same sku.
        /// </summary>
        /// <param name="purchase">purchase to consume</param>
        /// <param name="callback">Consume request callback</param>
        public static void Consume(AN_Purchase purchase, Action<SA_Result> callback) {


            var product = Inventory.GetProductById(purchase.ProductId);
            if(product != null) {
               if(!product.IsConsumable) {
                    Debug.LogWarning("You are trying to consume product with id: " + product.ProductId + " that was originally marked as non consumable product");
               }
            } else {
                Debug.LogWarning("product " + purchase.ProductId + " isn't the registered inventory product");
            }

            AN_VendingLib.API.Consume(purchase, (result) => {
                if(result.IsSucceeded) {
                    foreach (var invPurchase in Inventory.Purchases) {
                        if (invPurchase.ProductId.Equals(purchase.ProductId)) {
                            Inventory.Purchases.Remove(invPurchase);
                            break;
                        }
                    }
                }
                callback.Invoke(result);
            });

        }

        //--------------------------------------
        //  Get / Set
        //--------------------------------------


        /// <summary>
        /// Returns the <see cref="AN_Inventory"/> object which contains information about available products
        /// in the Google Play store and the information about purchases.
        /// 
        /// The purchases information will be empty until service is connected. Use the <see cref="Connect"/> method
        /// to connect to a Google Billing service. 
        /// If you try to query inventory products before service is connected it will contain  products you
        /// specified with the plugin editor settings.
        /// 
        /// Once service is connected products information will be update according to a billing service response.
        /// </summary>
        /// <value>The inventory.</value>
        public static AN_Inventory Inventory {
            get {
                if(s_Inventory == null) {
                    s_Inventory = new AN_Inventory();
                    s_Inventory.SetProducts(AN_Settings.Instance.InAppProducts);
                }
                return s_Inventory;
            }
        }


        //--------------------------------------
        //  Private Methods
        //--------------------------------------



        private static void SaveConnectionResult(AN_BillingConnectionResult result) {
            m_SuccessInitResultCache = result;
        }

        private static void UpdateInventory(AN_Inventory inventory) {
            Inventory.SetPurchases(inventory.Purchases);
            foreach (var product in inventory.Products) {
                if (Inventory.HasProductWithId(product.ProductId)) {
                    var inventoryProduct = Inventory.GetProductById(product.ProductId);
                    string json = JsonUtility.ToJson(product);


                    //We need to save consumable state since, it was set manually 
                    //and product from google server will not have it
                    bool isConsumable = inventoryProduct.IsConsumable;
                    JsonUtility.FromJsonOverwrite(json, inventoryProduct);
                    inventoryProduct.MarkAsValid();
                    inventoryProduct.IsConsumable = isConsumable;
                }
            }
        }


        private static AN_VendingLib.AN_ConnectionRequest GetConnectionRequest() {
            var request = new AN_VendingLib.AN_ConnectionRequest();
            foreach (var product in AN_Settings.Instance.InAppProducts) {

                switch (product.Type) {
                    case AN_ProductType.inapp:
                        request.m_inapp.Add(product.ProductId);
                        break;
                    case AN_ProductType.subs:
                        request.m_subs.Add(product.ProductId);
                        break;
                    case AN_ProductType.undefined:
                        request.m_inapp.Add(product.ProductId);
                        break;
                }
            }

            request.m_base64EncodedPublicKey = AN_Settings.Instance.RSAPublicKey;

            return request;
        }


    }

}
