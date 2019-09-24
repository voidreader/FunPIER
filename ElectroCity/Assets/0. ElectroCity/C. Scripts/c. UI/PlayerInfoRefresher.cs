using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum PlayerDataEnum {
    Level,
    Exp,
    Coin,
    Gem,
    DamageLevel,
    DiscountLevel
}


public class PlayerInfoRefresher : MonoBehaviour
{

    public PlayerDataEnum identifier;
    public Text textValue;

    public GameObject ObjectParent = null;


    private void Awake() {
        PIER.OnRefreshPlayerInfo += UpdateData;    
    }

    void UpdateData() {
        switch(identifier) {
            case PlayerDataEnum.DamageLevel:

                if (PIER.main.DamageLevel <= 1 && ObjectParent)
                    ObjectParent.SetActive(false);
                else
                    ObjectParent.SetActive(true);

                textValue.text = PIER.GetDamageLevelFactor(PIER.main.DamageLevel);
                break;
            case PlayerDataEnum.DiscountLevel:
                if (PIER.main.DiscountLevel <= 1 && ObjectParent)
                    ObjectParent.SetActive(false);
                else
                    ObjectParent.SetActive(true);

                textValue.text = PIER.GetDiscountLevelFactor(PIER.main.DiscountLevel);
                break;
            case PlayerDataEnum.Level:
                textValue.text = string.Empty;
                break;
            case PlayerDataEnum.Exp:
                textValue.text = string.Empty;
                break;
            case PlayerDataEnum.Gem:
                textValue.text = string.Empty;
                break;
            case PlayerDataEnum.Coin:
                textValue.text = string.Empty;
                break;
        }
    }
}
