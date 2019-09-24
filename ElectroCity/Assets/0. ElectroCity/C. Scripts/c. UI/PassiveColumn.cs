using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class PassiveColumn : MonoBehaviour {
    public Action OnUpgradeCallback = delegate { };

    public Image imageBG, imageIcon;
    public Text textTitle, textPrice, textLevel, textInfo;
    public GameObject Cover;
    public GameObject btnUpgrade;

    PassiveDataRow row;

    [SerializeField] int _level;
    [SerializeField] int _factor;
    [SerializeField] long _price;
    [SerializeField] string _debugPrice = string.Empty;


    public void InitPassiveColumn(PassiveDataRow r, Action p) {
        this.gameObject.SetActive(true);
        SetLock(false);

        row = r;
        OnUpgradeCallback = p;

        _level = r._level;
        _factor = r._factor;
        _price = long.Parse(r._price);

        if (row._rid.Contains("DAMAGE")) {
            SetDPS();
        }
        else {
            SetDiscount();
        }


        textLevel.text = "LEVEL " + _level.ToString(); // 레벨 
        // textPrice.text = string.Format("{0:#,###}", _price); // 가격 
        textPrice.text = PIER.GetBigNumber(_price);
        _debugPrice = PIER.GetBigNumber(_price);

        

    }

    void SetDPS() {
        imageIcon.sprite = Stock.main.SpriteDPSIcon;
        imageIcon.SetNativeSize();
        textTitle.text = "DPS";

        textInfo.text = "+" + _factor + "%\ndamage per sec";

        if (_level != PIER.main.DamageLevel)
            SetLock(true);
    }

    void SetDiscount() {
        imageIcon.sprite = Stock.main.SpriteDiscountIcon;
        textTitle.text = "DISCOUNT";

        textInfo.text = _factor + "% OFF\nOn all units";

        if (_level != PIER.main.DiscountLevel)
            SetLock(true);

    }

    public void SetLock(bool flag) {

        Cover.SetActive(flag);
        btnUpgrade.SetActive(!flag);


    }

    public void OnClickUpgrade() {
        // 코인 체크 


        if (row._rid.Contains("DAMAGE")) {
            PIER.main.SetDamageLevel(_level);
        }
        else {
            PIER.main.SetDiscountLevel(_level);
        }

        
        // Refresh. 자연스럽게 변경할 방법..? 
        OnUpgradeCallback();
    }

    

}
