using System.Collections.Generic;
using SA.Android.Utilities;
using SA.Android.Vending.BillingClient;
using SA.Foundation.Network.Web;
using SA.Foundation.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace SA.Android.Samples
{
    public class AN_VendingSample : MonoBehaviour, AN_iSkuDetailsResponseListener, AN_iConsumeResponseListener
    {
        private static AN_BillingClientSample m_BillingClientSample = null;
     
        [SerializeField] private Button m_ConnectButton = null;
        [SerializeField] private Button m_QuerySkuDetailsButton = null;
        [SerializeField] private AN_ProductView m_ProductView = null;
        
        private List<AN_Purchase> m_Purchases = new List<AN_Purchase>();
        

        private void Start()
        {
            if (m_BillingClientSample == null)
            {
                m_BillingClientSample = new AN_BillingClientSample();
            }            
            
            m_ConnectButton.onClick.AddListener(() =>
            {
                m_BillingClientSample.Connect();
            });
            
            m_QuerySkuDetailsButton.onClick.AddListener(() => { BuildProductsUI(); });
            
            //In this example we will rebuild whole UI when any product purchase state is changed.
            //But you can implement more advanced login and skip QuerySkuDetailsAsync step if you already done it earlier.
            m_BillingClientSample.OnStoreStateUpdated += BuildProductsUI;
        }

        private void Update()
        {
            m_ConnectButton.interactable = !m_BillingClientSample.IsConnected;
            m_QuerySkuDetailsButton.interactable = m_BillingClientSample.IsConnected;
        }

        private void OnDestroy()
        {
            m_BillingClientSample.OnStoreStateUpdated -= BuildProductsUI;
        }

        private void BuildProductsUI()
        {
            
            //Clean up current UI
            m_Purchases.Clear();
            m_ProductView.transform.parent.Clear();
            
            //Let's get all the purchases  first
            var purchasesResult = m_BillingClientSample.Client.QueryPurchases(AN_BillingClient.SkuType.inapp);
            if (purchasesResult.IsSucceeded)
            {
                m_Purchases.AddRange(purchasesResult.Purchases);
            }
                
            //In case you also have subs products you can also Query for it as well.
            //In this example we only have inapp products types.
            var paramsBuilder = AN_SkuDetailsParams.NewBuilder();
            paramsBuilder.SetType(AN_BillingClient.SkuType.inapp);
            
            var skusList = new List<string>();
            skusList.Add("android.test.purchased");
            skusList.Add("android.test.canceled");
            skusList.Add("android.test.item_unavailable");
            skusList.Add("androidnative.test.product.1");
            skusList.Add("androidnative.product.test.2");
            paramsBuilder.SetSkusList(skusList);
                
            m_BillingClientSample.Client.QuerySkuDetailsAsync(paramsBuilder.Build(), this);
        }
        
        //--------------------------------------
        // AN_iSkuDetailsResponseListener
        //--------------------------------------
        
        public void OnSkuDetailsResponse(SA_Result billingResult, List<AN_SkuDetails> skuDetailsList)
        {
            
            AN_Logger.Log("OnSkuDetailsResponse IsSucceeded: " + billingResult.IsSucceeded);
            if (billingResult.IsSucceeded)
            {
                AN_Logger.Log("Loaded " + skuDetailsList.Count + " products");
                foreach (var skuDetails in skuDetailsList)
                {
                    AN_Logger.Log("--------------------->");
                    PrintSku(skuDetails);
                    
                    var productView = Instantiate(m_ProductView.gameObject, m_ProductView.transform.parent).GetComponent<AN_ProductView>();
                    productView.transform.localScale = m_ProductView.transform.localScale;
                    productView.gameObject.SetActive(true);
                    productView.ProductTitle.text = skuDetails.Title;
                    if (!string.IsNullOrEmpty(skuDetails.IconUrl))
                    {
                        SA_CachedRequestsFactory.GetTexture2D(skuDetails.IconUrl, texture =>
                        {
                            productView.ProductImage.texture = texture;
                        });
                    }
                    
                    var productPurchasedInfo = IsProductPurchased(skuDetails);

                    if (productPurchasedInfo != null)
                    {
                        productView.BuyButton.GetComponentInChildren<Text>().text = "Consume";
                        productView.BuyButton.onClick.AddListener(() =>
                        {
                            var paramsBuilder = AN_ConsumeParams.NewBuilder();
                            paramsBuilder.SetPurchaseToken(productPurchasedInfo.PurchaseToken);
                        
                            m_BillingClientSample.Client.ConsumeAsync(paramsBuilder.Build(), this);
                        });  
                    }
                    else
                    {
                        productView.BuyButton.GetComponentInChildren<Text>().text = "Buy";
                        productView.BuyButton.onClick.AddListener(() =>
                        {
                            var paramsBuilder = AN_BillingFlowParams.NewBuilder();
                            paramsBuilder.SetSkuDetails(skuDetails);
                        
                            m_BillingClientSample.Client.LaunchBillingFlow(paramsBuilder.Build());
                        });  
                    } 
                }
            }
        }
       
            
        //--------------------------------------
        //  AN_iConsumeResponseListener
        //--------------------------------------
        
        public void OnConsumeResponse(SA_iResult billingResult, string purchaseToken)
        {
            if (billingResult.IsSucceeded)
            {
                //Let's updated our UI again
                BuildProductsUI();
            }
            else
            {
                AN_BillingClientSample.ShowErrorMessage(billingResult.Error);
            }
        }
        
        private AN_Purchase IsProductPurchased(AN_SkuDetails skuDetails)
        {
            foreach (var purchase in m_Purchases)
            {
                if (purchase.Sku.Equals(skuDetails.Sku))
                {
                    return purchase;
                }
            }

            return null;
        }

        private void PrintSku(AN_SkuDetails skuDetails) 
        {
            AN_Logger.Log("skuDetails.Sku: " + skuDetails.Sku);
            AN_Logger.Log("skuDetails.Price: " + skuDetails.Price);
            AN_Logger.Log("skuDetails.Title: " + skuDetails.Title);
            AN_Logger.Log("skuDetails.Description: " + skuDetails.Description);
            AN_Logger.Log("skuDetails.FreeTrialPeriod: " + skuDetails.FreeTrialPeriod);
            AN_Logger.Log("skuDetails.IconUrl: " + skuDetails.IconUrl);
            AN_Logger.Log("skuDetails.IntroductoryPrice: " + skuDetails.IntroductoryPrice);
            AN_Logger.Log("skuDetails.IntroductoryPriceAmountMicros: " + skuDetails.IntroductoryPriceAmountMicros);
            AN_Logger.Log("skuDetails.IntroductoryPriceCycles: " + skuDetails.IntroductoryPriceCycles);
            AN_Logger.Log("skuDetails.IntroductoryPricePeriod: " + skuDetails.IntroductoryPricePeriod);
            AN_Logger.Log("skuDetails.OriginalPrice: " + skuDetails.OriginalPrice);
            AN_Logger.Log("skuDetails.OriginalPriceAmountMicros: " + skuDetails.OriginalPriceAmountMicros);
            AN_Logger.Log("skuDetails.PriceAmountMicros: " + skuDetails.PriceAmountMicros);
            AN_Logger.Log("skuDetails.PriceCurrencyCode: " + skuDetails.PriceCurrencyCode);
            AN_Logger.Log("skuDetails.SubscriptionPeriod: " + skuDetails.SubscriptionPeriod);
            AN_Logger.Log("skuDetails.IsRewarded: " + skuDetails.IsRewarded);
            AN_Logger.Log("skuDetails.OriginalJson: " + skuDetails.OriginalJson);
        }
    }
}
