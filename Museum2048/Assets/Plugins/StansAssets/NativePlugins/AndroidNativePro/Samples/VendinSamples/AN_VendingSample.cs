using System.Collections.Generic;
using SA.Android.Vending.BillingClient;
using SA.Foundation.Templates;
using StansAssets.Foundation;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SA.Android.Samples
{
    public class AN_VendingSample : MonoBehaviour, AN_iSkuDetailsResponseListener, AN_iConsumeResponseListener
    {
        static AN_BillingClientSample m_BillingClientSample = null;

        [SerializeField]
        Button m_ConnectButton = null;
        [SerializeField]
        Button m_QuerySkuDetailsButton = null;
        [SerializeField]
        AN_ProductView m_ProductView = null;

        readonly List<AN_Purchase> m_Purchases = new List<AN_Purchase>();

        void Start()
        {
            if (m_BillingClientSample == null) m_BillingClientSample = new AN_BillingClientSample();

            m_ConnectButton.onClick.AddListener(() =>
            {
                m_BillingClientSample.Connect();
            });

            m_QuerySkuDetailsButton.onClick.AddListener(() => { BuildProductsUI(); });

            //In this example we will rebuild whole UI when any product purchase state is changed.
            //But you can implement more advanced login and skip QuerySkuDetailsAsync step if you already done it earlier.
            m_BillingClientSample.OnStoreStateUpdated += BuildProductsUI;
        }

        void Update()
        {
            m_ConnectButton.interactable = !m_BillingClientSample.IsConnected;
            m_QuerySkuDetailsButton.interactable = m_BillingClientSample.IsConnected;
        }

        void OnDestroy()
        {
            m_BillingClientSample.OnStoreStateUpdated -= BuildProductsUI;
        }

        void BuildProductsUI()
        {
            //Clean up current UI
            m_Purchases.Clear();
            m_ProductView.transform.parent.Clear();

            //Let's get all the purchases  first
            var purchasesResult = m_BillingClientSample.Client.QueryPurchases(AN_BillingClient.SkuType.inapp);
            if (purchasesResult.IsSucceeded) m_Purchases.AddRange(purchasesResult.Purchases);

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
            Debug.Log("OnSkuDetailsResponse IsSucceeded: " + billingResult.IsSucceeded);
            if (billingResult.IsSucceeded)
            {
                Debug.Log("Loaded " + skuDetailsList.Count + " products");
                foreach (var skuDetails in skuDetailsList)
                {
                    Debug.Log("--------------------->");
                    PrintSku(skuDetails);

                    var productView = Instantiate(m_ProductView.gameObject, m_ProductView.transform.parent).GetComponent<AN_ProductView>();
                    productView.transform.localScale = m_ProductView.transform.localScale;
                    productView.gameObject.SetActive(true);
                    productView.ProductTitle.text = skuDetails.Title;
                    if (!string.IsNullOrEmpty(skuDetails.IconUrl))
                        CachedWebRequest.GetTexture2D(skuDetails.IconUrl, texture =>
                        {
                            productView.ProductImage.texture = texture;
                        });

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

                //Let's updated our UI again
                BuildProductsUI();
            else
                AN_BillingClientSample.ShowErrorMessage(billingResult.Error);
        }

        AN_Purchase IsProductPurchased(AN_SkuDetails skuDetails)
        {
            foreach (var purchase in m_Purchases)
                if (purchase.Sku.Equals(skuDetails.Sku))
                    return purchase;

            return null;
        }

        void PrintSku(AN_SkuDetails skuDetails)
        {
            Debug.Log("skuDetails.Sku: " + skuDetails.Sku);
            Debug.Log("skuDetails.Price: " + skuDetails.Price);
            Debug.Log("skuDetails.Title: " + skuDetails.Title);
            Debug.Log("skuDetails.Description: " + skuDetails.Description);
            Debug.Log("skuDetails.FreeTrialPeriod: " + skuDetails.FreeTrialPeriod);
            Debug.Log("skuDetails.IconUrl: " + skuDetails.IconUrl);
            Debug.Log("skuDetails.IntroductoryPrice: " + skuDetails.IntroductoryPrice);
            Debug.Log("skuDetails.IntroductoryPriceAmountMicros: " + skuDetails.IntroductoryPriceAmountMicros);
            Debug.Log("skuDetails.IntroductoryPriceCycles: " + skuDetails.IntroductoryPriceCycles);
            Debug.Log("skuDetails.IntroductoryPricePeriod: " + skuDetails.IntroductoryPricePeriod);
            Debug.Log("skuDetails.OriginalPrice: " + skuDetails.OriginalPrice);
            Debug.Log("skuDetails.OriginalPriceAmountMicros: " + skuDetails.OriginalPriceAmountMicros);
            Debug.Log("skuDetails.PriceAmountMicros: " + skuDetails.PriceAmountMicros);
            Debug.Log("skuDetails.PriceCurrencyCode: " + skuDetails.PriceCurrencyCode);
            Debug.Log("skuDetails.SubscriptionPeriod: " + skuDetails.SubscriptionPeriod);
            Debug.Log("skuDetails.IsRewarded: " + skuDetails.IsRewarded);
            Debug.Log("skuDetails.OriginalJson: " + skuDetails.OriginalJson);
        }
    }
}
