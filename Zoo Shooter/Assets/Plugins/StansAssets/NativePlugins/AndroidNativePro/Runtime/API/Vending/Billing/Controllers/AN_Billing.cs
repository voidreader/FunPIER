using System;
using System.Collections.Generic;
using SA.Android.Vending.BillingClient;
using UnityEngine;

using SA.Foundation.Templates;

namespace SA.Android.Vending.Billing
{

    /// <summary>
    /// Communicates with the Billing service
    /// and presents a user interface so that the user can authorize payment. 
    /// </summary>
    [Obsolete("Use AN_BillingClient API instead")]
    public static class AN_Billing 
    {
        internal class InternalBillingClient : AN_iPurchasesUpdatedListener, AN_iBillingClientStateListener, AN_iSkuDetailsResponseListener, AN_iConsumeResponseListener
        {
            //Billing Client
            private AN_BillingClient m_BillingClient;

            private Action<AN_BillingConnectionResult> m_ConnectionResultAction;
            private Action<AN_BillingPurchaseResult> m_BillingPurchaseCallback;
            private Action<List<AN_SkuDetails>> m_OnSkuDetailsCallback;
            private Action<SA_iResult> m_OnConsumeCallback;
            
            public void Connect(Action<AN_BillingConnectionResult> callback)
            {
                m_ConnectionResultAction = callback;
                using (var builder = AN_BillingClient.NewBuilder())
                {
                    builder.SetListener(this);
                    builder.EnablePendingPurchases();
                    builder.SetChildDirected(AN_BillingClient.ChildDirected.Unspecified);
                    builder.SetUnderAgeOfConsent(AN_BillingClient.UnderAgeOfConsent.Unspecified);

                    m_BillingClient = builder.Build();
                    m_BillingClient.StartConnection(this);
                }
            }

            public void QuerySkuDetailsAsync(AN_SkuDetailsParams @params, Action<List<AN_SkuDetails>> callback)
            {
                m_OnSkuDetailsCallback = callback;
                m_BillingClient.QuerySkuDetailsAsync(@params, this);
                
            }

            public void Purchase(AN_SkuDetails skuDetails, string developerPayload, Action<AN_BillingPurchaseResult> callback)
            {
                m_BillingPurchaseCallback = callback;
                var paramsBuilder = AN_BillingFlowParams.NewBuilder();
                paramsBuilder.SetSkuDetails(skuDetails);
                        
                m_BillingClient.LaunchBillingFlow(paramsBuilder.Build());
            }

            public void Consume(string purchaseToken, Action<SA_iResult> callback)
            {
                m_OnConsumeCallback = callback;
                var paramsBuilder = AN_ConsumeParams.NewBuilder();
                paramsBuilder.SetPurchaseToken(purchaseToken);
                        
                m_BillingClient.ConsumeAsync(paramsBuilder.Build(), this);
            }
            
            public void onPurchasesUpdated(SA_iResult billingResult, List<BillingClient.AN_Purchase> purchases)
            {
                AN_BillingPurchaseResult result;
                if (billingResult.IsSucceeded)
                {
                    result = new AN_BillingPurchaseResult(new AN_Purchase(purchases[0]));
                }
                else
                {
                    result = new AN_BillingPurchaseResult(billingResult);
                }
                
                m_BillingPurchaseCallback.Invoke(result);
            }

            public void OnBillingSetupFinished(SA_iResult billingResult)
            {
                AN_BillingConnectionResult result = new AN_BillingConnectionResult(billingResult);
                m_ConnectionResultAction.Invoke(result);
            }

            public void OnBillingServiceDisconnected()
            {
                //Do nothing
            }

            public AN_BillingClient API
            {
                get { return m_BillingClient; }
            }

            public void OnSkuDetailsResponse(SA_Result billingResult, List<AN_SkuDetails> skuDetailsList)
            {
                m_OnSkuDetailsCallback.Invoke(skuDetailsList);
            }

            public void OnConsumeResponse(SA_iResult billingResult, string purchaseToken)
            {
                m_OnConsumeCallback.Invoke(billingResult);
            }
        }

         

        public const int RESULT_OK = 0;

        private static AN_Inventory s_Inventory;
        private static AN_BillingConnectionResult m_SuccessInitResultCache;
        private static bool s_IsConnectionInProgress;

        private static event Action<AN_BillingConnectionResult> s_OnConnect = delegate{};
        private static Action<SA_Result> s_GetPurchasesRequestCallback;

        private static readonly InternalBillingClient m_Client = new InternalBillingClient();

        private static List<AN_SkuDetails> m_LoadedSkus = new List<AN_SkuDetails>();
            


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
            
