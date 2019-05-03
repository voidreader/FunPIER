using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
//using SA.Android.Vending.Billing;
//using SA.Foundation.Templates;

public class IAPControl : MonoBehaviour, IStoreListener {


    public static IAPControl main = null;
    public static bool IsInitialized = false;

    IStoreController controller;
    IExtensionProvider extensions;

    public IStoreController Controller { get => controller; set => controller = value; }

    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitBilling();
    }


    void InitBilling() {
        if (IsInitialized)
            return;

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct("noads_m2048", ProductType.Consumable, new IDs {
            { "noads_m2048", GooglePlay.Name },
            { "noads_m2048", AppleAppStore.Name }
        });

        builder.AddProduct("back_m2048", ProductType.Consumable, new IDs {
            { "back_m2048", GooglePlay.Name },
            { "back_m2048", AppleAppStore.Name }
        });

        builder.AddProduct("back2_m2048", ProductType.Consumable, new IDs {
            { "back2_m2048", GooglePlay.Name },
            { "back2_m2048", AppleAppStore.Name }
        });

        builder.AddProduct("upgrader_m2048", ProductType.Consumable, new IDs {
            { "upgrader_m2048", GooglePlay.Name },
            { "upgrader_m2048", AppleAppStore.Name }
        });

        builder.AddProduct("upgrader2_m2048", ProductType.Consumable, new IDs {
            { "upgrader2_m2048", GooglePlay.Name },
            { "upgrader2_m2048", AppleAppStore.Name }
        });

        builder.AddProduct("cleaner_m2048", ProductType.Consumable, new IDs {
            { "cleaner_m2048", GooglePlay.Name },
            { "cleaner_m2048", AppleAppStore.Name }
        });

        builder.AddProduct("cleaner2_m2048", ProductType.Consumable, new IDs {
            { "cleaner2_m2048", GooglePlay.Name },
            { "cleaner2_m2048", AppleAppStore.Name }
        });

        builder.AddProduct("onechance_m2048", ProductType.Consumable, new IDs {
            { "onechance_m2048", GooglePlay.Name },
            { "onechance_m2048", AppleAppStore.Name }
        });

        builder.AddProduct("pack1_m2048", ProductType.Consumable, new IDs {
            { "pack1_m2048", GooglePlay.Name },
            { "pack1_m2048", AppleAppStore.Name }
        });


        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitializeFailed(InitializationFailureReason error) {
        Debug.Log("Unity IAP OnInitializeFailed :: " + error.ToString());
    }

    /// <summary>
    /// 구매 진행처리 
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {

        bool validPurchase = true;
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

        LobbyManager.isAnimation = false;

        try {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            var result = validator.Validate(e.purchasedProduct.receipt);
            // For informational purposes, we list the receipt(s)
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result) {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        } catch(IAPSecurityException) {
            Debug.Log("Invalid receipt, not unlocking content");
            validPurchase = false;
        }
       
        if(validPurchase) {
            UnLockProduct(e.purchasedProduct.definition.id);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p) {
        Debug.Log(">> OnPurchaseFailed :: " + p.ToString());

        LobbyManager.isAnimation = false;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        Debug.Log("Unity IAP OnInitialized");
        this.Controller = controller;
        this.extensions = extensions;


        /*
        foreach (var product in controller.products.all) {
            Debug.Log(product.metadata.localizedTitle);
            Debug.Log(product.metadata.localizedDescription);
            Debug.Log(product.metadata.localizedPriceString);
        }
        */

        IsInitialized = true;
    }


    /// <summary>
    /// 구매 처리 시작
    /// </summary>
    /// <param name="productid"></param>
    public void Purchase(string productid) {

        if (LobbyManager.isAnimation)
            return;

        LobbyManager.isAnimation = true;

        controller.InitiatePurchase(productid);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productid"></param>
    public void UnLockProduct(string productid) {

        Debug.Log(">> UnLockProduct :: " + productid);

        switch(productid) {

            case "back_m2048":
                PierSystem.main.itemBack += 10;
                break;
            case "back2_m2048":
                PierSystem.main.itemBack += 25;
                break;

            case "upgrader_m2048":
                PierSystem.main.itemUpgrade += 5;
                break;
            case "upgrader2_m2048":
                PierSystem.main.itemUpgrade += 15;
                break;

            case "cleaner_m2048":
                PierSystem.main.itemCleaner += 5;
                break;
            case "cleaner2_m2048":
                PierSystem.main.itemCleaner += 15;
                break;

            case "pack1_m2048":
                PierSystem.main.itemBack += 10;
                PierSystem.main.itemUpgrade += 10;
                PierSystem.main.itemCleaner += 10;
                break;


            case "noads_m2048":
                PierSystem.main.NoAds = 1;
                AdsControl.main.NoAdsCheck();
                // AdsControl.main.set
                break;


        }

        PierSystem.main.SaveProfile();
        ItemCounter.RefreshItems();
    }


}
