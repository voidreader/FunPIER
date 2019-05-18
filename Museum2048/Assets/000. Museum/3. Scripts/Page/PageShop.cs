using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class PageShop : UILayer {


    public UILabel _lblBack1, _lblBack2, _lblUpgrade1, _lblUpgrade2;
    public UILabel _lblClean1, _lblClean2, _lblNoAds, _lblPackage;

    public override UILayer Init(UILayerEnum type, Transform parent, Action pOpen, Action pClose) {

        // 상점 가격 초기화 
        InitStorePrices();

        return base.Init(type, parent, pOpen, pClose);
    }


    /// <summary>
    /// 가격정보 세팅 
    /// </summary>
    void InitStorePrices() {

        if (!IAPControl.IsInitialized) {
            Debug.Log("IAPControl is not init yet.");
        }


        _lblBack1.text = GetLocalizedPrice("back_m2048");
        _lblBack2.text = GetLocalizedPrice("back2_m2048");

        _lblUpgrade1.text = GetLocalizedPrice("upgrader_m2048");
        _lblUpgrade2.text = GetLocalizedPrice("upgrader2_m2048");

        _lblClean1.text = GetLocalizedPrice("cleaner_m2048");
        _lblClean2.text = GetLocalizedPrice("cleaner2_m2048");

        _lblNoAds.text = GetLocalizedPrice(IAPControl.noadsID);
        _lblPackage.text = GetLocalizedPrice("pack1_m2048");

    }

    /// <summary>
    /// 
    /// </summary>
    public static string GetLocalizedPrice(string p) { 

        

        foreach (Product product in IAPControl.main.Controller.products.all) {

            if(product.definition.id == p) {
                return product.metadata.localizedPriceString;
            }

            /*
            Debug.Log(product.metadata.localizedTitle);
            Debug.Log(product.metadata.localizedDescription);
            Debug.Log(product.metadata.localizedPriceString);
            */
        }

        return string.Empty;
    }

    public void BuyBack1() {
        IAPControl.main.Purchase("back_m2048");
    }

    public void BuyBack2() {
        IAPControl.main.Purchase("back2_m2048");
    }

    public void BuyUpgrader1() {
        IAPControl.main.Purchase("upgrader_m2048");
    }
    public void BuyUpgrader2() {
        IAPControl.main.Purchase("upgrader2_m2048");
    }

    public void BuyCleaner1() {
        IAPControl.main.Purchase("cleaner_m2048");
    }
    public void BuyCleaner2() {
        IAPControl.main.Purchase("cleaner2_m2048");
    }

    public void BuyNoAds() {
        IAPControl.main.Purchase(IAPControl.noadsID);
    }

    public void BuyPackage() {
        IAPControl.main.Purchase("pack1_m2048");
    }
}