            m_Client.Connect(connectionResult =>
            {
                if (connectionResult.IsSucceeded)
                {
                    var skusList = new List<string>();
                    foreach (var product in AN_Settings.Instance.InAppProducts)
                    {
                        if (product.Type == AN_BillingClient.SkuType.inapp)
                        {
                            skusList.Add(product.Sku);
                        }
                    }
                    
                    var paramsBuilder = AN_SkuDetailsParams.NewBuilder();
                    paramsBuilder.SetType(AN_BillingClient.SkuType.inapp);
                    paramsBuilder.SetSkusList(skusList);
                    
                    m_Client.QuerySkuDetailsAsync(paramsBuilder.Build(), (skus) =>
                    {
                        if (skus != null)
                        {
                            var products  = new List<AN_Product>();
                            foreach (var sku in skus)
                            {
                                m_LoadedSkus.Add(sku);
                                products.Add(new AN_Product(sku));
                            }
                            Inventory.SetProducts(products);
                        }
                        
                        skusList = new List<string>();
                        foreach (var product in AN_Settings.Instance.InAppProducts)
                        {
                            if (product.Type == AN_BillingClient.SkuType.subs)
                            {
                                skusList.Add(product.Sku);
                            }
                        }
                    
                        paramsBuilder = AN_SkuDetailsParams.NewBuilder();
                        paramsBuilder.SetType(AN_BillingClient.SkuType.subs);
                        paramsBuilder.SetSkusList(skusList);
                        m_Client.QuerySkuDetailsAsync(paramsBuilder.Build(), (subsSkus) =>
                        {
                            if (subsSkus != null)
                            {
                                var products  = new List<AN_Product>();
                                foreach (var sku in subsSkus)
                                {
                                    m_LoadedSkus.Add(sku);
                                    products.Add(new AN_Product(sku));
                                }
                                Inventory.SetProducts(products);
                            }

                            
                            GetPurchases((invResult) => {
                                if(invResult.IsSucceeded) {
                                    SaveConnectionResult(connectionResult);
                                } else {
                                    connectionResult.SetError(invResult.Error);
                                }
                                s_OnConnect.Invoke(connectionResult);
                                s_OnConnect = delegate { };
                            });
                            
                            
                        });

                    });
                }
                else
                {
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

            var inAppsResult =  m_Client.API.QueryPurchases(AN_BillingClient.SkuType.inapp);
            if (inAppsResult.IsFailed)
            {
                s_GetPurchasesRequestCallback.Invoke(inAppsResult);
                s_GetPurchasesRequestCallback = null;
                return;
            }

            var purchases = new List<AN_Purchase>();
            foreach (var purchase in inAppsResult.Purchases)
            {
                purchases.Add(new AN_Purchase(purchase));
            }
            
            var subsResult =  m_Client.API.QueryPurchases(AN_BillingClient.SkuType.subs);
            if (subsResult.IsFailed)
            {
                s_GetPurchasesRequestCallback.Invoke(subsResult);
                s_GetPurchasesRequestCallback = null;
                return;
            }
            
            purchases.Clear();
            foreach (var purchase in subsResult.Purchases)
            {
                purchases.Add(new AN_Purchase(purchase));
            }

            Inventory.SetPurchases(purchases);
            UpdateInventory(Inventory);
            s_GetPurchasesRequestCallback.Invoke(subsResult);
            s_GetPurchasesRequestCallback = null;

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

            
            foreach (var sku in m_LoadedSkus)
            {
                if (sku.Sku.Equals(product.ProductId))
                {
                    m_Client.Purchase(sku, developerPayload, callback);  
                }    
            }
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

            throw new NotImplementedException("Use new AN_BillingClient API.");
        }

        /// <summary>
        /// Consume purchase corresponding to the purchase token. 
        /// This will result in this item being removed from all 
        /// subsequent responses to <see cref="GetPurchases(Action{SA_Result})"/> and allow repurchase of items of the same sku.
        /// </summary>
        /// <param name="purchase">purchase to consume</param>
        /// <param name="callback">Consume request callback</param>
        public static void Consume(AN_Purchase purchase, Action<SA_Result> callback) {

            m_Client.Consume(purchase.Token, result =>
            {
                if (result.IsSucceeded)
                {
                    foreach (var invPurchase in Inventory.Purchases) {
                        if (invPurchase.ProductId.Equals(purchase.ProductId)) {
                            Inventory.Purchases.Remove(invPurchase);
                            break;
                        }
                    }
                }
                
                callback.Invoke((SA_Result)result);
            } );
            
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
                    var products = new List<AN_Product>();
                    foreach (var sku in AN_Settings.Instance.InAppProducts)
                    {
                        var product = new AN_Product(sku);
                        products.Add(product);
                    }
                    s_Inventory.SetProducts(products);
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
        
    }

}
