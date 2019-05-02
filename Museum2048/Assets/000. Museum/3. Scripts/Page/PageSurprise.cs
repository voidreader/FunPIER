using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageSurprise : UILayer {

    public UILabel _lblPrice;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parent"></param>
    /// <param name="pOpen"></param>
    /// <param name="pClose"></param>
    /// <returns></returns>
    public override UILayer Init(UILayerEnum type, Transform parent, Action pOpen, Action pClose) {

        _lblPrice.text = PageShop.GetLocalizedPrice("onechance_m2048");
        ES2.Save<int>(System.DateTime.Now.DayOfYear, ConstBox.KeySavedSurprisePack); // 열린 day of year 저장 

        return base.Init(type, parent, pOpen, pClose);
    }


    /// <summary>
    /// 구매 
    /// </summary>
    public void OnClickPurchase() {
        IAPControl.main.Purchase("onechance_m2048");
    }

    public void CloseCheck() {

    }
}
