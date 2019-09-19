using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class PassiveColumn : MonoBehaviour {
    public Image imageBG, imageIcon;
    public Text textTitle, textPrice, textLevel, textInfo;
    public GameObject Cover;

    PassiveDataRow row;

    [SerializeField] int _level;
    [SerializeField] int _factor;
    [SerializeField] long _price;
    
    
    public void InitPassiveColumn(PassiveDataRow r) {
        this.gameObject.SetActive(true);

        row = r;

        _level = r._level;
        _factor = r._factor;
        _price = long.Parse(r._price);

        if(row._rid.Contains("DAMAGE")) {
            SetDPS();
        }
        else {
            SetDiscount();
        }


        textLevel.text = "LEVEL " + _level.ToString();



    }

    void SetDPS() {
        imageIcon.sprite = Stock.main.SpriteDPSIcon;
        imageIcon.SetNativeSize();
        textTitle.text = "DPS";

        textInfo.text = "+" + _factor + "%\ndamage per sec";
    }

    void SetDiscount() {
        imageIcon.sprite = Stock.main.SpriteDiscountIcon;
        textTitle.text = "DISCOUNT";

        textInfo.text = _factor + "% OFF\nOn all units";

    }

}
