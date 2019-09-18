using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPControl : MonoBehaviour, IStoreListener {

    // ※ 모듈 초기화가 즉시 되는게 아니다. 
    public static IAPControl main = null;

    public static bool IsInitialized = false; // 정상적으로 초기화 되었는지에 대한 여부 
    public static bool IsModuleLoaded = false; // 모듈 초기화 여부 

    public bool IsSubscribe = false; // 구독 여부 체크 
    const string KeySubscribe = "KeySubscribe";
    const string subscriptionID = "hm_weekly_subs";

    IStoreController controller = null;
    IExtensionProvider extensions = null; // 여러 플랫폼을 위한 확장 처리를 제공
    private IAppleExtensions m_AppleExtensions; 


    public IStoreController Controller { get => controller; set => controller = value; } // 구매 과정을 제어하는 함수 제공 
    

    void Awake() {
        main = this;
        DontDestroyOnLoad(this);

        IsModuleLoaded = false;
        IsInitialized = false;
        
    }

    // Start is called before the first frame update
    void Start() {

        // yield return new WaitForSeconds(0.2f);

        LoadSubscribeData();  // 구독 정보 조회 (데이터) 
        InitBilling();
    }

    /// <summary>
    /// 
    /// </summary>
    void LoadSubscribeData() {

        // 네트워크 상의 이유로 초기화가 잘 되지 않는 경우도 있다. 
        // 그래서 데이터로 미리 저장 
        if (PlayerPrefs.GetInt(KeySubscribe, 0) == 0)
            IsSubscribe = false;
        else
            IsSubscribe = true;
    }

    void SaveSubscibeData(bool flag) {
        IsSubscribe = flag;

        if (flag)
            PlayerPrefs.SetInt(KeySubscribe, 1);
        else
            PlayerPrefs.SetInt(KeySubscribe, 0);

        PlayerPrefs.Save();
    }


    /// <summary>
    /// 결제 모듈 초기화
    /// </summary>
    public void InitBilling() {


        // 인터넷 되지 않을때. 
        if(Application.internetReachability == NetworkReachability.NotReachable) {
            IsInitialized = false;
            IsModuleLoaded = true;
            // 모듈은 로드되었지만 초기화는 되지 않음

            return;
        }


        if (IsInitialized)
            return;

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());


        // 상품 등록
        builder.AddProduct("hm_weekly_subs", ProductType.Subscription, new IDs {
            { "hm_weekly_subs", GooglePlay.Name },
            { "hm_weekly_subs", AppleAppStore.Name }
        });


        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        Debug.Log(">>> Unity IAP OnInitialized <<< ");

        // PIER.IsSpecialist = false;
        // PIER.IsSpecialist = true;

        this.Controller = controller;
        this.extensions = extensions;


        // 
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();


        // 구독상품 관련 처리 
        Debug.Log("Available Subscription items:");
        foreach(var item in controller.products.all) {

            // 구독상품만 체크 
            if(item.definition.id != "subscriptionID") {
                continue;
            }

            if(item.availableToPurchase) {
                Debug.Log(string.Join(" - ",
                        new[]
                        {
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString,
                        item.transactionID,
                        item.receipt}));

                


                if (item.hasReceipt && item.definition.type == ProductType.Subscription && checkIfProductIsAvailableForSubscriptionManager(item.receipt)) {
                    // 구독 처리
                    string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                    SubscriptionManager p = new SubscriptionManager(item, intro_json);
                    SubscriptionInfo info = p.getSubscriptionInfo();

                    Debug.Log("product id is: " + info.getProductId());
                    Debug.Log("purchase date is: " + info.getPurchaseDate());
                    Debug.Log("subscription next billing date is: " + info.getExpireDate());
                    Debug.Log("is subscribed? " + info.isSubscribed().ToString());
                    Debug.Log("is expired? " + info.isExpired().ToString());
                    Debug.Log("is cancelled? " + info.isCancelled());
                    Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                    Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                    Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                    Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                    Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                    Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                    Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());

                    
                    // 구독기록 있을 경우. 
                    if(info.isSubscribed() == Result.True) {

                        // 구독을 취소해도 기간이 끝날때까지는 상품을 유지해야 한다. 
                        // 만료 체크
                        if (info.isExpired() == Result.False) {
                            // PIER.main.SetSpecialist(true);
                            Debug.Log(">>> You are already Specialist <<<");
                            SaveSubscibeData(true);
                        }
                        else {
                            Debug.Log(">>> Specialist period is expired <<<");
                            SaveSubscibeData(false);
                        }

                    }
                    else {
                        SaveSubscibeData(false);
                    }


                } // end of.. !
                else {

                    if(!checkIfProductIsAvailableForSubscriptionManager(item.receipt))
                        Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                    else if(item.definition.type != ProductType.Subscription)
                        Debug.Log("the product is not a subscription product");
                    else if(item.receipt == null)
                        Debug.Log("the product should have a valid receipt");


                    SaveSubscibeData(false);
                }
                // end



            } // end of item.availableToPurchase 
        } // end of foreach

        /*
        if (!PIER.IsSpecialist)
            PIER.main.SetSpecialist(false);
        */


        
        Debug.Log("IAP init completed");
        IsInitialized = true;
        IsModuleLoaded = true;
    }


    public void OnInitializeFailed(InitializationFailureReason error) {
        Debug.Log(">>> Unity IAP OnInitializeFailed :: " + error.ToString());
        SaveSubscibeData(false);
        IsInitialized = false; 
        IsModuleLoaded = true;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="productID"></param>
    public void Purchase(string productID) {

        // 구매.. 
        if(Application.internetReachability == NetworkReachability.NotReachable) {

            return;
        }


        
        Product p = Controller.products.WithID(productID);

        if(p != null && p.availableToPurchase) {
            Debug.Log(">> Purchase Start...!!");
            controller.InitiatePurchase(p); 
        }
        else {
            Debug.Log(">> Purchase Start... Failed!!");
        }
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p) {
        Debug.Log(">> OnPurchaseFailed :: " + p.ToString());
    }


    /// <summary>
    /// 구매 진행처리 
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {

        Debug.Log(">> Called ProcessPurchase  :: " + e.purchasedProduct.definition.id);

        if(e.purchasedProduct.definition.type == ProductType.Subscription) {

            Debug.Log("Is Specialist!!!!!! by new purchase !!");
            SaveSubscibeData(true);

            if (PIER.main != null) {
                PIER.main.SetSpecialist(true);
                GameManager.main.RefreshPlayerWeapon(); // 구매시에는 무기도 변경해준다. 
            }
            

            if(SubscribeView.main.gameObject.activeSelf) {
                SubscribeView.main.CloseView();
            }

        }

        SingularSDK.InAppPurchase(e.purchasedProduct, null);


        // todo
        return PurchaseProcessingResult.Complete;
    }


    #region 구독 관련

    private bool checkIfProductIsAvailableForSubscriptionManager(string receipt) {

        if (Application.isEditor)
            return true;

        if (string.IsNullOrEmpty(receipt)) {
            Debug.Log(">> Check if product..... receipt is null or empty");
            return false;
        }

        var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
        if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload")) {
            Debug.Log("The product receipt does not contain enough information");
            return false;
        }
        var store = (string)receipt_wrapper["Store"];
        var payload = (string)receipt_wrapper["Payload"];

        if (payload != null) {
            switch (store) {
                case GooglePlay.Name: {
                        var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                        if (!payload_wrapper.ContainsKey("json")) {
                            Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                            return false;
                        }
                        var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                        if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload")) {
                            Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                            return false;
                        }
                        var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                        var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                        if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial")) {
                            Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                            return false;
                        }
                        return true;
                    }
                case AppleAppStore.Name:
                case AmazonApps.Name:
                case MacAppStore.Name: {
                        return true;
                    }
                default: {
                        return false;
                    }
            }
        }
        return false;
    }

    #endregion

    /// <summary>
    /// 구독상품 가격정보 
    /// </summary>
    /// <returns></returns>
    public static string GetSubscriptionProductPrice() {

        // 초기화 되어 있지 않으면 empty 
        if(!IsInitialized) {
            return string.Empty;
        }

        if (main == null)
            return string.Empty;

        foreach(var item in main.controller.products.all) {
            if(item.definition.id == subscriptionID) {
                return item.metadata.localizedPriceString;
            }
        }

        return string.Empty;
    }


    public void RestorePurchase() {
        if (!IsInitialized)
            return;

        if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
            Debug.Log("Purchase Restore Start...");
            m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
            m_AppleExtensions.RestoreTransactions(
                callback: result => Debug.Log(message: $"Restore result - {result}"));
                
        }
    }

    public bool HadPurchased(string productID) {
        if (!IsInitialized)
            return false;

        Product p = Controller.products.WithID(productID);
        if (p != null)
            return p.hasReceipt;

        return false;

    }

}
